import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  private authService = inject(AuthService);
  private router = inject(Router);
  userId = this.authService.userId;

  username = signal('');
  password = signal('');

  login() {
    this.authService.login(this.username(), this.password()).subscribe({
      next: data => {
        alert('Login successful');
        this.router.navigateByUrl('/user');
      },
      error: error => {
        alert("Invalid user or password");
      }
    });
  }
}
