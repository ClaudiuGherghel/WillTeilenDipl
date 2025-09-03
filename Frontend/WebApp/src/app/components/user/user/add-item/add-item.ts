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
import { Item, ItemPostDto } from '../../../../models/item.model';
import { ItemService } from '../../../../services/item-service';
import { switchMap } from 'rxjs/internal/operators/switchMap';
import { extractErrorMessage } from '../../../../utils/error';

@Component({
  selector: 'app-add-item',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-item.html',
  styleUrl: './add-item.css'
})
export class AddItem implements OnInit {

  private itemService = inject(ItemService);
  private geoPostalService = inject(GeoPostalService);
  private categoryService = inject(CategoryService);
  protected commonDataService = inject(CommonDataService);
  private authService = inject(AuthService);

  isTriedToSave = signal(false);

  name = signal("");
  description = signal("");
  isAvailable = signal(true);
  address = signal("");
  price = signal<number | undefined>(undefined);
  deposit = signal<number | undefined>(undefined);
  stock = signal<number | undefined>(undefined);
  subCategoryId = signal(0);
  ownerId = this.authService.userId;


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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
      }
    });
  }

  loadCountries() {
    this.geoPostalService.getCountries().subscribe({
      next: data => {
        this.countries.set(data);
      },
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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
    this.isTriedToSave.set(true);

    if (form.valid) {
      this.geoPostalService
        .getByQuery(this.selectedCountry(), this.selectedState(), this.selectedPostalCode(), this.selectedPlace())
        .pipe(
          switchMap(data => {
            const newItem: ItemPostDto = {
              name: this.name(),
              description: this.description(),
              isAvailable: this.isAvailable(),
              address: this.address(),
              price: this.price() ?? 0,
              deposit: this.deposit() ?? 0,
              stock: this.stock() ?? 0,
              rentalType: RentalType[this.selectedRentalType()],
              itemCondition: ItemCondition[this.selectedItemCondition()],
              subCategoryId: this.subCategoryId(),
              ownerId: this.ownerId() ?? 0,
              geoPostalId: data.id,
            };
            console.log(newItem);
            return this.itemService.postByUser(newItem);
          })
        )
        .subscribe({
          next: () => {
            alert("Item hinzugefügt");
            this.clearFields();
          },
          error: (err) => {
            const message = extractErrorMessage(err);
            alert(message);
          }
        });
      this.isTriedToSave.set(false);
    }
  }


  clearFields() {
    this.name = signal("");
    this.description = signal("");
    this.isAvailable = signal(true);
    this.address = signal("");
    this.price = signal<number | undefined>(undefined);
    this.deposit = signal<number | undefined>(undefined);
    this.stock = signal<number | undefined>(undefined);
    this.subCategoryId = signal(0);
    this.ownerId = this.authService.userId;

    this.selectedCategory = signal<Category | undefined>(undefined);
    this.selectedCountry = signal('');
    this.selectedState = signal('');
    this.postalCodesAndPlaces = signal<PostalCodeAndPlaceDto[]>([]);
    this.selectedPostalCode = signal('');
    this.selectedPlace = signal('');

    this.selectedRentalType = signal<RentalType>(RentalType.Unknown);
    this.selectedItemCondition = signal<ItemCondition>(ItemCondition.Unknown);
    this.isTriedToSave.set(false);
  }

}