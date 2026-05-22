using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class HealthcareProviderConfig : IEntityTypeConfiguration<HealthcareProvider>
    {
        public void Configure(EntityTypeBuilder<HealthcareProvider> builder)
        {
            // ★ دقة عالية للإحداثيات الجغرافية
            builder.Property(p => p.Latitude).HasColumnType("decimal(9,6)");
            builder.Property(p => p.Longitude).HasColumnType("decimal(9,6)");

            builder.Property(p => p.Name).HasMaxLength(200);
            builder.Property(p => p.Address).HasMaxLength(500);
            builder.Property(p => p.Phone).HasMaxLength(20);

            builder.HasIndex(p => new { p.Latitude, p.Longitude });
            builder.HasIndex(p => p.IsEmergencyCenter);
        }
    }
}