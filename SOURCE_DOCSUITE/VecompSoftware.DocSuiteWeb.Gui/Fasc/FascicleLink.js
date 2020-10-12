/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Models/Fascicles/FascicleLinkModel", "App/Models/Fascicles/FascicleLinkType", "App/Models/Fascicles/FascicleType", "App/Services/Fascicles/FascicleLinkService", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Securities/DomainUserService", "UserControl/uscFascSummary", "App/Services/Dossiers/DossierFolderService", "App/Models/Dossiers/DossierFolderStatus", "App/Models/InsertActionType", "App/Services/Fascicles/FascicleService", "App/Models/UpdateActionType", "UserControl/uscDossierSummary", "UserControl/uscDossierFolders", "../app/core/extensions/string"], function (require, exports, FascicleLinkModel, FascicleLinkType, FascicleType, FascicleLinkService, FascicleBase, ServiceConfigurationHelper, DomainUserService, UscFascSummary, DossierFolderService, DossierFolderStatus, InsertActionType, FascicleService, UpdateActionType, uscDossierSummary, UscDossierFolders) {
    var FascicleLink = /** @class */ (function (_super) {
        __extends(FascicleLink, _super);
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function FascicleLink(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            _this.isFascicleTabSelected = false;
            /**
             *------------------------- Events -----------------------------
             */
            _this.btnSearchDossier_OnClick = function (sender, args) {
                var url = "../Dossiers/DossierRicerca.aspx?Type=Dossier&IsWindowPopupEnable=True&DossierStatusEnabled=False";
                _this.openWindow(url, "windowOpenDossierRicerca", 750, 600, _this.closeWindowCallback);
            };
            _this.RtsFascicleLink_OnTabSelecting = function (source, args) {
                if (args.get_tab().get_value() == FascicleLink.DossierTAB) {
                    _this.isFascicleTabSelected = false;
                    $("#" + _this.dossierDisponibiliId).show();
                    $("#" + _this.dossierCollegatiId).show();
                    _this.loadDossier();
                    $("#" + _this.fascicoliCollegatiId).hide();
                    $("#" + _this.uscFascicleSearchId).hide();
                    _this._btnLink.set_enabled(false);
                    _this._btnRemove.set_enabled(false);
                }
                else {
                    _this.isFascicleTabSelected = true;
                    $("#" + _this.dossierDisponibiliId).hide();
                    $("#" + _this.dossierCollegatiId).hide();
                    $("#" + _this.fascicoliCollegatiId).show();
                    $("#" + _this.uscFascicleSearchId).show();
                    $("#".concat(_this.uscFascSummaryId)).bind(UscFascSummary.LOADED_EVENT, function (args) {
                        _this.loadFascicle();
                    });
                    _this.loadFascicle();
                }
            };
            _this.btnLink_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this.isFascicleTabSelected == true) {
                    _this.linkFascicle();
                }
                else {
                    _this.linkDossier();
                }
            };
            _this.btnRemove_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this.isFascicleTabSelected == true) {
                    _this.removeFascicleLink();
                }
                else {
                    _this.removeDossierLink();
                }
            };
            _this.closeWindowCallback = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var idDossier = args.get_argument();
                _this._loadingPanel.show(_this.pageContentId);
                _this.loadDossierSummary(idDossier);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Inizializzazione
        */
        FascicleLink.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnLink = $find(this.btnLinkId);
            if (this._btnLink) {
                this._btnLink.add_clicking(this.btnLink_OnClick);
            }
            this._btnRemove = $find(this.btnRemoveId);
            if (this._btnRemove) {
                this._btnRemove.add_clicking(this.btnRemove_OnClick);
            }
            this._rgvLinkedFascicles = $find(this.rgvLinkedFasciclesId);
            this._rgvLinkedDossiers = $find(this.rgvLinkedDossiersId);
            this._btnLink.set_enabled(false);
            this._btnRemove.set_enabled(false);
            this._rtsFascicleLink = $find(this.rtsFascicleLinkId);
            this._rtsFascicleLink.add_tabSelecting(this.RtsFascicleLink_OnTabSelecting);
            this._btnSearchDossier = $find(this.btnSearchDossierId);
            this._btnSearchDossier.add_clicked(this.btnSearchDossier_OnClick);
            $("#" + this.uscFascicleSearchId).hide();
            this.loadFascicleSummary();
            this._uscDossierSummary = $("#" + this.uscDossierSummaryId).data();
            try {
                var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOMAIN_TYPE_NAME);
                this._domainUserService = new DomainUserService(domainUserConfiguration);
                var fascicleLinkServiceConfiguration = $.grep(this._serviceConfigurations, function (x) { return x.Name == FascicleBase.FASCICLE_LINK_TYPE_NAME; })[0];
                this._fascicleLinkService = new FascicleLinkService(fascicleLinkServiceConfiguration);
                var dossierFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOSSIERFOLDER_TYPE_NAME);
                this._dossierFolderService = new DossierFolderService(dossierFolderServiceConfiguration);
                var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME);
                this._fascicleService = new FascicleService(fascicleServiceConfiguration);
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
            }
        };
        /**
      *------------------------- Methods -----------------------------
      */
        FascicleLink.prototype.openWindow = function (url, name, width, height, onCloseCallback) {
            var manager = $find(this.radWindowManagerFascicleLink);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            if (onCloseCallback) {
                wnd.add_close(onCloseCallback);
            }
            return false;
        };
        FascicleLink.prototype.loadFascicle = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null) {
                    _this._btnRemove.set_enabled(false);
                    _this._btnLink.set_enabled(false);
                    return;
                }
                var fascicleModel = data;
                var uscFascSummary = $("#".concat(_this.uscFascSummaryId)).data();
                if (!jQuery.isEmptyObject(uscFascSummary)) {
                    uscFascSummary.loadData(fascicleModel);
                }
                _this._currentFascicle = fascicleModel;
                _this.loadData(fascicleModel, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascicleLink.prototype.loadFascicleSummary = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                var fascicleModel = data;
                var uscFascSummary = $("#".concat(_this.uscFascSummaryId)).data();
                if (!jQuery.isEmptyObject(uscFascSummary)) {
                    uscFascSummary.loadData(fascicleModel);
                }
                _this._loadingPanel.hide(_this.pageContentId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascicleLink.prototype.loadDossierSummary = function (dossierId) {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            var promise = $.Deferred();
            this.dossierSumm(dossierId).done(function () {
                $("#" + _this.dossierSummaryContainerId).show();
                $("#" + _this.dossierFolderCotainerId).show();
                _this._dossierFolderToLink = dossierId;
                _this._btnLink.set_enabled(true);
                _this._btnRemove.set_enabled(true);
                _this._loadingPanel.hide(_this.pageContentId);
                promise.resolve();
            }).fail(function (exception) {
                promise.reject(exception);
            }).always(function () {
                _this._loadingPanel.hide(_this.pageContentId);
            });
            return promise.promise();
        };
        FascicleLink.prototype.dossierSumm = function (dossierId) {
            var _this = this;
            var promise = $.Deferred();
            var uscDossierSumm = $("#" + this.uscDossierSummaryId).data();
            uscDossierSumm.loadDossierSummary(dossierId).done(function () {
                _this.loadFolders(dossierId);
                promise.resolve();
            });
            return promise.promise();
        };
        FascicleLink.prototype.loadFolders = function (dossierId) {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._dossierFolderService.getChildren(dossierId, DossierFolderStatus.Folder, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        var uscDossierFolders = $("#".concat(_this.uscDossierFoldersId)).data();
                        uscDossierFolders.setRootNode(uscDossierSummary.DOSSIER_TITLE, dossierId);
                        UscDossierFolders.defaultFilterStatus = 8;
                        uscDossierFolders.loadNodes(data);
                        uscDossierFolders.setButtonVisibility(false);
                        uscDossierFolders.setStatusVisibility(false);
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
        * TODO: da togliere a favore di Signalr
        */
        FascicleLink.prototype.loadData = function (fascicle, callback) {
            var _this = this;
            this.service.getLinkedFascicles(fascicle, null, function (data) {
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    uscFascicleSearch.clearSelections();
                }
                _this.refreshLinkedFascicles(data);
                if (callback) {
                    callback();
                }
            }, function (exception) {
                _this._btnLink.set_enabled(true);
                _this._btnRemove.set_enabled(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
                if (callback) {
                    callback();
                }
            });
        };
        FascicleLink.prototype.refreshLinkedFascicles = function (data) {
            var models = new Array();
            if (data == null)
                return;
            if (data.FascicleLinks.length > 0) {
                try {
                    $.each(data.FascicleLinks, function (index, fascicleLink) {
                        var model;
                        var imageUrl = "";
                        var openCloseTooltip = "";
                        var fascicleTypeImageUrl = "";
                        var fascicleTypeTooltip = "";
                        if (fascicleLink.FascicleLinked.EndDate == null) {
                            imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                            openCloseTooltip = "Aperto";
                        }
                        else {
                            imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                            openCloseTooltip = "Chiuso";
                        }
                        switch (FascicleType[fascicleLink.FascicleLinked.FascicleType.toString()]) {
                            case FascicleType.Period:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/history.png";
                                fascicleTypeTooltip = "Periodico";
                                break;
                            case FascicleType.Legacy:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_legacy.png";
                                fascicleTypeTooltip = "Fascicolo non a norma";
                                break;
                            case FascicleType.Procedure:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png";
                                fascicleTypeTooltip = "Per procedimento";
                                break;
                            case FascicleType.SubFascicle:
                                fascicleTypeImageUrl = "";
                                fascicleTypeTooltip = "Sotto fascicolo";
                                break;
                        }
                        var tileText = fascicleLink.FascicleLinked.Title.concat(" ", fascicleLink.FascicleLinked.FascicleObject);
                        model = {
                            Name: tileText, FascicleLinkUniqueId: fascicleLink.UniqueId, UniqueId: fascicleLink.FascicleLinked.UniqueId, Category: fascicleLink.FascicleLinked.Category.Name,
                            ImageUrl: imageUrl, OpenCloseTooltip: openCloseTooltip, FascicleTypeImageUrl: fascicleTypeImageUrl, FascicleTypeToolTip: fascicleTypeTooltip
                        };
                        models.push(model);
                    });
                }
                catch (error) {
                    this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                    console.log(error.message);
                    return;
                }
            }
            var tableView = this._rgvLinkedFascicles.get_masterTableView();
            tableView.clearSelectedItems();
            tableView.set_dataSource(models);
            tableView.dataBind();
            //TODO: da rivedere
            var row = tableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
            this._btnLink.set_enabled(true);
            this._btnRemove.set_enabled(true);
        };
        FascicleLink.prototype.pad = function (currentNumber, paddingSize) {
            var s = currentNumber + "";
            while (s.length < paddingSize) {
                s = "0" + s;
            }
            return s;
        };
        FascicleLink.prototype.loadDossier = function () {
            var _this = this;
            var models = new Array();
            this._dossierFolderService.getLinkedDossierByFascicleId(this.currentFascicleId, function (data) {
                if (data == null) {
                    _this._btnRemove.set_enabled(false);
                    _this._btnLink.set_enabled(false);
                    return;
                }
                _this.dossierFoldersLinked = data;
                for (var _i = 0, _a = _this.dossierFoldersLinked; _i < _a.length; _i++) {
                    var dossierFolder = _a[_i];
                    var model = void 0;
                    var dossierName = "Dossier: " + dossierFolder.Dossier.Year + "/" + _this.pad(+dossierFolder.Dossier.Number, 7);
                    model = {
                        UniqueId: dossierFolder.Dossier.UniqueId, DossierFolderName: dossierFolder.Name, DossierName: dossierName,
                        Subject: dossierFolder.Dossier.Subject, StartDate: moment(dossierFolder.Dossier.StartDate).format("DD/MM/YYYY"),
                        Contenitori: dossierFolder.Dossier.Container.Name, Category: dossierFolder.Category.Name
                    };
                    models.push(model);
                }
                var tableView = _this._rgvLinkedDossiers.get_masterTableView();
                tableView.clearSelectedItems();
                tableView.set_dataSource(models);
                tableView.dataBind();
                _this._btnLink.set_enabled(true);
                _this._btnRemove.set_enabled(true);
            });
        };
        FascicleLink.prototype.linkFascicle = function () {
            var _this = this;
            var selectedFascicle;
            var uscFascicleSearch = $("#" + this.uscFascicleSearchId).data();
            if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                selectedFascicle = uscFascicleSearch.getSelectedFascicle();
            }
            if (selectedFascicle == null) {
                this.showNotificationMessage(this.uscNotificationId, "Nessun fascicolo selezionato");
                return;
            }
            var model = new FascicleLinkModel(selectedFascicle.UniqueId);
            var currentFascicle = this._currentFascicle;
            model.Fascicle = currentFascicle;
            model.FascicleLinkType = FascicleLinkType.Manual;
            this._loadingPanel.show(this.pageContentId);
            this._fascicleLinkService.insertFascicleLink(model, function (data) {
                _this.loadData(_this._currentFascicle, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascicleLink.prototype.linkDossier = function () {
            var _this = this;
            if (!this.currentFascicleId || !this._dossierFolderToLink)
                return;
            this._loadingPanel.show(this.pageContentId);
            this._fascicleService.getFascicle(this.currentFascicleId, function (data) {
                var fascicleModel = data;
                var dossierModel = {};
                dossierModel.UniqueId = _this._dossierFolderToLink;
                var uscDossierFolders = $("#".concat(_this.uscDossierFoldersId)).data();
                var selectedDossierFolderNode = uscDossierFolders.getSelectedDossierFolderNode().get_value();
                var dossierFolder = {
                    ParentInsertId: selectedDossierFolderNode,
                    Fascicle: fascicleModel,
                    Status: DossierFolderStatus.Fascicle,
                    Dossier: dossierModel,
                    Category: fascicleModel.Category
                };
                _this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.InsertDossierFolderAssociatedToFascicle, function (data) {
                    $("#" + _this.dossierSummaryContainerId).hide();
                    $("#" + _this.dossierFolderCotainerId).hide();
                    _this.loadDossier();
                    _this.dossierFoldersLinked.push(dossierFolder);
                    _this._loadingPanel.hide(_this.pageContentId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascicleLink.prototype.removeFascicleLink = function () {
            var _this = this;
            var dataItems = this._rgvLinkedFascicles.get_selectedItems();
            if (dataItems.length == 0) {
                this.showNotificationMessage(this.uscNotificationId, "Nessun fascicolo selezionato");
                return;
            }
            var currentFascicle = this._currentFascicle;
            var model = dataItems[0].get_dataItem();
            var fascicleLink = new FascicleLinkModel(model.UniqueId);
            fascicleLink.Fascicle = currentFascicle;
            fascicleLink.UniqueId = model.FascicleLinkUniqueId;
            this._loadingPanel.show(this.pageContentId);
            this._fascicleLinkService.deleteFascicleLink(fascicleLink, function (data) {
                _this.loadData(_this._currentFascicle, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascicleLink.prototype.removeDossierLink = function () {
            var _this = this;
            var dataItems = this._rgvLinkedDossiers.get_selectedItems();
            if (dataItems.length == 0) {
                this.showNotificationMessage(this.uscNotificationId, "Nessun dossier selezionato");
                return;
            }
            var dossierFModel = dataItems[0].get_dataItem();
            var dossierFolderLink = {};
            dossierFolderLink.UniqueId = this.dossierFoldersLinked.filter(function (x) { return x.Dossier.UniqueId == dossierFModel.UniqueId; })[0].UniqueId;
            dossierFolderLink.Name = this.dossierFoldersLinked.filter(function (x) { return x.Dossier.UniqueId == dossierFModel.UniqueId; })[0].Name;
            dossierFolderLink.Status = DossierFolderStatus.InProgress;
            this.dossierFoldersLinked = this.dossierFoldersLinked.filter(function (x) { return x.Dossier.UniqueId != dossierFModel.UniqueId; });
            this._loadingPanel.show(this.pageContentId);
            this._dossierFolderService.updateDossierFolder(dossierFolderLink, UpdateActionType.RemoveFascicleFromDossierFolder, function (data) {
                _this.loadDossier();
                _this._loadingPanel.hide(_this.pageContentId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascicleLink.DossierTAB = "Dossier";
        FascicleLink.FascicoliTAB = "Fascicoli";
        return FascicleLink;
    }(FascicleBase));
    return FascicleLink;
});
//# sourceMappingURL=FascicleLink.js.map