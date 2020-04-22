define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/UDS/UDSService", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, UDSService, ExceptionDTO) {
    var uscUDSLookup = /** @class */ (function () {
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function uscUDSLookup(serviceConfigurations) {
            var _this = this;
            /**
           *------------------------- Events -----------------------------
           */
            /**
           * Evento scatenato allo scrolling della RadComboBox di selezione lookup
           * @param args
           */
            this.rcbLookup_onScroll = function (args) {
                var element = args.target;
                if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
                    _this.rcbLookup_OnItemsRequesting(_this._rcbLookup, new Telerik.Web.UI.RadComboBoxRequestCancelEventArgs(args));
                }
            };
            this.rcbLookup_OnItemsRequesting = function (sender, args) {
                args.set_cancel(true);
                setTimeout(function () {
                    var numberOfItems = sender.get_items().get_count();
                    if (numberOfItems > 0) {
                        numberOfItems -= 1;
                    }
                    var totalCount = +sender.get_attributes().getAttribute('totalCount');
                    if (sender.get_attributes().getAttribute('filter') != _this._rcbLookup.get_text()) {
                        totalCount = undefined;
                        numberOfItems = 0;
                        sender.clearItems();
                    }
                    _this.setMoreResultBoxText('Caricamento...');
                    var updating = sender.get_attributes().getAttribute('updating') == 'true';
                    if ((!totalCount || numberOfItems < totalCount) && !updating) {
                        sender.get_attributes().setAttribute('updating', 'true');
                        var filter = _this._rcbLookup.get_text();
                        if (_this.checkBoxesEnabled && (filter.indexOf('checked') > 0 || filter.indexOf(',') > 0)) {
                            filter = '';
                        }
                        _this._udsService.getLookupValues(_this.propertyName, filter, _this.maxNumberElements, numberOfItems, function (data) {
                            if (data) {
                                if (data.values && data.values.length > 0) {
                                    _this.setLookupValues(data.values);
                                }
                                var hidden = $("#".concat(_this.hiddenLookupId));
                                if (_this.checkBoxesEnabled && hidden && hidden.val()) {
                                    var hiddenFieldValues_1 = JSON.parse(hidden.val());
                                    if (hiddenFieldValues_1 && hiddenFieldValues_1[_this.lookupLabel]) {
                                        var itemsToCheck = _this._rcbLookup.get_items().toArray().filter(function (i) {
                                            for (var _i = 0, _a = hiddenFieldValues_1[_this.lookupLabel]; _i < _a.length; _i++) {
                                                var j = _a[_i];
                                                if (j == i.get_value()) {
                                                    return true;
                                                }
                                            }
                                            return false;
                                        });
                                        for (var _i = 0, itemsToCheck_1 = itemsToCheck; _i < itemsToCheck_1.length; _i++) {
                                            var item = itemsToCheck_1[_i];
                                            item.set_checked(true);
                                        }
                                    }
                                }
                                sender.get_attributes().setAttribute('totalCount', data.count.toString());
                                sender.get_attributes().setAttribute('updating', 'false');
                                sender.get_attributes().setAttribute('filter', _this._rcbLookup.get_text());
                                if (sender.get_items().get_count() > 0) {
                                    numberOfItems = sender.get_items().get_count() - 1;
                                }
                                _this.setMoreResultBoxText('Visualizzati '.concat(numberOfItems.toString(), ' di ', data.count.toString()));
                            }
                        }, function (exception) {
                            sender.get_attributes().setAttribute('updating', 'false');
                            var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                            if (exception && uscNotification && exception instanceof ExceptionDTO) {
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    uscNotification.showNotification(exception);
                                }
                            }
                        });
                    }
                    else {
                        if (!numberOfItems) {
                            numberOfItems = 0;
                        }
                        if (!totalCount) {
                            totalCount = 0;
                        }
                        _this.setMoreResultBoxText('Visualizzati '.concat(numberOfItems.toString(), ' di ', totalCount.toString()));
                    }
                }, 100);
            };
            this.rcbLookup_OnDropDownOpened = function (sender, args) {
            };
            this.rcbLookup_OnItemChecked = function (sender, args) {
                if (args.get_item()) {
                    var items = _this._rcbLookup.get_checkedItems();
                    var hiddenFieldValues = {};
                    var checkedValues = items.map(function (i) { return i.get_text(); });
                    if (_this._hiddenLookup.val()) {
                        hiddenFieldValues = JSON.parse(_this._hiddenLookup.val());
                    }
                    hiddenFieldValues[_this.lookupLabel] = checkedValues;
                    _this._hiddenLookup.val(JSON.stringify(hiddenFieldValues));
                }
            };
            this.rcbLookup_OnSelectedIndexChange = function (sender, args) {
                var domEvent = args.get_domEvent();
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    sender.clearItems();
                    _this.setMoreResultBoxText("Visualizzati 1 di 1");
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
      *------------------------- Methods -----------------------------
      */
        uscUDSLookup.prototype.initialize = function () {
            this._udsConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, this.UDSName);
            this._udsService = new UDSService(this._udsConfiguration);
            this._rcbLookup = $find(this.rcbLookupId);
            this._rcbLookup.add_itemsRequesting(this.rcbLookup_OnItemsRequesting);
            this._rcbLookup.add_dropDownOpened(this.rcbLookup_OnDropDownOpened);
            this._rcbLookup.add_selectedIndexChanged(this.rcbLookup_OnSelectedIndexChange);
            this._rcbLookup.add_itemChecked(this.rcbLookup_OnItemChecked);
            this._hiddenLookup = $("#".concat(this.hiddenLookupId));
            this.setSelectedValues(this.lookupValue);
            var scrollContainer = $(this._rcbLookup.get_dropDownElement()).find('div.rcbScroll');
            $(scrollContainer).scroll(this.rcbLookup_onScroll);
        };
        uscUDSLookup.prototype.setLookupValues = function (values) {
            if (this._rcbLookup.get_items().get_count() == 0) {
                var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._rcbLookup.get_items().insert(0, emptyItem);
            }
            var item;
            for (var _i = 0, values_1 = values; _i < values_1.length; _i++) {
                var value = values_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(value);
                item.set_value(value);
                this._rcbLookup.get_items().add(item);
            }
            this._rcbLookup.trackChanges();
            this._rcbLookup.showDropDown();
        };
        /**
        * Metodo che setta la label MoreResultBoxText della RadComboBox
        * @param message
        */
        uscUDSLookup.prototype.setMoreResultBoxText = function (message) {
            this._rcbLookup.get_moreResultsBoxMessageElement().innerText = message;
        };
        uscUDSLookup.prototype.setSelectedValues = function (values) {
            if (values) {
                var selectedValues = JSON.parse(values);
                if (this.checkBoxesEnabled) {
                    if (selectedValues) {
                        var hiddenFieldValues = {};
                        if (this._hiddenLookup.val()) {
                            hiddenFieldValues = JSON.parse(this._hiddenLookup.val());
                        }
                        hiddenFieldValues[this.lookupLabel] = selectedValues;
                        this._hiddenLookup.val(JSON.stringify(hiddenFieldValues));
                    }
                }
                else {
                    if (selectedValues) {
                        this._rcbLookup.set_text(selectedValues[0]);
                        this._rcbLookup.set_value(selectedValues[0]);
                    }
                }
            }
        };
        return uscUDSLookup;
    }());
    return uscUDSLookup;
});
//# sourceMappingURL=uscUDSLookup.js.map