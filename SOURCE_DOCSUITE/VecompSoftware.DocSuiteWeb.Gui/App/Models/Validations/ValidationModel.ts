import ValidationMessageModel = require('ValidationMessageModel');
import ValidationCode = require('ValidationCode');

interface ValidationModel {
    ValidationCode: ValidationCode;
    ValidationMessages: ValidationMessageModel[];
}

export = ValidationModel;