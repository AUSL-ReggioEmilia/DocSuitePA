define(["require", "exports", "App/Helpers/EnumHelper"], function (require, exports, EnumHelper) {
    var uscCustomActionsRest = /** @class */ (function () {
        function uscCustomActionsRest(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscCustomActionsRest.prototype._menuContent = function () {
            return document.getElementById(this.menuContentId);
        };
        uscCustomActionsRest.prototype.initialize = function () {
            $("#" + this.pageContentId).data(this);
            $("#" + this.pageContentId).triggerHandler(uscCustomActionsRest.LOADED_EVENT);
            if (this.isSummary) {
                this._menuContent().parentElement.style.marginLeft = "0px";
                //style fix
                if (document.getElementById("mainTable").parentElement.parentElement.parentElement.parentElement) {
                    document.getElementById("mainTable").parentElement.parentElement.parentElement.parentElement.style.overflow = "hidden";
                    document.getElementById("mainTable").style.margin = "auto";
                }
            }
            else {
                document.getElementById("mainTable").removeAttribute("style");
            }
        };
        uscCustomActionsRest.prototype.loadItems = function (customActions, customActionsIcons) {
            this._clearPage();
            var incremental = 0;
            var _loop_1 = function (customActionProperty) {
                switch (typeof customActions[customActionProperty]) {
                    case uscCustomActionsRest.BOOLEAN_PropertyType: {
                        if (this_1.isSummary) {
                            if (customActionsIcons && customActionsIcons.filter(function (x) { return x.UseIconFor === customActionProperty; }).length > 0) {
                                var customActionsIcon = customActionsIcons.filter(function (x) { return x.UseIconFor === customActionProperty; })[0];
                                this_1._fillHTMLIconElement(incremental, this_1.summaryComponentIconId, customActionsIcon.IconURL, customActionsIcon.Tooltip);
                            }
                            else if (customActions[customActionProperty]) {
                                this_1._fillHTMLGenericElement(incremental, this_1.summaryComponentCheckboxId, customActionProperty, customActions[customActionProperty], "form-control");
                            }
                        }
                        else {
                            this_1._fillHTMLGenericElement(incremental, this_1.componentCheckBoxId, customActionProperty, customActions[customActionProperty], "form-control");
                        }
                        break;
                    }
                }
                incremental++;
            };
            var this_1 = this;
            for (var customActionProperty in customActions) {
                _loop_1(customActionProperty);
            }
        };
        uscCustomActionsRest.prototype._fillHTMLGenericElement = function (incremental, idComponent, propertyName, value, cssClass) {
            var idCloned = this._cloneElement(idComponent, incremental);
            var labelField = $("#" + idCloned).find('label')[0];
            labelField.textContent = this._enumHelper.getCustomActionDescription(propertyName) + ": ";
            labelField.setAttribute(uscCustomActionsRest.ATTRIBUTE_PropertyName, propertyName);
            var inputElement = ($("#" + idCloned + " :input." + cssClass))[0];
            if (value) {
                inputElement.value = value;
                inputElement.checked = value;
            }
        };
        uscCustomActionsRest.prototype._fillHTMLIconElement = function (incremental, idComponent, iconUrl, tooltip) {
            var idCloned = this._cloneElement(idComponent, incremental);
            var iconElement = document.getElementById(idCloned);
            var htmlIcon = iconElement.children[0];
            htmlIcon.src = iconUrl;
            htmlIcon.title = tooltip;
        };
        uscCustomActionsRest.prototype._cloneElement = function (elementId, incremental) {
            var element;
            element = document.getElementById(elementId);
            var cln = element.cloneNode(true);
            cln.setAttribute("id", element.id + incremental);
            this._menuContent().appendChild(cln);
            return cln.id;
        };
        uscCustomActionsRest.prototype._clearPage = function () {
            while (this._menuContent().firstChild) {
                this._menuContent().removeChild(this._menuContent().firstChild);
            }
        };
        uscCustomActionsRest.prototype.getCustomActions = function () {
            var customActions = {};
            $.each(this._menuContent().children, function (index, divElement) {
                var dataset = divElement.getAttribute("data-type");
                switch (dataset) {
                    case "CheckBox":
                        var propertyName = divElement.querySelectorAll("[" + uscCustomActionsRest.ATTRIBUTE_PropertyName + "]")[0].getAttribute(uscCustomActionsRest.ATTRIBUTE_PropertyName);
                        var propertyValue = divElement.querySelectorAll("input[type=radio]")[0].checked;
                        customActions[propertyName] = propertyValue;
                        break;
                    default:
                        break;
                }
            });
            return customActions;
        };
        uscCustomActionsRest.BOOLEAN_PropertyType = "boolean";
        uscCustomActionsRest.ATTRIBUTE_PropertyName = "PropertyName";
        uscCustomActionsRest.LOADED_EVENT = "onLoaded";
        return uscCustomActionsRest;
    }());
    return uscCustomActionsRest;
});
//# sourceMappingURL=uscCustomActionsRest.js.map