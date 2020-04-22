/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Declarations/jstree.d.ts" />

module UdsDesigner {
    export class CategoryViewModel
        extends BaseTreeViewModel {
        //Fields
        modalTitle: string;

        //Constructor
        constructor() {
            super();
            this.modalTitle = "classificatore";
        }

        //Methods
        setup(): void {
            this.initializeAjaxData();       
            super.setup();                 
            this.bind($("#configuration_modal")[0]);
        }

        private initializeAjaxData() {
            $.jstree.defaults.core.data = {
                'type': "POST",
                'contentType': "application/json; charset=utf-8",
                'dataType': "json",
                'url': function (node) {
                    var description: string = $("#txtSearch").val();
                    if (description != "") {
                        return "DesignerService.aspx/FindCategoryByDescription?lazy";
                    }
                    return node.id === '#' ?
                        'DesignerService.aspx/LoadRootCategories?lazy' :
                        'DesignerService.aspx/LoadChildCategories?lazy';
                },
                'data': function (node) {
                    var description: string = $("#txtSearch").val();
                    if (description != "") {
                        return '{description:"' + description + '"}';
                    }
                    return '{idCategory:"' + node.id + '"}';
                }
            }
        }

        private saveCallback(e: any): void {
            e.preventDefault();
            var selected = $("#configurationTree").jstree(true).get_selected(true)
            $(document).trigger("categorySelected", [selected[0].id, selected[0].text]);
        }
    }
}