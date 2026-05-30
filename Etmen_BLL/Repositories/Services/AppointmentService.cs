using Etmen_BLL.DTOs.Nearby;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Etmen_Domain.Enums;

namespace Etmen_BLL.Repositories.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _uow;

        public AppointmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<AppointmentDto>> BookAppointmentAsync(string userId, BookingRequestDto dto)
        {
            var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
            if (patient == null)
                return ServiceResult<AppointmentDto>.NotFound("Patient not found");

            var slot = await _uow.AvailableSlots.GetByIdAsync(dto.SlotId);
            if (slot == null)
                return ServiceResult<AppointmentDto>.NotFound("Slot not found");

            if (slot.IsBooked)
                return ServiceResult<AppointmentDto>.Conflict("Slot is already booked");

            await _uow.BeginTransactionAsync();
            try
            {
                var appointment = new Appointment
                {
                    PatientProfileId = patient.Id,
                    DoctorProfileId = dto.DoctorId,
                    AppointmentDate = dto.Date,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Notes = dto.Notes,
                    Status = AppointmentStatus.Scheduled
                };

                await _uow.Appointments.AddAsync(appointment);
                await _uow.AvailableSlots.MarkSlotAsBookedAsync(dto.SlotId);
                await _uow.CompleteAsync();
                await _uow.CommitTransactionAsync();

                var result = new AppointmentDto
                {
                    Id = appointment.Id,
                    PatientId = patient.Id,
                    DoctorId = dto.DoctorId,
                    Date = appointment.AppointmentDate,
                    StartTime = appointment.StartTime,
                    Status = appointment.Status.ToString(),
                    Notes = appointment.Notes
                };
                return ServiceResult<AppointmentDto>.Created(result);
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<AppointmentDto>>> GetPatientAppointmentsAsync(string userId)
        {
            var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
            if (patient == null)
                return ServiceResult<IEnumerable<AppointmentDto>>.NotFound("Patient not found");

            var appointments = await _uow.Appointments.GetByPatientIdAsync(patient.Id);
            var dtos = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientProfileId,
                DoctorId = a.DoctorProfileId ?? 0,
                Date = a.AppointmentDate,
                StartTime = a.StartTime,
                Status = a.Status.ToString(),
                Notes = a.Notes
            });
            return ServiceResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }

        public async Task<ServiceResult<AppointmentDto>> GetAppointmentByIdAsync(string userId, int appointmentId)
        {
            var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
            if (patient == null)
                return ServiceResult<AppointmentDto>.NotFound("Patient not found");

            var appointment = await _uow.Appointments.GetWithDetailsAsync(appointmentId);
            if (appointment == null || appointment.PatientProfileId != patient.Id)
                return ServiceResult<AppointmentDto>.NotFound("Appointment not found");

            var dto = new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientProfileId,
                DoctorId = appointment.DoctorProfileId ?? 0,
                Date = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                Status = appointment.Status.ToString(),
                Notes = appointment.Notes
            };
            return ServiceResult<AppointmentDto>.Success(dto);
        }

        public async Task<ServiceResult> CancelAppointmentAsync(string userId, int appointmentId)
        {
            var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
            if (patient == null)
                return ServiceResult.NotFound("Patient not found");

            var appointment = await _uow.Appointments.GetWithDetailsAsync(appointmentId);
            if (appointment == null || appointment.PatientProfileId != patient.Id)
                return ServiceResult.NotFound("Appointment not found");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return ServiceResult.Failure("Appointment is already cancelled");

            await _uow.Appointments.CancelAppointmentAsync(appointmentId, "Cancelled by patient");

            var slots = await _uow.AvailableSlots.GetByDoctorIdAndDateAsync(appointment.DoctorProfileId ?? 0, appointment.AppointmentDate);
            var matchingSlot = slots.FirstOrDefault(s => s.SlotStart == appointment.StartTime);
            if (matchingSlot != null)
                await _uow.AvailableSlots.MarkSlotAsAvailableAsync(matchingSlot.Id);

            await _uow.CompleteAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var slots = await _uow.AvailableSlots.GetByDoctorIdAndDateAsync(doctorId, date);
            var dtos = slots.Where(s => !s.IsBooked).Select(s => new AvailableSlotDto
            {
                Id = s.Id,
                DoctorId = s.DoctorProfileId,
                Date = s.SlotDate,
                StartTime = s.SlotStart,
                EndTime = s.SlotEnd,
                IsBooked = s.IsBooked
            });
            return ServiceResult<IEnumerable<AvailableSlotDto>>.Success(dtos);
        }

        public async Task<ServiceResult<IEnumerable<AppointmentDto>>> GetUpcomingAppointmentsAsync(string userId)
        {
            var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
            if (patient == null)
                return ServiceResult<IEnumerable<AppointmentDto>>.NotFound("Patient not found");

            var appointments = await _uow.Appointments.GetUpcomingAppointmentsAsync(patient.Id);
            var dtos = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientProfileId,
                DoctorId = a.DoctorProfileId ?? 0,
                Date = a.AppointmentDate,
                StartTime = a.StartTime,
                Status = a.Status.ToString(),
                Notes = a.Notes
            });
            return ServiceResult<IEnumerable<AppointmentDto>>.Success(dtos);
        }
    }
}
