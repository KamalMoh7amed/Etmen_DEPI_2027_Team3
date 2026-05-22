using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class EmergencyRequestConfig : IEntityTypeConfiguration<EmergencyRequest>
    {
        public void Configure(EntityTypeBuilder<EmergencyRequest> builder)
        {
            // ★ CHECK على حالة الطلب
            builder.HasCheckConstraint("CK_EmergencyRequest_Status", "Status IN (1,2,3,4,5,6)");

            // ★ NO ACTION لتجنب Multiple Cascade Paths
            builder.HasOne(e => e.PatientProfile)
                   .WithMany(p => p.EmergencyRequests)
                   .HasForeignKey(e => e.PatientProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.HealthcareProvider)
                   .WithMany()
                   .HasForeignKey(e => e.HealthcareProviderId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(e => e.EmergencyType).HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
            builder.Property(e => e.Longitude).HasColumnType("decimal(9,6)");
            builder.Property(e => e.ResponseNotes).HasMaxLength(500);

            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.RequestedAt);
        }
    }
}