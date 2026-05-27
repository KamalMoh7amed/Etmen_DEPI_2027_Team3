using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace Etmen_BLL.Repositories.Services
{
    public class AuthService : IAuthService
    {
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
        }
    }
}
