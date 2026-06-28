namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// Classe que representa o progresso da calibração do Kinect, incluindo informações
    /// sobre o ângulo atual, posição, passo atual, total de passos, status e porcentagem concluída.
    /// </summary>
    public class CalibrationProgress
    {
        public int CurrentAngle { get; set; }
        public string CurrentPosition { get; set; }
        public int Step { get; set; }
        public int TotalSteps { get; set; }
        public string Status { get; set; }
        public int Percentage { get; set; }
    }
}