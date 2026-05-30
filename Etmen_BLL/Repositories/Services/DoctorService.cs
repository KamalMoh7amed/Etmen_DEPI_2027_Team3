using Etmen_BLL.DTOs.Doctor;
using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Nearby;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
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

        public async Task<ServiceResult<DoctorProfileDto>> GetProfileAsync(string userId)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult<DoctorProfileDto>.NotFound("Doctor not found");

            var dto = new DoctorProfileDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName ?? "",
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee,
                IsAvailable = doctor.IsAvailable,
                CreatedAt = doctor.CreatedAt,
                UpdatedAt = doctor.UpdatedAt
            };
            return ServiceResult<DoctorProfileDto>.Success(dto);
        }

        public async Task<ServiceResult<DoctorProfileDto>> UpdateProfileAsync(string userId, DoctorProfileDto dto)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult<DoctorProfileDto>.NotFound("Doctor not found");

            doctor.FullName = dto.FullName;
            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.YearsOfExperience = dto.YearsOfExperience;
            doctor.Bio = dto.Bio;
            doctor.ConsultationFee = dto.ConsultationFee;
            doctor.IsAvailable = dto.IsAvailable;
            doctor.UpdatedAt = DateTime.UtcNow;

            _uow.DoctorProfiles.Update(doctor);
            await _uow.CompleteAsync();

            dto.Id = doctor.Id;
            dto.CreatedAt = doctor.CreatedAt;
            dto.UpdatedAt = doctor.UpdatedAt;
            return ServiceResult<DoctorProfileDto>.Success(dto);
        }

        public async Task<ServiceResult<DoctorDashboardDto>> GetDashboardAsync(string userId)
        {
            var doctor = await _uow.DoctorProfiles.GetWithAppointmentsAsync(userId);
            if (doctor == null)
                return ServiceResult<DoctorDashboardDto>.NotFound("Doctor not found");

            var today = DateTime.UtcNow.Date;
            var appointments = doctor.Appointments.ToList();

            var dashboard = new DoctorDashboardDto
            {
                DoctorName = doctor.FullName ?? "",
                Specialization = doctor.Specialization,
                TodayAppointmentsCount = appointments.Count(a => a.AppointmentDate.Date == today),
                PendingAppointmentsCount = appointments.Count(a => a.Status == AppointmentStatus.Scheduled),
                TotalPatientsCount = appointments.Select(a => a.PatientProfileId).Distinct().Count(),
                UpcomingAppointments = appointments
                    .Where(a => a.AppointmentDate >= today && a.Status == AppointmentStatus.Scheduled)
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5)
                    .Select(a => new UpcomingAppointmentDto
                    {
                        Id = a.Id,
                        PatientName = a.PatientProfile?.FullName ?? "",
                        AppointmentDate = a.AppointmentDate,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Status = a.Status.ToString(),
                        Notes = a.Notes
                    }).ToList()
            };
            return ServiceResult<DoctorDashboardDto>.Success(dashboard);
        }

        public async Task<ServiceResult<DoctorStatisticsDto>> GetStatisticsAsync(string userId)
        {
            var doctor = await _uow.DoctorProfiles.GetWithAppointmentsAsync(userId);
            if (doctor == null)
                return ServiceResult<DoctorStatisticsDto>.NotFound("Doctor not found");

            var appointments = doctor.Appointments.ToList();
            var total = appointments.Count;
            var completed = appointments.Count(a => a.Status == AppointmentStatus.Completed);

            var stats = new DoctorStatisticsDto
            {
                TotalAppointments = total,
                CompletedAppointments = completed,
                CancelledAppointments = appointments.Count(a => a.Status == AppointmentStatus.Cancelled),
                NoShowAppointments = appointments.Count(a => a.Status == AppointmentStatus.NoShow),
                CompletionRate = total > 0 ? Math.Round((decimal)completed / total * 100, 2) : 0,
                TotalPatients = appointments.Select(a => a.PatientProfileId).Distinct().Count(),
                NewPatientsThisMonth = appointments.Where(a => a.CreatedAt.Month == DateTime.UtcNow.Month && a.CreatedAt.Year == DateTime.UtcNow.Year).Select(a => a.PatientProfileId).Distinct().Count(),
                AverageConsultationFee = doctor.ConsultationFee,
                PeriodStart = appointments.Min(a => (DateTime?)a.AppointmentDate) ?? DateTime.UtcNow,
                PeriodEnd = DateTime.UtcNow
            };
            return ServiceResult<DoctorStatisticsDto>.Success(stats);
        }

        public async Task<ServiceResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlotsAsync(int doctorId)
        {
            var slots = await _uow.AvailableSlots.GetAvailableSlotsAsync(doctorId, DateTime.Today, DateTime.Today.AddMonths(1));
            var dtos = slots.Select(s => new AvailableSlotDto
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

        public async Task<ServiceResult<AvailableSlotDto>> AddSlotAsync(string userId, CreateAvailableSlotDto dto)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult<AvailableSlotDto>.NotFound("Doctor not found");

            var slot = new AvailableSlot
            {
                DoctorProfileId = doctor.Id,
                SlotDate = dto.SlotDate,
                SlotStart = dto.SlotStart,
                SlotEnd = dto.SlotEnd,
                IsBooked = false
            };

            await _uow.AvailableSlots.AddAsync(slot);
            await _uow.CompleteAsync();

            var result = new AvailableSlotDto
            {
                Id = slot.Id,
                DoctorId = slot.DoctorProfileId,
                Date = slot.SlotDate,
                StartTime = slot.SlotStart,
                EndTime = slot.SlotEnd,
                IsBooked = slot.IsBooked
            };
            return ServiceResult<AvailableSlotDto>.Created(result);
        }

        public async Task<ServiceResult> BulkAddSlotsAsync(string userId, BulkCreateSlotsDto dto)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult.NotFound("Doctor not found");

            var slots = new List<AvailableSlot>();
            for (var date = dto.StartDate.Date; date <= dto.EndDate.Date; date = date.AddDays(1))
            {
                if (dto.ExcludedDays.Contains(date.DayOfWeek))
                    continue;

                var time = dto.DailyStartTime;
                while (time + TimeSpan.FromMinutes(dto.SlotDurationMinutes) <= dto.DailyEndTime)
                {
                    slots.Add(new AvailableSlot
                    {
                        DoctorProfileId = doctor.Id,
                        SlotDate = date,
                        SlotStart = time,
                        SlotEnd = time + TimeSpan.FromMinutes(dto.SlotDurationMinutes),
                        IsBooked = false
                    });
                    time = time.Add(TimeSpan.FromMinutes(dto.SlotDurationMinutes));
                }
            }

            await _uow.AvailableSlots.AddRangeAsync(slots);
            await _uow.CompleteAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteSlotAsync(string userId, int slotId)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult.NotFound("Doctor not found");

            var slot = await _uow.AvailableSlots.GetByIdAsync(slotId);
            if (slot == null)
                return ServiceResult.NotFound("Slot not found");

            if (slot.DoctorProfileId != doctor.Id)
                return ServiceResult.Forbidden("Not your slot");

            if (slot.IsBooked)
                return ServiceResult.Failure("Cannot delete a booked slot");

            _uow.AvailableSlots.Remove(slot);
            await _uow.CompleteAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult<IEnumerable<DoctorAppointmentDto>>> GetAppointmentsAsync(string userId)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult<IEnumerable<DoctorAppointmentDto>>.NotFound("Doctor not found");

            var appointments = await _uow.Appointments.GetByDoctorIdAsync(doctor.Id);
            var dtos = appointments.Select(a => new DoctorAppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientProfileId,
                PatientName = a.PatientProfile?.FullName ?? "",
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status.ToString(),
                Notes = a.Notes,
                CreatedAt = a.CreatedAt
            });
            return ServiceResult<IEnumerable<DoctorAppointmentDto>>.Success(dtos);
        }

        public async Task<ServiceResult<DoctorAppointmentDto>> GetAppointmentAsync(string userId, int appointmentId)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult<DoctorAppointmentDto>.NotFound("Doctor not found");

            var appointment = await _uow.Appointments.GetWithDetailsAsync(appointmentId);
            if (appointment == null || appointment.DoctorProfileId != doctor.Id)
                return ServiceResult<DoctorAppointmentDto>.NotFound("Appointment not found");

            var dto = new DoctorAppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientProfileId,
                PatientName = appointment.PatientProfile?.FullName ?? "",
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status.ToString(),
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt
            };
            return ServiceResult<DoctorAppointmentDto>.Success(dto);
        }

        public async Task<ServiceResult> UpdateAppointmentStatusAsync(string userId, int appointmentId, UpdateAppointmentStatusDto dto)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(userId);
            if (doctor == null)
                return ServiceResult.NotFound("Doctor not found");

            var appointment = await _uow.Appointments.GetWithDetailsAsync(appointmentId);
            if (appointment == null || appointment.DoctorProfileId != doctor.Id)
                return ServiceResult.NotFound("Appointment not found");

            if (Enum.TryParse<AppointmentStatus>(dto.Status, out var status))
                appointment.Status = status;
            else
                return ServiceResult.Failure("Invalid status");

            appointment.Notes = dto.Notes;
            appointment.UpdatedAt = DateTime.UtcNow;
            _uow.Appointments.Update(appointment);
            await _uow.CompleteAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult<IEnumerable<PatientSearchDto>>> SearchPatientsAsync(string searchTerm)
        {
            var patients = await _uow.PatientProfiles.FindAsync(p => p.FullName != null && p.FullName.Contains(searchTerm));
            var dtos = patients.Select(p => new PatientSearchDto
            {
                SearchTerm = p.FullName
            });
            return ServiceResult<IEnumerable<PatientSearchDto>>.Success(dtos);
        }

        public async Task<ServiceResult<MedicalRecordDto>> AddMedicalRecordForPatientAsync(string doctorUserId, MedicalRecordCreateDto dto)
        {
            var doctor = await _uow.DoctorProfiles.GetByUserIdAsync(doctorUserId);
            if (doctor == null)
                return ServiceResult<MedicalRecordDto>.NotFound("Doctor not found");

            var patient = await _uow.PatientProfiles.GetByIdAsync(dto.PatientId);
            if (patient == null)
                return ServiceResult<MedicalRecordDto>.NotFound("Patient not found");

            var record = new MedicalRecord
            {
                PatientProfileId = dto.PatientId,
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

            var result = new MedicalRecordDto
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
            return ServiceResult<MedicalRecordDto>.Created(result);
        }
    }
}
