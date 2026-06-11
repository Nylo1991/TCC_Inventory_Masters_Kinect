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

       
        public async Task<CalibrationResult> CalibrateAndSaveAsync(
            string spaceName,
            IProgress<CalibrationProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
        
            var result = await _kinectService.CalibrateAsync(cancellationToken, progress);

            
            var space = new Space
            {
                Name = spaceName,
                MaxVolume = result.MaxVolume,
                CalibratedAt = DateTime.Now   
            };

            _context.Spaces.Add(space);
            await _context.SaveChangesAsync(cancellationToken);

            return result;
        }

      
        public async Task<Space> GetLastCalibratedSpaceAsync()
        {
            return await _context.Spaces
                .OrderByDescending(s => s.CalibratedAt)
                .FirstOrDefaultAsync();
        }
    }
}
