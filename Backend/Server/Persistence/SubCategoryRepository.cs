using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class SubCategoryRepository(ApplicationDbContext dbContext) : ISubCategoryRepository
    {

        public ApplicationDbContext DbContext { get; } = dbContext;


        public async Task<int> CountAsync()
        {
            return await DbContext.SubCategories
                .CountAsync();
        }

        public async Task<ICollection<SubCategory>> GetAllAsync()
        {
            return await DbContext.SubCategories
                .AsNoTracking()
                //.Include(i => i.Category)
                .Include(i => i.Items) //ICollection
                    .ThenInclude(i => i.GeoPostal)
                .Where(w => w.IsDeleted == false)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<SubCategory?> GetByIdAsync(int id)
        {
            return await DbContext.SubCategories
                .AsNoTracking()
                //.Include(i => i.Category)
                .Include(i => i.Items)
                    .ThenInclude(i=> i.GeoPostal)
                .Where(w => w.IsDeleted == false)
                .SingleOrDefaultAsync(s => s.Id == id);
        }


public async Task<SubCategoryWithMainImageDto?> GetWithMainImageByIdAsync(int id)
{
    return await DbContext.SubCategories
        .AsNoTracking()
        .Where(sc => sc.Id == id && !sc.IsDeleted)
        .Select(sc => new SubCategoryWithMainImageDto
        {
            Id = sc.Id,
            Name = sc.Name,
            ItemsDto = sc.Items
                .Where(i => !i.IsDeleted)
                .Select(i => new ItemForSearchQueryDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    Deposit = i.Deposit,
                    RentalType = i.RentalType,
                    ItemCondition = i.ItemCondition,
                    GeoPostal = i.GeoPostal,
                    MainImage = i.Images
                        .Where(img => img.IsMainImage && !img.IsDeleted)
                        .Select(img => new ImageForSearchQueryDto
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl,
                            AltText = img.AltText
                        })
                        .FirstOrDefault() // entweder das MainImage oder null
                })
                .ToList()
        })
        .FirstOrDefaultAsync();
}

        public void Insert(SubCategory subCategoryToPost)
        {
            DbContext.SubCategories.Add(subCategoryToPost);
        }

        public void Update(SubCategory subCategoryToPut)
        {
            DbContext.SubCategories.Update(subCategoryToPut);
        }

        public void Delete(SubCategory subCategoryToRemove)
        {
            DbContext.SubCategories.Remove(subCategoryToRemove);
        }

        public void SoftDelete(int id)
        {
            var subCategory = DbContext.SubCategories
                       .Include(sc => sc.Items)
                           .ThenInclude(i => i.Images)
                       .Include(sc => sc.Items)
                           .ThenInclude(i => i.Rentals)
                   .FirstOrDefault(sc => sc.Id == id);

            if (subCategory == null)
                return;

            subCategory.IsDeleted = true;
            subCategory.UpdatedAt = DateTime.UtcNow;


            foreach (var item in subCategory.Items)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;

                foreach (var img in item.Images)
                {
                    img.IsDeleted = true;
                    img.UpdatedAt = DateTime.UtcNow;
                }

                foreach (var rental in item.Rentals)
                {
                    rental.IsDeleted = true;
                    rental.UpdatedAt = DateTime.UtcNow;
                }
            }

        }


    }
}