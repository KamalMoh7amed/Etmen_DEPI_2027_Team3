using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            // TODO: FindAsync(a => a.PatientProfileId == patientId), include Patient and Doctor.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            // TODO: FindAsync(a => a.DoctorProfileId == doctorId), include Patient.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            // TODO: FindAsync(a => a.PatientProfileId == patientId && a.AppointmentDate > DateTime.UtcNow && a.Status == Scheduled).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date)
        {
            // TODO: FindAsync(a => a.AppointmentDate.Date == date.Date).
            throw new NotImplementedException();
        }

        public async Task<Appointment?> GetWithDetailsAsync(int appointmentId)
        {
            // TODO: _dbSet.Include(a=>a.PatientProfile).Include(a=>a.DoctorProfile).FirstOrDefaultAsync(a=>a.Id==appointmentId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        {
            // TODO: FindAsync(a => a.Status == status).
            throw new NotImplementedException();
        }

        public async Task<int> GetAppointmentsCountByDateAsync(DateTime date, int? doctorId = null)
        {
            // TODO: CountAsync with date filter and optional doctorId filter.
            throw new NotImplementedException();
        }

        public async Task CancelAppointmentAsync(int appointmentId, string cancellationReason)
        {
            // TODO: GetByIdAsync, set Status=Cancelled, CancellationReason=reason, Update.
            throw new NotImplementedException();
        }

    }
}