using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TCC_Inventory_Masters_Kinect.Command
{
    namespace TCC_Inventory_Masters_Kinect.Command
    {
        public class RelayCommand : ICommand
        {
            private readonly Func<Task> _executeAsync;
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
            {
                _executeAsync = executeAsync;
                _execute = null;
                _canExecute = canExecute;
            }

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute;
                _executeAsync = null;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute();
            }

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

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }

}