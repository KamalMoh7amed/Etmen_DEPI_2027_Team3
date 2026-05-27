using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Patient;
using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Mapster;

namespace Etmen_BLL.Repositories.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;

        public PatientService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ── Profile ───────────────────────────────────────────────────────────────

        public async Task<ServiceResult<ProfileDto>> GetProfileAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<ProfileDto>.NotFound("الملف الشخصي غير موجود");

            var dto = profile.Adapt<ProfileDto>();
            return ServiceResult<ProfileDto>.Success(dto);
        }

        public async Task<ServiceResult<ProfileDto>> UpdateProfileAsync(string userId, ProfileDto dto)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<ProfileDto>.NotFound("الملف الشخصي غير موجود");

            // Update profile fields
            profile.FullName = dto.FullName;
            profile.DateOfBirth = dto.DateOfBirth;
            profile.Gender = dto.Gender;
            profile.Height = dto.Height;
            profile.Weight = dto.Weight;
            profile.ActivityLevel = dto.ActivityLevel;
            profile.BloodType = dto.BloodType;
            profile.HasChronicDiseases = dto.HasChronicDiseases;
            profile.ChronicDiseasesNotes = dto.ChronicDiseasesNotes;
            profile.Allergies = dto.Allergies;
            profile.CurrentMedications = dto.CurrentMedications;
            profile.UpdatedAt = DateTime.UtcNow;

            _uow.PatientProfiles.Update(profile);
            await _uow.CompleteAsync();

            var result = profile.Adapt<ProfileDto>();
            return ServiceResult<ProfileDto>.Success(result);
        }

        // ── Dashboard ─────────────────────────────────────────────────────────────

        public async Task<ServiceResult<DashboardDto>> GetDashboardAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<DashboardDto>.NotFound("الملف الشخصي غير موجود");

            try
            {
                // Get upcoming appointments (next 7 days)
                var upcomingAppts = await _uow.Appointments.GetUpcomingAppointmentsAsync(profile.Id);
                var upcomingInNext7Days = upcomingAppts
                    .Where(a => a.AppointmentDate <= DateTime.Today.AddDays(7))
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5)
                    .ToList();

                var apptDtos = upcomingInNext7Days.Select(a => new RecentAppointmentDto
                {
                    Id = a.Id,
                    DoctorName = a.DoctorProfile?.FullName ?? "طبيب",
                    Date = a.AppointmentDate,
                    Status = a.Status.ToString()
                }).ToList();

                // Get latest risk assessment
                var latestRisk = await _uow.RiskAssessments.GetLatestByPatientIdAsync(profile.Id);
                var riskDto = latestRisk?.Adapt<RiskResultDto>();

                // Get unread alerts
                var unreadAlerts = await _uow.Alerts.GetUnreadAlertsAsync(userId);
                var alertsList = unreadAlerts.ToList();
                var alertDtos = alertsList.Take(5).Select(a => new RecentAlertDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CreatedAt = a.CreatedAt,
                    IsRead = a.Status.ToString() == "Read"
                }).ToList();

                // Get latest BMI
                var latestBmi = await _uow.PatientProfiles.GetLatestBmiAsync(userId);
                var bmiCategory = GetBmiCategory(latestBmi ?? 0);

                // Build dashboard
                var dashboard = new DashboardDto
                {
                    PatientName = profile.FullName ?? "المريض",
                    LatestRiskAssessment = riskDto,
                    UnreadAlertsCount = alertsList.Count,
                    UpcomingAppointmentsCount = upcomingInNext7Days.Count,
                    LatestBmi = latestBmi,
                    LatestBmiCategory = bmiCategory,
                    UpcomingAppointments = apptDtos,
                    RecentAlerts = alertDtos
                };

                return ServiceResult<DashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return ServiceResult<DashboardDto>.Failure($"حدث خطأ: {ex.Message}");
            }
        }

        // ── Medical Records ───────────────────────────────────────────────────────

        public async Task<ServiceResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<IEnumerable<MedicalRecordDto>>.NotFound();

            var records = await _uow.MedicalRecords.GetByPatientIdAsync(profile.Id);
            var dtos = records.Adapt<List<MedicalRecordDto>>();

            return ServiceResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
        }

        public async Task<ServiceResult<MedicalRecordDto>> GetLatestMedicalRecordAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<MedicalRecordDto>.NotFound();

            var record = await _uow.MedicalRecords.GetLatestByPatientIdAsync(profile.Id);
            if (record == null)
                return ServiceResult<MedicalRecordDto>.NotFound();

            var dto = record.Adapt<MedicalRecordDto>();
            return ServiceResult<MedicalRecordDto>.Success(dto);
        }

        public async Task<ServiceResult<MedicalRecordDto>> AddMedicalRecordAsync(string userId, MedicalRecordCreateDto dto)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<MedicalRecordDto>.NotFound();

            var record = new Etmen_Domain.Entities.MedicalRecord
            {
                PatientProfileId = profile.Id,
                RecordDate = dto.RecordDate == DateTime.MinValue ? DateTime.UtcNow : dto.RecordDate,
                SystolicBP = dto.SystolicBP,
                DiastolicBP = dto.DiastolicBP,
                BloodSugar = dto.BloodSugar,
                HeartRate = dto.HeartRate,
                Temperature = dto.Temperature,
                OxygenSaturation = dto.OxygenSaturation,
                Symptoms = dto.Symptoms,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.MedicalRecords.AddAsync(record);
            await _uow.CompleteAsync();

            var resultDto = record.Adapt<MedicalRecordDto>();
            return ServiceResult<MedicalRecordDto>.Created(resultDto);
        }

        // ── Risk Assessment ───────────────────────────────────────────────────────

        public async Task<ServiceResult<RiskResultDto>> AssessRiskAsync(string userId, RiskInputDto input)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<RiskResultDto>.NotFound();

            // Calculate risk score (basic implementation)
            var riskScore = CalculateRiskScore(input, profile);
            var riskLevel = GetRiskLevel(riskScore);

            var riskAssessment = new Etmen_Domain.Entities.RiskAssessment
            {
                PatientProfileId = profile.Id,
                RiskScore = riskScore,
                RiskLevel = riskLevel,
                Symptoms = input.Symptoms,
                IsEmergency = riskScore >= 0.7m,
                AssessmentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.RiskAssessments.AddAsync(riskAssessment);
            await _uow.CompleteAsync();

            var result = riskAssessment.Adapt<RiskResultDto>();
            return ServiceResult<RiskResultDto>.Created(result);
        }

        public async Task<ServiceResult<RiskResultDto>> GetLatestRiskAssessmentAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<RiskResultDto>.NotFound();

            var assessment = await _uow.RiskAssessments.GetLatestByPatientIdAsync(profile.Id);
            if (assessment == null)
                return ServiceResult<RiskResultDto>.NotFound();

            var dto = assessment.Adapt<RiskResultDto>();
            return ServiceResult<RiskResultDto>.Success(dto);
        }

        public async Task<ServiceResult<IEnumerable<RiskResultDto>>> GetRiskHistoryAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return ServiceResult<IEnumerable<RiskResultDto>>.NotFound();

            var assessments = await _uow.RiskAssessments.GetByPatientIdAsync(profile.Id);
            var dtos = assessments.Adapt<List<RiskResultDto>>();

            return ServiceResult<IEnumerable<RiskResultDto>>.Success(dtos);
        }

        private decimal CalculateRiskScore(RiskInputDto input, Etmen_Domain.Entities.PatientProfile profile)
        {
            decimal score = 0m;

            // BMI risk
            if (profile.BMI > 0)
            {
                if (profile.BMI < 18.5m || profile.BMI > 30m)
                    score += 0.15m;
                if (profile.BMI > 35m)
                    score += 0.15m;
            }

            // Systolic BP risk
            if (input.SystolicBP.HasValue)
            {
                if (input.SystolicBP > 140)
                    score += 0.25m;
                else if (input.SystolicBP > 130)
                    score += 0.15m;
            }

            // Blood Sugar risk
            if (input.BloodSugar.HasValue)
            {
                if (input.BloodSugar > 200)
                    score += 0.25m;
                else if (input.BloodSugar > 140)
                    score += 0.15m;
            }

            // Heart Rate risk
            if (input.HeartRate.HasValue)
            {
                if (input.HeartRate > 100 || input.HeartRate < 60)
                    score += 0.1m;
            }

            // Temperature risk
            if (input.Temperature.HasValue)
            {
                if (input.Temperature > 38 || input.Temperature < 36)
                    score += 0.1m;
            }

            // Oxygen Saturation risk
            if (input.OxygenSaturation.HasValue)
            {
                if (input.OxygenSaturation < 95)
                    score += 0.2m;
            }

            // Symptoms risk (if any symptoms present)
            if (!string.IsNullOrWhiteSpace(input.Symptoms))
                score += 0.1m;

            // Clamp between 0 and 1
            return Math.Min(score, 1m);
        }

        private Etmen_Domain.Enums.RiskLevel GetRiskLevel(decimal score)
        {
            if (score < 0.25m)
                return Etmen_Domain.Enums.RiskLevel.Low;
            else if (score < 0.5m)
                return Etmen_Domain.Enums.RiskLevel.Medium;
            else if (score < 0.75m)
                return Etmen_Domain.Enums.RiskLevel.High;
            else
                return Etmen_Domain.Enums.RiskLevel.Critical;
        }

        private string GetBmiCategory(decimal bmi)
        {
            if (bmi <= 0) return "غير متوفر";
            if (bmi < 18.5m) return "نقص الوزن";
            if (bmi < 25m) return "وزن صحي";
            if (bmi < 30m) return "زيادة الوزن";
            return "السمنة";
        }
    }
}
