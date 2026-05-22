using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class FamilyLinkRepository : GenericRepository<FamilyLink>, IFamilyLinkRepository
    {
        public FamilyLinkRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<FamilyLink>> GetByPrimaryPatientIdAsync(int primaryPatientId)
        {
            // TODO: FindAsync(f => f.PrimaryPatientId == primaryPatientId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FamilyLink>> GetByLinkedPatientIdAsync(int linkedPatientId)
        {
            // TODO: FindAsync(f => f.LinkedPatientId == linkedPatientId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PatientProfile>> GetFamilyMembersAsync(int patientId)
        {
            // TODO: _dbSet.Where(f=>f.PrimaryPatientId==patientId).Include(f=>f.LinkedPatient).Select(f=>f.LinkedPatient).ToListAsync().
            throw new NotImplementedException();
        }

        public async Task<FamilyLink?> GetByInviteTokenAsync(string inviteToken)
        {
            // TODO: FirstOrDefaultAsync(f => f.InviteToken == inviteToken).
            throw new NotImplementedException();
        }

        public async Task<bool> IsFamilyLinkExistsAsync(int primaryPatientId, int linkedPatientId)
        {
            // TODO: AnyAsync(f => f.PrimaryPatientId==primaryPatientId && f.LinkedPatientId==linkedPatientId).
            throw new NotImplementedException();
        }

        public async Task UpdatePermissionsAsync(int familyLinkId, bool canViewRecords, bool canViewRisk, bool canBookAppointments)
        {
            // TODO: GetByIdAsync, update permission fields, Update.
            throw new NotImplementedException();
        }

        public async Task AcceptInviteAsync(string inviteToken, int linkedPatientId)
        {
            // TODO: GetByInviteTokenAsync, set LinkedPatientId=linkedPatientId, IsAccepted=true, Update.
            throw new NotImplementedException();
        }

    }
}