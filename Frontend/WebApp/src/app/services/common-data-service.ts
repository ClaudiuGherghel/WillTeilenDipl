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
  ]

  constructor() { }


  // -------- Rental Status --------

  getRentalStatusList(): RentalStatus[] {
    return this.rentalStatusList;
  }

  getRentalStatusLabelGerman(status: RentalStatus): string {
    switch (status) {
      case RentalStatus.Active:
        return 'Aktiv';
      case RentalStatus.Cancelled:
        return 'Abgebrochen';
      case RentalStatus.Completed:
        return 'Abgeschlossen';
      default:
        return 'Aktiv';
    }
  }


  getRentalStatusLabelEnglish(status: RentalStatus): string {
    switch (status) {
      case RentalStatus.Active:
        return 'Activ';
      case RentalStatus.Cancelled:
        return 'Cancalled';
      case RentalStatus.Completed:
        return 'Completed';
      default:
        return 'Aktiv';
    }
  }


  // -------- Rental Types --------

  getRentalTypeList(): RentalType[] {
    return this.rentalTypeList;
  }

  getRentalTypeLabelGerman(type: RentalType): string {
    switch (type) {
      case RentalType.Unknown:
        return 'Unbekannt';
      case RentalType.Privat:
        return 'Privat';
      case RentalType.Dealer:
        return 'Dealer';
      default:
        return 'Unbekannt';
    }
  }

  getRentalTypeLabelEnglish(type: RentalType): string {
    switch (type) {
      case RentalType.Unknown:
        return 'Unknown';
      case RentalType.Privat:
        return 'Privat';
      case RentalType.Dealer:
        return 'Dealer';
      default:
        return 'Unknown';
    }
  }



  // -------- Item Conditions --------

  getItemConditionList(): ItemCondition[] {
    return this.itemConditionList;
  }
  getItemConditionLabelGerman(condition: ItemCondition): string {
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
        return 'Unbekannt';
    }
  }

  getItemConditionLabelEnglish(condition: ItemCondition): string {
    switch (condition) {
      case ItemCondition.Unknown:
        return 'Unknown';
      case ItemCondition.LikeNew:
        return 'LikeNew';
      case ItemCondition.Good:
        return 'Good';
      case ItemCondition.Used:
        return 'Used';
      default:
        return 'Unknown';
    }
  }



}
