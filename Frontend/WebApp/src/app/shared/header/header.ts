import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SearchBar } from '../search-bar/search-bar';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [SearchBar, RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  userId = signal<number | undefined>(undefined);

  constructor(/* private authService: AuthService */) {
    // Beispiel: this.userId.set(this.authService.getUserId());
  }
}
