import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SubCategory } from '../models/sub-category.model';

@Injectable({
  providedIn: 'root'
})
export class SubCategoryService {

  private http = inject(HttpClient);
  private apiUrl = "https://localhost:7267/api/subcategories/";


  constructor() { }

  get(id: number) {
    return this.http.get<SubCategory>(this.apiUrl + "get/" + id);
  }
}
