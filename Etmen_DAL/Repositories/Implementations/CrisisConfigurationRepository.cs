using Etmen_Domain.Entities;
using Etmen_Domain.Enums;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class CrisisConfigurationRepository : GenericRepository<CrisisConfiguration>, ICrisisConfigurationRepository
    {
        public CrisisConfigurationRepository(EtmenDbContext context) : base(context) { }

        public async Task<CrisisConfiguration?> GetActiveCrisisAsync()
        {
            return await FirstOrDefaultAsync(c => c.IsActive);
        }

        public async Task<CrisisConfiguration?> GetWithOutbreakZonesAsync(int crisisId)
        {
            return await _dbSet.Include(c => c.OutbreakZones)
                .FirstOrDefaultAsync(c => c.Id == crisisId);
        }

        public async Task<CrisisConfiguration?> GetWithSymptomWeightsAsync(int crisisId)
        {
            return await _dbSet.Include(c => c.SymptomWeights)
                .FirstOrDefaultAsync(c => c.Id == crisisId);
        }

        public async Task<IEnumerable<CrisisConfiguration>> GetAllCrisesAsync()
        {
            return await _dbSet.AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CrisisConfiguration>> GetByTypeAsync(CrisisType crisisType)
        {
            return await FindAsync(c => c.CrisisType == crisisType);
        }

        public async Task<IEnumerable<CrisisConfiguration>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await FindAsync(c => c.StartDate >= startDate && (c.EndDate == null || c.EndDate <= endDate));
        }

        public async Task ActivateCrisisAsync(int crisisId)
        {
            // Deactivate all other crises first
            var activeCrisis = await FirstOrDefaultAsync(c => c.IsActive);
            if (activeCrisis != null)
            {
                activeCrisis.IsActive = false;
                Update(activeCrisis);
            }

            // Activate the target crisis
            var crisis = await GetByIdAsync(crisisId);
            if (crisis != null)
            {
                crisis.IsActive = true;
                Update(crisis);
            }
        }

        public async Task DeactivateCrisisAsync(int crisisId)
        {
            var crisis = await GetByIdAsync(crisisId);
            if (crisis != null)
            {
                crisis.IsActive = false;
                Update(crisis);
            }
        }

        public async Task UpdateSystemModeAsync(int crisisId, SystemMode mode)
        {
            var crisis = await GetByIdAsync(crisisId);
            if (crisis != null)
            {
                crisis.SystemMode = mode;
                Update(crisis);
            }
        }

    }
}