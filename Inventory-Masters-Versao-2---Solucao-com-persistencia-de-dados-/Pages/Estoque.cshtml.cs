using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace InventoryMaster.Pages
{
    public class EstoqueModel : PageModel
    {
        public List<EntradaResiduos> Entradas { get; set; } = new();

        public void OnGet()
        {
            // Simulação de dados
            Entradas.Add(new EntradaResiduos { Data = DateTime.Now.AddHours(-4), Volume = 0 });
            Entradas.Add(new EntradaResiduos { Data = DateTime.Now.AddHours(-3), Volume = 0 });
            Entradas.Add(new EntradaResiduos { Data = DateTime.Now.AddHours(-2), Volume = 0 });
            Entradas.Add(new EntradaResiduos { Data = DateTime.Now.AddHours(-1), Volume = 0 });
        }

        // Endpoint chamado pelo JS - apenas simula envio
        public IActionResult OnPostEnviarEmail()
        {
            try
            {
                // Simula envio de email
                Console.WriteLine("Simulação: Email enviado aos parceiros.");
                
                // Retorna sucesso ao JS
                return new JsonResult(new { sucesso = true, mensagem = "Simulação de envio concluída." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }

    public class EntradaResiduos
    {
        public DateTime Data { get; set; }
        public double Volume { get; set; }
    }
}