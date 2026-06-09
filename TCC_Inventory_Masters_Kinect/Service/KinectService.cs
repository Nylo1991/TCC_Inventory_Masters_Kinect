// KinectService.cs
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class KinectService : IDisposable
{
    private KinectSensor _sensor;
    private DepthFrameReader _depthReader;
    private ushort[] _depthData;
    private bool _isRunning;

    // Posicoes fixas de inclinacao para calibracao
    private readonly int[] _calibrationAngles = { 27, 0, -27 };

    public bool IsConnected => _sensor != null && _sensor.IsAvailable;

    public KinectService()
    {
        Initialize();
    }

    private void Initialize()
    {
        _sensor = KinectSensor.GetDefault();

        if (_sensor == null)
            throw new InvalidOperationException("Nenhum Kinect encontrado.");

        _depthReader = _sensor.DepthFrameSource.OpenReader();
        _sensor.Open();
        _isRunning = true;
    }

    // -------------------------------------------------------
    // CALIBRACAO PRINCIPAL
    // -------------------------------------------------------
    public async Task<CalibrationResult> CalibrateAsync(
        IProgress<CalibrationProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        var allPoints = new List<CameraSpacePoint>();
        int totalAngles = _calibrationAngles.Length;

        for (int i = 0; i < totalAngles; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int angle = _calibrationAngles[i];
            string positionName = GetPositionName(angle);

            // Reporta progresso
            progress?.Report(new CalibrationProgress
            {
                CurrentAngle = angle,
                CurrentPosition = positionName,
                Step = i + 1,
                TotalSteps = totalAngles,
                Status = $"Movendo para posicao {positionName}...",
                Percentage = (int)((i / (double)totalAngles) * 100)
            });

            // Move o Kinect para o angulo fixo
            await MoveToAngleAsync(angle, cancellationToken);

            // Aguarda estabilizacao da imagem
            progress?.Report(new CalibrationProgress
            {
                CurrentAngle = angle,
                CurrentPosition = positionName,
                Step = i + 1,
                TotalSteps = totalAngles,
                Status = $"Estabilizando na posicao {positionName}...",
                Percentage = (int)((i / (double)totalAngles) * 100)
            });

            await Task.Delay(1500, cancellationToken); // Aguarda estabilizar

            // Captura pontos nessa posicao
            progress?.Report(new CalibrationProgress
            {
                CurrentAngle = angle,
                CurrentPosition = positionName,
                Step = i + 1,
                TotalSteps = totalAngles,
                Status = $"Capturando pontos na posicao {positionName}...",
                Percentage = (int)(((i + 0.5) / (double)totalAngles) * 100)
            });

            var points = await CapturePointsAsync(cancellationToken);
            allPoints.AddRange(points);
        }

        // Volta para posicao neutra
        await MoveToAngleAsync(0, cancellationToken);

        // Calcula o volume maximo com todos os pontos combinados
        progress?.Report(new CalibrationProgress
        {
            Step = totalAngles,
            TotalSteps = totalAngles,
            Status = "Calculando volume maximo...",
            Percentage = 95
        });

        double maxVolume = CalculateVolume(allPoints);

        progress?.Report(new CalibrationProgress
        {
            Step = totalAngles,
            TotalSteps = totalAngles,
            Status = "Calibracao concluida!",
            Percentage = 100
        });

        return new CalibrationResult
        {
            MaxVolume = maxVolume,
            TotalPointsFound = allPoints.Count,
            CalibratedAt = DateTime.UtcNow
        };
    }

    // -------------------------------------------------------
    // MOVIMENTO DE INCLINACAO
    // -------------------------------------------------------
    private async Task MoveToAngleAsync(int targetAngle, CancellationToken cancellationToken)
    {
        if (_sensor == null) return;

        int currentAngle = _sensor.ElevationAngle;
        int step = currentAngle < targetAngle ? 2 : -2;

        while (currentAngle != targetAngle)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currentAngle += step;

            // Garante que nao ultrapasse o alvo
            if (step > 0 && currentAngle > targetAngle) currentAngle = targetAngle;
            if (step < 0 && currentAngle < targetAngle) currentAngle = targetAngle;

            // ElevationAngle so aceita valores entre -27 e +27
            currentAngle = Math.Clamp(currentAngle, -27, 27);

            _sensor.ElevationAngle = currentAngle;

            // Pausa entre cada passo para nao forcar o motor
            await Task.Delay(150, cancellationToken);
        }
    }

    // -------------------------------------------------------
    // CAPTURA DE PONTOS
    // -------------------------------------------------------
    private async Task<List<CameraSpacePoint>> CapturePointsAsync(
        CancellationToken cancellationToken)
    {
        var points = new List<CameraSpacePoint>();
        var tcs = new TaskCompletionSource<List<CameraSpacePoint>>();
        int captures = 0;
        int maxFrames = 5; // Captura 5 frames por posicao

        void OnFrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using var frame = e.FrameReference.AcquireFrame();
            if (frame == null) return;

            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;

            _depthData = new ushort[width * height];
            frame.CopyFrameDataToArray(_depthData);

            var framePoints = ConvertDepthToPoints(_depthData, width, height);
            points.AddRange(framePoints);

            captures++;
            if (captures >= maxFrames)
            {
                _depthReader.FrameArrived -= OnFrameArrived;
                tcs.TrySetResult(points);
            }
        }

        _depthReader.FrameArrived += OnFrameArrived;

        using (cancellationToken.Register(() =>
        {
            _depthReader.FrameArrived -= OnFrameArrived;
            tcs.TrySetCanceled();
        }))
        {
            return await tcs.Task;
        }
    }

    // -------------------------------------------------------
    // CONVERSAO DE PONTOS
    // -------------------------------------------------------
    private List<CameraSpacePoint> ConvertDepthToPoints(
        ushort[] depthData, int width, int height)
    {
        var mapper = _sensor.CoordinateMapper;
        var spacePoints = new CameraSpacePoint[depthData.Length];

        mapper.MapDepthFrameToCameraSpace(depthData, spacePoints);

        return spacePoints
            .Where(p => !float.IsInfinity(p.X)
                     && !float.IsInfinity(p.Y)
                     && !float.IsInfinity(p.Z)
                     && p.Z > 0)
            .ToList();
    }

    // -------------------------------------------------------
    // CALCULO DE VOLUME
    // -------------------------------------------------------
    private double CalculateVolume(List<CameraSpacePoint> points)
    {
        if (!points.Any()) return 0;

        float minX = points.Min(p => p.X);
        float maxX = points.Max(p => p.X);
        float minY = points.Min(p => p.Y);
        float maxY = points.Max(p => p.Y);
        float minZ = points.Min(p => p.Z);
        float maxZ = points.Max(p => p.Z);

        double width = maxX - minX;
        double height = maxY - minY;
        double depth = maxZ - minZ;

        // Volume em metros cubicos convertido para litros (1 m3 = 1000 L)
        return width * height * depth * 1000;
    }

    // -------------------------------------------------------
    // MONITORAMENTO CONTINUO (Historico)
    // -------------------------------------------------------
    public async Task<double> MeasureCurrentVolumeAsync(
        CancellationToken cancellationToken = default)
    {
        var points = await CapturePointsAsync(cancellationToken);
        return CalculateVolume(points);
    }

    // -------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------
    private string GetPositionName(int angle)
    {
        return angle switch
        {
            > 0 => "Cima",
            0 => "Meio",
            < 0 => "Baixo",
            _ => "Desconhecida"
        };
    }

    public void Dispose()
    {
        _depthReader?.Dispose();
        _sensor?.Close();
        _isRunning = false;
    }
}
