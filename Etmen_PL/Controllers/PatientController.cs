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

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET: /Patient/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var result = await _patientService.GetDashboardAsync(UserId);

            if (!result.IsSuccess)
                return RedirectToAction("Error");

            return View(result.Data);
        }

        // GET: /Patient/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var result = await _patientService.GetProfileAsync(UserId);

            if (!result.IsSuccess)
                return RedirectToAction("Error");

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
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to update profile");
                return View(dto);
            }

            TempData["SuccessMessage"] = "Profile updated successfully";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Patient/MedicalRecords
        [HttpGet]
        public async Task<IActionResult> MedicalRecords()
        {
            var result = await _patientService.GetMedicalRecordsAsync(UserId);

            if (!result.IsSuccess)
                return RedirectToAction("Error");

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
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to add medical record";
                return RedirectToAction(nameof(MedicalRecords));
            }

            TempData["SuccessMessage"] = "Medical record added successfully";
            return RedirectToAction(nameof(MedicalRecords));
        }

        // GET: /Patient/RiskAssessment
        [HttpGet]
        public async Task<IActionResult> RiskAssessment()
        {
            return View();
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
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to assess risk");
                return View(dto);
            }

            return RedirectToAction(nameof(RiskResult));
        }

        // GET: /Patient/RiskResult
        [HttpGet]
        public async Task<IActionResult> RiskResult()
        {
            var result = await _patientService.GetLatestRiskAssessmentAsync(UserId);

            if (!result.IsSuccess)
                return RedirectToAction(nameof(RiskAssessment));

            return View(result.Data);
        }

        // GET: /Patient/Appointments
        [HttpGet]
        public async Task<IActionResult> Appointments()
        {
            var result = await _appointmentService.GetPatientAppointmentsAsync(UserId);

            if (!result.IsSuccess)
                return RedirectToAction("Error");

            return View(result.Data);
        }

        // POST: /Patient/CancelAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _appointmentService.CancelAppointmentAsync(UserId, appointmentId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to cancel appointment";
                return RedirectToAction(nameof(Appointments));
            }

            TempData["SuccessMessage"] = "Appointment cancelled successfully";
            return RedirectToAction(nameof(Appointments));
        }

        // GET: /Patient/Alerts
        [HttpGet]
        public async Task<IActionResult> Alerts()
        {
            var result = await _alertService.GetUserAlertsAsync(UserId);

            if (!result.IsSuccess)
                return RedirectToAction("Error");

            return View(result.Data);
        }

        // GET: /Patient/LabResults
        [HttpGet]
        public async Task<IActionResult> LabResults()
        {
            // Resolve patientId from UserId
            var patientIdResult = await _patientService.GetPatientIdAsync(UserId);

            if (!patientIdResult.IsSuccess)
                return RedirectToAction("Error");

            var labResult = await _labService.GetPatientLabResultsAsync(patientIdResult.Data);

            if (!labResult.IsSuccess)
                return RedirectToAction("Error");

            return View(labResult.Data);
        }

        // GET: /Patient/Nearby
        [HttpGet]
        public async Task<IActionResult> Nearby()
        {
            return View();
        }
    }
}
