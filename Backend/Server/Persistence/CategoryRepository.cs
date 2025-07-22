using Core.Contracts;

namespace Persistence
{
    internal class CategoryRepository : ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}