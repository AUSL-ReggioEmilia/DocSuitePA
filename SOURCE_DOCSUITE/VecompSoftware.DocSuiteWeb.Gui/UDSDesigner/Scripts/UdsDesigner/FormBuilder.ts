/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../Scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../Scripts/typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../app/utils.ts" />
/// <reference path="controls.ts" />

module UdsDesigner {

    declare var JSONEditor: any;
    declare var rivets: any;

    export class FormBuilder {

        delimeter: string = '=';    // delimiter to split value from display text in option based input
        selected: Element = null;
        tempData: any = null;
        jsonEdit: any = null;
        modelChanged: Event = null;
        status: FormStatus = null;
        statusBinder: any = null;
        columns: number;
        rows: number;
        differentCols: number = -1; //check if columns layout is changed, if not, don't delete the existing components

        setup() {
            this.initEvents();
            this.initEditor();
            this.initModals();
            this.initControls();
        }


        resizeEditor() {
            var height = window.innerHeight;

            $("#source").height(height - 110);
            this.jsonEdit.editor.resize();
        }


        renderModel(elements) {
            if (this.tryRender(elements)) {
                $("#content").html("");
                this.tryRender(elements);
                document.dispatchEvent(this.modelChanged);
            }
        }


        getModelJson(): any {
            return this.jsonEdit.get();
        }


        private initEvents() {
            this.modelChanged = document.createEvent("Event");
            this.modelChanged.initEvent('modelChanged', true, true);
            document.addEventListener('modelChanged', (e) => {
                this.onModelChanged(e);
            }, false);
        }


        private initEditor() {
            var editorOptions = {
                mode: 'code',
                indentation: 2,
                error: function (err) {
                    alert(err.toString());
                }
            };

            $("#source").empty();
            this.jsonEdit = new JSONEditor($("#source")[0], editorOptions);
            this.jsonEdit.editor.addEventListener("blur", () => {
                var model = this.getModelJson();
                if (model != null)
                    this.renderModel(model.elements);
            });

            $('a[data-toggle="tab"]').on('shown.bs.tab', (e) => {
                this.jsonEdit.editor.resize();
                this.jsonEdit.editor.renderer.updateFull();
                document.dispatchEvent(this.modelChanged);
            })
        }


        private initModals() {
            //gestione stato della form
            this.status = new FormStatus();
            this.statusBinder = rivets.bind($("#status")[0], { status: this.status });

            $('#options_modal').on('shown.bs.modal', (e) => {
                if (!isNaN(this.differentCols))
                    $("#itemChanger").val(this.differentCols).trigger('change');//bind layout column to existing value
                $(e.currentTarget).find("input").first().focus();
                $(e.currentTarget).find("span").each((i: number, spanElement: Element) => {
                    let ctrl = CtrlBase.getElementCtrl(this.getElement());
                    if (spanElement.id === "deleteSelectedItem" && ctrl.getValues().defaultValue === "") {
                        $(`#${spanElement.id}`).hide();
                    }
                });
            });

            $("#options_modal").submit(function (e) {
                e.preventDefault();
                return false;
            });

            // options modal save button
            $("#save_options").on('click', (e) => {
                e.preventDefault();

                var optionsDlg = $("#options_modal");
                var content = optionsDlg.find('.modal-body');
                var component = optionsDlg.data('type');

                //aggiorna i valori
                var newValue: any = this.tempData.clone();
                var ctrl = CtrlBase.getElementCtrl(this.getElement());
                ctrl.onConfirm();
                newValue.bind(this.getElement());

                optionsDlg.modal('hide');
                document.dispatchEvent(this.modelChanged);

                if (component === "Title" && this.differentCols != Number(document.getElementById('layoutColumns').innerText))
                    if (this.differentCols <= Number(document.getElementById('layoutColumns').innerText)) {
                        let skipCol = Number(document.getElementById('layoutColumns').innerText) - this.differentCols;
                        this.createLayoutColumns(true, skipCol);
                    }
                    else {
                        this.createLayoutColumns(false);
                    }

            });
        }


        private createLayoutColumns(diffCols: boolean, skipCol?: number) {
            let skipCols = 0;
            let layoutColumns = Number(document.getElementById('layoutColumns').innerText);
            this.differentCols = layoutColumns;
            let columnTable = $("#tableLayoutColumns");
            if (!diffCols) {
                columnTable.text("");
                columnTable.append("<tr></tr>");
                this.removeExtraColumns();
            } else {
                skipCols = skipCol;
                this.addExtraColumns(skipCols, layoutColumns);
            }
        }

