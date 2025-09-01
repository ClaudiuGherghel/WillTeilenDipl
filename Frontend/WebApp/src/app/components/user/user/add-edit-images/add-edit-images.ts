import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImageService } from '../../../../services/image-service';
import { ItemService } from '../../../../services/item-service';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { Image, ImagePutDto } from '../../../../models/image.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-edit-images',
  imports: [DragDropModule, FormsModule],
  templateUrl: './add-edit-images.html',
  styleUrl: './add-edit-images.css'
})
export class AddEditImages implements OnInit {



  private route = inject(ActivatedRoute);
  private imageService = inject(ImageService);


  itemId: number = 0;
  images = signal<Image[]>([]);



  ngOnInit(): void {
    this.itemId = +this.route.snapshot.paramMap.get('itemId')!;
    this.loadImages();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const file = input.files[0];

    this.imageService.post(this.itemId, file).subscribe({
      next: data => {
        console.log(data);
        this.loadImages();
      },
      error: error => {
        alert("Upload fehlgeschlagen " + error.message);
      }
    });
  }

  loadImages() {
    this.imageService.getImagesByItem(this.itemId).subscribe({
      next: data => {
        this.images.set(data);
        console.log(data);
      },
      error: error => {
        alert("Laden der Bilder fehlgeschlagen: " + error.message);
      }
    });
  }

  changeMainImage(image: Image) {

    const imagePutDto: ImagePutDto = {
      rowVersion: image.rowVersion,
      altText: image.altText,
      displayOrder: image.displayOrder,
      imageUrl: image.imageUrl,
      id: image.id,
      isMainImage: true,
      itemId: image.itemId
    };

    this.imageService.updateImage(imagePutDto).subscribe({
      next: data => {
        this.loadImages();
      },
      error: error => {
        alert("Update ist fehlgeschlagen: " + error.message);
      }
    });
  }

  deleteImage(image: Image) {
    this.imageService.deleteImage(image.id).subscribe({
      next: data => {
        this.loadImages();
      },
      error: error => {
        alert("Löschen des Bildes fehlgeschlagen:" + error.message);
      }
    });
  }
}