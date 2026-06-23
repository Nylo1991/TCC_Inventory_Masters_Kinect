using System;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// Implementação de ICommand para facilitar a criação de comandos no MVVM.
/// Projeto: TCC Inventory Masters Kinect
/// </summary>
namespace TCC_Inventory_Masters_Kinect.Command
{
    /// <summary>
    /// Permite a execução de ações síncronas ou assíncronas, 
    /// e a definição de condições para habilitar ou desabilitar o comando.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Construtor para comandos síncronos. 
        /// Método de execução que trava até a finalização da tarefa.
        /// </summary>
        /// <param name="execute">Ação a ser executada pelo comando.</param>
        /// <param name="canExecute">Função que determina se o comando pode ser executado.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Construtor para comandos assíncronos.
        /// Método de execução que não bloqueia a interface do usuário.
        /// </summary>
        /// <param name="executeAsync">Função assíncrona a ser executada pelo comando.</param>
        /// <param name="canExecute">Função que determina se o comando pode ser executado.</param>
        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina se o comando pode ser executado, verificando a função de condição (se fornecida).
        /// </summary>
        /// <param name="parameter">Parâmetro passado para o comando (não utilizado neste caso).</param>
        /// <returns>Retorna true se o comando pode ser executado, caso contrário, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
       
        /// <summary>
        /// Executa a ação associada ao comando
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter)
        {
            if (_executeAsync != null)
                await _executeAsync();
            else
                _execute?.Invoke();
        }

        /// <summary>
        /// Evento que é acionado quando a condição de execução do comando muda,
        /// permitindo que a interface do usuário atualize o estado dos controles vinculados ao comando.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
