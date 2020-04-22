/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/ContactService", "../App/Helpers/ImageHelper"], function (require, exports, ServiceConfigurationHelper, ContactService, ImageHelper) {
    var uscContactSearchRest = /** @class */ (function () {
        function uscContactSearchRest(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.rsbSearchBox_dataRequesting = function (sender, args) {
                args.set_cancel(true);
                var finderModel = {
                    Filter: args.get_text(),
                    ApplyAuthorizations: _this.applyAuthorizations,
                    ExcludeRoleContacts: _this.excludeRoleContacts,
                    ParentId: _this.filterByParentId,
                    ParentToExclude: _this.parentToExclude
                };
                sender._onRequestStart();
                _this._contactService.findContacts(finderModel, function (data) {
                    _this._rcdsContactsFinder.set_data(data);
                    _this._rcdsContactsFinder.get_filterExpressions().clear();
                    _this._rcdsContactsFinder.fetch(function () {
                        var dataItemView = _this._rcdsContactsFinder.view();
                        sender._loadItemsFromData(dataItemView, true);
                    });
                }, function (exception) {
                    console.error(exception.statusText);
                });
            };
            this.rsbSearchBox_search = function (sender, args) {
                if (args.get_value()) {
                    $("#" + _this.pnlMainContentId).triggerHandler(uscContactSearchRest.SELECTED_CONTACT_EVENT, args.get_value());
                }
            };
            var contactServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
            this._contactService = new ContactService(contactServiceConfiguration);
        }
        /**
        *------------------------- Methods -----------------------------
        */
        uscContactSearchRest.prototype.initialize = function () {
            this._rcdsContactsFinder = $find(this.rcdsContactsFinderId);
            this._rsbSearchBox = $find(this.rsbSearchBoxId);
            this._rsbSearchBox.add_dataRequesting(this.rsbSearchBox_dataRequesting);
            this._rsbSearchBox.add_search(this.rsbSearchBox_search);
            this._toolTipManager = $find(this.toolTipManagerId);
            this.bindLoaded();
        };
        uscContactSearchRest.prototype.bindLoaded = function () {
            $("#" + this.pnlMainContentId).data(this);
            $("#" + this.pnlMainContentId).triggerHandler(uscContactSearchRest.LOADED_EVENT);
        };
        uscContactSearchRest.prototype.showTooltip = function (idContact) {
            var _this = this;
            var targetId = "item_" + idContact;
            var target = Sys.UI.DomElement.getElementById(targetId);
            var tooltip = this._toolTipManager.getToolTipByElement(target);
            var createTooltipAction = function () { return $.Deferred().resolve().promise(); };
            if (!tooltip) {
                createTooltipAction = function () {
                    var promise = $.Deferred();
                    _this.createContactTree("parentTree_" + idContact, idContact)
                        .done(function () {
                        var content = document.getElementById(targetId.replace("item", "toolTipContent")).innerHTML;
                        tooltip = _this._toolTipManager.createToolTip(target);
                        tooltip.set_content(content);
                        promise.resolve();
                    })
                        .fail(function (exception) { return promise.reject(exception); });
                    return promise.promise();
                };
            }
            createTooltipAction()
                .done(function () {
                setTimeout(function () {
                    tooltip.show();
                }, 20);
            })
                .fail(function (exception) { return console.error(exception.statusText); });
        };
        uscContactSearchRest.prototype.createContactTree = function (targetId, idContact) {
            var promise = $.Deferred();
            var treeListHtml = "<ul>";
            this._contactService.getContactParents(idContact, function (data) {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }
                var imageControlHtml;
                var labelControlHtml;
                for (var i = 0; i < data.length; i++) {
                    imageControlHtml = "<img src=\"" + ImageHelper.getContactTypeImageUrl(data[i].ContactType) + "\" style=\"vertical-align: middle; margin-left: " + i * 20 + "px;\"></img>";
                    labelControlHtml = "<span style=\"vertical-align: middle;\">" + data[i].Description + "</span>";
                    treeListHtml = treeListHtml + "<li>" + imageControlHtml + labelControlHtml + "</li>";
                }
                treeListHtml = treeListHtml + "</ul>";
                $("#" + targetId).html(treeListHtml);
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscContactSearchRest.prototype.clear = function () {
            this._rsbSearchBox.clear();
            this._rsbSearchBox.get_inputElement().value = "";
        };
        uscContactSearchRest.LOADED_EVENT = "onLoaded";
        uscContactSearchRest.SELECTED_CONTACT_EVENT = "onSelectedContact";
        return uscContactSearchRest;
    }());
    return uscContactSearchRest;
});
//# sourceMappingURL=uscContactSearchRest.js.map