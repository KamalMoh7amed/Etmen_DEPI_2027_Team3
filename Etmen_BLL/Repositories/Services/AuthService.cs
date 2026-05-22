using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;

namespace Etmen_BLL.Repositories.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;

        public AuthService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<ServiceResult<AuthResult>> RegisterAsync(RegisterDto dto)
        {
            // TODO: Validate dto, create ApplicationUser via UserManager,
            //       assign role, generate email verification token, send email.
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AuthResult>> LoginAsync(LoginDto dto)
        {
            // TODO: Find user by email, check password via SignInManager,
            //       verify email confirmed, generate JWT token.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> VerifyEmailAsync(string userId, string token)
        {
            // TODO: Find user by userId, call UserManager.ConfirmEmailAsync(user, token).
            throw new NotImplementedException();
        }

        public Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            // TODO: Find user by email, generate reset token via UserManager,
            //       send reset email with token link.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            // TODO: Find user by email/userId, call UserManager.ResetPasswordAsync.
            throw new NotImplementedException();
        }

        public Task<ServiceResult> DeactivateAccountAsync(string userId)
        {
            // TODO: Find user, set IsActive = false (or LockoutEnd), save changes.
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailTakenAsync(string email)
        {
            // TODO: Query UserManager or _uow.Users for existing email.
            throw new NotImplementedException();
        }
    }
}
