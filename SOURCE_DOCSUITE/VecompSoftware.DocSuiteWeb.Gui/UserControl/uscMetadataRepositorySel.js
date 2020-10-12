define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/MetadataRepositoryService", "App/Models/Commons/MetadataRepositoryModel", "App/DTOs/ExceptionDTO", "./uscSetiContactSel"], function (require, exports, ServiceConfigurationHelper, MetadataRepositoryService, MetadataRepositoryModel, ExceptionDTO, uscSetiContactSel) {
    var uscMetadataRepositorySel = /** @class */ (function () {
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function uscMetadataRepositorySel(serviceConfigurations) {
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
                    _this.rcbMetadataRepository_OnItemsRequested(_this._rcbMetadataRepository, new Telerik.Web.UI.RadComboBoxRequestEventArgs(args));
                }
            };
            this.rcbMetadataRepository_OnItemsRequested = function (sender, args) {
                var numberOfItems = sender.get_items().get_count();
                if (numberOfItems > 0) {
                    numberOfItems -= 1;
                }
                var totalCount = +sender.get_attributes().getAttribute('totalCount');
                if (sender.get_attributes().getAttribute('filter') != _this._rcbMetadataRepository.get_text()) {
                    totalCount = undefined;
                    numberOfItems = 0;
                    sender.clearItems();
                }
                _this.setMoreResultBoxText('Caricamento...');
                var updating = sender.get_attributes().getAttribute('updating') == 'true';
                if ((!totalCount || numberOfItems < totalCount) && !updating) {
                    sender.get_attributes().setAttribute('updating', 'true');
                    var metadataRestrictions = [];
                    var metadataRestrictionsAttribute = _this._rcbMetadataRepository.get_attributes().getAttribute("repositoryRestrictions");
                    if (metadataRestrictionsAttribute) {
                        metadataRestrictions = JSON.parse(metadataRestrictionsAttribute);
                    }
                    var safeEncoding = sender.get_text().replace(/'/g, '%27%27');
                    _this._metadataRepositoryService.getAvailableMetadataRepositories(safeEncoding, metadataRestrictions, _this.maxNumberElements, numberOfItems, function (data) {
                        if (data) {
                            if (data.count > 0) {
                                _this.setValues(data.value);
                            }
                            sender.get_attributes().setAttribute('totalCount', data.count.toString());
                            sender.get_attributes().setAttribute('updating', 'false');
                            sender.get_attributes().setAttribute('filter', _this._rcbMetadataRepository.get_text());
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
                    _this.setMoreResultBoxText('Visualizzati '.concat(numberOfItems.toString(), ' di ', totalCount.toString()));
                }
            };
            this.rcbMetadataRepository_OnDropDownOpened = function (sender, args) {
            };
            this.rcbMetadataRepository_OnSelectedIndexChange = function (sender, args) {
                var domEvent = args.get_domEvent();
                var selectedItem = args.get_item();
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    sender.clearItems();
                    _this.setMoreResultBoxText("Visualizzati 1 di 1");
                }
                var isMetadataRepositorySelected = selectedItem && selectedItem.get_text() !== "";
                if (_this.advancedMetadataRepositoryEnabled) {
                    _this._setMetadataValueElementsState(isMetadataRepositorySelected);
                }
                if (isMetadataRepositorySelected) {
                    if (_this.advancedMetadataRepositoryEnabled && _this._enableAdvancedMetadataSearchBtn.checked) {
                        _this._uscAdvancedSearchDynamicMetadataRest.loadMetadataRepository(selectedItem.get_value());
                    }
                    $("#".concat(_this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, selectedItem.get_value());
                    _this.getSelectedMetadata().then(function (data) {
                        if (data && data.JsonMetadata) {
                            var metadataVM = JSON.parse(data.JsonMetadata);
                            $("#".concat(_this.uscSetiContactSelId)).triggerHandler(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, _this.setiContactEnabledId && metadataVM.SETIFieldEnabled && _this.setiVisibilityButtonId);
                        }
                    });
                }
                else {
                    //notify that no repository is selected to clear the metadata values control contents
                    $("#".concat(_this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, null);
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        uscMetadataRepositorySel.prototype._setMetadataValueElementsState = function (isMetadaRepositorySelected) {
            var currentAdvancedSearchBtnState = this._enableAdvancedMetadataSearchBtn.checked;
            this._enableAdvancedMetadataSearchBtn.disabled = !isMetadaRepositorySelected;
            this._enableAdvancedMetadataSearchBtn.checked = isMetadaRepositorySelected ? currentAdvancedSearchBtnState : false;
            if (!$.isEmptyObject(this._uscAdvancedSearchDynamicMetadataRest) && !this._enableAdvancedMetadataSearchBtn.checked) {
                this._uscAdvancedSearchDynamicMetadataRest.clearAdvancedSearchPanelContent();
            }
            this._txtMetadataValue.clear();
            this._txtMetadataValue.set_visible(!this._enableAdvancedMetadataSearchBtn.checked);
        };
        /**
      *------------------------- Methods -----------------------------
      */
        uscMetadataRepositorySel.prototype.initialize = function () {
            this._metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
            this._metadataRepositoryService = new MetadataRepositoryService(this._metadataRepositoryConfiguration);
            this._rcbMetadataRepository = $find(this.rcbMetadataRepositoryId);
            this._rcbMetadataRepository.add_itemsRequested(this.rcbMetadataRepository_OnItemsRequested);
            this._rcbMetadataRepository.add_dropDownOpened(this.rcbMetadataRepository_OnDropDownOpened);
            this._rcbMetadataRepository.add_selectedIndexChanged(this.rcbMetadataRepository_OnSelectedIndexChange);
            this._uscAdvancedSearchDynamicMetadataRest = $("#".concat(this.uscAdvancedSearchDynamicMetadataRestId)).data();
            if (this.advancedMetadataRepositoryEnabled) {
                this.initializeMetadataPanel();
            }
            var scrollContainer = $(this._rcbMetadataRepository.get_dropDownElement()).find('div.rcbScroll');
            $(scrollContainer).scroll(this.rcbLookup_onScroll);
            $("#".concat(this.metadataPageContentId)).data(this);
            $("#".concat(this.uscSetiContactSelId)).data(this);
        };
        uscMetadataRepositorySel.prototype.initializeMetadataPanel = function () {
            var _this = this;
            var $advancedMetadataSearchBtn = $("#" + this.enableAdvancedMetadataSearchBtnId);
            this._enableAdvancedMetadataSearchBtn = $advancedMetadataSearchBtn[0];
            $advancedMetadataSearchBtn.on("change", function () {
                var advancedMetadataSearchEnabled = _this._enableAdvancedMetadataSearchBtn.checked;
                _this._txtMetadataValue.set_visible(!advancedMetadataSearchEnabled);
                var selectedMetadataRepositoryId = _this.getSelectedMetadataRepositoryId();
                _this._uscAdvancedSearchDynamicMetadataRest.setPanelSearchType(advancedMetadataSearchEnabled, selectedMetadataRepositoryId);
            });
            this._txtMetadataValue = $find(this.txtMetadataValueId);
            this._setMetadataValueElementsState(false);
        };
        uscMetadataRepositorySel.prototype.setValues = function (repositories) {
            if (this._rcbMetadataRepository.get_items().get_count() == 0) {
                var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._rcbMetadataRepository.get_items().insert(0, emptyItem);
            }
            var item;
            for (var _i = 0, repositories_1 = repositories; _i < repositories_1.length; _i++) {
                var repository = repositories_1[_i];
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(repository.Name);
                item.set_value(repository.UniqueId);
                this._rcbMetadataRepository.get_items().add(item);
            }
            this._rcbMetadataRepository.trackChanges();
            this._rcbMetadataRepository.showDropDown();
        };
        /**
        * Metodo che setta la label MoreResultBoxText della RadComboBox
        * @param message
        */
        uscMetadataRepositorySel.prototype.setMoreResultBoxText = function (message) {
            this._rcbMetadataRepository.get_moreResultsBoxMessageElement().innerText = message;
        };
        uscMetadataRepositorySel.prototype.setComboboxText = function (id, generateMetadataInputs) {
            var _this = this;
            if (generateMetadataInputs === void 0) { generateMetadataInputs = true; }
            this._metadataRepositoryService.getById(id, function (data) {
                if (data) {
                    _this._rcbMetadataRepository.set_text(data.Name);
                    _this._rcbMetadataRepository.set_value(data.UniqueId);
                    if (generateMetadataInputs) {
                        $("#".concat(_this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, data.UniqueId);
                    }
                }
            }, function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (exception && uscNotification && exception instanceof ExceptionDTO) {
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                }
            });
        };
        uscMetadataRepositorySel.prototype.getSelectedMetadata = function () {
            var promise = $.Deferred();
            var model = new MetadataRepositoryModel();
            var selectedValue = this._rcbMetadataRepository.get_value();
            if (!selectedValue) {
                promise.resolve(model);
                return;
            }
            this._metadataRepositoryService.getFullModelById(selectedValue, function (data) {
                promise.resolve(data);
            }, function (exception) {
                console.error(exception.statusText);
                promise.reject();
            });
            return promise.promise();
        };
        uscMetadataRepositorySel.prototype.getSelectedMetadataRepositoryId = function () {
            var selectedValue = this._rcbMetadataRepository.get_value();
            return selectedValue;
        };
        uscMetadataRepositorySel.prototype.clearComboboxText = function () {
            if (this._rcbMetadataRepository.get_text()) {
                this._rcbMetadataRepository.set_text("");
            }
        };
        uscMetadataRepositorySel.prototype.setRepositoryRestrictions = function (repositoryIds) {
            this._rcbMetadataRepository.clearItems();
            this._rcbMetadataRepository.get_attributes().setAttribute("repositoryRestrictions", JSON.stringify(repositoryIds));
        };
        uscMetadataRepositorySel.prototype.disableSelection = function () {
            this._rcbMetadataRepository.set_enabled(false);
        };
        uscMetadataRepositorySel.prototype.enableSelection = function () {
            this._rcbMetadataRepository.set_enabled(true);
        };
        uscMetadataRepositorySel.prototype.getMetadataFinderModels = function () {
            var metadataFinderModels = this._uscAdvancedSearchDynamicMetadataRest.getMetadataFinderModels();
            return metadataFinderModels;
        };
        uscMetadataRepositorySel.prototype.getMetadataFilterValues = function () {
            var metadataFilterValues = [null, [], true];
            if (this._enableAdvancedMetadataSearchBtn.checked) {
                var _a = this.getMetadataFinderModels(), metadataFinderModels = _a[0], metadataValuesAreValid = _a[1];
                metadataFilterValues[1] = metadataFinderModels;
                metadataFilterValues[2] = metadataValuesAreValid;
            }
            else {
                var metadataValueFilter = this._txtMetadataValue.get_value();
                metadataFilterValues[0] = metadataValueFilter;
            }
            return metadataFilterValues;
        };
        uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT = "onSelectedRepositoryChangeEvent";
        uscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT = "onSelectedSetiContactEvent";
        return uscMetadataRepositorySel;
    }());
    return uscMetadataRepositorySel;
});
//# sourceMappingURL=uscMetadataRepositorySel.js.map