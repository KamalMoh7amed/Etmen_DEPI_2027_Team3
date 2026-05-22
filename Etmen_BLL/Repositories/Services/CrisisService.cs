using Etmen_BLL.DTOs.Crisis;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class CrisisService : ICrisisService
    {
        private readonly IUnitOfWork _uow;

        public CrisisService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<CrisisConfigurationDto>> GetActiveCrisisAsync()
        {
            // TODO: _uow.CrisisConfigurations.GetActiveCrisisAsync(), map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<CrisisConfigurationDto>>> GetAllCrisesAsync()
        {
            // TODO: _uow.CrisisConfigurations.GetAllCrisesAsync(), map list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<CrisisConfigurationDto>> GetCrisisByIdAsync(int crisisId)
        {
            // TODO: GetWithSymptomWeightsAsync(crisisId), map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<CrisisStatsDto>> GetCrisisStatsAsync(int crisisId)
        {
            // TODO: Aggregate patient counts by risk level, outbreak zones, etc.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<CrisisConfigurationDto>> CreateCrisisAsync(CreateCrisisDto dto)
        {
            // TODO: Map dto to CrisisConfiguration entity, AddAsync, CompleteAsync, return Created.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<CrisisConfigurationDto>> UpdateCrisisAsync(int crisisId, EditCrisisDto dto)
        {
            // TODO: GetByIdAsync, apply dto, CompleteAsync, return updated DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> ActivateCrisisAsync(int crisisId)
        {
            // TODO: Deactivate any currently active crisis, then ActivateCrisisAsync(crisisId).
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeactivateCrisisAsync(int crisisId)
        {
            // TODO: _uow.CrisisConfigurations.DeactivateCrisisAsync(crisisId), CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteCrisisAsync(int crisisId)
        {
            // TODO: Verify not active, Remove entity, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> AddSymptomAsync(int crisisId, SymptomWeightDto symptomDto)
        {
            // TODO: GetWithSymptomWeightsAsync, add SymptomWeight to collection, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> AddMultipleSymptomsAsync(int crisisId, List<SymptomWeightDto> symptomsDto)
        {
            // TODO: Bulk add SymptomWeight entities linked to crisisId.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateSymptomAsync(int crisisId, string symptomName, SymptomWeightDto updatedSymptomDto)
        {
            // TODO: Find symptom by name in crisis, update weight, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> RemoveSymptomAsync(int crisisId, string symptomName)
        {
            // TODO: Find symptom, Remove entity, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<SymptomWeightDto>>> GetSymptomsByCrisisAsync(int crisisId)
        {
            // TODO: GetWithSymptomWeightsAsync, extract and map SymptomWeights to DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateRiskThresholdsAsync(int crisisId, decimal? emergencyThreshold, decimal? highRiskThreshold, decimal? mediumRiskThreshold)
        {
            // TODO: Find crisis, update threshold fields, CompleteAsync.
            throw new NotImplementedException();
        }
    }
}
