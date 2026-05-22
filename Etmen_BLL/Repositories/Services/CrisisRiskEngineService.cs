using Etmen_BLL.DTOs.Crisis;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class CrisisRiskEngineService : ICrisisRiskEngineService
    {
        private readonly IUnitOfWork _uow;

        public CrisisRiskEngineService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<CrisisRiskResultDto>> CalculateCrisisRiskAsync(int patientProfileId, int crisisConfigurationId)
        {
            // TODO: Load PatientProfile with medical records/symptoms,
            //       load CrisisConfiguration with SymptomWeights,
            //       score each symptom against patient data, sum weighted scores,
            //       determine RiskLevel from thresholds, persist RiskAssessment, return result.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<decimal>> CalculateOutbreakProbabilityAsync(decimal latitude, decimal longitude, int crisisConfigurationId)
        {
            // TODO: Find OutbreakZones for this crisis within radius of (lat, lon),
            //       compute density/probability score, return as decimal.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<OutbreakZoneDto>>> GetPatientsInZoneAsync(int crisisConfigurationId)
        {
            // TODO: _uow.OutbreakZones.GetActiveZonesAsync(crisisConfigurationId),
            //       for each zone gather patient count, map to OutbreakZoneDto list.
            throw new NotImplementedException();
        }
    }
}
