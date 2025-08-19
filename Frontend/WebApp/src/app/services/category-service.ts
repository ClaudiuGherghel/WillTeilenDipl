import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Category } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private apiUrl = "https://localhost:7267/api/categories/";
  private http = inject(HttpClient);

  constructor() { }

  get() {
    return this.http.get<Category[]>(this.apiUrl + "get");
  }

}


