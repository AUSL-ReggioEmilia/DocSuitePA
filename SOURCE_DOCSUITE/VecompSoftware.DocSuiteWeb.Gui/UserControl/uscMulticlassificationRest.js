define(["require", "exports", "../App/Helpers/ServiceConfigurationHelper", "../App/Services/DocumentUnits/DocumentUnitFascicleCategoriesService"], function (require, exports, ServiceConfigurationHelper, DocumentUnitFascicleCategoriesService) {
    var UscMulticlassificationRest = /** @class */ (function () {
        function UscMulticlassificationRest(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
        }
        UscMulticlassificationRest.prototype.initialize = function () {
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnitFascicleCategory");
            this._documentUnitFascicleCategoriesService = new DocumentUnitFascicleCategoriesService(serviceConfiguration);
            this._radTreeCategories = $find(this.radTreeCategoriesId);
            if (this.isVisible === "True") {
                this.loadDocumentUnitModel();
            }
        };
        UscMulticlassificationRest.prototype.loadDocumentUnitModel = function () {
            var _this = this;
            this._documentUnitFascicleCategoriesService.getDocumentUnitFascicleCategory(this.idDocumentUnit, function (response) {
                _this.fascicleDocumentUnitCategoryModel = response;
                if (_this.fascicleDocumentUnitCategoryModel.length === 0) {
                    _this.multiclassificationContainer.setAttribute("style", "display:none");
                    return;
                }
                _this.populateTreeView();
            });
        };
        UscMulticlassificationRest.prototype.populateTreeView = function () {
            for (var _i = 0, _a = this.fascicleDocumentUnitCategoryModel; _i < _a.length; _i++) {
                var model = _a[_i];
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(model.CategoryTitle);
                node.set_cssClass("font_node");
                this._radTreeCategories.get_nodes().add(node);
            }
        };
        return UscMulticlassificationRest;
    }());
    return UscMulticlassificationRest;
});
//# sourceMappingURL=uscMulticlassificationRest.js.map