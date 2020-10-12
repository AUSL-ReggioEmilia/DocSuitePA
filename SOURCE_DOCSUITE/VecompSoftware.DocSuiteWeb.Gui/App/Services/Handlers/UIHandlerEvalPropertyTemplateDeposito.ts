import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateDocumentRepositoryService = require('App/Services/Templates/TemplateDocumentRepositoryService');
import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import TemplateDocumentFinderViewModel = require('App/ViewModels/Templates/TemplateDocumentFinderViewModel');

class UIHandlerEvalPropertyTemplateDeposito {
    private _rtbValueString: Telerik.Web.UI.RadTextBox;
    private _templateComboBox: Telerik.Web.UI.RadComboBox;
    private _service: TemplateDocumentRepositoryService;

    private lastSelectedTemplateId: string = null;

    constructor(
        rtbValueString: Telerik.Web.UI.RadTextBox,
        templateComboBox: Telerik.Web.UI.RadComboBox,
        serviceConfigurations: ServiceConfiguration[]) {

        this._rtbValueString = rtbValueString;
        this._templateComboBox = templateComboBox;

        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
        this._service = new TemplateDocumentRepositoryService(serviceConfiguration);
    }

    private getFinder(): TemplateDocumentFinderViewModel {
        let finder: TemplateDocumentFinderViewModel = new TemplateDocumentFinderViewModel();
        finder.Status.push(TemplateDocumentRepositoryStatus.Available);
        return finder;
    }

    public LoadTemplateDocumenti = () => {
        try {

            let finder: TemplateDocumentFinderViewModel = this.getFinder();
            this._service.findTemplateDocument(finder, (data: TemplateDocumentRepositoryModel[]) => {

                let item: Telerik.Web.UI.RadComboBoxItem;
                for (let templateCollaboration of data) {
                    item = new Telerik.Web.UI.RadComboBoxItem();
                    item.set_text(templateCollaboration.Name);
                    item.set_value(templateCollaboration.UniqueId);
                    this._templateComboBox.get_items().add(item);
                }

                this._templateComboBox.enable();

                this._templateComboBox.add_selectedIndexChanged((sender, args) => {
                    const templateId: string = args.get_item().get_value();
                    this.SetSelectedItem(templateId);
                });

                //- loadTemplateCollaborations is asynronous.
                //- When the evaluation property window opens both loadTemplateCollaborations() and getFirstNonNull()
                //    are fired, creating a race between:
                //      - loading the templates
                //      - setting up the current Template Id in getFirstNonNull ().
                //- Updating the selection again here makes sure that the saved template id is selected in cmbBox
                this.UpdateSelection();
            });

        } catch (error) {
            console.log((<Error>error).stack);
        }
    }

    /**
     * Will set the selected item field. Will not update the combobox selected item.
     * @param uniqueId
     */
    public SetSelectedItem(uniqueId: string) {
        this.lastSelectedTemplateId = uniqueId;
    }

    /**
     * Updates the ComboBox selected item based item set by SetSelectedItem
     */
    public UpdateSelection() {
        if (this.lastSelectedTemplateId) {
            let selectedItem: Telerik.Web.UI.RadComboBoxItem = this._templateComboBox.findItemByValue(this.lastSelectedTemplateId);
            if (selectedItem) {
                selectedItem.select();
            }
        }
    }

    /**
     * Saving the value of the selection into the RadTextBox
     */
    public Commit() {
        if (this.lastSelectedTemplateId) {
            this._rtbValueString.set_value(this.lastSelectedTemplateId);
        }
    }
}
export = UIHandlerEvalPropertyTemplateDeposito;