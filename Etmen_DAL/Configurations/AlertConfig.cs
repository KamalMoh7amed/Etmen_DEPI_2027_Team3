using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class AlertConfig : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.HasOne(a => a.User)
                   .WithMany(u => u.Alerts)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.Title).HasMaxLength(200);
            builder.Property(a => a.Message).HasMaxLength(1000);
            builder.Property(a => a.AlertType).HasMaxLength(50);

            builder.HasIndex(a => new { a.UserId, a.Status });
        }
    }
}