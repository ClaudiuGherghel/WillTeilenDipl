import { GeoPostal } from "../models/geo-postal.model";
import { ImageDtoForSeachQuery } from "./image-dto-for-seach-query";

export interface ItemDtoForSearchQuery {
    id: number,
    name: string,
    description: string,
    price: number,
    deposit: number,
    rentalType: string,
    itemCondition: string,
    geoPostal: GeoPostal,
    mainImage: ImageDtoForSeachQuery | null,
}
