/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../Declarations/jstree.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var UdsDesigner;
(function (UdsDesigner) {
    var ContainerViewModel = /** @class */ (function (_super) {
        __extends(ContainerViewModel, _super);
        //Constructor
        function ContainerViewModel() {
            var _this = _super.call(this) || this;
            _this.modalTitle = "contenitore";
            return _this;
        }
        //Methods
        ContainerViewModel.prototype.setup = function () {
            this.initializeAjaxData();
            _super.prototype.setup.call(this);
            this.bind($("#configuration_modal")[0]);
        };
        ContainerViewModel.prototype.initializeAjaxData = function () {
            $.jstree.defaults.core.data = {
                'type': "POST",
                'contentType': "application/json; charset=utf-8",
                'dataType': "json",
                'url': function (node) {
                    var description = $("#txtSearch").val();
                    if (description != "") {
                        return "DesignerService.aspx/FindContainerByDescription";
                    }
                    return 'DesignerService.aspx/LoadContainers';
                },
                'data': function (node) {
                    var description = $("#txtSearch").val();
                    if (description != "") {
                        return '{description:"' + description + '"}';
                    }
                    return '{idContainer:"' + node.id + '"}';
                }
            };
        };
        ContainerViewModel.prototype.saveCallback = function (e) {
            e.preventDefault();
            var selected = $("#configurationTree").jstree(true).get_selected(true);
            $(document).trigger("containerSelected", [selected[0].id, selected[0].text]);
        };
        return ContainerViewModel;
    }(UdsDesigner.BaseTreeViewModel));
    UdsDesigner.ContainerViewModel = ContainerViewModel;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=ContainerViewModel.js.map