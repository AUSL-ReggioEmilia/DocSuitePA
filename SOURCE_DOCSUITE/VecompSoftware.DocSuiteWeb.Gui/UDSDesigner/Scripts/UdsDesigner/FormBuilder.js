/// <reference path="../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../Scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../Scripts/typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../app/utils.ts" />
/// <reference path="controls.ts" />
var UdsDesigner;
(function (UdsDesigner) {
    var FormBuilder = /** @class */ (function () {
        function FormBuilder() {
            this.delimeter = '='; // delimiter to split value from display text in option based input
            this.selected = null;
            this.tempData = null;
            this.jsonEdit = null;
            this.modelChanged = null;
            this.status = null;
            this.statusBinder = null;
            this.differentCols = -1; //check if columns layout is changed, if not, don't delete the existing components
        }
        FormBuilder.prototype.setup = function () {
            this.initEvents();
            this.initEditor();
            this.initModals();
            this.initControls();
        };
        FormBuilder.prototype.resizeEditor = function () {
            var height = window.innerHeight;
            $("#source").height(height - 110);
            this.jsonEdit.editor.resize();
        };
        FormBuilder.prototype.renderModel = function (elements) {
            if (this.tryRender(elements)) {
                $("#content").html("");
                this.tryRender(elements);
                document.dispatchEvent(this.modelChanged);
            }
        };
        FormBuilder.prototype.getModelJson = function () {
            return this.jsonEdit.get();
        };
        FormBuilder.prototype.initEvents = function () {
            var _this = this;
            this.modelChanged = document.createEvent("Event");
            this.modelChanged.initEvent('modelChanged', true, true);
            document.addEventListener('modelChanged', function (e) {
                _this.onModelChanged(e);
            }, false);
        };
        FormBuilder.prototype.initEditor = function () {
            var _this = this;
            var editorOptions = {
                mode: 'code',
                indentation: 2,
                error: function (err) {
                    alert(err.toString());
                }
            };
            $("#source").empty();
            this.jsonEdit = new JSONEditor($("#source")[0], editorOptions);
            this.jsonEdit.editor.addEventListener("blur", function () {
                var model = _this.getModelJson();
                if (model != null)
                    _this.renderModel(model.elements);
            });
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                _this.jsonEdit.editor.resize();
                _this.jsonEdit.editor.renderer.updateFull();
                document.dispatchEvent(_this.modelChanged);
            });
        };
        FormBuilder.prototype.initModals = function () {
            var _this = this;
            //gestione stato della form
            this.status = new FormStatus();
            this.statusBinder = rivets.bind($("#status")[0], { status: this.status });
            $('#options_modal').on('shown.bs.modal', function (e) {
                if (!isNaN(_this.differentCols))
                    $("#itemChanger").val(_this.differentCols).trigger('change'); //bind layout column to existing value
                $(e.currentTarget).find("input").first().focus();
            });
            $("#options_modal").submit(function (e) {
                e.preventDefault();
                return false;
            });
            // options modal save button
            $("#save_options").on('click', function (e) {
                e.preventDefault();
                var optionsDlg = $("#options_modal");
                var content = optionsDlg.find('.modal-body');
                var component = optionsDlg.data('type');
                //aggiorna i valori
                var newValue = _this.tempData.clone();
                var ctrl = UdsDesigner.CtrlBase.getElementCtrl(_this.getElement());
                ctrl.onConfirm();
                newValue.bind(_this.getElement());
                optionsDlg.modal('hide');
                document.dispatchEvent(_this.modelChanged);
                if (component === "Title" && _this.differentCols != Number(document.getElementById('layoutColumns').innerText))
                    if (_this.differentCols <= Number(document.getElementById('layoutColumns').innerText)) {
                        var skipCol = Number(document.getElementById('layoutColumns').innerText) - _this.differentCols;
                        _this.createLayoutColumns(true, skipCol);
                    }
                    else {
                        _this.createLayoutColumns(false);
                    }
            });
        };
        FormBuilder.prototype.createLayoutColumns = function (diffCols, skipCol) {
            var skipCols = 0;
            var layoutColumns = Number(document.getElementById('layoutColumns').innerText);
            this.differentCols = layoutColumns;
            var columnTable = $("#tableLayoutColumns");
            if (!diffCols) {
                columnTable.text("");
                columnTable.append("<tr></tr>");
                this.removeExtraColumns();
            }
            else {
                skipCols = skipCol;
                this.addExtraColumns(skipCols, layoutColumns);
            }
        };
        FormBuilder.prototype.removeExtraColumns = function () {
            var _this = this;
            var model = this.getModelJson();
            var colsToHave = Number(document.getElementById('layoutColumns').innerText) - 1;
            var adjustRow = 0;
            $.each(model.elements, function (index, element) {
                if (element.columns > colsToHave) {
                    element.columns = colsToHave;
                    element.rows = adjustRow + 1;
                    adjustRow++;
                }
                else if (element.columns === colsToHave) {
                    adjustRow++;
                }
            });
            try {
                this.generateForRemoval(model.elements);
                var sortedElements = this.sortByColumnsAndRows(model.elements);
                $.each(sortedElements, function (index, item) {
                    if (item.ctrlType == "Title") { }
                    else {
                        var ctrl = _this.addCustomComponent(item.ctrlType, item.columns);
                        UdsDesigner.CtrlBase.getElementCtrl(ctrl).setValues(item);
                    }
                });
                this.onModelChanged(model.elements);
            }
            catch (err) {
                console.log(err);
            }
        };
        FormBuilder.prototype.generateForRemoval = function (elements) {
            var _this = this;
            var noOfColumns = Number(document.getElementById('layoutColumns').innerText);
            this.differentCols = noOfColumns; // we need this to check if you edit the project
            var columnTable = $("#tableLayoutColumns");
            columnTable.text("");
            columnTable.append("<tr></tr>");
            var _loop_1 = function (i) {
                columnTable.append("<td style=\"background:RGBA(132,121,121,0,51); width:30%; height:100%;\" class=\"component\" id=\"column" + i + "\">Trascina nella colonna " + (i + 1) + "</td>");
                $("#column" + i).droppable({
                    accept: '.component',
                    hoverClass: 'content-hover',
                    drop: function (e, ui) {
                        var ctrlType = ui.draggable.data('type');
                        _this.addCustomComponent(ctrlType, i);
                        _this.columns = i;
                        _this.rows = $("#column" + i).children("div").length;
                        document.dispatchEvent(_this.modelChanged);
                    }
                });
            };
            for (var i = 0; i < noOfColumns; i++) {
                _loop_1(i);
            }
        };
        FormBuilder.prototype.addExtraColumns = function (skipCols, layoutColumns) {
            var _this = this;
            var columnTable = $("#tableLayoutColumns");
            var _loop_2 = function (i) {
                columnTable.append("<td style=\"background:RGBA(132,121,121,0,51); width:30%; height:100%;\" class=\"component\" id=\"column" + i + "\">Trascina nella colonna " + (i + 1) + "</td>");
                $("#column" + i).droppable({
                    accept: '.component',
                    hoverClass: 'content-hover',
                    drop: function (e, ui) {
                        var ctrlType = ui.draggable.data('type');
                        _this.addCustomComponent(ctrlType, i);
                        _this.columns = Number(i); //remember dropped column
                        _this.rows = $("#column" + i).children("div").length; //remember dropped row
                        document.dispatchEvent(_this.modelChanged);
                    }
                });
            };
            for (var i = layoutColumns - skipCols; i < layoutColumns; i++) {
                _loop_2(i);
            }
        };
        // the form editor is a droppable area that accepts components,
        // converts them to elements and makes them sortable
        FormBuilder.prototype.addCustomComponent = function (componentType, indexColumn, elementClass, addClose) {
            if (elementClass === void 0) { elementClass = "element"; }
            if (addClose === void 0) { addClose = true; }
            //debugger;
            var ctrl = $("#component-" + componentType).clone();
            ctrl.addClass(elementClass);
            ctrl.removeAttr('id');
            if (addClose)
                ctrl.prepend('<div class="close">&times;</div>');
            ctrl.appendTo("#column" + indexColumn);
            var ctrlData = new UdsDesigner[componentType + "Ctrl"]();
            ctrlData.bind(ctrl[0]);
            ctrlData.afterAppend();
            $("#options_modal").modal('hide');
            return ctrl[0];
        };
        FormBuilder.prototype.initControls = function () {
            var _this = this;
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
                update: function (e, ui) {
                    document.dispatchEvent(_this.modelChanged);
                }
            });
            $("#content").disableSelection();
            // elements are components that have been added to the form
            // clicking elements brings up customizable options in a
            // modal window
            $(document).on('click', '.element', function (e) {
                _this.loadOptions($(e.currentTarget)[0], $(e.currentTarget).data('type'));
            });
            $(document).on('click', '.element-Title', function (e) {
                _this.loadOptions($(e.currentTarget)[0], $(e.currentTarget).data('type'));
            });
            // remove element when clicking close button
            $(document).on('click', '.element > .close', function (e) {
                e.stopPropagation();
                $(e.currentTarget).parent().fadeOut('normal', function () {
                    var ctrl = UdsDesigner.CtrlBase.getElementCtrl($(e.currentTarget).parent()[0]);
                    ctrl.beforeRemoval();
                    $(e.currentTarget).parent().remove();
                    _this.refreshElementsLayoutPosition(ctrl.columns); //update specific column when delete occured
                    document.dispatchEvent(_this.modelChanged);
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
        };
        FormBuilder.prototype.refreshElementsLayoutPosition = function (column) {
            $("#column" + column).children("div").each(function (index, item) {
                var ctrl = UdsDesigner.CtrlBase.getElementCtrl(item);
                if (ctrl.rows > index + 1) {
                    ctrl.rows = index + 1;
                }
            });
        };
        // set current element
        FormBuilder.prototype.setElement = function (el) {
            this.selected = el;
        };
        // get current element
        FormBuilder.prototype.getElement = function () {
            return this.selected;
        };
        // update source code
        FormBuilder.prototype.updateSource = function () {
            this.jsonEdit.set({
                elements: this.getJsonEditElements()
            });
        };
        FormBuilder.prototype.getJsonEditElements = function () {
            var _this = this;
            var elements = [];
            $(".element-Title").each(function (index, item) {
                var ctrl = UdsDesigner.CtrlBase.getElementCtrl(item);
                elements.push(ctrl.getValues());
            });
            $(".element").each(function (index, item) {
                var ctrl = UdsDesigner.CtrlBase.getElementCtrl(item);
                if (ctrl.columns === undefined && ctrl.rows === undefined) {
                    ctrl.columns = _this.columns;
                    ctrl.rows = _this.rows;
                }
                elements.push(ctrl.getValues());
            });
            return elements;
        };
        FormBuilder.prototype.updateRequiredRevisionUDSRepository = function (value) {
            var elements = this.getJsonEditElements();
            for (var _i = 0, elements_1 = elements; _i < elements_1.length; _i++) {
                var element = elements_1[_i];
                if (element.ctrlType === "Title") {
                    element.requiredRevisionUDSRepository = value;
                    break;
                }
            }
            this.jsonEdit.set({
                elements: elements
            });
        };
        // add component to form
        FormBuilder.prototype.addComponent = function (componentType, elementClass, addClose) {
            if (elementClass === void 0) { elementClass = "element"; }
            if (addClose === void 0) { addClose = true; }
            var ctrl = $("#component-" + componentType).clone();
            ctrl.addClass(elementClass);
            ctrl.removeAttr('id');
            if (addClose)
                ctrl.prepend('<div class="close">&times;</div>');
            ctrl.appendTo("#content");
            var ctrlData = new UdsDesigner[componentType + "Ctrl"]();
            ctrlData.bind(ctrl[0]);
            ctrlData.afterAppend();
            $("#options_modal").modal('hide');
            return ctrl[0];
        };
        // load element options
        FormBuilder.prototype.loadOptions = function (element, componentType) {
            var _this = this;
            var optionsDlg = $("#options_modal");
            var content = optionsDlg.find('.modal-body');
            // fail if no type set
            if (!componentType) {
                return false;
            }
            //carica il componente 
            UdsDesigner.Service.loadComponentOptions(componentType, function (data) {
                if (data === undefined) {
                    return false;
                }
                // set options modal componentType
                optionsDlg.data('type', componentType);
                // load relevant options
                content.html(data);
                //bind data clone
                var ctrlData = UdsDesigner.CtrlBase.getElementCtrl(element);
                ctrlData.initCallbacks.forEach(function (func) {
                    func(ctrlData);
                });
                _this.tempData = ctrlData.clone();
                _this.tempData.bind($("#" + componentType + "Option"));
                _this.setElement(element);
                // show options modal
                optionsDlg.modal('show');
                return true;
            });
        };
        FormBuilder.prototype.tryRender = function (elements) {
            var _this = this;
            try {
                this.generateDynamicColumns(elements);
                var sortedElements = this.sortByColumnsAndRows(elements);
                $.each(sortedElements, function (index, item) {
                    if (item.ctrlType == "Title") {
                        var ctrlTitle = _this.addComponent("Title", "element-Title", false);
                        UdsDesigner.CtrlBase.getElementCtrl(ctrlTitle).setValues(item);
                    }
                    else {
                        var ctrl = _this.addCustomComponent(item.ctrlType, item.columns);
                        UdsDesigner.CtrlBase.getElementCtrl(ctrl).setValues(item);
                    }
                });
                return true;
            }
            catch (err) {
                return false;
            }
        };
        FormBuilder.prototype.sortByColumnsAndRows = function (elements) {
            return elements.sort(function (a, b) {
                var aCol = a.columns;
                var bCol = b.columns;
                var aRow = a.rows;
                var bRow = b.rows;
                if (aCol == bCol)
                    return (aRow < bRow) ? -1 : (aRow > bRow) ? 1 : 0;
                else
                    return (aCol < bCol) ? -1 : 1;
            });
        };
        FormBuilder.prototype.generateDynamicColumns = function (elements) {
            var _this = this;
            var sortedElements = this.sortByColumnsAndRows(elements);
            var noOfColumns = sortedElements[sortedElements.length - 1].columns + 1; //+1 starting from zero
            this.differentCols = noOfColumns; // we need this to check if you edit the project
            var columnTable = $("#tableLayoutColumns");
            columnTable.text("");
            columnTable.append("<tr></tr>");
            var _loop_3 = function (i) {
                columnTable.append("<td style=\"background:RGBA(132,121,121,0,51); width:30%; height:100%;\" class=\"component\" id=\"column" + i + "\">Trascina nella colonna " + (i + 1) + "</td>");
                $("#column" + i).droppable({
                    accept: '.component',
                    hoverClass: 'content-hover',
                    drop: function (e, ui) {
                        var ctrlType = ui.draggable.data('type');
                        _this.addCustomComponent(ctrlType, i);
                        _this.columns = i;
                        _this.rows = $("#column" + i).children("div").length;
                        document.dispatchEvent(_this.modelChanged);
                    }
                });
            };
            for (var i = 0; i < noOfColumns; i++) {
                _loop_3(i);
            }
        };
        FormBuilder.prototype.checkHeaders = function () {
            var idx = 2;
            var equals = true;
            while (equals) {
                equals = false;
                $(".element[data-type='Header']").each(function (index, sourceElement) {
                    var source = UdsDesigner.CtrlBase.getElementCtrl(sourceElement);
                    $(".element[data-type='Header']").each(function (index, destElement) {
                        if (destElement !== sourceElement) {
                            var dest = UdsDesigner.CtrlBase.getElementCtrl(destElement);
                            if (source.label == dest.label) {
                                dest.label = source.label.replace(/\d+/g, '') + idx.toString();
                                idx++;
                                equals = true;
                            }
                        }
                    });
                });
            }
        };
        FormBuilder.prototype.onModelChanged = function (e) {
            var _this = this;
            this.checkHeaders();
            this.updateSource();
            var model = this.getModelJson();
            var modelJson = '{jsModel:' + JSON.stringify(model) + '}';
            UdsDesigner.Service.validateModel(modelJson, function (response) {
                _this.status.error = response.d.error;
                _this.status.messages = response.d.messages;
                if (!_this.status.error) {
                    UdsDesigner.Service.saveTempModel(modelJson, function (response) {
                    });
                }
            });
        };
        return FormBuilder;
    }());
    UdsDesigner.FormBuilder = FormBuilder;
    var FormStatus = /** @class */ (function () {
        function FormStatus() {
            this.messages = [];
            this.error = false;
        }
        return FormStatus;
    }());
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=FormBuilder.js.map