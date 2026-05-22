using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class CrisisConfigurationConfig : IEntityTypeConfiguration<CrisisConfiguration>
    {
        public void Configure(EntityTypeBuilder<CrisisConfiguration> builder)
        {
            // ★ CHECK على نوع الأزمة
            builder.HasCheckConstraint("CK_CrisisConfiguration_Type", "CrisisType IN (1,2,3,4)");

            // ★ SymptomWeight كـ Owned Entity (يُخزن في جدول منفصل مرتبط بـ CrisisConfigurationId)
            builder.OwnsMany(c => c.SymptomWeights, sw =>
            {
                sw.WithOwner().HasForeignKey("CrisisConfigurationId");
                sw.Property(x => x.SymptomName).HasMaxLength(150);
                sw.Property(x => x.Weight).HasColumnType("decimal(3,2)");
                sw.ToTable("CrisisSymptomWeights");
            });

            builder.HasMany(c => c.OutbreakZones)
                   .WithOne(z => z.CrisisConfiguration)
                   .HasForeignKey(z => z.CrisisConfigurationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.CrisisName).HasMaxLength(150);
            builder.Property(c => c.Description).HasMaxLength(1000);
        }
    }
}