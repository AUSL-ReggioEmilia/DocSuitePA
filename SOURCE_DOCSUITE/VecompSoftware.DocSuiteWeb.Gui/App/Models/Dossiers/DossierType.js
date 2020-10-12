define(["require", "exports"], function (require, exports) {
    var DossierType;
    (function (DossierType) {
        DossierType[DossierType["Person"] = 0] = "Person";
        DossierType[DossierType["PhysicalObject"] = 1] = "PhysicalObject";
        DossierType[DossierType["Procedure"] = 2] = "Procedure";
        DossierType[DossierType["Process"] = 3] = "Process";
    })(DossierType || (DossierType = {}));
    return DossierType;
});
//# sourceMappingURL=DossierType.js.map