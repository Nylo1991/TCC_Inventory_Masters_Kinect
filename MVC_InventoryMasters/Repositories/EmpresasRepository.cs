<<<<<<< HEAD
using Google.Cloud.Firestore;
=======
﻿using Google.Cloud.Firestore;
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    public class EmpresasRepository
    {
        private readonly FirestoreDb _db;
        private readonly ILogger<EmpresasRepository> _logger;
        private readonly string _colecao = "Empresas";

        public EmpresasRepository(FirebaseService firebaseService, ILogger<EmpresasRepository> logger)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
        }

        public async Task<List<Empresa>> ListarTodas()
        {
            try
            {
                var snapshot = await _db.Collection(_colecao).GetSnapshotAsync();

                return snapshot.Documents.Select(doc =>
                {
                    var empresa = doc.ConvertTo<Empresa>();
                    empresa.Id = doc.Id;
                    return empresa;
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar empresas.");
                return new List<Empresa>();
            }
        }

        public async Task<Empresa?> BuscarPorId(string id)
        {
            try
            {
                var doc = await _db.Collection(_colecao).Document(id).GetSnapshotAsync();

                if (!doc.Exists)
                    return null;

                var empresa = doc.ConvertTo<Empresa>();
                empresa.Id = doc.Id;
                return empresa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar empresa {EmpresaId}.", id);
                return null;
            }
        }
    }
}
