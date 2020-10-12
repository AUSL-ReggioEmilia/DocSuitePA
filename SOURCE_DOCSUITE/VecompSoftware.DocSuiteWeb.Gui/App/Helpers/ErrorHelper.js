define(["require", "exports", "App/DTOs/ExceptionDTO", "App/DTOs/ValidationMessageDTO", "App/DTOs/ValidationExceptionDTO", "App/DTOs/ExceptionStatusCode", "App/Models/Validations/ValidationCode"], function (require, exports, ExceptionDTO, ValidationMessageDTO, ValidationExceptionDTO, ExceptionStatusCode, ValidationCode) {
    var ErrorHelper = /** @class */ (function () {
        function ErrorHelper() {
        }
        ErrorHelper.prototype.getExceptionModel = function (XMLHttpRequest) {
            if (XMLHttpRequest) {
                var exceptionDto = new ExceptionDTO();
                exceptionDto.statusCode = XMLHttpRequest.status;
                exceptionDto.statusText = XMLHttpRequest.statusText;
                switch (exceptionDto.statusCode) {
                    case ExceptionStatusCode.BadRequest: {
                        exceptionDto.statusText = ErrorHelper.DEFAULT_ERROR_MESSAGE;
                        if (XMLHttpRequest.responseJSON) {
                            var jsonHelper = new JSONHelper();
                            var validation = jsonHelper.resolveReferences(XMLHttpRequest.responseJSON);
                            if (validation && validation.ValidationCode && validation.ValidationCode in ValidationCode && validation.ValidationMessages.length > 0) {
                                var validationExceptionDto_1 = new ValidationExceptionDTO();
                                validationExceptionDto_1.statusCode = XMLHttpRequest.status;
                                validationExceptionDto_1.statusText = "Alcuni dati non sono corretti, per maggiori informazioni scorrere i messaggi (".concat(validation.ValidationMessages.length.toString(), ").");
                                var validationMessage_1;
                                $.each(validation.ValidationMessages, function (index, message) {
                                    validationMessage_1 = new ValidationMessageDTO();
                                    validationMessage_1.message = message.Message;
                                    validationMessage_1.messageCode = message.MessageCode;
                                    validationExceptionDto_1.validationMessages.push(validationMessage_1);
                                });
                                return validationExceptionDto_1;
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
        };
        ErrorHelper.DEFAULT_ERROR_MESSAGE = "Anomalia critica nell'esecuzione della richiesta. Contattare l'assistenza.";
        ErrorHelper.DEFAULT_UNAUTHORIZED_MESSAGE = "Non si è autorizzati ad effetturare la richiesta. Verificare di avere i permessi necessari.";
        return ErrorHelper;
    }());
    return ErrorHelper;
});
//# sourceMappingURL=ErrorHelper.js.map