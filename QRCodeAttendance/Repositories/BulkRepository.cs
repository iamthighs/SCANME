using QRCodeAttendance.Data;
using QRCodeAttendance.Models;

namespace QRCodeAttendance.Repositories
{
    public class BulkRepository
    {
        private readonly ApplicationDbContext _context;

        public BulkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void BulkInsertSamples(IEnumerable<Sample> samples)
        {
            _context.Samples.AddRange(samples);
            _context.SaveChanges();
        }

        
    }
}
