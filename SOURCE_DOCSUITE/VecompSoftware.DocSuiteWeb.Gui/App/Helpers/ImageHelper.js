define(["require", "exports"], function (require, exports) {
    var ImageHelper = /** @class */ (function () {
        function ImageHelper() {
        }
        ImageHelper.getContactTypeImageUrl = function (contactType) {
            switch (contactType) {
                case "Administration": {
                    return "../comm/images/interop/Amministrazione.gif";
                }
                case "AOO": {
                    return "../comm/images/interop/Aoo.gif";
                }
                case "AO": {
                    return "../comm/images/interop/Uo.gif";
                }
                case "Role": {
                    return "../comm/images/interop/Ruolo.gif";
                }
                case "Group": {
                    return "../comm/images/interop/Gruppo.gif";
                }
                case "Sector": {
                    return "../App_Themes/DocSuite2008/imgset16/GroupMembers.png";
                }
                case "Citizen": {
                    return "../App_Themes/DocSuite2008/imgset16/user.png";
                }
                default: {
                    return "";
                }
            }
        };
        ImageHelper.roleRootNodeImageUrl = "../App_Themes/DocSuite2008/imgset16/bricks.png";
        ImageHelper.roleChildNodeImageUrl = "../App_Themes/DocSuite2008/imgset16/brick.png";
        return ImageHelper;
    }());
    return ImageHelper;
});
//# sourceMappingURL=ImageHelper.js.map