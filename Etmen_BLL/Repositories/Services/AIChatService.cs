using Etmen_BLL.DTOs.Chat;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class AIChatService : IAIChatService
    {
        private readonly IUnitOfWork _uow;

        public AIChatService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<ChatMessageDto>> SendMessageAsync(int userId, string message)
        {
            // TODO: Persist user message as ChatMessage entity,
            //       call AI/LLM API with conversation history as context,
            //       persist AI response, return AI ChatMessageDto.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ChatThreadDto>> GetChatThreadAsync(int userId)
        {
            // TODO: _uow.ChatMessages.GetRecentMessagesAsync(userId.ToString(), count: 50),
            //       group into ChatThreadDto.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<List<ChatMessageDto>>> GetChatHistoryAsync(int userId, int pageNumber = 1, int pageSize = 20)
        {
            // TODO: Query ChatMessages for userId ordered by date desc, paginate, map to DTOs.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> ClearChatHistoryAsync(int userId)
        {
            // TODO: Load all ChatMessages for userId, RemoveRange, CompleteAsync.
            throw new NotImplementedException();
        }
    }
}
