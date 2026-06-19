using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Etmen_Domain.Enums;

namespace Etmen_BLL.Repositories.Services
{
    public class RiskService : IRiskService
    {
        private readonly IUnitOfWork _uow;

        public RiskService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<RiskResultDto>> CalculateRiskAsync(RiskInputDto dto)
        {
            try
            {
                if (dto == null)
                    return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.Failure("Input data cannot be null");

               
                string? symptomsString = dto.Symptoms != null ? string.Join(", ", dto.Symptoms) : null;

               
                var calculation = RiskCalculatorHelper.Calculate(
                    null, 
                    null,
                    null, 
                    null, 
                    null, 
                    null, 
                    symptomsString
                );

                var riskLevel = RiskCalculatorHelper.GetRiskLevel(calculation.Score);

                var resultDto = new RiskResultDto
                {
                    RiskScore = calculation.Score,
                    RiskLevel = riskLevel,
                    RiskColor = RiskCalculatorHelper.GetRiskColor(riskLevel),
                    RiskLabel = RiskCalculatorHelper.GetRiskLabel(riskLevel),
                    IsEmergency = calculation.IsEmergency,
                    TriggeredSymptoms = calculation.TriggeredFactors,
                    Recommendations = RiskCalculatorHelper.GenerateRecommendations(riskLevel, calculation.TriggeredFactors),
                    NearestEmergencyCenter = calculation.IsEmergency ? "Emergency Center" : null
                };

                return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<List<RiskResultDto>>> GetPatientRiskHistoryAsync(int patientProfileId)
        {
            try
            {
                var assessments = await _uow.RiskAssessments.GetByPatientIdAsync(patientProfileId);

                if (assessments == null)
                    return Etmen_BLL.Helpers.ServiceResult<List<RiskResultDto>>.NotFound("No history found");

                var historyList = assessments.Select(r => new RiskResultDto
                {
                    RiskScore = r.RiskScore,
                    RiskLevel = r.RiskLevel,
                    RiskColor = RiskCalculatorHelper.GetRiskColor(r.RiskLevel),
                    RiskLabel = RiskCalculatorHelper.GetRiskLabel(r.RiskLevel),
                    IsEmergency = r.RiskLevel == RiskLevel.Emergency, 
                    NearestEmergencyCenter = r.RiskLevel == RiskLevel.Emergency ? "Emergency Center" : null,
                    Recommendations = RiskCalculatorHelper.GenerateRecommendations(r.RiskLevel, new List<string>()),
                    TriggeredSymptoms = new List<string>()
                }).ToList();

                return Etmen_BLL.Helpers.ServiceResult<List<RiskResultDto>>.Success(historyList);
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<List<RiskResultDto>>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult> SaveRiskAssessmentAsync(int patientProfileId, RiskResultDto riskResult)
        {
            try
            {
                if (riskResult == null)
                    return Etmen_BLL.Helpers.ServiceResult.Failure("Risk result is null");

              
                var entity = new RiskAssessment
                {
                    PatientProfileId = patientProfileId,
                    RiskScore = riskResult.RiskScore,
                    RiskLevel = riskResult.RiskLevel
                };

                await _uow.RiskAssessments.AddAsync(entity);
                await _uow.CompleteAsync();

                return Etmen_BLL.Helpers.ServiceResult.Success(201);
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult.Failure(ex.Message);
            }
        }
    }
}