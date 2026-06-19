using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Etmen_BLL.DTOs.Lab;
using Etmen_BLL.Helpers;
using Etmen_BLL.Repositories.IServices;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Hosting;

namespace Etmen_BLL.Repositories.Services
{
    public class LabService : ILabService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env; 

        public LabService(IUnitOfWork uow, IWebHostEnvironment env)
        {
            _unitOfWork = uow;
            _env = env;
        }

        // ─────────────────────────────
        // PRIVATE HELPERS (SECURITY)
        // ─────────────────────────────

        private async Task<PatientProfile?> GetPatientAsync(string userId)
        {
            return await _unitOfWork.PatientProfiles.GetByUserIdAsync(userId);
        }

        // ─────────────────────────────
        //  LAB RESULTS (READ)
        // ─────────────────────────────

        public async Task<ServiceResult<LabResultDto>> GetLabResultByIdAsync(int labResultId)
        {
            try
            {
                var labResult = await _unitOfWork.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult<LabResultDto>.NotFound("نتيجة التحليل غير موجودة.");

                var dto = labResult.Adapt<LabResultDto>();
                return ServiceResult<LabResultDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<LabResultDto>.Failure($"حدث خطأ أثناء جلب التحليل: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<LabResultDto>>> GetPatientLabResultsAsync(string userId)
        {
            try
            {
                var patient = await GetPatientAsync(userId);
                if (patient == null)
                    return ServiceResult<List<LabResultDto>>.NotFound("المريض غير موجود.");

                var labResults = await _unitOfWork.LabResults.GetByPatientIdAsync(patient.Id);
                if (labResults == null || !labResults.Any())
                    return ServiceResult<List<LabResultDto>>.NotFound("لا توجد تحاليل مسجلة لهذا المريض.");

                var dtos = labResults.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Failure($"حدث خطأ أثناء جلب تحاليل المريض: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<LabResultDto>>> GetLabResultsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var patient = await GetPatientAsync(userId);
                if (patient == null)
                    return ServiceResult<List<LabResultDto>>.NotFound("المريض غير موجود.");

                var labResults = await _unitOfWork.LabResults.GetByDateRangeAsync(patient.Id, startDate, endDate);
                var dtos = labResults.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Failure($"حدث خطأ أثناء جلب التحاليل بالفترة المحددة: {ex.Message}");
            }
        }

        // ─────────────────────────────
        // ⬆️ UPLOAD 
        // ─────────────────────────────

        public async Task<ServiceResult<LabResultDto>> UploadLabResultAsync(LabUploadDto dto)
        {
            try
            {
                if (dto == null)
                    return ServiceResult<LabResultDto>.Failure("بيانات الرفع فارغة.");

                if (dto.LabFile == null || dto.LabFile.Length == 0)
                    return ServiceResult<LabResultDto>.Failure("برجاء إرفاق ملف التحليل أولاً.");

                
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "labs");

                
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

               
                var extension = Path.GetExtension(dto.LabFile.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.LabFile.CopyToAsync(fileStream);
                }

                var relativePath = $"/uploads/labs/{uniqueFileName}";

                var labResult = new LabResult
                {
                    TestName = dto.TestName,
                    TestDate = dto.TestDate,
                    FilePath = relativePath,
                    FileUrl = relativePath, 
                    Results = "Pending Review",
                    CreatedAt = DateTime.UtcNow,
                    OcrExtractedData = dto.UseOcr ? "Simulated OCR text" : null
                };

                await _unitOfWork.LabResults.AddAsync(labResult);
                await _unitOfWork.CompleteAsync();

                var resultDto = labResult.Adapt<LabResultDto>();
                return ServiceResult<LabResultDto>.Created(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<LabResultDto>.Failure($"خطأ أثناء حفظ ومعالجة ملف التحليل: {ex.Message}");
            }
        }

        // ─────────────────────────────
        //  UPDATE
        // ─────────────────────────────

        public async Task<ServiceResult> UpdateLabResultAsync(int labResultId, LabUploadDto dto)
        {
            try
            {
                var labResult = await _unitOfWork.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.NotFound("التحليل المراد تعديله غير موجود.");

                labResult.TestName = dto.TestName;
                labResult.TestDate = dto.TestDate;

                if (dto.LabFile != null && dto.LabFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "labs");
                    var extension = Path.GetExtension(dto.LabFile.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.LabFile.CopyToAsync(fileStream);
                    }

                    var relativePath = $"/uploads/labs/{uniqueFileName}";
                    labResult.FilePath = relativePath;
                    labResult.FileUrl = relativePath;
                }

                if (dto.UseOcr)
                {
                    labResult.OcrExtractedData = "Updated Simulated OCR text";
                }

                _unitOfWork.LabResults.Update(labResult);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"خطأ أثناء تحديث التحليل: {ex.Message}");
            }
        }

        // ─────────────────────────────
        //  DELETE
        // ─────────────────────────────

        public async Task<ServiceResult> DeleteLabResultAsync(int labResultId)
        {
            try
            {
                var labResult = await _unitOfWork.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.NotFound("التحليل غير موجود بالفعل.");

                if (!string.IsNullOrEmpty(labResult.FilePath))
                {
                    var fullPath = Path.Combine(_env.WebRootPath, labResult.FilePath.TrimStart('/'));
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }

                _unitOfWork.LabResults.Remove(labResult);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"خطأ أثناء حذف التحليل: {ex.Message}");
            }
        }

        // ─────────────────────────────
        // ANALYSIS
        // ─────────────────────────────

        public async Task<ServiceResult<Dictionary<string, object>>> AnalyzeLabResultsAsync(string userId)
        {
            try
            {
                var patient = await GetPatientAsync(userId);
                if (patient == null)
                    return ServiceResult<Dictionary<string, object>>.NotFound("المريض غير موجود.");

                var results = await _unitOfWork.LabResults.GetByPatientIdAsync(patient.Id);

                var analysis = new Dictionary<string, object>
                {
                    { "PatientId", patient.Id },
                    { "TotalTestsRun", results?.Count() ?? 0 },
                    { "LastAnalyzedAt", DateTime.UtcNow },
                    { "Message", "تم فحص تتابع التحاليل وسجل القياسات بنجاح." }
                };

                return ServiceResult<Dictionary<string, object>>.Success(analysis);
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, object>>.Failure($"خطأ أثناء تحليل النتائج: {ex.Message}");
            }
        }

        // ─────────────────────────────
        // ABNORMAL
        // ─────────────────────────────

        public async Task<ServiceResult<List<LabResultDto>>> GetAbnormalResultsAsync(string userId)
        {
            try
            {
                var patient = await GetPatientAsync(userId);
                if (patient == null)
                    return ServiceResult<List<LabResultDto>>.NotFound("المريض غير موجود.");

                var results = await _unitOfWork.LabResults.GetByPatientIdAsync(patient.Id);

                var abnormalResults = results?
                    .Where(r => !string.IsNullOrEmpty(r.Results) &&
                               (r.Results.Contains("High", StringComparison.OrdinalIgnoreCase) ||
                                r.Results.Contains("Low", StringComparison.OrdinalIgnoreCase) ||
                                r.Results.Contains("Abnormal", StringComparison.OrdinalIgnoreCase)))
                    .ToList() ?? new List<LabResult>();

                var dtos = abnormalResults.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Failure($"خطأ أثناء جلب التحاليل غير الطبيعية: {ex.Message}");
            }
        }

        // ─────────────────────────────
        // SEARCH
        // ─────────────────────────────

        public async Task<ServiceResult<List<LabResultDto>>> SearchLabResultsAsync(string userId, string testName, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var patient = await GetPatientAsync(userId);
                if (patient == null)
                    return ServiceResult<List<LabResultDto>>.NotFound("المريض غير موجود.");

                var results = await _unitOfWork.LabResults.GetByPatientIdAsync(patient.Id);

                var filtered = results
                    .Where(r => string.IsNullOrEmpty(testName) || r.TestName.Contains(testName, StringComparison.OrdinalIgnoreCase))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var dtos = filtered.Adapt<List<LabResultDto>>();
                return ServiceResult<List<LabResultDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<LabResultDto>>.Failure($"خطأ في عملية البحث: {ex.Message}");
            }
        }

