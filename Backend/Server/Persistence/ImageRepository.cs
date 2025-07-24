using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class ImageRepository(ApplicationDbContext dbContext) : IImageRepository
    {

        public ApplicationDbContext DbContext { get; } = dbContext;


        public async Task<int> CountAsync()
        {
            return await DbContext.Images.CountAsync();
        }


        public async Task<ICollection<Image>> GetAllAsync()
        {
            return await DbContext.Images
                //.Include(i => i.Item)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Image?> GetByIdAsync(int id)
        {
            return await DbContext.Images
                //.Include(i => i.Item)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == id);
        }
        public void Insert(Image imageToPost)
        {
            DbContext.Images.Add(imageToPost);
        }

        public void Update(Image imageToPut)
        {
            DbContext.Images.Update(imageToPut);
        }

        public void Delete(Image imageToRemove)
        {
            DbContext.Images.Remove(imageToRemove);
        }

    }
}