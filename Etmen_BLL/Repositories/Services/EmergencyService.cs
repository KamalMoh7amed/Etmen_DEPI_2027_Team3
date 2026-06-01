using Etmen_BLL.DTOs.Emergency;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Helpers;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Mapster;

namespace Etmen_BLL.Repositories.Services
{
    public class EmergencyService : IEmergencyService
    {
        private readonly IUnitOfWork _uow;

        public EmergencyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<EmergencyRequestDto>> CreateEmergencyRequestAsync(EmergencyRequestDto dto)
        {
            try
            {
                // Verify patient exists
                var patient = await _uow.PatientProfiles.GetByIdAsync(dto.PatientProfileId);
                if (patient == null)
                    return ServiceResult<EmergencyRequestDto>.NotFound($"Patient with ID {dto.PatientProfileId} not found");

                // Map DTO to entity
                var request = dto.Adapt<EmergencyRequest>();
                request.Status = EmergencyRequestStatus.Pending;
                request.RequestedAt = DateTime.UtcNow;

                // Find nearest available provider
                if (dto.Latitude != 0 && dto.Longitude != 0)
                {
                    var nearbyProviders = await _uow.HealthcareProviders.GetNearbyProvidersAsync(
                        dto.Latitude, 
                        dto.Longitude, 
                        50 // 50km search radius
                    );

                    var availableProviders = nearbyProviders
                        .Where(p => p.IsActive && (p.AvailableBeds ?? 0) > 0)
                        .ToList();

                    if (availableProviders.Any())
                    {
                        // Find the nearest provider
                        var nearest = GeoHelper.FindNearest(
                            availableProviders,
                            dto.Latitude,
                            dto.Longitude,
                            p => p.Latitude,
                            p => p.Longitude
                        );

                        if (nearest != null)
                        {
                            request.HealthcareProviderId = nearest.Id;
                        }
                    }
                }

                await _uow.EmergencyRequests.AddAsync(request);
                await _uow.CompleteAsync();

                var result = request.Adapt<EmergencyRequestDto>();
                return ServiceResult<EmergencyRequestDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<EmergencyRequestDto>.Failure($"Error creating emergency request: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<EmergencyRequestDto>> GetEmergencyRequestAsync(int requestId)
        {
            try
            {
                var request = await _uow.EmergencyRequests.GetWithTrackingInfoAsync(requestId);
                if (request == null)
                    return ServiceResult<EmergencyRequestDto>.NotFound($"Emergency request with ID {requestId} not found");

                var dto = request.Adapt<EmergencyRequestDto>();
                return ServiceResult<EmergencyRequestDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EmergencyRequestDto>.Failure($"Error retrieving emergency request: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<List<EmergencyTrackingDto>>> GetPendingEmergenciesAsync()
        {
            try
            {
                var requests = await _uow.EmergencyRequests.GetPendingRequestsAsync();
                var dtos = new List<EmergencyTrackingDto>();

                foreach (var request in requests)
                {
                    var dto = new EmergencyTrackingDto
                    {
                        RequestId = request.Id,
                        Status = request.Status,
                        ProviderName = request.HealthcareProvider?.Name,
                        ProviderPhone = request.HealthcareProvider?.Phone,
                        DistanceInKm = request.HealthcareProvider != null && request.Latitude.HasValue && request.Longitude.HasValue
                            ? GeoHelper.CalculateDistance(
                                request.Latitude.Value,
                                request.Longitude.Value,
                                request.HealthcareProvider.Latitude,
                                request.HealthcareProvider.Longitude
                            )
                            : 0,
                        RequestedAt = request.RequestedAt,
                        AcceptedAt = request.AcceptedAt
                    };
                    dtos.Add(dto);
                }

                return ServiceResult<List<EmergencyTrackingDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<EmergencyTrackingDto>>.Failure($"Error retrieving pending emergencies: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult> UpdateEmergencyStatusAsync(int requestId, EmergencyUpdateDto dto)
        {
            try
            {
                var request = await _uow.EmergencyRequests.GetByIdAsync(requestId);
                if (request == null)
                    return ServiceResult.NotFound($"Emergency request with ID {requestId} not found");

                // Parse status
                if (!Enum.TryParse<EmergencyRequestStatus>(dto.Status, true, out var status))
                    return ServiceResult.Failure($"Invalid status: {dto.Status}", 400);

                request.Status = status;

                if (!string.IsNullOrEmpty(dto.ResponseNotes))
                    request.ResponseNotes = dto.ResponseNotes;

                if (dto.AssignedProviderId.HasValue)
                {
                    var provider = await _uow.HealthcareProviders.GetByIdAsync(dto.AssignedProviderId.Value);
                    if (provider == null)
                        return ServiceResult.NotFound($"Provider with ID {dto.AssignedProviderId} not found");

                    request.HealthcareProviderId = dto.AssignedProviderId.Value;
                }

                // Set timestamp based on status
                if (status == EmergencyRequestStatus.Accepted)
                    request.AcceptedAt = DateTime.UtcNow;
                else if (status == EmergencyRequestStatus.Completed)
                    request.CompletedAt = DateTime.UtcNow;

                _uow.EmergencyRequests.Update(request);
                await _uow.CompleteAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error updating emergency status: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<HospitalQueueDto>> GetHospitalQueueAsync()
        {
            try
            {
                var pendingRequests = await _uow.EmergencyRequests.GetPendingRequestsAsync();

                if (!pendingRequests.Any())
                    return ServiceResult<HospitalQueueDto>.NotFound("No pending emergencies in queue");

                // Aggregate by provider
                var queueItem = pendingRequests.First();

                var queueDto = new HospitalQueueDto
                {
                    RequestId = queueItem.Id,
                    PatientName = queueItem.PatientProfile?.FullName ?? "Unknown",
                    EmergencyType = queueItem.EmergencyType ?? "General Emergency",
                    Status = queueItem.Status,
                    RequestedAt = queueItem.RequestedAt,
                    AvailableBeds = queueItem.HealthcareProvider?.AvailableBeds
                };

                return ServiceResult<HospitalQueueDto>.Success(queueDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<HospitalQueueDto>.Failure($"Error retrieving hospital queue: {ex.Message}", 500);
            }
        }
    }
}
