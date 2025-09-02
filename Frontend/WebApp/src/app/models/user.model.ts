import { Roles } from "../enums/roles";
import { GeoPostal } from "./geo-postal.model";
import { Item } from "./item.model";
import { Rental } from "./rental.model";

export interface User {
    id: number,
    rowVersion: any,
    userName: string,
    password: string,
    role: Roles,
    email: string,
    firstName: string,
    lastName: string,
    birthDate: string, //Backend liefert ISO-String
    geoPostalId: number,
    // subCategory: SubCategory | null,
    // owern: User | null,
    geoPostal: GeoPostal,
    address: string,
    phoneNumber: string,
    rentals: Rental[],
    ownedItems: Item[]
}


export interface UserPutDo {
    id: number,
    rowVersion: any,
    userName: string,
    email: string,
    firstName: string,
    lastName: string,
    birthDate: string,
    geoPostalId: number,
    address: string,
    phoneNumber: string,
}

export interface UserChangePwDto {
    id: number,
    rowVersion: any,
    currentPassword: string,
    newPassword: string,
}