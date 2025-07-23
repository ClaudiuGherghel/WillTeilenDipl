using Core.Entities;
using WebApi.Controllers;

namespace WebApi.Mappings
{
    public static class SubCategoryMapping
    {
        public static SubCategory ToEntity(this SubCategoryPostDto subCategoryDto)
        {
            return new SubCategory
            {
                CategoryId = subCategoryDto.CategoryId,
                Name = subCategoryDto.Name,
            };
        }


        public static void UpdateEntity(this SubCategoryPutDto subCategoryDto, SubCategory subCategoryToPut)
        {
            subCategoryToPut.Name = subCategoryDto.Name;
            subCategoryToPut.CategoryId = subCategoryDto.CategoryId;
            subCategoryToPut.RowVersion = subCategoryDto.RowVersion;
        }
    }
}
