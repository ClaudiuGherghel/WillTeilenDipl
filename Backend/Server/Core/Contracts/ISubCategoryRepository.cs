using Core.Entities;

namespace Core.Contracts
{
    public interface ISubCategoryRepository
    {
        void Delete(SubCategory subCategory);
        Task<ICollection<SubCategory>> GetAllAsync();
        Task<SubCategory?> GetByIdAsync(int id);
        void Insert(SubCategory newSubCategory);
        void Update(SubCategory subCategory);
    }
}