import { ItemCondition } from "../enums/item-condition";
import { RentalType } from "../enums/rental-type";
import { Image } from "./image.model";
import { Rental } from "./rental.model";
import { SubCategory } from "./sub-category.model";
import { User } from "./user.model";

export interface Item {
    id: number,
    rowVersoin: any,
    name: string,
    description: string,
    isAvailable: boolean,
    country: string,
    state: string,
    postalCode: string,
    place: string,
    address: string,
    price: number,
    stock: number,
    deposit: number,
    rentalType: RentalType,
    itemCondition: ItemCondition,
    subCategoryId: number,
    ownerId: number,
    subCategory: SubCategory | null,
    owern: User | null,
    rentals: Rental[],
    images: Image[]
}
