import { ItemCondition } from "../enums/item-condition";
import { RentalType } from "../enums/rental-type";

export interface SidebarFilter {
    country: string;
    state: string;
    postalCode: string;
    place: string;
    price: number,
    deposit: number;
    rentalType: RentalType;
    itemCondition: ItemCondition;
}
