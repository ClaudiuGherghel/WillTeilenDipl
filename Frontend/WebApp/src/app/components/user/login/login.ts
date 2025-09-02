import { Component, inject, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
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
  isTriedToSave = signal(false);

  username = signal('');
  password = signal('');


  login(form: NgForm) {
    this.isTriedToSave.set(true);

    if (form.valid) {
      this.authService.login(this.username(), this.password()).subscribe({
        next: data => {
          this.router.navigateByUrl('/user');
        },
        error: error => {
          alert("Anmeldung fehlgeschlagen" + error.message);
        }
      });
      this.isTriedToSave.set(false);
    }
  }
}
