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
            return await FindAsync(s => s.DoctorProfileId == doctorId && s.SlotDate.Date == date.Date);
        }

        public async Task<IEnumerable<AvailableSlot>> GetAvailableSlotsAsync(int doctorId, DateTime fromDate, DateTime toDate)
        {
            return await FindAsync(s => s.DoctorProfileId == doctorId && !s.IsBooked && s.SlotDate >= fromDate && s.SlotDate <= toDate);
        }

        public async Task<AvailableSlot?> GetNextAvailableSlotAsync(int doctorId, DateTime fromDateTime)
        {
            return await _dbSet.Where(s => s.DoctorProfileId == doctorId && !s.IsBooked && s.SlotDate >= fromDateTime).OrderBy(s => s.SlotDate).ThenBy(s => s.SlotStart).FirstOrDefaultAsync();
        }

        public async Task MarkSlotAsBookedAsync(int slotId)
        {
            var slot = await GetByIdAsync(slotId);
            if (slot != null)
            {
                slot.IsBooked = true;
                Update(slot);
            }
        }

        public async Task MarkSlotAsAvailableAsync(int slotId)
        {
            var slot = await GetByIdAsync(slotId);
            if (slot != null)
            {
                slot.IsBooked = false;
                Update(slot);
            }
        }

        public async Task<IEnumerable<AvailableSlot>> GetSlotsByDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await FindAsync(s => s.DoctorProfileId == doctorId && s.SlotDate >= startDate && s.SlotDate <= endDate);
        }

    }
}
