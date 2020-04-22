import { Directive } from '@angular/core';
import { NG_VALIDATORS, Validator, FormControl, ValidationErrors } from '@angular/forms';

@Directive({
    selector: '[emailValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: EmailValidatorDirective, multi: true }]
})
export class EmailValidatorDirective implements Validator {

    constructor() { }

    validate(control: FormControl): ValidationErrors | null {
        if (!control.value) {
            return null;
        }

        const isValidEmail = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(control.value);
        const message = {
            'email': {
                'message': 'Inserire un indirizzo mail valido'
            }
        };
        return isValidEmail ? null : message;
    }
}