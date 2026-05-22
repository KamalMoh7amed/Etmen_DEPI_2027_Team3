using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Helpers;

namespace Etmen_BLL.Repositories.IServices
{
    /// <summary>
    /// Contract for all Identity-related operations (register, login, email verification, password reset).
    /// </summary>
    public interface IAuthService
    {
        /// <summary>Registers a new user and sends a verification e-mail.</summary>
        Task<ServiceResult<AuthResult>> RegisterAsync(RegisterDto dto);

        /// <summary>Authenticates a user and returns an auth result (token / claims handled at API layer).</summary>
        Task<ServiceResult<AuthResult>> LoginAsync(LoginDto dto);

        /// <summary>Confirms the user's e-mail using the verification token.</summary>
        Task<ServiceResult> VerifyEmailAsync(string userId, string token);

        /// <summary>Initiates the forgot-password flow by generating a reset token and sending e-mail.</summary>
        Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto);

        /// <summary>Resets the password using the token received via e-mail.</summary>
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto);

        /// <summary>Marks the user account as inactive (soft-delete / admin action).</summary>
        Task<ServiceResult> DeactivateAccountAsync(string userId);

        /// <summary>Returns whether an e-mail is already registered.</summary>
        Task<bool> IsEmailTakenAsync(string email);
    }
}
