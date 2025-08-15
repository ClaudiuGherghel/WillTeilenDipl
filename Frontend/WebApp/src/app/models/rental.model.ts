import { RentalStatus } from "../enums/rental-status";
import { Item } from "./item.model";
import { User } from "./user.model";

export interface Rental {
    id: number,
    rowVersoin: any,
    from: Date,
    to: Date,
    note: string,
    status: RentalStatus,
    renterId: number,
    ownerId: number,
    itemId: number,
    renter: User,
    owner: User,
    item: Item
}
