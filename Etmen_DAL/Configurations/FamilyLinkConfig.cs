using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class FamilyLinkConfig : IEntityTypeConfiguration<FamilyLink>
    {
        public void Configure(EntityTypeBuilder<FamilyLink> builder)
        {
            // ★ UNIQUE Pair لمنع تكرار الربط بين نفس المريضين
            builder.HasIndex(f => new { f.PrimaryPatientId, f.LinkedPatientId }).IsUnique();

            builder.HasOne(f => f.PrimaryPatient)
                   .WithMany(p => p.PrimaryLinks)
                   .HasForeignKey(f => f.PrimaryPatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.LinkedPatient)
                   .WithMany(p => p.LinkedLinks)
                   .HasForeignKey(f => f.LinkedPatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(f => f.Relationship).HasMaxLength(50);
            builder.Property(f => f.InviteToken).HasMaxLength(100);
        }
    }
}