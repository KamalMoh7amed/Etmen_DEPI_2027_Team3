using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
<<<<<<< HEAD
using Etmen_DAL.Models;
using Microsoft.AspNetCore.Identity;
=======
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
>>>>>>> 70a21bed046c888d602bdbbeacc6b6849e3638ac

namespace Etmen_BLL.Repositories.Services
{
    // ─────────────────────────────────────────────────────────────────────────
    // Service Result
    // ─────────────────────────────────────────────────────────────────────────

    public class ServiceResult
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; } = string.Empty;

        public static ServiceResult Ok(string message = "Success")
            => new() { Success = true, Message = message };

        public static ServiceResult Fail(string message)
            => new() { Success = false, Message = message };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        public static ServiceResult<T> Ok(T? data, string message = "Success")
            => new()
            {
                Success = true,
                Message = message,
                Data = data
            };

        public new static ServiceResult<T> Fail(string message)
            => new()
            {
                Success = false,
                Message = message
            };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Auth Result
    // ─────────────────────────────────────────────────────────────────────────

    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }

        public UserInfo User { get; set; } = default!;

        public IList<string> Roles { get; set; }
            = new List<string>();
    }

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Roles
    // ─────────────────────────────────────────────────────────────────────────

    public static class AppRoles
    {
        public const string User = "User";
        public const string Admin = "Admin";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // JWT Service
    // ─────────────────────────────────────────────────────────────────────────

    public interface IJwtService
    {
        (string token, DateTime expiresAt)
            GenerateToken(ApplicationUser user, IList<string> roles);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Email Service
    // ─────────────────────────────────────────────────────────────────────────

    public interface IEmailService
    {
        Task SendEmailVerificationAsync(
            string toEmail,
            string userId,
            string token);

        Task SendPasswordResetAsync(
            string toEmail,
            string userId,
            string token);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Auth Service
    // ─────────────────────────────────────────────────────────────────────────

    public class AuthService : IAuthService
    {
<<<<<<< HEAD
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IJwtService _jwtService;

        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Register
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ServiceResult<AuthResult>>
            RegisterAsync(RegisterDto dto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(dto.Email)
                    is not null)
                {
                    return ServiceResult<AuthResult>.Fail(
                        "An account with this email already exists.");
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = false
                };

                var createResult =
                    await _userManager.CreateAsync(user, dto.Password);

                if (!createResult.Succeeded)
                {
                    return ServiceResult<AuthResult>.Fail(
                        GetErrors(createResult));
                }

                // Create default role if not exists
                if (!await _roleManager.RoleExistsAsync(AppRoles.User))
                {
                    await _roleManager.CreateAsync(
                        new IdentityRole(AppRoles.User));
                }

                // Add role
                await _userManager.AddToRoleAsync(user, AppRoles.User);

                // Send email verification
                var verifyToken =
                    await _userManager
                        .GenerateEmailConfirmationTokenAsync(user);

                await _emailService.SendEmailVerificationAsync(
                    user.Email!,
                    user.Id,
                    verifyToken);

                // Don't return JWT before verification
                return ServiceResult<AuthResult>.Ok(
                    null,
                    "Registration successful. Please verify your email.");
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResult>.Fail(
                    $"Registration failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Login
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ServiceResult<AuthResult>>
            LoginAsync(LoginDto dto)
        {
            try
            {
                var user =
                    await _userManager.FindByEmailAsync(dto.Email);

                if (user is null)
                {
                    return ServiceResult<AuthResult>.Fail(
                        "Invalid email or password.");
                }

                // Check lockout first
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return ServiceResult<AuthResult>.Fail(
                        "This account has been deactivated.");
                }

                if (!user.EmailConfirmed)
                {
                    return ServiceResult<AuthResult>.Fail(
                        "Please verify your email address before signing in.");
                }

                var signInResult =
                    await _signInManager.PasswordSignInAsync(
                        user,
                        dto.Password,
                        false,
                        true);

                if (!signInResult.Succeeded)
                {
                    return ServiceResult<AuthResult>.Fail(
                        "Invalid email or password.");
                }

                return ServiceResult<AuthResult>.Ok(
                    await BuildAuthResultAsync(user));
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResult>.Fail(
                    $"Login failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Verify Email
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ServiceResult>
            VerifyEmailAsync(string userId, string token)
        {
            try
            {
                var user =
                    await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    return ServiceResult.Fail("User not found.");
                }

                if (user.EmailConfirmed)
                {
                    return ServiceResult.Ok(
                        "Email is already verified.");
                }

                var result =
                    await _userManager.ConfirmEmailAsync(user, token);

                return result.Succeeded
                    ? ServiceResult.Ok(
                        "Email verified successfully.")
                    : ServiceResult.Fail(
                        GetErrors(result));
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(
                    $"Verification failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Forgot Password
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ServiceResult>
            ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user =
                    await _userManager.FindByEmailAsync(dto.Email);

                // Prevent user enumeration
                if (user is null || !user.EmailConfirmed)
                {
                    return ServiceResult.Ok(
                        "If that email is registered, a reset link has been sent.");
                }

                var resetToken =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(user);

                await _emailService.SendPasswordResetAsync(
                    user.Email!,
                    user.Id,
                    resetToken);

                return ServiceResult.Ok(
                    "If that email is registered, a reset link has been sent.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(
                    $"Password reset request failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Reset Password
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ServiceResult>
            ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                var user =
                    await _userManager.FindByEmailAsync(dto.Email);

                if (user is null)
                {
                    return ServiceResult.Fail("Invalid request.");
                }

                var result =
                    await _userManager.ResetPasswordAsync(
                        user,
                        dto.Token,
                        dto.NewPassword);

                return result.Succeeded
                    ? ServiceResult.Ok(
                        "Password reset successfully.")
                    : ServiceResult.Fail(
                        GetErrors(result));
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(
                    $"Password reset failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Deactivate Account
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ServiceResult>
            DeactivateAccountAsync(string userId)
        {
            try
            {
                var user =
                    await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    return ServiceResult.Fail("User not found.");
                }

                await _userManager.SetLockoutEnabledAsync(user, true);

                var result =
                    await _userManager.SetLockoutEndDateAsync(
                        user,
                        DateTimeOffset.MaxValue);

                // Invalidate old tokens
                await _userManager.UpdateSecurityStampAsync(user);

                return result.Succeeded
                    ? ServiceResult.Ok("Account deactivated.")
                    : ServiceResult.Fail(GetErrors(result));
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(
                    $"Account deactivation failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Check Email
        // ─────────────────────────────────────────────────────────────────────

        public async Task<bool>
            IsEmailTakenAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email)
                is not null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private async Task<AuthResult>
            BuildAuthResultAsync(ApplicationUser user)
        {
            var roles =
                await _userManager.GetRolesAsync(user);

            var (token, expiresAt) =
                _jwtService.GenerateToken(user, roles);

            return new AuthResult
            {
                Token = token,
                ExpiresAt = expiresAt,
                Roles = roles,

                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                }
            };
        }

        private static string GetErrors(IdentityResult result)
        {
            return string.Join(
                ", ",
                result.Errors.Select(e => e.Description));
=======
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IUnitOfWork uow, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _uow = uow;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServiceResult<AuthResult>> RegisterAsync(RegisterDto dto)
        {
            // Validate DTO
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<AuthResult>.Failure("البريد الإلكتروني وكلمة المرور مطلوبان");

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return ServiceResult<AuthResult>.Conflict("البريد الإلكتروني مسجل بالفعل");

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                IsEmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<AuthResult>.Failure(errors);
            }

            // Assign role
            var role = dto.Role ?? "Patient";
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return ServiceResult<AuthResult>.Failure("فشل في تعيين الدور");
            }

            // Create PatientProfile if role is Patient
            if (role == "Patient")
            {
                var profile = new PatientProfile
                {
                    ApplicationUserId = user.Id,
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.PatientProfiles.AddAsync(profile);
                await _uow.CompleteAsync();
            }

            // Generate email verification token
            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.VerificationToken = verificationToken;
            user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            await _userManager.UpdateAsync(user);

            var authResult = new AuthResult
            {
                Success = true,
                UserId = user.Id,
                Role = role,
                Message = "تم التسجيل بنجاح. يرجى التحقق من بريدك الإلكتروني."
            };

            return ServiceResult<AuthResult>.Created(authResult);
        }

        public async Task<ServiceResult<AuthResult>> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<AuthResult>.Failure("البريد الإلكتروني وكلمة المرور مطلوبان");

            // Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult<AuthResult>.Unauthorized("البريد الإلكتروني أو كلمة المرور غير صحيحة");

            // Check if email is confirmed
            if (!user.EmailConfirmed)
                return ServiceResult<AuthResult>.Unauthorized("يجب تأكيد بريدك الإلكتروني أولاً");

            // Check if account is active
            if (!user.IsActive)
                return ServiceResult<AuthResult>.Forbidden("حسابك معطل");

            // Verify password
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return ServiceResult<AuthResult>.Failure("حسابك مقفل مؤقتاً. حاول لاحقاً.", 429);
                if (signInResult.RequiresTwoFactor)
                    return ServiceResult<AuthResult>.Failure("يتطلب التحقق من خطوتين");
                return ServiceResult<AuthResult>.Unauthorized("البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Patient";

            var authResult = new AuthResult
            {
                Success = true,
                UserId = user.Id,
                Role = userRole,
                Message = "تم تسجيل الدخول بنجاح"
            };

            return ServiceResult<AuthResult>.Success(authResult);
        }

        public async Task<ServiceResult> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.NotFound("المستخدم غير موجود");

            if (user.EmailConfirmed)
                return ServiceResult.Success();

            try
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                    return ServiceResult.Failure("رمز التحقق غير صحيح أو انتهى الصلاحية");

                user.IsEmailVerified = true;
                user.VerificationToken = null;
                user.VerificationTokenExpiry = null;
                await _userManager.UpdateAsync(user);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"حدث خطأ: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.Failure("البريد الإلكتروني غير موجود");

            // Generate password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userManager.UpdateAsync(user);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult.Failure("البريد الإلكتروني وكلمة المرور الجديدة مطلوبان");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.NotFound("المستخدم غير موجود");

            // Check token expiry
            if (user.ResetPasswordTokenExpiry == null || DateTime.UtcNow > user.ResetPasswordTokenExpiry)
                return ServiceResult.Failure("رمز إعادة تعيين كلمة المرور انتهت صلاحيته");

            var token = dto.Token ?? user.ResetPasswordToken;
            if (string.IsNullOrWhiteSpace(token))
                return ServiceResult.Failure("رمز إعادة تعيين غير صحيح");

            try
            {
                var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ServiceResult.Failure(errors);
                }

                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpiry = null;
                await _userManager.UpdateAsync(user);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"حدث خطأ: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeactivateAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.NotFound("المستخدم غير موجود");

            user.IsActive = false;
            user.LockoutEnd = DateTime.MaxValue;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ServiceResult.Failure("فشل في تعطيل الحساب");

            return ServiceResult.Success();
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
>>>>>>> 70a21bed046c888d602bdbbeacc6b6849e3638ac
        }
    }
}