/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/ContactService", "../App/Helpers/ImageHelper"], function (require, exports, ServiceConfigurationHelper, ContactService, ImageHelper) {
    var uscContactSearchRest = /** @class */ (function () {
        function uscContactSearchRest(serviceConfigurations) {
            var _this = this;
            this.SEARCH_BY_PARENT_COMMAND = "searchByParent";
            this.DOWN_ARROW_ICON = "../App_Themes/DocSuite2008/imgset16/down_arrow.png";
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
                    ParentToExclude: _this.parentToExclude,
                    IdTenant: _this.tenantId !== "" ? _this.tenantId : null
                };
                sender._onRequestStart();
                _this._contactService.findContacts(finderModel, function (data) {
                    _this._rcdsContactsFinder.set_data(data);
                    _this._rcdsContactsFinder.get_filterExpressions().clear();
                    _this._rcdsContactsFinder.fetch(function () {
                        var dataItemView = _this._rcdsContactsFinder.view();
                        sender._loadItemsFromData(dataItemView, true);
                        var searchBoxButton = _this._rsbSearchBox.get_buttons().getButton(0);
                        searchBoxButton.set_imageUrl(_this.DOWN_ARROW_ICON);
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
            this.rsbSearchBox_buttonCommand = function (sender, args) {
                1;
                switch (args.get_commandName()) {
                    case _this.SEARCH_BY_PARENT_COMMAND: {
                        if (!_this.filterByParentId) {
                            break;
                        }
                        sender._onRequestStart();
                        _this._contactService.getByParentId(_this.filterByParentId, 20, function (data) {
                            for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                                var contact = data_1[_i];
                                contact.Description = contact.Description.replace("|", " ");
                            }
                            _this._rcdsContactsFinder.set_data(data);
                            _this._rcdsContactsFinder.get_filterExpressions().clear();
                            _this._rcdsContactsFinder.fetch(function () {
                                var dataItemView = _this._rcdsContactsFinder.view();
                                sender._loadItemsFromData(dataItemView, true);
                            });
                        }, function (exception) {
                            console.error(exception.statusText);
                        });
                        break;
                    }
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
            this._rsbSearchBox.add_buttonCommand(this.rsbSearchBox_buttonCommand);
            this._toolTipManager = $find(this.toolTipManagerId);
            var searchBoxButton = this._rsbSearchBox.get_buttons().getButton(0);
            if (!this.filterByParentId) {
                searchBoxButton.get_element().style.display = "none";
            }
            searchBoxButton.set_cssClass("searchBoxButton");
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