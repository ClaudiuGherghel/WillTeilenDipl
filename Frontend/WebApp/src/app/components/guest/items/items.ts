import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SubCategoryService } from '../../../services/sub-category-service';
import { SubCategory } from '../../../models/sub-category.model';
import { SidebarFilter } from '../../../models/sidebar-filter';
import { Item } from '../../../models/item.model';
import { ItemService } from '../../../services/item-service';
import { SideBar } from '../../../shared/side-bar/side-bar';
import { CommonDataService } from '../../../services/common-data-service';

@Component({
  selector: 'app-items',
  imports: [RouterLink, SideBar],
  templateUrl: './items.html',
  styleUrl: './items.css'
})
export class Items implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private subCategoryService = inject(SubCategoryService);
  private itemService = inject(ItemService);
  private commonDataService = inject(CommonDataService);

  isSubCategoryForm = signal(false);

  subCategoryId = 0;
  subCategory = signal<SubCategory | null>(null);

  items = signal<Item[]>([]);
  queryFilter = signal<string>("");

  // For SidebarFilter
  sideBarFilter = signal<SidebarFilter | undefined>(undefined);



  ngOnInit(): void {
    if (this.router.url.indexOf('search') > 0) {
      this.route.queryParams.subscribe(params => {
        if (params['query']) {
          this.queryFilter.set(params['query']);
          this.loadItemsByFilter();
          this.isSubCategoryForm.set(false);
        }
      })
    } else {
      this.isSubCategoryForm.set(true);
      this.route.paramMap.subscribe(params => {
        this.subCategoryId = (Number)(params.get('subCategoryId'));
        if (this.subCategoryId != 0) {
          this.isSubCategoryForm.set(true);
          this.loadSubCategory();
        }
        else {
          alert("Fehler beim Lesen der SubCategoryId");
        }
      });
    }
  }

  loadSubCategory() {
    this.subCategoryService.get(this.subCategoryId).subscribe({
      next: data => {
        this.subCategory.set(data);
      },
      error: error => {
        alert("Laden der Subkategorie ist fehlgeschlagen: " + error.message);
      }
    });
  }

  loadItemsByFilter() {
    this.itemService.getByFilter(this.queryFilter()).subscribe({
      next: data => {
        this.items.set(data);
      },
      error: error => {
        alert("Fehler beim Laden der Items: " + error.message);
      }
    });
  }

  onFilterUseOnItem(item: Item): boolean {

    if (this.sideBarFilter() == undefined) {
      return true;
    }

    if (!(this.sideBarFilter()!.country == '' || item.geoPostal.country.includes(this.sideBarFilter()!.country))) return false;
    if (!(this.sideBarFilter()!.state == '' || item.geoPostal.state.includes(this.sideBarFilter()!.state))) return false;
    if (!(this.sideBarFilter()!.postalCode == '' || item.geoPostal.postalCode.includes(this.sideBarFilter()!.postalCode))) return false;
    if (!(this.sideBarFilter()!.place == '' || item.geoPostal.place.includes(this.sideBarFilter()!.place))) return false;
    if (!(this.sideBarFilter()!.price == 0 || item.price <= this.sideBarFilter()!.price)) return false;
    if (!(this.sideBarFilter()!.deposit == 0 || item.deposit <= this.sideBarFilter()!.deposit)) return false;
    if (!(this.sideBarFilter()!.itemCondition === 0 || item.itemCondition.toString() == this.commonDataService.getItemConditionLabelEnglish(this.sideBarFilter()!.itemCondition))) return false;
    if (!(this.sideBarFilter()!.rentalType === 0 || item.rentalType.toString() == this.commonDataService.getRentalTypeLabelEnglish(this.sideBarFilter()!.rentalType))) return false;



    return true;
  }

  onSideBarFilter(filter: SidebarFilter) {
    this.sideBarFilter.set(filter);
  }


}
