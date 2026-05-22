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

        // GET: /Doctor/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // TODO: _doctorService.GetDashboardAsync(UserId), pass to Dashboard view.
            throw new NotImplementedException();
        }

        // GET: /Doctor/Profile
        public async Task<IActionResult> Profile()
        {
            // TODO: _doctorService.GetProfileAsync(UserId), return Profile view.
            throw new NotImplementedException();
        }

        // POST: /Doctor/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(DoctorProfileDto dto)
        {
            // TODO: _doctorService.UpdateProfileAsync(UserId, dto), redirect to Profile.
            throw new NotImplementedException();
        }

        // GET: /Doctor/Statistics
        public async Task<IActionResult> Statistics()
        {
            // TODO: _doctorService.GetStatisticsAsync(UserId), pass to Statistics view.
            throw new NotImplementedException();
        }

        // GET: /Doctor/AvailableSlots
        public async Task<IActionResult> AvailableSlots()
        {
            // TODO: Resolve doctorId, _doctorService.GetAvailableSlotsAsync(doctorId), pass to view.
            throw new NotImplementedException();
        }

        // POST: /Doctor/AddSlot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSlot(CreateAvailableSlotDto dto)
        {
            // TODO: _doctorService.AddSlotAsync(UserId, dto), redirect to AvailableSlots.
            throw new NotImplementedException();
        }

        // POST: /Doctor/DeleteSlot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSlot(int slotId)
        {
            // TODO: _doctorService.DeleteSlotAsync(UserId, slotId), redirect to AvailableSlots.
            throw new NotImplementedException();
        }

        // GET: /Doctor/Appointments
        public async Task<IActionResult> Appointments()
        {
            // TODO: _doctorService.GetAppointmentsAsync(UserId), pass to Appointments view.
            throw new NotImplementedException();
        }

        // GET: /Doctor/AppointmentDetail/{id}
        [HttpGet]
        public async Task<IActionResult> AppointmentDetail(int id)
        {
            // TODO: _doctorService.GetAppointmentAsync(UserId, id), pass to AppointmentDetail view.
            throw new NotImplementedException();
        }

        // POST: /Doctor/UpdateAppointmentStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, UpdateAppointmentStatusDto dto)
        {
            // TODO: _doctorService.UpdateAppointmentStatusAsync(UserId, appointmentId, dto), redirect to Appointments.
            throw new NotImplementedException();
        }

        // GET: /Doctor/Patients
        public async Task<IActionResult> Patients(string searchTerm = "")
        {
            // TODO: _doctorService.SearchPatientsAsync(searchTerm), pass list to Patients view.
            throw new NotImplementedException();
        }

        // POST: /Doctor/AddMedicalRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMedicalRecord(/*MedicalRecordCreateDto dto*/)
        {
            // TODO: _doctorService.AddMedicalRecordForPatientAsync(UserId, dto), redirect to Patients.
            throw new NotImplementedException();
        }
    }
}
