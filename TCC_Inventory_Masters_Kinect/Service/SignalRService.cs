using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class SignalRService
    {
        private HubConnection _connection;

        public async Task ConectarAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(KinectConfig.UrlSignalR)
                .WithAutomaticReconnect()
                .Build();

            await _connection.StartAsync();
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
            }
        }
    }
}