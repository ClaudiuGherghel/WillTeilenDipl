import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Item } from '../../../models/item.model';
import { ItemService } from '../../../services/item-service';

@Component({
  selector: 'app-item-detail',
  imports: [],
  templateUrl: './item-detail.html',
  styleUrl: './item-detail.css'
})
export class ItemDetail {

  private itemService = inject(ItemService);
  private route = inject(ActivatedRoute);

  item = signal<Item | undefined>(undefined);
  itemId: number = 0;


  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.itemId = (Number)(params.get('itemId'));
      if (this.itemId == 0) {
        alert("Fehler beim Lesen der ItemId");
      }
      this.loadItem();
    });


  }
  loadItem() {
    this.itemService.getById(this.itemId!).subscribe({
      next: data => {
        this.item.set(data);
      },
      error: error => {
        alert("Laden des Items ist fehlgeschlagen: " + error.message);
      }
    });
  }



  // Standard-Bild beim Laden
  mainImage: string = 'trailer.jpg';

  // Liste der Thumbnails (Pfad zu Bildern)
  thumbnails: string[] = [
    'trailer.jpg',
    'anhänger.png',
    'img3',
    'img4',
    'img5',
    'img6',
    'img7',
    'img8',
    'img9',
  ];

  // Aktuell ausgewähltes Bild
  activeThumbnail: string = this.mainImage;

  // Vollbildsteuerung
  isFullscreen: boolean = false;

  // Thumbnail-Klick: Setzt das Hauptbild und markiert das aktive Thumbnail
  onThumbnailClick(thumbnail: string) {
    this.mainImage = thumbnail;
    this.activeThumbnail = thumbnail;
  }

  // Klick auf Hauptbild: Wechselt in den Vollbildmodus
  onMainImageClick() {
    this.isFullscreen = true;
    const modal = document.getElementById('fullscreenModal');
    const fullImage = document.getElementById('fullImage') as HTMLImageElement;
    if (modal && fullImage) {
      modal.style.display = 'block';
      fullImage.src = this.mainImage;
    }
  }

  // Schließen des Vollbildmodus
  closeFullscreen() {
    const modal = document.getElementById('fullscreenModal');
    if (modal) {
      modal.style.display = 'none';
    }
    this.isFullscreen = false;
  }
}
