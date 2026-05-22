using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Patient;
using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;

        public PatientService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ── Profile ───────────────────────────────────────────────────────────────

        public Task<ServiceResult<ProfileDto>> GetProfileAsync(string userId)
        {
            // TODO: Get PatientProfile by userId via _uow.PatientProfiles.GetByUserIdAsync,
            //       map to ProfileDto, return ServiceResult.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ProfileDto>> UpdateProfileAsync(string userId, ProfileDto dto)
        {
            // TODO: Find profile, update fields from dto, call _uow.CompleteAsync(), return updated dto.
            throw new NotImplementedException();
        }

        // ── Dashboard ─────────────────────────────────────────────────────────────

        public Task<ServiceResult<DashboardDto>> GetDashboardAsync(string userId)
        {
            // TODO: Aggregate upcoming appointments, latest risk assessment, recent alerts,
            //       and unread notifications into DashboardDto.
            throw new NotImplementedException();
        }

        // ── Medical Records ───────────────────────────────────────────────────────

        public Task<ServiceResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsAsync(string userId)
        {
            // TODO: Get patient profile, call _uow.MedicalRecords.GetByPatientIdAsync, map to DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<MedicalRecordDto>> GetLatestMedicalRecordAsync(string userId)
        {
            // TODO: Get patient profile, call _uow.MedicalRecords.GetLatestByPatientIdAsync, map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<MedicalRecordDto>> AddMedicalRecordAsync(string userId, MedicalRecordCreateDto dto)
        {
            // TODO: Get patient profile, map dto to MedicalRecord entity,
            //       call _uow.MedicalRecords.AddAsync, complete, return Created result.
            throw new NotImplementedException();
        }

        // ── Risk Assessment ───────────────────────────────────────────────────────

        public Task<ServiceResult<RiskResultDto>> AssessRiskAsync(string userId, RiskInputDto input)
        {
            // TODO: Use RiskCalculatorHelper to compute risk score from input,
            //       persist via _uow.RiskAssessments.AddAsync, return result.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<RiskResultDto>> GetLatestRiskAssessmentAsync(string userId)
        {
            // TODO: Get patient profile, call _uow.RiskAssessments.GetLatestByPatientIdAsync, map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IEnumerable<RiskResultDto>>> GetRiskHistoryAsync(string userId)
        {
            // TODO: Get patient profile, call _uow.RiskAssessments.GetByPatientIdAsync, map list to DTOs.
            throw new NotImplementedException();
        }
    }
}
