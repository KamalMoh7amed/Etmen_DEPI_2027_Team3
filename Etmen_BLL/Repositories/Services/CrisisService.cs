using Etmen_BLL.DTOs.Crisis;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Mapster;

namespace Etmen_BLL.Repositories.Services
{
    public class CrisisService : ICrisisService
    {
        private readonly IUnitOfWork _uow;

        public CrisisService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<CrisisConfigurationDto>> GetActiveCrisisAsync()
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetActiveCrisisAsync();
                if (crisis == null)
                    return ServiceResult<CrisisConfigurationDto>.NotFound("No active crisis found");

                var dto = crisis.Adapt<CrisisConfigurationDto>();
                dto.ZonesCount = crisis.OutbreakZones.Count;
                return ServiceResult<CrisisConfigurationDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CrisisConfigurationDto>.Failure($"Error retrieving active crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<List<CrisisConfigurationDto>>> GetAllCrisesAsync()
        {
            try
            {
                var crises = await _uow.CrisisConfigurations.GetAllCrisesAsync();
                var dtos = crises.Select(c => 
                {
                    var dto = c.Adapt<CrisisConfigurationDto>();
                    dto.ZonesCount = c.OutbreakZones.Count;
                    return dto;
                }).ToList();

                return ServiceResult<List<CrisisConfigurationDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CrisisConfigurationDto>>.Failure($"Error retrieving crises: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<CrisisConfigurationDto>> GetCrisisByIdAsync(int crisisId)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisId);
                if (crisis == null)
                    return ServiceResult<CrisisConfigurationDto>.NotFound($"Crisis with ID {crisisId} not found");

                var dto = crisis.Adapt<CrisisConfigurationDto>();
                dto.ZonesCount = crisis.OutbreakZones.Count;
                return ServiceResult<CrisisConfigurationDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CrisisConfigurationDto>.Failure($"Error retrieving crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<CrisisStatsDto>> GetCrisisStatsAsync(int crisisId)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithOutbreakZonesAsync(crisisId);
                if (crisis == null)
                    return ServiceResult<CrisisStatsDto>.NotFound($"Crisis with ID {crisisId} not found");

                var assessments = await _uow.RiskAssessments.FindAsync(a => a.PatientProfile.Id > 0);
                var crisisAssessments = assessments.Where(a => a.RiskScore > 0).ToList();

                var stats = new CrisisStatsDto
                {
                    TotalAssessments = crisisAssessments.Count,
                    HighRiskCount = crisisAssessments.Count(a => a.RiskLevel == Etmen_Domain.Enums.RiskLevel.High),
                    CriticalCount = crisisAssessments.Count(a => a.RiskLevel == Etmen_Domain.Enums.RiskLevel.Critical),
                    OutbreakZonesCount = crisis.OutbreakZones.Count,
                    AverageRiskScore = crisisAssessments.Any() ? (decimal)crisisAssessments.Average(a => (double)a.RiskScore) : 0,
                    LastUpdated = DateTime.UtcNow
                };

                return ServiceResult<CrisisStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return ServiceResult<CrisisStatsDto>.Failure($"Error calculating crisis stats: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<CrisisConfigurationDto>> CreateCrisisAsync(CreateCrisisDto dto)
        {
            try
            {
                var crisis = dto.Adapt<CrisisConfiguration>();
                crisis.CreatedAt = DateTime.UtcNow;

                await _uow.CrisisConfigurations.AddAsync(crisis);
                await _uow.CompleteAsync();

                var resultDto = crisis.Adapt<CrisisConfigurationDto>();
                resultDto.ZonesCount = 0;
                return ServiceResult<CrisisConfigurationDto>.Created(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CrisisConfigurationDto>.Failure($"Error creating crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<CrisisConfigurationDto>> UpdateCrisisAsync(int crisisId, EditCrisisDto dto)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetByIdAsync(crisisId);
                if (crisis == null)
                    return ServiceResult<CrisisConfigurationDto>.NotFound($"Crisis with ID {crisisId} not found");

                crisis.CrisisName = dto.CrisisName;
                crisis.CrisisType = dto.CrisisType;
                crisis.SystemMode = dto.SystemMode;
                crisis.EndDate = dto.EndDate;
                crisis.EmergencyThreshold = dto.EmergencyThreshold;
                crisis.HighRiskThreshold = dto.HighRiskThreshold;
                crisis.MediumRiskThreshold = dto.MediumRiskThreshold;
                crisis.UpdatedAt = DateTime.UtcNow;

                _uow.CrisisConfigurations.Update(crisis);
                await _uow.CompleteAsync();

                var resultDto = crisis.Adapt<CrisisConfigurationDto>();
                return ServiceResult<CrisisConfigurationDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CrisisConfigurationDto>.Failure($"Error updating crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> ActivateCrisisAsync(int crisisId)
        {
            try
            {
                await _uow.CrisisConfigurations.ActivateCrisisAsync(crisisId);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error activating crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> DeactivateCrisisAsync(int crisisId)
        {
            try
            {
                await _uow.CrisisConfigurations.DeactivateCrisisAsync(crisisId);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error deactivating crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> DeleteCrisisAsync(int crisisId)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetByIdAsync(crisisId);
                if (crisis == null)
                    return ServiceResult.NotFound($"Crisis with ID {crisisId} not found");

                if (crisis.IsActive)
                    return ServiceResult.Failure("Cannot delete an active crisis", 400);

                _uow.CrisisConfigurations.Remove(crisis);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error deleting crisis: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> AddSymptomAsync(int crisisId, SymptomWeightDto symptomDto)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisId);
                if (crisis == null)
                    return ServiceResult.NotFound($"Crisis with ID {crisisId} not found");

                var symptom = symptomDto.Adapt<SymptomWeight>();
                crisis.SymptomWeights.Add(symptom);

                _uow.CrisisConfigurations.Update(crisis);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error adding symptom: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> AddMultipleSymptomsAsync(int crisisId, List<SymptomWeightDto> symptomsDto)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisId);
                if (crisis == null)
                    return ServiceResult.NotFound($"Crisis with ID {crisisId} not found");

                foreach (var symptomDto in symptomsDto)
                {
                    var symptom = symptomDto.Adapt<SymptomWeight>();
                    crisis.SymptomWeights.Add(symptom);
                }

                _uow.CrisisConfigurations.Update(crisis);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error adding symptoms: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> UpdateSymptomAsync(int crisisId, string symptomName, SymptomWeightDto updatedSymptomDto)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisId);
                if (crisis == null)
                    return ServiceResult.NotFound($"Crisis with ID {crisisId} not found");

                var symptom = crisis.SymptomWeights.FirstOrDefault(s => s.SymptomName == symptomName);
                if (symptom == null)
                    return ServiceResult.NotFound($"Symptom '{symptomName}' not found in this crisis");

                symptom.Weight = updatedSymptomDto.Weight;
                symptom.IsEmergencySymptom = updatedSymptomDto.IsEmergencySymptom;

                _uow.CrisisConfigurations.Update(crisis);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error updating symptom: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> RemoveSymptomAsync(int crisisId, string symptomName)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisId);
                if (crisis == null)
                    return ServiceResult.NotFound($"Crisis with ID {crisisId} not found");

                var symptom = crisis.SymptomWeights.FirstOrDefault(s => s.SymptomName == symptomName);
                if (symptom == null)
                    return ServiceResult.NotFound($"Symptom '{symptomName}' not found in this crisis");

                crisis.SymptomWeights.Remove(symptom);

                _uow.CrisisConfigurations.Update(crisis);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error removing symptom: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<List<SymptomWeightDto>>> GetSymptomsByCrisisAsync(int crisisId)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisId);
                if (crisis == null)
                    return ServiceResult<List<SymptomWeightDto>>.NotFound($"Crisis with ID {crisisId} not found");

                var dtos = crisis.SymptomWeights.Adapt<List<SymptomWeightDto>>();
                return ServiceResult<List<SymptomWeightDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<SymptomWeightDto>>.Failure($"Error retrieving symptoms: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> UpdateRiskThresholdsAsync(int crisisId, decimal? emergencyThreshold, decimal? highRiskThreshold, decimal? mediumRiskThreshold)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetByIdAsync(crisisId);
                if (crisis == null)
                    return ServiceResult.NotFound($"Crisis with ID {crisisId} not found");

                if (emergencyThreshold.HasValue)
                    crisis.EmergencyThreshold = emergencyThreshold.Value;
                if (highRiskThreshold.HasValue)
                    crisis.HighRiskThreshold = highRiskThreshold.Value;
                if (mediumRiskThreshold.HasValue)
                    crisis.MediumRiskThreshold = mediumRiskThreshold.Value;

                crisis.UpdatedAt = DateTime.UtcNow;

                _uow.CrisisConfigurations.Update(crisis);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error updating risk thresholds: {ex.Message}", 500);
            }
        }
    }
}
