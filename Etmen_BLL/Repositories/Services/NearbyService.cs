using Etmen_BLL.DTOs.Nearby;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Helpers;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Mapster;

namespace Etmen_BLL.Repositories.Services
{
    public class NearbyService : INearbyService
    {
        private readonly IUnitOfWork _uow;

        public NearbyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<List<ProviderDto>>> SearchNearbyProvidersAsync(NearbySearchDto dto)
        {
            try
            {
                var nearbyProviders = await _uow.HealthcareProviders.GetNearbyProvidersAsync(
                    dto.Latitude,
                    dto.Longitude,
                    dto.RadiusInKm
                );

                var filtered = nearbyProviders.AsEnumerable();

                // Filter by type if specified
                if (!string.IsNullOrEmpty(dto.Type))
                {
                    filtered = filtered.Where(p => p.Type.Equals(dto.Type, StringComparison.OrdinalIgnoreCase));
                }

                // Calculate distances and map to DTO
                var providerDtos = filtered.Select(p =>
                {
                    var providerDto = p.Adapt<ProviderDto>();
                    providerDto.DistanceKm = GeoHelper.CalculateDistance(
                        dto.Latitude,
                        dto.Longitude,
                        p.Latitude,
                        p.Longitude
                    );
                    return providerDto;
                })
                .OrderBy(p => p.DistanceKm)
                .ToList();

                return ServiceResult<List<ProviderDto>>.Success(providerDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ProviderDto>>.Failure($"Error searching nearby providers: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<List<AvailableSlotDto>>> GetAvailableSlotsByProviderAsync(int providerId)
        {
            try
            {
                var provider = await _uow.HealthcareProviders.GetByIdAsync(providerId);
                if (provider == null)
                    return ServiceResult<List<AvailableSlotDto>>.NotFound($"Provider with ID {providerId} not found");

                // Get slots for this provider (assuming AvailableSlots linked to HealthcareProvider via DoctorProfile)
                var slots = await _uow.AvailableSlots.FindAsync(s => s.DoctorProfile.Id > 0);

                // Filter for available slots not booked and future dates
                var availableSlots = slots
                    .Where(s => !s.IsBooked && s.SlotDate >= DateTime.UtcNow.Date)
                    .Select(s => new AvailableSlotDto
                    {
                        Id = s.Id,
                        DoctorId = s.DoctorProfileId,
                        Date = s.SlotDate,
                        StartTime = s.SlotStart,
                        EndTime = s.SlotEnd,
                        IsBooked = s.IsBooked
                    })
                    .OrderBy(s => s.Date)
                    .ThenBy(s => s.StartTime)
                    .ToList();

                return ServiceResult<List<AvailableSlotDto>>.Success(availableSlots);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<AvailableSlotDto>>.Failure($"Error retrieving available slots: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> BookAppointmentAsync(BookingRequestDto dto)
        {
            try
            {
                // Verify slot exists and is available
                var slot = await _uow.AvailableSlots.GetByIdAsync(dto.SlotId);
                if (slot == null)
                    return ServiceResult.NotFound($"Slot with ID {dto.SlotId} not found");

                if (slot.IsBooked)
                    return ServiceResult.Failure("Slot is already booked", 409);

                if (slot.SlotDate != dto.Date || slot.SlotStart != dto.StartTime || slot.SlotEnd != dto.EndTime)
                    return ServiceResult.Failure("Slot details mismatch", 400);

                // Verify doctor exists
                var doctor = await _uow.DoctorProfiles.GetByIdAsync(dto.DoctorId);
                if (doctor == null)
                    return ServiceResult.NotFound($"Doctor with ID {dto.DoctorId} not found");

                // Begin transaction for atomicity
                await _uow.BeginTransactionAsync();

                try
                {
                    // Mark slot as booked
                    slot.IsBooked = true;
                    _uow.AvailableSlots.Update(slot);

                    // Create appointment
                    var appointment = new Appointment
                    {
                        DoctorProfileId = dto.DoctorId,
                        AppointmentDate = dto.Date,
                        StartTime = dto.StartTime,
                        EndTime = dto.EndTime,
                        Notes = dto.Notes,
                        Status = Etmen_Domain.Enums.AppointmentStatus.Scheduled,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _uow.Appointments.AddAsync(appointment);
                    await _uow.CompleteAsync();
                    await _uow.CommitTransactionAsync();

                    return ServiceResult.Success(201);
                }
                catch (Exception ex)
                {
                    await _uow.RollbackTransactionAsync();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error booking appointment: {ex.Message}", 500);
            }
        }
    }
}
