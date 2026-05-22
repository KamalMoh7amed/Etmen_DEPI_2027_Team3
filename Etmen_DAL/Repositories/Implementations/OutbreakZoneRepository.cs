using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class OutbreakZoneRepository : GenericRepository<OutbreakZone>, IOutbreakZoneRepository
    {
        public OutbreakZoneRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<OutbreakZone>> GetByCrisisIdAsync(int crisisId)
        {
            // TODO: FindAsync(z => z.CrisisConfigurationId == crisisId).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OutbreakZone>> GetNearbyZonesAsync(decimal latitude, decimal longitude, decimal radiusInKm)
        {
            // TODO: Use GeoHelper/Haversine to find zones whose center is within radius.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OutbreakZone>> GetActiveZonesAsync(int crisisId)
        {
            // TODO: FindAsync(z => z.CrisisConfigurationId==crisisId && z.IsActive).
            throw new NotImplementedException();
        }

        public async Task<bool> IsPointInZoneAsync(decimal latitude, decimal longitude, int zoneId)
        {
            // TODO: GetByIdAsync, check if (lat,lon) falls within zone radius using GeoHelper.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OutbreakZone>> GetZonesByRiskLevelAsync(int crisisId, int riskLevel)
        {
            // TODO: FindAsync(z => z.CrisisConfigurationId==crisisId && z.RiskLevel==riskLevel).
            throw new NotImplementedException();
        }

        public async Task UpdateZoneRiskLevelAsync(int zoneId, int newRiskLevel)
        {
            // TODO: GetByIdAsync, set RiskLevel=newRiskLevel, Update.
            throw new NotImplementedException();
        }

    }
}