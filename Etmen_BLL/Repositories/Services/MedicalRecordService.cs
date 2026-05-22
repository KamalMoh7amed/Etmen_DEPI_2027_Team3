using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOfWork _uow;

        public MedicalRecordService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<IEnumerable<MedicalRecordDto>>> GetByPatientAsync(string userId)
        {
            // TODO: Resolve PatientProfile from userId,
            //       _uow.MedicalRecords.GetByPatientIdAsync, map to DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<MedicalRecordDto>> GetByIdAsync(string userId, int recordId)
        {
            // TODO: GetByIdAsync(recordId), verify it belongs to this patient, map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<MedicalRecordDto>> GetLatestAsync(string userId)
        {
            // TODO: Resolve PatientProfile, GetLatestByPatientIdAsync, map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<MedicalRecordDto>> CreateAsync(string userId, MedicalRecordCreateDto dto)
        {
            // TODO: Resolve PatientProfile, map dto to MedicalRecord entity,
            //       AddAsync (with symptoms if any), CompleteAsync, return Created.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteAsync(string userId, int recordId)
        {
            // TODO: Verify ownership, Remove entity, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IEnumerable<MedicalRecordDto>>> GetByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            // TODO: Resolve PatientProfile,
            //       _uow.MedicalRecords.GetByDateRangeAsync(patientId, startDate, endDate), map list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IEnumerable<MedicalRecordDto>>> GetWithAbnormalValuesAsync(string userId)
        {
            // TODO: Resolve PatientProfile, GetWithAbnormalValuesAsync(patientId), map list.
            throw new NotImplementedException();
        }
    }
}
