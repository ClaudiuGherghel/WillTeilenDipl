using Core.Contracts;

namespace Persistence
{
    internal class GeoPostalRepository : IGeoPostalRepository
    {
        public GeoPostalRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}