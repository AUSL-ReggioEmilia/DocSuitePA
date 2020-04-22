/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />

module UdsDesigner {

    export class DesignerResultsController {
        
        //Fields
        private column: any = null;
        //Properties
        public static get designerUrlFormat(): string { return "Designer.aspx?IdUds={0}" };

        //Contructor
        constructor() {
        }

        //Functions
        public menuShowing(sender: any, eventArgs: any): any {
            if (this.column == null) return;
            var menu: any = sender; var items: any = menu.get_items();
            if (this.column.get_uniqueName() == "ActiveDate" || this.column.get_uniqueName() == "ExpiredDate") {
                var i = 0;
                while (i < items.get_count()) {
                    if (!(items.getItem(i).get_value() in {
                        'NoFilter': '', 'NotIsEmpty': '', 'IsEmpty': '', 'NotEqualTo': '', 'EqualTo': '', 'GreaterThan': '', 'LessThan': '', 'GreaterThanOrEqualTo': '', 'LessThanOrEqualTo': ''
                    })) {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(false);
                    } i++;
                }
            }
            this.column = null;
            menu.repaint();
        }

        public filterMenuShowing(sender: any, eventArgs: any): void {
            this.column = eventArgs.get_column();
        }

        public static setStatus(idStatus: any): string {
            return idStatus == 1 ? "Bozza" : "Confermata";
        }

        public static getStatusCssClass(idStatus: any): string {
            return idStatus == 1 ? "warning" : "success";
        }
    }
}