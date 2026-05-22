using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Etmen_Domain.Entities;

namespace Etmen_DAL.Configurations
{
    public class ChatMessageConfig : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            // ★ NO ACTION لتجنب خطأ Multiple Cascade Paths
            builder.HasOne(c => c.Sender)
                   .WithMany(u => u.SentMessages)
                   .HasForeignKey(c => c.SenderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Receiver)
                   .WithMany(u => u.ReceivedMessages)
                   .HasForeignKey(c => c.ReceiverId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(c => c.Message).HasMaxLength(2000);
            builder.HasIndex(c => c.SentAt);
        }
    }
}