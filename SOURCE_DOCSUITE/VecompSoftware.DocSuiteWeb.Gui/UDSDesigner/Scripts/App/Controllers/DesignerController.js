/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../UdsDesigner/FormBuilder.ts" />
/// <reference path="../../UdsDesigner/Service.ts" />
/// <reference path="../Service/PathService.ts" />
var UdsDesigner;
(function (UdsDesigner) {
    var DesignerController = /** @class */ (function () {
        //Constructor
        function DesignerController() {
            this.form_builder = new UdsDesigner.FormBuilder();
            this.form_builder.setup();
            this.idRepository = new UdsDesigner.PathService().Querystring("IdUds");
            this.initializeEvents();
            this.initialize();
            this.activeDateModal = $("#activeDate_modal");
        }
        //Functions
        DesignerController.prototype.initializeEvents = function () {
            var _this = this;
            $("#menuNew").on('click', function (e) {
                UdsDesigner.Service.newDocument(function (elements) {
                    _this.form_builder.renderModel(elements);
                });
            });
            $("#menuSample").on('click', function (e) {
                UdsDesigner.Service.loadSample(function (elements) {
                    _this.form_builder.renderModel(elements);
                });
            });
            $("#menuSave").on('click', function (e) {
                if (_this.form_builder.status.error) {
                    alert("Operazione annullata. Verificare che l'archivio abbia superato i vincoli di validazione");
                    return;
                }
                UdsDesigner.Service.saveModel(_this.form_builder.getModelJson(), false, function (response) {
                    $("#alertSuccessMessage").show().delay(3000).fadeOut(1000);
                    rivets.bind($("#alertSuccessMessage"), { message: "Archivio salvato nelle bozze" });
                    $("#menuNew").trigger("click");
                }, moment());
            });
            $("#menuPublish").on('click', function (e) {
                if (_this.form_builder.status.error) {
                    alert("Operazione annullata. Verificare che l'archivio abbia superato i vincoli di validazione");
                    return;
                }
                $("#lblDuplicate").hide();
                $("#activeDuplicate").hide();
                $("#activeDateTime").data("DateTimePicker").clear();
                _this.activeDateModal.modal("show");
                if (_this.idRepository !== undefined) {
                    UdsDesigner.Service.loadRepositoryVersionByRepositoryId(_this.idRepository, function (response) {
                        if (response.d.Version > 0) {
                            $("#lblDuplicate").show();
                            $("#activeDuplicate").show();
                        }
                        $("#chkDuplicate").prop('checked', false);
                    });
                }
            });
            $("#btnPublish").on('click', function (e) {
                e.preventDefault();
                if (_this.form_builder.status.error) {
                    alert("Operazione annullata. Verificare che l'archivio abbia superato i vincoli di validazione");
                    return;
                }
                var activeDate = $("#activeDateTime").data("DateTimePicker").date();
                if (activeDate == null || activeDate == undefined) {
                    alert("La Data di attivazione è obbligatoria");
                    _this.activeDateModal.modal("hide");
                }
                else {
                    _this.activeDateModal.modal("hide");
                    var duplicate = $("#chkDuplicate").is(':checked');
                    _this.form_builder.updateRequiredRevisionUDSRepository(duplicate);
                    UdsDesigner.Service.saveModel(_this.form_builder.getModelJson(), true, function (response) {
                        $("#alertSuccessMessage").show().delay(3000).fadeOut(1000);
                        rivets.bind($("#alertSuccessMessage"), { message: "Messaggio di pubblicazione inviato. Attendere alcuni minuti per verificare l'esito di pubblicazione nella ricerca degli archivi" });
                        $("#menuNew").trigger("click");
                    }, activeDate);
                }
            });
            $(window).resize(function () {
                _this.form_builder.resizeEditor();
            });
        };
        DesignerController.prototype.initialize = function () {
            var _this = this;
            if (this.idRepository == undefined || this.idRepository == "") {
                UdsDesigner.Service.newDocument(function (elements) {
                    _this.form_builder.renderModel(elements);
                });
            }
            else {
                UdsDesigner.Service.loadRepository(this.idRepository, function (response) {
                    if (response.d != null && response.d.elements != null) {
                        _this.form_builder.renderModel(response.d.elements);
                    }
                    _this.form_builder.resizeEditor();
                });
            }
            $("#activeDateTime").datetimepicker({
                showClear: true,
                locale: 'it',
                format: "DD/MM/YYYY"
            });
        };
        return DesignerController;
    }());
    UdsDesigner.DesignerController = DesignerController;
})(UdsDesigner || (UdsDesigner = {}));
//# sourceMappingURL=DesignerController.js.map