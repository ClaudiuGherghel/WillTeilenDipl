import { Injectable } from '@angular/core';
import { ItemCondition } from '../enums/item-condition';
import { RentalType } from '../enums/rental-type';
import { RentalStatus } from '../enums/rental-status';

@Injectable({
  providedIn: 'root'
})
export class CommonDataService {

  private rentalStatus: RentalStatus[] = [
    RentalStatus.Active,
    RentalStatus.Cancelled,
    RentalStatus.Completed,
  ];


  private rentalTypes: RentalType[] = [
    RentalType.Unknown,
    RentalType.Privat,
    RentalType.Dealer,
  ];

  private itemConditions: ItemCondition[] = [
    ItemCondition.Unknown,
    ItemCondition.LikeNew,
    ItemCondition.Good,
    ItemCondition.Used,
  ]

  constructor() { }


  getRentalStatus(): RentalStatus[] {
    return this.rentalStatus;
  }

  // Methode, um den Enum-Wert in einen lesbaren String umzuwandeln
  getRentalStatusLabel(rentalType: RentalStatus): string {
    switch (rentalType) {
      case RentalStatus.Active:
        return 'Aktiv';
      case RentalStatus.Cancelled:
        return 'Abgebrochen';
      case RentalStatus.Completed:
        return 'Abgeschlossen';
      default:
        return '';
    }
  }


  getRentalTypes(): RentalType[] {
    return this.rentalTypes;
  }

  getRentalTypeLabel(rentalType: RentalType): string {
    switch (rentalType) {
      case RentalType.Unknown:
        return 'Unbekannt';
      case RentalType.Privat:
        return 'Privat';
      case RentalType.Dealer:
        return 'Dealer';
      default:
        return '';
    }
  }


  getItemConditions(): ItemCondition[] {
    return this.itemConditions;
  }
  getItemConditionLabel(condition: ItemCondition): string {
    switch (condition) {
      case ItemCondition.Unknown:
        return 'Unbekannt';
      case ItemCondition.LikeNew:
        return 'Wie Neu';
      case ItemCondition.Good:
        return 'Gut';
      case ItemCondition.Used:
        return 'Gebraucht';
      default:
        return '';
    }
  }

}
