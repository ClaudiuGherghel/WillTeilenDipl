using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class SubCategoryWithMainImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ItemForSearchQueryDto> ItemsDto { get; set; } = [];
    }
}
