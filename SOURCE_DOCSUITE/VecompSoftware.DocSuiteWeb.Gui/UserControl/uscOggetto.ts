/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/helpers/stringhelper.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

declare var ValidatorEnable: any;
class uscOggetto {
    validationId: string;
    contentId: string;
    txtObjectId: string;

    public static LOADED_EVENT: string = "onLoaded";

    constructor() {
    }

    /**
     *------------------------- Events -----------------------------
     */


    /**
     *------------------------- Methods -----------------------------
     */
    /**
    * Metodo di inizializzazione
    */
    initialize(): void {
        this.bindLoaded();
    }

    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.contentId)).data(this);
        $("#".concat(this.contentId)).triggerHandler(uscOggetto.LOADED_EVENT);

    }

    enableVaidators = (state: boolean) => {
        ValidatorEnable(document.getElementById(this.validationId), state)
    }

    getText = () => {
        let txtOggetto: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        return txtOggetto.get_textBoxValue();
    }

    isValid = () => {
        let txtOggetto: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        if (txtOggetto.get_maxLength() != 0 && txtOggetto.get_textBoxValue().length > txtOggetto.get_maxLength()) {            
            return false;
        }
        return true;
    }

    getMaxLength = () => {
        let txtOggetto: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        return txtOggetto.get_maxLength();
    }
}

export = uscOggetto;