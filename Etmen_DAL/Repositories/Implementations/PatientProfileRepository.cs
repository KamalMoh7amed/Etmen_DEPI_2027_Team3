using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class PatientProfileRepository : GenericRepository<PatientProfile>, IPatientProfileRepository
    {
        public PatientProfileRepository(EtmenDbContext context) : base(context) { }

        public async Task<PatientProfile?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        }

        public async Task<PatientProfile?> GetWithMedicalRecordsAsync(string userId)
        {
            return await _dbSet
                .Include(p => p.MedicalRecords)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        }

        public async Task<PatientProfile?> GetWithRiskAssessmentsAsync(string userId)
        {
            return await _dbSet
                .Include(p => p.RiskAssessments)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        }

        public async Task<PatientProfile?> GetWithAppointmentsAsync(string userId)
        {
            return await _dbSet
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
        }

        public async Task<PatientProfile?> GetWithFamilyLinksAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.PrimaryLinks)
                .Include(p => p.LinkedLinks)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<IEnumerable<PatientProfile>> GetFamilyMembersAsync(int patientId)
        {
            var profile = await _dbSet
                .Include(p => p.PrimaryLinks)
                .ThenInclude(f => f.LinkedPatient)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            return profile?.PrimaryLinks.Select(f => f.LinkedPatient) ?? new List<PatientProfile>();
        }

        public async Task<decimal?> GetLatestBmiAsync(string userId)
        {
            var profile = await GetByUserIdAsync(userId);
            if (profile == null)
                return null;

            // BMI is calculated property in PatientProfile
            return profile.BMI > 0 ? profile.BMI : null;
        }
    }
}
