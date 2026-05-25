using System;
using System.Windows.Input;

namespace InventoryMaster.Commands;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    // 1. Colocamos o "?" aqui na variável
    private readonly Func<bool>? _canExecute;

    // Colocamos o "?" aqui para avisar o C# que esse evento pode ser nulo sem problemas
    public event EventHandler? CanExecuteChanged;

    // 2. Colocamos o "?" aqui no parâmetro do construtor para resolver o CS8625
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object? parameter)
    {
        _execute();
    }

    // Método opcional para forçar a tela a reavaliar se o botão deve ficar ativo/inativo
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}