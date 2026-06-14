using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    /// <summary>
    /// Serviço responsável pela comunicação em tempo real entre o sistema WPF
    /// Inventory Masters Kinect e a aplicação MVC por meio do SignalR.
    /// </summary>
    public class SignalRService
    {
        private HubConnection _connection;

        /// <summary>
        /// Armazena a última mensagem de erro relacionada à comunicação SignalR.
        /// </summary>
        public string UltimoErro { get; private set; } = string.Empty;

        /// <summary>
        /// Evento utilizado para atualizar a interface com o status atual da conexão SignalR.
        /// </summary>
        public event Action<string> StatusSignalRAtualizado;

        /// <summary>
        /// Retorna o estado atual da conexão SignalR.
        /// </summary>
        public HubConnectionState EstadoConexao =>
            _connection?.State ?? HubConnectionState.Disconnected;

        /// <summary>
        /// Indica se a conexão SignalR está ativa e conectada.
        /// </summary>
        public bool EstaConectado =>
            _connection != null &&
            _connection.State == HubConnectionState.Connected;

        /// <summary>
        /// Retorna verdadeiro quando a conexão SignalR está operacional.
        /// </summary>
        public bool ConexaoSaudavel()
        {
            return _connection != null &&
                   _connection.State == HubConnectionState.Connected;
        }

        /// <summary>
        /// Estabelece conexão com o Hub SignalR configurado na aplicação MVC.
        /// Evita múltiplas conexões simultâneas e configura reconexão automática.
        /// </summary>
        public async Task ConectarAsync()
        {
            try
            {
                UltimoErro = string.Empty;

                LoggerService.Info("Iniciando conexao com o MVC via SignalR.");
                LoggerService.Info("URL SignalR: " + KinectConfig.UrlSignalR);

                if (string.IsNullOrWhiteSpace(KinectConfig.UrlSignalR))
                {
                    UltimoErro = "URL do SignalR nao configurada.";
                    StatusSignalRAtualizado?.Invoke("SignalR: URL nao configurada");
                    LoggerService.LogWarning(UltimoErro);
                    return;
                }

                if (_connection != null &&
                    _connection.State != HubConnectionState.Disconnected)
                {
                    LoggerService.Info("SignalR ja esta conectado, conectando ou reconectando.");
                    StatusSignalRAtualizado?.Invoke("SignalR: " + _connection.State);
                    return;
                }

                _connection = new HubConnectionBuilder()
                    .WithUrl(KinectConfig.UrlSignalR)
                    .WithAutomaticReconnect()
                    .Build();

                _connection.ServerTimeout = TimeSpan.FromSeconds(30);
                _connection.KeepAliveInterval = TimeSpan.FromSeconds(10);

                _connection.Reconnecting += error =>
                {
                    LoggerService.LogWarning("Reconectando ao MVC via SignalR.");

                    if (error != null)
                    {
                        LoggerService.Erro("Motivo da reconexao SignalR.", error);
                    }

                    StatusSignalRAtualizado?.Invoke("SignalR: Reconectando");
                    return Task.CompletedTask;
                };

                _connection.Reconnected += connectionId =>
                {
                    LoggerService.Info("Reconectado ao MVC via SignalR. ConnectionId: " + connectionId);
                    StatusSignalRAtualizado?.Invoke("SignalR: Reconectado");
                    return Task.CompletedTask;
                };

                _connection.Closed += error =>
                {
                    LoggerService.LogWarning("Conexao com MVC encerrada.");

                    if (error != null)
                    {
                        LoggerService.Erro("Motivo do encerramento SignalR.", error);
                    }

                    StatusSignalRAtualizado?.Invoke("SignalR: Desconectado");
                    return Task.CompletedTask;
                };

                await _connection.StartAsync();

                LoggerService.Info("Conectado ao MVC via SignalR.");
                StatusSignalRAtualizado?.Invoke("SignalR: Conectado");
            }
            catch (Exception ex)
            {
                UltimoErro = "Erro ao conectar ao MVC via SignalR.";

                LoggerService.Erro("Erro ao conectar ao MVC via SignalR.", ex);

                StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                throw;
            }
        }

        /// <summary>
        /// Envia o volume calculado pelo Kinect para a aplicação MVC via SignalR.
        /// </summary>
        /// <param name="volumeCm3">Volume calculado em centímetros cúbicos.</param>
        /// <returns>True quando o envio é concluído com sucesso.</returns>
        public async Task<bool> EnviarVolumeAsync(double volumeCm3)
        {
            try
            {
                UltimoErro = string.Empty;

                if (_connection == null)
                {
                    UltimoErro = "A conexao SignalR ainda nao foi inicializada.";

                    LoggerService.LogWarning("Volume nao enviado: conexao SignalR nula.");

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return false;
                }

                if (_connection.State != HubConnectionState.Connected)
                {
                    UltimoErro = "SignalR nao esta conectado. Estado atual: " + _connection.State;

                    LoggerService.LogWarning("Volume nao enviado: " + UltimoErro);

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return false;
                }

                await _connection.InvokeAsync("EnviarVolume", volumeCm3);

                LoggerService.Info($"Volume enviado ao MVC via SignalR: {volumeCm3:N0} cm³.");

                StatusSignalRAtualizado?.Invoke("SignalR: Volume enviado");

                return true;
            }
            catch (Exception ex)
            {
                UltimoErro = "Erro ao enviar volume ao MVC.";

                LoggerService.Erro("Erro ao enviar volume ao MVC.", ex);

                StatusSignalRAtualizado?.Invoke("SignalR: Falha no envio");

                return false;
            }
        }

        /// <summary>
        /// Envia uma mensagem de status operacional para a aplicação MVC via SignalR.
        /// </summary>
        /// <param name="status">Mensagem de status a ser enviada.</param>
        public async Task EnviarStatusAsync(string status)
        {
            try
            {
                UltimoErro = string.Empty;

                if (_connection == null)
                {
                    UltimoErro = "A conexao SignalR ainda nao foi inicializada.";

                    LoggerService.LogWarning("Status nao enviado: conexao SignalR nula.");

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return;
                }

                if (_connection.State != HubConnectionState.Connected)
                {
                    UltimoErro = "SignalR nao esta conectado. Estado atual: " + _connection.State;

                    LoggerService.LogWarning("Status nao enviado: " + UltimoErro);

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return;
                }

                await _connection.InvokeAsync("EnviarStatus", status);

                LoggerService.Info("Status enviado ao MVC via SignalR: " + status);
            }
            catch (Exception ex)
            {
                UltimoErro = "Erro ao enviar status ao MVC.";

                LoggerService.Erro("Erro ao enviar status ao MVC.", ex);

                StatusSignalRAtualizado?.Invoke("SignalR: Erro ao enviar status");
            }
        }

        /// <summary>
        /// Finaliza a conexão SignalR e libera os recursos utilizados.
        /// </summary>
        public async Task DesconectarAsync()
        {
            try
            {
                UltimoErro = string.Empty;

                if (_connection == null)
                {
                    return;
                }

                if (_connection.State != HubConnectionState.Disconnected)
                {
                    LoggerService.Info("Desconectando do MVC via SignalR.");
                    await _connection.StopAsync();
                }

                await _connection.DisposeAsync();
                _connection = null;

                LoggerService.Info("Desconectado do MVC.");
                StatusSignalRAtualizado?.Invoke("SignalR: Desconectado");
            }
            catch (Exception ex)
            {
                UltimoErro = "Erro ao desconectar do MVC.";

                LoggerService.Erro("Erro ao desconectar do MVC.", ex);

                StatusSignalRAtualizado?.Invoke("SignalR: Erro ao desconectar");
            }
        }
    }
}