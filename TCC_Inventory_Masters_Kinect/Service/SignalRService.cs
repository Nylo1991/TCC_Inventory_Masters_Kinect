using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class SignalRService
    {
        /// <summary>
        /// Conexão ativa com o Hub SignalR.
        /// </summary>
        private HubConnection _connection;

        /// <summary>
        /// Último erro registrado na comunicação SignalR.
        /// </summary>
        public string UltimoErro
        {
            get;
            private set;
        } =
        string.Empty;

        /// <summary>
        /// Evento utilizado para atualizar a interface
        /// sobre o status da conexão SignalR.
        /// </summary>
        public event Action<string> StatusSignalRAtualizado;

        /// <summary>
        /// Retorna o estado atual da conexão SignalR.
        /// </summary>
        public HubConnectionState EstadoConexao =>
            _connection?.State ?? HubConnectionState.Disconnected;

        /// <summary>
        /// Indica se a conexão está ativa.
        /// </summary>
        public bool EstaConectado =>
            _connection != null &&
            _connection.State == HubConnectionState.Connected;

        // ======================================
        // CONECTAR AO SIGNALR
        // ======================================

        public async Task ConectarAsync()
        {
            try
            {
                UltimoErro =
                    string.Empty;

                LoggerService.Info(
                    "Iniciando conexão com o MVC via SignalR.");

                LoggerService.Info(
                    "URL SignalR: " + KinectConfig.UrlSignalR);

                if (string.IsNullOrWhiteSpace(
                    KinectConfig.UrlSignalR))
                {
                    UltimoErro =
                        "URL do SignalR não configurada.";

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: URL não configurada");

                    return;
                }

                if (_connection != null &&
                    _connection.State == HubConnectionState.Connected)
                {
                    LoggerService.Info(
                        "SignalR já estava conectado.");

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Conectado");

                    return;
                }

                _connection =
                    new HubConnectionBuilder()
                    .WithUrl(KinectConfig.UrlSignalR)
                    .WithAutomaticReconnect()
                    .Build();

                _connection.Reconnecting += error =>
                {
                    LoggerService.Info(
                        "Reconectando ao MVC via SignalR.");

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Reconectando");

                    return Task.CompletedTask;
                };

                _connection.Reconnected += connectionId =>
                {
                    LoggerService.Info(
                        "Reconectado ao MVC via SignalR.");

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Reconectado");

                    return Task.CompletedTask;
                };

                _connection.Closed += error =>
                {
                    LoggerService.Info(
                        "Conexão com MVC encerrada.");

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Desconectado");

                    return Task.CompletedTask;
                };

                await _connection
                    .StartAsync();

                LoggerService.Info(
                    "Conectado ao MVC via SignalR.");

                StatusSignalRAtualizado?.Invoke(
                    "SignalR: Conectado");
            }
            catch (Exception ex)
            {
                UltimoErro =
                    ex.Message;

                if (ex.InnerException != null)
                {
                    UltimoErro +=
                        " | Detalhes: " +
                        ex.InnerException.Message;
                }

                LoggerService.Erro(
                    "Erro ao conectar ao MVC via SignalR.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "SignalR: Sem conexão");

                throw;
            }
        }

        // ======================================
        // ENVIAR VOLUME
        // ======================================

        public async Task<bool> EnviarVolumeAsync(
            double volumeCm3)
        {
            try
            {
                UltimoErro =
                    string.Empty;

                if (_connection == null)
                {
                    UltimoErro =
                        "A conexão SignalR ainda não foi inicializada.";

                    LoggerService.Info(
                        "Volume não enviado: conexão SignalR nula.");

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Sem conexão");

                    return false;
                }

                if (_connection.State !=
                    HubConnectionState.Connected)
                {
                    UltimoErro =
                        "SignalR não está conectado. Estado atual: " +
                        _connection.State;

                    LoggerService.Info(
                        "Volume não enviado: " + UltimoErro);

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Sem conexão");

                    return false;
                }

                await _connection
                    .InvokeAsync(
                        "EnviarVolume",
                        volumeCm3);

                LoggerService.Info(
                    $"Volume enviado ao MVC via SignalR: {volumeCm3:F2} cm³.");

                StatusSignalRAtualizado?.Invoke(
                    "SignalR: Enviado");

                return true;
            }
            catch (Exception ex)
            {
                UltimoErro =
                    ex.Message;

                if (ex.InnerException != null)
                {
                    UltimoErro +=
                        " | Detalhes: " +
                        ex.InnerException.Message;
                }

                LoggerService.Erro(
                    "Erro ao enviar volume ao MVC.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "SignalR: Falha no envio");

                return false;
            }
        }

        // ======================================
        // ENVIAR STATUS
        // ======================================

        public async Task EnviarStatusAsync(
            string status)
        {
            try
            {
                if (_connection == null)
                {
                    LoggerService.Info(
                        "Status não enviado: conexão SignalR nula.");

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Sem conexão");

                    return;
                }

                if (_connection.State !=
                    HubConnectionState.Connected)
                {
                    LoggerService.Info(
                        "Status não enviado: conexão SignalR não está ativa. Estado: " +
                        _connection.State);

                    StatusSignalRAtualizado?.Invoke(
                        "SignalR: Sem conexão");

                    return;
                }

                await _connection
                    .InvokeAsync(
                        "EnviarStatus",
                        status);

                LoggerService.Info(
                    "Status enviado ao MVC via SignalR: " + status);
            }
            catch (Exception ex)
            {
                UltimoErro =
                    ex.Message;

                if (ex.InnerException != null)
                {
                    UltimoErro +=
                        " | Detalhes: " +
                        ex.InnerException.Message;
                }

                LoggerService.Erro(
                    "Erro ao enviar status ao MVC.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "SignalR: Erro ao enviar status");
            }
        }

        // ======================================
        // DESCONECTAR
        // ======================================

        public async Task DesconectarAsync()
        {
            try
            {
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
                UltimoErro = ex.Message;
                LoggerService.Erro("Erro ao desconectar do MVC.", ex);
                StatusSignalRAtualizado?.Invoke("SignalR: Erro ao desconectar");
            }
        }

    }
}