import { Item } from "./item.model";

export interface Image {
    id: number,
    rowVersoin: any,
    imageUrl: string,
    altText: string,
    mimeType: string,
    itemId: number,
    item: Item | null,
}
