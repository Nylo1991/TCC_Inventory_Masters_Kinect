using System;
using System.Collections.Generic;
using System.Reflection;
using TCC_Inventory_Masters_Kinect.Service;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Danilo.Tests.Service_Test_MVVM
{
    /// <summary>
    /// Testes dos cálculos e da estabilização de volume
    /// realizados pelo KinectService.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class KinectServiceVolumeTests
    {
        #region CalcularVolumeAtualCm3

        /// <summary>
        /// Verifica se o cálculo retorna zero
        /// quando o Kinect não está conectado.
        /// </summary>
        [Fact]
        public void CalcularVolumeAtualCm3_SemKinectConectado_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double volume = service.CalcularVolumeAtualCm3();

            // Assert
            Assert.Equal(0d, volume);
        }

        /// <summary>
        /// Verifica se a tentativa de calcular o volume
        /// sem Kinect não lança uma exceção.
        /// </summary>
        [Fact]
        public void CalcularVolumeAtualCm3_SemKinectConectado_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new KinectService();

            // Act
            Exception excecao = Record.Exception(() =>
            {
                service.CalcularVolumeAtualCm3();
            });

            // Assert
            Assert.Null(excecao);
        }

        #endregion

        #region EstabilizarVolume

        /// <summary>
        /// Verifica se volume zero retorna zero.
        /// </summary>
        [Fact]
        public void EstabilizarVolume_VolumeZero_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double resultado = InvocarEstabilizarVolume(service, 0);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se volume negativo retorna zero.
        /// </summary>
        [Fact]
        public void EstabilizarVolume_VolumeNegativo_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double resultado = InvocarEstabilizarVolume(service, -500);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se o primeiro volume válido é utilizado
        /// como valor inicial e arredondado.
        /// </summary>
        [Fact]
        public void EstabilizarVolume_PrimeiroVolumeValido_DeveRetornarValorArredondado()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double resultado = InvocarEstabilizarVolume(service, 1250.4);

            // Assert
            Assert.Equal(1250d, resultado);
        }

        /// <summary>
        /// Verifica a aplicação da média histórica
        /// e da suavização ponderada.
        /// </summary>
        [Fact]
        public void EstabilizarVolume_DoisVolumes_DeveAplicarSuavizacao()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double primeiroResultado =
                InvocarEstabilizarVolume(service, 1000);

            double segundoResultado =
                InvocarEstabilizarVolume(service, 2000);

            // Média histórica: (1000 + 2000) / 2 = 1500
            // Suavização: (1000 × 0,35) + (1500 × 0,65) = 1325

            // Assert
            Assert.Equal(1000d, primeiroResultado);
            Assert.Equal(1325d, segundoResultado);
        }

        /// <summary>
        /// Verifica se o histórico curto permite resposta rápida
        /// durante o monitoramento em tempo real.
        /// </summary>
        [Fact]
        public void EstabilizarVolume_MaisDeTresLeituras_DeveLimitarHistorico()
        {
            // Arrange
            var service = new KinectService();

            // Act
            for (int volume = 1; volume <= 15; volume++)
            {
                InvocarEstabilizarVolume(service, volume * 100);
            }

            Queue<double> historico = ObterHistoricoVolumes(service);

            // Assert
            Assert.Equal(3, historico.Count);
        }

        #endregion

        #region CalcularVolumeRealCm3

        /// <summary>
        /// Verifica se mapas nulos retornam zero.
        /// </summary>
        [Fact]
        public void CalcularVolumeRealCm3_MapasNulos_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();

            // Act
            double resultado = InvocarCalcularVolumeReal(
                service,
                null,
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
        public void CalcularVolumeRealCm3_DimensoesInvalidas_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();
            short[] mapaCalibrado = new short[1];
            short[] mapaAtual = new short[1];

            // Act
            double resultado = InvocarCalcularVolumeReal(
                service,
                mapaCalibrado,
                mapaAtual,
                0,
                0);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se mapas com tamanhos diferentes
        /// são rejeitados.
        /// </summary>
        [Fact]
        public void CalcularVolumeRealCm3_MapasComTamanhosDiferentes_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();
            short[] mapaCalibrado = new short[1600];
            short[] mapaAtual = new short[1500];

            // Act
            double resultado = InvocarCalcularVolumeReal(
                service,
                mapaCalibrado,
                mapaAtual,
                40,
                40);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se uma leitura com menos de mil pontos
        /// válidos é descartada.
        /// </summary>
        [Fact]
        public void CalcularVolumeRealCm3_PoucosPontosValidos_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();

            const int largura = 30;
            const int altura = 30;

            short[] mapaCalibrado =
                CriarMapaDepth(largura, altura, 2000);

            short[] mapaAtual =
                CriarMapaDepth(largura, altura, 1900);

            // A área útil possui somente 576 pontos:
            // 24 × 24, abaixo do mínimo de 1000.

            // Act
            double resultado = InvocarCalcularVolumeReal(
                service,
                mapaCalibrado,
                mapaAtual,
                largura,
                altura);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se diferenças menores que a altura
        /// mínima do objeto são descartadas.
        /// </summary>
        [Fact]
        public void CalcularVolumeRealCm3_AlturaAbaixoDoMinimo_DeveRetornarZero()
        {
            // Arrange
            var service = new KinectService();

            const int largura = 40;
            const int altura = 40;

            short[] mapaCalibrado =
                CriarMapaDepth(largura, altura, 2000);

            short[] mapaAtual =
                CriarMapaDepth(largura, altura, 1980);

            // A diferença é de apenas 20 mm.
            // O mínimo permitido é 30 mm.

            // Act
            double resultado = InvocarCalcularVolumeReal(
                service,
                mapaCalibrado,
                mapaAtual,
                largura,
                altura);

            // Assert
            Assert.Equal(0d, resultado);
        }

        /// <summary>
        /// Verifica se mapas válidos produzem
        /// um volume maior que zero.
        /// </summary>
        [Fact]
        public void CalcularVolumeRealCm3_DadosValidos_DeveRetornarVolumePositivo()
        {
            // Arrange
            var service = new KinectService();

            const int largura = 40;
            const int altura = 40;

            short[] mapaCalibrado =
                CriarMapaDepth(largura, altura, 2000);

            short[] mapaAtual =
                CriarMapaDepth(largura, altura, 1900);

            // A área útil possui 1024 pontos:
            // 32 × 32, acima do mínimo de 1000.

            // Act
            double resultado = InvocarCalcularVolumeReal(
                service,
                mapaCalibrado,
                mapaAtual,
                largura,
                altura);

            // Assert
            Assert.True(
                resultado > 0,
                "O volume calculado deveria ser maior que zero.");
        }

        [Fact]
        public void CalcularVolumeRealCm3_AreaTotalOcupada_DeveAlcancarVolumeMaximoDetectavel()
        {
            var service = new KinectService();
            const int largura = 40;
            const int altura = 40;
            short[] mapaCalibrado = CriarMapaDepth(largura, altura, 2000);
            short[] mapaOcupado = CriarMapaDepth(largura, altura, 800);

            double volumeAtual = InvocarCalcularVolumeReal(
                service,
                mapaCalibrado,
                mapaOcupado,
                largura,
                altura);
            double volumeMaximo = InvocarCalcularVolumeReferencia(
                service,
                mapaCalibrado,
                largura,
                altura);

            Assert.True(volumeMaximo > 0);
            Assert.InRange(volumeAtual / volumeMaximo, 0.99, 1.01);
        }

        #endregion

        #region Métodos auxiliares

        /// <summary>
        /// Invoca o método privado EstabilizarVolume.
        /// </summary>
        private static double InvocarEstabilizarVolume(
            KinectService service,
            double volume)
        {
            MethodInfo? metodo = typeof(KinectService).GetMethod(
                "EstabilizarVolume",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(metodo);

            object? resultado = metodo!.Invoke(
                service,
                new object[] { volume });

            Assert.NotNull(resultado);

            return (double)resultado!;
        }

        /// <summary>
        /// Invoca o método privado CalcularVolumeRealCm3.
        /// </summary>
        private static double InvocarCalcularVolumeReal(
            KinectService service,
            short[]? mapaCalibrado,
            short[]? mapaAtual,
            int largura,
            int altura)
        {
            MethodInfo? metodo = typeof(KinectService).GetMethod(
                "CalcularVolumeRealCm3",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(metodo);

            object? resultado = metodo!.Invoke(
                service,
                new object?[]
                {
                    mapaCalibrado,
                    mapaAtual,
                    largura,
                    altura
                });

            Assert.NotNull(resultado);

            return (double)resultado!;
        }

        private static double InvocarCalcularVolumeReferencia(
            KinectService service,
            short[] mapaCalibrado,
            int largura,
            int altura)
        {
            MethodInfo? metodo = typeof(KinectService).GetMethod(
                "CalcularVolumeReferenciaCm3",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(metodo);

            object? resultado = metodo!.Invoke(
                service,
                new object[] { mapaCalibrado, largura, altura });

            Assert.NotNull(resultado);
            return (double)resultado!;
        }

        /// <summary>
        /// Obtém o histórico privado de volumes.
        /// </summary>
        private static Queue<double> ObterHistoricoVolumes(
            KinectService service)
        {
            FieldInfo? campo = typeof(KinectService).GetField(
                "_historicoVolumes",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(campo);

            object? valor = campo!.GetValue(service);

            Assert.NotNull(valor);

            return (Queue<double>)valor!;
        }

        /// <summary>
        /// Cria um mapa simulando os dados brutos
        /// fornecidos pelo Kinect.
        /// </summary>
        private static short[] CriarMapaDepth(
            int largura,
            int altura,
            int profundidadeMm)
        {
            short valorDepth = (short)(profundidadeMm << 3);
            short[] mapa = new short[largura * altura];

            for (int i = 0; i < mapa.Length; i++)
            {
                mapa[i] = valorDepth;
            }

            return mapa;
        }

        #endregion
    }
}
