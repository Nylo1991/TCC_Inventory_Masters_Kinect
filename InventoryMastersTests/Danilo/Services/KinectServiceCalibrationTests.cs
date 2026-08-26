using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Service;
using Xunit;


namespace TCC_Inventory_Masters_Kinect.Danilo.Tests.Service_Test_MVVM
{
    /// <summary>
    /// Testes da calibração, detecção do chão
    /// e cálculo do volume de referência.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class KinectServiceCalibrationTests
    {
        #region Calibração do ambiente

        /// <summary>
        /// Verifica se a calibração sem Kinect
        /// lança a exceção esperada.
        /// </summary>
        [Fact]
        public async Task Calibrar_SemKinect_LancaExcecao()
        {
            // Arrange
            var service = new KinectService();

            Func<Task> acao = async () =>
            {
                await service.CalibrateAsync(
                    CancellationToken.None);
            };

            // Act
            InvalidOperationException excecao =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    acao);

            // Assert
            Assert.Equal(
                "Kinect nao esta conectado para calibrar.",
                excecao.Message);
        }

        #endregion

        #region Detecção da referência do chão

        /// <summary>
        /// Verifica o comportamento quando não existem leituras.
        /// </summary>
        [Fact]
        public void DetectarChao_SemLeituras_RetornaZeros()
        {
            // Arrange
            var service = new KinectService();

            var leituras =
                new List<(int Angulo, double MediaDepth, int Pontos)>();

            // Act
            var resultado = InvocarDetectarChao(
                service,
                leituras);

            // Assert
            Assert.Equal(0d, resultado.DistanciaChaoMm);
            Assert.Equal(0, resultado.AnguloDetectado);
            Assert.Equal(0, resultado.TotalPontos);
        }

        /// <summary>
        /// Verifica se uma leitura com menos de mil
        /// pontos válidos é rejeitada.
        /// </summary>
        [Fact]
        public void DetectarChao_ComPoucosPontos_RetornaZeros()
        {
            // Arrange
            var service = new KinectService();

            var leituras =
                new List<(int Angulo, double MediaDepth, int Pontos)>
                {
                    (0, 1800, 999)
                };

            // Act
            var resultado = InvocarDetectarChao(
                service,
                leituras);

            // Assert
            Assert.Equal(0d, resultado.DistanciaChaoMm);
            Assert.Equal(0, resultado.AnguloDetectado);
            Assert.Equal(0, resultado.TotalPontos);
        }

        /// <summary>
        /// Verifica se a menor profundidade média
        /// válida é selecionada.
        /// </summary>
        [Fact]
        public void DetectarChao_ComDadosValidos_SelecionaMenorMedia()
        {
            // Arrange
            var service = new KinectService();

            var leituras =
                new List<(int Angulo, double MediaDepth, int Pontos)>
                {
                    (-20, 1700, 1200),
                    (0, 1800, 1400),
                    (15, 1900, 1300)
                };

            double distanciaEsperada =
                1700 * Math.Cos(20 * Math.PI / 180.0);

            // Act
            var resultado = InvocarDetectarChao(
                service,
                leituras);

            // Assert
            Assert.Equal(-20, resultado.AnguloDetectado);
            Assert.Equal(1200, resultado.TotalPontos);

            Assert.InRange(
                Math.Abs(
                    resultado.DistanciaChaoMm -
                    distanciaEsperada),
                0,
                0.001);
        }

        #endregion

        #region Cálculo do volume de referência

