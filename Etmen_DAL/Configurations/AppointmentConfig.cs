using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // ★ CHECK على حالة الموعد
            builder.HasCheckConstraint("CK_Appointment_Status", "Status IN (1,2,3,4)");

            builder.HasOne(a => a.PatientProfile)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.DoctorProfile)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.Notes).HasMaxLength(500);
            builder.HasIndex(a => new { a.AppointmentDate, a.Status });
        }
    }
}