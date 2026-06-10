using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectCalibrationService
    {
        private readonly KinectService _kinectService;
        private readonly AppDbContext _context;

        public KinectCalibrationService(KinectService kinectService, AppDbContext context)
        {
            _kinectService = kinectService;
            _context = context;
        }

        /// <summary>
        /// Realiza a calibração completa e salva o resultado no banco de dados
        /// </summary>
        public async Task<CalibrationResult> CalibrateAndSaveAsync(
            string spaceName,
            IProgress<CalibrationProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            // Chama o método com a ordem correta de parâmetros
            var result = await _kinectService.CalibrateAsync(cancellationToken, progress);

            // Cria e salva o espaço calibrado
            var space = new Space
            {
                Name = spaceName,
                MaxVolume = result.MaxVolume,
                CalibratedAt = DateTime.Now   // Define a data da calibração
            };

            _context.Spaces.Add(space);
            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// Retorna o último espaço calibrado (caso exista)
        /// </summary>
        public async Task<Space> GetLastCalibratedSpaceAsync()
        {
            return await _context.Spaces
                .OrderByDescending(s => s.CalibratedAt)
                .FirstOrDefaultAsync();
        }
    }
}
