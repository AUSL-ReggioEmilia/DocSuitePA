define(["require", "exports"], function (require, exports) {
    var ValidationCode;
    (function (ValidationCode) {
        ValidationCode[ValidationCode["Anomaly"] = 3000] = "Anomaly";
        ValidationCode[ValidationCode["RulesetValidation"] = 3001] = "RulesetValidation";
        ValidationCode[ValidationCode["Mapper"] = 3002] = "Mapper";
        ValidationCode[ValidationCode["CustomValidatorObject"] = 3003] = "CustomValidatorObject";
        ValidationCode[ValidationCode["CustomValidatorRelation"] = 3004] = "CustomValidatorRelation";
    })(ValidationCode || (ValidationCode = {}));
    return ValidationCode;
});
//# sourceMappingURL=ValidationCode.js.map