import { Component, inject, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { switchMap } from 'rxjs';
import { PostalCodeAndPlaceDto } from '../../../../dtos/postal-code-and-place-dto';
import { ItemCondition } from '../../../../enums/item-condition';
import { RentalType } from '../../../../enums/rental-type';
import { Category } from '../../../../models/category.model';
import { ItemPostDto } from '../../../../models/item.model';
import { AuthService } from '../../../../services/auth-service';
import { CategoryService } from '../../../../services/category-service';
import { CommonDataService } from '../../../../services/common-data-service';
import { GeoPostalService } from '../../../../services/geo-postal-service';
import { ItemService } from '../../../../services/item-service';

@Component({
  selector: 'app-edit-item',
  imports: [FormsModule],
  templateUrl: './edit-item.html',
  styleUrl: './edit-item.css'
})
export class EditItem {

  // private router = inject(Router);
  // private route = inject(ActivatedRoute);
  private itemService = inject(ItemService);
  private geoPostalService = inject(GeoPostalService);
  private categoryService = inject(CategoryService);
  protected commonDataService = inject(CommonDataService);
  private authService = inject(AuthService);

  // itemId = 0;

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
    // if (this.router.url.indexOf('edit') >= 0) {
    //   this.route.params.subscribe(params => {
    //     if (params['itemId']) {
    //       this.itemId = params['itemId'];
    //       this.loadItem();
    //     }
    //   });
    // }

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

  // loadItem() {
  //   this.itemService.getById(this.itemId).subscribe({
  //     next: data => {
  //       this.fillFields(data);
  //     },
  //     error: error => {
  //       alert("Laden des Items fehlgeschlagen: " + error.message);
  //     }
  //   });
  // }

  // fillFields(data: Item) {
  //   this.name.set(data.name);
  //   this.description.set(data.description);
  //   this.isAvailable.set(data.isAvailable);
  //   this.address = signal(data.address);
  //   this.price.set(data.price);
  //   this.deposit.set(data.deposit);
  //   this.stock.set(data.stock);
  //   this.subCategoryId.set(data.subCategoryId);
  //   this.ownerId = signal(data.ownerId); //weil ownerId auch null sein kann funktioniert set nicht

  //   // this.selectedCategory.set() // item hat keine CategoryProperty
  //   this.selectedCountry.set(data.geoPostal.country);
  //   this.selectedState.set(data.geoPostal.state);
  //   // this.postalCodesAndPlaces = signal<PostalCodeAndPlaceDto[]>([]);
  //   this.selectedPostalCode.set(data.geoPostal.postalCode);
  //   this.selectedPlace.set(data.geoPostal.place);

  //   this.selectedRentalType.set(data.rentalType);
  //   this.selectedItemCondition.set(data.itemCondition);
  // }


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
    if (!form.valid) return;

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
          return this.itemService.post(newItem);
        })
      )
      .subscribe({
        next: () => {
          alert("Item hinzugefügt");
          this.clearFields();
        },
        error: error => {
          alert("Fehler: " + error.message);
        }
      });
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
  }

}