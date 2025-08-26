import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { ItemCondition } from '../../enums/item-condition';
import { RentalType } from '../../enums/rental-type';
import { SidebarFilter } from '../../models/sidebar-filter';
import { FormsModule } from '@angular/forms';
import { CommonDataService } from '../../services/common-data-service';
import { GeoPostalService } from '../../services/geo-postal-service';
import { PostalCodeAndPlaceDto } from '../../dtos/postal-code-and-place-dto';

@Component({
  selector: 'app-side-bar',
  imports: [FormsModule],
  standalone: true,
  templateUrl: './side-bar.html',
  styleUrl: './side-bar.css'
})
export class SideBar implements OnInit {

  private geoPostalService = inject(GeoPostalService);
  protected commonDataService = inject(CommonDataService);
  filterEmitter = output<SidebarFilter>();

  countries = signal<string[]>([]);
  selectedCountry = signal('');

  states = signal<string[]>([]);
  selectedState = signal('');

  postalCodesAndPlaces = signal<PostalCodeAndPlaceDto[]>([]);
  selectedPostalCode = signal('');
  selectedPlace = signal('');

  rentalTypes = signal<RentalType[]>([]);
  selectedRentalType = signal<RentalType>(RentalType.Unknown);

  itemConditions = signal<ItemCondition[]>([]);
  selectedItemCondition = signal<ItemCondition>(ItemCondition.Unknown);


  deposit = signal<number | undefined>(undefined);
  price = signal<number | undefined>(undefined);

  ngOnInit(): void {
    this.loadCountries();
    this.rentalTypes.set(this.commonDataService.getRentalTypeList());
    this.itemConditions.set(this.commonDataService.getItemConditionList());

  }
  loadCountries() {
    this.geoPostalService.getCountries().subscribe({
      next: data => {
        this.countries.set(data);
      },
      error: error => {
        alert("Laden der Länder fehlgeschlagen: " + error.message);
      }
    });
  }

  onCountryChanged() {
    this.geoPostalService.getStates(this.selectedCountry()).subscribe({
      next: data => {
        this.states.set(data);
        this.selectedState.set('');
        this.selectedPostalCode.set('');
        this.selectedPlace.set('');
      },
      error: err => {
        alert("Laden der Bundesländer fehlgeschlagen: " + err.message);
      }
    });
  }

  onStateChanged() {
    this.geoPostalService.getPostlCodesAndPlaces(this.selectedState()).subscribe({
      next: data => {
        this.postalCodesAndPlaces.set(data);
        this.selectedPostalCode.set('');
        this.selectedPlace.set('');
      },
      error: err => {
        alert("Laden der Postleitzahlen/Orte fehlgeschlagen: " + err.message);
      }
    });
  }

  onPostalCodeChanged() {
    const match = this.postalCodesAndPlaces().find(p => p.postalCode === this.selectedPostalCode());
    if (match) {
      this.selectedPlace.set(match.place);
    }
  }

  onPlaceChanged() {
    const match = this.postalCodesAndPlaces().find(p => p.place === this.selectedPlace());
    if (match) {
      this.selectedPostalCode.set(match.postalCode);
    }
  }

  onEmitFilter() {
    let filter: SidebarFilter = {
      country: this.selectedCountry(),
      state: this.selectedState(),
      postalCode: this.selectedPostalCode(),
      place: this.selectedPlace(),
      price: this.price() ?? 0,
      deposit: this.deposit() ?? 0,
      rentalType: this.selectedRentalType(),
      itemCondition: this.selectedItemCondition(),
    }
    console.log(filter);
    this.filterEmitter.emit(filter)
  }
}
