define(["require", "exports"], function (require, exports) {
    var AuthorizationRoleType;
    (function (AuthorizationRoleType) {
        AuthorizationRoleType[AuthorizationRoleType["Responsible"] = 0] = "Responsible";
        AuthorizationRoleType[AuthorizationRoleType["Accounted"] = 1] = "Accounted";
        AuthorizationRoleType[AuthorizationRoleType["Consulted"] = 2] = "Consulted";
        AuthorizationRoleType[AuthorizationRoleType["Informed"] = 3] = "Informed";
    })(AuthorizationRoleType || (AuthorizationRoleType = {}));
    return AuthorizationRoleType;
});
//# sourceMappingURL=AuthorizationRoleType.js.map