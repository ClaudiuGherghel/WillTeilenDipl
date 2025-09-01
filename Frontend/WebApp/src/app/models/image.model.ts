import { Item } from "./item.model";

export interface Image {
    id: number,
    rowVersion: any,
    imageUrl: string,
    altText: string,
    mimeType: string,
    displayOrder: number,
    isMainImage: boolean,
    itemId: number,
    item: Item | null,
}

export interface ImagePostDo {
    id: number,
    imageUrl: string,
    altText: string,
    displayOrder: number,
    isMainImage: boolean,
    itemId: number,
}

export interface ImagePutDto {
    id: number,
    rowVersion: any,
    imageUrl: string,
    altText: string,
    displayOrder: number,
    isMainImage: boolean,
    itemId: number,
}