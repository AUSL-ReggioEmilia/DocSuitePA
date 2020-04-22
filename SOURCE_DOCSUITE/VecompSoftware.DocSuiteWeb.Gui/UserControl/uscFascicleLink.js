/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleService", "App/Services/Securities/DomainUserService", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, FascicleService, DomainUserService, ExceptionDTO) {
    var uscFascicleLink = /** @class */ (function () {
        /**
        * Costruttore
        * @param serviceConfiguration
        */
        function uscFascicleLink(serviceConfigurations) {
            var _this = this;
            /**
            * -------------------------- Events ---------------------------
            */
            this.rcbOtherFascicles_OnSelectedIndexChange = function (sender, args) {
                var selectedItem = sender.get_selectedItem();
                var domEvent = args.get_domEvent();
                if (domEvent.type == 'mousedown') {
                    return;
                }
                if (selectedItem == null || selectedItem.get_value() == "") {
                    _this._fascicleSummary.hide();
                    return;
                }
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    var emptyItem = sender.findItemByText('');
                    sender.clearItems();
                    sender.get_items().add(emptyItem);
                    sender.get_items().add(selectedItem);
                    sender.get_attributes().setAttribute('currentFilter', selectedItem.get_text());
                    sender.get_attributes().setAttribute('otherFascicleCount', '1');
                }
                _this._loadingPanel.show(_this.pageId);
                _this._fascicleService.getFascicle(selectedItem.get_value(), function (data) {
                    if (data == null) {
                        _this._fascicleSummary.hide();
                        return;
                    }
                    var fascicle = data;
                    _this.currentFascicleId = fascicle.UniqueId;
                    _this._domainUserService.getUser(fascicle.RegistrationUser, function (user) {
                        _this.setFascicleSummaryData(fascicle);
                        $("#".concat(_this.pageId)).data(_this);
                        _this.onExternalCategoryChange(fascicle.Category.EntityShortId);
                        _this._loadingPanel.hide(_this.pageId);
                    }, function (exception) {
                        //Carico ugualmente il sommario del fascicolo
                        _this.setFascicleSummaryData(fascicle);
                        _this._loadingPanel.hide(_this.pageId);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            /**
             * Evento scatenato allo scrolling della RadComboBox di selezione fascicoli
             * @param args
             */
            this.rcbOtherFascicles_onScroll = function (args) {
                var element = args.target;
                if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
                    var filter = _this._rcbOtherFascicles.get_text();
                    _this.rcbOtherFascicles_OnClientItemsRequested(_this._rcbOtherFascicles, new Telerik.Web.UI.RadComboBoxRequestEventArgs(filter, args));
                }
            };
            /**
             * Evento scatenato dalla RadComboBox per inizializzare i dati da visualizzare
             * @param sender
             * @param args
             */
            this.rcbOtherFascicles_OnClientItemsRequested = function (sender, args) {
                if (!_this.selectedCategoryId) {
                    _this.selectedCategoryId = 0;
                }
                var numberOfItems = sender.get_items().get_count();
                if (numberOfItems > 0) {
                    //Decremento di 1 perchè la combo visualizza anche un item vuoto
                    numberOfItems -= 1;
                }
                var currentOtherFascicleItems = numberOfItems;
                var currentComboFilter = sender.get_attributes().getAttribute('currentFilter');
                var otherFascicleCount = Number(sender.get_attributes().getAttribute('otherFascicleCount'));
                var updating = sender.get_attributes().getAttribute('updating') == 'true';
                if (isNaN(otherFascicleCount) || currentComboFilter != args.get_text()) {
                    //Se il valore del filtro è variato re-inizializzo la radcombobox per chiamare le WebAPI
                    otherFascicleCount = undefined;
                }
                sender.get_attributes().setAttribute('currentFilter', args.get_text());
                if ((otherFascicleCount == undefined || currentOtherFascicleItems < otherFascicleCount) && !updating) {
                    sender.get_attributes().setAttribute('updating', 'true');
                    _this._fascicleService.getFascicleByCategory(_this.selectedCategoryId, args.get_text(), function (data) {
                        try {
                            _this.refreshFascicles(data.value);
                            var scrollToPosition = args.get_domEvent() == undefined;
                            if (scrollToPosition) {
                                if (sender.get_items().get_count() > 0) {
                                    var scrollContainer = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                    scrollContainer.scrollTop($(sender.get_items().getItem(currentOtherFascicleItems + 1).get_element()).position().top);
                                }
                            }
                            sender.get_attributes().setAttribute('updating', 'false');
                        }
                        catch (error) {
                            console.log(JSON.stringify(error));
                        }
                    });
                }
            };
            /**
        * ------------------------------ Methods ----------------------------
        */
            /**
             * Metodo per popolare la RadComboBox di selezione fascicoli
             * @param data
             */
            this.refreshFascicles = function (data) {
                if (data.length > 0) {
                    _this._rcbOtherFascicles.clearItems();
                    _this._rcbOtherFascicles.beginUpdate();
                    if (_this._rcbOtherFascicles.get_items().get_count() == 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._rcbOtherFascicles.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, fascicle) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(fascicle.Title.concat(" ", fascicle.FascicleObject));
                        item.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
                        item.set_value(fascicle.UniqueId);
                        _this._rcbOtherFascicles.get_items().add(item);
                    });
                    _this._rcbOtherFascicles.showDropDown();
                    _this._rcbOtherFascicles.endUpdate();
                }
            };
            /**
            * Evento al cambio di classificatore
            */
            this.onCategoryChanged = function (idCategory) {
                _this._rcbOtherFascicles.clearItems();
                _this.selectedCategoryId = idCategory;
                _this._lblCategoryMessage.html("");
                $("#".concat(_this.pageId)).data(_this);
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle");
            this._serviceConfigurations = serviceConfigurations;
            if (!serviceConfiguration) {
                return;
            }
            this._fascicleService = new FascicleService(serviceConfiguration);
        }
        /**
        * Inizializzazione
        */
        uscFascicleLink.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._lblCategoryMessage = $("#".concat(this.lblCategoryMessageId));
            this._rcbOtherFascicles = $find(this.rcbOtherFascicles);
            this._rcbOtherFascicles.add_selectedIndexChanged(this.rcbOtherFascicles_OnSelectedIndexChange);
            this._rcbOtherFascicles.add_itemsRequested(this.rcbOtherFascicles_OnClientItemsRequested);
            this._fascicleSummary = $("#".concat(this.fascicleSummaryId));
            this._ajaxManager = $find(this.ajaxManagerId);
            this._fascicleSummary.hide();
            var scrollContainer = $(this._rcbOtherFascicles.get_dropDownElement()).find('div.rcbScroll');
            $(scrollContainer).scroll(this.rcbOtherFascicles_onScroll);
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleConfiguration);
            this.bindLoaded();
        };
        /*
        * Cambio esternamente il classificatore
        */
        uscFascicleLink.prototype.onExternalCategoryChange = function (idCategory) {
            this._ajaxManager.ajaxRequest(this.uscCategoryId.concat("|").concat(idCategory.toString()));
        };
        /**
     * Popolo i dati del fieldset di sommario di fascicolo selezionato
     * @param fascicle
     */
        uscFascicleLink.prototype.setFascicleSummaryData = function (fascicle) {
            this.selectedCategoryId = fascicle.Category.EntityShortId;
            var uscFascSummary = $("#".concat(this.uscFascSummaryId)).data();
            if (!jQuery.isEmptyObject(uscFascSummary)) {
                uscFascSummary.loadData(fascicle);
            }
            this._fascicleSummary.show();
        };
        /**
        * Chiude la RadWindow
        */
        uscFascicleLink.prototype.closeWindow = function (args) {
            var wnd = this.getRadWindow();
            wnd.close(args);
        };
        /**
        * Recupera una RadWindow dalla pagina
        */
        uscFascicleLink.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
    * Scateno l'evento di "Load Completed" del controllo
    */
        uscFascicleLink.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscFascicleLink.LOADED_EVENT);
        };
        uscFascicleLink.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscFascicleLink.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscFascicleLink.LOADED_EVENT = "onLoaded";
        return uscFascicleLink;
    }());
    return uscFascicleLink;
});
//# sourceMappingURL=uscFascicleLink.js.map