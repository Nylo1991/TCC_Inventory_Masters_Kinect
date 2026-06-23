<<<<<<< HEAD
using Google.Cloud.Firestore;
=======
﻿using Google.Cloud.Firestore;
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
using System;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class LogSistema
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? EmpresaId { get; set; }

        [FirestoreProperty]
        public string? UsuarioId { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        public string? Acao { get; set; }

        [FirestoreProperty]
        public string? Mensagem { get; set; }

        [FirestoreProperty]
        public string? Nivel { get; set; } = "Informacao";

        [FirestoreProperty]
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
    }
}
