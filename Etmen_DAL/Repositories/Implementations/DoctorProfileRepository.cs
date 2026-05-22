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
            // TODO: FirstOrDefaultAsync(d => d.UserId == userId).
            throw new NotImplementedException();
        }

        public async Task<DoctorProfile?> GetWithAppointmentsAsync(string userId)
        {
            // TODO: _dbSet.Include(d=>d.Appointments).FirstOrDefaultAsync(d=>d.UserId==userId).
            throw new NotImplementedException();
        }

        public async Task<DoctorProfile?> GetWithAvailableSlotsAsync(int doctorId)
        {
            // TODO: _dbSet.Include(d=>d.AvailableSlots).FirstOrDefaultAsync(d=>d.Id==doctorId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DoctorProfile>> GetAvailableDoctorsAsync()
        {
            // TODO: FindAsync(d => d.IsAvailable && d.IsVerified).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DoctorProfile>> GetBySpecializationAsync(string specialization)
        {
            // TODO: FindAsync(d => d.Specialization == specialization).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DoctorProfile>> SearchDoctorsAsync(string searchTerm)
        {
            // TODO: FindAsync(d => d.FullName.Contains(searchTerm) || d.Specialization.Contains(searchTerm)).
            throw new NotImplementedException();
        }

    }
}