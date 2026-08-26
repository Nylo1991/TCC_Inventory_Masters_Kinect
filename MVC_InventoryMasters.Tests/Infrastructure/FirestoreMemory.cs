using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using MVC_InventoryMasters.Services;
using ProtoTimestamp = Google.Protobuf.WellKnownTypes.Timestamp;
using Precondition = Google.Cloud.Firestore.V1.Precondition;
using WriteResult = Google.Cloud.Firestore.V1.WriteResult;

namespace MVC_InventoryMasters.Tests.Infrastructure;

/// <summary>
/// Transporte Firestore em memória. Não cria canal gRPC, não lê credenciais e não usa rede.
/// Implementa somente as operações utilizadas pelos repositórios; demais operações falham.
/// Não substitui testes de integração do Firestore (índices, regras, concorrência).
/// </summary>
internal sealed class FirestoreMemory : CallInvoker
{
    private readonly Dictionary<string, Document> documents = new();
    private static ProtoTimestamp Now() => ProtoTimestamp.FromDateTime(DateTime.UtcNow);
    public Exception? Failure { get; set; }
    public int Calls { get; private set; }
    public FirestoreDb Db { get; }
    public FirebaseService Firebase { get; }
    public ContextoUsuarioService Context { get; }

    public FirestoreMemory(string empresa = "empresa-a")
    {
        var grpc = new Google.Cloud.Firestore.V1.Firestore.FirestoreClient(this);
        Db = FirestoreDb.Create("unit-tests-only", new FirestoreClientImpl(grpc, null, null));
        Firebase = new FirebaseService(Db);
        var http = new DefaultHttpContext();
        http.Request.Headers["X-Empresa-Id"] = empresa;
        Context = new ContextoUsuarioService(new HttpContextAccessor { HttpContext = http });
    }

    public Task Seed(string collection, string id, object value) =>
        Db.Collection(collection).Document(id).SetAsync(value);
    public async Task<T> Read<T>(string collection, string id) =>
        (await Db.Collection(collection).Document(id).GetSnapshotAsync()).ConvertTo<T>();

