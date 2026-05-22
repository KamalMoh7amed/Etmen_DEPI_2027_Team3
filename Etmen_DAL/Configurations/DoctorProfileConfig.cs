using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class DoctorProfileConfig : IEntityTypeConfiguration<DoctorProfile>
    {
        public void Configure(EntityTypeBuilder<DoctorProfile> builder)
        {
            builder.HasOne(d => d.ApplicationUser)
                   .WithOne(u => u.DoctorProfile)
                   .HasForeignKey<DoctorProfile>(d => d.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(d => d.FullName).HasMaxLength(150);
            builder.Property(d => d.Specialization).HasMaxLength(200);
            builder.Property(d => d.LicenseNumber).HasMaxLength(50);
            builder.Property(d => d.Bio).HasMaxLength(1000);

            builder.HasIndex(d => d.ApplicationUserId).IsUnique();
            builder.HasIndex(d => d.LicenseNumber).IsUnique();
        }
    }
}