using Etmen_Domain.Entities;
using Etmen_DAL.DbContext;
using Etmen_DAL.Helpers;
using Etmen_DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etmen_DAL.Repositories.Implementations
{
    public class HealthcareProviderRepository : GenericRepository<HealthcareProvider>, IHealthcareProviderRepository
    {
        public HealthcareProviderRepository(EtmenDbContext context) : base(context) { }

        public async Task<IEnumerable<HealthcareProvider>> GetNearbyProvidersAsync(decimal latitude, decimal longitude, decimal radiusInKm)
        {
            var providers = await GetAllAsync();
            return providers.Where(p => GeoHelper.CalculateDistance(latitude, longitude, p.Latitude, p.Longitude) <= radiusInKm);
        }

        public async Task<IEnumerable<HealthcareProvider>> GetEmergencyCentersAsync(decimal latitude, decimal longitude, decimal radiusInKm)
        {
            var nearbyProviders = await GetNearbyProvidersAsync(latitude, longitude, radiusInKm);
            return nearbyProviders.Where(p => p.IsEmergencyCenter);
        }

        public async Task<IEnumerable<HealthcareProvider>> GetByTypeAsync(string type)
        {
            return await FindAsync(p => p.Type == type);
        }

        public async Task<IEnumerable<HealthcareProvider>> GetWithAvailableBedsAsync()
        {
            return await FindAsync(p => p.AvailableBeds.HasValue && p.AvailableBeds > 0);
        }

        public async Task UpdateAvailableBedsAsync(int providerId, int bedsCount)
        {
            var provider = await GetByIdAsync(providerId);
            if (provider != null)
            {
                provider.AvailableBeds = bedsCount;
                Update(provider);
            }
        }

        public async Task<IEnumerable<HealthcareProvider>> SearchProvidersAsync(string searchTerm, decimal? latitude, decimal? longitude)
        {
            var providers = await FindAsync(p => p.Name.Contains(searchTerm));

            if (latitude.HasValue && longitude.HasValue)
            {
                return providers.OrderBy(p => GeoHelper.CalculateDistance(latitude.Value, longitude.Value, p.Latitude, p.Longitude));
            }

            return providers;
        }

    }
}