        // ─────────────────────────────
        // STATS
        // ─────────────────────────────

        public async Task<ServiceResult<Dictionary<string, object>>> GetLabStatisticsAsync(string userId)
        {
            try
            {
                var patient = await GetPatientAsync(userId);
                if (patient == null)
                    return ServiceResult<Dictionary<string, object>>.NotFound("المريض غير موجود.");

                var allResults = await _unitOfWork.LabResults.GetByPatientIdAsync(patient.Id);

                var stats = new Dictionary<string, object>
                {
                    { "TotalRecords", allResults.Count() },
                    { "VerifiedRecords", allResults.Count(r => r.Results != null && r.Results.Contains("Verified")) },
                    { "PendingRecords", allResults.Count(r => r.Results == null || !r.Results.Contains("Verified")) }
                };

                return ServiceResult<Dictionary<string, object>>.Success(stats);
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<string, object>>.Failure($"خطأ أثناء حساب الإحصائيات: {ex.Message}");
            }
        }

        // ─────────────────────────────
        //  VERIFY / REJECT
        // ─────────────────────────────

        public async Task<ServiceResult> VerifyLabResultAsync(int labResultId)
        {
            try
            {
                var labResult = await _unitOfWork.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.NotFound("التحليل المطلوب غير موجود.");

                labResult.Results = "Verified";

                _unitOfWork.LabResults.Update(labResult);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"خطأ أثناء توثيق التحليل: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RejectLabResultAsync(int labResultId, string reason)
        {
            try
            {
                var labResult = await _unitOfWork.LabResults.GetByIdAsync(labResultId);
                if (labResult == null)
                    return ServiceResult.NotFound("التحليل المطلوب غير موجود.");

                labResult.Results = $"Rejected: {reason}";

                _unitOfWork.LabResults.Update(labResult);
                await _unitOfWork.CompleteAsync();

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"خطأ أثناء رفض التحليل: {ex.Message}");
            }
        }
    }
}