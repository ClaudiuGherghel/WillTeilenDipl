import { HttpHeaders, HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Item, ItemPostDto, ItemPutDto } from '../models/item.model';

const httpOptions = {
  headers: new HttpHeaders({
    'Content': 'application/json'
  })
}

@Injectable({
  providedIn: 'root'
})


export class ItemService {
  private apiUrl = "https://localhost:7267/api/items";
  private http = inject(HttpClient);

  constructor() { }

  // getItemById(id: number) {
  //   return this.http.get<Item>(this.apiUrl + "/get/" + id);
  // }

  getByFilter(filter: string) {
    return this.http.get<Item[]>(this.apiUrl + "/getbyfilter?filter=" + filter);
  }

  getById(id: number) {
    return this.http.get<Item>(this.apiUrl + "/get/" + id);
  }

  getByUser(userId: number) {
    return this.http.get<Item[]>(`${this.apiUrl}/getbyuser/${userId}`);
  }

  postByUser(item: ItemPostDto) {
    return this.http.post<Item>(`${this.apiUrl}/postbyuser`, item,)
  }

  putByUser(itemId: number, item: ItemPutDto) {
    return this.http.put<Item>(`${this.apiUrl}/putbyuser/${itemId}`, item);
  }

  delete(itemId: number) {
    return this.http.delete(`${this.apiUrl}/deletebyuser/${itemId}`);
  }



}
