import { Component, inject, OnInit, signal } from '@angular/core';
import { Category } from '../../../models/category.model';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../../services/category-service';

@Component({
  selector: 'app-categories',
  imports: [RouterLink],
  templateUrl: './categories.html',
  styleUrl: './categories.css'
})
export class Categories implements OnInit {

  private categoryService = inject(CategoryService);

  categories = signal<Category[]>([]);

  constructor() { }

  ngOnInit(): void {
    this.loadAllCategories();
  }

  loadAllCategories() {
    this.categoryService.get().subscribe({
      next: data => {
        this.categories.set(data);
      },
      error: error => {
        alert("Laden der Kategorien ist fehlgeschlagen: " + error.message);
      }
    });
  }
}
