using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class UserRepository(ApplicationDbContext dbContext) : IUserRepository
    {


        private ApplicationDbContext DbContext { get; } = dbContext;
        
        public async Task<int> CountAsync()
        {
            return await DbContext.Users.CountAsync();
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            return await DbContext.Users
                //.Include(i=> i.Reservations)
                //.Include(i=> i.UserItems)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await DbContext.Users
                //.Include(i => i.Reservations)
                //.Include(i => i.UserItems)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == id);
        }
        public void Insert(User userToPost)
        {
            DbContext.Users.Add(userToPost);
        }

        public void Update(User userToPut)
        {
            DbContext.Users.Update(userToPut);
        }

        public void Delete(User userToRemove)
        {
            DbContext.Users.Remove(userToRemove);
        }

    }
}