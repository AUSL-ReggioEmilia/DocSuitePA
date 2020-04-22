var UdsDesigner;
(function (UdsDesigner) {
    var PathService = /** @class */ (function () {
        function PathService() {
            //Filds
            this._queryString = this.QueryStringParser(location.search);
        }
        //Functions
        PathService.prototype.QueryStringParser = function (url) {
            var result = new Array();
            try {
                var parts = (url.split("?")[1]).split("#");
                url = parts[0];
                if (parts[1]) {
                    result["#"] = decodeURIComponent(parts[1]);
                }
                var args = url.split('&');
                var len = args.length;
                for (var i = 0; i < len; i++) {
                    var param = args[i].split('=');
                    result[param[0]] = decodeURIComponent(param[1]);
                }
            }
            catch (e) {
            }
            return result;
        };
        PathService.prototype.Querystring = function (key) {
            if (key === "#") {
                if (location.hash) {
                    return location.hash.substr(1);
                }
            }
            else {
                return this._queryString[key];
            }
        };
        ;
        PathService.prototype.PathName = function () {
            var sPath = window.location.pathname;
            var sPage = sPath.substring(sPath.lastIndexOf('/') + 1);
            return sPage;
        };
        return PathService;
    }());
    UdsDesigner.PathService = PathService;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=PathService.js.map