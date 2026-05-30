using Etmen_BLL.DTOs.Auth;
using Etmen_BLL.Repositories.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Etmen_Domain.Entities;
using System.Security.Claims;

namespace Etmen_PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAuthService authService, SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess)
            {
                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage ?? "فشل التسجيل");
                }
                return View(dto);
            }

            TempData["SuccessMessage"] = "تم التسجيل بنجاح. يرجى التحقق من بريدك الإلكتروني.";
            return RedirectToAction(nameof(VerifyEmailNotice));
        }

        // GET: /Account/VerifyEmailNotice
        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyEmailNotice()
        {
            return View();
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.LoginAsync(dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "فشل تسجيل الدخول");
                return View(dto);
            }

            var user = new ApplicationUser { Id = result.Data.UserId, Email = dto.Email };
            await _signInManager.SignInAsync(user, dto.RememberMe);

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Redirect by role
            return result.Data.Role switch
            {
                "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("Dashboard", "Patient")
            };
        }

        // GET: /Account/Logout
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "تم تسجيل الخروج بنجاح.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/VerifyEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "رابط التحقق غير صحيح.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _authService.VerifyEmailAsync(userId, token);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل التحقق من البريد الإلكتروني.";
                return RedirectToAction(nameof(Login));
            }

            TempData["SuccessMessage"] = "تم التحقق من بريدك الإلكتروني بنجاح. يمكنك الآن تسجيل الدخول.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.ForgotPasswordAsync(dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "حدث خطأ.");
                return View(dto);
            }

            TempData["SuccessMessage"] = "تم إرسال رابط إعادة تعيين كلمة المرور إلى بريدك الإلكتروني.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordDto
            {
                Token = token
            };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "فشل إعادة تعيين كلمة المرور.");
                return View(dto);
            }

            TempData["SuccessMessage"] = "تم إعادة تعيين كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول.";
            return RedirectToAction(nameof(Login));
        }
    }
}
