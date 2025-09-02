using Core.Entities;

namespace Core.Contracts
{
    public interface IImageRepository
    {
        Task<int> CountAsync(int itemId);
        void Delete(Image imageToRemove);
        Task<ICollection<Image>> GetAllAsync(int itemId = 0);
        Task<Image?> GetByIdAsync(int id);
        Task<ICollection<Image>> GetByItemIdAsync(int itemId);
        Task<Image?> GetOtherMainImageAsync(int itemId, int imageId);
        void Insert(Image imageToPost);
        void SoftDelete(int id);
        void Update(Image imageToPut);
    }
}