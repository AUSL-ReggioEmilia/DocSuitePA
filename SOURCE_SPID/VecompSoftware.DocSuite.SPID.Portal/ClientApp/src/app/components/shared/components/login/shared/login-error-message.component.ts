import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'app-login-error-message',
    templateUrl: './login-error-message.component.html'
})
export class LoginErrorMessageComponent implements OnInit {

    @Input()
    hasError: boolean;

    constructor() { }

    ngOnInit() {
    }

}
