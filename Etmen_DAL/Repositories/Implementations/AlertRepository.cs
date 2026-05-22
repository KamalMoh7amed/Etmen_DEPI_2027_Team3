using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        public AlertRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<Alert>> GetByUserIdAsync(string userId)
        {
            // TODO: FindAsync(a => a.UserId == userId), order by CreatedAt desc.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Alert>> GetUnreadAlertsAsync(string userId)
        {
            // TODO: FindAsync(a => a.UserId == userId && !a.IsRead).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Alert>> GetByTypeAsync(string userId, string alertType)
        {
            // TODO: FindAsync(a => a.UserId == userId && a.AlertType == alertType).
            throw new NotImplementedException();
        }

        public async Task MarkAsReadAsync(int alertId)
        {
            // TODO: GetByIdAsync, set IsRead = true, Update.
            throw new NotImplementedException();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            // TODO: FindAsync all unread for userId, set IsRead = true, UpdateRange.
            throw new NotImplementedException();
        }

        public async Task DismissAlertAsync(int alertId)
        {
            // TODO: GetByIdAsync, set IsDismissed = true, Update.
            throw new NotImplementedException();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            // TODO: CountAsync(a => a.UserId == userId && !a.IsRead).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Alert>> GetByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync(a => a.UserId == userId && a.CreatedAt >= startDate && a.CreatedAt <= endDate).
            throw new NotImplementedException();
        }

    }
}