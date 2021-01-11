import ValidationModel = require('App/Models/Validations/ValidationModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageModel = require('App/Models/Validations/ValidationMessageModel');
import ExceptionStatusCode = require('App/DTOs/ExceptionStatusCode');
import ValidationCode = require('App/Models/Validations/ValidationCode');

declare var JSONHelper: any;
class ErrorHelper {
    public static DEFAULT_ERROR_MESSAGE = "Anomalia critica nell'esecuzione della richiesta. Contattare l'assistenza.";
    public static DEFAULT_UNAUTHORIZED_MESSAGE = "Non si è autorizzati ad effetturare la richiesta. Verificare di avere i permessi necessari.";

    getExceptionModel(XMLHttpRequest: JQueryXHR): ExceptionDTO {
        if (XMLHttpRequest) {
            let exceptionDto: ExceptionDTO = new ExceptionDTO();
            exceptionDto.statusCode = XMLHttpRequest.status;
            exceptionDto.statusText = XMLHttpRequest.statusText;

            switch (exceptionDto.statusCode) {
                case ExceptionStatusCode.BadRequest: {

                    exceptionDto.statusText = ErrorHelper.DEFAULT_ERROR_MESSAGE;
                    if (XMLHttpRequest.responseJSON) {
                        let jsonHelper: any = new JSONHelper();
                        let validation: ValidationModel = <ValidationModel>jsonHelper.resolveReferences(XMLHttpRequest.responseJSON);

                        if (validation && validation.ValidationCode && validation.ValidationCode in ValidationCode && validation.ValidationMessages.length > 0) {
                            let validationExceptionDto = new ValidationExceptionDTO();
                            validationExceptionDto.statusCode = XMLHttpRequest.status;
                            validationExceptionDto.statusText = "Alcuni dati non sono corretti, per maggiori informazioni scorrere i messaggi (".concat(validation.ValidationMessages.length.toString(), ").");
                            let validationMessage: ValidationMessageDTO;
                            $.each(validation.ValidationMessages, (index: number, message: ValidationMessageModel) => {
                                validationMessage = new ValidationMessageDTO();
                                validationMessage.message = message.Message;
                                validationMessage.messageCode = message.MessageCode;
                                validationExceptionDto.validationMessages.push(validationMessage);
                            });
                            return validationExceptionDto;
                        }
                    }                    
                    break;
                }
                case ExceptionStatusCode.Unauthorized: {
                    exceptionDto.statusText = ErrorHelper.DEFAULT_UNAUTHORIZED_MESSAGE;
                    break;
                }
                case ExceptionStatusCode.InternalServerError: {
                    exceptionDto.statusText = ErrorHelper.DEFAULT_ERROR_MESSAGE;
                    break;
                }
                default: {
                    exceptionDto.statusText = ErrorHelper.DEFAULT_ERROR_MESSAGE;
                }
            }

            return exceptionDto;
        }
    }
}

export = ErrorHelper;