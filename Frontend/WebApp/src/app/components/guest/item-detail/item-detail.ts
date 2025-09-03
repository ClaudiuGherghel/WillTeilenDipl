import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Item } from '../../../models/item.model';
import { ItemService } from '../../../services/item-service';
import { CommonDataService } from '../../../services/common-data-service';
import { extractErrorMessage } from '../../../utils/error';

@Component({
  selector: 'app-item-detail',
  imports: [],
  templateUrl: './item-detail.html',
  styleUrl: './item-detail.css'
})
export class ItemDetail {

  private itemService = inject(ItemService);
  private route = inject(ActivatedRoute);
  protected commonDataService = inject(CommonDataService);

  item = signal<Item | undefined>(undefined);
  itemId: number = 0;

  // Bilder
  mainImage = signal('');
  activeThumbnail = signal('');

  // Vollbildsteuerung
  isFullscreen: boolean = false;

  // Labels
  rentalType: string = '';
  itemCondition: string = '';

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.itemId = Number(params.get('itemId'));
      if (this.itemId === 0) {
        alert('Fehler beim Lesen der ItemId');
      }
      this.loadItem();
    });
  }

  loadItem() {
    this.itemService.getById(this.itemId!).subscribe({
      next: data => {
        this.item.set(data);

        // Hauptbild setzen
        const mainImg = data.images?.find(i => i.isMainImage) ?? data.images?.[0];
        if (mainImg) {
          this.onThumbnailClick(mainImg.imageUrl);
        }

        // Strings -> Enums -> Labels
        const rentalEnum = this.commonDataService.parseRentalTypeFromString(data.rentalType);
        const conditionEnum = this.commonDataService.parseItemConditionFromString(data.itemCondition);

        this.rentalType = this.commonDataService.getRentalTypeLabelGerman(rentalEnum);
        this.itemCondition = this.commonDataService.getItemConditionLabelGerman(conditionEnum);

        console.log(data);
      },
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
      }
    });
  }


  // Thumbnail-Klick
  onThumbnailClick(thumbnail: string) {
    this.mainImage.set(thumbnail);
    this.activeThumbnail.set(thumbnail);
  }

  // Klick auf Hauptbild → Vollbild öffnen
  onMainImageClick() {
    this.isFullscreen = true;
    const modal = document.getElementById('fullscreenModal');
    const fullImage = document.getElementById('fullImage') as HTMLImageElement;
    if (modal && fullImage) {
      modal.classList.add('show');
      fullImage.src = this.mainImage();
    }

    // Keyboard Events aktivieren
    document.addEventListener('keydown', this.handleKeydown);
  }

  // Schließen
  closeFullscreen() {
    const modal = document.getElementById('fullscreenModal');
    if (modal) {
      modal.classList.remove('show');
    }
    this.isFullscreen = false;

    // Keyboard Events entfernen
    document.removeEventListener('keydown', this.handleKeydown);
  }

  // Navigation
  nextImage() {
    const images = this.item()?.images ?? [];
    const index = images.findIndex(img => img.imageUrl === this.mainImage());
    const nextIndex = (index + 1) % images.length;
    this.onThumbnailClick(images[nextIndex].imageUrl);
    this.updateFullscreenImage();
  }

  prevImage() {
    const images = this.item()?.images ?? [];
    const index = images.findIndex(img => img.imageUrl === this.mainImage());
    const prevIndex = (index - 1 + images.length) % images.length;
    this.onThumbnailClick(images[prevIndex].imageUrl);
    this.updateFullscreenImage();
  }

  // Vollbild Bild aktualisieren
  private updateFullscreenImage() {
    const fullImage = document.getElementById('fullImage') as HTMLImageElement;
    if (fullImage) {
      fullImage.src = this.mainImage();
    }
  }

  // Keyboard-Handler
  private handleKeydown = (event: KeyboardEvent) => {
    if (event.key === 'ArrowRight') {
      this.nextImage();
    } else if (event.key === 'ArrowLeft') {
      this.prevImage();
    } else if (event.key === 'Escape') {
      this.closeFullscreen();
    }
  };

}