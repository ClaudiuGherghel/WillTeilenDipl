import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';

@Directive({
  selector: '[appDateNotInFuture]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: DateNotInFutureDirective,
      multi: true
    }
  ]
})
export class DateNotInFutureDirective implements Validator {


  validate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null; // Leeres Feld ist hier erlaubt
    }

    const inputDate = new Date(control.value);
    const today = new Date();

    // Uhrzeit auf 00:00 setzen, damit nur Datum verglichen wird
    inputDate.setHours(0, 0, 0, 0);
    today.setHours(0, 0, 0, 0);

    if (inputDate > today) {
      return { appDateNotInFuture: true };
    }

    return null;
  }
}
