using Google.Cloud.Firestore;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

var builder = WebApplication.CreateBuilder(args);

#region Firebase

/// <summary>
/// Define o caminho das credenciais da conta de serviço
/// utilizada para autenticação com o Firebase.
/// </summary>
string jsonPath = Path.Combine(
    AppContext.BaseDirectory,
    "firebase-service-account.json");

Environment.SetEnvironmentVariable(
    "GOOGLE_APPLICATION_CREDENTIALS",
    jsonPath);

/// <summary>
/// ID do projeto Firebase.
/// </summary>
string projectId = "inventorymasters";

#endregion

#region MVC

/// <summary>
/// Registra suporte a Controllers e Views.
/// </summary>
builder.Services.AddControllersWithViews();

#endregion

#region Firebase Services

/// <summary>
/// Serviço principal responsável pela comunicação
/// com o Firestore.
/// </summary>
builder.Services.AddSingleton<FirebaseService>();

#endregion

#region Repositories

/// <summary>
/// Repositórios responsáveis pelo acesso aos dados.
/// Cada repositório encapsula uma coleção do Firestore.
/// </summary>
builder.Services.AddScoped<ParceirosRepository>();
builder.Services.AddScoped<MedicaoVolumeRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<ParametrosSistemaRepository>();
builder.Services.AddScoped<UsuariosRepository>();

#endregion

#region CORS

/// <summary>
/// Política de CORS utilizada para permitir conexões
/// externas ao SignalR e à aplicação.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region SignalR

/// <summary>
/// Configuração do SignalR responsável pela comunicação
/// em tempo real entre sensores e dashboard.
/// </summary>
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

/// <summary>
/// Redireciona requisições HTTP para HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Habilita acesso a arquivos estáticos
/// (CSS, JS, imagens).
/// </summary>
app.UseStaticFiles();

app.UseRouting();

/// <summary>
/// Deve ser executado antes dos endpoints
/// para permitir conexões externas.
/// </summary>
app.UseCors("AllowAll");

app.UseAuthorization();

#endregion

#region Endpoints

/// <summary>
/// Endpoint do SignalR utilizado pelos sensores
/// e pelo Dashboard para troca de dados em tempo real.
///
/// </summary>
app.MapHub<MedicaoHub>("/medicaoHub");

/// <summary>
/// Rota padrão MVC.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();