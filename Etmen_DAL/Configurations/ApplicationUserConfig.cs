using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);
            builder.Property(u => u.ProfilePicture).HasMaxLength(500);
            builder.Property(u => u.VerificationToken).HasMaxLength(256);
            builder.Property(u => u.ResetPasswordToken).HasMaxLength(256);

            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}