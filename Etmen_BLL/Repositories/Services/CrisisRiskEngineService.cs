using Etmen_BLL.DTOs.Crisis;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Helpers;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Mapster;

namespace Etmen_BLL.Repositories.Services
{
    public class CrisisRiskEngineService : ICrisisRiskEngineService
    {
        private readonly IUnitOfWork _uow;

        public CrisisRiskEngineService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceResult<CrisisRiskResultDto>> CalculateCrisisRiskAsync(int patientProfileId, int crisisConfigurationId)
        {
            try
            {
                // Load patient profile with medical records
                var patient = await _uow.PatientProfiles.GetByIdAsync(patientProfileId);
                if (patient == null)
                    return ServiceResult<CrisisRiskResultDto>.NotFound($"Patient with ID {patientProfileId} not found");

                // Load crisis configuration with symptom weights
                var crisis = await _uow.CrisisConfigurations.GetWithSymptomWeightsAsync(crisisConfigurationId);
                if (crisis == null)
                    return ServiceResult<CrisisRiskResultDto>.NotFound($"Crisis with ID {crisisConfigurationId} not found");

                // Get patient's medical records to extract symptoms
                var medicalRecords = await _uow.MedicalRecords.FindAsync(m => m.PatientProfileId == patientProfileId);

                // Calculate weighted risk score based on symptoms
                decimal totalScore = 0;
                var matchedSymptoms = new List<string>();

                if (medicalRecords.Any() && crisis.SymptomWeights.Any())
                {
                    var latestRecord = medicalRecords.OrderByDescending(m => m.RecordDate).First();
                    var patientSymptoms = latestRecord.Symptoms?.Split(',') ?? Array.Empty<string>();

                    foreach (var symptom in patientSymptoms)
                    {
                        var symptomWeight = crisis.SymptomWeights.FirstOrDefault(sw => 
                            sw.SymptomName.Equals(symptom.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (symptomWeight != null)
                        {
                            totalScore += symptomWeight.Weight;
                            matchedSymptoms.Add(symptom.Trim());
                        }
                    }

                    totalScore = Math.Min(totalScore / Math.Max(1, crisis.SymptomWeights.Count), 1);
                }

                // Determine risk level based on thresholds
                RiskLevel riskLevel = DetermineRiskLevel(totalScore, crisis);
                bool isEmergency = totalScore >= crisis.EmergencyThreshold;

                // Create and persist risk assessment
                var riskAssessment = new RiskAssessment
                {
                    PatientProfileId = patientProfileId,
                    AssessmentDate = DateTime.UtcNow,
                    RiskScore = totalScore,
                    RiskLevel = riskLevel,
                    Symptoms = string.Join(", ", matchedSymptoms),
                    IsEmergency = isEmergency,
                    CreatedAt = DateTime.UtcNow
                };

                await _uow.RiskAssessments.AddAsync(riskAssessment);
                await _uow.CompleteAsync();

                // Build result DTO
                var result = new CrisisRiskResultDto
                {
                    RiskScore = totalScore,
                    RiskLevel = riskLevel,
                    IsInOutbreakZone = false,
                    ZoneName = null,
                    SystemMode = crisis.SystemMode,
                    Recommendations = GenerateRecommendations(riskLevel, isEmergency)
                };

                return ServiceResult<CrisisRiskResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<CrisisRiskResultDto>.Failure($"Error calculating crisis risk: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<decimal>> CalculateOutbreakProbabilityAsync(decimal latitude, decimal longitude, int crisisConfigurationId)
        {
            try
            {
                var crisis = await _uow.CrisisConfigurations.GetWithOutbreakZonesAsync(crisisConfigurationId);
                if (crisis == null)
                    return ServiceResult<decimal>.NotFound($"Crisis with ID {crisisConfigurationId} not found");

                // Find outbreak zones within a reasonable radius (e.g., 50km)
                const decimal searchRadius = 50m;
                var nearbyZones = crisis.OutbreakZones.Where(z => 
                    GeoHelper.CalculateDistance(latitude, longitude, z.CenterLatitude, z.CenterLongitude) <= searchRadius)
                    .ToList();

                if (!nearbyZones.Any())
                    return ServiceResult<decimal>.Success(0);

                // Calculate probability based on zones' risk levels and distance
                decimal totalProbability = 0;
                foreach (var zone in nearbyZones)
                {
                    var distance = GeoHelper.CalculateDistance(latitude, longitude, zone.CenterLatitude, zone.CenterLongitude);
                    var zoneRiskFactor = (decimal)zone.RiskLevel / 10m; // Normalize to 0-1
                    var distanceFactor = 1 - (distance / searchRadius); // Closer = higher probability
                    var zoneProbability = zoneRiskFactor * Math.Max(distanceFactor, 0);
                    totalProbability += zoneProbability;
                }

                // Average probability across zones
                var finalProbability = nearbyZones.Any() ? totalProbability / nearbyZones.Count : 0;
                finalProbability = Math.Min(finalProbability, 1); // Cap at 1.0

                return ServiceResult<decimal>.Success(finalProbability);
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.Failure($"Error calculating outbreak probability: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResult<List<OutbreakZoneDto>>> GetPatientsInZoneAsync(int crisisConfigurationId)
        {
            try
            {
                var zones = await _uow.OutbreakZones.GetActiveZonesAsync(crisisConfigurationId);
                var zoneList = zones.ToList();

                if (!zoneList.Any())
                    return ServiceResult<List<OutbreakZoneDto>>.Success(new List<OutbreakZoneDto>());

                var result = zoneList.Adapt<List<OutbreakZoneDto>>();
                return ServiceResult<List<OutbreakZoneDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<OutbreakZoneDto>>.Failure($"Error retrieving zones: {ex.Message}", 500);
            }
        }

        private RiskLevel DetermineRiskLevel(decimal score, CrisisConfiguration crisis)
        {
            if (score >= crisis.EmergencyThreshold)
                return RiskLevel.Critical;
            if (score >= crisis.HighRiskThreshold)
                return RiskLevel.High;
            if (score >= crisis.MediumRiskThreshold)
                return RiskLevel.Medium;
            return RiskLevel.Low;
        }

        private List<string> GenerateRecommendations(RiskLevel level, bool isEmergency)
        {
            var recommendations = new List<string>();

            if (isEmergency)
            {
                recommendations.Add("IMMEDIATE ACTION REQUIRED: Contact emergency services");
                recommendations.Add("Seek immediate medical attention");
            }
            else if (level == RiskLevel.Critical)
            {
                recommendations.Add("Contact healthcare provider urgently");
                recommendations.Add("Prepare for possible hospitalization");
            }
            else if (level == RiskLevel.High)
            {
                recommendations.Add("Schedule urgent medical consultation");
                recommendations.Add("Monitor symptoms closely");
                recommendations.Add("Follow preventive measures");
            }
            else if (level == RiskLevel.Medium)
            {
                recommendations.Add("Schedule regular medical check-up");
                recommendations.Add("Maintain healthy lifestyle");
                recommendations.Add("Follow recommended preventive measures");
            }
            else
            {
                recommendations.Add("Continue routine health monitoring");
                recommendations.Add("Maintain preventive measures");
            }

            return recommendations;
        }
    }
}
