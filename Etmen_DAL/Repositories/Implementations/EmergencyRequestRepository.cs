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
            // TODO: FindAsync(e => e.PatientProfileId == patientId) ordered by CreatedAt desc.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmergencyRequest>> GetByProviderIdAsync(int providerId)
        {
            // TODO: FindAsync(e => e.ProviderId == providerId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmergencyRequest>> GetPendingRequestsAsync()
        {
            // TODO: FindAsync(e => e.Status == EmergencyRequestStatus.Pending).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmergencyRequest>> GetByStatusAsync(EmergencyRequestStatus status)
        {
            // TODO: FindAsync(e => e.Status == status).
            throw new NotImplementedException();
        }

        public async Task<EmergencyRequest?> GetWithTrackingInfoAsync(int requestId)
        {
            // TODO: _dbSet.Include(e=>e.PatientProfile).Include(e=>e.Provider).FirstOrDefaultAsync(e=>e.Id==requestId).
            throw new NotImplementedException();
        }

        public async Task AcceptRequestAsync(int requestId, int providerId)
        {
            // TODO: GetByIdAsync, set Status=Accepted, ProviderId=providerId, AcceptedAt=UtcNow, Update.
            throw new NotImplementedException();
        }

        public async Task RejectRequestAsync(int requestId, string reason)
        {
            // TODO: GetByIdAsync, set Status=Rejected, RejectionReason=reason, Update.
            throw new NotImplementedException();
        }

        public async Task CompleteRequestAsync(int requestId, string notes)
        {
            // TODO: GetByIdAsync, set Status=Completed, Notes=notes, CompletedAt=UtcNow, Update.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmergencyRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate).
            throw new NotImplementedException();
        }

        public async Task<int> GetPendingCountAsync()
        {
            // TODO: CountAsync(e => e.Status == EmergencyRequestStatus.Pending).
            throw new NotImplementedException();
        }

    }
}