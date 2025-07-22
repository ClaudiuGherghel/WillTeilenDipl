using Core.Contracts;

namespace Persistence
{
    internal class UserRepository : IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }
    }
}