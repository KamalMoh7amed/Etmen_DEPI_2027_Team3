using Etmen_BLL.DTOs.Nearby;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _uow;

        public AppointmentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<AppointmentDto>> BookAppointmentAsync(string userId, BookingRequestDto dto)
        {
            // TODO: Resolve PatientProfile, verify slot is available via _uow.AvailableSlots,
            //       create Appointment entity, mark slot as booked, CompleteAsync, return Created.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IEnumerable<AppointmentDto>>> GetPatientAppointmentsAsync(string userId)
        {
            // TODO: Resolve PatientProfile, _uow.Appointments.GetByPatientIdAsync, map to DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AppointmentDto>> GetAppointmentByIdAsync(string userId, int appointmentId)
        {
            // TODO: GetWithDetailsAsync, verify ownership, map to DTO.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> CancelAppointmentAsync(string userId, int appointmentId)
        {
            // TODO: Verify ownership, check cancellation policy,
            //       _uow.Appointments.CancelAppointmentAsync, free slot, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            // TODO: _uow.AvailableSlots.GetByDoctorIdAndDateAsync(doctorId, date), map to DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<IEnumerable<AppointmentDto>>> GetUpcomingAppointmentsAsync(string userId)
        {
            // TODO: Resolve PatientProfile, _uow.Appointments.GetUpcomingAppointmentsAsync, map to DTOs.
            throw new NotImplementedException();
        }
    }
}
