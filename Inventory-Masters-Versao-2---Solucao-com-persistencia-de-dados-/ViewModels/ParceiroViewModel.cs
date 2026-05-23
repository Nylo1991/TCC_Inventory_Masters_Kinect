using System.Collections.ObjectModel;
using System.Windows.Input;
using InventoryMaster.Commands;
using InventoryMaster.Data;
using InventoryMaster.Repositories;
using InventoryMaster.Models;

namespace InventoryMaster.ViewModels
{
    public class ParceiroViewModel : BaseViewModel
    {
        public ObservableCollection<Parceiro> Parceiros { get; set; }

        public ICommand SalvarCommand { get; }
        public ICommand AtualizarCommand { get; }

        public ParceiroViewModel()
        {
            Parceiros = new ObservableCollection<Parceiro>();

            SalvarCommand = new RelayCommand(Salvar, PodeSalvar);
            AtualizarCommand = new RelayCommand(CarregarParceiros);

            CarregarParceiros();
        }

        // Carregar parceiros do banco
        private void CarregarParceiros()
        {
            Parceiros.Clear();

            var lista = new ParceiroRepository().ListarParceiro();
            foreach (var parceiro in lista)
                Parceiros.Add(parceiro);
        }

        // Campos do formulário
        public string Nome { get; set; }
        public string Empresa { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }

        // Salvar parceiro
        private void Salvar()
        {
            var parceiro = new Parceiro
            {
                Nome = Nome,
                Empresa = Empresa,
                Email = Email,
                Telefone = Telefone,
                Endereco = Endereco,
                Data_Cadastro = DateTime.Now,
                Ativo = true
            };

            new ParceiroRepository().Inserir(parceiro);

            CarregarParceiros();
            LimparCampos();
        }

        private bool PodeSalvar()
        {
            return !string.IsNullOrWhiteSpace(Nome);
        }

        // Limpar campos
        private void LimparCampos()
        {
            Nome = string.Empty;
            Empresa = string.Empty;
            Email = string.Empty;
            Telefone = string.Empty;
            Endereco = string.Empty;
        }
    }
}