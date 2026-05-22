using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class LabResultRepository : GenericRepository<LabResult>, ILabResultRepository
    {
        public LabResultRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId)
        {
            // TODO: FindAsync(l => l.PatientProfileId == patientId) ordered by TestDate desc.
            throw new NotImplementedException();
        }

        public async Task<LabResult?> GetLatestByPatientIdAsync(int patientId)
        {
            // TODO: FirstOrDefaultAsync ordered by TestDate desc.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LabResult>> GetByTestNameAsync(int patientId, string testName)
        {
            // TODO: FindAsync(l => l.PatientProfileId==patientId && l.TestName==testName).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LabResult>> GetWithOcrDataAsync(int patientId)
        {
            // TODO: FindAsync(l => l.PatientProfileId==patientId && l.OcrData != null).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LabResult>> GetByDateRangeAsync(int patientId, DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync(l => l.PatientProfileId==patientId && l.TestDate>=startDate && l.TestDate<=endDate).
            throw new NotImplementedException();
        }

        public async Task UpdateOcrDataAsync(int labResultId, string ocrData)
        {
            // TODO: GetByIdAsync, set OcrData=ocrData, Update.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LabResult>> SearchLabResultsAsync(int patientId, string searchTerm)
        {
            // TODO: FindAsync(l => l.PatientProfileId==patientId && l.TestName.Contains(searchTerm)).
            throw new NotImplementedException();
        }

    }
}