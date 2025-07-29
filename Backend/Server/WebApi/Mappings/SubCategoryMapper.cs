using Core.Entities;
using WebApi.Controllers;
using static WebApi.Dtos.SubCategoryDto;

namespace WebApi.Mappings
{
    public static class SubCategoryMapper
    {
        public static SubCategory ToEntity(this SubCategoryPostDto subCategoryDto)
        {
            return new SubCategory
            {
                CreatedAt = DateTime.UtcNow,
                CategoryId = subCategoryDto.CategoryId,
                Name = subCategoryDto.Name ?? string.Empty,
            };
        }


        public static void UpdateEntity(this SubCategoryPutDto subCategoryDto, SubCategory subCategoryToPut)
        {
            subCategoryToPut.UpdatedAt = DateTime.UtcNow;
            subCategoryToPut.RowVersion = subCategoryDto.RowVersion;
            subCategoryToPut.Name = subCategoryDto.Name ?? string.Empty;
            subCategoryToPut.CategoryId = subCategoryDto.CategoryId;
        }
    }
}
