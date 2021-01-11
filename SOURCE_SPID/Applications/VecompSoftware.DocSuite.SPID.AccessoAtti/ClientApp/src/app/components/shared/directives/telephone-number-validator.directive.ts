import { Directive } from '@angular/core';
import { NG_VALIDATORS, Validator, FormControl, ValidationErrors } from '@angular/forms';

@Directive({
    selector: '[telephoneNumberValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: TelephoneNumberValidatorDirective, multi: true }]
})
export class TelephoneNumberValidatorDirective implements Validator {

    constructor() { }

    validate(control: FormControl): ValidationErrors|null {
        const isValidPhoneNumber = /^[0|3]{1}[0-9]{7,10}$/.test(control.value);
        const message = {
            'telephoneNumber': {
                'message': 'Inserire un numero di telefono valido'
            }
        };
        return isValidPhoneNumber ? null : message;
    }
}
