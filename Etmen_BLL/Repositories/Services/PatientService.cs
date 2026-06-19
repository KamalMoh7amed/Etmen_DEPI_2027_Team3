using Etmen_BLL.DTOs.Medical;
using Etmen_BLL.DTOs.Patient;
using Etmen_BLL.DTOs.Risk;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Enums;
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

        public async Task<Etmen_BLL.Helpers.ServiceResult<ProfileDto>> GetProfileAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<ProfileDto>.NotFound("الملف الشخصي غير موجود");

            var dto = profile.Adapt<ProfileDto>();
            return Etmen_BLL.Helpers.ServiceResult<ProfileDto>.Success(dto);
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<ProfileDto>> UpdateProfileAsync(string userId, ProfileDto dto)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<ProfileDto>.NotFound("الملف الشخصي غير موجود");

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
            return Etmen_BLL.Helpers.ServiceResult<ProfileDto>.Success(result);
        }

        // ── Dashboard ─────────────────────────────────────────────────────────────

        public async Task<Etmen_BLL.Helpers.ServiceResult<DashboardDto>> GetDashboardAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<DashboardDto>.NotFound("الملف الشخصي غير موجود");

            try
            {
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

                var latestRisk = await _uow.RiskAssessments.GetLatestByPatientIdAsync(profile.Id);
                RiskResultDto? riskDto = null;

                if (latestRisk != null)
                {
                    riskDto = new RiskResultDto
                    {
                        RiskScore = latestRisk.RiskScore,
                        RiskLevel = latestRisk.RiskLevel,
                        RiskColor = RiskCalculatorHelper.GetRiskColor(latestRisk.RiskLevel),
                        RiskLabel = RiskCalculatorHelper.GetRiskLabel(latestRisk.RiskLevel),
                        IsEmergency = latestRisk.RiskLevel == RiskLevel.Emergency,
                        NearestEmergencyCenter = latestRisk.RiskLevel == RiskLevel.Emergency ? "مركز الطوارئ" : null,
                        Recommendations = RiskCalculatorHelper.GenerateRecommendations(latestRisk.RiskLevel, new List<string>()),
                        TriggeredSymptoms = new List<string>()
                    };
                }

                var unreadAlerts = await _uow.Alerts.GetUnreadAlertsAsync(userId);
                var alertsList = unreadAlerts.ToList();
                var alertDtos = alertsList.Take(5).Select(a => new RecentAlertDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    CreatedAt = a.CreatedAt,
                    IsRead = a.Status.ToString() == "Read"
                }).ToList();

                var latestBmi = await _uow.PatientProfiles.GetLatestBmiAsync(userId);
                var bmiCategory = GetBmiCategory(latestBmi ?? 0);

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

                return Etmen_BLL.Helpers.ServiceResult<DashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return Etmen_BLL.Helpers.ServiceResult<DashboardDto>.Failure($"حدث خطأ: {ex.Message}");
            }
        }

        // ── Medical Records ───────────────────────────────────────────────────────

        public async Task<Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.NotFound();

            var records = await _uow.MedicalRecords.GetByPatientIdAsync(profile.Id);
            var dtos = records.Adapt<List<MedicalRecordDto>>();

            return Etmen_BLL.Helpers.ServiceResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>> GetLatestMedicalRecordAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound();

            var record = await _uow.MedicalRecords.GetLatestByPatientIdAsync(profile.Id);
            if (record == null)
                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound();

            var dto = record.Adapt<MedicalRecordDto>();
            return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Success(dto);
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>> AddMedicalRecordAsync(string userId, MedicalRecordCreateDto dto)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.NotFound();

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
            return Etmen_BLL.Helpers.ServiceResult<MedicalRecordDto>.Created(resultDto);
        }

        // ── Risk Assessment ───────────────────────────────────────────────────────

        public async Task<Etmen_BLL.Helpers.ServiceResult<RiskResultDto>> AssessRiskAsync(string userId, RiskInputDto input)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.NotFound();

            // استخدام الحقل input.Symptoms بدلاً من المسمى القديم المسبب للخطأ
            var calculation = RiskCalculatorHelper.Calculate(
                input.SystolicBP,
                input.DiastolicBP,
                input.HeartRate,
                input.Temperature,
                input.OxygenSaturation,
                input.BloodSugar,
                input.Symptoms
            );

            var riskLevel = RiskCalculatorHelper.GetRiskLevel(calculation.Score);

            var riskAssessment = new Etmen_Domain.Entities.RiskAssessment
            {
                PatientProfileId = profile.Id,
                RiskScore = calculation.Score,
                RiskLevel = riskLevel,
                AssessmentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.RiskAssessments.AddAsync(riskAssessment);
            await _uow.CompleteAsync();

            var resultDto = new RiskResultDto
            {
                RiskScore = calculation.Score,
                RiskLevel = riskLevel,
                RiskColor = RiskCalculatorHelper.GetRiskColor(riskLevel),
                RiskLabel = RiskCalculatorHelper.GetRiskLabel(riskLevel),
                IsEmergency = calculation.IsEmergency,
                TriggeredSymptoms = calculation.TriggeredFactors,
                Recommendations = RiskCalculatorHelper.GenerateRecommendations(riskLevel, calculation.TriggeredFactors),
                NearestEmergencyCenter = calculation.IsEmergency ? "مركز الطوارئ المركزي" : null
            };

            return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.Created(resultDto);
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<RiskResultDto>> GetLatestRiskAssessmentAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.NotFound();

            var assessment = await _uow.RiskAssessments.GetLatestByPatientIdAsync(profile.Id);
            if (assessment == null)
                return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.NotFound();

            var dto = new RiskResultDto
            {
                RiskScore = assessment.RiskScore,
                RiskLevel = assessment.RiskLevel,
                RiskColor = RiskCalculatorHelper.GetRiskColor(assessment.RiskLevel),
                RiskLabel = RiskCalculatorHelper.GetRiskLabel(assessment.RiskLevel),
                IsEmergency = assessment.RiskLevel == RiskLevel.Emergency,
                NearestEmergencyCenter = assessment.RiskLevel == RiskLevel.Emergency ? "مركز الطوارئ" : null,
                Recommendations = RiskCalculatorHelper.GenerateRecommendations(assessment.RiskLevel, new List<string>()),
                TriggeredSymptoms = new List<string>()
            };

            return Etmen_BLL.Helpers.ServiceResult<RiskResultDto>.Success(dto);
        }

        public async Task<Etmen_BLL.Helpers.ServiceResult<IEnumerable<RiskResultDto>>> GetRiskHistoryAsync(string userId)
        {
            var profile = await _uow.PatientProfiles.GetByUserIdAsync(userId);
            if (profile == null)
                return Etmen_BLL.Helpers.ServiceResult<IEnumerable<RiskResultDto>>.NotFound();

            var assessments = await _uow.RiskAssessments.GetByPatientIdAsync(profile.Id);

            var dtos = assessments.Select(a => new RiskResultDto
            {
                RiskScore = a.RiskScore,
                RiskLevel = a.RiskLevel,
                RiskColor = RiskCalculatorHelper.GetRiskColor(a.RiskLevel),
                RiskLabel = RiskCalculatorHelper.GetRiskLabel(a.RiskLevel),
                IsEmergency = a.RiskLevel == RiskLevel.Emergency,
                NearestEmergencyCenter = a.RiskLevel == RiskLevel.Emergency ? "مركز الطوارئ" : null,
                Recommendations = RiskCalculatorHelper.GenerateRecommendations(a.RiskLevel, new List<string>()),
                TriggeredSymptoms = new List<string>()
            }).ToList();

            return Etmen_BLL.Helpers.ServiceResult<IEnumerable<RiskResultDto>>.Success(dtos);
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