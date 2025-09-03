import { Injectable } from '@angular/core';
import { ItemCondition } from '../enums/item-condition';
import { RentalType } from '../enums/rental-type';
import { RentalStatus } from '../enums/rental-status';


@Injectable({
  providedIn: 'root'
})
export class CommonDataService {

  private rentalStatusList: RentalStatus[] = [
    RentalStatus.Active,
    RentalStatus.Cancelled,
    RentalStatus.Completed,
  ];

  private rentalTypeList: RentalType[] = [
    RentalType.Unknown,
    RentalType.Privat,
    RentalType.Dealer,
  ];

  private itemConditionList: ItemCondition[] = [
    ItemCondition.Unknown,
    ItemCondition.LikeNew,
    ItemCondition.Good,
    ItemCondition.Used,
  ];

  constructor() { }

  // -------- Rental Status --------
  getRentalStatusList(): RentalStatus[] {
    return this.rentalStatusList;
  }

  getRentalStatusLabelGerman(status: RentalStatus): string {
    switch (status) {
      case RentalStatus.Active: return 'Aktiv';
      case RentalStatus.Cancelled: return 'Abgebrochen';
      case RentalStatus.Completed: return 'Abgeschlossen';
      default: return 'Aktiv';
    }
  }

  getRentalStatusLabelEnglish(status: RentalStatus): string {
    switch (status) {
      case RentalStatus.Active: return 'Active';
      case RentalStatus.Cancelled: return 'Cancelled';
      case RentalStatus.Completed: return 'Completed';
      default: return 'Active';
    }
  }

  // -------- Rental Types --------
  getRentalTypeList(): RentalType[] {
    return this.rentalTypeList;
  }

  getRentalTypeLabelGerman(type: RentalType): string {
    switch (type) {
      case RentalType.Unknown: return 'Unbekannt';
      case RentalType.Privat: return 'Privat';
      case RentalType.Dealer: return 'Händler';
      default: return 'Unbekannt';
    }
  }

  getRentalTypeLabelEnglish(type: RentalType): string {
    switch (type) {
      case RentalType.Unknown: return 'Unknown';
      case RentalType.Privat: return 'Private';
      case RentalType.Dealer: return 'Dealer';
      default: return 'Unknown';
    }
  }

  // -------- Item Conditions --------
  getItemConditionList(): ItemCondition[] {
    return this.itemConditionList;
  }

  getItemConditionLabelGerman(condition: ItemCondition): string {
    switch (condition) {
      case ItemCondition.Unknown: return 'Unbekannt';
      case ItemCondition.LikeNew: return 'Wie Neu';
      case ItemCondition.Good: return 'Gut';
      case ItemCondition.Used: return 'Gebraucht';
      default: return 'Unbekannt';
    }
  }

  getItemConditionLabelEnglish(condition: ItemCondition): string {
    switch (condition) {
      case ItemCondition.Unknown: return 'Unknown';
      case ItemCondition.LikeNew: return 'Like New';
      case ItemCondition.Good: return 'Good';
      case ItemCondition.Used: return 'Used';
      default: return 'Unknown';
    }
  }

  // -------- Parser: String -> Enum --------
  public parseRentalTypeFromString(s?: string | number): RentalType {
    if (s == null) return RentalType.Unknown;
    const norm = String(s).trim().toLowerCase();

    if (!norm || ['unknown', 'unbekannt'].includes(norm)) return RentalType.Unknown;
    if (['privat', 'private', '1', 'person'].includes(norm)) return RentalType.Privat;
    if (['dealer', 'händler', 'haendler', '2'].includes(norm)) return RentalType.Dealer;

    const n = Number(norm);
    if (!Number.isNaN(n)) { return this.rentalTypeList[n] ?? RentalType.Unknown; }

    return RentalType.Unknown;
  }

  public parseItemConditionFromString(s?: string | number): ItemCondition {
    if (s == null) return ItemCondition.Unknown;
    const norm = String(s).trim().toLowerCase();

    if (!norm || ['unknown', 'unbekannt'].includes(norm)) return ItemCondition.Unknown;
    if (['likenew', 'like new', 'wie neu', '1'].includes(norm)) return ItemCondition.LikeNew;
    if (['good', 'gut', '2'].includes(norm)) return ItemCondition.Good;
    if (['used', 'gebraucht', '3'].includes(norm)) return ItemCondition.Used;

    const n = Number(norm);
    if (!Number.isNaN(n)) { return this.itemConditionList[n] ?? ItemCondition.Unknown; }

    return ItemCondition.Unknown;
  }
}
