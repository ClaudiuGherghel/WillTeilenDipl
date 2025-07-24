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
                //.Include(i=> i.SubCategory)
                //.Include(i => i.Reservations)  //ICollection
                //.Include(i => i.Images) //ICollection
                //.Include(i => i.UserItems) //ICollection
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int itemId)
        {
            return await DbContext.Items
                //.Include(i => i.SubCategory)
                //.Include(i => i.Reservations)  //ICollection
                //.Include(i => i.Images) //ICollection
                //.Include(i => i.UserItems) //ICollection
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == itemId);
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
                EF.Functions.Like(w.Country.ToUpper(), $"%{filter}%") ||
                EF.Functions.Like(w.State.ToUpper(), $"%{filter}%") ||
                EF.Functions.Like(w.Place.ToUpper(), $"%{filter}%") ||
                EF.Functions.Like(w.Name.ToUpper(), $"{filter}%") ||
                EF.Functions.Like(w.Description.ToUpper(), $"%{filter}%") ||
                w.PostalCode.StartsWith(filter) ||
                (w.SubCategory != null && EF.Functions.Like(w.SubCategory.Name.ToUpper(), $"{filter}%"))
            );

            return await query
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .ToListAsync();
        }
    }
}