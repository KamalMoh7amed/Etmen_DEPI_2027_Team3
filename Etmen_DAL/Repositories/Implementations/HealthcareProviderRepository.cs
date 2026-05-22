using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class HealthcareProviderRepository : GenericRepository<HealthcareProvider>, IHealthcareProviderRepository
    {
        public HealthcareProviderRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<HealthcareProvider>> GetNearbyProvidersAsync(decimal latitude, decimal longitude, decimal radiusInKm)
        {
            // TODO: Use GeoHelper.CalculateDistance or raw Haversine to filter providers within radius.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HealthcareProvider>> GetEmergencyCentersAsync(decimal latitude, decimal longitude, decimal radiusInKm)
        {
            // TODO: GetNearbyProvidersAsync filtered by Type == EmergencyCenter.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HealthcareProvider>> GetByTypeAsync(string type)
        {
            // TODO: FindAsync(p => p.Type == type).
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HealthcareProvider>> GetWithAvailableBedsAsync()
        {
            // TODO: FindAsync(p => p.AvailableBeds > 0).
            throw new NotImplementedException();
        }

        public async Task UpdateAvailableBedsAsync(int providerId, int bedsCount)
        {
            // TODO: GetByIdAsync, set AvailableBeds=bedsCount, Update.
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HealthcareProvider>> SearchProvidersAsync(string searchTerm, decimal? latitude, decimal? longitude)
        {
            // TODO: Filter by name containing searchTerm, optionally sort by distance.
            throw new NotImplementedException();
        }

    }
}