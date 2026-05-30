using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class RiskAssessmentRepository : GenericRepository<RiskAssessment>, IRiskAssessmentRepository
    {
        public RiskAssessmentRepository(EtmenDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RiskAssessment>> GetByPatientIdAsync(int patientId)
        {
            return await Table .Where(r => r.PatientProfileId == patientId) .OrderByDescending(r => r.AssessmentDate) .ToListAsync();
        }

        public async Task<RiskAssessment?> GetLatestByPatientIdAsync(int patientId)
        {
            return await Table.Where(r => r.PatientProfileId == patientId).OrderByDescending(r => r.AssessmentDate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RiskAssessment>> GetHighRiskPatientsAsync()
        {
            return await Table .Where(r => r.RiskLevel == RiskLevel.High || r.RiskLevel == RiskLevel.Emergency) .OrderByDescending(r => r.AssessmentDate).ToListAsync();
        }

        public async Task<IEnumerable<RiskAssessment>> GetByDateRangeAsync( DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be greater than end date.");

            return await Table .Where(r => r.AssessmentDate >= startDate && r.AssessmentDate <= endDate).OrderByDescending(r => r.AssessmentDate) .ToListAsync();
        }

        public async Task<IEnumerable<RiskAssessment>> GetByRiskLevelAsync(RiskLevel riskLevel)
        {
            return await Table .Where(r => r.RiskLevel == riskLevel) .OrderByDescending(r => r.AssessmentDate) .ToListAsync();
        }

        public async Task<int> GetRiskCountByLevelAsync(RiskLevel riskLevel)
        {
            return await CountAsync(r => r.RiskLevel == riskLevel);
        }

        public async Task<decimal> GetAverageRiskScoreAsync(int patientId)
        {
            var query = Table.Where(r => r.PatientProfileId == patientId);

            if (!await query.AnyAsync())
                return 0;

            return await query.AverageAsync(r => r.RiskScore);
        }
    }
}