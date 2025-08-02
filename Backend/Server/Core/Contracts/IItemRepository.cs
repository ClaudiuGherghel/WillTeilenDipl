using Core.Entities;

namespace Core.Contracts
{
    public interface IItemRepository
    {
        Task<int> CountAsync();
        void Delete(Item itemToRemove);
        Task<ICollection<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(int id);
        Task<ICollection<Item>> GetItemsByFilterAsync(string filter);
        void Insert(Item itemToPost);
        void SoftDelete(int id);
        void Update(Item itemToPut);
    }
}