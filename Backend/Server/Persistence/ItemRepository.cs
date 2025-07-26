using Core.Contracts;
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
                .AsNoTracking()
                //.Include(i=> i.SubCategory)
                //.Include(i=> i.Rentals)
                //.Include(i => i.Images) //ICollection
                //.Include(i => i.Images) //ICollection
                .Where(w => w.IsDeleted == false)
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await DbContext.Items
                .AsNoTracking()
                //.Include(i=> i.SubCategory)
                //.Include(i=> i.Rentals)
                //.Include(i => i.Images) //ICollection
                //.Include(i => i.Images) //ICollection
                .Where(w => w.IsDeleted == false)
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

        public async Task<ICollection<Item>> GetItemsByFilterAsync(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return [];
            }

            filter = filter.ToUpper();

            var query = DbContext.Items.AsQueryable();

            // Nur bei Bedarf Includes laden
            //query = query.Include(i => i.SubCategory)
            //             .ThenInclude(sc => sc.Category);

            // Filterbedingungen aufbauen
            query = query.Where(w =>
                w.IsDeleted == false &&
                (EF.Functions.Like(w.Country.ToUpper(), $"%{filter}%") ||
                EF.Functions.Like(w.State.ToUpper(), $"%{filter}%") ||
                EF.Functions.Like(w.Place.ToUpper(), $"%{filter}%") ||
                EF.Functions.Like(w.Name.ToUpper(), $"{filter}%") ||
                EF.Functions.Like(w.Description.ToUpper(), $"%{filter}%") ||
                w.PostalCode.StartsWith(filter) ||
                (w.SubCategory != null && EF.Functions.Like(w.SubCategory.Name.ToUpper(), $"{filter}%")))
            );

            return await query
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .OrderBy(o => o.Name)
                .ToListAsync();
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