namespace MVC_InventoryMasters.ViewModels
{
    public abstract class BaseViewModel
    {
        // Propriedade que toda página

        public string NomeUsuario { get; set; } 
        public string TituloPagina { get; set; }
    }

    // Se precisar de suporte a notificação de mudança no futuro:
    // public event PropertyChangedEventHandler PropertyChanged;
}
