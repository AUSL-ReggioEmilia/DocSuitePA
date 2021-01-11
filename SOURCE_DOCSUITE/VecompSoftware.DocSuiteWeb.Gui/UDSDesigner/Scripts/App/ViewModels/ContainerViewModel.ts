/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Declarations/jstree.d.ts" />

module UdsDesigner {
    declare var rivets: any;

    export class ContainerViewModel
        extends BaseTreeViewModel {
        //Fields
        modalTitle: string;

        //Constructor
        constructor() {
            super();
            this.modalTitle = "contenitore";
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
                        return "DesignerService.aspx/FindContainerByDescription";
                    }
                    return 'DesignerService.aspx/LoadContainers';
                },
                'data': function (node) {
                    var description: string = $("#txtSearch").val();
                    if (description != "") {
                        return '{description:"' + description + '"}';
                    }
                    return '{idContainer:"' + node.id + '"}';
                }
            }
        }

        private saveCallback(e: any): void {
            e.preventDefault();
            var selected = $("#configurationTree").jstree(true).get_selected(true)
            $(document).trigger("containerSelected", [selected[0].id, selected[0].text]);
        }
    }
}