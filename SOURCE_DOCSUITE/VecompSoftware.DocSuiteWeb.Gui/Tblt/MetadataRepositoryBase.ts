import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

abstract class MetadataRepositoryBase {

    protected static METADATA_REPOSITORY_NAME = "MetadataRepository";

    private _serviceConfiguration: ServiceConfiguration;
    protected _service: MetadataRepositoryService;

    constructor(serviceConfiguration: ServiceConfiguration) {
        this._serviceConfiguration = serviceConfiguration;
    }

    initialize() {
        this._service = new MetadataRepositoryService(this._serviceConfiguration);
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    /**
     * funzione che rimuove tutti gli elementi dalla pagina
     */
    public clearPage() {
        let content: HTMLElement = document.getElementById("menuContent");

        while (content.firstChild) {
            content.removeChild(content.firstChild);
        }
    }

    /**
     * funzione che clona un elemento del DOM
     * @param elementId
     */
    cloneElement(elementId: string, incremental: number): string {
        let element;
        let content: HTMLElement = document.getElementById("menuContent");
        element = document.getElementById(elementId);
        let cln = element.cloneNode(true);
        cln.setAttribute("id", element.id + incremental)
        content.appendChild(cln);

        return cln.id;
    }

    /**
     * funzione che cerca nel DOM gli elementi di input specifici della posizione specificatata
     * @param idElement
     * @param position
     */
    findStandardInputElement(idElement: string, position: number): HTMLInputElement {
        return this.findGenericInputControl(idElement, position, "form-control");
    }

    /**
     * funzione che cerca nel DOM gli elementi di input relativi alla classe di stile desiderata della posizione specificatata
     * @param idElement
     * @param position
     * @param cssClass
     */
    findGenericInputControl(idElement: string, position: number, cssClass: string): HTMLInputElement {
        let inputElement: HTMLInputElement = <HTMLInputElement>($("#".concat(idElement, " :input.", cssClass))[position]);
        return inputElement;
    }

    findSelectControl(idElement: string, position: number): HTMLSelectElement {
        let selectElement: HTMLSelectElement = <HTMLSelectElement>$("#" + idElement).find('select')[position];
        return selectElement;
    }

    /**
     * funzione che cerca nel DOM gli elementi di tipo checkbox della posizione specificatata
     * @param idElement
     * @param position
     */
    findInputCheckBoxElement(idElement: string, position: number): HTMLInputElement {
        return this.findGenericInputControl(idElement, position, "checkBox");
    }

    /**
     * funzione che cerca nel DOM gli elementi di label nella posizione specificata
     * @param idElement
     * @param position
     */
    findLabelElement(idElement: string, position: number): HTMLLabelElement {
        let labelElement: HTMLLabelElement = <HTMLLabelElement>$("#" + idElement).find('label')[position];
        return labelElement;
    }

    /**
     * funzione che cerca nel DOM gli elementi di  tipo ul nella posizione specificata
     * @param idElement
     * @param position
     */
    findListElement(idElement: string, position: number): HTMLOListElement {
        let listElement: HTMLOListElement = <HTMLOListElement>($("#" + idElement).find('ul')[position]);
        return listElement;
    }

    /**
     * Creo un elemento e lo aggiungo alla componente
     * @param nodeText
     */
    createNewNode(nodeText: string, node: HTMLElement, idElement: string) {
        let listElement: HTMLOListElement;
        let textNode = document.createTextNode(nodeText);
        node.appendChild(textNode);
        listElement = this.findListElement(idElement, 0);
        listElement.appendChild(node);
    }
}
export = MetadataRepositoryBase;