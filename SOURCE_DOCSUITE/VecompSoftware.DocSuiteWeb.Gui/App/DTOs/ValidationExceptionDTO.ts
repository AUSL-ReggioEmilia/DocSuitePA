import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');

class ValidationExceptionDTO extends ExceptionDTO {

    constructor() {
        super();
        this.validationMessages = [];        
    }

    validationMessages: ValidationMessageDTO[];
}

export = ValidationExceptionDTO;