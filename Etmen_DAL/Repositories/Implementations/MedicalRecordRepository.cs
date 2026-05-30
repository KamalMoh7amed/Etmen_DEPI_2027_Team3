using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Etmen_Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(EtmenDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {

            return await Table.Where(r => r.PatientProfileId == patientId).OrderByDescending(r => r.RecordDate).ToListAsync();

        }

        public async Task<MedicalRecord?> GetLatestByPatientIdAsync(int patientId)
        {
            return await Table.Where(r => r.PatientProfileId == patientId).OrderByDescending(r => r.RecordDate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDateRangeAsync(int patientId, DateTime startDate, DateTime endDate)
        {

            if (startDate > endDate)
                throw new ArgumentException(
                    "Start date cannot be greater than end date.");

            return await Table
                .Where(r =>
                    r.PatientProfileId == patientId &&
                    r.RecordDate >= startDate &&
                    r.RecordDate <= endDate)
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();

        }

        public async Task<IEnumerable<MedicalRecord>> GetWithAbnormalValuesAsync(int patientId)
        {

            return await Table
                .Where(r =>
                    r.PatientProfileId == patientId &&
                    (
                        (r.SystolicBP.HasValue &&
                         (r.SystolicBP < 90 || r.SystolicBP > 140))

                        || (r.DiastolicBP.HasValue &&
                            (r.DiastolicBP < 60 || r.DiastolicBP > 90))

                        || (r.BloodSugar.HasValue &&
                            (r.BloodSugar < 70 || r.BloodSugar > 180))

                        || (r.HeartRate.HasValue &&
                            (r.HeartRate < 60 || r.HeartRate > 100))

                        || (r.Temperature.HasValue &&
                            (r.Temperature < 36 || r.Temperature > 37.5m))

                        || (r.OxygenSaturation.HasValue &&
                            r.OxygenSaturation < 95)
                    ))
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();


        }
        

        public async Task AddRecordWithSymptomsAsync(MedicalRecord record, IEnumerable<string> symptoms)
        {

            if (record == null)
                throw new ArgumentNullException(nameof(record));

            if (symptoms == null)
                throw new ArgumentNullException(nameof(symptoms));

            record.Symptoms = string.Join(", ", symptoms);

            await AddAsync(record);
        }



    }
}