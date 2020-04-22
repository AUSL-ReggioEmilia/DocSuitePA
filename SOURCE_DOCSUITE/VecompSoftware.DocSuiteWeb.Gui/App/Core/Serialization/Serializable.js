define(["require", "exports"], function (require, exports) {
    /**
     * Classe astratta che permette per la serializzazione/deserializzazione
     * di oggetti JSON e che permette di ricreare l'istanza della classe in maniera ricorsiva.
     */
    var Serializable = /** @class */ (function () {
        /**
         * Costruttore
         */
        function Serializable() {
            this._classMappings = new Array();
            this.initClassMapping();
        }
        Serializable.prototype.fromJson = function (JSO) {
            this.makeObject(JSO, this);
        };
        /**
         * Serializza la classe in un oggetto JSON
         */
        Serializable.prototype.toJson = function () {
            return JSON.stringify(this);
        };
        /**
         * Aggiunge la mappatura delle proprietà figlie da deserializzare in maniera ricorsiva
         * @param tclass
         * @param attributeName
         */
        Serializable.prototype.addChildClassMapping = function (tclass, attributeName) {
            var classMap = {};
            classMap.attributeName = attributeName;
            classMap.className = tclass.name;
            classMap.classObject = tclass;
            this._classMappings.push(classMap);
        };
        /**
         * Dato un attribute_name ritorna la relativa mappatura se definita
         * @param attrName
         */
        Serializable.prototype.getClassMappingFor = function (attrName) {
            var props = $.grep(this._classMappings, function (classMap, index) {
                return classMap.attributeName == attrName;
            });
            if (props.length > 0) {
                return props[0];
            }
            return null;
        };
        ;
        /**
         * Crea l'istanza delle proprietà figlie se definite nella mappatura
         * @param prop
         * @param obj
         */
        Serializable.prototype.makeInstance = function (prop, obj) {
            var classMap = this.getClassMappingFor(prop);
            if (classMap != undefined && classMap.classObject instanceof Function) {
                var instance = new classMap.classObject();
                if (instance instanceof Serializable) {
                    instance.fromJson(obj);
                    return instance;
                }
                return obj;
            }
            else {
                return obj;
            }
        };
        /**
         * Ritorna l'elemento verificando se creare una nuova istanza o ritornare l'elemento stesso
         * @param obj
         * @param propName
         */
        Serializable.prototype.resolveElement = function (obj, propName) {
            var _this = this;
            if (obj == undefined)
                return obj;
            if (this.isArray(obj)) {
                var propCollection_1 = new Array();
                $.each(obj, function (index, prop) {
                    propCollection_1.push(_this.resolveElement(prop, propName));
                });
                return propCollection_1;
            }
            else if (this.isObject(obj)) {
                return this.makeInstance(propName, obj);
            }
            else {
                return obj;
            }
        };
        /**
         * Crea il nuovo oggetto
         * @param JSO
         * @param thisx
         */
        Serializable.prototype.makeObject = function (JSO, thisx) {
            JSO = (this.isObject(JSO)) ? JSO : JSON.parse(JSO);
            for (var prop in JSO) {
                var attrName = prop;
                if ((this.isArray(JSO[prop]) || this.isObject(JSO[prop])) && JSO[prop] != undefined) {
                    var classMap = this.getClassMappingFor(prop);
                    attrName = classMap ? classMap.attributeName : attrName;
                }
                thisx[prop] = this.resolveElement(JSO[prop], attrName);
            }
            ;
        };
        /**
         * Verifica se l'elemento passato è un array
         * @param obj
         */
        Serializable.prototype.isArray = function (obj) {
            return $.isArray(obj);
        };
        /**
         * Verifica se l'elemento passato è un oggetto
         * @param obj
         */
        Serializable.prototype.isObject = function (obj) {
            return $.isPlainObject(obj);
        };
        return Serializable;
    }());
    return Serializable;
});
//# sourceMappingURL=Serializable.js.map