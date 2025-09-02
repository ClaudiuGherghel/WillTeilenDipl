using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Persistence
{
    internal class ImageRepository(ApplicationDbContext dbContext) : IImageRepository
    {

        public ApplicationDbContext DbContext { get; } = dbContext;


        public async Task<int> CountAsync(int itemId)
        {
            return await DbContext.Images.CountAsync(i => !i.IsDeleted && i.ItemId == itemId);
        }


        public async Task<ICollection<Image>> GetAllAsync(int itemId = 0)
        {

            IQueryable<Image> query = DbContext.Images.AsQueryable();

            if (itemId != 0)
            {
                query = query.Where(i => i.ItemId == itemId);
            }

            return await query
                .AsNoTracking()
                .Where(i => !i.IsDeleted)
                .OrderBy(i => i.IsMainImage)
                .ThenBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<Image?> GetByIdAsync(int id)
        {
            return await DbContext.Images
                .AsNoTracking()
                //.Include(i => i.Item)
                .Where(i => !i.IsDeleted)
                .SingleOrDefaultAsync(s => s.Id == id);
        }


        public async Task<ICollection<Image>> GetByItemIdAsync(int itemId)
        {
            return await DbContext.Images
                .AsNoTracking()
                .Where(i => i.ItemId == itemId && !i.IsDeleted)
                .OrderByDescending(i => i.IsMainImage)
                .ThenBy(i => i.Id)
                .ToListAsync();
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

        public void SoftDelete(int id)
        {
            var image = DbContext.Images
                .FirstOrDefault(img => img.Id == id);

            if (image == null)
                return;

            image.IsMainImage = false;
            image.IsDeleted = true;
            image.UpdatedAt = DateTime.UtcNow;

        }

        public async Task<Image?> GetOtherMainImageAsync(int itemId, int imageId)
        {
            return await DbContext.Images
                .AsNoTracking()
                .Where(i => i.ItemId == itemId && i.Id != imageId && !i.IsDeleted && i.IsMainImage)
                .FirstOrDefaultAsync();
        }


    }
}