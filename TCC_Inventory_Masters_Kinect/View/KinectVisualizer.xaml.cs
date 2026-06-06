using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectVisualizer : Page
    {
        private KinectService _service;
        private PointsVisual3D _pontosVisual;

        public KinectVisualizer(KinectService service)
        {
            InitializeComponent();
            _service = service;

            // Configuração do Visual 3D
            _pontosVisual = new PointsVisual3D { Size = 2, Color = Colors.DeepSkyBlue };
            viewPort.Children.Add(_pontosVisual);

            // Assinatura do evento
            _service.PointCloudAtualizada += AtualizarPontos;
        }

        public void AtualizarPontos(List<Point3DData> pontos)
        {
            Dispatcher.Invoke(() =>
            {
                var collection = new Point3DCollection();

                foreach (var p in pontos)
                {
                    // Adiciona o ponto na coleção
                    collection.Add(new Point3D(p.X, p.Y, p.Z));
                }

                // Aplica a coleção ao objeto que criamos no XAML
                pontosKinect.Points = collection;

                // Ajusta a câmera para enquadrar os novos pontos
                viewPort.ZoomExtents();
            });
        }
    }
}