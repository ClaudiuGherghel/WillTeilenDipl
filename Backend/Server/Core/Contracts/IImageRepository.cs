using Core.Entities;

namespace Core.Contracts
{
    public interface IImageRepository
    {
        void Delete(Image imageToRemove);
        Task<ICollection<Image>> GetAllAsync();
        Task<Image?> GetByIdAsync(int id);
        void Insert(Image imageToPost);
        void Update(Image imageToPut);
    }
}