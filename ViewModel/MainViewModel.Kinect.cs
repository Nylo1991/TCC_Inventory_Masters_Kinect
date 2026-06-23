using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Atualiza a imagem da câmera RGB exibida na interface.
        /// </summary>
        private void AtualizarCameraRgb(BitmapSource imagem)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CameraImage = imagem;
            }));
        }

        /// <summary>
        /// Liga o Kinect, conecta ao SignalR e inicia os timers visuais.
        /// </summary>
        private async Task LigarKinectAsync()
        {
            try
            {
                _kinectService.CameraFrameAtualizado -= AtualizarCameraRgb;
                _kinectService.CameraFrameAtualizado += AtualizarCameraRgb;

                _kinectService.Start();

                StatusKinect = "Kinect conectado";
                StatusMessage = "Kinect iniciado com sucesso";

                StatusSignalR = "SignalR: Conectando...";
                await _signalRService.ConectarAsync();

                StatusSignalR = _signalRService.EstaConectado
                    ? "SignalR: Conectado"
                    : "SignalR: Sem conexão";

                IniciarTimerFrames();

                LoggerService.Info("Kinect iniciado pela MainViewModel.");
            }
            catch
            {
                StatusKinect = "Kinect: erro ao conectar";
                StatusMessage = "Erro ao iniciar Kinect";
                LoggerService.Erro("Erro ao iniciar Kinect pela MainViewModel.");
            }
        }

        /// <summary>
        /// Inicia o timer responsável pela atualização visual do mapa de profundidade.
        /// </summary>
        private void IniciarTimerFrames()
        {
            try
            {
                _frameTimer?.Stop();

                _frameTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };

                _frameTimer.Tick += (s, e) =>
                {
                    var depthFrame = _kinectService.CapturarDepthColorido();

                    if (depthFrame != null)
                    {
                        DepthImage = depthFrame;
                    }
                };

                _frameTimer.Start();
            }
            catch
            {
                LoggerService.Erro("Erro ao iniciar timer de frames.");
            }
        }

        /// <summary>
        /// Desliga o Kinect, para os timers e libera os recursos utilizados.
        /// </summary>
        private void DesligarKinect()
        {
            try
            {
                _frameTimer?.Stop();
                _frameTimer = null;

                _volumeTimer?.Stop();
                _volumeTimer = null;

                _kinectService.CameraFrameAtualizado -= AtualizarCameraRgb;
                _kinectService.Stop();

                CameraImage = null;
                DepthImage = null;

                StatusKinect = "Kinect desligado";
                StatusMessage = "Kinect encerrado pelo usuário";

                LoggerService.Info("Kinect desligado pela MainViewModel.");
            }
            catch
            {
                StatusKinect = "Erro ao desligar Kinect";
                StatusMessage = "Erro ao desligar Kinect.";
                LoggerService.Erro("Erro ao desligar Kinect pela MainViewModel.");
            }
        }

        /// <summary>
        /// Executa a calibração do ambiente vazio usando o Kinect.
        /// </summary>
        private async Task ExecutarCalibracaoAsync()
        {
            try
            {
                IsCalibrating = true;
                EspacoSalvo = false;

                _volumeTimer?.Stop();

                StatusMessage = "Calibrando ambiente ...";
                StatusCalibracao = "Capturando dados de profundidade.";

                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);

                _volumeMaximoCm3 = resultado.MaxVolume;

                VolumeMaximo = FormatarVolumeM3(resultado.MaxVolume);
                QuantidadePontosDepth = resultado.TotalPointsFound.ToString();

                VolumeTexto = "0.000 m3";
                PercentualOcupacaoTexto = "0%";
                EspacoLivreTexto = FormatarVolumeM3(resultado.MaxVolume);

                StatusMessage = $"Calibração concluída. Volume máximo: {FormatarVolumeM3(resultado.MaxVolume)}";
                StatusCalibracao = "Calibração finalizada.";
                MensagemEspaco = "Calibração concluída. Salve o espaço para liberar medições.";

                CalibracaoFinalizada?.Invoke();

                LoggerService.Info($"Calibração concluída. Volume máximo: {resultado.MaxVolume:F0} cm3");
            }
            catch
            {
                StatusMessage = "Erro na calibração";
                StatusCalibracao = "Erro ao calibrar.";
                LoggerService.Erro("Erro na calibração pela MainViewModel.");
            }
            finally
            {
                IsCalibrating = false;
            }
        }

        /// <summary>
        /// Método utilizado ao sair manualmente do sistema.
        /// Desliga o monitoramento antes de retornar para a tela de login.
        /// </summary>
        public void DesligarMonitoramento()
        {
            DesligarKinect();
        }
    }
}