/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
var UdsDesigner;
(function (UdsDesigner) {
    var DesignerResultsController = /** @class */ (function () {
        //Contructor
        function DesignerResultsController() {
            //Fields
            this.column = null;
        }
        Object.defineProperty(DesignerResultsController, "designerUrlFormat", {
            //Properties
            get: function () { return "Designer.aspx?IdUds={0}"; },
            enumerable: true,
            configurable: true
        });
        ;
        //Functions
        DesignerResultsController.prototype.menuShowing = function (sender, eventArgs) {
            if (this.column == null)
                return;
            var menu = sender;
            var items = menu.get_items();
            if (this.column.get_uniqueName() == "ActiveDate" || this.column.get_uniqueName() == "ExpiredDate") {
                var i = 0;
                while (i < items.get_count()) {
                    if (!(items.getItem(i).get_value() in {
                        'NoFilter': '', 'NotIsEmpty': '', 'IsEmpty': '', 'NotEqualTo': '', 'EqualTo': '', 'GreaterThan': '', 'LessThan': '', 'GreaterThanOrEqualTo': '', 'LessThanOrEqualTo': ''
                    })) {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(false);
                    }
                    i++;
                }
            }
            this.column = null;
            menu.repaint();
        };
        DesignerResultsController.prototype.filterMenuShowing = function (sender, eventArgs) {
            this.column = eventArgs.get_column();
        };
        DesignerResultsController.setStatus = function (idStatus) {
            return idStatus == 1 ? "Bozza" : "Confermata";
        };
        DesignerResultsController.getStatusCssClass = function (idStatus) {
            return idStatus == 1 ? "warning" : "success";
        };
        return DesignerResultsController;
    }());
    UdsDesigner.DesignerResultsController = DesignerResultsController;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=DesignerResultsController.js.map