        private removeExtraColumns() {
            let model = this.getModelJson();
            let colsToHave = Number(document.getElementById('layoutColumns').innerText) - 1;
            let adjustRow = 0;
            $.each(model.elements, (index, element) => {
                if (element.columns > colsToHave) {
                    element.columns = colsToHave;
                    element.rows = adjustRow + 1;
                    adjustRow++;
                } else if (element.columns === colsToHave) {
                    adjustRow++;
                }
            });

            try {
                this.generateForRemoval(model.elements);
                let sortedElements = this.sortByColumnsAndRows(model.elements);
                $.each(sortedElements, (index: number, item) => {
                    if (item.ctrlType == "Title") { }
                    else {
                        var ctrl = this.addCustomComponent(item.ctrlType, item.columns);
                        CtrlBase.getElementCtrl(ctrl).setValues(item);
                    }
                });

                this.onModelChanged(model.elements);
            }
            catch (err) {
                console.log(err);
            }
        }

        private generateForRemoval(elements: any) {
            let noOfColumns = Number(document.getElementById('layoutColumns').innerText);
            this.differentCols = noOfColumns; // we need this to check if you edit the project

            let columnTable = $("#tableLayoutColumns");
            columnTable.text("");
            columnTable.append("<tr></tr>");
            for (let i = 0; i < noOfColumns; i++) {
                columnTable.append(`<td style=\"background:RGBA(132,121,121,0,51); width:30%; height:100%;\" class=\"component\" id="column${i}">Trascina nella colonna ${i + 1}</td>`);
                $(`#column${i}`).droppable({
                    hoverClass: 'content-hover',
                    drop: (e, ui) => {
                        if (ui.draggable[0].parentElement.id === "") {
                            var ctrlType = ui.draggable.data('type');
                            this.addCustomComponent(ctrlType, i);
                            this.columns = i;
                            this.rows = $(`#column${i}`).children("div").length;
                        }
                        else {
                            if ($(`#column${i}`).children("div").length === 0) {
                                $(`#column${i}`).append(ui.draggable);
                            }
                        }
                        document.dispatchEvent(this.modelChanged);
                    }
                });
            }
        }

        private addExtraColumns(skipCols: number, layoutColumns: number) {
            let columnTable = $("#tableLayoutColumns");
            for (let i = layoutColumns - skipCols; i < layoutColumns; i++) {
                columnTable.append(`<td style=\"background:RGBA(132,121,121,0,51); width:30%; height:100%;\" class=\"component\" id="column${i}">Trascina nella colonna ${i + 1}</td>`);
                $(`#column${i}`).droppable({
                    accept: '.component',
                    hoverClass: 'content-hover',
                    drop: (e, ui) => {
                        var ctrlType = ui.draggable.data('type');
                        this.addCustomComponent(ctrlType, i);
                        this.columns = Number(i); //remember dropped column
                        this.rows = $(`#column${i}`).children("div").length; //remember dropped row
                        document.dispatchEvent(this.modelChanged);
                    }
                });
            }
        }

        // the form editor is a droppable area that accepts components,
        // converts them to elements and makes them sortable

        private addCustomComponent(componentType: string, indexColumn: number, elementClass: string = "element", addClose: boolean = true, addDrag: boolean = true): Element {
            var ctrl = $("#component-" + componentType).clone();
            ctrl.addClass(elementClass);
            ctrl.removeAttr('id');

            if (addDrag) {
                ctrl.draggable({
                    helper: function (e) {
                        return $(e.currentTarget).clone().addClass('component-drag');
                    }
                });
                ctrl.droppable({
                    hoverClass: 'content-hover',
                    drop: (e, ui) => {
                        if (ui.draggable[0].parentElement.id !== "") {
                            if (ui.draggable[0].parentElement.id === ctrl[0].parentElement.id) {
                                this.swapNodes(ui.draggable[0], ctrl[0]);
                            }
                            else {
                                ui.draggable.insertAfter(ctrl);
                            }
                            document.dispatchEvent(this.modelChanged);
                        }
                    }
                });
            }

            if (addClose)
                ctrl.prepend('<div class="close">&times;</div>');
            ctrl.appendTo(`#column${indexColumn}`);
            var ctrlData: CtrlBase = new UdsDesigner[componentType + "Ctrl"]();
            ctrlData.bind(ctrl[0]);
            ctrlData.afterAppend();

            $("#options_modal").modal('hide');
            return ctrl[0];
        }

