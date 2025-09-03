import { Component, inject, OnInit, signal } from '@angular/core';
import { switchMap } from 'rxjs';
import { of } from 'rxjs';
import { PostalCodeAndPlaceDto } from '../../../../dtos/postal-code-and-place-dto';
import { AuthService } from '../../../../services/auth-service';
import { GeoPostalService } from '../../../../services/geo-postal-service';
import { FormsModule, NgForm } from '@angular/forms';
import { UserService } from '../../../../services/user-service';
import { User, UserPutDo } from '../../../../models/user.model';
import { DateNotInFutureDirective } from "../../../../directives/date-not-in-future";
import { extractErrorMessage } from '../../../../utils/error';

@Component({
  selector: 'app-profile',
  imports: [FormsModule, DateNotInFutureDirective],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {

  private authService = inject(AuthService);
  private userService = inject(UserService);
  private geoPostalService = inject(GeoPostalService);

  isTriedToSave = signal(false);
  formIsValid = signal(true);

  userId = this.authService.userId;
  rowVersion: any = "";
  userName = signal('');
  currentPassword = signal('');
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
      });
    }
    this.loadCountries();
  }




  fillFields(data: User) {

    this.rowVersion = data.rowVersion;
    this.userName.set(data.userName);
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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
      error: (err) => {
        const message = extractErrorMessage(err);
        alert(message);
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

  save(form: NgForm) {
    this.isTriedToSave.set(true);

    if (form.valid) {

      if (this.newPassword() != "" && this.currentPassword() == "") {
        this.formIsValid.set(false);
        return;
      }

      const pwChange$ = this.newPassword()
        ? this.userService.changePw(this.userId()!, {
          id: this.userId()!,
          rowVersion: this.rowVersion,
          currentPassword: this.currentPassword(),
          newPassword: this.newPassword()
        }).pipe(
          switchMap(data => {
            this.currentPassword.set(this.newPassword());
            this.rowVersion = data.rowVersion;

            // Passwort erfolgreich geändert → Alert
            alert("Passwort erfolgreich geändert!");

            return of(true); // Weiter mit der nächsten Operation
          })
        )
        : of(true); // kein Passwortwechsel, einfach weiter

      pwChange$.pipe(
        switchMap(() => this.geoPostalService.getByQuery(
          this.selectedCountry(),
          this.selectedState(),
          this.selectedPostalCode(),
          this.selectedPlace()
        )),
        switchMap(geoPostalData => {
          const updatedUser: UserPutDo = {
            id: this.userId()!,
            rowVersion: this.rowVersion,
            userName: this.userName(),
            email: this.email(),
            firstName: this.firstName(),
            lastName: this.lastName(),
            birthDate: new Date(this.birthDate()).toISOString(),
            geoPostalId: geoPostalData.id,
            address: this.address(),
            phoneNumber: this.phoneNumber()
          };
          return this.userService.put(this.userId()!, updatedUser);
        }),
        switchMap(() => this.userService.get(this.userId()!)) // User neu laden
      ).subscribe({
        next: data => {
          this.fillFields(data); // Formular wieder befüllen
          alert("Daten wurden geändert");
        },
        error: (err) => {
          const message = extractErrorMessage(err);
          alert(message);
        }
      });

      this.isTriedToSave.set(false);
      this.formIsValid.set(true);
    }
  }
  // alert("Fehler beim Speichern: " + error.message)

}