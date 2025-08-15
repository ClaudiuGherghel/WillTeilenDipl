import { Roles } from "../enums/roles";
import { Item } from "./item.model";
import { Rental } from "./rental.model";

export interface User {
    id: number,
    rowVersoin: any,
    userName: string,
    password: string,
    role: Roles,
    email: string,
    firstName: string,
    lastName: string,
    birthDate: Date,
    country: string,
    postalCode: string,
    place: string,
    address: string,
    phoneNumber: string,
    rentals: Rental[],
    ownedItems: Item[]
}
