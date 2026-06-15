using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{

    /// <summary>
    /// resposável por fornecer a funcionalidade básica de notificação de propriedade para os ViewModels da aplicação,
    /// permitindo que a interface do usuário seja atualizada automaticamente quando os dados subjacentes mudam.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento que é acionado quando uma propriedade é alterada, permitindo que a interface do usuário seja notificada
        /// sobre a mudança.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Método protegido que dispara o evento PropertyChanged para notificar a 
        /// interface do usuário sobre mudanças em uma propriedade.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade que mudou.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
