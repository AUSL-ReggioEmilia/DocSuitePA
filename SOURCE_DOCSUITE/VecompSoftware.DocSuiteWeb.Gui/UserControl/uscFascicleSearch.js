/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
define(["require", "exports", "App/Services/Fascicles/FascicleService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Helpers/PageClassHelper", "App/Helpers/SessionStorageKeysHelper", "App/Models/Workflows/WorkflowDSWEnvironmentType"], function (require, exports, FascicleService, ServiceConfigurationHelper, ExceptionDTO, PageClassHelper, SessionStorageKeysHelper, DSWEnvironmentType) {
    var uscFascicleSearch = /** @class */ (function () {
        function uscFascicleSearch(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.btnSearch_OnClick = function (sender, args) {
                var url = "../Fasc/FascRicerca.aspx?Type=Fasc&Action=SearchFascicles&DefaultCategoryId=" + _this.defaultCategoryId + "&BackButtonEnabled=True";
                _this.openWindow(url, "searchFascicles", 900, 600, _this.closeSearchFasciclesWindow);
            };
            this.closeSearchFasciclesWindow = function (sender, args) {
                if (args.get_argument()) {
                    try {
                        var fascicleId = args.get_argument();
                        sessionStorage.removeItem(_this._sessionStorageFascicleKey);
                        _this._showSearchButtonsLoader();
                        _this._loadingPanel.show(_this.fascDetailsPaneId);
                        $.when(_this.loadFascicle(fascicleId, true), _this._loadFascFoldersData(fascicleId))
                            .done(function () {
                            _this.summaryContentPanel().show();
                            _this.fascFoldersPanel().show();
                        })
                            .fail(function (exception) {
                            _this.showNotificationError(exception);
                        }).always(function () {
                            _this._hideSearchButtonsLoader();
                            _this._loadingPanel.hide(_this.fascDetailsPaneId);
                        });
                    }
                    catch (error) {
                        console.error(JSON.stringify(error));
                        _this.showNotificationError("Errore nella richiesta. Nessun fascicolo selezionato.");
                    }
                }
            };
            this._loadFascFoldersData = function (fascicleId) {
                var promise = $.Deferred();
                if (!_this.folderSelectionEnabled) {
                    return promise.resolve();
                }
                PageClassHelper.callUserControlFunctionSafe(_this.uscFascFoldersId)
                    .done(function (instance) {
                    instance.loadFolders(fascicleId)
                        .done(function () {
                        var fascFoldersTreeRootNode = instance.getFascicleFolderTree().get_nodes().getNode(0);
                        fascFoldersTreeRootNode.get_attributes().setAttribute("selectable", !_this.folderSelectionEnabled);
                        instance.selectFascicleNode(false);
                        instance.setRootNode(fascicleId, uscFascicleSearch.FOLDERTREE_ROOTNODE_TEXT, !_this.folderSelectionEnabled);
                        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_FASCICE_ID) == fascicleId &&
                            sessionStorage.getItem(instance.SESSION_FascicleHierarchy) &&
                            sessionStorage.getItem(instance.SESSION_FascicleHierarchy) != "[]") {
                            instance.rebuildTreeFromSession(fascicleId)
                                .done(function () { return promise.resolve(); })
                                .fail(function (exception) { return promise.reject(exception); });
                        }
                        else {
                            sessionStorage.removeItem(instance.SESSION_FascicleHierarchy);
                            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASCICE_ID, fascicleId);
                            promise.resolve();
                        }
                    })
                        .fail(function (exception) { return promise.reject(exception); });
                });
                return promise.promise();
            };
            this.btnSearchByCategory_OnClick = function (sender, args) {
                _this.sendAjaxRequest(uscFascicleSearch.SEARCH_BY_CATEGORY_ACTION, [_this.categoryFullIncrementalPath]);
            };
            this.btnSearchBySubject_OnClick = function (sender, args) {
                _this.sendAjaxRequest(uscFascicleSearch.SEARCH_BY_SUBJECT_ACTION, [_this.fascicleObject]);
            };
            this.btnSearchByMetadata_OnClick = function (sender, args) {
                $("#" + _this.metadataContainerId).show();
                _this._rddlSelectMetadata = $find(_this.rddlSelectMetadataId);
                var metadatas = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_UDS_METADATAS));
                if (_this._rddlSelectMetadata) {
                    _this.populateMetadataDropdown(metadatas, _this._rddlSelectMetadata);
                    _this._rddlSelectMetadata.add_selectedIndexChanged(_this.rddlSelectMetadata_OnSelectedIndexChanged);
                }
            };
            this.rddlSelectMetadata_OnSelectedIndexChanged = function (sender, eventArgs) {
                if (sender.get_selectedItem()) {
                    _this.sendAjaxRequest(uscFascicleSearch.SEARCH_BY_MEADATA_VALUE_ACTION, [sender.get_selectedItem().get_value()]);
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        uscFascicleSearch.prototype.summaryContentPanel = function () {
            return $("#" + this.summaryContentId);
        };
        uscFascicleSearch.prototype.fascFoldersPanel = function () {
            return $("#" + this.fascFoldersContentId);
        };
        /**
         *------------------------- Methods -----------------------------
         */
        uscFascicleSearch.prototype.initialize = function () {
            this._flatLoadingPanel = $find(this.ajaxFlatLoadingPanelId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_OnClick);
            if (!this.categoryFullIncrementalPath) {
                $("#" + this.btnSearchByCategoryId).hide();
            }
            else {
                this._btnSearchByCategory = $find(this.btnSearchByCategoryId);
                this._btnSearchByCategory.add_clicked(this.btnSearchByCategory_OnClick);
            }
            if (!this.fascicleObject) {
                $("#" + this.btnSearchBySubjectId).hide();
            }
            else {
                this._btnSearchBySubject = $find(this.btnSearchBySubjectId);
                this._btnSearchBySubject.add_clicked(this.btnSearchBySubject_OnClick);
            }
            if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_UDS_METADATAS) && this.dswEnvironment == DSWEnvironmentType.UDS) {
                $("#" + this.btnSearchByMetadataId).show();
                this._btnSearchByMetadata = $find(this.btnSearchByMetadataId);
                this._btnSearchByMetadata.add_clicked(this.btnSearchByMetadata_OnClick);
            }
            this._sessionStorageFascicleKey = this.pageId + "_selectedFascicle";
            sessionStorage.removeItem(this._sessionStorageFascicleKey);
            var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleServiceConfiguration);
            this._uscFascFolders = $("#" + this.uscFascFoldersId).data();
            this.bindLoaded();
        };
        uscFascicleSearch.prototype.getSelectedFascicleFolder = function () {
            var selectedFascicle = this.getSelectedFascicle();
            return this._uscFascFolders.isVisible && selectedFascicle ? this._uscFascFolders.getSelectedFascicleFolder(selectedFascicle.UniqueId) : undefined;
        };
        uscFascicleSearch.prototype.bindLoaded = function () {
            $("#" + this.pageId).data(this);
            $("#" + this.pageId).triggerHandler(uscFascicleSearch.LOADED_EVENT);
        };
        uscFascicleSearch.prototype.clearSelections = function () {
            sessionStorage.removeItem(this._sessionStorageFascicleKey);
            this.summaryContentPanel().hide();
            this.fascFoldersPanel().hide();
        };
        uscFascicleSearch.prototype.showContentPanel = function () {
            this.summaryContentPanel().show();
            this.fascFoldersPanel().show();
        };
        uscFascicleSearch.prototype.loadFascicle = function (fascicleId, cacheSelectedFascicle) {
            var _this = this;
            if (cacheSelectedFascicle === void 0) { cacheSelectedFascicle = false; }
            var promise = $.Deferred();
            if (!fascicleId) {
                return promise.reject("Nessun id fascicolo definito per la ricerca");
            }
            this._fascicleService.getFascicle(fascicleId, function (data) {
                if (!data) {
                    return promise.reject("Nessun fascicolo trovato con id " + fascicleId);
                }
                _this._uscFascicleSummary = $("#" + _this.uscFascicleSummaryId).data();
                if (jQuery.isEmptyObject(_this._uscFascicleSummary)) {
                    return promise.reject("E' avvenuto un errore durante il carimento delle informazioni del fascicolo selezionato. Si prega di riprovare.");
                }
                if (cacheSelectedFascicle) {
                    sessionStorage.setItem(_this._sessionStorageFascicleKey, JSON.stringify(data));
                }
                $("#" + _this.pageId).triggerHandler(uscFascicleSearch.FASCICLE_SELECTED_EVENT, data);
                _this._uscFascicleSummary.loadData(data)
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscFascicleSearch.prototype.getSelectedFascicle = function () {
            if (sessionStorage[this._sessionStorageFascicleKey]) {
                return JSON.parse(sessionStorage[this._sessionStorageFascicleKey]);
            }
            return undefined;
        };
        uscFascicleSearch.prototype.setButtonSearchEnabled = function (value) {
            this._btnSearch.set_enabled(value);
        };
        uscFascicleSearch.prototype.populateMetadataDropdown = function (metadatas, rddlSelectMetadata) {
            var item;
            for (var _i = 0, metadatas_1 = metadatas; _i < metadatas_1.length; _i++) {
                var metadata = metadatas_1[_i];
                item = new Telerik.Web.UI.DropDownListItem();
                item.set_text(metadata.KeyName);
                item.set_value(JSON.stringify(metadata));
                rddlSelectMetadata.get_items().add(item);
            }
            rddlSelectMetadata.trackChanges();
        };
        uscFascicleSearch.prototype.sendAjaxRequest = function (actionName, ajaxValues) {
            var ajaxModel = {
                ActionName: actionName,
                Value: ajaxValues
            };
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        uscFascicleSearch.prototype.openWindowCallback = function (url, windowName) {
            this.openWindow(url, windowName, 900, 600, this.closeSearchFasciclesWindow);
        };
        uscFascicleSearch.prototype.openWindow = function (url, name, width, height, closeHandler) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.remove_close(closeHandler);
            wnd.add_close(closeHandler);
            wnd.center();
            return false;
        };
        uscFascicleSearch.prototype.showNotificationError = function (exception) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(exception);
                }
            }
        };
        uscFascicleSearch.prototype._showSearchButtonsLoader = function () {
            if (this._btnSearch) {
                this._flatLoadingPanel.show(this.btnSearchId);
            }
            if (this._btnSearchByCategory) {
                this._flatLoadingPanel.show(this.btnSearchByCategoryId);
            }
            if (this._btnSearchBySubject) {
                this._flatLoadingPanel.show(this.btnSearchBySubjectId);
            }
        };
        uscFascicleSearch.prototype._hideSearchButtonsLoader = function () {
            if (this._btnSearch) {
                this._flatLoadingPanel.hide(this.btnSearchId);
            }
            if (this._btnSearchByCategory) {
                this._flatLoadingPanel.hide(this.btnSearchByCategoryId);
            }
            if (this._btnSearchBySubject) {
                this._flatLoadingPanel.hide(this.btnSearchBySubjectId);
            }
        };
        uscFascicleSearch.LOADED_EVENT = "onLoaded";
        uscFascicleSearch.FASCICLE_SELECTED_EVENT = "onFascicleSelected";
        uscFascicleSearch.FOLDERTREE_ROOTNODE_TEXT = "Cartelle del fascicolo";
        uscFascicleSearch.SEARCH_BY_CATEGORY_ACTION = "searchByCategory";
        uscFascicleSearch.SEARCH_BY_SUBJECT_ACTION = "searchBySubject";
        uscFascicleSearch.SEARCH_BY_MEADATA_VALUE_ACTION = "searchByMetadataValue";
        return uscFascicleSearch;
    }());
    return uscFascicleSearch;
});
//# sourceMappingURL=uscFascicleSearch.js.map