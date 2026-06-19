using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Patient;
using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.DTOs.Lab; 
using Etmen_BLL.Repositories.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Etmen_PL.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IAlertService _alertService;
        private readonly ILabService _labService;
        private readonly INearbyService _nearbyService;

        public PatientController(
            IPatientService patientService,
            IAppointmentService appointmentService,
            IAlertService alertService,
            ILabService labService,
            INearbyService nearbyService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _alertService = alertService;
            _labService = labService;
            _nearbyService = nearbyService;
        }

        private string userId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: /Patient/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var result = await _patientService.GetDashboardAsync(userId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل في تحميل لوحة التحكم.";
                return View(new DashboardDto());
            }

            return View(result.Data);
        }

        // GET: /Patient/Profile
        public async Task<IActionResult> Profile()
        {
            var result = await _patientService.GetProfileAsync(userId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل في تحميل الملف الشخصي.";
                return View(new ProfileDto());
            }

            return View(result.Data);
        }

        // POST: /Patient/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _patientService.UpdateProfileAsync(userId, dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "فشل تحديث الملف الشخصي.");
                return View(dto);
            }

            TempData["SuccessMessage"] = "تم تحديث الملف الشخصي بنجاح.";
            return RedirectToAction(nameof(Profile));
        }

        // ────────────────────────────────────────────────────────
        // MEDICAL RECORDS (تنفيذ السجلات الطبية)
        // ────────────────────────────────────────────────────────

        // GET: /Patient/MedicalRecords
        public async Task<IActionResult> MedicalRecords()
        {
            var result = await _patientService.GetMedicalRecordsAsync(userId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل في تحميل السجلات الطبية.";
                return View(new List<MedicalRecordDto>());
            }

            return View(result.Data);
        }

        // POST: /Patient/AddMedicalRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicalRecord(MedicalRecordCreateDto dto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(MedicalRecords));

            var result = await _patientService.AddMedicalRecordAsync(userId, dto);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل إضافة السجل الطبي.";
                return RedirectToAction(nameof(MedicalRecords));
            }

            TempData["SuccessMessage"] = "تم إضافة السجل الطبي بنجاح.";
            return RedirectToAction(nameof(MedicalRecords));
        }

        // ────────────────────────────────────────────────────────
        // RISK ASSESSMENT (تنفيذ تقييم المخاطر)
        // ────────────────────────────────────────────────────────

        // GET: /Patient/RiskAssessment
        public IActionResult RiskAssessment()
        {
            return View(new RiskInputDto());
        }

        // POST: /Patient/RiskAssessment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RiskAssessment(RiskInputDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _patientService.AssessRiskAsync(userId, dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "فشل حساب المخاطر.");
                return View(dto);
            }

            TempData["RiskResult"] = System.Text.Json.JsonSerializer.Serialize(result.Data);
            return RedirectToAction(nameof(RiskResult));
        }

        // GET: /Patient/RiskResult
        public async Task<IActionResult> RiskResult()
        {
            var result = await _patientService.GetLatestRiskAssessmentAsync(userId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "لا توجد تقييمات مخاطر سابقة.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(result.Data);
        }

        public Task<IActionResult> LabResults(string userId)
        {
            return LabResults(userId);
        }

        // ────────────────────────────────────────────────────────
        // LAB RESULTS 
        // ────────────────────────────────────────────────────────

        // GET: /Patient/LabResults
        public async Task<IActionResult> LabResults(int userId)
        {
            var result = await _labService.GetPatientLabResultsAsync(userId);
            if (!result.IsSuccess)
            {
                ViewBag.InfoMessage = result.ErrorMessage ?? "لا توجد تحاليل مسجلة حالياً.";
                return View(new List<LabResultDto>());
            }

            return View(result.Data);
        }

        // POST: /Patient/UploadLabResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLabResult(LabUploadDto dto, IFormFile labFile)
        {
            if (labFile == null || labFile.Length == 0)
            {
                TempData["ErrorMessage"] = "برجاء اختيار ملف التحليل (صورة أو PDF) أولاً.";
                return RedirectToAction(nameof(LabResults));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "البيانات المدخلة غير صالحة.";
                return RedirectToAction(nameof(LabResults));
            }

            dto.LabFile = labFile;

            var result = await _labService.UploadLabResultAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل حفظ بيانات التحليل.";
                return RedirectToAction(nameof(LabResults));
            }

            TempData["SuccessMessage"] = "تم رفع وحفظ التحليل بنجاح داخل السيرفر وجاري مراجعته.";
            return RedirectToAction(nameof(LabResults));
        }

        // ────────────────────────────────────────────────────────
        // APPOINTMENTS
        // ────────────────────────────────────────────────────────

        // GET: /Patient/Appointments
        public async Task<IActionResult> Appointments()
        {
            // This will be implemented when IAppointmentService is fully available
            return View(new List<dynamic>());
        }
    }
}