        /// <summary>
        /// Verifica se mapa nulo retorna zero.
        /// </summary>
        [Fact]
        public void VolumeReferencia_MapaNulo_RetornaZero()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double resultado = InvocarVolumeReferencia(
                service,
                null,
                40,
                40);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se dimensões inválidas retornam zero.
        /// </summary>
        [Fact]
        public void VolumeReferencia_DimensoesInvalidas_RetornaZero()
        {
            // Arrange
            var service = new KinectService();
            short[] mapa = new short[1];

            // Act
            double resultado = InvocarVolumeReferencia(
                service,
                mapa,
                0,
                0);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se um mapa menor que as dimensões
        /// informadas é rejeitado.
        /// </summary>
        [Fact]
        public void VolumeReferencia_MapaPequeno_RetornaZero()
        {
            // Arrange
            var service = new KinectService();
            short[] mapa = new short[1500];

            // Act
            double resultado = InvocarVolumeReferencia(
                service,
                mapa,
                40,
                40);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se um mapa com menos de mil
        /// pontos válidos é descartado.
        /// </summary>
        [Fact]
        public void VolumeReferencia_ComPoucosPontos_RetornaZero()
        {
            // Arrange
            var service = new KinectService();

            const int largura = 30;
            const int altura = 30;

            short[] mapa = CriarMapaDepth(
                largura,
                altura,
                2000);

            // A região útil possui 576 pontos:
            // 24 × 24, abaixo do mínimo de 1000.

            // Act
            double resultado = InvocarVolumeReferencia(
                service,
                mapa,
                largura,
                altura);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se um mapa válido produz
        /// um volume de referência positivo.
        /// </summary>
        [Fact]
        public void VolumeReferencia_MapaValido_RetornaVolumePositivo()
        {
            // Arrange
            var service = new KinectService();

            const int largura = 40;
            const int altura = 40;

            short[] mapa = CriarMapaDepth(
                largura,
                altura,
                2000);

            // A região útil possui 1024 pontos:
            // 32 × 32, acima do mínimo de 1000.

            // Act
            double resultado = InvocarVolumeReferencia(
                service,
                mapa,
                largura,
                altura);

            // Assert
            Assert.True(
                resultado > 0,
                "O volume de referência deveria ser positivo.");

            Assert.Equal(
                Math.Round(resultado, 0),
                resultado);
        }

        #endregion

        #region Captura de dados sem Kinect

        /// <summary>
        /// Verifica se a captura do mapa retorna false
        /// quando o Kinect não está conectado.
        /// </summary>
        [Fact]
        public void CapturarMapa_SemKinect_RetornaFalse()
        {
            // Arrange
            var service = new KinectService();

            // Act
            bool resultado = InvocarCapturarMapa(service);

            // Assert
            Assert.False(resultado);
        }

        /// <summary>
        /// Verifica se a média retorna zero
        /// quando o Kinect não está disponível.
        /// </summary>
        [Fact]
        public async Task CapturarMedia_SemKinect_RetornaZeros()
        {
            // Arrange
            var service = new KinectService();

            // Act
            var resultado = await InvocarCapturarMedia(service);

            // Assert
            Assert.Equal(0d, resultado.MediaDepth);
            Assert.Equal(0, resultado.TotalPontos);
        }

        #endregion

        #region Métodos auxiliares dos testes

        private static (
            double DistanciaChaoMm,
            int AnguloDetectado,
            int TotalPontos)
            InvocarDetectarChao(
                KinectService service,
                List<(int Angulo, double MediaDepth, int Pontos)> leituras)
        {
            MethodInfo metodo = ObterMetodoPrivado(
                "DetectarChao");

            object? retorno = metodo.Invoke(
                service,
                new object[] { leituras });

            Assert.NotNull(retorno);

            return ((
                double DistanciaChaoMm,
                int AnguloDetectado,
                int TotalPontos))retorno!;
        }

        private static double InvocarVolumeReferencia(
            KinectService service,
            short[]? mapa,
            int largura,
            int altura)
        {
            MethodInfo metodo = ObterMetodoPrivado(
                "CalcularVolumeReferenciaCm3");

            object? retorno = metodo.Invoke(
                service,
                new object?[]
                {
                    mapa,
                    largura,
                    altura
                });

            Assert.NotNull(retorno);

            return (double)retorno!;
        }

        private static bool InvocarCapturarMapa(
            KinectService service)
        {
            MethodInfo metodo = ObterMetodoPrivado(
                "CapturarMapaDepthCalibrado");

            object? retorno = metodo.Invoke(
                service,
                null);

            Assert.NotNull(retorno);

            return (bool)retorno!;
        }

        private static async Task<(
            double MediaDepth,
            int TotalPontos)> InvocarCapturarMedia(
                KinectService service)
        {
            MethodInfo metodo = ObterMetodoPrivado(
                "CapturarMediaDepthAsync");

            object? retorno = metodo.Invoke(
                service,
                new object[]
                {
                    1,
                    CancellationToken.None
                });

            Assert.NotNull(retorno);

            var tarefa = (Task<(
                double MediaDepth,
                int TotalPontos)>)retorno!;

            return await tarefa;
        }

        private static MethodInfo ObterMetodoPrivado(
            string nome)
        {
            MethodInfo? metodo = typeof(KinectService).GetMethod(
                nome,
                BindingFlags.Instance |
                BindingFlags.NonPublic);

            Assert.NotNull(metodo);

            return metodo!;
        }

        private static short[] CriarMapaDepth(
            int largura,
            int altura,
            int profundidadeMm)
        {
            short valorDepth =
                (short)(profundidadeMm << 3);

            short[] mapa =
                new short[largura * altura];

            for (int i = 0; i < mapa.Length; i++)
            {
                mapa[i] = valorDepth;
            }

            return mapa;
        }

        #endregion
    }
}

