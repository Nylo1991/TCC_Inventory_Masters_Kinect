<<<<<<< HEAD
using Google.Cloud.Firestore;
=======
﻿using Google.Cloud.Firestore;
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
using System;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class TokenAcessoKinect
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? UsuarioId { get; set; }

        [FirestoreProperty]
        public string? UsuarioNome { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        public string? EmpresaId { get; set; }

        [FirestoreProperty]
        public string? Empresa { get; set; }

        [FirestoreProperty]
        public string? Perfil { get; set; }

        [FirestoreProperty]
        public string? TokenHash { get; set; }

        [FirestoreProperty]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime ExpiraEm { get; set; }

        [FirestoreProperty]
        public DateTime? ValidadoEm { get; set; }

        [FirestoreProperty]
        public bool Utilizado { get; set; }

        [FirestoreProperty]
        public bool Revogado { get; set; }
    }
}
