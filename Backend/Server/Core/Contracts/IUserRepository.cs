using Core.Entities;

namespace Core.Contracts
{
    public interface IUserRepository
    {
        Task<int> CountAsync();
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> AuthenticateSimpleAsync(string username, string password);
        void Delete(User userToRemove);
        Task<ICollection<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        void Insert(User userToPost);
        void SoftDelete(int id);
        void Update(User userToPut);
        Task<User?> GetWithoutReferencesByIdAsync(int id);
    }
}