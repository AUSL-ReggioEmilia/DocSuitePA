define(["require", "exports"], function (require, exports) {
    /*
     * UI handler for TbltWorkflowEvaluationPropertyGes.ts.
     * The handler loads template collaboration models in the ComboBox and handles setting up
     * the selected value in the RadTextBox when commit is called
     */
    var UIHandlerEvalPropertyTemplateCollaboration = /** @class */ (function () {
        function UIHandlerEvalPropertyTemplateCollaboration(rtbValueString, templateComboBox, templateCollaborationService) {
            var _this = this;
            this.lastSelectedCollborationId = null;
            this.LoadTemplateCollaborations = function () {
                _this._templateCollaborationService.getTemplates(function (data) {
                    var item;
                    for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                        var templateCollaboration = data_1[_i];
                        item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(templateCollaboration.Name);
                        item.set_value(templateCollaboration.UniqueId);
                        _this._templateComboBox.get_items().add(item);
                    }
                    _this._templateComboBox.enable();
                    _this._templateComboBox.add_selectedIndexChanged(function (sender, args) {
                        var templateId = args.get_item().get_value();
                        _this.SetSelectedItem(templateId);
                    });
                    //- loadTemplateCollaborations is asynronous.
                    //- When the evaluation property window opens both loadTemplateCollaborations() and getFirstNonNull()
                    //    are fired, creating a race between:
                    //      - loading the templates
                    //      - setting up the current Template Id in getFirstNonNull ().
                    //- Updating the selection again here makes sure that the saved template id is selected in cmbBox
                    _this.UpdateSelection();
                });
            };
            this._rtbValueString = rtbValueString;
            this._templateComboBox = templateComboBox;
            this._templateCollaborationService = templateCollaborationService;
        }
        /**
         * Will set the selected item field. Will not update the combobox selected item.
         * @param uniqueId
         */
        UIHandlerEvalPropertyTemplateCollaboration.prototype.SetSelectedItem = function (uniqueId) {
            this.lastSelectedCollborationId = uniqueId;
        };
        /**
         * Updates the ComboBox selected item based item set by SetSelectedItem
         */
        UIHandlerEvalPropertyTemplateCollaboration.prototype.UpdateSelection = function () {
            if (this.lastSelectedCollborationId) {
                var selectedItem = this._templateComboBox.findItemByValue(this.lastSelectedCollborationId);
                if (selectedItem) {
                    selectedItem.select();
                }
            }
        };
        /**
         * Saving the value of the selection into the RadTextBox
         */
        UIHandlerEvalPropertyTemplateCollaboration.prototype.Commit = function () {
            if (this.lastSelectedCollborationId) {
                this._rtbValueString.set_value(this.lastSelectedCollborationId);
            }
        };
        return UIHandlerEvalPropertyTemplateCollaboration;
    }());
    return UIHandlerEvalPropertyTemplateCollaboration;
});
//# sourceMappingURL=UIHandlerEvalPropertyTemplateCollaboration.js.map