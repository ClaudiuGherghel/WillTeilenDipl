using Core.Contracts;

namespace Persistence
{
    internal class ItemRepository : IItemRepository
    {
        public ItemRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}