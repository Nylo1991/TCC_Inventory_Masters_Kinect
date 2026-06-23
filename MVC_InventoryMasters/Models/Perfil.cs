<<<<<<< HEAD
using Google.Cloud.Firestore;
=======
﻿using Google.Cloud.Firestore;
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec
using System;
using System.Collections.Generic;

namespace MVC_InventoryMasters.Models
{
    /// <summary>
    /// Representa um perfil de acesso disponível para a empresa.
    /// </summary>
    [FirestoreData]
    public class Perfil
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty("Perfil")]
        public string? Nome { get; set; }

        [FirestoreProperty]
        public string? EmpresaId { get; set; }

        [FirestoreProperty]
        public string? Descricao { get; set; }

        [FirestoreProperty]
        public List<string> Permissoes { get; set; } = new();

        [FirestoreProperty]
        public DateTime Data_Cadastro { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public bool Ativo { get; set; } = true;

        // Centraliza a checagem para evitar comparações diferentes nas telas e controllers.
        public bool PossuiPermissao(string permissao)
        {
            return Permissoes.Contains(permissao, StringComparer.OrdinalIgnoreCase);
        }
    }
}
