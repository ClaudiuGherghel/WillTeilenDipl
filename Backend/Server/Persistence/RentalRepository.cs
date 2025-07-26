using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Persistence
{
    internal class RentalRepository(ApplicationDbContext dbContext) : IRentalRepository
    {

        public ApplicationDbContext DbContext { get; } = dbContext;


        public async Task<int> CountAsync()
        {
            return await DbContext.Rentals
                .CountAsync();
        }

        public async Task<ICollection<Rental>> GetAllAsync()
        {
            return await DbContext.Rentals
                .AsNoTracking()
                //.Include(i => i.Item)
                //.Include(i => i.Renter)
                .Where(w=> w.IsDeleted == false)
                .OrderBy(o => o.From)
                .ThenBy(t=> t.To)
                .ToListAsync();
        }

        public async Task<Rental?> GetByIdAsync(int id)
        {
            return await DbContext.Rentals
                .AsNoTracking()
                //.Include(i => i.Item)
                //.Include(i => i.Renter)
                .Where(w=> w.IsDeleted == false)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public void Insert(Rental rentalToPost)
        {
            DbContext.Rentals.Add(rentalToPost);
        }

        public void Update(Rental rentalToPut)
        {
            DbContext.Rentals.Update(rentalToPut);
        }
        public void Delete(Rental rentalToRemove)
        {
            DbContext.Rentals.Remove(rentalToRemove);
        }

        public void SoftDelete(int id)
        {
            var rental = DbContext.Rentals
                .FirstOrDefault(r => r.Id == id);

            if (rental == null)
                return;

            rental.IsDeleted = true;
            rental.UpdatedAt = DateTime.UtcNow;
        }
    }
}