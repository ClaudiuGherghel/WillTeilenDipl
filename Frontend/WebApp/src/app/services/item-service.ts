import { HttpHeaders, HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Item } from '../models/item.model';

const httpOptions = {
  headers: new HttpHeaders({
    'Content': 'application/json'
  })
}

@Injectable({
  providedIn: 'root'
})


export class ItemService {

  private apiUrl = "https://localhost:7267/api/items/";
  private http = inject(HttpClient);

  constructor() { }

  // getItemById(id: number) {
  //   return this.http.get<Item>(this.apiUrl + "/get/" + id);
  // }
  getByFilter(filter: string) {
    return this.http.get<Item[]>(this.apiUrl + "getbyfilter?filter=" + filter);
  }

  getById(id: number) {
    return this.http.get<Item>(this.apiUrl + "get/" + id);
  }

}
