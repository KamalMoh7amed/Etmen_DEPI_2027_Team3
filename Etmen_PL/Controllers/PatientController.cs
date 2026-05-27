using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Patient;
using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.Repositories.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: /Patient/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var result = await _patientService.GetDashboardAsync(UserId);
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
            var result = await _patientService.GetProfileAsync(UserId);
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

            var result = await _patientService.UpdateProfileAsync(UserId, dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "فشل تحديث الملف الشخصي.");
                return View(dto);
            }

            TempData["SuccessMessage"] = "تم تحديث الملف الشخصي بنجاح.";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Patient/MedicalRecords
        public async Task<IActionResult> MedicalRecords()
        {
            var result = await _patientService.GetMedicalRecordsAsync(UserId);
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

            var result = await _patientService.AddMedicalRecordAsync(UserId, dto);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "فشل إضافة السجل الطبي.";
                return RedirectToAction(nameof(MedicalRecords));
            }

            TempData["SuccessMessage"] = "تم إضافة السجل الطبي بنجاح.";
            return RedirectToAction(nameof(MedicalRecords));
        }

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

            var result = await _patientService.AssessRiskAsync(UserId, dto);
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
            var result = await _patientService.GetLatestRiskAssessmentAsync(UserId);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "لا توجد تقييمات مخاطر سابقة.";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(result.Data);
        }

        // GET: /Patient/Appointments
        public async Task<IActionResult> Appointments()
        {
            // This will be implemented when IAppointmentService is fully available
            return View(new List<dynamic>());
        }
    }
}
