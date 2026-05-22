using Etmen_BLL.DTOs.Nearby;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class NearbyService : INearbyService
    {
        private readonly IUnitOfWork _uow;

        public NearbyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<List<ProviderDto>>> SearchNearbyProvidersAsync(NearbySearchDto dto)
        {
            // TODO: _uow.HealthcareProviders.GetNearbyProvidersAsync(dto.Latitude, dto.Longitude, dto.RadiusKm),
            //       optionally filter by type, map to ProviderDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<AvailableSlotDto>>> GetAvailableSlotsByProviderAsync(int providerId)
        {
            // TODO: Resolve DoctorProfile linked to provider,
            //       _uow.AvailableSlots.GetAvailableSlotsAsync, map to AvailableSlotDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> BookAppointmentAsync(BookingRequestDto dto)
        {
            // TODO: Verify slot available, create Appointment, mark slot as booked, CompleteAsync.
            throw new NotImplementedException();
        }
    }
}
