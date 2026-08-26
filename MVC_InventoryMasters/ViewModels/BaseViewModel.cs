namespace MVC_InventoryMasters.ViewModels
{
    public abstract class BaseViewModel
    {
        // Propriedade que toda página

        public string NomeUsuario { get; set; } = string.Empty;
        public string TituloPagina { get; set; } = string.Empty;
    }

    // Se precisar de suporte a notificação de mudança no futuro:
    // public event PropertyChangedEventHandler PropertyChanged;
}
