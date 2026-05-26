using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class SignalRService
    {
        private HubConnection _connection;

        public event Action<string> StatusSignalRAtualizado;

        public async Task ConectarAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(KinectConfig.UrlSignalR)
                .WithAutomaticReconnect()
                .Build();

            _connection.Reconnecting += error =>
            {
                StatusSignalRAtualizado?.Invoke(
                    "Reconectando ao MVC via SignalR...");

                return Task.CompletedTask;
            };

            _connection.Reconnected += connectionId =>
            {
                StatusSignalRAtualizado?.Invoke(
                    "Reconectado ao MVC via SignalR.");

                return Task.CompletedTask;
            };

            _connection.Closed += error =>
            {
                StatusSignalRAtualizado?.Invoke(
                    "Conexão com MVC encerrada.");

                return Task.CompletedTask;
            };

            await _connection.StartAsync();

            StatusSignalRAtualizado?.Invoke(
                "Conectado ao MVC via SignalR.");
        }

        public async Task EnviarVolumeAsync(double volumeCm3)
        {
            if (_connection != null &&
                _connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("EnviarVolume", volumeCm3);
            }
        }

        public async Task EnviarStatusAsync(string status)
        {
            if (_connection != null &&
                _connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("EnviarStatus", status);
            }
        }

        public async Task DesconectarAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();

                StatusSignalRAtualizado?.Invoke(
                    "Desconectado do MVC.");
            }
        }
    }
}