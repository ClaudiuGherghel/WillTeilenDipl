import { Component, inject, OnInit, signal } from '@angular/core';
import { Item, ItemPutDto } from '../../../../models/item.model';
import { ItemService } from '../../../../services/item-service';
import { AuthService } from '../../../../services/auth-service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RentalType } from '../../../../enums/rental-type';
import { ItemCondition } from '../../../../enums/item-condition';

@Component({
  selector: 'app-item-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './item-list.html',
  styleUrl: './item-list.css'
})
export class ItemList implements OnInit {

  itemService = inject(ItemService);
  authService = inject(AuthService);

  userId = this.authService.userId;

  items = signal<Item[]>([]);


  ngOnInit(): void {
    this.loadItems();
  }

  loadItems() {
    if (this.userId() != null) {
      this.itemService.getByUser(this.userId()!).subscribe({
        next: data => {
          this.items.set(data);
        },
        error: error => {
          alert("Laden der Items fehlgeschlagen: " + error.message);
        }
      });
    }
  }


  onChangeActivation(item: Item) {

    if (item.ownerId != this.userId()) {
      alert("Benuter und Eigentümer des Items sind nicht identisch!");
      return;
    }

    const itemPutDto: ItemPutDto = {
      id: item.id,
      rowVersion: item.rowVersion,
      name: item.name,
      description: item.description,
      address: item.address,
      price: item.price,
      stock: item.stock,
      deposit: item.deposit,
      rentalType: RentalType[item.rentalType],
      itemCondition: ItemCondition[item.itemCondition],
      geoPostalId: item.geoPostalId,
      subCategoryId: item.subCategoryId,
      isAvailable: !item.isAvailable,
      ownerId: item.ownerId
    };

    this.itemService.putByUser(itemPutDto.id, itemPutDto).subscribe({
      next: data => {
        console.log(data);
        this.loadItems();
      },
      error: error => {
        alert("Änderung fehlgeschlagen: " + error.message);
      }
    });
  }


  onDelete(item: Item) {
    if (item.ownerId != this.userId()) {
      alert("Benuter und Eigentümer des Items sind nicht identisch!");
      return;
    }
    this.itemService.delete(item.id).subscribe({
      next: data => {
        console.log(data);
        this.loadItems();
      },
      error: error => {
        alert("Fehler beim Löschen des Items: " + error.message);
      }
    });
  }

}
