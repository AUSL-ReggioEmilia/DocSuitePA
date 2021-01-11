import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSService = require('App/Services/UDS/UDSService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UDSLookupViewModel = require('App/ViewModels/UDS/UDSLookupViewModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class uscUDSLookup {
    UDSName: string;
    propertyName: string;
    rcbLookupId: string;
    uscNotificationId: string;
    maxNumberElements: string;
    lookupValue: string;
    checkBoxesEnabled: boolean;
    hiddenLookupId: string;
    lookupLabel: string;

    private _rcbLookup: Telerik.Web.UI.RadComboBox;
    private _serviceConfigurations: ServiceConfiguration[];
    private _udsConfiguration: ServiceConfiguration;
    private _udsService: UDSService;
    private _hiddenLookup: JQuery;

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }


    /**
   *------------------------- Events -----------------------------
   */

    /**
   * Evento scatenato allo scrolling della RadComboBox di selezione lookup
   * @param args
   */
    rcbLookup_onScroll = (args: JQueryEventObject) => {
        let element = args.target;
        if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
            this.rcbLookup_OnItemsRequesting(this._rcbLookup, new (<any>Telerik.Web.UI.RadComboBoxRequestCancelEventArgs)(args));       
        }
    }

    rcbLookup_OnItemsRequesting = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestCancelEventArgs) => {
        args.set_cancel(true);       
        setTimeout(() => {
            let numberOfItems: number = sender.get_items().get_count();
            if (numberOfItems > 0) {
                numberOfItems -= 1;
            }
            let totalCount: number = +sender.get_attributes().getAttribute('totalCount');
            if (sender.get_attributes().getAttribute('filter') != this._rcbLookup.get_text()) {
                totalCount = undefined;
                numberOfItems = 0;
                sender.clearItems();
            }
            this.setMoreResultBoxText('Caricamento...');
            let updating: boolean = sender.get_attributes().getAttribute('updating') == 'true';
            if ((!totalCount || numberOfItems < totalCount) && !updating) {
                sender.get_attributes().setAttribute('updating', 'true');
                let filter: string = this._rcbLookup.get_text();
                if (this.checkBoxesEnabled && (filter.indexOf('checked') > 0 || filter.indexOf(',') > 0)) {
                    filter = '';
                }
                this._udsService.getLookupValues(this.propertyName, filter, this.maxNumberElements, numberOfItems,
                    (data: UDSLookupViewModel) => {
                        if (data) {
                            if (data.values && data.values.length > 0) {
                                this.setLookupValues(data.values);
                            }
                            let hidden = $("#".concat(this.hiddenLookupId));
                            if (this.checkBoxesEnabled && hidden && hidden.val()) {
                                let hiddenFieldValues: { [label: string]: string[] } = JSON.parse(hidden.val());
                                if (hiddenFieldValues && hiddenFieldValues[this.lookupLabel]) {
                                    let itemsToCheck: Telerik.Web.UI.RadComboBoxItem[] = this._rcbLookup.get_items().toArray().filter((i) => {
                                        for (let j of hiddenFieldValues[this.lookupLabel]) {
                                            if (j == i.get_value()) {
                                                return true;
                                            }
                                        }
                                        return false;
                                    });
                                    for (let item of itemsToCheck) {
                                        (<Telerik.Web.UI.RadComboBoxItem>item).set_checked(true);
                                    }
                                }
                            }


                            sender.get_attributes().setAttribute('totalCount', data.count.toString());
                            sender.get_attributes().setAttribute('updating', 'false');
                            sender.get_attributes().setAttribute('filter', this._rcbLookup.get_text());
                            if (sender.get_items().get_count() > 0) {
                                numberOfItems = sender.get_items().get_count() - 1;
                            }
                            this.setMoreResultBoxText('Visualizzati '.concat(numberOfItems.toString(), ' di ', data.count.toString()));
                        }
                    },
                    (exception: ExceptionDTO) => {
                        sender.get_attributes().setAttribute('updating', 'false');
                        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
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
                this.setMoreResultBoxText('Visualizzati '.concat(numberOfItems.toString(), ' di ', totalCount.toString()));
            }
        }, 100);
    }

    rcbLookup_OnDropDownOpened = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
    }

    rcbLookup_OnItemChecked = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        if (args.get_item()) {
            let items: Array<Telerik.Web.UI.RadComboBoxItem> = this._rcbLookup.get_checkedItems();

            let hiddenFieldValues: { [label: string]: string[] } = {};
            let checkedValues: string[] = items.map((i) => i.get_text());
            if (this._hiddenLookup.val()) {
                hiddenFieldValues = JSON.parse(this._hiddenLookup.val());
            }
            hiddenFieldValues[this.lookupLabel] = checkedValues;
            this._hiddenLookup.val(JSON.stringify(hiddenFieldValues));                 
        }        
    }


    rcbLookup_OnSelectedIndexChange = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            sender.clearItems();
            this.setMoreResultBoxText("Visualizzati 1 di 1");
        }
    }
    /**
  *------------------------- Methods -----------------------------
  */

    initialize() {
        this._udsConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, this.UDSName);
        this._udsService = new UDSService(this._udsConfiguration);
        this._rcbLookup = <Telerik.Web.UI.RadComboBox>$find(this.rcbLookupId);
        this._rcbLookup.add_itemsRequesting(this.rcbLookup_OnItemsRequesting);
        this._rcbLookup.add_dropDownOpened(this.rcbLookup_OnDropDownOpened);
        this._rcbLookup.add_selectedIndexChanged(this.rcbLookup_OnSelectedIndexChange);
        this._rcbLookup.add_itemChecked(this.rcbLookup_OnItemChecked);
        this._hiddenLookup = $("#".concat(this.hiddenLookupId));
        this.setSelectedValues(this.lookupValue);
        
        let scrollContainer: JQuery = $(this._rcbLookup.get_dropDownElement()).find('div.rcbScroll');
        $(scrollContainer).scroll(this.rcbLookup_onScroll);
    }

    private setLookupValues(values: string[]) {
        if (this._rcbLookup.get_items().get_count() == 0) {
            let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            emptyItem.set_text("");
            emptyItem.set_value("");
            this._rcbLookup.get_items().insert(0, emptyItem);
        }
        let item: Telerik.Web.UI.RadComboBoxItem;
        for (let value of values) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(value);
            item.set_value(value);
            this._rcbLookup.get_items().add(item);
        }
        this._rcbLookup.trackChanges();
        this._rcbLookup.showDropDown();
    }

    /**
    * Metodo che setta la label MoreResultBoxText della RadComboBox
    * @param message
    */
    private setMoreResultBoxText(message: string) {
        this._rcbLookup.get_moreResultsBoxMessageElement().innerText = message;
    }

    setSelectedValues(values: string) {
        if (values) {
            let selectedValues: string[] = JSON.parse(values);
            if (this.checkBoxesEnabled) {
                if (selectedValues) {
                    let hiddenFieldValues: { [label: string]: string[] } = {};
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
    }
}

export = uscUDSLookup;