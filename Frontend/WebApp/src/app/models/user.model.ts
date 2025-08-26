import { Roles } from "../enums/roles";
import { GeoPostal } from "./geo-postal.model";
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
    geoPostalId: number,
    // subCategory: SubCategory | null,
    // owern: User | null,
    // geoPostal: GeoPostal,
    address: string,
    phoneNumber: string,
    rentals: Rental[],
    ownedItems: Item[]
}
