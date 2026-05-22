using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class RiskService : IRiskService
    {
        private readonly IUnitOfWork _uow;

        public RiskService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<RiskResultDto>> CalculateRiskAsync(RiskInputDto dto)
        {
            // TODO: Use RiskCalculatorHelper to compute score from dto inputs (BMI, age, symptoms, etc.),
            //       determine RiskLevel from thresholds, build and return RiskResultDto.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<RiskResultDto>>> GetPatientRiskHistoryAsync(int patientProfileId)
        {
            // TODO: _uow.RiskAssessments.GetByPatientIdAsync(patientProfileId), map to RiskResultDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> SaveRiskAssessmentAsync(int patientProfileId, RiskResultDto riskResult)
        {
            // TODO: Map riskResult to RiskAssessment entity, set PatientProfileId,
            //       AddAsync, CompleteAsync.
            throw new NotImplementedException();
        }
    }
}
