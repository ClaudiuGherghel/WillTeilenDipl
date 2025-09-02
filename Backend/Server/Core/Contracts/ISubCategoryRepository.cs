using Core.Dtos;
using Core.Entities;

namespace Core.Contracts
{
    public interface ISubCategoryRepository
    {
        Task<int> CountAsync();
        void Delete(SubCategory subCategoryToRemove);
        Task<ICollection<SubCategory>> GetAllAsync();
        Task<SubCategory?> GetByIdAsync(int id);
        Task<SubCategoryWithMainImageDto?> GetWithMainImageByIdAsync(int id);
        void Insert(SubCategory subCategoryToPost);
        void SoftDelete(int id);
        void Update(SubCategory subCategoryToPut);
    }
}