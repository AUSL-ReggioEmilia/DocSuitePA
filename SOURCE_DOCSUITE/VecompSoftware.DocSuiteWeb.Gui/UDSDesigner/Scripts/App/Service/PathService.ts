
module UdsDesigner {
    export class PathService implements IPathService {
        //Filds
        private _queryString: string[] = this.QueryStringParser(location.search);

        //Functions
        QueryStringParser(url: string): string[] {
            var result = new Array<string>();
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
            } catch (e) {
            }
            return result;
        }

        Querystring(key: string): string {
            if (key === "#") {
                if (location.hash) {
                    return location.hash.substr(1);
                }
            } else {
                return this._queryString[key];
            }
        };

        PathName(): string {
            var sPath = window.location.pathname;
            var sPage = sPath.substring(sPath.lastIndexOf('/') + 1);
            return sPage;
        }
    }

    interface IPathService {
        PathName(): string;
        //Gets the querystring parameter for the key specified (returns undefined if no value is present)
        Querystring(key: string): string;

        QueryStringParser(url: string): string[];
    }
}