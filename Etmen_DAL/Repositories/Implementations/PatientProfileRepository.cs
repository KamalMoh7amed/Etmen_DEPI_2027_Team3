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
            // TODO: FirstOrDefaultAsync(p => p.UserId == userId).
            throw new NotImplementedException();
        }

        public async Task<PatientProfile?> GetWithMedicalRecordsAsync(string userId)
        {
            // TODO: _dbSet.Include(p=>p.MedicalRecords).FirstOrDefaultAsync(p=>p.UserId==userId).
            throw new NotImplementedException();
        }

        public async Task<PatientProfile?> GetWithRiskAssessmentsAsync(string userId)
        {
            // TODO: _dbSet.Include(p=>p.RiskAssessments).FirstOrDefaultAsync(p=>p.UserId==userId).
            throw new NotImplementedException();
        }

        public async Task<PatientProfile?> GetWithAppointmentsAsync(string userId)
        {
            // TODO: _dbSet.Include(p=>p.Appointments).FirstOrDefaultAsync(p=>p.UserId==userId).
            throw new NotImplementedException();
        }

        public async Task<PatientProfile?> GetWithFamilyLinksAsync(int patientId)
        {
            // TODO: _dbSet.Include(p=>p.FamilyLinks).FirstOrDefaultAsync(p=>p.Id==patientId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PatientProfile>> GetFamilyMembersAsync(int patientId)
        {
            // TODO: Via FamilyLinks, get all linked PatientProfiles.
            throw new NotImplementedException();
        }

        public async Task<decimal?> GetLatestBmiAsync(string userId)
        {
            // TODO: Get latest MedicalRecord for userId, return BMI field.
            throw new NotImplementedException();
        }

    }
}