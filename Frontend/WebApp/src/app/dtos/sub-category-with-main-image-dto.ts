import { ItemDtoForSearchQuery } from "./item-dto-for-search-query";

export interface SubCategoryWithMainImageDto {
    id: number,
    name: string,
    itemsDto: ItemDtoForSearchQuery[]
}
