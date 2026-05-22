using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            // TODO: FindAsync(r => r.PatientProfileId == patientId) ordered by RecordDate desc.
            throw new NotImplementedException();
        }

        public async Task<MedicalRecord?> GetLatestByPatientIdAsync(int patientId)
        {
            // TODO: FirstOrDefaultAsync(r => r.PatientProfileId==patientId) order by RecordDate desc.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDateRangeAsync(int patientId, DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync with date range filter.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MedicalRecord>> GetWithAbnormalValuesAsync(int patientId)
        {
            // TODO: FindAsync(r => r.PatientProfileId==patientId && r.HasAbnormalValues).
            throw new NotImplementedException();
        }

        public async Task AddRecordWithSymptomsAsync(MedicalRecord record, IEnumerable<string> symptoms)
        {
            // TODO: AddAsync(record), then add each symptom as related SymptomEntry entity.
            throw new NotImplementedException();
        }

    }
}