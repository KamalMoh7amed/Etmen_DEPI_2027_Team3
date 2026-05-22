using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
        {
            // TODO: FindAsync(n => n.UserId == userId) ordered by CreatedAt desc.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            // TODO: FindAsync(n => n.UserId==userId && !n.IsRead).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Notification>> GetLatestNotificationsAsync(string userId, int count)
        {
            // TODO: _dbSet.Where(n=>n.UserId==userId).OrderByDescending(n=>n.CreatedAt).Take(count).ToListAsync().
            throw new NotImplementedException();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            // TODO: GetByIdAsync, set IsRead=true, Update.
            throw new NotImplementedException();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            // TODO: FindAsync all unread, set IsRead=true, UpdateRange.
            throw new NotImplementedException();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            // TODO: CountAsync(n => n.UserId==userId && !n.IsRead).
            throw new NotImplementedException();
        }

        public async Task DeleteNotificationAsync(int notificationId, string userId)
        {
            // TODO: FirstOrDefaultAsync with id and userId, Remove.
            throw new NotImplementedException();
        }

    }
}