using Etmen_BLL.DTOs.Doctor;
using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.Repositories.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Etmen_PL.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;

        public DoctorController(IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Dashboard()
        {
            var result = await _doctorService.GetDashboardAsync(UserId);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        public async Task<IActionResult> Profile()
        {
            var result = await _doctorService.GetProfileAsync(UserId);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(DoctorProfileDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            await _doctorService.UpdateProfileAsync(UserId, dto);
            return RedirectToAction(nameof(Profile));
        }

        public async Task<IActionResult> Statistics()
        {
            var result = await _doctorService.GetStatisticsAsync(UserId);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        public async Task<IActionResult> AvailableSlots()
        {
            var doctor = await _doctorService.GetProfileAsync(UserId);
            if (!doctor.IsSuccess)
                return NotFound();
            var result = await _doctorService.GetAvailableSlotsAsync(doctor.Data!.Id);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSlot(CreateAvailableSlotDto dto)
        {
            await _doctorService.AddSlotAsync(UserId, dto);
            return RedirectToAction(nameof(AvailableSlots));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlot(int slotId)
        {
            await _doctorService.DeleteSlotAsync(UserId, slotId);
            return RedirectToAction(nameof(AvailableSlots));
        }

        public async Task<IActionResult> Appointments()
        {
            var result = await _doctorService.GetAppointmentsAsync(UserId);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentDetail(int id)
        {
            var result = await _doctorService.GetAppointmentAsync(UserId, id);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, UpdateAppointmentStatusDto dto)
        {
            await _doctorService.UpdateAppointmentStatusAsync(UserId, appointmentId, dto);
            return RedirectToAction(nameof(Appointments));
        }

        public async Task<IActionResult> Patients(string searchTerm = "")
        {
            var result = await _doctorService.SearchPatientsAsync(searchTerm);
            if (!result.IsSuccess)
                return NotFound();
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicalRecord(MedicalRecordCreateDto dto)
        {
            await _doctorService.AddMedicalRecordForPatientAsync(UserId, dto);
            return RedirectToAction(nameof(Patients));
        }
    }
}
