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
        private readonly IAlertService _alertService;

        public PatientService(IUnitOfWork uow, IAlertService alertService)
        {
            _uow = uow;
            _alertService = alertService;
        }

        // ── Profile ───────────────────────────────────────────────────────────────

        public async Task<ServiceResult<ProfileDto>> GetProfileAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<ProfileDto>.NotFound($"Patient profile not found for user {userId}");

                var dto = patient.Adapt<ProfileDto>();
                return ServiceResult<ProfileDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileDto>.Failure($"Error retrieving profile: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<ProfileDto>> UpdateProfileAsync(string userId, ProfileDto dto)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<ProfileDto>.NotFound($"Patient profile not found for user {userId}");

                patient.FullName = dto.FullName;
                patient.DateOfBirth = dto.DateOfBirth;
                patient.Gender = dto.Gender;
                patient.Height = dto.Height;
                patient.Weight = dto.Weight;
                patient.ActivityLevel = dto.ActivityLevel;
                patient.BloodType = dto.BloodType;
                patient.HasChronicDiseases = dto.HasChronicDiseases;
                patient.ChronicDiseasesNotes = dto.ChronicDiseasesNotes;
                patient.Allergies = dto.Allergies;
                patient.CurrentMedications = dto.CurrentMedications;
                patient.UpdatedAt = DateTime.UtcNow;

                _uow.PatientProfiles.Update(patient);
                await _uow.CompleteAsync();

                var updatedDto = patient.Adapt<ProfileDto>();
                return ServiceResult<ProfileDto>.Success(updatedDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileDto>.Failure($"Error updating profile: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<int>> GetPatientIdAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<int>.NotFound($"Patient profile not found for user {userId}");

                return ServiceResult<int>.Success(patient.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Failure($"Error retrieving patient ID: {ex.Message}", 500);
            }
        }

        // ── Dashboard ─────────────────────────────────────────────────────────────

        public async Task<ServiceResult<DashboardDto>> GetDashboardAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<DashboardDto>.NotFound($"Patient profile not found for user {userId}");

                var upcomingAppointments = await _uow.Appointments.FindAsync(a => 
                    a.PatientProfileId == patient.Id && 
                    a.AppointmentDate >= DateTime.UtcNow.Date &&
                    a.Status == Etmen_Domain.Enums.AppointmentStatus.Scheduled);

                var latestRiskAssessment = await _uow.RiskAssessments.FindAsync(r => r.PatientProfileId == patient.Id);

                var unreadAlerts = await _alertService.GetUnreadCountAsync(userId);

                var dashboard = new DashboardDto
                {
                    PatientName = patient.FullName ?? "Patient",
                    LatestRiskAssessment = latestRiskAssessment.OrderByDescending(r => r.AssessmentDate).FirstOrDefault()?.Adapt<RiskResultDto>(),
                    UnreadAlertsCount = unreadAlerts.Data,
                    UpcomingAppointmentsCount = upcomingAppointments.Count(),
                    LatestBmi = patient.Weight.HasValue && patient.Height.HasValue && patient.Height > 0 
                        ? patient.Weight.Value / ((patient.Height.Value / 100) * (patient.Height.Value / 100))
                        : null,
                    UpcomingAppointments = upcomingAppointments.Take(5).Adapt<List<RecentAppointmentDto>>(),
                    RecentAlerts = (await _alertService.GetUserAlertsAsync(userId)).Data?.Take(5).Adapt<List<RecentAlertDto>>() ?? new()
                };

                return ServiceResult<DashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return ServiceResult<DashboardDto>.Failure($"Error retrieving dashboard: {ex.Message}", 500);
            }
        }

        // ── Medical Records ───────────────────────────────────────────────────────

        public async Task<ServiceResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<IEnumerable<MedicalRecordDto>>.NotFound($"Patient profile not found for user {userId}");

                var records = await _uow.MedicalRecords.FindAsync(m => m.PatientProfileId == patient.Id);
                var dtos = records.Adapt<List<MedicalRecordDto>>();
                return ServiceResult<IEnumerable<MedicalRecordDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<MedicalRecordDto>>.Failure($"Error retrieving medical records: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<MedicalRecordDto>> GetLatestMedicalRecordAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<MedicalRecordDto>.NotFound($"Patient profile not found for user {userId}");

                var latestRecord = (await _uow.MedicalRecords.FindAsync(m => m.PatientProfileId == patient.Id))
                    .OrderByDescending(m => m.RecordDate)
                    .FirstOrDefault();

                if (latestRecord == null)
                    return ServiceResult<MedicalRecordDto>.NotFound("No medical records found");

                var dto = latestRecord.Adapt<MedicalRecordDto>();
                return ServiceResult<MedicalRecordDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<MedicalRecordDto>.Failure($"Error retrieving latest medical record: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<MedicalRecordDto>> AddMedicalRecordAsync(string userId, MedicalRecordCreateDto dto)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<MedicalRecordDto>.NotFound($"Patient profile not found for user {userId}");

                var record = dto.Adapt<Etmen_Domain.Entities.MedicalRecord>();
                record.PatientProfileId = patient.Id;
                record.RecordDate = DateTime.UtcNow;
                record.CreatedAt = DateTime.UtcNow;

                await _uow.MedicalRecords.AddAsync(record);
                await _uow.CompleteAsync();

                var resultDto = record.Adapt<MedicalRecordDto>();
                return ServiceResult<MedicalRecordDto>.Created(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<MedicalRecordDto>.Failure($"Error adding medical record: {ex.Message}", 500);
            }
        }

        // ── Risk Assessment ───────────────────────────────────────────────────────

        public async Task<ServiceResult<RiskResultDto>> AssessRiskAsync(string userId, RiskInputDto input)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<RiskResultDto>.NotFound($"Patient profile not found for user {userId}");

                // Create medical record from input
                var medicalRecord = new Etmen_Domain.Entities.MedicalRecord
                {
                    PatientProfileId = patient.Id,
                    RecordDate = DateTime.UtcNow,
                    HeartRate = input.HeartRate,
                    SystolicBP = input.SystolicBP,
                    DiastolicBP = input.DiastolicBP,
                    Temperature = input.Temperature,
                    OxygenSaturation = input.OxygenSaturation,
                    BloodSugar = input.BloodSugar,
                    Symptoms = input.Symptoms,
                    CreatedAt = DateTime.UtcNow
                };

                await _uow.MedicalRecords.AddAsync(medicalRecord);
                await _uow.CompleteAsync();

                // Calculate risk score based on inputs
                var riskScore = CalculateRiskScore(input);
                var riskLevel = DetermineRiskLevel(riskScore);

                // Create risk assessment
                var riskAssessment = new Etmen_Domain.Entities.RiskAssessment
                {
                    PatientProfileId = patient.Id,
                    AssessmentDate = DateTime.UtcNow,
                    RiskScore = riskScore,
                    RiskLevel = riskLevel,
                    Symptoms = input.Symptoms,
                    IsEmergency = riskScore >= 0.7m,
                    CreatedAt = DateTime.UtcNow
                };

                await _uow.RiskAssessments.AddAsync(riskAssessment);
                await _uow.CompleteAsync();

                var result = new RiskResultDto
                {
                    RiskScore = riskScore,
                    RiskLevel = riskLevel,
                    RiskColor = GetRiskColor(riskLevel),
                    RiskLabel = GetRiskLabel(riskLevel),
                    IsEmergency = riskScore >= 0.7m,
                    Recommendations = GenerateRecommendations(riskLevel),
                    TriggeredSymptoms = string.IsNullOrEmpty(input.Symptoms) 
                        ? new List<string>() 
                        : input.Symptoms.Split(',').Select(s => s.Trim()).ToList()
                };

                return ServiceResult<RiskResultDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<RiskResultDto>.Failure($"Error assessing risk: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<RiskResultDto>> GetLatestRiskAssessmentAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<RiskResultDto>.NotFound($"Patient profile not found for user {userId}");

                var latestAssessment = (await _uow.RiskAssessments.FindAsync(r => r.PatientProfileId == patient.Id))
                    .OrderByDescending(r => r.AssessmentDate)
                    .FirstOrDefault();

                if (latestAssessment == null)
                    return ServiceResult<RiskResultDto>.NotFound("No risk assessments found");

                var dto = new RiskResultDto
                {
                    RiskScore = latestAssessment.RiskScore,
                    RiskLevel = latestAssessment.RiskLevel,
                    RiskColor = GetRiskColor(latestAssessment.RiskLevel),
                    RiskLabel = GetRiskLabel(latestAssessment.RiskLevel),
                    IsEmergency = latestAssessment.IsEmergency,
                    Recommendations = GenerateRecommendations(latestAssessment.RiskLevel),
                    TriggeredSymptoms = string.IsNullOrEmpty(latestAssessment.Symptoms) 
                        ? new List<string>() 
                        : latestAssessment.Symptoms.Split(',').Select(s => s.Trim()).ToList()
                };

                return ServiceResult<RiskResultDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<RiskResultDto>.Failure($"Error retrieving risk assessment: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<IEnumerable<RiskResultDto>>> GetRiskHistoryAsync(string userId)
        {
            try
            {
                var patient = await _uow.PatientProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == userId);
                if (patient == null)
                    return ServiceResult<IEnumerable<RiskResultDto>>.NotFound($"Patient profile not found for user {userId}");

                var assessments = (await _uow.RiskAssessments.FindAsync(r => r.PatientProfileId == patient.Id))
                    .OrderByDescending(r => r.AssessmentDate)
                    .ToList();

                var dtos = assessments.Select(a => new RiskResultDto
                {
                    RiskScore = a.RiskScore,
                    RiskLevel = a.RiskLevel,
                    RiskColor = GetRiskColor(a.RiskLevel),
                    RiskLabel = GetRiskLabel(a.RiskLevel),
                    IsEmergency = a.IsEmergency,
                    Recommendations = GenerateRecommendations(a.RiskLevel),
                    TriggeredSymptoms = string.IsNullOrEmpty(a.Symptoms) 
                        ? new List<string>() 
                        : a.Symptoms.Split(',').Select(s => s.Trim()).ToList()
                }).ToList();

                return ServiceResult<IEnumerable<RiskResultDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RiskResultDto>>.Failure($"Error retrieving risk history: {ex.Message}", 500);
            }
        }

        // ── Helper Methods ────────────────────────────────────────────────────────

        private decimal CalculateRiskScore(RiskInputDto input)
        {
            decimal score = 0;

            // Blood pressure risk
            if (input.SystolicBP.HasValue)
            {
                if (input.SystolicBP >= 180 || input.DiastolicBP >= 120)
                    score += 0.3m; // Hypertensive crisis
                else if (input.SystolicBP >= 140 || input.DiastolicBP >= 90)
                    score += 0.2m; // Hypertension
            }

            // Heart rate risk
            if (input.HeartRate.HasValue)
            {
                if (input.HeartRate < 40 || input.HeartRate > 120)
                    score += 0.2m; // Abnormal heart rate
            }

            // Temperature risk
            if (input.Temperature.HasValue)
            {
                if (input.Temperature >= 39 || input.Temperature < 35)
                    score += 0.15m; // Severe fever/hypothermia
                else if (input.Temperature >= 38 || input.Temperature < 36)
                    score += 0.1m; // Mild fever
            }

            // Oxygen saturation risk
            if (input.OxygenSaturation.HasValue)
            {
                if (input.OxygenSaturation < 90)
                    score += 0.25m; // Low oxygen
                else if (input.OxygenSaturation < 95)
                    score += 0.1m; // Slightly low oxygen
            }

            // Blood sugar risk
            if (input.BloodSugar.HasValue)
            {
                if (input.BloodSugar < 70 || input.BloodSugar > 400)
                    score += 0.2m; // Dangerous glucose levels
            }

            return Math.Min(score, 1); // Cap at 1.0
        }

        private Etmen_Domain.Enums.RiskLevel DetermineRiskLevel(decimal score)
        {
            if (score >= 0.7m)
                return Etmen_Domain.Enums.RiskLevel.Critical;
            else if (score >= 0.5m)
                return Etmen_Domain.Enums.RiskLevel.High;
            else if (score >= 0.3m)
                return Etmen_Domain.Enums.RiskLevel.Medium;
            else
                return Etmen_Domain.Enums.RiskLevel.Low;
        }

        private string GetRiskColor(Etmen_Domain.Enums.RiskLevel level)
        {
            return level switch
            {
                Etmen_Domain.Enums.RiskLevel.Critical => "#dc3545",
                Etmen_Domain.Enums.RiskLevel.High => "#fd7e14",
                Etmen_Domain.Enums.RiskLevel.Medium => "#ffc107",
                Etmen_Domain.Enums.RiskLevel.Low => "#28a745",
                _ => "#6c757d"
            };
        }

        private string GetRiskLabel(Etmen_Domain.Enums.RiskLevel level)
        {
            return level switch
            {
                Etmen_Domain.Enums.RiskLevel.Critical => "Critical",
                Etmen_Domain.Enums.RiskLevel.High => "High",
                Etmen_Domain.Enums.RiskLevel.Medium => "Medium",
                Etmen_Domain.Enums.RiskLevel.Low => "Low",
                _ => "Unknown"
            };
        }

        private List<string> GenerateRecommendations(Etmen_Domain.Enums.RiskLevel level)
        {
            return level switch
            {
                Etmen_Domain.Enums.RiskLevel.Critical => new()
                {
                    "Seek immediate medical attention",
                    "Call emergency services if symptoms worsen",
                    "Do not delay - contact healthcare provider now"
                },
                Etmen_Domain.Enums.RiskLevel.High => new()
                {
                    "Schedule urgent appointment with healthcare provider",
                    "Monitor vital signs closely",
                    "Keep emergency contact nearby"
                },
                Etmen_Domain.Enums.RiskLevel.Medium => new()
                {
                    "Book appointment with your doctor",
                    "Monitor symptoms and vital signs",
                    "Follow recommended preventive measures"
                },
                Etmen_Domain.Enums.RiskLevel.Low => new()
                {
                    "Continue routine health monitoring",
                    "Maintain healthy lifestyle",
                    "Schedule regular check-ups"
                },
                _ => new()
            };
        }
    }
}
