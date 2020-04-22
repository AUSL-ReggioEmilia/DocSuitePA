/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import uscContactSearchRest = require("UserControl/uscContactSearchRest");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ContactService = require("App/Services/Commons/ContactService");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ContactModel = require("App/Models/Commons/ContactModel");
import ImageHelper = require("App/Helpers/ImageHelper");
import uscErrorNotification = require("UserControl/uscErrorNotification");
import ContactTitleService = require("../App/Services/Commons/ContactTitleService");
import ContactPlaceNameService = require("../App/Services/Commons/ContactPlaceNameService");
import ContactTitleModel = require("../App/Models/Commons/ContactTitleModel");
import ContactPlaceNameModel = require("../App/Models/Commons/ContactPlaceNameModel");
import ValidationExceptionDTO = require("../App/DTOs/ValidationExceptionDTO");

class uscContactRest {
    uscContactSearchRestId: string;
    rowNameId: string;
    txtNameId: string;
    rowSurnameId: string;
    txtSurnameId: string;
    rowDescriptionId: string;
    txtDescriptionId: string;
    rowBirthdateId: string;
    rdpBirthdateId: string;
    rowTitleId: string;
    rcbTitleId: string;
    rowCertifiedMailId: string;
    txtCertifiedMailId: string;
    rowCodeId: string;
    txtCodeId: string;
    rowPivaId: string;
    txtPivaId: string;
    rowAddressTypeId: string;
    rcbAddressTypeId: string;
    rowAddressId: string;
    txtAddressId: string;
    rowCivicNumberId: string;
    txtCivicNumberId: string;
    rowZipCodeId: string;
    txtZipCodeId: string;
    rowCityId: string;
    txtCityId: string;
    rowCityCodeId: string;
    txtCityCodeId: string;
    rowTelephoneNumberId: string;
    txtTelephoneNumberId: string;
    rowFAXId: string;
    txtFAXId: string;
    rowEmailId: string;
    txtEmailId: string;
    rowNoteId: string;
    txtNoteId: string;
    rowBirthplaceId: string;
    txtBirthplaceId: string;
    rowNationalityId: string;
    txtNationalityId: string;
    rowLanguageId: string;
    rcbLanguageId: string;
    pnlMainId: string;
    btnCollapseInformationsId: string;
    toolbarId: string;
    btnNewContactId: string;
    ajaxLoadingPanelId: string;
    pnlContentId: string;
    pnlToolbarId: string;
    rowTreeContactId: string;
    uscNotificationId: string;
    clientId: string;
    spidEnabeld: boolean;
    contactNationalityEnabled: boolean;
    callerId: string;

    private _contactService: ContactService;
    private _contactTitleService: ContactTitleService;
    private _contactPlaceNameService: ContactPlaceNameService;
    private _txtDescription: Telerik.Web.UI.RadTextBox;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtSurname: Telerik.Web.UI.RadTextBox;
    private _rdpBirthdate: Telerik.Web.UI.RadDatePicker;
    private _txtBirthplace: Telerik.Web.UI.RadTextBox;
    private _txtNationality: Telerik.Web.UI.RadTextBox;
    private _rcbLanguage: Telerik.Web.UI.RadComboBox;
    private _rcbTitle: Telerik.Web.UI.RadComboBox;
    private _txtCertifiedMail: Telerik.Web.UI.RadTextBox;
    private _txtCode: Telerik.Web.UI.RadTextBox;
    private _txtPiva: Telerik.Web.UI.RadTextBox;
    private _rcbAddressType: Telerik.Web.UI.RadComboBox;
    private _txtAddress: Telerik.Web.UI.RadTextBox;
    private _txtCivicNumber: Telerik.Web.UI.RadTextBox;
    private _txtZipCode: Telerik.Web.UI.RadTextBox;
    private _txtCity: Telerik.Web.UI.RadTextBox;
    private _txtCityCode: Telerik.Web.UI.RadTextBox;
    private _txtTelephoneNumber: Telerik.Web.UI.RadTextBox;
    private _txtFAX: Telerik.Web.UI.RadTextBox;
    private _txtEmail: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _btnCollapseInformations: Telerik.Web.UI.RadButton;
    private _toolbar: Telerik.Web.UI.RadToolBar;
    private _btnNewContact: Telerik.Web.UI.RadButton;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;    

