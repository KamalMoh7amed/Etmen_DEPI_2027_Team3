using Etmen_BLL.DTOs.Emergency;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class EmergencyService : IEmergencyService
    {
        private readonly IUnitOfWork _uow;

        public EmergencyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<EmergencyRequestDto>> CreateEmergencyRequestAsync(EmergencyRequestDto dto)
        {
            // TODO: Map dto to EmergencyRequest entity, set Status = Pending,
            //       find nearest provider via _uow.HealthcareProviders.GetNearbyProvidersAsync,
            //       AddAsync, CompleteAsync, return Created.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<EmergencyRequestDto>> GetEmergencyRequestAsync(int requestId)
        {
            // TODO: _uow.EmergencyRequests.GetWithTrackingInfoAsync(requestId), map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<EmergencyTrackingDto>>> GetPendingEmergenciesAsync()
        {
            // TODO: _uow.EmergencyRequests.GetPendingRequestsAsync(), map to EmergencyTrackingDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateEmergencyStatusAsync(int requestId, EmergencyUpdateDto dto)
        {
            // TODO: GetByIdAsync, update Status and relevant fields from dto, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<HospitalQueueDto>> GetHospitalQueueAsync()
        {
            // TODO: Aggregate pending emergencies per provider into HospitalQueueDto.
            throw new NotImplementedException();
        }
    }
}