    private void Check()
    {
        Calls++;
        if (Failure != null) throw Failure;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options, TRequest request)
    {
        Check();
        if (request is not CommitRequest commit) throw new NotSupportedException(method.FullName);
        var response = new CommitResponse { CommitTime = Now() };
        foreach (var write in commit.Writes)
        {
            string name = write.Update?.Name ?? write.Delete;
            bool exists = documents.TryGetValue(name, out var old);
            if (write.CurrentDocument?.ConditionTypeCase == Precondition.ConditionTypeOneofCase.Exists
                && write.CurrentDocument.Exists != exists)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Documento ausente ou duplicado."));
            if (write.OperationCase == Write.OperationOneofCase.Delete)
                documents.Remove(name);
            else if (write.OperationCase == Write.OperationOneofCase.Update)
            {
                var doc = write.UpdateMask == null
                    ? write.Update!.Clone()
                    : old?.Clone() ?? new Document { Name = name };
                if (write.UpdateMask != null)
                    foreach (string field in write.UpdateMask.FieldPaths)
                    {
                        // Os modelos usam campos simples; caminhos aninhados não são suportados.
                        if (field.Contains('.') || field.Contains('`')) throw new NotSupportedException(field);
                        if (write.Update!.Fields.TryGetValue(field, out var value)) doc.Fields[field] = value.Clone();
                        else doc.Fields.Remove(field);
                    }
                if (write.UpdateTransforms.Count != 0) throw new NotSupportedException("Transforms");
                doc.CreateTime = old?.CreateTime ?? Now();
                doc.UpdateTime = Now();
                documents[name] = doc;
            }
            else throw new NotSupportedException(write.OperationCase.ToString());
            response.WriteResults.Add(new WriteResult { UpdateTime = Now() });
        }
        return new AsyncUnaryCall<TResponse>(Task.FromResult((TResponse)(object)response),
            Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options, TRequest request)
    {
        Check();
        IEnumerable<object> responses;
        if (request is BatchGetDocumentsRequest batch)
            responses = batch.Documents.Select(name => documents.TryGetValue(name, out var doc)
                ? new BatchGetDocumentsResponse { Found = doc.Clone(), ReadTime = Now() }
                : new BatchGetDocumentsResponse { Missing = name, ReadTime = Now() });
        else if (request is RunQueryRequest query)
        {
            var structured = query.StructuredQuery;
            if (structured.From.Count != 1 || structured.From[0].AllDescendants)
                throw new NotSupportedException("Collection group");
            string prefix = query.Parent + "/" + structured.From[0].CollectionId + "/";
            IEnumerable<Document> matches = documents.Values.Where(d =>
                d.Name.StartsWith(prefix, StringComparison.Ordinal) &&
                !d.Name.Substring(prefix.Length).Contains('/') && Matches(d, structured.Where));
            IOrderedEnumerable<Document>? ordered = null;
            foreach (var order in structured.OrderBy)
            {
                Func<Document, string> key = order.Field.FieldPath == "__name__"
                    ? d => d.Name
                    : d => d.Fields.TryGetValue(order.Field.FieldPath, out var v) ? SortKey(v) : "";
                bool descending = order.Direction == StructuredQuery.Types.Direction.Descending;
                ordered = ordered == null
                    ? (descending ? matches.OrderByDescending(key, StringComparer.Ordinal) : matches.OrderBy(key, StringComparer.Ordinal))
                    : (descending ? ordered.ThenByDescending(key, StringComparer.Ordinal) : ordered.ThenBy(key, StringComparer.Ordinal));
            }
            matches = ordered ?? matches;
            if (structured.Limit.HasValue) matches = matches.Take(structured.Limit.Value);
            responses = matches.Select(d => (object)new RunQueryResponse { Document = d.Clone(), ReadTime = Now() }).ToList();
            if (!responses.Any()) responses = new object[] { new RunQueryResponse { ReadTime = Now() } };
        }
        else throw new NotSupportedException(method.FullName);
        return new AsyncServerStreamingCall<TResponse>(new MemoryStream<TResponse>(responses.Cast<TResponse>()),
            Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });
    }

    private static string SortKey(Value value) => value.ValueTypeCase switch
    {
        Value.ValueTypeOneofCase.TimestampValue => value.TimestampValue.ToDateTime().ToString("O"),
        Value.ValueTypeOneofCase.StringValue => value.StringValue,
        _ => throw new NotSupportedException("Ordenação: " + value.ValueTypeCase)
    };

    private static bool Matches(Document doc, StructuredQuery.Types.Filter? filter)
    {
        if (filter == null) return true;
        if (filter.CompositeFilter != null)
        {
            if (filter.CompositeFilter.Op != StructuredQuery.Types.CompositeFilter.Types.Operator.And)
                throw new NotSupportedException("Filtro não AND");
            return filter.CompositeFilter.Filters.All(f => Matches(doc, f));
        }
        var field = filter.FieldFilter ?? throw new NotSupportedException("Filtro unário");
        if (field.Op != StructuredQuery.Types.FieldFilter.Types.Operator.Equal)
            throw new NotSupportedException("Filtro não Equal");
        return doc.Fields.TryGetValue(field.Field.FieldPath, out var value) && value.Equals(field.Value);
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method,
        string? host, CallOptions options, TRequest request) => throw new NotSupportedException();
    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options) => throw new NotSupportedException();
    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options) => throw new NotSupportedException();

    private sealed class MemoryStream<T>(IEnumerable<T> items) : IAsyncStreamReader<T>
    {
        private readonly IEnumerator<T> enumerator = items.GetEnumerator();
        public T Current => enumerator.Current;
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(enumerator.MoveNext());
        }
        public void Dispose() => enumerator.Dispose();
    }
}