    private get _pnlMain(): JQuery {
        return $(`#${this.pnlMainId}`);
    }

    private get _pnlToolbar(): JQuery {
        return $(`#${this.pnlToolbarId}`);
    }

    private get _rowDescription(): JQuery {
        return $(`#${this.rowDescriptionId}`);
    }

    private get _rowName(): JQuery {
        return $(`#${this.rowNameId}`);
    }

    private get _rowSurname(): JQuery {
        return $(`#${this.rowSurnameId}`);
    }

    private get _rowBirthdate(): JQuery {
        return $(`#${this.rowBirthdateId}`);
    }

    private get _rowTitle(): JQuery {
        return $(`#${this.rowTitleId}`);
    }

    private get _rowCertifiedMail(): JQuery {
        return $(`#${this.rowCertifiedMailId}`);
    }

    private get _rowCode(): JQuery {
        return $(`#${this.rowCodeId}`);
    }

    private get _rowPiva(): JQuery {
        return $(`#${this.rowPivaId}`);
    }

    private get _rowAddressType(): JQuery {
        return $(`#${this.rowAddressTypeId}`);
    }

    private get _rowAddress(): JQuery {
        return $(`#${this.rowAddressId}`);
    }

    private get _rowCivicNumber(): JQuery {
        return $(`#${this.rowCivicNumberId}`);
    }

    private get _rowZipCode(): JQuery {
        return $(`#${this.rowZipCodeId}`);
    }

    private get _rowCity(): JQuery {
        return $(`#${this.rowCityId}`);
    }

    private get _rowCityCode(): JQuery {
        return $(`#${this.rowCityCodeId}`);
    }

    private get _rowBirthplace(): JQuery {
        return $(`#${this.rowBirthplaceId}`);
    }

    private get _rowNationality(): JQuery {
        return $(`#${this.rowNationalityId}`);
    }

    private get _rowLanguage(): JQuery {
        return $(`#${this.rowLanguageId}`);
    }

    private get _rowTreeContact(): JQuery {
        return $(`#${this.rowTreeContactId}`);
    }

    private get _rcbPersistanceType(): Telerik.Web.UI.RadComboBox {
        let toolbarItem: Telerik.Web.UI.RadToolBarItem = this._toolbar.findItemByValue("persistanceType");
        return toolbarItem.findControl("rcbPersistanceType") as Telerik.Web.UI.RadComboBox;
    }

    private get _rcbContactType(): Telerik.Web.UI.RadComboBox {
        let toolbarItem: Telerik.Web.UI.RadToolBarItem = this._toolbar.findItemByValue("contactType");
        return toolbarItem.findControl("rcbContactType") as Telerik.Web.UI.RadComboBox;
    }

    private get _rcbRoleContact(): Telerik.Web.UI.RadComboBox {
        let toolbarItem: Telerik.Web.UI.RadToolBarItem = this._toolbar.findItemByValue("roleContact");
        return toolbarItem.findControl("rcbRoleContact") as Telerik.Web.UI.RadComboBox;
    }

    private static ALL_INFORMATIONS_ARGUMENT: string = "A";
    private static SIMPLE_INFORMATIONS_ARGUMENT: string = "S";
    private static RUBRICA_ADDRESS_TYPE: string = "Rubrica";
    private static MANUALE_ADDRESS_TYPE: string = "Manuale";
    private static AZIENDA_CONTACT_TYPE: string = "A";
    private static PERSONA_CONTACT_TYPE: string = "P";
    private _lastSearchContactSessionKey: string = "";
    private _selectedContactsSessionKey: string = "";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let contactServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
        this._contactService = new ContactService(contactServiceConfiguration);

