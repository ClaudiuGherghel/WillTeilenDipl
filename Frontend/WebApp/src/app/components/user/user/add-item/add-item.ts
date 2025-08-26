import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RentalType } from '../../../../enums/rental-type';
import { ItemCondition } from '../../../../enums/item-condition';
import { PostalCodeAndPlaceDto } from '../../../../dtos/postal-code-and-place-dto';
import { CommonDataService } from '../../../../services/common-data-service';
import { GeoPostalService } from '../../../../services/geo-postal-service';
import { Category } from '../../../../models/category.model';
import { CategoryService } from '../../../../services/category-service';
import { AuthService } from '../../../../services/auth-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-item',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-item.html',
  styleUrl: './add-item.css'
})
export class AddItem implements OnInit {

  private geoPostalService = inject(GeoPostalService);
  private categoryService = inject(CategoryService);
  protected commonDataService = inject(CommonDataService);
  private authService = inject(AuthService);

  name = signal("");
  description = signal("");
  isAvailable = signal(true);
  address = signal("");
  price = signal<number | undefined>(undefined);
  deposit = signal<number | undefined>(undefined);
  stock = signal<number | undefined>(undefined);
  rentalType = signal<RentalType>(RentalType.Unknown);
  itemCondition = signal<ItemCondition>(ItemCondition.Unknown);
  subCategoryId = signal(0);
  ownerId = signal(this.authService.userId);
  geoPostalId = signal(0);

  categories = signal<Category[]>([]);
  selectedCategory = signal<Category | undefined>(undefined);
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



  ngOnInit(): void {
    this.loadCountries();
    this.loadCategories();
    this.rentalTypes.set(this.commonDataService.getRentalTypeList());
    this.itemConditions.set(this.commonDataService.getItemConditionList());
  }
  loadCategories() {
    this.categoryService.get().subscribe({
      next: data => {
        this.categories.set(data);
      },
      error: error => {
        alert("Laden der Kategorien fehlgeschlagen: " + error.message);
      }
    });
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

  onSubmit(form: NgForm) {

  }

}
