using Core.Entities;

namespace Core.Contracts
{
    public interface IUserRepository
    {
        void Delete(User userToRemove);
        Task<ICollection<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        void Insert(User userToPost);
        void Update(User userToPut);
    }
}