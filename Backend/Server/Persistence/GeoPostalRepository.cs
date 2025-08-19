using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class GeoPostalRepository : IGeoPostalRepository
    {
        public GeoPostalRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }

        public async Task<int> CountAsync()
        {
            return await DbContext.GeoPostals.CountAsync();
        }

        public async Task<GeoPostal?> GetByIdAsync(int id)
        {
            return await DbContext.GeoPostals
                .AsNoTracking()
                .FirstOrDefaultAsync(gp=> gp.Id == id);
        }

        public async Task<ICollection<string>> GetCountriesAsync()
        {
            return await DbContext.GeoPostals
                .AsNoTracking()
                .Select(gp => gp.Country)
                .OrderBy(gp=> gp)
                .Distinct()
                .ToListAsync();
        }

        public async Task<ICollection<PostalCodeAndPlaceDto>> GetPostalCodesAndPlacesAsync(string state)
        {
            return await DbContext.GeoPostals
                .AsNoTracking()
                .Where(gp => gp.State == state)
                .Select(gp => new PostalCodeAndPlaceDto()
                {
                    Place = gp.Place,
                    PostalCode = gp.PostalCode
                })
                .OrderBy(gp => gp.PostalCode)
                .Distinct()
                .ToListAsync();
        }

        public async Task<ICollection<string>> GetStatesAsync(string country)
        {
            return await DbContext.GeoPostals
                .AsNoTracking()
                .Where(gp => gp.Country == country)
                .Select(gp => gp.State)
                .OrderBy(gp => gp)
                .Distinct()
                .ToListAsync();
        }
    }
}