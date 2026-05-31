using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class SignalRService
    {
        // ==========================================
        // CONEXÃO SIGNALR
        // ==========================================

        private HubConnection _connection;

        // ==========================================
        // EVENTOS
        // ==========================================

        public event Action<string>
            StatusSignalRAtualizado;

        // ==========================================
        // CONECTAR AO MVC
        // ==========================================

        public async Task ConectarAsync()
        {
            try
            {
                LoggerService.Info(
                    "Iniciando conexão com o MVC via SignalR.");

                _connection =
                    new HubConnectionBuilder()
                    .WithUrl(
                        KinectConfig.UrlSignalR)
                    .WithAutomaticReconnect()
                    .Build();

                // ==========================================
                // EVENTO: RECONECTANDO
                // ==========================================

                _connection.Reconnecting += error =>
                {
                    LoggerService.Info(
                        "Reconectando ao MVC via SignalR.");

                    StatusSignalRAtualizado?.Invoke(
                        "Reconectando ao MVC via SignalR...");

                    return Task.CompletedTask;
                };

                // ==========================================
                // EVENTO: RECONECTADO
                // ==========================================

                _connection.Reconnected += connectionId =>
                {
                    LoggerService.Info(
                        "Reconectado ao MVC via SignalR.");

                    StatusSignalRAtualizado?.Invoke(
                        "Reconectado ao MVC via SignalR.");

                    return Task.CompletedTask;
                };

                // ==========================================
                // EVENTO: CONEXÃO ENCERRADA
                // ==========================================

                _connection.Closed += error =>
                {
                    LoggerService.Info(
                        "Conexão com MVC encerrada.");

                    StatusSignalRAtualizado?.Invoke(
                        "Conexão com MVC encerrada.");

                    return Task.CompletedTask;
                };

                await _connection
                    .StartAsync();

                LoggerService.Info(
                    "Conectado ao MVC via SignalR.");

                StatusSignalRAtualizado?.Invoke(
                    "Conectado ao MVC via SignalR.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao conectar ao MVC via SignalR.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "Erro ao conectar ao MVC via SignalR: " + ex.Message);

                throw;
            }
        }

        // ==========================================
        // ENVIAR VOLUME
        // ==========================================

        public async Task EnviarVolumeAsync(
            double volumeCm3)
        {
            try
            {
                if (_connection != null &&
                    _connection.State ==
                    HubConnectionState.Connected)
                {
                    await _connection
                        .InvokeAsync(
                            "EnviarVolume",
                            volumeCm3);

                    LoggerService.Info(
                        $"Volume enviado ao MVC via SignalR: {volumeCm3:F2} cm³.");
                }
                else
                {
                    LoggerService.Info(
                        "Volume não enviado: conexão SignalR não está ativa.");

                    StatusSignalRAtualizado?.Invoke(
                        "Volume não enviado: sem conexão com MVC.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao enviar volume ao MVC.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "Erro ao enviar volume ao MVC: " + ex.Message);

                throw;
            }
        }

        // ==========================================
        // ENVIAR STATUS
        // ==========================================

        public async Task EnviarStatusAsync(
            string status)
        {
            try
            {
                if (_connection != null &&
                    _connection.State ==
                    HubConnectionState.Connected)
                {
                    await _connection
                        .InvokeAsync(
                            "EnviarStatus",
                            status);

                    LoggerService.Info(
                        "Status enviado ao MVC via SignalR: " + status);
                }
                else
                {
                    LoggerService.Info(
                        "Status não enviado: conexão SignalR não está ativa.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao enviar status ao MVC.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "Erro ao enviar status ao MVC: " + ex.Message);

                throw;
            }
        }

        // ==========================================
        // DESCONECTAR DO MVC
        // ==========================================

        public async Task DesconectarAsync()
        {
            try
            {
                if (_connection != null)
                {
                    LoggerService.Info(
                        "Desconectando do MVC via SignalR.");

                    if (_connection.State !=
                        HubConnectionState.Disconnected)
                    {
                        await _connection
                            .StopAsync();
                    }

                    await _connection
                        .DisposeAsync();

                    _connection =
                        null;

                    LoggerService.Info(
                        "Desconectado do MVC.");

                    StatusSignalRAtualizado?.Invoke(
                        "Desconectado do MVC.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao desconectar do MVC.",
                    ex);

                StatusSignalRAtualizado?.Invoke(
                    "Erro ao desconectar do MVC: " + ex.Message);
            }
        }
    }
}