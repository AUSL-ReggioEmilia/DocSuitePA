/// <reference path="../../../../Scripts/typings/jquery/jquery.d.ts" />

$(function () {
    setCulture("it-IT");
    addKendoTreeListTranslation();
});

function setCulture(cultureName): void{
    kendo.culture(cultureName);
}

function addKendoTreeListTranslation(): void {
    if (kendo.ui.TreeList) {
        kendo.ui.TreeList.prototype.options.messages =
            $.extend(true, kendo.ui.TreeList.prototype.options.messages, {
                "commands": {
                    "cancel": "Annulla",
                    "canceledit": "Annulla",
                    "create": "Aggiungi",
                    "destroy": "Rimuovi",
                    "edit": "Modifica",
                    "excel": "Esporta su Excel",
                    "pdf": "Esporta su PDF",
                    "save": "Salva",
                    "select": "Seleziona",
                    "update": "Conferma"
                },
                "editable": {
                    "cancelDelete": "Annulla",
                    "confirmation": "Conferma",
                    "confirmDelete": "Conferma"
                }
            });
    }
}