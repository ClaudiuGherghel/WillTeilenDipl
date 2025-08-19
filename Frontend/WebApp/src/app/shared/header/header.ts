import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SearchBar } from '../search-bar/search-bar';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, SearchBar],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  userId = signal<number | undefined>(undefined);


}
