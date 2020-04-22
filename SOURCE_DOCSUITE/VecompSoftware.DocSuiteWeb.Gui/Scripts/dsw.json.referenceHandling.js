var JSONHelper = (function () {

    function JSONHelper() {

    }

    JSONHelper.prototype.resolveReferences = function (jsonObj) {
        var byid = {},
            refs = [];

        jsonObj = (function recurse(obj, prop, parent) {
            if (typeof obj !== 'object' || !obj)
                return obj;

            if (Object.prototype.toString.call(obj) === '[object Array]') {
                for (var i = 0; i < obj.length; i++)
                    if (typeof obj[i] !== 'object' || !obj[i])
                        continue;
                    else if ("$ref" in obj[i])
                        obj[i] = recurse(obj[i], i, obj);
                    else
                        obj[i] = recurse(obj[i], prop, obj);
                return obj;
            }

            if ("$ref" in obj) {
                var ref = obj.$ref;
                if (ref in byid)
                    return byid[ref];

                if (parent !== undefined)
                    refs.push([parent, prop, ref]);
                return;
            } else if ("$id" in obj) {
                var id = obj.$id;
                delete obj.$id;
                if ("$values" in obj)
                    obj = $.map(obj.$values, recurse);
                else
                    for (var prop in obj)
                        obj[prop] = recurse(obj[prop], prop, obj);
                byid[id] = obj;
            }

            return obj;
        })(jsonObj);

        for (var i = 0; i < refs.length; i++) {
            var ref = refs[i];
            ref[0][ref[1]] = byid[ref[2]];
        }

        return jsonObj;
    }

    return JSONHelper;
})();