using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class RiskAssessmentRepository : GenericRepository<RiskAssessment>, IRiskAssessmentRepository
    {
        public RiskAssessmentRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<RiskAssessment>> GetByPatientIdAsync(int patientId)
        {
            // TODO: FindAsync(r => r.PatientProfileId == patientId) ordered by AssessedAt desc.
            throw new NotImplementedException();
        }

        public async Task<RiskAssessment?> GetLatestByPatientIdAsync(int patientId)
        {
            // TODO: FirstOrDefaultAsync ordered by AssessedAt desc.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RiskAssessment>> GetHighRiskPatientsAsync()
        {
            // TODO: FindAsync(r => r.RiskLevel == RiskLevel.High || r.RiskLevel == RiskLevel.Emergency).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RiskAssessment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync(r => r.AssessedAt >= startDate && r.AssessedAt <= endDate).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RiskAssessment>> GetByRiskLevelAsync(RiskLevel riskLevel)
        {
            // TODO: FindAsync(r => r.RiskLevel == riskLevel).
            throw new NotImplementedException();
        }

        public async Task<int> GetRiskCountByLevelAsync(RiskLevel riskLevel)
        {
            // TODO: CountAsync(r => r.RiskLevel == riskLevel).
            throw new NotImplementedException();
        }

        public async Task<decimal> GetAverageRiskScoreAsync(int patientId)
        {
            // TODO: _dbSet.Where(r=>r.PatientProfileId==patientId).AverageAsync(r=>r.RiskScore).
            throw new NotImplementedException();
        }

    }
}