using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Service;
using TCC_Inventory_Masters_Kinect.View;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    /// <summary>
    /// Estado e comandos da interface da janela principal.
    /// Mantém no ViewModel as regras que antes estavam no code-behind.
    /// </summary>
    public partial class MainViewModel
    {
        private readonly TimeSpan _tempoLimiteInatividade = TimeSpan.FromMinutes(20);
        private DispatcherTimer _inatividadeTimer;
        private bool _encerrado;

        private bool _sessaoBloqueada;
        public bool SessaoBloqueada
        {
            get => _sessaoBloqueada;
            private set => SetProperty(ref _sessaoBloqueada, value);
        }

        private string _emailSessao;
        public string EmailSessao
        {
            get => _emailSessao;
            private set => SetProperty(ref _emailSessao, value);
        }

        private string _tokenDesbloqueio;
        public string TokenDesbloqueio
        {
            get => _tokenDesbloqueio;
            set => SetProperty(ref _tokenDesbloqueio, value);
        }

        private string _mensagemBloqueio;
        public string MensagemBloqueio
        {
            get => _mensagemBloqueio;
            private set => SetProperty(ref _mensagemBloqueio, value);
        }

        private bool _solicitarNovoTokenHabilitado = true;
        public bool SolicitarNovoTokenHabilitado
        {
            get => _solicitarNovoTokenHabilitado;
            private set => SetProperty(ref _solicitarNovoTokenHabilitado, value);
        }

        private bool _desbloquearHabilitado = true;
        public bool DesbloquearHabilitado
        {
            get => _desbloquearHabilitado;
            private set => SetProperty(ref _desbloquearHabilitado, value);
        }

        private bool _calibrationVideoPlaying;
        public bool CalibrationVideoPlaying
        {
            get => _calibrationVideoPlaying;
            private set => SetProperty(ref _calibrationVideoPlaying, value);
        }

        private string _calibrationTitle = "Calibração";
        public string CalibrationTitle
        {
            get => _calibrationTitle;
            private set => SetProperty(ref _calibrationTitle, value);
        }

        private string _calibrationSubtitle = string.Empty;
        public string CalibrationSubtitle
        {
            get => _calibrationSubtitle;
            private set => SetProperty(ref _calibrationSubtitle, value);
        }

        private bool _avisoHistoricoVisivel;
        public bool AvisoHistoricoVisivel
        {
            get => _avisoHistoricoVisivel;
            private set => SetProperty(ref _avisoHistoricoVisivel, value);
        }

        private void InicializarInterfaceMonitor(bool iniciarTimerInatividade)
        {
            EmailSessao = _sessao.Email;
            MensagemBloqueio = string.Empty;

            if (!iniciarTimerInatividade)
            {
                return;
            }

            _inatividadeTimer = new DispatcherTimer
            {
                Interval = _tempoLimiteInatividade
            };
            _inatividadeTimer.Tick += InatividadeTimerTick;
            _inatividadeTimer.Start();
        }

        private void RegistrarAtividadeUsuario()
        {
            if (!SessaoBloqueada)
            {
                ReiniciarTimerInatividade();
            }
        }

        private void ReiniciarTimerInatividade()
        {
            _inatividadeTimer?.Stop();
            _inatividadeTimer?.Start();
        }

        internal void InatividadeTimerTick(object sender, EventArgs e)
        {
            _inatividadeTimer?.Stop();

            if (SessaoBloqueada)
            {
                return;
            }

            SessaoBloqueada = true;
            TokenDesbloqueio = string.Empty;
            MensagemBloqueio = "Solicite um novo token para desbloquear esta sessão.";
            LoggerService.LogWarning("Sessão bloqueada por inatividade. Monitoramento Kinect continua ativo.");
        }

        internal async Task SolicitarNovoTokenAsync()
        {
            SolicitarNovoTokenHabilitado = false;
            MensagemBloqueio = "Solicitando novo token...";

            try
            {
                var resultado = await _autenticacaoService.SolicitarTokenAsync(_sessao.Email);

                MensagemBloqueio = resultado != null && resultado.Sucesso
                    ? "Token enviado. Consulte seu e-mail e informe o código recebido."
                    : resultado?.Mensagem ?? "Não foi possível solicitar um novo token.";

                if (resultado != null && resultado.Sucesso)
                {
                    LoggerService.Info("Novo token solicitado para desbloqueio por inatividade.");
                }
            }
            catch (Exception ex)
            {
                MensagemBloqueio = "Erro ao solicitar o token de desbloqueio.";
                LoggerService.Erro("Erro ao solicitar token de desbloqueio: " + ex.Message);
            }
            finally
            {
                SolicitarNovoTokenHabilitado = true;
            }
        }

        internal async Task DesbloquearSessaoAsync()
        {
            DesbloquearHabilitado = false;

            try
            {
                string token = TokenDesbloqueio?.Trim();

                if (string.IsNullOrWhiteSpace(token))
                {
                    MensagemBloqueio = "Informe o token para desbloquear.";
                    return;
                }

                var resultado = await _autenticacaoService.ValidarTokenAsync(token);

                bool mesmaSessao = resultado != null &&
                    resultado.TokenValido &&
                    string.Equals(resultado.Email, _sessao.Email, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(resultado.Empresa, _sessao.Empresa, StringComparison.OrdinalIgnoreCase);

                if (!mesmaSessao)
                {
                    MensagemBloqueio = resultado?.Mensagem ??
                        "Token inválido, expirado ou pertencente a outro usuário.";
                    LoggerService.LogWarning("Tentativa inválida de desbloqueio por token.");
                    return;
                }

                SessaoBloqueada = false;
                TokenDesbloqueio = string.Empty;
                MensagemBloqueio = string.Empty;
                ReiniciarTimerInatividade();

                LoggerService.Info("Sessão desbloqueada após inatividade.");
            }
            catch (Exception ex)
            {
                MensagemBloqueio = "Erro ao desbloquear sessão.";
                LoggerService.Erro("Erro ao desbloquear sessão por inatividade: " + ex.Message);
            }
            finally
            {
                DesbloquearHabilitado = true;
            }
        }

        private void AbrirHistorico()
        {
            if (!EspacoSalvo)
            {
                AvisoHistoricoVisivel = true;
                return;
            }

            var janelaAtual = ObterJanelaAtual();
            var janelaHistorico = new HistoricoMedicoesWindow(this)
            {
                Owner = janelaAtual
            };

            janelaHistorico.ShowDialog();
        }

        private void Sair()
        {
            Encerrar();

            var janelaAtual = ObterJanelaAtual();
            var login = new KinectLogin();
            login.Show();
            janelaAtual?.Close();
        }

        private Window ObterJanelaAtual()
        {
            return Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(janela => ReferenceEquals(janela.DataContext, this));
        }

        private void Encerrar()
        {
            if (_encerrado)
            {
                return;
            }

            _encerrado = true;
            PararAtualizacaoHistorico();

            if (_inatividadeTimer != null)
            {
                _inatividadeTimer.Stop();
                _inatividadeTimer.Tick -= InatividadeTimerTick;
                _inatividadeTimer = null;
            }

            DesligarMonitoramento();
        }

        private void IniciarVideoCalibracao()
        {
            CalibrationTitle = "Calibração em andamento";
            CalibrationSubtitle = "Aguarde enquanto o Kinect calibra o espaço vazio";
            CalibrationVideoPlaying = true;
        }

        private void FinalizarVideoCalibracao(bool calibracaoConcluida)
        {
            CalibrationVideoPlaying = false;
            CalibrationTitle = calibracaoConcluida
                ? "Calibração concluída"
                : "Erro na calibração";
            CalibrationSubtitle = calibracaoConcluida
                ? "Salve o espaço para liberar as medições automáticas"
                : "Verifique o Kinect e tente novamente";
        }
    }
}
