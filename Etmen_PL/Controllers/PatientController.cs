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
        public async Task<IActionResult> Dashboard()
        {
            // TODO: Call _patientService.GetDashboardAsync(UserId), pass to view.
            throw new NotImplementedException();
        }

        // GET: /Patient/Profile
        public async Task<IActionResult> Profile()
        {
            // TODO: Call _patientService.GetProfileAsync(UserId), return Profile view.
            throw new NotImplementedException();
        }

        // POST: /Patient/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileDto dto)
        {
            // TODO: ModelState check, call _patientService.UpdateProfileAsync(UserId, dto),
            //       redirect to Profile on success.
            throw new NotImplementedException();
        }

        // GET: /Patient/MedicalRecords
        public async Task<IActionResult> MedicalRecords()
        {
            // TODO: _patientService.GetMedicalRecordsAsync(UserId), pass list to view.
            throw new NotImplementedException();
        }

        // POST: /Patient/AddMedicalRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicalRecord(MedicalRecordCreateDto dto)
        {
            // TODO: _patientService.AddMedicalRecordAsync(UserId, dto), redirect to MedicalRecords.
            throw new NotImplementedException();
        }

        // GET: /Patient/RiskAssessment
        public async Task<IActionResult> RiskAssessment()
        {
            // TODO: Return RiskAssessment form view (view already exists).
            throw new NotImplementedException();
        }

        // POST: /Patient/RiskAssessment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RiskAssessment(RiskInputDto dto)
        {
            // TODO: _patientService.AssessRiskAsync(UserId, dto), redirect to RiskResult view with result.
            throw new NotImplementedException();
        }

        // GET: /Patient/RiskResult
        public async Task<IActionResult> RiskResult()
        {
            // TODO: _patientService.GetLatestRiskAssessmentAsync(UserId), pass to RiskResult view.
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Appointments()
        {
            var result = await _appointmentService.GetPatientAppointmentsAsync(UserId);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            await _appointmentService.CancelAppointmentAsync(UserId, appointmentId);
            return RedirectToAction(nameof(Appointments));
        }

        // GET: /Patient/Alerts
        public async Task<IActionResult> Alerts()
        {
            // TODO: _alertService.GetUserAlertsAsync(UserId), pass to Alerts view.
            throw new NotImplementedException();
        }

        // GET: /Patient/LabResults
        public async Task<IActionResult> LabResults()
        {
            // TODO: Resolve patientId from UserId, _labService.GetPatientLabResultsAsync(patientId), pass to view.
            throw new NotImplementedException();
        }

        // GET: /Patient/Nearby
        public IActionResult Nearby()
        {
            // TODO: Return Nearby view (search form). Results loaded via AJAX or POST.
            throw new NotImplementedException();
        }
    }
}
