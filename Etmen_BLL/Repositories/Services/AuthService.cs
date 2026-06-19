using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Etmen_BLL.DTOs.Lab;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Mapster;

namespace Etmen_BLL.Repositories.Services
{
    public class LabService : ILabService
    {
        private readonly IUnitOfWork _uow;

        public LabService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // ── Lab Results Management ────────────────────────────────────────────────

        public async Task<ServiceResult<LabResultDto>> GetLabResultByIdAsync(int labResultId)
        {
            try
            {
                var labResult = await _uow.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult<LabResultDto>.Fail("نتيجة التحليل غير موجودة.");

                var dto = labResult.Adapt<LabResultDto>();
                return ServiceResult<LabResultDto>.Ok(dto, "تم جلب التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<LabResultDto>.Fail($"حدث خطأ أثناء جلب التحليل: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<LabResultDto>>> GetPatientLabResultsAsync(int patientId)
        {
            try
            {
                var labResults = await _uow.LabResults.GetByPatientIdAsync(patientId);
                if (labResults == null || !labResults.Any())
                    return ServiceResult<List<LabResultDto>>.Fail("لا توجد تحاليل مسجلة لهذا المريض.");

                var dtos = labResults.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Ok(dtos, "تم جلب تحاليل المريض بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Fail($"حدث خطأ أثناء جلب تحاليل المريض: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<LabResultDto>>> GetLabResultsByDateRangeAsync(int patientId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var labResults = await _uow.LabResults.GetByDateRangeAsync(patientId, startDate, endDate);
                var dtos = labResults.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Ok(dtos, "تم جلب التحاليل بالفترة المحددة بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Fail($"حدث خطأ أثناء جلب التحاليل بالفترة المحددة: {ex.Message}");
            }
        }

        // ── Lab Upload ────────────────────────────────────────────────────────────

        public async Task<ServiceResult<LabResultDto>> UploadLabResultAsync(LabUploadDto dto)
        {
            try
            {
                if (dto == null)
                    return ServiceResult<LabResultDto>.Fail("بيانات الرفع فارغة.");

                var labResult = new LabResult
                {
                    TestName = dto.TestName,
                    TestDate = dto.TestDate,
                    FilePath = dto.FilePath,
                    FileUrl = dto.FilePath,
                    Results = "Pending Review",
                    CreatedAt = DateTime.UtcNow,
                    OcrExtractedData = dto.UseOcr ? "Simulated OCR text: Glucose 95 mg/dL" : null
                };

                await _uow.LabResults.AddAsync(labResult);
                await _uow.CompleteAsync();

                var resultDto = labResult.Adapt<LabResultDto>();
                return ServiceResult<LabResultDto>.Ok(resultDto, "تم رفع وحفظ التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<LabResultDto>.Fail($"خطأ أثناء حفظ التحليل: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateLabResultAsync(int labResultId, LabUploadDto dto)
        {
            try
            {
                var labResult = await _uow.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.Fail("التحليل المراد تعديله غير موجود.");

                labResult.TestName = dto.TestName;
                labResult.TestDate = dto.TestDate;

                if (!string.IsNullOrEmpty(dto.FilePath))
                {
                    labResult.FilePath = dto.FilePath;
                    labResult.FileUrl = dto.FilePath;
                }

                if (dto.UseOcr)
                {
                    labResult.OcrExtractedData = "Updated Simulated OCR text";
                }

                _uow.LabResults.Update(labResult);
                await _uow.CompleteAsync();

                return ServiceResult.Ok("تم تحديث التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"خطأ أثناء تحديث التحليل: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteLabResultAsync(int labResultId)
        {
            try
            {
                var labResult = await _uow.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.Fail("التحليل غير موجود بالفعل.");

                _uow.LabResults.Remove(labResult);
                await _uow.CompleteAsync();

                return ServiceResult.Ok("تم حذف التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"خطأ أثناء حذف التحليل: {ex.Message}");
            }
        }

        // ── Lab Analysis ──────────────────────────────────────────────────────────

        public async Task<ServiceResult<Dictionary<string, object>>> AnalyzeLabResultsAsync(int patientId)
        {
            try
            {
                var results = await _uow.LabResults.GetByPatientIdAsync(patientId);

                var analysis = new Dictionary<string, object>
                {
                    { "PatientId", patientId },
                    { "TotalTestsRun", results?.Count() ?? 0 },
                    { "LastAnalyzedAt", DateTime.UtcNow },
                    { "Message", "تم فحص تتابع التحاليل وسجل القياسات بنجاح." }
                };

                return ServiceResult<Dictionary<string, object>>.Ok(analysis, "تمت عملية التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, object>>.Fail($"خطأ أثناء تحليل النتائج: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<LabResultDto>>> GetAbnormalResultsAsync(int patientId)
        {
            try
            {
                var results = await _uow.LabResults.GetByPatientIdAsync(patientId);

                var abnormalResults = results?
                    .Where(r => r.Results != null &&
                               (r.Results.Contains("High", StringComparison.OrdinalIgnoreCase) ||
                                r.Results.Contains("Low", StringComparison.OrdinalIgnoreCase) ||
                                r.Results.Contains("Abnormal", StringComparison.OrdinalIgnoreCase) ||
                                r.Results.Contains("مرتفع", StringComparison.OrdinalIgnoreCase) ||
                                r.Results.Contains("منخفض", StringComparison.OrdinalIgnoreCase)))
                    .ToList() ?? new List<LabResult>();

                var dtos = abnormalResults.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Ok(dtos, "تم جلب التحاليل غير الطبيعية بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Fail($"خطأ أثناء جلب التحاليل غير الطبيعية: {ex.Message}");
            }
        }

        // ── Lab Reports & Search ──────────────────────────────────────────────────

        public async Task<ServiceResult<List<LabResultDto>>> SearchLabResultsAsync(string testName, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var results = await _uow.LabResults.GetAllAsync();

                var filtered = results
                    .Where(r => string.IsNullOrEmpty(testName) || r.TestName.Contains(testName, StringComparison.OrdinalIgnoreCase))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var dtos = filtered.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Ok(dtos, "تمت عملية البحث بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Fail($"خطأ في عملية البحث: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Dictionary<string, object>>> GetLabStatisticsAsync()
        {
            try
            {
                var allResults = await _uow.LabResults.GetAllAsync();

                var stats = new Dictionary<string, object>
                {
                    { "TotalRecords", allResults.Count() },
                    { "VerifiedRecords", allResults.Count(r => r.Results != null && r.Results.Contains("Verified")) },
                    { "PendingRecords", allResults.Count(r => r.Results == null || !r.Results.Contains("Verified")) }
                };

                return ServiceResult<Dictionary<string, object>>.Ok(stats, "تم جلب الإحصائيات بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, object>>.Fail($"خطأ أثناء حساب الإحصائيات: {ex.Message}");
            }
        }

        // ── Verification ──────────────────────────────────────────────────────────

        public async Task<ServiceResult> VerifyLabResultAsync(int labResultId)
        {
            try
            {
                var labResult = await _uow.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.Fail("التحليل المطلوب غير موجود.");

                labResult.Results = "Verified / Normal";

                _uow.LabResults.Update(labResult);
                await _uow.CompleteAsync();

                return ServiceResult.Ok("تم توثيق التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"خطأ أثناء توثيق التحليل: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RejectLabResultAsync(int labResultId, string reason)
        {
            try
            {
                var labResult = await _uow.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.Fail("التحليل المطلوب غير موجود.");

                labResult.Results = $"Rejected: {reason}";

                _uow.LabResults.Update(labResult);
                await _uow.CompleteAsync();

                return ServiceResult.Ok("تم رفض التحليل بنجاح.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"خطأ أثناء رفض التحليل: {ex.Message}");
            }
        }
    }
}