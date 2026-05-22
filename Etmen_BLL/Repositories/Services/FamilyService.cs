using Etmen_BLL.DTOs.Family;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class FamilyService : IFamilyService
    {
        private readonly IUnitOfWork _uow;

        public FamilyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<FamilyDto>> InviteFamilyMemberAsync(FamilyInviteDto dto)
        {
            // TODO: Create FamilyLink with a generated InviteToken (Guid), set permissions from dto,
            //       AddAsync, CompleteAsync, send invitation email/notification, return Created.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> AcceptFamilyInviteAsync(string inviteToken)
        {
            // TODO: _uow.FamilyLinks.GetByInviteTokenAsync(inviteToken),
            //       verify token not expired, AcceptInviteAsync, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<FamilyDto>>> GetFamilyMembersAsync(int patientProfileId)
        {
            // TODO: _uow.FamilyLinks.GetByPrimaryPatientIdAsync(patientProfileId), map to FamilyDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> RemoveFamilyMemberAsync(int familyLinkId)
        {
            // TODO: GetByIdAsync, Remove entity, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateFamilyPermissionsAsync(int familyLinkId, FamilyDto dto)
        {
            // TODO: _uow.FamilyLinks.UpdatePermissionsAsync with values from dto, CompleteAsync.
            throw new NotImplementedException();
        }
    }
}
