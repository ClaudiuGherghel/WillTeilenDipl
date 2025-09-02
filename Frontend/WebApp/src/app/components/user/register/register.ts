import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth-service';
import { GeoPostalService } from '../../../services/geo-postal-service';
import { PostalCodeAndPlaceDto } from '../../../dtos/postal-code-and-place-dto';
import { RegisterRequest } from '../../../models/register-request';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements OnInit {

  private authService = inject(AuthService);
  private geoPostalService = inject(GeoPostalService);
  private router = inject(Router);


  userName = signal('');
  password = signal('');
  passwordRepeat = signal('');
  email = signal('');
  firstName = signal('');
  lastName = signal('');
  birthDate = signal('');
  address = signal('');
  phoneNumber = signal('');


  countries = signal<string[]>([]);
  selectedCountry = signal('');
  states = signal<string[]>([]);
  selectedState = signal('');
  postalCodesAndPlaces = signal<PostalCodeAndPlaceDto[]>([]);
  selectedPostalCode = signal('');
  selectedPlace = signal('');

  ngOnInit(): void {
    this.loadCountries();
  }



  loadCountries() {
    this.geoPostalService.getCountries().subscribe({
      next: data => {
        this.countries.set(data);
      },
      error: error => {
        alert("Laden der Länder fehlgeschlagen: " + error.message);
      }
    });
  }

  onCountryChanged() {
    this.geoPostalService.getStates(this.selectedCountry()).subscribe({
      next: data => {
        this.states.set(data);
        this.selectedState.set('');
        this.selectedPostalCode.set('');
        this.selectedPlace.set('');
      },
      error: err => {
        alert("Laden der Bundesländer fehlgeschlagen: " + err.message);
      }
    });
  }

  onStateChanged() {
    this.geoPostalService.getPostlCodesAndPlaces(this.selectedState()).subscribe({
      next: data => {
        this.postalCodesAndPlaces.set(data);
        this.selectedPostalCode.set('');
        this.selectedPlace.set('');
      },
      error: err => {
        alert("Laden der Postleitzahlen/Orte fehlgeschlagen: " + err.message);
      }
    });
  }

  onPostalCodeChanged() {
    const match = this.postalCodesAndPlaces().find(p => p.postalCode === this.selectedPostalCode());
    if (match) {
      this.selectedPlace.set(match.place);
    }
  }

  onPlaceChanged() {
    const match = this.postalCodesAndPlaces().find(p => p.place === this.selectedPlace());
    if (match) {
      this.selectedPostalCode.set(match.postalCode);
    }
  }


  register(form: NgForm) {

    if (!form || this.password() != this.passwordRepeat()) {
      alert("Formular ist ungültig!");
      return;
    }

    this.geoPostalService.getByQuery(this.selectedCountry(), this.selectedState(), this.selectedPostalCode(), this.selectedPlace())
      .pipe(
        switchMap(data => {
          const newUser: RegisterRequest = {
            role: "User",
            userName: this.userName(),
            password: this.password(),
            email: this.email(),
            firstName: this.firstName(),
            lastName: this.lastName(),
            birthDate: new Date(this.birthDate()).toISOString(),
            geoPostalId: data.id,
            address: this.address(),
            phoneNumber: this.phoneNumber()
          };
          console.log(newUser);
          return this.authService.register(newUser);
        })
      )
      .subscribe({
        next: (data) => {
          alert('Registration successful');
          this.router.navigate(['/categories']);
        },
        error: error => {
          alert('Registration failed' + error.message);
        }
      });
  }
}
