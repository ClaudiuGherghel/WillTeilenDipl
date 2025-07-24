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
                Name = categoryDto.Name ?? string.Empty,
            };
        }


        public static void UpdateEntity(this CategoryPutDto categoryDto, Category categoryToPut)
        {
            categoryToPut.Name = categoryDto.Name ?? string.Empty;
            categoryToPut.RowVersion = categoryDto.RowVersion;
        }
    }
}
