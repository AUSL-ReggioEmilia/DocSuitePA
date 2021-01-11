import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import ExceptionStatusCode = require('App/DTOs/ExceptionStatusCode');

class ExceptionDTO {
    statusCode: ExceptionStatusCode;
    statusText: string;
}

export = ExceptionDTO;