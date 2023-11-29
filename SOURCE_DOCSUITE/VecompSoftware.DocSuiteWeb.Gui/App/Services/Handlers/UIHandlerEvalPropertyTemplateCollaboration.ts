import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import UscTemplateCollaborationSelRest = require('UserControl/uscTemplateCollaborationSelRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');

/*
 * UI handler for TbltWorkflowEvaluationPropertyGes.ts.
 * The handler loads template collaboration models in the ComboBox and handles setting up
 * the selected value in the RadTextBox when commit is called
 */
class UIHandlerEvalPropertyTemplateCollaboration {
    private _rtbValueString: Telerik.Web.UI.RadTextBox;
    private _templateSelectionControlId: string;

    private lastSelectedCollborationId: string = null;
    private _controlInitialized: boolean = false;

    constructor(
        rtbValueString: Telerik.Web.UI.RadTextBox,
        templateSelectionControlId: string) {
        this._rtbValueString = rtbValueString;
        this._templateSelectionControlId = templateSelectionControlId;
    }

    public InitializeTemplateTreeviewControl = (reloadTreeNodes: boolean = true) => {
        if (this._controlInitialized) {
            return;
        }
        PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationSelRest>(this._templateSelectionControlId)
            .then(controlInstance => {
                this._controlInitialized = true;

                if (reloadTreeNodes) {
                    controlInstance.ReloadRoot();
                }

                controlInstance.OnFixedTemplateClick(this._templateSelectionControlId, (fixedTemplate) => {
                    this.SetSelectedItem(fixedTemplate.UniqueId);
                });

                controlInstance.OnTemplateClick(this._templateSelectionControlId, (fixedTemplate, template) => {
                    this.SetSelectedItem(template.UniqueId);
                });
            });
    }

    public InitializeDisableButtonEvent(btnConfirm: Telerik.Web.UI.RadButton): void {
        PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationSelRest>(this._templateSelectionControlId)
            .done((instance) => {
                instance.OnFolderClick_DisableConfirmaButton(this._templateSelectionControlId, (disableButton) => {
                    btnConfirm.set_enabled(!disableButton);
                });
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
            PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationSelRest>(this._templateSelectionControlId)
                .then(controlInstance => {
                    controlInstance.SelectAndForceLoadNode(this.lastSelectedCollborationId);
                });
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