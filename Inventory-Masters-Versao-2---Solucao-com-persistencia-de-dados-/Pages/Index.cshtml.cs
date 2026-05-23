using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace InventoryMaster.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Senha { get; set; }

        public string MensagemErro { get; set; }

        // Usuário simulado
        public class UsuarioSimulado
        {
            public string Email { get; set; }
            public string Senha { get; set; }
            public string Nome { get; set; }
            public string FotoPerfil { get; set; }
        }

        private List<UsuarioSimulado> Usuarios = new List<UsuarioSimulado>
        {
            new UsuarioSimulado 
            { 
                Email = "admin@teste.com", 
                Senha = "123456", 
                Nome = "Administrador", 
                FotoPerfil = "https://i.pravatar.cc/100?img=1" 
            },
            new UsuarioSimulado 
            { 
                Email = "joao@teste.com", 
                Senha = "abc123", 
                Nome = "João Silva", 
                FotoPerfil = "https://i.pravatar.cc/100?img=2" 
            },
            new UsuarioSimulado 
            { 
                Email = "maria@teste.com", 
                Senha = "senha123", 
                Nome = "Maria Oliveira", 
                FotoPerfil = "https://i.pravatar.cc/100?img=3" 
            }
        };

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var usuario = Usuarios.FirstOrDefault(u =>
                u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase) &&
                u.Senha == Senha
            );

            if (usuario != null)
            {
                // Salva dados na sessão
                HttpContext.Session.SetString("NomeUsuario", usuario.Nome);
                HttpContext.Session.SetString("EmailUsuario", usuario.Email);
                HttpContext.Session.SetString("FotoPerfil", usuario.FotoPerfil);

                return RedirectToPage("/Inicial");
            }
            else
            {
                MensagemErro = "E-mail ou senha inválidos!";
                return Page();
            }
        }
    }
}
