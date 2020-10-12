/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "UserControl/uscContactSearchRest", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/ContactService", "App/DTOs/ExceptionDTO", "App/Helpers/ImageHelper", "../App/Services/Commons/ContactTitleService", "../App/Services/Commons/ContactPlaceNameService"], function (require, exports, uscContactSearchRest, ServiceConfigurationHelper, ContactService, ExceptionDTO, ImageHelper, ContactTitleService, ContactPlaceNameService) {
    var uscContactRest = /** @class */ (function () {
        function uscContactRest(serviceConfigurations) {
            var _this = this;
            this._lastSearchContactSessionKey = "";
            this._selectedContactsSessionKey = "";
            /**
            *------------------------- Events -----------------------------
            */
            this.uscContactSearchRest_selectedContact = function (eventObject, idContact) {
                _this._ajaxLoadingPanel.show(_this.pnlContentId);
                _this._contactService.getById(idContact, function (data) {
                    if (!data) {
                        return;
                    }
                    try {
                        var contact = data;
                        _this.setLastSearchedContactModel(contact);
                        _this.bindContactToPage(contact);
                        _this.setContactVisibilityBehaviour(contact);
                        _this.drawContactTree(idContact)
                            .done(function () {
                            _this._rowTreeContact().show();
                            _this.resetCollapseButton();
                            _this.setFormControlEnableState(false);
                            _this.hideToolbar();
                            _this.showMainPanel();
                        })
                            .fail(function (exception) {
                            _this.showNotificationException(exception);
                        })
                            .always(function () { return _this._ajaxLoadingPanel.hide(_this.pnlContentId); });
                    }
                    catch (error) {
                        console.error(error.message);
                        var exception = new ExceptionDTO();
                        exception.statusText = "E' avvenuto un errore durante il caricamento dei dati del contatto";
                        _this._ajaxLoadingPanel.hide(_this.pnlContentId);
                        _this.showNotificationException(exception);
                    }
                }, function (exception) {
                    _this._ajaxLoadingPanel.hide(_this.pnlContentId);
                    _this.showNotificationException(exception);
                });
            };
            this.btnCollapseInformations_onClicked = function (sender, args) {
                switch (sender.get_commandArgument()) {
                    case uscContactRest.ALL_INFORMATIONS_ARGUMENT: {
                        sender.set_commandArgument(uscContactRest.SIMPLE_INFORMATIONS_ARGUMENT);
                        sender.set_text("Altri campi");
                        break;
                    }
                    case uscContactRest.SIMPLE_INFORMATIONS_ARGUMENT: {
                        sender.set_commandArgument(uscContactRest.ALL_INFORMATIONS_ARGUMENT);
                        sender.set_text("Meno campi");
                        break;
                    }
                }
                _this.toggleMetadataVisibility();
            };
            this.btnNewContact_onClicked = function (sender, args) {
                _this.setLastSearchedContactModel(null);
                var emptyContact = { Description: "" };
                _this._rowTreeContact().hide();
                _this.bindContactToPage(emptyContact);
                _this.setContactVisibilityBehaviour(emptyContact);
                _this.resetCollapseButton();
                _this.setFormControlEnableState(true);
                _this.showToolbar();
                _this.showMainPanel();
            };
            this.rcbContactType_onSelectedIndexChanged = function (sender, args) {
                _this.setContactVisibilityBehaviour(args.get_item().get_value());
                _this.toggleMetadataVisibility();
            };
            this.rcbPersistanceType_onSelectedIndexChanged = function (sender, args) {
                _this.hideToolbarRoleElements();
                if (_this._rcbPersistanceType().get_selectedItem().get_value() == uscContactRest.RUBRICA_ADDRESS_TYPE) {
                    _this.showToolbarRoleElements();
                }
            };
            var contactServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
            this._contactService = new ContactService(contactServiceConfiguration);
            var contactTitleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "ContactTitle");
            this._contactTitleService = new ContactTitleService(contactTitleServiceConfiguration);
            var contactPlaceNameServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "ContactPlaceName");
            this._contactPlaceNameService = new ContactPlaceNameService(contactPlaceNameServiceConfiguration);
        }
        uscContactRest.prototype._pnlMain = function () {
            return $("#" + this.pnlMainId);
        };
        uscContactRest.prototype._pnlToolbar = function () {
            return $("#" + this.pnlToolbarId);
        };
        uscContactRest.prototype._rowDescription = function () {
            return $("#" + this.rowDescriptionId);
        };
        uscContactRest.prototype._rowName = function () {
            return $("#" + this.rowNameId);
        };
        uscContactRest.prototype._rowSurname = function () {
            return $("#" + this.rowSurnameId);
        };
        uscContactRest.prototype._rowBirthdate = function () {
            return $("#" + this.rowBirthdateId);
        };
        uscContactRest.prototype._rowTitle = function () {
            return $("#" + this.rowTitleId);
        };
        uscContactRest.prototype._rowCertifiedMail = function () {
            return $("#" + this.rowCertifiedMailId);
        };
        uscContactRest.prototype._rowCode = function () {
            return $("#" + this.rowCodeId);
        };
        uscContactRest.prototype._rowPiva = function () {
            return $("#" + this.rowPivaId);
        };
        uscContactRest.prototype._rowAddressType = function () {
            return $("#" + this.rowAddressTypeId);
        };
        uscContactRest.prototype._rowAddress = function () {
            return $("#" + this.rowAddressId);
        };
        uscContactRest.prototype._rowCivicNumber = function () {
            return $("#" + this.rowCivicNumberId);
        };
        uscContactRest.prototype._rowZipCode = function () {
            return $("#" + this.rowZipCodeId);
        };
        uscContactRest.prototype._rowCity = function () {
            return $("#" + this.rowCityId);
        };
        uscContactRest.prototype._rowCityCode = function () {
            return $("#" + this.rowCityCodeId);
        };
        uscContactRest.prototype._rowBirthplace = function () {
            return $("#" + this.rowBirthplaceId);
        };
        uscContactRest.prototype._rowNationality = function () {
            return $("#" + this.rowNationalityId);
        };
        uscContactRest.prototype._rowLanguage = function () {
            return $("#" + this.rowLanguageId);
        };
        uscContactRest.prototype._rowTreeContact = function () {
            return $("#" + this.rowTreeContactId);
        };
        uscContactRest.prototype._rcbPersistanceType = function () {
            var toolbarItem = this._toolbar.findItemByValue("persistanceType");
            return toolbarItem.findControl("rcbPersistanceType");
        };
        uscContactRest.prototype._rcbContactType = function () {
            var toolbarItem = this._toolbar.findItemByValue("contactType");
            return toolbarItem.findControl("rcbContactType");
        };
        uscContactRest.prototype._rcbRoleContact = function () {
            var toolbarItem = this._toolbar.findItemByValue("roleContact");
            return toolbarItem.findControl("rcbRoleContact");
        };
        /**
        *------------------------- Methods -----------------------------
        */
        uscContactRest.prototype.initialize = function () {
            var _this = this;
            this._txtDescription = $find(this.txtDescriptionId);
            this._txtName = $find(this.txtNameId);
            this._txtSurname = $find(this.txtSurnameId);
            this._rdpBirthdate = $find(this.rdpBirthdateId);
            this._txtBirthplace = $find(this.txtBirthplaceId);
            this._txtNationality = $find(this.txtNationalityId);
            this._rcbLanguage = $find(this.rcbLanguageId);
            this._rcbTitle = $find(this.rcbTitleId);
            this._txtCertifiedMail = $find(this.txtCertifiedMailId);
            this._txtCode = $find(this.txtCodeId);
            this._txtPiva = $find(this.txtPivaId);
            this._rcbAddressType = $find(this.rcbAddressTypeId);
            this._txtAddress = $find(this.txtAddressId);
            this._txtCivicNumber = $find(this.txtCivicNumberId);
            this._txtZipCode = $find(this.txtZipCodeId);
            this._txtCity = $find(this.txtCityId);
            this._txtCityCode = $find(this.txtCityCodeId);
            this._txtTelephoneNumber = $find(this.txtTelephoneNumberId);
            this._txtFAX = $find(this.txtFAXId);
            this._txtEmail = $find(this.txtEmailId);
            this._txtNote = $find(this.txtNoteId);
            this._btnCollapseInformations = $find(this.btnCollapseInformationsId);
            this._btnCollapseInformations.add_clicked(this.btnCollapseInformations_onClicked);
            this._toolbar = $find(this.toolbarId);
            this._btnNewContact = $find(this.btnNewContactId);
            this._btnNewContact.add_clicked(this.btnNewContact_onClicked);
            this._btnNewContact.set_visible(this.createManualContactEnabled);
            this._rcbContactType().add_selectedIndexChanged(this.rcbContactType_onSelectedIndexChanged);
            this._rcbPersistanceType().add_selectedIndexChanged(this.rcbPersistanceType_onSelectedIndexChanged);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._selectedContactsSessionKey = this.callerId + "_selectedContactsSessionKey";
            this.clearSelectedContactsSession();
            this._lastSearchContactSessionKey = this.clientId + "_lastSearchContactSessionKey";
            this.setLastSearchedContactModel(null);
            $("#" + this.uscContactSearchRestId).on(uscContactSearchRest.SELECTED_CONTACT_EVENT, this.uscContactSearchRest_selectedContact);
            this._ajaxLoadingPanel.show(this.pnlContentId);
            this.initializeControls();
            this.loadUserData()
                .fail(function (exception) {
                _this.showNotificationException(exception);
            })
                .always(function () {
                _this._ajaxLoadingPanel.hide(_this.pnlContentId);
                _this.bindLoaded();
            });
        };
        uscContactRest.prototype.initializeControls = function () {
            this._rowBirthplace().hide();
            this._rowNationality().hide();
            this._rowLanguage().hide();
            if (this.spidEnabeld) {
                this._rowBirthplace().show();
            }
            if (this.contactNationalityEnabled) {
                this._rowNationality().show();
                this._rowLanguage().show();
            }
        };
        uscContactRest.prototype.loadUserData = function () {
            var _this = this;
            var promise = $.Deferred();
            $.when(this.loadContactTitlesComboBox(), this.loadContactPlaceNamesComboBox())
                .done(function () { return _this.loadContactRoles()
                .done(function () { return promise.resolve(); })
                .fail(function (exception) { return promise.reject(exception); }); })
                .fail(function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscContactRest.prototype.loadContactRoles = function () {
            var _this = this;
            var promise = $.Deferred();
            this._contactService.getRoleContacts(function (data) {
                if (!data || data.length == 0) {
                    _this.hideToolbarRoleElements();
                    var toDeleteItem = _this._rcbPersistanceType().findItemByText(uscContactRest.RUBRICA_ADDRESS_TYPE);
                    _this._rcbPersistanceType().get_items().remove(toDeleteItem);
                    promise.resolve();
                    return;
                }
                var comboItem;
                for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                    var contactTitle = data_1[_i];
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_imageUrl(ImageHelper.getContactTypeImageUrl("Sector"));
                    comboItem.set_text(contactTitle.Description);
                    comboItem.set_value(contactTitle.Id.toString());
                    _this._rcbRoleContact().get_items().add(comboItem);
                }
                if (data.length == 1) {
                    _this._rcbRoleContact().get_items().getItem(0).select();
                    _this.hideToolbarRoleElements();
                }
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscContactRest.prototype.hideToolbarRoleElements = function () {
            this._toolbar.findItemByValue("roleContact").hide();
            this._toolbar.findItemByValue("roleContactLabel").hide();
            this._toolbar.findItemByValue("roleContactSeparator").hide();
        };
        uscContactRest.prototype.showToolbarRoleElements = function () {
            this._toolbar.findItemByValue("roleContact").show();
            this._toolbar.findItemByValue("roleContactLabel").show();
            this._toolbar.findItemByValue("roleContactSeparator").show();
        };
        uscContactRest.prototype.loadContactTitlesComboBox = function () {
            var _this = this;
            var promise = $.Deferred();
            this._contactTitleService.getAll(function (data) {
                if (!data) {
                    promise.resolve();
                    return;
                }
                var contactTitles = data;
                var comboItem;
                for (var _i = 0, contactTitles_1 = contactTitles; _i < contactTitles_1.length; _i++) {
                    var contactTitle = contactTitles_1[_i];
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(contactTitle.Description);
                    comboItem.set_value(contactTitle.EntityId.toString());
                    _this._rcbContactType().get_items().add(comboItem);
                }
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscContactRest.prototype.loadContactPlaceNamesComboBox = function () {
            var _this = this;
            var promise = $.Deferred();
            this._contactPlaceNameService.getAll(function (data) {
                if (!data) {
                    promise.resolve();
                    return;
                }
                var contactPlaceNames = data;
                var comboItem;
                for (var _i = 0, contactPlaceNames_1 = contactPlaceNames; _i < contactPlaceNames_1.length; _i++) {
                    var contactPlaceName = contactPlaceNames_1[_i];
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(contactPlaceName.Description);
                    comboItem.set_value(contactPlaceName.EntityShortId.toString());
                    _this._rcbAddressType.get_items().add(comboItem);
                }
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscContactRest.prototype.bindLoaded = function () {
            $("#" + this.pnlContentId).data(this);
            $("#" + this.pnlContentId).triggerHandler(uscContactSearchRest.LOADED_EVENT);
        };
        uscContactRest.prototype.bindContactToPage = function (contact) {
            this._txtName.clear();
            this._txtSurname.clear();
            if (contact.Description.indexOf("|")) {
                var fullNameSplitted = contact.Description.split("|");
                this._txtName.set_value(fullNameSplitted[0]);
                this._txtSurname.set_value(fullNameSplitted[1]);
            }
            this._txtDescription.set_value(contact.Description.replace("|", " "));
            this._rdpBirthdate.clear();
            if (contact.BirthDate) {
                this._rdpBirthdate.set_selectedDate(new Date(contact.BirthDate));
            }
            this._txtCertifiedMail.set_value(contact.CertifiedMail);
            this._txtCode.set_value(contact.SearchCode);
            this._txtPiva.set_value(contact.FiscalCode);
            this._txtAddress.set_value(contact.Address);
            this._txtCivicNumber.set_value(contact.CivicNumber);
            this._txtZipCode.set_value(contact.ZipCode);
            this._txtCity.set_value(contact.City);
            this._txtCityCode.set_value(contact.CityCode);
            this._txtTelephoneNumber.set_value(contact.TelephoneNumber);
            this._txtFAX.set_value(contact.FaxNumber);
            this._txtEmail.set_value(contact.EmailAddress);
            this._txtNote.set_value(contact.Note);
            this._txtBirthplace.set_value(contact.BirthPlace);
            this._txtNationality.set_value(contact.Nationality);
            this._rcbLanguage.clearSelection();
            if (contact.Language) {
                var toSelectedItem = this._rcbLanguage.findItemByValue(contact.Language.toString());
                if (toSelectedItem) {
                    toSelectedItem.select();
                }
            }
            this._rcbTitle.clearSelection();
            if (contact.Title) {
                var toSelectedItem = this._rcbTitle.findItemByValue(contact.Title.EntityId.toString());
                if (toSelectedItem) {
                    toSelectedItem.select();
                }
            }
            this._rcbAddressType.clearSelection();
            if (contact.PlaceName) {
                var toSelectedItem = this._rcbAddressType.findItemByValue(contact.PlaceName.EntityShortId.toString());
                if (toSelectedItem) {
                    toSelectedItem.select();
                }
            }
            var item = this._rcbPersistanceType().findItemByValue(uscContactRest.RUBRICA_ADDRESS_TYPE);
            if (item) {
                item.select();
            }
            else {
                var mitem = this._rcbPersistanceType().findItemByValue(uscContactRest.MANUALE_ADDRESS_TYPE);
                mitem.select();
            }
            switch (contact.IdContactType) {
                case uscContactRest.PERSONA_CONTACT_TYPE: {
                    var item_1 = this._rcbContactType().findItemByValue(uscContactRest.PERSONA_CONTACT_TYPE);
                    item_1.select();
                    break;
                }
                default: {
                    var item_2 = this._rcbContactType().findItemByValue(uscContactRest.AZIENDA_CONTACT_TYPE);
                    item_2.select();
                }
            }
        };
        uscContactRest.prototype.resetCollapseButton = function () {
            this._btnCollapseInformations.set_commandArgument(uscContactRest.SIMPLE_INFORMATIONS_ARGUMENT);
            this._btnCollapseInformations.set_text("Altri campi");
            this.toggleMetadataVisibility();
        };
        uscContactRest.prototype.toggleAction = function (control) {
            var toExpand = this._btnCollapseInformations.get_commandArgument() == uscContactRest.ALL_INFORMATIONS_ARGUMENT;
            control.hide();
            if (toExpand) {
                control.show();
            }
        };
        uscContactRest.prototype.toggleMetadataVisibility = function () {
            if (this.isCitizenContact()) {
                this.toggleAction(this._rowBirthdate());
                this.toggleAction(this._rowTitle());
                if (this.spidEnabeld) {
                    this.toggleAction(this._rowBirthplace());
                }
            }
            this.toggleAction(this._rowCode());
            this.toggleAction(this._rowPiva());
            this.toggleAction(this._rowAddressType());
            this.toggleAction(this._rowAddress());
            this.toggleAction(this._rowCivicNumber());
            this.toggleAction(this._rowZipCode());
            this.toggleAction(this._rowCity());
            this.toggleAction(this._rowCityCode());
            if (this.contactNationalityEnabled) {
                this.toggleAction(this._rowNationality());
                this.toggleAction(this._rowLanguage());
            }
        };
        uscContactRest.prototype.isCitizenContact = function () {
            var lastSearchedContact = this.getLastSearchedContactModel();
            if (lastSearchedContact) {
                return lastSearchedContact.IdContactType == uscContactRest.PERSONA_CONTACT_TYPE;
            }
            return this._rcbContactType().get_selectedItem().get_value() == uscContactRest.PERSONA_CONTACT_TYPE;
        };
        uscContactRest.prototype.setContactVisibilityBehaviour = function (contactOrContactType) {
            this._rowName().hide();
            this._rowSurname().hide();
            this._rowDescription().hide();
            this._rowBirthdate().hide();
            this._rowBirthplace().hide();
            this._rowTitle().hide();
            var contactType = "";
            if (typeof contactOrContactType == "string") {
                contactType = contactOrContactType;
            }
            else {
                contactType = contactOrContactType.IdContactType;
            }
            switch (contactType) {
                case uscContactRest.PERSONA_CONTACT_TYPE: {
                    this._rowName().show();
                    this._rowSurname().show();
                    this._rowBirthdate().show();
                    if (this.spidEnabeld) {
                        this._rowBirthplace().show();
                    }
                    this._rowTitle().show();
                    break;
                }
                default: {
                    this._rowDescription().show();
                }
            }
        };
        uscContactRest.prototype.setFormControlEnableState = function (state) {
            var changeStateAction = function (control) {
                if (control instanceof Telerik.Web.UI.RadTextBox) {
                    control.disable();
                    if (state) {
                        control.enable();
                    }
                }
                else {
                    control.set_enabled(state);
                }
            };
            changeStateAction(this._txtName);
            changeStateAction(this._txtSurname);
            changeStateAction(this._txtDescription);
            changeStateAction(this._rdpBirthdate);
            changeStateAction(this._txtCertifiedMail);
            changeStateAction(this._txtCode);
            changeStateAction(this._txtPiva);
            changeStateAction(this._txtAddress);
            changeStateAction(this._txtCivicNumber);
            changeStateAction(this._txtZipCode);
            changeStateAction(this._txtCity);
            changeStateAction(this._txtCityCode);
            changeStateAction(this._txtTelephoneNumber);
            changeStateAction(this._txtFAX);
            changeStateAction(this._txtEmail);
            changeStateAction(this._txtNote);
            changeStateAction(this._rcbAddressType);
            changeStateAction(this._rcbTitle);
        };
        uscContactRest.prototype.drawContactTree = function (idContact) {
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
                    labelControlHtml = "<span style=\"vertical-align: middle;\"> " + data[i].Description + "</span>";
                    treeListHtml = treeListHtml + "<li>" + imageControlHtml + labelControlHtml + "</li>";
                }
                treeListHtml = treeListHtml + "</ul>";
                $("#treeContact").html(treeListHtml);
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscContactRest.prototype.createContact = function () {
            var _this = this;
            var promise = $.Deferred();
            var lastSearchedContact = this.getLastSearchedContactModel();
            if (lastSearchedContact) {
                this.addSelectedContactToSession(lastSearchedContact);
                return promise.resolve([uscContactRest.RUBRICA_ADDRESS_TYPE, lastSearchedContact]);
            }
            try {
                var contactFromPage_1 = this.prepareContactModel();
                if (!contactFromPage_1.Description) {
                    return promise.reject("E' necessario definire una descrizione per il contatto");
                }
                var contactAction = function () { return $.Deferred().resolve(contactFromPage_1).promise(); };
                if (this._rcbPersistanceType().get_selectedItem().get_value() == uscContactRest.RUBRICA_ADDRESS_TYPE) {
                    if (!this._rcbRoleContact().get_selectedItem()) {
                        return promise.reject("E' necessario selezionare un contatto di rubrica che conterrà il nuovo contatto");
                    }
                    contactFromPage_1.IncrementalFather = +this._rcbRoleContact().get_selectedItem().get_value();
                    contactAction = function () {
                        var promise = $.Deferred();
                        _this._contactService.insertContact(contactFromPage_1, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
                        return promise.promise();
                    };
                }
                contactAction()
                    .done(function (contact) {
                    _this.addSelectedContactToSession(contact);
                    promise.resolve([_this._rcbPersistanceType().get_selectedItem().get_value(), contact]);
                })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            catch (error) {
                console.error(error.message);
                var exception = new ExceptionDTO();
                exception.statusText = "E' avvenuto un errore durante la fase di gestione del contatto";
                return promise.reject(exception);
            }
            return promise.promise();
        };
        uscContactRest.prototype.prepareContactModel = function () {
            var contact = {};
            contact.Description = this._txtDescription.get_value();
            contact.IdContactType = "A";
            if (this.isCitizenContact()) {
                contact.IdContactType = "P";
                if (this._txtName.get_value() || this._txtSurname.get_value()) {
                    contact.Description = this._txtName.get_value() + "|" + this._txtSurname.get_value();
                }
                if (this._rdpBirthdate.get_selectedDate()) {
                    contact.BirthDate = moment(this._rdpBirthdate.get_selectedDate()).endOf("day").toDate();
                }
            }
            contact.isActive = 1;
            contact.BirthPlace = this._txtBirthplace.get_value();
            contact.Nationality = this._txtNationality.get_value();
            contact.CertifiedMail = this._txtCertifiedMail.get_value();
            contact.SearchCode = this._txtCode.get_value();
            contact.FiscalCode = this._txtPiva.get_value();
            contact.Address = this._txtAddress.get_value();
            contact.CivicNumber = this._txtCivicNumber.get_value();
            contact.ZipCode = this._txtZipCode.get_value();
            contact.City = this._txtCity.get_value();
            contact.CityCode = this._txtCityCode.get_value();
            contact.TelephoneNumber = this._txtTelephoneNumber.get_value();
            contact.FaxNumber = this._txtFAX.get_value();
            contact.EmailAddress = this._txtEmail.get_value();
            contact.Note = this._txtNote.get_value();
            if (this._rcbLanguage.get_selectedItem()) {
                contact.Language = this._rcbLanguage.get_selectedItem().get_value();
            }
            if (this._rcbTitle.get_selectedItem()) {
                contact.Title = {};
                contact.Title.EntityId = +this._rcbTitle.get_selectedItem().get_value();
            }
            if (this._rcbAddressType.get_selectedItem()) {
                contact.PlaceName = {};
                contact.PlaceName.EntityShortId = +this._rcbAddressType.get_selectedItem().get_value();
            }
            return contact;
        };
        uscContactRest.prototype.showNotificationException = function (exception, customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception) {
                    uscNotification.showNotification(exception);
                    return;
                }
                uscNotification.showWarningMessage(customMessage);
            }
        };
        uscContactRest.prototype.getLastSearchedContactModel = function () {
            var sessionItem = sessionStorage.getItem(this._lastSearchContactSessionKey);
            if (sessionItem) {
                return JSON.parse(sessionItem);
            }
            return null;
        };
        uscContactRest.prototype.setLastSearchedContactModel = function (value) {
            sessionStorage.removeItem(this._lastSearchContactSessionKey);
            if (value) {
                sessionStorage.setItem(this._lastSearchContactSessionKey, JSON.stringify(value));
            }
        };
        uscContactRest.prototype.clear = function () {
            var uscContactSearchRest = $("#" + this.uscContactSearchRestId).data();
            if (!jQuery.isEmptyObject(uscContactSearchRest)) {
                uscContactSearchRest.clear();
            }
            this.setLastSearchedContactModel(null);
            this.resetCollapseButton();
            this.hideToolbar();
            this.hideMainPanel();
            this._rcbRoleContact().clearSelection();
        };
        uscContactRest.prototype.showMainPanel = function () {
            this._pnlMain().show();
        };
        uscContactRest.prototype.hideMainPanel = function () {
            this._pnlMain().hide();
        };
        uscContactRest.prototype.showToolbar = function () {
            this._pnlToolbar().show();
        };
        uscContactRest.prototype.hideToolbar = function () {
            this._pnlToolbar().hide();
        };
        uscContactRest.prototype.addSelectedContactToSession = function (contact) {
            var sessionItem = sessionStorage.getItem(this._selectedContactsSessionKey);
            var contacts = [];
            if (sessionItem) {
                contacts = JSON.parse(sessionItem);
            }
            contacts.push(contact);
            sessionStorage.setItem(this._selectedContactsSessionKey, JSON.stringify(contacts));
        };
        uscContactRest.prototype.clearSelectedContactsSession = function () {
            sessionStorage.removeItem(this._selectedContactsSessionKey);
        };
        uscContactRest.ALL_INFORMATIONS_ARGUMENT = "A";
        uscContactRest.SIMPLE_INFORMATIONS_ARGUMENT = "S";
        uscContactRest.RUBRICA_ADDRESS_TYPE = "Rubrica";
        uscContactRest.MANUALE_ADDRESS_TYPE = "Manuale";
        uscContactRest.AZIENDA_CONTACT_TYPE = "A";
        uscContactRest.PERSONA_CONTACT_TYPE = "P";
        return uscContactRest;
    }());
    return uscContactRest;
});
//# sourceMappingURL=uscContactREST.js.map