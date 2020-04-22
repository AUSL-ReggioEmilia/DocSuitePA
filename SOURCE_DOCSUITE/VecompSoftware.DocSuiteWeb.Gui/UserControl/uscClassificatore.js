/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
define(["require", "exports"], function (require, exports) {
    var uscClassificatore = /** @class */ (function () {
        /**
        * Costruttore
             * @param serviceConfiguration
             * @param workflowStartConfiguration
        */
        function uscClassificatore(serviceConfigurations) {
            var _this = this;
            this.setCategories = function (ajaxResult) {
                var CategoriesToAdd = new Array();
                //per ora ne passa una sola, ma potenzialmente potrebbero essere un array
                CategoriesToAdd.push(ajaxResult.Value);
                _this._selectedCategories = new Array();
                if (ajaxResult.ActionName == "true") {
                    var sessionValue = _this.getCategories();
                    if (sessionValue != null) {
                        var source = JSON.parse(sessionValue);
                        _this._selectedCategories = _this.parseCategoriesFromJson(source);
                    }
                }
                for (var _i = 0, CategoriesToAdd_1 = CategoriesToAdd; _i < CategoriesToAdd_1.length; _i++) {
                    var r = CategoriesToAdd_1[_i];
                    var categoryAdded = {};
                    categoryAdded.EntityShortId = parseInt(r);
                    _this._selectedCategories.push(categoryAdded);
                }
                sessionStorage[_this._sessionStorageKey] = JSON.stringify(_this._selectedCategories);
            };
            this.getCategories = function () {
                var result = sessionStorage[_this._sessionStorageKey];
                if (result == null) {
                    return null;
                }
                return result;
            };
            this.parseCategoriesFromJson = function (source) {
                var result = new Array();
                for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                    var s = source_1[_i];
                    var category = {};
                    category.EntityShortId = s.EntityShortId;
                    result.push(category);
                }
                return result;
            };
            this.clearSessionStorage = function () {
                if (sessionStorage[_this._sessionStorageKey] != null) {
                    sessionStorage.removeItem(_this._sessionStorageKey);
                }
            };
            this.enableValidators = function (state) {
                ValidatorEnable($get(_this.anyNodeCheckId), state);
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
        *------------------------- Events -----------------------------
        */
        /**
        * ------------------------- Methods -----------------------------
        */
        /**
        * Initialize
        */
        uscClassificatore.prototype.initialize = function () {
            this._selectedCategories = new Array();
            this._sessionStorageKey = this.contentId.concat(uscClassificatore.SESSION_NAME_SELECTED_CATEGORIES);
            this.bindLoaded();
        };
        /**
    * Scateno l'evento di "Load Completed" del controllo
    */
        uscClassificatore.prototype.bindLoaded = function () {
            $("#".concat(this.contentId)).data(this);
            $("#".concat(this.contentId)).triggerHandler(uscClassificatore.LOADED_EVENT);
        };
        uscClassificatore.SESSION_NAME_SELECTED_CATEGORIES = "SelectedCategories";
        uscClassificatore.LOADED_EVENT = "onLoaded";
        return uscClassificatore;
    }());
    return uscClassificatore;
});
//# sourceMappingURL=uscClassificatore.js.map