define(["require", "exports", "App/Services/Commons/MetadataRepositoryService", "App/DTOs/ExceptionDTO"], function (require, exports, MetadataRepositoryService, ExceptionDTO) {
    var MetadataRepositoryBase = /** @class */ (function () {
        function MetadataRepositoryBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        MetadataRepositoryBase.prototype.initialize = function () {
            this._service = new MetadataRepositoryService(this._serviceConfiguration);
        };
        MetadataRepositoryBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        MetadataRepositoryBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        /**
         * funzione che rimuove tutti gli elementi dalla pagina
         */
        MetadataRepositoryBase.prototype.clearPage = function () {
            var content = document.getElementById("menuContent");
            while (content.firstChild) {
                content.removeChild(content.firstChild);
            }
        };
        /**
         * funzione che clona un elemento del DOM
         * @param elementId
         */
        MetadataRepositoryBase.prototype.cloneElement = function (elementId, incremental) {
            var element;
            var content = document.getElementById("menuContent");
            element = document.getElementById(elementId);
            var cln = element.cloneNode(true);
            cln.setAttribute("id", element.id + incremental);
            content.appendChild(cln);
            return cln.id;
        };
        /**
         * funzione che cerca nel DOM gli elementi di input specifici della posizione specificatata
         * @param idElement
         * @param position
         */
        MetadataRepositoryBase.prototype.findStandardInputElement = function (idElement, position) {
            return this.findGenericInputControl(idElement, position, "form-control");
        };
        /**
         * funzione che cerca nel DOM gli elementi di input relativi alla classe di stile desiderata della posizione specificatata
         * @param idElement
         * @param position
         * @param cssClass
         */
        MetadataRepositoryBase.prototype.findGenericInputControl = function (idElement, position, cssClass) {
            var inputElement = ($("#".concat(idElement, " :input.", cssClass))[position]);
            return inputElement;
        };
        MetadataRepositoryBase.prototype.findSelectControl = function (idElement, position) {
            var selectElement = $("#" + idElement).find('select')[position];
            return selectElement;
        };
        /**
         * funzione che cerca nel DOM gli elementi di tipo checkbox della posizione specificatata
         * @param idElement
         * @param position
         */
        MetadataRepositoryBase.prototype.findInputCheckBoxElement = function (idElement, position) {
            return this.findGenericInputControl(idElement, position, "checkBox");
        };
        /**
         * funzione che cerca nel DOM gli elementi di label nella posizione specificata
         * @param idElement
         * @param position
         */
        MetadataRepositoryBase.prototype.findLabelElement = function (idElement, position) {
            var labelElement = $("#" + idElement).find('label')[position];
            return labelElement;
        };
        /**
         * funzione che cerca nel DOM gli elementi di  tipo ul nella posizione specificata
         * @param idElement
         * @param position
         */
        MetadataRepositoryBase.prototype.findListElement = function (idElement, position) {
            var listElement = ($("#" + idElement).find('ul')[position]);
            return listElement;
        };
        /**
         * Creo un elemento e lo aggiungo alla componente
         * @param nodeText
         */
        MetadataRepositoryBase.prototype.createNewNode = function (nodeText, node, idElement) {
            var listElement;
            var textNode = document.createTextNode(nodeText);
            node.appendChild(textNode);
            listElement = this.findListElement(idElement, 0);
            listElement.appendChild(node);
        };
        MetadataRepositoryBase.METADATA_REPOSITORY_NAME = "MetadataRepository";
        MetadataRepositoryBase.CONTROL_TEXT_FIELD = "TextFields";
        MetadataRepositoryBase.CONTROL_DATE_FIELD = "DateFields";
        MetadataRepositoryBase.CONTROL_NUMBER_FIELD = "NumberFields";
        MetadataRepositoryBase.CONTROL_BOOL_FIELD = "BoolFields";
        MetadataRepositoryBase.CONTROL_ENUM_FIELD = "EnumFields";
        MetadataRepositoryBase.CONTROL_DISCUSION_FIELD = "DiscussionFields";
        return MetadataRepositoryBase;
    }());
    return MetadataRepositoryBase;
});
//# sourceMappingURL=MetadataRepositoryBase.js.map