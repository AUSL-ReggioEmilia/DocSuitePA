define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/MetadataRepositoryService", "App/Models/Commons/MetadataRepositoryModel", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, MetadataRepositoryService, MetadataRepositoryModel, ExceptionDTO) {
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
                    _this._metadataRepositoryService.getAvailableMetadataRepositories(sender.get_text(), metadataRestrictions, _this.maxNumberElements, numberOfItems, function (data) {
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
                if (selectedItem) {
                    $("#".concat(_this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_INDEX_EVENT, selectedItem.get_value());
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
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
            var scrollContainer = $(this._rcbMetadataRepository.get_dropDownElement()).find('div.rcbScroll');
            $(scrollContainer).scroll(this.rcbLookup_onScroll);
            $("#".concat(this.metadataPageContentId)).data(this);
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
        uscMetadataRepositorySel.prototype.setComboboxText = function (id) {
            var _this = this;
            this._metadataRepositoryService.getById(id, function (data) {
                if (data) {
                    _this._rcbMetadataRepository.set_text(data.Name);
                    _this._rcbMetadataRepository.set_value(data.UniqueId);
                    $("#".concat(_this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_INDEX_EVENT, data.UniqueId);
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
        uscMetadataRepositorySel.SELECTED_INDEX_EVENT = "onSelectedIndexChangeEvent";
        return uscMetadataRepositorySel;
    }());
    return uscMetadataRepositorySel;
});
//# sourceMappingURL=uscMetadataRepositorySel.js.map