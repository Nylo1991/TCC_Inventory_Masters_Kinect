using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class SignalRService
    {
        private HubConnection _connection;

        public string UltimoErro { get; private set; } = string.Empty;

        public event Action<string> StatusSignalRAtualizado;

        public HubConnectionState EstadoConexao =>
            _connection?.State ?? HubConnectionState.Disconnected;

        public bool EstaConectado =>
            _connection != null &&
            _connection.State == HubConnectionState.Connected;

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
                    return;
                }

                if (_connection != null &&
                    _connection.State == HubConnectionState.Connected)
                {
                    LoggerService.Info("SignalR ja estava conectado.");
                    StatusSignalRAtualizado?.Invoke("SignalR: Conectado");
                    return;
                }

                _connection = new HubConnectionBuilder()
                    .WithUrl(KinectConfig.UrlSignalR)
                    .WithAutomaticReconnect()
                    .Build();

                _connection.Reconnecting += _ =>
                {
                    LoggerService.Info("Reconectando ao MVC via SignalR.");
                    StatusSignalRAtualizado?.Invoke("SignalR: Reconectando");
                    return Task.CompletedTask;
                };

                _connection.Reconnected += _ =>
                {
                    LoggerService.Info("Reconectado ao MVC via SignalR.");
                    StatusSignalRAtualizado?.Invoke("SignalR: Reconectado");
                    return Task.CompletedTask;
                };

                _connection.Closed += _ =>
                {
                    LoggerService.Info("Conexao com MVC encerrada.");
                    StatusSignalRAtualizado?.Invoke("SignalR: Desconectado");
                    return Task.CompletedTask;
                };

                await _connection.StartAsync();

                LoggerService.Info("Conectado ao MVC via SignalR.");
                StatusSignalRAtualizado?.Invoke("SignalR: Conectado");
            }
            catch
            {
                UltimoErro = "Erro ao conectar ao MVC via SignalR.";

                LoggerService.Erro("Erro ao conectar ao MVC via SignalR.");

                StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                throw;
            }
        }

        public async Task<bool> EnviarVolumeAsync(double volumeCm3)
        {
            try
            {
                UltimoErro = string.Empty;

                if (_connection == null)
                {
                    UltimoErro = "A conexao SignalR ainda nao foi inicializada.";

                    LoggerService.Info("Volume nao enviado: conexao SignalR nula.");

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return false;
                }

                if (_connection.State != HubConnectionState.Connected)
                {
                    UltimoErro = "SignalR nao esta conectado. Estado atual: " + _connection.State;

                    LoggerService.Info("Volume nao enviado: " + UltimoErro);

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return false;
                }

                await _connection.InvokeAsync("EnviarVolume", volumeCm3);

                LoggerService.Info($"Volume enviado ao MVC via SignalR: {volumeCm3:F2} cm3.");

                StatusSignalRAtualizado?.Invoke("SignalR: Enviado");

                return true;
            }
            catch
            {
                UltimoErro = "Erro ao enviar volume ao MVC.";

                LoggerService.Erro("Erro ao enviar volume ao MVC.");

                StatusSignalRAtualizado?.Invoke("SignalR: Falha no envio");

                return false;
            }
        }

        public async Task EnviarStatusAsync(string status)
        {
            try
            {
                if (_connection == null)
                {
                    LoggerService.Info("Status nao enviado: conexao SignalR nula.");

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return;
                }

                if (_connection.State != HubConnectionState.Connected)
                {
                    LoggerService.Info("Status nao enviado: conexao SignalR nao esta ativa. Estado: " + _connection.State);

                    StatusSignalRAtualizado?.Invoke("SignalR: Sem conexao");

                    return;
                }

                await _connection.InvokeAsync("EnviarStatus", status);

                LoggerService.Info("Status enviado ao MVC via SignalR: " + status);
            }
            catch
            {
                UltimoErro = "Erro ao enviar status ao MVC.";

                LoggerService.Erro("Erro ao enviar status ao MVC.");

                StatusSignalRAtualizado?.Invoke("SignalR: Erro ao enviar status");
            }
        }

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
            catch
            {
                UltimoErro = "Erro ao desconectar do MVC.";

                LoggerService.Erro("Erro ao desconectar do MVC.");

                StatusSignalRAtualizado?.Invoke("SignalR: Erro ao desconectar");
            }
        }
    }
}