        private swapNodes(a, b) {
            var aparent = a.parentNode;
            var asibling = a.nextSibling === b ? a : a.nextSibling;
            b.parentNode.insertBefore(a, b);
            aparent.insertBefore(b, asibling);
        }

        private initControls() {
            // components are useable form elements that can be dragged 
            $(".component").draggable({
                helper: function (e) {
                    return $(e.currentTarget).clone().addClass('component-drag');
                }
            });

            //click to add them to the form
            //$(".component").on('click', (e) => {
            //    var ctrlType = $(e.currentTarget).data('type');
            //    this.addComponent(ctrlType);
            //    document.dispatchEvent(this.modelChanged);
            //});

            $("#content").sortable({
                items: ".element",
                placeholder: "element-placeholder",
                start: function (e, ui) {
                    ui.item.popover('hide');
                },
                update: (e, ui) => {
                    document.dispatchEvent(this.modelChanged);
                }
            });

            $("#content").disableSelection();

            // elements are components that have been added to the form
            // clicking elements brings up customizable options in a
            // modal window
            $(document).on('click', '.element', (e) => {
                this.loadOptions($(e.currentTarget)[0], $(e.currentTarget).data('type'));
            });

            $(document).on('click', '.element-Title', (e) => {
                this.loadOptions($(e.currentTarget)[0], $(e.currentTarget).data('type'));
            });

            // remove element when clicking close button
            $(document).on('click', '.element > .close', (e) => {
                e.stopPropagation();

                $(e.currentTarget).parent().fadeOut('normal', () => {
                    var ctrl = CtrlBase.getElementCtrl($(e.currentTarget).parent()[0]);
                    ctrl.beforeRemoval();
                    $(e.currentTarget).parent().remove();
                    this.refreshElementsLayoutPosition(ctrl.columns); //update specific column when delete occured
                    document.dispatchEvent(this.modelChanged);
                });
            });

            //prevent default behaviour of element form items
            $(document).on('click', '.element > input, .element > button, .element > textarea, .element > label', function (e) {
                e.preventDefault();
            });

            // random bug makes options modal load when certain components are clicked. prevent this!
            $(".component > input, .component > textarea, .component > label, .checkbox, .radio").on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
            });
        }

        private refreshElementsLayoutPosition(column: number) {
            $(`#column${column}`).children("div").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (ctrl.rows > index + 1) {
                    ctrl.rows = index + 1;
                }
            });
        }
        // set current element
        private setElement(el: Element) {
            this.selected = el;
        }

        // get current element
        private getElement(): Element {
            return this.selected;
        }

        // update source code
        private updateSource() {
            this.jsonEdit.set({
                elements: this.getJsonEditElements()
            });
        }

        private getJsonEditElements(): any[] {

            var elements = [];

            $(".element-Title").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                elements.push(ctrl.getValues());
            });
            let allElementsArePositioned: boolean = true;
            $(".element").each((index, item) => {
                var ctrl = CtrlBase.getElementCtrl(item);
                if (!ctrl)
                    return;
                if (ctrl.columns === undefined && ctrl.rows === undefined) {
                    allElementsArePositioned = false;
                    ctrl.columns = this.columns;
                    ctrl.rows = this.rows;
                }
                elements.push(ctrl.getValues());
            });
            if (allElementsArePositioned) {
                let tempCollumns: number = 0;
                let rowIndex: number = 0;
                $(".element").each((index, item) => {
                    var ctrl = CtrlBase.getElementCtrl(item);
                    if (!ctrl)
                        return;
                    rowIndex++;
                    let currentColumn: number = Number(item.parentElement.id[item.parentElement.id.length - 1]);
                    ctrl.columns = currentColumn;
                    if (currentColumn !== tempCollumns) {
                        tempCollumns = currentColumn;
                        rowIndex = 1;
                    }
                    ctrl.rows = rowIndex;
                });
            }
            return elements;
        }

        public updateRequiredRevisionUDSRepository(value: boolean) {
            var elements = this.getJsonEditElements();
            for (let element of elements) {
                if (element.ctrlType === "Title") {
                    element.requiredRevisionUDSRepository = value;
                    break;
                }
            }
            this.jsonEdit.set({
                elements: elements
            });
        }

        // add component to form
        private addComponent(componentType: string, elementClass: string = "element", addClose: boolean = true): Element {

            var ctrl = $("#component-" + componentType).clone();
            ctrl.addClass(elementClass);
            ctrl.removeAttr('id');

            if (addClose)
                ctrl.prepend('<div class="close">&times;</div>');
            ctrl.appendTo("#content");

            var ctrlData: CtrlBase = new UdsDesigner[componentType + "Ctrl"]();
            ctrlData.bind(ctrl[0]);
            ctrlData.afterAppend();

            $("#options_modal").modal('hide');
            return ctrl[0];
        }

        // load element options
        private loadOptions(element: Element, componentType: string): boolean {
            var optionsDlg: JQuery = $("#options_modal");
            var content: JQuery = optionsDlg.find('.modal-body');

            // fail if no type set
            if (!componentType) {
                return false;
            }

            //carica il componente 
            Service.loadComponentOptions(componentType, (data) => {

                if (data === undefined) {
                    return false;
                }

                // set options modal componentType
                optionsDlg.data('type', componentType);

                // load relevant options
                content.html(data);

                //bind data clone
                var ctrlData = CtrlBase.getElementCtrl(element);
                ctrlData.initCallbacks.forEach(func => {
                    func(ctrlData);
                });

                this.tempData = ctrlData.clone();
                this.tempData.bind($("#" + componentType + "Option"));
                this.setElement(element);

                // show options modal
                optionsDlg.modal('show');
                return true;
            });

        }

        private tryRender(elements): boolean {
            try {
                this.generateDynamicColumns(elements);
                let sortedElements = this.sortByColumnsAndRows(elements);

                $.each(sortedElements, (index: number, item) => {
                    if (item.ctrlType == "Title") {
                        var ctrlTitle = this.addComponent("Title", "element-Title", false);
                        CtrlBase.getElementCtrl(ctrlTitle).setValues(item);
                    }
                    else {
                        var ctrl = this.addCustomComponent(item.ctrlType, item.columns);
                        CtrlBase.getElementCtrl(ctrl).setValues(item);
                    }
                });
                return true;
            }
            catch (err) {
                return false;
            }
        }

        private sortByColumnsAndRows(elements: any) {

            return elements.sort((a, b) => {
                var aCol = a.columns;
                var bCol = b.columns;
                var aRow = a.rows;
                var bRow = b.rows;
                if (aCol == bCol)
                    return (aRow < bRow) ? -1 : (aRow > bRow) ? 1 : 0;
                else
                    return (aCol < bCol) ? -1 : 1;
            });
        }

        private generateDynamicColumns(elements: any) {
            let sortedElements = this.sortByColumnsAndRows(elements);
            let noOfColumns = sortedElements[sortedElements.length - 1].columns + 1; //+1 starting from zero
            this.differentCols = noOfColumns; // we need this to check if you edit the project

            let columnTable = $("#tableLayoutColumns");
            columnTable.text("");
            columnTable.append("<tr></tr>");
            for (let i = 0; i < noOfColumns; i++) {
                columnTable.append(`<td style=\"background:RGBA(132,121,121,0,51); width:30%; height:100%;\" class=\"component\" id="column${i}">Trascina nella colonna ${i + 1}</td>`);
                $(`#column${i}`).droppable({
                    accept: '.component',
                    hoverClass: 'content-hover',
                    drop: (e, ui) => {
                        var ctrlType = ui.draggable.data('type');
                        this.addCustomComponent(ctrlType, i);
                        this.columns = i;
                        this.rows = $(`#column${i}`).children("div").length;
                        document.dispatchEvent(this.modelChanged);
                    }
                });
            }
        }

        private checkHeaders() {
            var idx: number = 2;
            var equals: boolean = true;

            while (equals) {
                equals = false;

                $(".element[data-type='Header']").each((index, sourceElement) => {
                    var source: HeaderCtrl = <HeaderCtrl>CtrlBase.getElementCtrl(sourceElement);

                    $(".element[data-type='Header']").each((index, destElement) => {
                        if (destElement !== sourceElement) {
                            var dest: HeaderCtrl = <HeaderCtrl>CtrlBase.getElementCtrl(destElement);
                            if (source.label == dest.label) {
                                dest.label = source.label.replace(/\d+/g, '') + idx.toString();
                                idx++;
                                equals = true;
                            }
                        }
                    });
                });
            }
        }

        private onModelChanged(e: Event) {

            this.checkHeaders();
            this.updateSource();
            var model = this.getModelJson();
            var modelJson = '{jsModel:' + JSON.stringify(model) + '}';

            Service.validateModel(modelJson, (response) => {
                this.status.error = response.d.error;
                this.status.messages = response.d.messages;

                if (!this.status.error) {
                    Service.saveTempModel(modelJson, (response) => {
                    });
                }
            });

        }
    }


    class FormStatus {
        messages: string[] = [];
        error: boolean = false;
    }


}