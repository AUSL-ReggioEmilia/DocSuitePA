/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../UdsDesigner/FormBuilder.ts" />
/// <reference path="../../UdsDesigner/Service.ts" />
/// <reference path="../Service/PathService.ts" />

module UdsDesigner {

    declare var moment: any;
    export class DesignerController {
        //Fields
        private form_builder: UdsDesigner.FormBuilder;
        private idRepository: string;
        private activeDateModal: any;

        //Constructor
        constructor() {
            this.form_builder = new UdsDesigner.FormBuilder();
            this.form_builder.setup();
            this.idRepository = new UdsDesigner.PathService().Querystring("IdUds");
            this.initializeEvents();
            this.initialize();
            this.activeDateModal = $("#activeDate_modal");
        }

        //Functions
        private initializeEvents(): void {
            $("#menuNew").on('click', (e) => {
                UdsDesigner.Service.newDocument((elements) => {
                    this.form_builder.renderModel(elements);
                });
            });


            $("#menuSample").on('click', (e) => {
                UdsDesigner.Service.loadSample((elements) => {
                    this.form_builder.renderModel(elements);
                });
            });

            $("#menuSave").on('click', (e) => {
                if (this.form_builder.status.error) {
                    alert("Operazione annullata. Verificare che l'archivio abbia superato i vincoli di validazione");
                    return;
                }

                UdsDesigner.Service.saveModel(this.form_builder.getModelJson(), false, (response) => {
                    $("#alertSuccessMessage").show().delay(3000).fadeOut(1000);
                    rivets.bind($("#alertSuccessMessage"), { message: "Archivio salvato nelle bozze" });
                    $("#menuNew").trigger("click");
                }, moment());
            });

            $("#menuPublish").on('click', (e) => {
                if (this.form_builder.status.error) {
                    alert("Operazione annullata. Verificare che l'archivio abbia superato i vincoli di validazione");
                    return;
                }

                $("#lblDuplicate").hide();
                $("#activeDuplicate").hide();
                $("#activeDateTime").data("DateTimePicker").clear();
                this.activeDateModal.modal("show");
                if (this.idRepository !== undefined) {
                    UdsDesigner.Service.loadRepositoryVersionByRepositoryId(this.idRepository, (response) => {
                        if (response.d.Version > 0) {
                            $("#lblDuplicate").show();
                            $("#activeDuplicate").show();
                        }
                        $("#chkDuplicate").prop('checked', false);
                    });
                }
            });

            $("#btnPublish").on('click', (e) => {
                e.preventDefault();

                if (this.form_builder.status.error) {
                    alert("Operazione annullata. Verificare che l'archivio abbia superato i vincoli di validazione");
                    return;
                }

                var activeDate: Date = $("#activeDateTime").data("DateTimePicker").date();
                if (activeDate == null || activeDate == undefined) {
                    alert("La Data di attivazione è obbligatoria");
                    this.activeDateModal.modal("hide");
                } else {
                    this.activeDateModal.modal("hide");
                    var duplicate: boolean = $("#chkDuplicate").is(':checked');
                    this.form_builder.updateRequiredRevisionUDSRepository(duplicate);
                    UdsDesigner.Service.saveModel(this.form_builder.getModelJson(), true, (response) => {
                        $("#alertSuccessMessage").show().delay(3000).fadeOut(1000);
                        rivets.bind($("#alertSuccessMessage"), { message: "Messaggio di pubblicazione inviato. Attendere alcuni minuti per verificare l'esito di pubblicazione nella ricerca degli archivi" });
                        $("#menuNew").trigger("click");
                    }, activeDate);
                }
            });

            $(window).resize(() => {
                this.form_builder.resizeEditor();
            });
        }

        private initialize(): void {
            if (this.idRepository == undefined || this.idRepository == "") {
                UdsDesigner.Service.newDocument((elements) => {
                    this.form_builder.renderModel(elements);
                });
            } else {
                UdsDesigner.Service.loadRepository(this.idRepository, (response) => {
                    if (response.d != null && response.d.elements != null) {
                        this.form_builder.renderModel(response.d.elements);
                    }

                    this.form_builder.resizeEditor();
                });
            }

            $("#activeDateTime").datetimepicker({
                showClear: true,
                locale: 'it',
                format: "DD/MM/YYYY"
            });
        }
    }
}