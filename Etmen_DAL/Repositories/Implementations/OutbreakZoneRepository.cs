using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Helpers;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class OutbreakZoneRepository : GenericRepository<OutbreakZone>, IOutbreakZoneRepository
    {
        public OutbreakZoneRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<OutbreakZone>> GetByCrisisIdAsync(int crisisId)
        {
            return await FindAsync(z => z.CrisisConfigurationId == crisisId);
        }

        public async Task<IEnumerable<OutbreakZone>> GetNearbyZonesAsync(decimal latitude, decimal longitude, decimal radiusInKm)
        {
            var zones = await GetAllAsync();
            return zones.Where(z => GeoHelper.CalculateDistance(latitude, longitude, z.CenterLatitude, z.CenterLongitude) <= radiusInKm);
        }

        public async Task<IEnumerable<OutbreakZone>> GetActiveZonesAsync(int crisisId)
        {
            return await FindAsync(z => z.CrisisConfigurationId == crisisId && z.RiskLevel > 0);
        }

        public async Task<bool> IsPointInZoneAsync(decimal latitude, decimal longitude, int zoneId)
        {
            var zone = await GetByIdAsync(zoneId);
            if (zone == null)
                return false;

            return GeoHelper.IsPointInZone(latitude, longitude, zone);
        }

        public async Task<IEnumerable<OutbreakZone>> GetZonesByRiskLevelAsync(int crisisId, int riskLevel)
        {
            return await FindAsync(z => z.CrisisConfigurationId == crisisId && z.RiskLevel == riskLevel);
        }

        public async Task UpdateZoneRiskLevelAsync(int zoneId, int newRiskLevel)
        {
            var zone = await GetByIdAsync(zoneId);
            if (zone != null)
            {
                zone.RiskLevel = newRiskLevel;
                Update(zone);
            }
        }

    }
}