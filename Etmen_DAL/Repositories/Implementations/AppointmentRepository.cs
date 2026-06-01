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
            return await _dbSet.Include(a => a.PatientProfile).Include(a => a.DoctorProfile).Where(a => a.PatientProfileId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet.Include(a => a.PatientProfile).Where(a => a.DoctorProfileId == doctorId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            return await _dbSet.Where(a => a.PatientProfileId == patientId && a.AppointmentDate > DateTime.UtcNow && a.Status == AppointmentStatus.Scheduled).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date)
        {
            return await FindAsync(a => a.AppointmentDate.Date == date.Date);
        }

        public async Task<Appointment?> GetWithDetailsAsync(int appointmentId)
        {
            return await _dbSet.Include(a => a.PatientProfile).Include(a => a.DoctorProfile).FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        {
            return await FindAsync(a => a.Status == status);
        }

        public async Task<int> GetAppointmentsCountByDateAsync(DateTime date, int? doctorId = null)
        {
            if (doctorId.HasValue)
                return await CountAsync(a => a.AppointmentDate.Date == date.Date && a.DoctorProfileId == doctorId.Value);
            return await CountAsync(a => a.AppointmentDate.Date == date.Date);
        }

        public async Task CancelAppointmentAsync(int appointmentId, string cancellationReason)
        {
            var appointment = await GetByIdAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.Notes = cancellationReason;
                appointment.UpdatedAt = DateTime.UtcNow;
                Update(appointment);
            }
        }

    }
}
