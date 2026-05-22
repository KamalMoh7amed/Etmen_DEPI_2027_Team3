using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class AvailableSlotConfig : IEntityTypeConfiguration<AvailableSlot>
    {
        public void Configure(EntityTypeBuilder<AvailableSlot> builder)
        {
            // ★ UNIQUE Constraint لمنع حجز نفس الوقت مرتين لنفس الطبيب
            builder.HasIndex(s => new { s.DoctorProfileId, s.SlotDate, s.SlotStart }).IsUnique();

            builder.HasOne(s => s.DoctorProfile)
                   .WithMany(d => d.AvailableSlots)
                   .HasForeignKey(s => s.DoctorProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}