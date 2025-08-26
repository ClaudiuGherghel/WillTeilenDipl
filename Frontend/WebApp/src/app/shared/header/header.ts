import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SearchBar } from '../search-bar/search-bar';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, SearchBar],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  auth = inject(AuthService);

  // Signale für Template
  userId = this.auth.userId;
  role = this.auth.role;

  logout() {
    this.auth.logout();
  }

}
