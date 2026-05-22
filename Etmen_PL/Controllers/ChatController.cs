using Etmen_BLL.Repositories.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Etmen_PL.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IAIChatService _chatService;

        public ChatController(IAIChatService chatService)
        {
            _chatService = chatService;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Chat/Index
        public async Task<IActionResult> Index()
        {
            // TODO: _chatService.GetChatThreadAsync(UserId), pass to Chat/Index view.
            throw new NotImplementedException();
        }

        // POST: /Chat/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string message)
        {
            // TODO: _chatService.SendMessageAsync(UserId, message),
            //       return JSON result for AJAX or redirect to Index.
            throw new NotImplementedException();
        }

        // POST: /Chat/ClearHistory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearHistory()
        {
            // TODO: _chatService.ClearChatHistoryAsync(UserId), redirect to Index.
            throw new NotImplementedException();
        }
    }
}
