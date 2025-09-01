import { Component, inject, OnInit, signal } from '@angular/core';
import { switchMap } from 'rxjs';
import { of } from 'rxjs';
import { PostalCodeAndPlaceDto } from '../../../../dtos/postal-code-and-place-dto';
import { RegisterRequest } from '../../../../models/register-request';
import { AuthService } from '../../../../services/auth-service';
import { GeoPostalService } from '../../../../services/geo-postal-service';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../../services/user-service';
import { User, UserChangePwDto, UserPutDo } from '../../../../models/user.model';

@Component({
  selector: 'app-profile',
  imports: [FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {

  private authService = inject(AuthService);
  private userService = inject(UserService);
  private geoPostalService = inject(GeoPostalService);


  userId = this.authService.userId;
  rowVersion: any = "";
  userName = signal('');
  password = signal('');
  newPassword = signal('');
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
    if (this.userId()) {
      this.userService.get(this.userId()!).subscribe({
        next: data => {
          this.fillFields(data);
        }
      })
    }
    this.loadCountries();
  }




  fillFields(data: User) {

    this.rowVersion = data.rowVersion;
    this.userName.set(data.userName);
    this.password.set(data.password);
    this.email.set(data.email);
    this.firstName.set(data.firstName);
    this.lastName.set(data.lastName);
    // convert string to Date
    const birth = new Date(data.birthDate);

    this.birthDate.set(birth.toISOString().substring(0, 10)); // YYYY-MM-DD for <input type="date">
    this.address.set(data.address);
    this.phoneNumber.set(data.phoneNumber);



    // GeoPostal laden + setzen 
    this.geoPostalService.getCountries().subscribe({
      next: countries => {
        this.countries.set(countries);
        this.selectedCountry.set(data.geoPostal.country);

        this.geoPostalService.getStates(data.geoPostal.country).subscribe({
          next: states => {
            this.states.set(states);
            this.selectedState.set(data.geoPostal.state);

            this.geoPostalService.getPostlCodesAndPlaces(data.geoPostal.state).subscribe({
              next: postalData => {
                this.postalCodesAndPlaces.set(postalData);
                this.selectedPostalCode.set(data.geoPostal.postalCode);
                this.selectedPlace.set(data.geoPostal.place);
              }
            });
          }
        });
      }
    });
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


  save() {
    if (!this.userId()) return;

    const pwChange$ = this.newPassword()
      ? this.userService.changePw(this.userId()!, {
        id: this.userId()!,
        rowVersion: this.rowVersion,
        newPassword: this.newPassword()
      }).pipe(
        // aktualisiere rowVersion nach erfolgreicher Änderung
        switchMap(data => {
          this.password.set(data.newPassword);
          this.rowVersion = data.rowVersion;
          return [true]; // dummy Observable um fortzufahren
        })
      )
      : [true]; // wenn kein Passwort, einfach weitermachen

    // jetzt die Aktualisierung der User-Daten
    of(null).pipe(
      switchMap(() => pwChange$), // warten, bis pwChange abgeschlossen ist
      switchMap(() => this.geoPostalService.getByQuery(
        this.selectedCountry(),
        this.selectedState(),
        this.selectedPostalCode(),
        this.selectedPlace()
      )),
      switchMap(data => {
        const updatedUser: UserPutDo = {
          id: this.userId()!,
          rowVersion: this.rowVersion,
          userName: this.userName(),
          email: this.email(),
          firstName: this.firstName(),
          lastName: this.lastName(),
          birthDate: new Date(this.birthDate()).toISOString(),
          geoPostalId: data.id,
          address: this.address(),
          phoneNumber: this.phoneNumber()
        };
        return this.userService.put(this.userId()!, updatedUser);
      })
    ).subscribe({
      next: () => alert("Daten wurden geändert"),
      error: error => alert("Fehler beim speichern: " + error.message)
    });
  }

}
