using System;
using System.Linq;
using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Etmen_BLL.Repositories.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            IUnitOfWork uow,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _uow = uow;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServiceResult<AuthResult>> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<AuthResult>.Failure("Email and password are required.");

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return ServiceResult<AuthResult>.Conflict("This email is already registered.");

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

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResult<AuthResult>.Failure(errors);
            }

            var role = string.IsNullOrWhiteSpace(dto.Role) ? "Patient" : dto.Role;
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return ServiceResult<AuthResult>.Failure("Failed to assign user role.");
            }

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

            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            user.VerificationToken = verificationToken;
            user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            await _userManager.UpdateAsync(user);

            var authResult = new AuthResult
            {
                Success = true,
                UserId = user.Id,
                Role = role,
                Message = "Registration successful. Please verify your email."
            };

            return ServiceResult<AuthResult>.Created(authResult);
        }

        public async Task<ServiceResult<AuthResult>> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<AuthResult>.Failure("Email and password are required.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult<AuthResult>.Unauthorized("Invalid email or password.");

            if (!user.EmailConfirmed)
                return ServiceResult<AuthResult>.Unauthorized("Please confirm your email before signing in.");

            if (!user.IsActive)
                return ServiceResult<AuthResult>.Forbidden("This account has been deactivated.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return ServiceResult<AuthResult>.Failure("Your account is temporarily locked. Please try again later.", 429);
                if (signInResult.RequiresTwoFactor)
                    return ServiceResult<AuthResult>.Failure("Two-factor authentication is required.");
                return ServiceResult<AuthResult>.Unauthorized("Invalid email or password.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Patient";

            var authResult = new AuthResult
            {
                Success = true,
                UserId = user.Id,
                Role = userRole,
                Message = "Login successful."
            };

            return ServiceResult<AuthResult>.Success(authResult);
        }

        public async Task<ServiceResult> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.NotFound("User not found.");

            if (user.EmailConfirmed)
                return ServiceResult.Success();

            try
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                    return ServiceResult.Failure("Verification token is invalid or expired.");

                user.IsEmailVerified = true;
                user.VerificationToken = null;
                user.VerificationTokenExpiry = null;
                await _userManager.UpdateAsync(user);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.Success();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userManager.UpdateAsync(user);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult.Failure("Email and new password are required.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResult.NotFound("User not found.");

            if (user.ResetPasswordTokenExpiry == null || DateTime.UtcNow > user.ResetPasswordTokenExpiry)
                return ServiceResult.Failure("Password reset token has expired.");

            var token = string.IsNullOrWhiteSpace(dto.Token) ? user.ResetPasswordToken : dto.Token;
            if (string.IsNullOrWhiteSpace(token))
                return ServiceResult.Failure("Invalid password reset token.");

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
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeactivateAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult.NotFound("User not found.");

            user.IsActive = false;
            user.LockoutEnd = DateTime.MaxValue;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ServiceResult.Failure("Failed to deactivate account.");

            return ServiceResult.Success();
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
