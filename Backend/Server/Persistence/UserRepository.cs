using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Core.Helper;

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
                .AsNoTracking()
                //.Include(i => i.Rentals) // ICollection
                //.Include(i => i.OwnedItems) // ICollection
                .Where(w=> w.IsDeleted == false)
                .OrderBy(o=> o.LastName)
                .ThenBy(t=> t.FirstName)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await DbContext.Users
                .AsNoTracking()
                //.Include(i=> i.Rentals) // ICollection
                //.Include(i=> i.OwnedItems) // ICollection
                .Where(w=> w.IsDeleted == false)
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

        public void SoftDelete(int id)
        {
            var user = DbContext.Users
                .Include(u => u.OwnedItems)
                    .ThenInclude(i => i.Images)
                .Include(u => u.OwnedItems)
                    .ThenInclude(i => i.Rentals)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;


            foreach (var item in user.OwnedItems)
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

        public async Task<User?> AuthenticateSimpleAsync(string username, string password)
        {
            var hashedPassword = SecurityHelper.HashPasswordSimple(password);

            return await DbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted &&
                            u.Username == username &&
                            u.PasswordHash == hashedPassword)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await DbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted && u.Username == username)
                .FirstOrDefaultAsync();

            if (user is null)
                return null;

            bool isValid = SecurityHelper.VerifyPassword(password, user.PasswordHash);

            return isValid ? user : null;
        }



    }
}