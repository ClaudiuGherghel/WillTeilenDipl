using Core.Contracts;

namespace Persistence
{
    internal class RentalRepository : IRentalRepository
    {
        public RentalRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}