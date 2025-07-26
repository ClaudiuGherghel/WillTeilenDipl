using Core.Entities;
using WebApi.Controllers;

namespace WebApi.Mappings
{
    public static class CategoryMapper
    {
        public static Category ToEntity(this CategoryPostDto categoryDto)
        {
            return new Category
            {
                CreatedAt = DateTime.UtcNow,
                Name = categoryDto.Name ?? string.Empty,
            };
        }


        public static void UpdateEntity(this CategoryPutDto categoryDto, Category categoryToPut)
        {
            categoryToPut.UpdatedAt = DateTime.UtcNow;
            categoryToPut.RowVersion = categoryDto.RowVersion;
            categoryToPut.Name = categoryDto.Name ?? string.Empty;
        }
    }
}
