import { Component, ElementRef, inject, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-bar',
  imports: [],
  templateUrl: './search-bar.html',
  styleUrl: './search-bar.css'
})
export class SearchBar {

  @ViewChild('search') searchInput!: ElementRef<HTMLInputElement>;
  router = inject(Router);

  onSearch(event: Event) {
    const input = event.target as HTMLInputElement;
    const query = input.value.trim();
    this.router.navigateByUrl('/item-search?query=' + query);
  }

  clearSearch() {
    if (this.searchInput) {
      this.searchInput.nativeElement.value = '';
    }
  }

}
