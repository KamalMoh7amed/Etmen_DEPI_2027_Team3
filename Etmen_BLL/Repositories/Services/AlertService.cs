using Etmen_BLL.DTOs.Alert;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork _uow;

        public AlertService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<List<AlertDto>>> GetUserAlertsAsync(string userId)
        {
            // TODO: _uow.Alerts.GetByUserIdAsync(userId), map to AlertDto list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<AlertDto>>> GetUnreadAlertsAsync(string userId)
        {
            // TODO: _uow.Alerts.GetUnreadAlertsAsync(userId), map to list.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AlertDto>> GetAlertByIdAsync(int alertId)
        {
            // TODO: _uow.Alerts.GetByIdAsync(alertId), map to AlertDto.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AlertDto>> CreateAlertAsync(int userId, string title, string message, string alertType)
        {
            // TODO: Build Alert entity, _uow.Alerts.AddAsync, CompleteAsync, return Created result.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> MarkAsReadAsync(int alertId)
        {
            // TODO: _uow.Alerts.MarkAsReadAsync(alertId), CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> MarkAllAsReadAsync(string userId)
        {
            // TODO: _uow.Alerts.MarkAllAsReadAsync(userId), CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeleteAlertAsync(int alertId)
        {
            // TODO: GetByIdAsync, Remove entity, CompleteAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<int>> GetUnreadCountAsync(string userId)
        {
            // TODO: _uow.Alerts.GetUnreadCountAsync(userId), wrap in ServiceResult.
            throw new NotImplementedException();
        }
    }
}
