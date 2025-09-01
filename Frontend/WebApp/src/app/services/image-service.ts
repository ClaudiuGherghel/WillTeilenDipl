import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Image, ImagePutDto } from '../models/image.model';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  private apiUrl = "https://localhost:7267/api/images";
  private http = inject(HttpClient);

  getImagesByItem(itemId: number) {
    return this.http.get<Image[]>(`${this.apiUrl}/getbyitemid/${itemId}`);
  }

  post(itemId: number, file: File) {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('ItemId', itemId.toString());
    formData.append('AltText', file.name); // default: Dateiname als AltText

    return this.http.post<Image>(`${this.apiUrl}/postbyuser`, formData);
  }

  updateImage(image: ImagePutDto) {
    return this.http.put<Image>(`${this.apiUrl}/putbyuser/${image.id}`, image);
  }

  deleteImage(id: number) {
    return this.http.delete<void>(`${this.apiUrl}/deletebyuser/${id}`);
  }

}
