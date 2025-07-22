using Core.Contracts;

namespace Persistence
{
    internal class SubCategoryRepository : ISubCategoryRepository
    {
        public SubCategoryRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}