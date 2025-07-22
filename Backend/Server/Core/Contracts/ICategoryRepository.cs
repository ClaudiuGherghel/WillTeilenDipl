using Core.Entities;

namespace Core.Contracts
{
    public interface ICategoryRepository
    {
        void Delete(Category categoryToRemove);
        Task<ICollection<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        void Insert(Category categoryToPost);
        void Update(Category categoryToPut);
    }
}