import { Component, inject, OnChanges, OnDestroy, OnInit, signal, SimpleChanges } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { switchMap } from 'rxjs';
import { PostalCodeAndPlaceDto } from '../../../../dtos/postal-code-and-place-dto';
import { ItemCondition } from '../../../../enums/item-condition';
import { RentalType } from '../../../../enums/rental-type';
import { Category } from '../../../../models/category.model';
import { Item, ItemPutDto } from '../../../../models/item.model';
import { AuthService } from '../../../../services/auth-service';
import { CategoryService } from '../../../../services/category-service';
import { CommonDataService } from '../../../../services/common-data-service';
import { GeoPostalService } from '../../../../services/geo-postal-service';
import { ItemService } from '../../../../services/item-service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-edit-item',
  imports: [FormsModule],
  templateUrl: './edit-item.html',
  styleUrl: './edit-item.css'
})
export class EditItem implements OnInit {

  private route = inject(ActivatedRoute);
  private itemService = inject(ItemService);
  private geoPostalService = inject(GeoPostalService);
  private categoryService = inject(CategoryService);
  protected commonDataService = inject(CommonDataService);
  private authService = inject(AuthService);

  isTriedToSave = signal(false);

  itemId = 0;
  rowVersion = signal<any>("");
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
    this.route.params.subscribe(params => {
      if (params['itemId']) {
        this.itemId = params['itemId'];
        this.loadItem();
      }
    });


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

  loadItem() {
    this.itemService.getById(this.itemId).subscribe({
      next: data => {
        this.fillFields(data);
      },
      error: error => {
        alert("Laden des Items fehlgeschlagen: " + error.message);
      }
    });
  }


  fillFields(data: Item) {

    this.rowVersion.set(data.rowVersion);
    this.name.set(data.name);
    this.description.set(data.description);
    this.isAvailable.set(data.isAvailable);
    this.address.set(data.address);
    this.price.set(data.price);
    this.deposit.set(data.deposit);
    this.stock.set(data.stock);
    this.subCategoryId.set(data.subCategoryId);
    this.ownerId = signal(data.ownerId);

    // Kategorie/Subkategorie laden + setzen 
    this.categoryService.get().subscribe({
      next: cats => {
        this.categories.set(cats);
        const subCat = cats.flatMap(c => c.subCategories).find(sc => sc.id === data.subCategoryId);
        if (subCat) {
          this.selectedCategory.set(cats.find(c => c.subCategories.some(sc => sc.id === subCat.id)));
          this.subCategoryId.set(subCat.id);
        }
      }
    });

    // GeoPostal laden + setzen 
    this.geoPostalService.getCountries().subscribe({
      next: countries => {
        this.countries.set(countries);
        this.selectedCountry.set(data.geoPostal.country);

        this.geoPostalService.getStates(data.geoPostal.country).subscribe({
          next: states => {
            this.states.set(states);
            this.selectedState.set(data.geoPostal.state);

            this.geoPostalService.getPostlCodesAndPlaces(data.geoPostal.state).subscribe({
              next: postalData => {
                this.postalCodesAndPlaces.set(postalData);
                this.selectedPostalCode.set(data.geoPostal.postalCode);
                this.selectedPlace.set(data.geoPostal.place);
              }
            });
          }
        });
      }
    });

    // --- RentalType & ItemCondition ---
    this.rentalTypes.set(this.commonDataService.getRentalTypeList());
    this.selectedRentalType.set(
      RentalType[data.rentalType as unknown as keyof typeof RentalType]
    );

    this.itemConditions.set(this.commonDataService.getItemConditionList());
    this.selectedItemCondition.set(ItemCondition[data.itemCondition as unknown as keyof typeof ItemCondition]);

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
    this.isTriedToSave.set(true);
    if (form.valid) {
      this.geoPostalService
        .getByQuery(this.selectedCountry(), this.selectedState(), this.selectedPostalCode(), this.selectedPlace())
        .pipe(
          switchMap(data => {
            const newItem: ItemPutDto = {
              id: this.itemId,
              rowVersion: this.rowVersion(),
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
            console.log(this.itemId);
            return this.itemService.putByUser(this.itemId, newItem);
          })
        )
        .subscribe({
          next: () => {
            alert("Item erfolgreich geändert");
          },
          error: error => {
            alert("Fehler: " + error.message);
          }
        });
      this.isTriedToSave.set(false);
    }
  }




}