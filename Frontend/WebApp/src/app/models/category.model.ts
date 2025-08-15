import { SubCategory } from "./sub-category.model";

export interface Category {
    id: number,
    rowVersoin: any,
    name: string,
    subCategories: SubCategory[]
}
