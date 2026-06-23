using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using Microsoft.Extensions.Logging;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento das configurações
    /// e parâmetros gerais do sistema.
    /// </summary>
    /// <remarks>
    /// Centraliza o acesso às configurações armazenadas no Firebase Firestore,
    /// permitindo consulta, atualização e cálculos relacionados aos parâmetros
    /// operacionais do sistema.
    /// </remarks>
    public class ParametrosSistemaRepository
    {
        private readonly string _colecao = "parametrosSistema";
        private readonly FirestoreDb _db;
        private readonly ILogger<ParametrosSistemaRepository> _logger;
        private readonly ContextoUsuarioService _contextoUsuario;

        /// <summary>
        /// Inicializa uma nova instância do repositório de parâmetros do sistema.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável por fornecer a conexão com o Firebase Firestore.
        /// </param>
        public ParametrosSistemaRepository(
            FirebaseService firebaseService,
            ILogger<ParametrosSistemaRepository> logger,
            ContextoUsuarioService contextoUsuario)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
            _contextoUsuario = contextoUsuario;
        }

        /// <summary>
        /// Obtém as configurações atualmente cadastradas no sistema.
        /// </summary>
        /// <remarks>
        /// Caso não exista uma configuração cadastrada ou ocorra alguma falha
        /// durante a consulta, será retornada uma instância padrão de
        /// <see cref="ParametrosSistema"/>.
        /// </remarks>
        /// <returns>
        /// Objeto contendo os parâmetros configurados no sistema.
        /// </returns>
        public ParametrosSistema Buscar()
        {
            return BuscarPorEmpresa(_contextoUsuario.ObterEmpresaId());
        }

        public ParametrosSistema BuscarPorEmpresa(string empresaId)
        {
            try
            {
                string documentId = ObterDocumentId(empresaId);

                var docRef = _db
                    .Collection(_colecao)
                    .Document(documentId);

                var snapshot = docRef
                    .GetSnapshotAsync()
                    .Result;

                if (!snapshot.Exists)
                {
                    return empresaId == ContextoUsuarioService.EmpresaPadraoId
                        ? new ParametrosSistema { EmpresaId = empresaId }
                        : BuscarConfiguracaoGlobalComoFallback(empresaId);
                }

                var dados = snapshot.ToDictionary();

                return new ParametrosSistema
                {
                    CapacidadeMaxima =
                        dados.TryGetValue("CapacidadeMaxima", out var capacidadeMaxima)
                            ? Convert.ToDouble(capacidadeMaxima)
                            : 0,

                    CapacidadeMinima =
                        dados.TryGetValue("CapacidadeMinima", out var capacidadeMinima)
                            ? Convert.ToDouble(capacidadeMinima)
                            : 0,

                    PercentualAlerta =
                        dados.TryGetValue("PercentualAlerta", out var percentualAlerta)
                            ? Convert.ToInt32(percentualAlerta)
                            : 80,

                    DataAtualizacao =
                        dados.TryGetValue("DataAtualizacao", out var dataAtualizacao)
                        && dataAtualizacao is Timestamp timestamp
                            ? timestamp.ToDateTime()
                            : DateTime.MinValue,

                    NotificacaoAutomatica =
                        dados.TryGetValue("NotificacaoAutomatica", out var notificacaoAutomatica)
                            ? Convert.ToBoolean(notificacaoAutomatica)
                            : true,

                    ExibirAlertaDashboard =
                        dados.TryGetValue("ExibirAlertaDashboard", out var exibirAlertaDashboard)
                            ? Convert.ToBoolean(exibirAlertaDashboard)
                            : true,

                    ParceiroPadraoId =
                        dados.TryGetValue("ParceiroPadraoId", out var parceiroPadraoId)
                            ? parceiroPadraoId?.ToString()
                            : null,

                    DiasSemColetaAlerta =
                        dados.TryGetValue("DiasSemColetaAlerta", out var diasSemColeta)
                            ? Convert.ToInt32(diasSemColeta)
                            : 15,

                    EmpresaId =
                        dados.TryGetValue("EmpresaId", out var empresaConfiguracao)
                            ? empresaConfiguracao?.ToString()
                            : empresaId,

                    AtivarSistemaCalibracao =
                        dados.TryGetValue("AtivarSistemaCalibracao", out var ativarCalibracao)
                            ? Convert.ToBoolean(ativarCalibracao)
                            : false,

                    RaioDeteccaoKinect =
                        dados.TryGetValue("RaioDeteccaoKinect", out var raioDeteccao)
                            ? Convert.ToDouble(raioDeteccao)
                            : 0,

                    HabilitarZonaExclusaoDeteccao =
                        dados.TryGetValue("HabilitarZonaExclusaoDeteccao", out var zonaExclusao)
                            ? Convert.ToBoolean(zonaExclusao)
                            : false,

                    TaxaAmostragemVolumeMinutos =
                        dados.TryGetValue("TaxaAmostragemVolumeMinutos", out var taxaAmostragem)
                            ? Convert.ToInt32(taxaAmostragem)
                            : 10,

                    DuracaoMaximaMedicaoSegundos =
                        dados.TryGetValue("DuracaoMaximaMedicaoSegundos", out var duracaoMedicao)
                            ? Convert.ToInt32(duracaoMedicao)
                            : 2000,

                    TipoAlertaPadrao =
                        dados.TryGetValue("TipoAlertaPadrao", out var tipoAlerta)
                            ? tipoAlerta?.ToString() ?? "Critico"
                            : "Critico",

                    TemplateMensagemPadrao =
                        dados.TryGetValue("TemplateMensagemPadrao", out var templateMensagem)
                            ? templateMensagem?.ToString() ?? string.Empty
                            : "Olá, {{Parceiro}}.\n\nO estoque em {{EspacoID}} atingiu {{VolumePercentual}}% da capacidade crítica às {{DataHora}}. Por favor, realize a coleta imediata.\n\nAcompanhe no painel.",

                    CanalEmailAtivo =
                        dados.TryGetValue("CanalEmailAtivo", out var canalEmail)
                            ? Convert.ToBoolean(canalEmail)
                            : true,

                    CanalWhatsAppAtivo =
                        dados.TryGetValue("CanalWhatsAppAtivo", out var canalWhatsApp)
                            ? Convert.ToBoolean(canalWhatsApp)
                            : true,

                    CanalDashboardPushAtivo =
                        dados.TryGetValue("CanalDashboardPushAtivo", out var canalDashboard)
                            ? Convert.ToBoolean(canalDashboard)
                            : true,

                    NomeRemetenteWhatsApp =
                        dados.TryGetValue("NomeRemetenteWhatsApp", out var remetenteWhatsApp)
                            ? remetenteWhatsApp?.ToString()
                            : null,

                    EscalonamentoMinutos =
                        dados.TryGetValue("EscalonamentoMinutos", out var escalonamentoMinutos)
                            ? Convert.ToInt32(escalonamentoMinutos)
                            : 10,

                    CanalEscalonamento =
                        dados.TryGetValue("CanalEscalonamento", out var canalEscalonamento)
                            ? canalEscalonamento?.ToString() ?? "E-mail"
                            : "E-mail"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao recuperar os parâmetros do sistema.");

                return new ParametrosSistema();
            }
        }

        private ParametrosSistema BuscarConfiguracaoGlobalComoFallback(string empresaId)
        {
            var global = _db
                .Collection(_colecao)
                .Document("configuracao")
                .GetSnapshotAsync()
                .Result;

            if (!global.Exists)
                return new ParametrosSistema { EmpresaId = empresaId };

            var parametros = global.ConvertTo<ParametrosSistema>();
            parametros.EmpresaId = empresaId;
            return parametros;
        }

        public ParametrosSistema ObterPadroes()
        {
            return new ParametrosSistema
            {
                CapacidadeMaxima = 300,
                CapacidadeMinima = 0,
                PercentualAlerta = 10,
                NotificacaoAutomatica = true,
                ExibirAlertaDashboard = true,
                DiasSemColetaAlerta = 10,
                AtivarSistemaCalibracao = false,
                RaioDeteccaoKinect = 0,
                HabilitarZonaExclusaoDeteccao = false,
                TaxaAmostragemVolumeMinutos = 10,
                DuracaoMaximaMedicaoSegundos = 2000,
                TipoAlertaPadrao = "Critico",
                TemplateMensagemPadrao =
                    "Olá, {{Parceiro}}.\n\nO estoque em {{EspacoID}} atingiu {{VolumePercentual}}% da capacidade crítica às {{DataHora}}. Por favor, realize a coleta imediata.\n\nAcompanhe no painel.",
                CanalEmailAtivo = true,
                CanalWhatsAppAtivo = true,
                CanalDashboardPushAtivo = true,
                NomeRemetenteWhatsApp = string.Empty,
                EscalonamentoMinutos = 10,
                CanalEscalonamento = "E-mail"
            };
        }

        /// <summary>
        /// Salva as configurações do sistema.
        /// </summary>
        public void Salvar(ParametrosSistema parametros)
        {
            try
            {
                parametros.EmpresaId = string.IsNullOrWhiteSpace(parametros.EmpresaId)
                    ? _contextoUsuario.ObterEmpresaId()
                    : parametros.EmpresaId;

                parametros.DataAtualizacao = DateTime.UtcNow;

                var dados = new Dictionary<string, object>
        {
            { "EmpresaId", parametros.EmpresaId ?? ContextoUsuarioService.EmpresaPadraoId },
            { "CapacidadeMaxima", parametros.CapacidadeMaxima },
            { "CapacidadeMinima", parametros.CapacidadeMinima },
            { "PercentualAlerta", parametros.PercentualAlerta },
            { "DataAtualizacao", parametros.DataAtualizacao },
            { "NotificacaoAutomatica", parametros.NotificacaoAutomatica },
            { "ExibirAlertaDashboard", parametros.ExibirAlertaDashboard },
            { "ParceiroPadraoId", parametros.ParceiroPadraoId ?? string.Empty },
            { "DiasSemColetaAlerta", parametros.DiasSemColetaAlerta },
            { "AtivarSistemaCalibracao", parametros.AtivarSistemaCalibracao },
            { "RaioDeteccaoKinect", parametros.RaioDeteccaoKinect },
            { "HabilitarZonaExclusaoDeteccao", parametros.HabilitarZonaExclusaoDeteccao },
            { "TaxaAmostragemVolumeMinutos", parametros.TaxaAmostragemVolumeMinutos },
            { "DuracaoMaximaMedicaoSegundos", parametros.DuracaoMaximaMedicaoSegundos },
            { "TipoAlertaPadrao", parametros.TipoAlertaPadrao ?? "Critico" },
            { "TemplateMensagemPadrao", parametros.TemplateMensagemPadrao ?? string.Empty },
            { "CanalEmailAtivo", parametros.CanalEmailAtivo },
            { "CanalWhatsAppAtivo", parametros.CanalWhatsAppAtivo },
            { "CanalDashboardPushAtivo", parametros.CanalDashboardPushAtivo },
            { "NomeRemetenteWhatsApp", parametros.NomeRemetenteWhatsApp ?? string.Empty },
            { "EscalonamentoMinutos", parametros.EscalonamentoMinutos },
            { "CanalEscalonamento", parametros.CanalEscalonamento ?? "E-mail" }
        };

                _db
                    .Collection(_colecao)
                    .Document(ObterDocumentId(parametros.EmpresaId))
                    .SetAsync(dados)
                    .Wait();

                _logger.LogInformation(
                    "Parâmetros do sistema atualizados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao salvar os parâmetros do sistema.");

                throw new Exception(
                    "Não foi possível salvar os parâmetros do sistema.");
            }
        }

        private static string ObterDocumentId(string? empresaId)
        {
            if (string.IsNullOrWhiteSpace(empresaId) ||
                empresaId == ContextoUsuarioService.EmpresaPadraoId)
            {
                return "configuracao";
            }

            return $"configuracao_{empresaId}";
        }

        /// <summary>
        /// Calcula o percentual de ocupação do estoque.
        /// </summary>
        /// <param name="volumeAtual">
        /// Volume atualmente ocupado no estoque.
        /// </param>
        /// <param name="capacidadeMaxima">
        /// Capacidade máxima configurada para o estoque.
        /// </param>
        /// <returns>
        /// Percentual de ocupação calculado.
        /// Caso a capacidade máxima seja menor ou igual a zero,
        /// será retornado o valor zero.
        /// </returns>
        public double CalcularPercentualOcupacao(
            double volumeAtual,
            double capacidadeMaxima)
        {
            if (capacidadeMaxima <= 0)
                return 0;

            return (volumeAtual / capacidadeMaxima) * 100;
        }
    }
}