        let contactTitleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "ContactTitle");
        this._contactTitleService = new ContactTitleService(contactTitleServiceConfiguration);

        let contactPlaceNameServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "ContactPlaceName");
        this._contactPlaceNameService= new ContactPlaceNameService(contactPlaceNameServiceConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */
    uscContactSearchRest_selectedContact = (eventObject: JQueryEventObject, idContact: number) => {
        this._ajaxLoadingPanel.show(this.pnlContentId);
        this._contactService.getById(idContact,
            (data: any) => {
                if (!data) {
                    return;
                }

                try {
                    let contact: ContactModel = data as ContactModel;
                    this.setLastSearchedContactModel(contact);
                    this.bindContactToPage(contact);
                    this.setContactVisibilityBehaviour(contact);                    
                    this.drawContactTree(idContact)
                        .done(() => {
                            this._rowTreeContact.show();
                            this.resetCollapseButton();
                            this.setFormControlEnableState(false);
                            this.hideToolbar();
                            this.showMainPanel();
                        })
                        .fail((exception: ExceptionDTO) => {                            
                            this.showNotificationException(exception);                            
                        })
                        .always(() => this._ajaxLoadingPanel.hide(this.pnlContentId));
                } catch (error) {
                    console.error(error.message);
                    let exception: ExceptionDTO = new ExceptionDTO();
                    exception.statusText = "E' avvenuto un errore durante il caricamento dei dati del contatto";
                    this._ajaxLoadingPanel.hide(this.pnlContentId);
                    this.showNotificationException(exception);
                }                       
            },
            (exception: ExceptionDTO) => {
                this._ajaxLoadingPanel.hide(this.pnlContentId);
                this.showNotificationException(exception);                
            }
        )
    }

    btnCollapseInformations_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
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
        this.toggleMetadataVisibility();
    }

    btnNewContact_onClicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.setLastSearchedContactModel(null);
        let emptyContact: ContactModel = {Description: ""} as ContactModel;
        this._rowTreeContact.hide();
        this.bindContactToPage(emptyContact);        
        this.setContactVisibilityBehaviour(emptyContact);            
        this.resetCollapseButton();
        this.setFormControlEnableState(true);
        this.showToolbar();
        this.showMainPanel();
    }

    rcbContactType_onSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.setContactVisibilityBehaviour(args.get_item().get_value());
        this.toggleMetadataVisibility();
    }

    rcbPersistanceType_onSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.hideToolbarRoleElements();
        if (this._rcbPersistanceType.get_selectedItem().get_value() == uscContactRest.RUBRICA_ADDRESS_TYPE) {
            this.showToolbarRoleElements();
        }
    }

    /**
    *------------------------- Methods -----------------------------
    */
    initialize(): void {
        this._txtDescription = $find(this.txtDescriptionId) as Telerik.Web.UI.RadTextBox;
        this._txtName = $find(this.txtNameId) as Telerik.Web.UI.RadTextBox;
        this._txtSurname = $find(this.txtSurnameId) as Telerik.Web.UI.RadTextBox;
        this._rdpBirthdate = $find(this.rdpBirthdateId) as Telerik.Web.UI.RadDatePicker;
        this._txtBirthplace = $find(this.txtBirthplaceId) as Telerik.Web.UI.RadTextBox;
        this._txtNationality = $find(this.txtNationalityId) as Telerik.Web.UI.RadTextBox;
        this._rcbLanguage = $find(this.rcbLanguageId) as Telerik.Web.UI.RadComboBox;
        this._rcbTitle = $find(this.rcbTitleId) as Telerik.Web.UI.RadComboBox;
        this._txtCertifiedMail = $find(this.txtCertifiedMailId) as Telerik.Web.UI.RadTextBox;
        this._txtCode = $find(this.txtCodeId) as Telerik.Web.UI.RadTextBox;
        this._txtPiva = $find(this.txtPivaId) as Telerik.Web.UI.RadTextBox;
        this._rcbAddressType = $find(this.rcbAddressTypeId) as Telerik.Web.UI.RadComboBox;
        this._txtAddress = $find(this.txtAddressId) as Telerik.Web.UI.RadTextBox;
        this._txtCivicNumber = $find(this.txtCivicNumberId) as Telerik.Web.UI.RadTextBox;
        this._txtZipCode = $find(this.txtZipCodeId) as Telerik.Web.UI.RadTextBox;
        this._txtCity = $find(this.txtCityId) as Telerik.Web.UI.RadTextBox;
        this._txtCityCode = $find(this.txtCityCodeId) as Telerik.Web.UI.RadTextBox;
        this._txtTelephoneNumber = $find(this.txtTelephoneNumberId) as Telerik.Web.UI.RadTextBox;
        this._txtFAX = $find(this.txtFAXId) as Telerik.Web.UI.RadTextBox;
        this._txtEmail = $find(this.txtEmailId) as Telerik.Web.UI.RadTextBox;
        this._txtNote = $find(this.txtNoteId) as Telerik.Web.UI.RadTextBox;
        this._btnCollapseInformations = $find(this.btnCollapseInformationsId) as Telerik.Web.UI.RadButton;
        this._btnCollapseInformations.add_clicked(this.btnCollapseInformations_onClicked);
        this._toolbar = $find(this.toolbarId) as Telerik.Web.UI.RadToolBar;
        this._btnNewContact = $find(this.btnNewContactId) as Telerik.Web.UI.RadButton;
        this._btnNewContact.add_clicked(this.btnNewContact_onClicked);
        this._rcbContactType.add_selectedIndexChanged(this.rcbContactType_onSelectedIndexChanged);
        this._rcbPersistanceType.add_selectedIndexChanged(this.rcbPersistanceType_onSelectedIndexChanged);
        this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;

        this._selectedContactsSessionKey = `${this.callerId}_selectedContactsSessionKey`;
        this.clearSelectedContactsSession();
        this._lastSearchContactSessionKey = `${this.clientId}_lastSearchContactSessionKey`;
        this.setLastSearchedContactModel(null);

        $(`#${this.uscContactSearchRestId}`).on(uscContactSearchRest.SELECTED_CONTACT_EVENT, this.uscContactSearchRest_selectedContact);

        this._ajaxLoadingPanel.show(this.pnlContentId);        
        this.initializeControls();
        this.loadUserData()
            .fail((exception: ExceptionDTO) => {
                this.showNotificationException(exception);
            })
            .always(() => {
                this._ajaxLoadingPanel.hide(this.pnlContentId);
                this.bindLoaded();
            });        
    }

    private initializeControls(): void {
        this._rowBirthplace.hide();
        this._rowNationality.hide();
        this._rowLanguage.hide();
        if (this.spidEnabeld) {
            this._rowBirthplace.show();
        }
        if (this.contactNationalityEnabled) {
            this._rowNationality.show();
            this._rowLanguage.show();
        }
    }

    private loadUserData(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        $.when(this.loadContactTitlesComboBox(), this.loadContactPlaceNamesComboBox())
            .done(() => this.loadContactRoles()
                .done(() => promise.resolve())
                .fail((exception: ExceptionDTO) => promise.reject(exception)))
            .fail((exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    private loadContactRoles(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._contactService.getRoleContacts(
            (data: any) => {
                if (!data || data.length == 0) {
                    this.hideToolbarRoleElements();
                    let toDeleteItem: Telerik.Web.UI.RadComboBoxItem = this._rcbPersistanceType.findItemByText(uscContactRest.RUBRICA_ADDRESS_TYPE);
                    this._rcbPersistanceType.get_items().remove(toDeleteItem);
                    promise.resolve();
                    return;
                }

                let comboItem: Telerik.Web.UI.RadComboBoxItem
                for (let contactTitle of data) {
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_imageUrl(ImageHelper.getContactTypeImageUrl("Sector"))
                    comboItem.set_text(contactTitle.Description);
                    comboItem.set_value(contactTitle.Id.toString());
                    this._rcbRoleContact.get_items().add(comboItem);
                }

                if (data.length == 1) {
                    this._rcbRoleContact.get_items().getItem(0).select();
                    this.hideToolbarRoleElements();
                }
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private hideToolbarRoleElements(): void {
        this._toolbar.findItemByValue("roleContact").hide();
        this._toolbar.findItemByValue("roleContactLabel").hide();
        this._toolbar.findItemByValue("roleContactSeparator").hide();
    }

    private showToolbarRoleElements(): void {
        this._toolbar.findItemByValue("roleContact").show();
        this._toolbar.findItemByValue("roleContactLabel").show();
        this._toolbar.findItemByValue("roleContactSeparator").show();
    }

    private loadContactTitlesComboBox(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._contactTitleService.getAll(
            (data: any) => {
                if (!data) {
                    promise.resolve();
                    return;
                }

                let contactTitles: ContactTitleModel[] = data;
                let comboItem: Telerik.Web.UI.RadComboBoxItem
                for (let contactTitle of contactTitles) {
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(contactTitle.Description);
                    comboItem.set_value(contactTitle.EntityId.toString());
                    this._rcbContactType.get_items().add(comboItem);
                }                
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private loadContactPlaceNamesComboBox(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._contactPlaceNameService.getAll(
            (data: any) => {
                if (!data) {
                    promise.resolve();
                    return;
                }

                let contactPlaceNames: ContactPlaceNameModel[] = data;
                let comboItem: Telerik.Web.UI.RadComboBoxItem
                for (let contactPlaceName of contactPlaceNames) {
                    comboItem = new Telerik.Web.UI.RadComboBoxItem();
                    comboItem.set_text(contactPlaceName.Description);
                    comboItem.set_value(contactPlaceName.EntityShortId.toString());
                    this._rcbAddressType.get_items().add(comboItem);
                }
                promise.resolve();
            },
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private bindLoaded(): void {
        $(`#${this.pnlContentId}`).data(this);
        $(`#${this.pnlContentId}`).triggerHandler(uscContactSearchRest.LOADED_EVENT);
    }

    private bindContactToPage(contact: ContactModel): void {
        this._txtName.clear();
        this._txtSurname.clear();
        if (contact.Description.indexOf("|")) {
            let fullNameSplitted: string[] = contact.Description.split("|");
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
            let toSelectedItem: Telerik.Web.UI.RadComboBoxItem = this._rcbLanguage.findItemByValue(contact.Language.toString());
            if (toSelectedItem) {
                toSelectedItem.select();
            }
        }

        this._rcbTitle.clearSelection();
        if (contact.Title) {
            let toSelectedItem: Telerik.Web.UI.RadComboBoxItem = this._rcbTitle.findItemByValue(contact.Title.EntityId.toString());
            if (toSelectedItem) {
                toSelectedItem.select();
            }
        }

        this._rcbAddressType.clearSelection();
        if (contact.PlaceName) {
            let toSelectedItem: Telerik.Web.UI.RadComboBoxItem = this._rcbAddressType.findItemByValue(contact.PlaceName.EntityShortId.toString());
            if (toSelectedItem) {
                toSelectedItem.select();
            }
        }

        let item: Telerik.Web.UI.RadComboBoxItem = this._rcbPersistanceType.findItemByValue(uscContactRest.RUBRICA_ADDRESS_TYPE);
        item.select();
        switch (contact.IdContactType) {
            case uscContactRest.PERSONA_CONTACT_TYPE: {                
                let item: Telerik.Web.UI.RadComboBoxItem = this._rcbContactType.findItemByValue(uscContactRest.PERSONA_CONTACT_TYPE);
                item.select();
                break;
            }
            default: {
                let item: Telerik.Web.UI.RadComboBoxItem = this._rcbContactType.findItemByValue(uscContactRest.AZIENDA_CONTACT_TYPE);
                item.select();
            }
        }
    }

    private resetCollapseButton(): void {
        this._btnCollapseInformations.set_commandArgument(uscContactRest.SIMPLE_INFORMATIONS_ARGUMENT);
        this._btnCollapseInformations.set_text("Altri campi");
        this.toggleMetadataVisibility();
    }

    private toggleMetadataVisibility(): void {
        let toExpand: boolean = this._btnCollapseInformations.get_commandArgument() == uscContactRest.ALL_INFORMATIONS_ARGUMENT;
        let toggleAction: Function = (control: JQuery) => {
            control.hide();
            if (toExpand) {
                control.show();
            }
        };

        if (this.isCitizenContact()) {
            toggleAction(this._rowBirthdate);
            toggleAction(this._rowTitle);
            if (this.spidEnabeld) {
                toggleAction(this._rowBirthplace);
            }            
        }        
        toggleAction(this._rowCode);
        toggleAction(this._rowPiva);
        toggleAction(this._rowAddressType);
        toggleAction(this._rowAddress);
        toggleAction(this._rowCivicNumber);
        toggleAction(this._rowZipCode);
        toggleAction(this._rowCity);
        toggleAction(this._rowCityCode);
        if (this.contactNationalityEnabled) {
            toggleAction(this._rowNationality);
            toggleAction(this._rowLanguage);
        }        
    }

    private isCitizenContact(): boolean {
        let lastSearchedContact: ContactModel = this.getLastSearchedContactModel();
        if (lastSearchedContact) {
            return lastSearchedContact.IdContactType == uscContactRest.PERSONA_CONTACT_TYPE;
        }

        return this._rcbContactType.get_selectedItem().get_value() == uscContactRest.PERSONA_CONTACT_TYPE;
    }

    private setContactVisibilityBehaviour(contact: ContactModel): void
    private setContactVisibilityBehaviour(contactType: string): void
    private setContactVisibilityBehaviour(contactOrContactType: any): void {
        this._rowName.hide();
        this._rowSurname.hide();
        this._rowDescription.hide();
        this._rowBirthdate.hide();
        this._rowBirthplace.hide();
        this._rowTitle.hide();

        let contactType: string = "";
        if (typeof contactOrContactType == "string") {
            contactType = contactOrContactType;
        } else {
            contactType = contactOrContactType.IdContactType;
        }

        switch (contactType) {
            case uscContactRest.PERSONA_CONTACT_TYPE: {
                this._rowName.show();
                this._rowSurname.show();
                this._rowBirthdate.show();
                if (this.spidEnabeld) {
                    this._rowBirthplace.show();
                }                
                this._rowTitle.show();
                break;
            }
            default: {
                this._rowDescription.show();                
            }
        }
    }

    private setFormControlEnableState(state: boolean): void {
        let changeStateAction: Function = (control: Telerik.Web.UI.RadTextBox | Telerik.Web.UI.RadDatePicker | Telerik.Web.UI.RadComboBox) => {
            if (control instanceof Telerik.Web.UI.RadTextBox) {
                control.disable();
                if (state) {
                    control.enable();
                }
            } else {
                control.set_enabled(state);
            }           
        }

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
    }

    private drawContactTree(idContact: number): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let treeListHtml: string = "<ul>";
        this._contactService.getContactParents(idContact,
            (data: any) => {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }

                let imageControlHtml: string;
                let labelControlHtml: string;
                for (var i = 0; i < data.length; i++) {
                    imageControlHtml = `<img src="${ImageHelper.getContactTypeImageUrl(data[i].ContactType)}" style="vertical-align: middle; margin-left: ${i * 20}px;"></img>`;
                    labelControlHtml = `<span style="vertical-align: middle;"> ${data[i].Description}</span>`;
                    treeListHtml = `${treeListHtml}<li>${imageControlHtml}${labelControlHtml}</li>`;
                }
                treeListHtml = `${treeListHtml}</ul>`;
                $(`#treeContact`).html(treeListHtml);
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
    }

    createContact(): JQueryPromise<[string, ContactModel]> {
        let promise: JQueryDeferred<[string, ContactModel]> = $.Deferred<[string, ContactModel]>();
        let lastSearchedContact: ContactModel = this.getLastSearchedContactModel();
        if (lastSearchedContact) {
            this.addSelectedContactToSession(lastSearchedContact);
            return promise.resolve([uscContactRest.RUBRICA_ADDRESS_TYPE, lastSearchedContact]);
        }

        try {
            let contactFromPage: ContactModel = this.prepareContactModel();
            if (!contactFromPage.Description) {
                return promise.reject("E' necessario definire una descrizione per il contatto");
            }

            let contactAction: () => JQueryPromise<ContactModel> = () => $.Deferred<ContactModel>().resolve(contactFromPage).promise();
            if (this._rcbPersistanceType.get_selectedItem().get_value() == uscContactRest.RUBRICA_ADDRESS_TYPE) {
                if (!this._rcbRoleContact.get_selectedItem()) {
                    return promise.reject("E' necessario selezionare un contatto di rubrica che conterrà il nuovo contatto");
                }

                contactFromPage.IncrementalFather = +this._rcbRoleContact.get_selectedItem().get_value();
                contactAction = () => {
                    let promise: JQueryDeferred<ContactModel> = $.Deferred<ContactModel>();
                    this._contactService.insertContact(contactFromPage,
                        (data: any) => promise.resolve(data),
                        (exception: ExceptionDTO) => promise.reject(exception)
                    )
                    return promise.promise();
                }
            }

            contactAction()
                .done((contact) => {
                    this.addSelectedContactToSession(contact);
                    promise.resolve([this._rcbPersistanceType.get_selectedItem().get_value(), contact]);
                })
                .fail((exception: ExceptionDTO) => promise.reject(exception));
        } catch (error) {
            console.error(error.message);
            let exception: ExceptionDTO = new ExceptionDTO();
            exception.statusText = "E' avvenuto un errore durante la fase di gestione del contatto";
            return promise.reject(exception);
        }

        return promise.promise();
    }

    private prepareContactModel(): ContactModel {
        let contact: ContactModel = {} as ContactModel;
        contact.Description = this._txtDescription.get_value();
        contact.IdContactType = "A";
        if (this.isCitizenContact()) {
            contact.IdContactType = "P";
            if (this._txtName.get_value() || this._txtSurname.get_value()) {
                contact.Description = `${this._txtName.get_value()}|${this._txtSurname.get_value()}`;   
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
            contact.Title = {} as ContactTitleModel;
            contact.Title.EntityId = +this._rcbTitle.get_selectedItem().get_value();
        }

        if (this._rcbAddressType.get_selectedItem()) {
            contact.PlaceName = {} as ContactPlaceNameModel;
            contact.PlaceName.EntityShortId = +this._rcbAddressType.get_selectedItem().get_value();
        }

        return contact;
    }

    protected showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        let uscNotification: uscErrorNotification = <uscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception) {
                uscNotification.showNotification(exception);
                return;
            }
            uscNotification.showWarningMessage(customMessage);
        }
    }

    public getLastSearchedContactModel(): ContactModel {
        let sessionItem: string = sessionStorage.getItem(this._lastSearchContactSessionKey);
        if (sessionItem) {
            return JSON.parse(sessionItem);
        }
        return null;
    }

    private setLastSearchedContactModel(value): void {
        sessionStorage.removeItem(this._lastSearchContactSessionKey);
        if (value) {
            sessionStorage.setItem(this._lastSearchContactSessionKey, JSON.stringify(value));
        }
    }

    clear(): void {
        let uscContactSearchRest: uscContactSearchRest = <uscContactSearchRest>$(`#${this.uscContactSearchRestId}`).data();
        if (!jQuery.isEmptyObject(uscContactSearchRest)) {
            uscContactSearchRest.clear();
        }
        this.setLastSearchedContactModel(null);
        this.resetCollapseButton();
        this.hideToolbar();
        this.hideMainPanel();
        this._rcbRoleContact.clearSelection();
    }

    private showMainPanel(): void {
        this._pnlMain.show();
    }

    private hideMainPanel(): void {
        this._pnlMain.hide();
    }

    private showToolbar(): void {
        this._pnlToolbar.show();
    }

    private hideToolbar(): void {
        this._pnlToolbar.hide();
    }

    private addSelectedContactToSession(contact: ContactModel): void {
        let sessionItem: string = sessionStorage.getItem(this._selectedContactsSessionKey);
        let contacts: ContactModel[] = [];
        if (sessionItem) {
            contacts = JSON.parse(sessionItem);
        }

        contacts.push(contact);
        sessionStorage.setItem(this._selectedContactsSessionKey, JSON.stringify(contacts));
    }

    private clearSelectedContactsSession(): void {
        sessionStorage.removeItem(this._selectedContactsSessionKey);
    }
}

export = uscContactRest;