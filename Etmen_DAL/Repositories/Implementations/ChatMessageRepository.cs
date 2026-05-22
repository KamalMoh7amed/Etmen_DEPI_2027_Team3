using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<ChatMessage>> GetByConversationAsync(string userId1, string userId2)
        {
            // TODO: FindAsync(m => (m.SenderId==userId1 && m.ReceiverId==userId2) || (m.SenderId==userId2 && m.ReceiverId==userId1)), order by SentAt.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ChatMessage>> GetUnreadMessagesAsync(string receiverId, string senderId)
        {
            // TODO: FindAsync(m => m.ReceiverId==receiverId && m.SenderId==senderId && !m.IsRead).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(string userId, int count)
        {
            // TODO: _dbSet.Where(m=>m.SenderId==userId||m.ReceiverId==userId).OrderByDescending(m=>m.SentAt).Take(count).
            throw new NotImplementedException();
        }

        public async Task MarkAsReadAsync(string receiverId, string senderId)
        {
            // TODO: Find unread messages for receiverId from senderId, set IsRead=true, UpdateRange.
            throw new NotImplementedException();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            // TODO: CountAsync(m => m.ReceiverId==userId && !m.IsRead).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByDateRangeAsync(string userId1, string userId2, DateTime startDate, DateTime endDate)
        {
            // TODO: Filter conversation messages by date range.
            throw new NotImplementedException();
        }

    }
}