import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryViewModel = require('App/ViewModels/Commons/MetadataRepositoryViewModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import uscSetiContactSel = require('./uscSetiContactSel');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import UscAdvancedSearchDynamicMetadataRest = require('UserControl/UscAdvancedSearchDynamicMetadataRest');
import MetadataFinderViewModel = require('App/ViewModels/Metadata/MetadataFinderViewModel');

class uscMetadataRepositorySel {
    rcbMetadataRepositoryId: string;
    uscNotificationId: string;
    maxNumberElements: string;
    selectedRepositoryId: string;
    metadataPageContentId: string;
    setiContactEnabledId: boolean;
    uscSetiContactSelId: string;
    setiVisibilityButtonId: boolean;
    uscAdvancedSearchDynamicMetadataRestId: string;
    enableAdvancedMetadataSearchBtnId: string;
    txtMetadataValueId: string;
    advancedMetadataRepositoryEnabled: boolean;

    private _rcbMetadataRepository: Telerik.Web.UI.RadComboBox;
    private _uscAdvancedSearchDynamicMetadataRest: UscAdvancedSearchDynamicMetadataRest;
    private _serviceConfigurations: ServiceConfiguration[];
    private _metadataRepositoryConfiguration: ServiceConfiguration;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _enableAdvancedMetadataSearchBtn: HTMLInputElement;
    private _txtMetadataValue: Telerik.Web.UI.RadTextBox;

    public static SELECTED_REPOSITORY_EVENT: string = "onSelectedRepositoryChangeEvent";
    public static SELECTED_SETI_CONTACT_EVENT: string = "onSelectedSetiContactEvent";

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
    private rcbLookup_onScroll = (args: JQueryEventObject) => {
        let element = args.target;
        if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
            this.rcbMetadataRepository_OnItemsRequested(this._rcbMetadataRepository, new (<any>Telerik.Web.UI.RadComboBoxRequestEventArgs)(args));
        }
    }

    private rcbMetadataRepository_OnItemsRequested = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let numberOfItems: number = sender.get_items().get_count();
        if (numberOfItems > 0) {
            numberOfItems -= 1;
        }
        let totalCount: number = +sender.get_attributes().getAttribute('totalCount');
        if (sender.get_attributes().getAttribute('filter') != this._rcbMetadataRepository.get_text()) {
            totalCount = undefined;
            numberOfItems = 0;
            sender.clearItems();
        }
        this.setMoreResultBoxText('Caricamento...');
        let updating: boolean = sender.get_attributes().getAttribute('updating') == 'true';
        if ((!totalCount || numberOfItems < totalCount) && !updating) {
            sender.get_attributes().setAttribute('updating', 'true');
            let metadataRestrictions: string[] = [];
            let metadataRestrictionsAttribute = this._rcbMetadataRepository.get_attributes().getAttribute("repositoryRestrictions");
            if (metadataRestrictionsAttribute) {
                metadataRestrictions = JSON.parse(metadataRestrictionsAttribute);
            }
            let safeEncoding: string = sender.get_text().replace(/'/g, '%27%27');
            this._metadataRepositoryService.getAvailableMetadataRepositories(safeEncoding, metadataRestrictions, this.maxNumberElements, numberOfItems,
                (data: ODATAResponseModel<MetadataRepositoryViewModel>) => {
                    if (data) {
                        if (data.count > 0) {
                            this.setValues(data.value);
                        }
                        sender.get_attributes().setAttribute('totalCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        sender.get_attributes().setAttribute('filter', this._rcbMetadataRepository.get_text());
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
            this.setMoreResultBoxText('Visualizzati '.concat(numberOfItems.toString(), ' di ', totalCount.toString()));
        }
    }

    private rcbMetadataRepository_OnDropDownOpened = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
    }

    private _setMetadataValueElementsState(isMetadaRepositorySelected: boolean): void {
        let currentAdvancedSearchBtnState: boolean = this._enableAdvancedMetadataSearchBtn.checked;
        this._enableAdvancedMetadataSearchBtn.disabled = !isMetadaRepositorySelected;
        this._enableAdvancedMetadataSearchBtn.checked = isMetadaRepositorySelected ? currentAdvancedSearchBtnState : false;

        if (!$.isEmptyObject(this._uscAdvancedSearchDynamicMetadataRest) && !this._enableAdvancedMetadataSearchBtn.checked) {
            this._uscAdvancedSearchDynamicMetadataRest.clearAdvancedSearchPanelContent();
        }

        this._txtMetadataValue.clear();
        this._txtMetadataValue.set_visible(!this._enableAdvancedMetadataSearchBtn.checked);
    }

    private rcbMetadataRepository_OnSelectedIndexChange = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = args.get_item();
        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            sender.clearItems();
            this.setMoreResultBoxText("Visualizzati 1 di 1");
        }

        let isMetadataRepositorySelected: boolean = selectedItem && selectedItem.get_text() !== "";

        if (this.advancedMetadataRepositoryEnabled) {
            this._setMetadataValueElementsState(isMetadataRepositorySelected);
        }

        if (isMetadataRepositorySelected) {

            if (this.advancedMetadataRepositoryEnabled && this._enableAdvancedMetadataSearchBtn.checked) {
                this._uscAdvancedSearchDynamicMetadataRest.loadMetadataRepository(selectedItem.get_value());
            }

            $("#".concat(this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, selectedItem.get_value());
            this.getSelectedMetadata().then(data => {
                if (data && data.JsonMetadata) {
                    let metadataVM: MetadataDesignerViewModel = JSON.parse(data.JsonMetadata);
                    $("#".concat(this.uscSetiContactSelId)).triggerHandler(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, this.setiContactEnabledId && metadataVM.SETIFieldEnabled && this.setiVisibilityButtonId);
                }
            })
        } else {
            //notify that no repository is selected to clear the metadata values control contents
            $("#".concat(this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, null);
        }
    }
    /**
  *------------------------- Methods -----------------------------
  */

    initialize() {
        this._metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
        this._metadataRepositoryService = new MetadataRepositoryService(this._metadataRepositoryConfiguration);
        this._rcbMetadataRepository = <Telerik.Web.UI.RadComboBox>$find(this.rcbMetadataRepositoryId);
        this._rcbMetadataRepository.add_itemsRequested(this.rcbMetadataRepository_OnItemsRequested);
        this._rcbMetadataRepository.add_dropDownOpened(this.rcbMetadataRepository_OnDropDownOpened);
        this._rcbMetadataRepository.add_selectedIndexChanged(this.rcbMetadataRepository_OnSelectedIndexChange);
        this._uscAdvancedSearchDynamicMetadataRest = <UscAdvancedSearchDynamicMetadataRest>$("#".concat(this.uscAdvancedSearchDynamicMetadataRestId)).data();

        if (this.advancedMetadataRepositoryEnabled) {
            this.initializeMetadataPanel();
        }

        let scrollContainer: JQuery = $(this._rcbMetadataRepository.get_dropDownElement()).find('div.rcbScroll');
        $(scrollContainer).scroll(this.rcbLookup_onScroll);

        $("#".concat(this.metadataPageContentId)).data(this);
        $("#".concat(this.uscSetiContactSelId)).data(this);
    }

    private initializeMetadataPanel(): void {
        let $advancedMetadataSearchBtn = $(`#${this.enableAdvancedMetadataSearchBtnId}`);
        this._enableAdvancedMetadataSearchBtn = <HTMLInputElement>$advancedMetadataSearchBtn[0];
        $advancedMetadataSearchBtn.on("change", () => {
            let advancedMetadataSearchEnabled: boolean = this._enableAdvancedMetadataSearchBtn.checked;
            this._txtMetadataValue.set_visible(!advancedMetadataSearchEnabled);

            let selectedMetadataRepositoryId: string = this.getSelectedMetadataRepositoryId();
            this._uscAdvancedSearchDynamicMetadataRest.setPanelSearchType(advancedMetadataSearchEnabled, selectedMetadataRepositoryId);
        });

        this._txtMetadataValue = <Telerik.Web.UI.RadTextBox>$find(this.txtMetadataValueId);
        this._setMetadataValueElementsState(false);
    }

    private setValues(repositories: MetadataRepositoryViewModel[]) {
        if (this._rcbMetadataRepository.get_items().get_count() == 0) {
            let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            emptyItem.set_text("");
            emptyItem.set_value("");
            this._rcbMetadataRepository.get_items().insert(0, emptyItem);
        }
        let item: Telerik.Web.UI.RadComboBoxItem;
        for (let repository of repositories) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(repository.Name);
            item.set_value(repository.UniqueId);
            this._rcbMetadataRepository.get_items().add(item);
        }
        this._rcbMetadataRepository.trackChanges();
        this._rcbMetadataRepository.showDropDown();
    }

    /**
    * Metodo che setta la label MoreResultBoxText della RadComboBox
    * @param message
    */
    private setMoreResultBoxText(message: string) {
        this._rcbMetadataRepository.get_moreResultsBoxMessageElement().innerText = message;
    }

    setComboboxText(id: string, generateMetadataInputs: boolean = true) {
        this._metadataRepositoryService.getById(id,
            (data: MetadataRepositoryViewModel) => {
                if (data) {
                    this._rcbMetadataRepository.set_text(data.Name);
                    this._rcbMetadataRepository.set_value(data.UniqueId);
                    if (generateMetadataInputs) {
                        $("#".concat(this.metadataPageContentId)).triggerHandler(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, data.UniqueId);
                    }
                }
            },
            (exception: ExceptionDTO) => {
                let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
                if (exception && uscNotification && exception instanceof ExceptionDTO) {
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                }
            });
    }

    getSelectedMetadata(): JQueryPromise<MetadataRepositoryModel> {
        let promise: JQueryDeferred<MetadataRepositoryModel> = $.Deferred<MetadataRepositoryModel>();
        let model: MetadataRepositoryModel = new MetadataRepositoryModel();
        let selectedValue: string = this._rcbMetadataRepository.get_value();
        if (!selectedValue) {
            promise.resolve(model);
            return;
        }

        this._metadataRepositoryService.getFullModelById(selectedValue,
            (data: MetadataRepositoryModel) => {
                promise.resolve(data);
            },
            (exception: ExceptionDTO) => {
                console.error(exception.statusText);
                promise.reject();
            });
        return promise.promise();
    }

    public getSelectedMetadataRepositoryId(): string {
        let selectedValue: string = this._rcbMetadataRepository.get_value();
        return selectedValue;
    }

    clearComboboxText() {
        if (this._rcbMetadataRepository.get_text()) {
            this._rcbMetadataRepository.set_text("");
        }
    }

    setRepositoryRestrictions(repositoryIds: string[]): void {
        this._rcbMetadataRepository.clearItems();
        this._rcbMetadataRepository.get_attributes().setAttribute("repositoryRestrictions", JSON.stringify(repositoryIds));
    }

    disableSelection(): void {
        this._rcbMetadataRepository.set_enabled(false);
    }

    enableSelection(): void {
        this._rcbMetadataRepository.set_enabled(true);
    }

    getMetadataFinderModels(): [MetadataFinderViewModel[], boolean] {
        let metadataFinderModels: [MetadataFinderViewModel[], boolean] = this._uscAdvancedSearchDynamicMetadataRest.getMetadataFinderModels();

        return metadataFinderModels;
    }

    getMetadataFilterValues(): [string, MetadataFinderViewModel[], boolean] {
        let metadataFilterValues: [string, MetadataFinderViewModel[], boolean] = [null, [], true];

        if (this._enableAdvancedMetadataSearchBtn.checked) {
            let [metadataFinderModels, metadataValuesAreValid]: [MetadataFinderViewModel[], boolean] = this.getMetadataFinderModels();
            metadataFilterValues[1] = metadataFinderModels;
            metadataFilterValues[2] = metadataValuesAreValid;
        } else {
            let metadataValueFilter: string = this._txtMetadataValue.get_value();
            metadataFilterValues[0] = metadataValueFilter;
        }

        return metadataFilterValues;
    }
}

export = uscMetadataRepositorySel;