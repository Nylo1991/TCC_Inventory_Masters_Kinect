using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    public class Notificacao
    {
        [Key] 
        public int Id { get; set; }
        
        public int MedicaoId { get; set; }
        
        public int ParceiroId { get; set; }
        
        public DateTime DataEnvio { get; set; }
        
        public string? StatusEnvio { get; set; }
        
        public string? Mensagem { get; set; }
    }
}