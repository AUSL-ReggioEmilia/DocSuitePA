import { Component, Input } from '@angular/core';
import { AbstractControlDirective, AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-form-show-errors',
  templateUrl: './form-show-errors.component.html'
})
export class FormShowErrorsComponent {

    constructor() { }

    private static readonly errorMessages: { [index: string]: any } = {
        'required': () => 'Il campo &eacute; obbligatorio',
        'telephoneNumber': (params: any) => params.message,
        'email': (params: any) => params.message
    };

    @Input()
    private control: AbstractControlDirective | AbstractControl;

    showErrors(): boolean|null {
        return this.control &&
            this.control.errors &&
            (this.control.dirty || this.control.touched);
    }

    errors(): string[] {
        if (this.control.errors) {
            return Object.keys(this.control.errors)
                .map((field: string) => this.getMessage(field, this.control.errors![field]));
        }
        return [];
    }

    private getMessage(messageType: string, params: any): string {
        return FormShowErrorsComponent.errorMessages[messageType](params);
    }

}
