using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TCC_Inventory_Masters_Kinect.Command
{
    public class RelayCommands : ICommand  
    {
        // ==========================================
        // 1. CAMPOS PRIVADOS
        // ==========================================

        // Guarda uma ação assíncrona, usada quando o comando precisa executar await.
        private readonly Func<Task> _executeAsync;

        // Guarda uma ação comum, usada quando o comando não é assíncrono.
        private readonly Action _execute;

        // Guarda a condição para permitir ou bloquear a execução do comando.
        private readonly Func<bool> _canExecute;

        // ==========================================
        // 2. CONSTRUTOR PARA COMANDO ASSÍNCRONO
        // ==========================================

        public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _execute = null;
            _canExecute = canExecute;
        }

        // ==========================================
        // 3. CONSTRUTOR PARA COMANDO NORMAL
        // ==========================================

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _executeAsync = null;
            _canExecute = canExecute;
        }

        // ==========================================
        // 4. VERIFICA SE O COMANDO PODE EXECUTAR
        // ==========================================

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        // ==========================================
        // 5. EXECUTA O COMANDO
        // ==========================================

        public async void Execute(object parameter)
        {
            if (_executeAsync != null)
            {
                await _executeAsync();
            }
            else
            {
                _execute?.Invoke();
            }
        }

        // ==========================================
        // 6. EVENTO DE ATUALIZAÇÃO DO COMANDO
        // ==========================================

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }


}
}
