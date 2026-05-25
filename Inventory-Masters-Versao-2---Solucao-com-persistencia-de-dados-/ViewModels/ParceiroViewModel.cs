using InventoryMaster.Commands;
using InventoryMaster.Data;
using InventoryMaster.Models;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InventoryMaster.ViewModels;

public class ParceiroViewModel : BaseViewModel
{
    private readonly ParceiroRepository _repository;

    public ObservableCollection<Parceiro> Parceiros { get; set; }

    public ICommand SalvarCommand { get; }
    public ICommand AtualizarCommand { get; }

    // --------------------------------------------------------
    // CAMPOS DO FORMULÁRIO (Completos para notificar a View)
    // --------------------------------------------------------
    private string _nome = string.Empty;
    public string Nome
    {
        get => _nome;
        set { _nome = value; OnPropertyChanged(nameof(Nome)); }
    }

    private string _empresa = string.Empty;
    public string Empresa
    {
        get => _empresa;
        set { _empresa = value; OnPropertyChanged(nameof(Empresa)); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(nameof(Email)); }
    }

    private string _telefone = string.Empty;
    public string Telefone
    {
        get => _telefone;
        set { _telefone = value; OnPropertyChanged(nameof(Telefone)); }
    }

    private string _endereco = string.Empty;
    public string Endereco
    {
        get => _endereco;
        set { _endereco = value; OnPropertyChanged(nameof(Endereco)); }
    }

 
    public ParceiroViewModel(ParceiroRepository repository)
    {
        _repository = repository;

        Parceiros = new ObservableCollection<Parceiro>();

        SalvarCommand = new RelayCommand(async () => await SalvarAsync(), PodeSalvar);
        AtualizarCommand = new RelayCommand(async () => await CarregarParceirosAsync());

        _ = CarregarParceirosAsync();
    }

 
    private async Task CarregarParceirosAsync()
    {
        Parceiros.Clear();

        var lista = await _repository.ListarParceiroAsync();

        foreach (var parceiro in lista)
        {
            Parceiros.Add(parceiro);
        }
    }

    
    private async Task SalvarAsync()
    {
        var parceiro = new Parceiro
        {
            Nome = Nome,
            Empresa = Empresa,
            Email = Email,
            Telefone = Telefone,
            Endereco = Endereco,
            Data_Cadastro = DateTime.UtcNow,
            Ativo = true
        };

        await _repository.InserirAsync(parceiro);

        await CarregarParceirosAsync();

        LimparCampos();
    }

    private bool PodeSalvar()
    {
        return !string.IsNullOrWhiteSpace(Nome);
    }

    private void LimparCampos()
    {
        
        Nome = string.Empty;
        Empresa = string.Empty;
        Email = string.Empty;
        Telefone = string.Empty;
        Endereco = string.Empty;
    }
}