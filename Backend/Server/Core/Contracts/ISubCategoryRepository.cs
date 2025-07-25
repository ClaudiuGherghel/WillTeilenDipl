﻿using Core.Entities;

namespace Core.Contracts
{
    public interface ISubCategoryRepository
    {
        void Delete(SubCategory subCategoryToRemove);
        Task<ICollection<SubCategory>> GetAllAsync();
        Task<SubCategory?> GetByIdAsync(int id);
        void Insert(SubCategory subCategoryToPost);
        void SoftDelete(int id);
        void Update(SubCategory subCategoryToPut);
    }
}