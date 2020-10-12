import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');

/*
 * UI handler for TbltWorkflowEvaluationPropertyGes.ts.
 * The handler loads template collaboration models in the ComboBox and handles setting up
 * the selected value in the RadTextBox when commit is called
 */
class UIHandlerEvalPropertyTemplateCollaboration {
    private _rtbValueString: Telerik.Web.UI.RadTextBox;
    private _templateComboBox: Telerik.Web.UI.RadComboBox;
    private _templateCollaborationService: TemplateCollaborationService;

    private lastSelectedCollborationId: string = null;

    constructor(
        rtbValueString: Telerik.Web.UI.RadTextBox,
        templateComboBox: Telerik.Web.UI.RadComboBox,
        templateCollaborationService: TemplateCollaborationService) {
        this._rtbValueString = rtbValueString;
        this._templateComboBox = templateComboBox;
        this._templateCollaborationService = templateCollaborationService;
    }

    public LoadTemplateCollaborations = () => {
        this._templateCollaborationService.getTemplates((data) => {

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
    }

    /**
     * Will set the selected item field. Will not update the combobox selected item.
     * @param uniqueId
     */
    public SetSelectedItem(uniqueId: string) {
        this.lastSelectedCollborationId = uniqueId;
    }

    /**
     * Updates the ComboBox selected item based item set by SetSelectedItem
     */
    public UpdateSelection() {
        if (this.lastSelectedCollborationId) {
            let selectedItem: Telerik.Web.UI.RadComboBoxItem = this._templateComboBox.findItemByValue(this.lastSelectedCollborationId);
            if (selectedItem) {
                selectedItem.select();
            }
        }
    }

    /**
     * Saving the value of the selection into the RadTextBox
     */
    public Commit() {
        if (this.lastSelectedCollborationId) {
            this._rtbValueString.set_value(this.lastSelectedCollborationId);
        }
    }
}

export = UIHandlerEvalPropertyTemplateCollaboration;