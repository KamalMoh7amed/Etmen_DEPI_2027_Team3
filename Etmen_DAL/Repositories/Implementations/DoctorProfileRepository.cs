using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class DoctorProfileRepository : GenericRepository<DoctorProfile>, IDoctorProfileRepository
    {
        public DoctorProfileRepository(EtmenDbContext context) : base(context) { }

        public async Task<DoctorProfile?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.ApplicationUserId == userId);
        }

        public async Task<DoctorProfile?> GetWithAppointmentsAsync(string userId)
        {
            return await _dbSet.Include(d => d.Appointments).FirstOrDefaultAsync(d => d.ApplicationUserId == userId);
        }

        public async Task<DoctorProfile?> GetWithAvailableSlotsAsync(int doctorId)
        {
            return await _dbSet.Include(d => d.AvailableSlots).FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<IEnumerable<DoctorProfile>> GetAvailableDoctorsAsync()
        {
            return await FindAsync(d => d.IsAvailable);
        }

        public async Task<IEnumerable<DoctorProfile>> GetBySpecializationAsync(string specialization)
        {
            return await FindAsync(d => d.Specialization == specialization);
        }

        public async Task<IEnumerable<DoctorProfile>> SearchDoctorsAsync(string searchTerm)
        {
            return await FindAsync(d => (d.FullName != null && d.FullName.Contains(searchTerm)) || (d.Specialization != null && d.Specialization.Contains(searchTerm)));
        }

    }
}
