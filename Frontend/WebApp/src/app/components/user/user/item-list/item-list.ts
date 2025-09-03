import { Component, inject, OnInit, signal } from '@angular/core';
import { Item, ItemPutDto } from '../../../../models/item.model';
import { ItemService } from '../../../../services/item-service';
import { AuthService } from '../../../../services/auth-service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RentalType } from '../../../../enums/rental-type';
import { ItemCondition } from '../../../../enums/item-condition';
import { ImageService } from '../../../../services/image-service';
import { MainImageDto } from '../../../../dtos/main-image-dto';
import { extractErrorMessage } from '../../../../utils/error';



@Component({
  selector: 'app-item-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './item-list.html',
  styleUrl: './item-list.css'
})

export class ItemList implements OnInit {


  authService = inject(AuthService);
  itemService = inject(ItemService);
  imageService = inject(ImageService);

  userId = this.authService.userId;

  items = signal<Item[]>([]);
  mainImages = signal<MainImageDto[]>([]);

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems() {
    if (this.userId() != null) {
      this.itemService.getByUser(this.userId()!).subscribe({
        next: data => {
          this.items.set(data);

          const imgs: MainImageDto[] = [];

          for (let i = 0; i < data.length; i++) {
            const mainImg = data[i].images.find(img => img.isMainImage);
            if (mainImg) {
              imgs.push({
                itemId: data[i].id,
                imageUrl: mainImg.imageUrl
              });
            }
          }
          this.mainImages.set(imgs);
        },
        error: (err) => {
          const message = extractErrorMessage(err);
          alert(message);
        }
      });
    }
  }

  getMainImageUrl(itemId: number): string {
    const entry = this.mainImages().find(img => img.itemId === itemId);
    return entry ? entry.imageUrl : '';
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
      }
    });
  }

}
