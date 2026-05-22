using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Repositories.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Etmen_PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // TODO: Return Register view (view already exists).
            throw new NotImplementedException();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // TODO: Check ModelState.IsValid, call _authService.RegisterAsync(dto),
            //       on success redirect to VerifyEmailNotice; on failure re-render view with errors.
            throw new NotImplementedException();
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // TODO: Return Login view.
            throw new NotImplementedException();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // TODO: ModelState check, call _authService.LoginAsync(dto),
            //       on success set auth cookie / redirect by role;
            //       on failure add ModelError and re-render.
            throw new NotImplementedException();
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            // TODO: Sign out user (SignInManager.SignOutAsync or cookie clear), redirect to Login.
            throw new NotImplementedException();
        }

        // GET: /Account/VerifyEmail
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            // TODO: Call _authService.VerifyEmailAsync(userId, token), redirect to Login with message.
            throw new NotImplementedException();
        }

        // GET: /Account/VerifyEmailNotice
        [HttpGet]
        public IActionResult VerifyEmailNotice()
        {
            // TODO: Return VerifyEmailNotice view.
            throw new NotImplementedException();
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            // TODO: Return ForgotPassword view.
            throw new NotImplementedException();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            // TODO: Call _authService.ForgotPasswordAsync(dto), return confirmation message.
            throw new NotImplementedException();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            // TODO: Pass userId and token to view via model/ViewBag.
            throw new NotImplementedException();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            // TODO: Call _authService.ResetPasswordAsync(dto), redirect to Login on success.
            throw new NotImplementedException();
        }
    }
}
