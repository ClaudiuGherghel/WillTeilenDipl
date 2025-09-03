using Core.Contracts;
using Core.Dtos;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class ItemRepository(ApplicationDbContext dbContext) : IItemRepository
    {

        public ApplicationDbContext DbContext { get; } = dbContext;

        public async Task<int> CountAsync()
        {
            return await DbContext.Items.CountAsync();
        }

        public async Task<ICollection<Item>> GetAllAsync()
        {
            return await DbContext.Items
                //.Include(i=> i.SubCategory)
                //.Include(i=> i.Rentals)
                .Include(i => i.Images) //ICollection
                .Include(i=> i.GeoPostal)
                .AsNoTracking()
                .Where(w => w.IsDeleted == false)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await DbContext.Items
                //.Include(i=> i.SubCategory)
                //.Include(i=> i.Rentals)
                .Include(i => i.Images) //ICollection
                .Include(i => i.GeoPostal)
                .AsNoTracking()
                .Where(w => w.IsDeleted == false && w.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Item?> GetByIdInclDeleted(int id)
        {
            return await DbContext.Items
                //.Include(i=> i.SubCategory)
                //.Include(i=> i.Rentals)
                .Include(i => i.Images) //ICollection
                .Include(i => i.GeoPostal)
                .AsNoTracking()
                .Where(w => w.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<Item>> GetByUserIdAsync(int userId)
        {
            return await DbContext.Items
                .AsNoTracking()
                .Include(i=> i.GeoPostal) 
                .Include(i=> i.Images) //ICollection
                .Where(i => i.IsDeleted == false && i.OwnerId == userId)
                .OrderBy(i=> i.CreatedAt)
                .ToListAsync();          
        }

        public async Task<Item?> GetWithoutReferencesByIdAsync(int id)
        {
            return await DbContext.Items
              .AsNoTracking()
              .Where(w => w.IsDeleted == false && w.Id == id)
              .SingleOrDefaultAsync(s => s.Id == id);
        }


        public void Insert(Item itemToPost)
        {
            DbContext.Items.Add(itemToPost);
        }

        public void Update(Item itemToPut)
        {
            DbContext.Items.Update(itemToPut);
        }

        public void Delete(Item itemToRemove)
        {
            DbContext.Items.Remove(itemToRemove);
        }

        public async Task<ICollection<ItemForSearchQueryDto>> GetFilteredAsync(string filter)
        {
            IQueryable<Item> query = DbContext.Items
                .AsNoTracking()
                .Include(i => i.GeoPostal)
                .Include(i => i.SubCategory)
                .ThenInclude(sc => sc.Category);

            filter = filter.ToUpper();

            query = query.Where(i =>
                EF.Functions.Like(i.Name.ToUpper(), $"{filter}%") ||
                EF.Functions.Like(i.Description.ToUpper(), $"%{filter}%") ||
                (i.SubCategory != null && EF.Functions.Like(i.SubCategory.Name.ToUpper(), $"{filter}%")) ||
                (i.SubCategory != null && i.SubCategory.Category != null && EF.Functions.Like(i.SubCategory.Category.Name.ToUpper(), $"{filter}%")) ||
                (i.GeoPostal != null && EF.Functions.Like(i.GeoPostal.Country.ToUpper(), $"{filter}%")) ||
                (i.GeoPostal != null && EF.Functions.Like(i.GeoPostal.Place.ToUpper(), $"{filter}%")) ||
                (i.GeoPostal != null && EF.Functions.Like(i.GeoPostal.PostalCode.ToUpper(), $"{filter}%")) ||
                (i.GeoPostal != null && EF.Functions.Like(i.GeoPostal.State.ToUpper(), $"{filter}%"))
            );

            var result = await query
                .Where(i => i.IsAvailable)
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
                        .FirstOrDefault()   // kann null sein, wenn Item keine Images hat
                })
                .ToListAsync();

            return result;
        }



        public void SoftDelete(int id)
        {
            var item = DbContext.Items
                .Include(i => i.Images)
                .Include(i => i.Rentals)
                .FirstOrDefault(sc => sc.Id == id);

            if (item == null)
                return;

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