using Etmen_BLL.DTOs.Doctor;
using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Nearby;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using MedicalRecordCreateDto = Etmen_BLL.DTOs.Medical.MedicalRecordCreateDto;

namespace Etmen_BLL.Repositories.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _uow;

        public DoctorService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ── Profile ───────────────────────────────────────────────────────────────

        public Task<ServiceResult<DoctorProfileDto>> GetProfileAsync(string userId)
        {
            // TODO: Call _uow.DoctorProfiles.GetByUserIdAsync, map to DoctorProfileDto.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<DoctorProfileDto>> UpdateProfileAsync(string userId, DoctorProfileDto dto)
        {
            // TODO: Find profile, apply dto fields, _uow.CompleteAsync(), return updated dto.
            throw new NotImplementedException();
        }

        // ── Dashboard ─────────────────────────────────────────────────────────────

        public Task<ServiceResult<DoctorDashboardDto>> GetDashboardAsync(string userId)
        {
            // TODO: Aggregate today's appointments, pending count, recent patients
            //       into DoctorDashboardDto.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<DoctorStatisticsDto>> GetStatisticsAsync(string userId)
        {
            // TODO: Count total appointments, completed, cancelled;
            //       compute average rating or other stats into DoctorStatisticsDto.
            throw new NotImplementedException();
        }

        // ── Availability Slots ────────────────────────────────────────────────────

        public Task<ServiceResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlotsAsync(int doctorId)
        {
            // TODO: Call _uow.AvailableSlots.GetAvailableSlotsAsync(doctorId, DateTime.Today, ...),
            //       map to AvailableSlotDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AvailableSlotDto>> AddSlotAsync(string userId, CreateAvailableSlotDto dto)
        {
            // TODO: Resolve DoctorProfile from userId, map dto to AvailableSlot entity,
            //       _uow.AvailableSlots.AddAsync, CompleteAsync, return Created.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> BulkAddSlotsAsync(string userId, BulkCreateSlotsDto dto)
        {
            // TODO: Generate list of AvailableSlot entities from date range/times in dto,
            //       _uow.AvailableSlots.AddRangeAsync, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteSlotAsync(string userId, int slotId)
        {
            // TODO: Verify slot belongs to this doctor, check not already booked,
            //       Remove entity, CompleteAsync.
            throw new NotImplementedException();
        }

        // ── Appointments (doctor view) ────────────────────────────────────────────

        public Task<ServiceResult<IEnumerable<DoctorAppointmentDto>>> GetAppointmentsAsync(string userId)
        {
            // TODO: Resolve DoctorProfile, call _uow.Appointments.GetByDoctorIdAsync, map DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<DoctorAppointmentDto>> GetAppointmentAsync(string userId, int appointmentId)
        {
            // TODO: GetWithDetailsAsync, verify it belongs to this doctor, map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateAppointmentStatusAsync(string userId, int appointmentId, UpdateAppointmentStatusDto dto)
        {
            // TODO: Find appointment, verify ownership, update Status, CompleteAsync.
            throw new NotImplementedException();
        }

        // ── Patient Records ───────────────────────────────────────────────────────

        public Task<ServiceResult<IEnumerable<PatientSearchDto>>> SearchPatientsAsync(string searchTerm)
        {
            // TODO: Query ApplicationUser / PatientProfile by name or email containing searchTerm,
            //       map to PatientSearchDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<MedicalRecordDto>> AddMedicalRecordForPatientAsync(string doctorUserId, MedicalRecordCreateDto dto)
        {
            // TODO: Verify doctor exists, resolve PatientProfile from dto.PatientId,
            //       create MedicalRecord entity, AddAsync, CompleteAsync, return Created.
            throw new NotImplementedException();
        }
    }
}
