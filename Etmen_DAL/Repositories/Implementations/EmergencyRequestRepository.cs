using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class EmergencyRequestRepository : GenericRepository<EmergencyRequest>, IEmergencyRequestRepository
    {
        public EmergencyRequestRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<EmergencyRequest>> GetByPatientIdAsync(int patientId)
        {
            return await _dbSet.AsNoTracking()
                .Where(e => e.PatientProfileId == patientId)
                .OrderByDescending(e => e.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmergencyRequest>> GetByProviderIdAsync(int providerId)
        {
            return await FindAsync(e => e.HealthcareProviderId == providerId);
        }

        public async Task<IEnumerable<EmergencyRequest>> GetPendingRequestsAsync()
        {
            return await FindAsync(e => e.Status == EmergencyRequestStatus.Pending);
        }

        public async Task<IEnumerable<EmergencyRequest>> GetByStatusAsync(EmergencyRequestStatus status)
        {
            return await FindAsync(e => e.Status == status);
        }

        public async Task<EmergencyRequest?> GetWithTrackingInfoAsync(int requestId)
        {
            return await _dbSet.Include(e => e.PatientProfile)
                .Include(e => e.HealthcareProvider)
                .FirstOrDefaultAsync(e => e.Id == requestId);
        }

        public async Task AcceptRequestAsync(int requestId, int providerId)
        {
            var request = await GetByIdAsync(requestId);
            if (request != null)
            {
                request.Status = EmergencyRequestStatus.Accepted;
                request.HealthcareProviderId = providerId;
                request.AcceptedAt = DateTime.UtcNow;
                Update(request);
            }
        }

        public async Task RejectRequestAsync(int requestId, string reason)
        {
            var request = await GetByIdAsync(requestId);
            if (request != null)
            {
                request.Status = EmergencyRequestStatus.Rejected;
                request.ResponseNotes = reason;
                Update(request);
            }
        }

        public async Task CompleteRequestAsync(int requestId, string notes)
        {
            var request = await GetByIdAsync(requestId);
            if (request != null)
            {
                request.Status = EmergencyRequestStatus.Completed;
                request.ResponseNotes = notes;
                request.CompletedAt = DateTime.UtcNow;
                Update(request);
            }
        }

        public async Task<IEnumerable<EmergencyRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await FindAsync(e => e.RequestedAt >= startDate && e.RequestedAt <= endDate);
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await CountAsync(e => e.Status == EmergencyRequestStatus.Pending);
        }

    }
}