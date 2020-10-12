define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Templates/TemplateDocumentRepositoryService", "App/Models/Templates/TemplateDocumentRepositoryStatus", "App/ViewModels/Templates/TemplateDocumentFinderViewModel"], function (require, exports, ServiceConfigurationHelper, TemplateDocumentRepositoryService, TemplateDocumentRepositoryStatus, TemplateDocumentFinderViewModel) {
    var UIHandlerEvalPropertyTemplateDeposito = /** @class */ (function () {
        function UIHandlerEvalPropertyTemplateDeposito(rtbValueString, templateComboBox, serviceConfigurations) {
            var _this = this;
            this.lastSelectedTemplateId = null;
            this.LoadTemplateDocumenti = function () {
                try {
                    var finder = _this.getFinder();
                    _this._service.findTemplateDocument(finder, function (data) {
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
                }
                catch (error) {
                    console.log(error.stack);
                }
            };
            this._rtbValueString = rtbValueString;
            this._templateComboBox = templateComboBox;
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
            this._service = new TemplateDocumentRepositoryService(serviceConfiguration);
        }
        UIHandlerEvalPropertyTemplateDeposito.prototype.getFinder = function () {
            var finder = new TemplateDocumentFinderViewModel();
            finder.Status.push(TemplateDocumentRepositoryStatus.Available);
            return finder;
        };
        /**
         * Will set the selected item field. Will not update the combobox selected item.
         * @param uniqueId
         */
        UIHandlerEvalPropertyTemplateDeposito.prototype.SetSelectedItem = function (uniqueId) {
            this.lastSelectedTemplateId = uniqueId;
        };
        /**
         * Updates the ComboBox selected item based item set by SetSelectedItem
         */
        UIHandlerEvalPropertyTemplateDeposito.prototype.UpdateSelection = function () {
            if (this.lastSelectedTemplateId) {
                var selectedItem = this._templateComboBox.findItemByValue(this.lastSelectedTemplateId);
                if (selectedItem) {
                    selectedItem.select();
                }
            }
        };
        /**
         * Saving the value of the selection into the RadTextBox
         */
        UIHandlerEvalPropertyTemplateDeposito.prototype.Commit = function () {
            if (this.lastSelectedTemplateId) {
                this._rtbValueString.set_value(this.lastSelectedTemplateId);
            }
        };
        return UIHandlerEvalPropertyTemplateDeposito;
    }());
    return UIHandlerEvalPropertyTemplateDeposito;
});
//# sourceMappingURL=UIHandlerEvalPropertyTemplateDeposito.js.map