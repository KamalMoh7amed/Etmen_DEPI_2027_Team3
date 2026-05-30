using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class LabResultRepository : GenericRepository<LabResult>, ILabResultRepository
    {
        public LabResultRepository(EtmenDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId)
        {
           
                return await Table.Where(l => l.PatientProfileId == patientId).ToListAsync();
        }

        public async Task<LabResult?> GetLatestByPatientIdAsync(int patientId)
        {
           
                return await Table.Where(l => l.PatientProfileId == patientId).OrderByDescending(l => l.TestDate).FirstOrDefaultAsync(); 
        }

        public async Task<IEnumerable<LabResult>> GetByTestNameAsync(int patientId,string testName)
        {
           
                if (string.IsNullOrWhiteSpace(testName))
                    throw new ArgumentException("Test name cannot be empty.",nameof(testName));

                return await Table.Where(l =>l.PatientProfileId == patientId && l.TestName == testName).OrderByDescending(l => l.TestDate).ToListAsync();

        }

        public async Task<IEnumerable<LabResult>> GetWithOcrDataAsync(int patientId)
        {
            
                return await Table.Where(l =>l.PatientProfileId == patientId && !string.IsNullOrEmpty(l.OcrExtractedData)).OrderByDescending(l => l.TestDate).ToListAsync();
            
            
        }

        public async Task<IEnumerable<LabResult>> GetByDateRangeAsync(int patientId,DateTime startDate, DateTime endDate)
        {
           
                if (startDate > endDate)
                    throw new ArgumentException("Start date cannot be greater than end date.");

            return await Table .Where(l => l.PatientProfileId == patientId && l.TestDate >= startDate && l.TestDate <= endDate).OrderByDescending(l => l.TestDate).ToListAsync();
        }

        public async Task UpdateOcrDataAsync(int labResultId, string ocrData)
        {
           
                if (string.IsNullOrWhiteSpace(ocrData))
                    throw new ArgumentException( "OCR data cannot be empty.", nameof(ocrData));

                var labResult = await GetByIdAsync(labResultId);

                if (labResult == null)
                    throw new KeyNotFoundException($"Lab result with ID {labResultId} was not found.");

                labResult.OcrExtractedData = ocrData;

                Update(labResult);
           
        }
        
        

        public async Task<IEnumerable<LabResult>> SearchLabResultsAsync( int patientId,string searchTerm)
        {
            
                if (string.IsNullOrWhiteSpace(searchTerm))
                    throw new ArgumentException( "Search term cannot be empty.",nameof(searchTerm));

                searchTerm = searchTerm.Trim();

                return await Table.Where(l =>l.PatientProfileId == patientId &&  l.TestName.Contains(searchTerm)) .OrderByDescending(l => l.TestDate) .ToListAsync();
           
        }
    }
}