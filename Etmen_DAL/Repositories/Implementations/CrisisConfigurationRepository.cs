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
            // TODO: FirstOrDefaultAsync(c => c.IsActive).
            throw new NotImplementedException();
        }

        public async Task<CrisisConfiguration?> GetWithOutbreakZonesAsync(int crisisId)
        {
            // TODO: _dbSet.Include(c=>c.OutbreakZones).FirstOrDefaultAsync(c=>c.Id==crisisId).
            throw new NotImplementedException();
        }

        public async Task<CrisisConfiguration?> GetWithSymptomWeightsAsync(int crisisId)
        {
            // TODO: _dbSet.Include(c=>c.SymptomWeights).FirstOrDefaultAsync(c=>c.Id==crisisId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CrisisConfiguration>> GetAllCrisesAsync()
        {
            // TODO: _dbSet.AsNoTracking().OrderByDescending(c=>c.CreatedAt).ToListAsync().
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CrisisConfiguration>> GetByTypeAsync(CrisisType crisisType)
        {
            // TODO: FindAsync(c => c.CrisisType == crisisType).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CrisisConfiguration>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // TODO: FindAsync(c => c.StartDate >= startDate && (c.EndDate == null || c.EndDate <= endDate)).
            throw new NotImplementedException();
        }

        public async Task ActivateCrisisAsync(int crisisId)
        {
            // TODO: GetByIdAsync, set IsActive=true, deactivate others if needed, Update.
            throw new NotImplementedException();
        }

        public async Task DeactivateCrisisAsync(int crisisId)
        {
            // TODO: GetByIdAsync, set IsActive=false, Update.
            throw new NotImplementedException();
        }

        public async Task UpdateSystemModeAsync(int crisisId, SystemMode mode)
        {
            // TODO: GetByIdAsync, set SystemMode=mode, Update.
            throw new NotImplementedException();
        }

    }
}