using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class AvailableSlotRepository : GenericRepository<AvailableSlot>, IAvailableSlotRepository
    {
        public AvailableSlotRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<AvailableSlot>> GetByDoctorIdAndDateAsync(int doctorId, DateTime date)
        {
            // TODO: FindAsync(s => s.DoctorProfileId == doctorId && s.SlotDate.Date == date.Date).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AvailableSlot>> GetAvailableSlotsAsync(int doctorId, DateTime fromDate, DateTime toDate)
        {
            // TODO: FindAsync(s => s.DoctorProfileId == doctorId && !s.IsBooked && s.SlotDate >= fromDate && s.SlotDate <= toDate).
            throw new NotImplementedException();
        }

        public async Task<AvailableSlot?> GetNextAvailableSlotAsync(int doctorId, DateTime fromDateTime)
        {
            // TODO: FirstOrDefaultAsync(s => s.DoctorProfileId == doctorId && !s.IsBooked && s.SlotDate >= fromDateTime) order by SlotDate asc.
            throw new NotImplementedException();
        }

        public async Task MarkSlotAsBookedAsync(int slotId)
        {
            // TODO: GetByIdAsync, set IsBooked = true, Update.
            throw new NotImplementedException();
        }

        public async Task MarkSlotAsAvailableAsync(int slotId)
        {
            // TODO: GetByIdAsync, set IsBooked = false, Update.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AvailableSlot>> GetSlotsByDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync(s => s.DoctorProfileId == doctorId && s.SlotDate >= startDate && s.SlotDate <= endDate).
            throw new NotImplementedException();
        }

    }
}