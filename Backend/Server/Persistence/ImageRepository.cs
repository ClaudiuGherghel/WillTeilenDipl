using Core.Contracts;

namespace Persistence
{
    internal class ImageRepository : IImageRepository
    {
        public ImageRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}