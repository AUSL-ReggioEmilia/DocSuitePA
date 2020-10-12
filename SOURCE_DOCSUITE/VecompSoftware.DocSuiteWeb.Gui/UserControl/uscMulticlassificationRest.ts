import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import DocumentUnitFascicleCategoriesService = require("App/Services/DocumentUnits/DocumentUnitFascicleCategoriesService");
import DocumentUnitModel = require("App/Models/DocumentUnits/DocumentUnitModel");
import FascicleDocumentUnitCategoryModel = require("App/Models/Fascicles/FascicleDocumentUnitCategoryModel");


class UscMulticlassificationRest {
    radTreeCategoriesId: string;
    idDocumentUnit: string;
    documentUnitModel: DocumentUnitModel;
    fascicleDocumentUnitCategoryModel: FascicleDocumentUnitCategoryModel[];
    isVisible: string;
    multiclassificationContainer: HTMLElement;

    private _serviceConfigurations: ServiceConfiguration[];
    private _radTreeCategories: Telerik.Web.UI.RadTreeView;
    private _documentUnitFascicleCategoriesService: DocumentUnitFascicleCategoriesService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize(): void{
        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnitFascicleCategory");
        this._documentUnitFascicleCategoriesService = new DocumentUnitFascicleCategoriesService(serviceConfiguration);

        this._radTreeCategories = <Telerik.Web.UI.RadTreeView>$find(this.radTreeCategoriesId);

        if (this.isVisible === "True") {
            this.loadDocumentUnitModel();
        }        
    }

    private loadDocumentUnitModel(): void {       
        this._documentUnitFascicleCategoriesService.getDocumentUnitFascicleCategory(this.idDocumentUnit, (response) => {
            this.fascicleDocumentUnitCategoryModel = response;
            if (this.fascicleDocumentUnitCategoryModel.length === 0) {
                this.multiclassificationContainer.setAttribute("style", "display:none");
                return;
            }
            this.populateTreeView();
        });
    }

    private populateTreeView(): void {
        for (let model of this.fascicleDocumentUnitCategoryModel) {
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(model.CategoryTitle);
            node.set_cssClass("font_node");
            this._radTreeCategories.get_nodes().add(node);
        }
    }
}

export = UscMulticlassificationRest;