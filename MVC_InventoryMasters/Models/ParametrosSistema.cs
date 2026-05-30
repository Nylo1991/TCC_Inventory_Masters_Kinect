using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    public class ParametrosSistema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NomeParametro { get; set; } // Ex: "VolumeMaximoEstoque"

        [Required]
        public double ValorMaximo { get; set; }   // Valor limite para gatilho de alerta

        public double ValorMinimo { get; set; }   // Opcional: limite mínimo, se necessário

        public string UnidadeMedida { get; set; } // Ex: "cm3"

        public DateTime DataAtualizacao { get; set; }
    }
}