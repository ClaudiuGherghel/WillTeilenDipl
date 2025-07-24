using Core.Entities;

namespace Core.Contracts
{
    public interface IItemRepository
    {
        Task<Item?> GetByIdAsync(int itemId);
    }
}