define(["require", "exports"], function (require, exports) {
    var UscRoleRestEventType;
    (function (UscRoleRestEventType) {
        UscRoleRestEventType[UscRoleRestEventType["NewRolesAdded"] = 0] = "NewRolesAdded";
        UscRoleRestEventType[UscRoleRestEventType["RoleDeleted"] = 1] = "RoleDeleted";
        UscRoleRestEventType[UscRoleRestEventType["AllRolesAdded"] = 2] = "AllRolesAdded";
        UscRoleRestEventType[UscRoleRestEventType["AllRolesDeleted"] = 3] = "AllRolesDeleted";
        UscRoleRestEventType[UscRoleRestEventType["SetFascicleVisibilityType"] = 4] = "SetFascicleVisibilityType";
    })(UscRoleRestEventType || (UscRoleRestEventType = {}));
    return UscRoleRestEventType;
});
//# sourceMappingURL=UscRoleRestEventType.js.map