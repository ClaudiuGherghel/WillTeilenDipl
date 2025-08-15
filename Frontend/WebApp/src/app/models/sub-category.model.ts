import { Category } from "./category.model";
import { Item } from "./item.model";

export interface SubCategory {
    id: number,
    rowVersoin: any,
    name: string,
    category: Category,
    categoryId: number,
    items: Item[],
}
