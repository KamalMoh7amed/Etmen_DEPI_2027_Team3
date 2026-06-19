using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;

namespace Etmen_BLL.Repositories.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IUnitOfWork _uow;

        public MedicalRecordService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>> GetByPatientAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.GetByUserIdAsync(userId);

                if (patient == null)
                    return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.NotFound("Patient not found");

                var records = await _uow.MedicalRecords.GetByPatientIdAsync(patient.Id);

                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Success(records.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>> GetByIdAsync(string userId, int recordId)
        {
            try
            {
                var record = await _uow.MedicalRecords.GetByIdAsync(recordId);

                if (record == null)
                    return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound("Record not found");

                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Success(MapToDto(record));
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>> GetLatestAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.GetByUserIdAsync(userId);

                if (patient == null)
                    return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound("Patient not found");

                var record = await _uow.MedicalRecords.GetLatestByPatientIdAsync(patient.Id);

                if (record == null)
                    return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound("No record found");

                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Success(MapToDto(record));
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>> CreateAsync(string userId, MedicalRecordCreateDto dto)
        {
            try
            {
                var patient = await _uow.PatientProfiles.GetByUserIdAsync(userId);

                if (patient == null)
                    return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound("Patient not found");

                var record = new MedicalRecord
                {
                    PatientProfileId = patient.Id,
                    RecordDate = dto.RecordDate,
                    SystolicBP = dto.SystolicBP,
                    DiastolicBP = dto.DiastolicBP,
                    BloodSugar = dto.BloodSugar,
                    HeartRate = dto.HeartRate,
                    Temperature = dto.Temperature,
                    OxygenSaturation = dto.OxygenSaturation,
                    Symptoms = dto.Symptoms,
                    Notes = dto.Notes
                };

                await _uow.MedicalRecords.AddAsync(record);
                await _uow.CompleteAsync();

                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Created(MapToDto(record));
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult> DeleteAsync(string userId, int recordId)
        {
            try
            {
                var record = await _uow.MedicalRecords.GetByIdAsync(recordId);

                if (record == null)
                    return Etmen_BLL.Helpers.ServiceResult.NotFound("Record not found");

                _uow.MedicalRecords.Remove(record);
                await _uow.CompleteAsync();

                return Etmen_BLL.Helpers.ServiceResult.Success(200);
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>> GetByDateRangeAsync(
            string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var patient = await _uow.PatientProfiles.GetByUserIdAsync(userId);

                if (patient == null)
                    return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.NotFound("Patient not found");

                var records = await _uow.MedicalRecords.GetByDateRangeAsync(patient.Id, startDate, endDate);

                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Success(records.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Failure(ex.Message);
            }
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>> GetWithAbnormalValuesAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.GetByUserIdAsync(userId);

                if (patient == null)
                    return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.NotFound("Patient not found");

                var records = await _uow.MedicalRecords.GetWithAbnormalValuesAsync(patient.Id);

                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Success(records.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Failure(ex.Message);
            }
        }

        private static MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                Id = record.Id,
                RecordDate = record.RecordDate,
                SystolicBP = record.SystolicBP,
                DiastolicBP = record.DiastolicBP,
                BloodSugar = record.BloodSugar,
                HeartRate = record.HeartRate,
                Temperature = record.Temperature,
                OxygenSaturation = record.OxygenSaturation,
                Symptoms = record.Symptoms,
                Notes = record.Notes
            };
        }
    }
}