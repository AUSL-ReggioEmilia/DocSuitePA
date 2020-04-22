define(["require", "exports", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Messages/MessageService", "App/Models/Messages/MessageContactPosition", "App/Helpers/GenericHelper"], function (require, exports, EnumHelper, ServiceConfigurationHelper, MessageService, MessageContactPosition, GenericHelper) {
    var MessageDetails = /** @class */ (function () {
        function MessageDetails(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        MessageDetails.prototype.initialize = function () {
            var messageServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Message");
            this._messageService = new MessageService(messageServiceConfiguration);
            this._dateId = $("#" + this.dateId);
            this._nameId = $("#" + this.nameId);
            this._senderId = $("#" + this.senderId);
            this._receiverId = $("#" + this.receiverId);
            this._receiverBccId = $("#" + this.receiverBccId);
            this._statusId = $("#" + this.statusId);
            this._pecImageId = $("#" + this.pecImageId);
            this._layoutRowRecipientId = $("#" + this.layoutRowRecipientId);
            this._layoutRowRecipientBccId = $("#" + this.layoutRowRecipientBccId);
            var entityId = Number(GenericHelper.getUrlParams(window.location.href, "MessageId"));
            this.loadData(entityId);
        };
        MessageDetails.prototype.loadData = function (entityId) {
            var _this = this;
            this._messageService.getProtocolMessagesByShortId(entityId, function (data) {
                if (!data)
                    return;
                _this.messages = data;
                if (_this.messages.MessageEmails[0].SentDate) {
                    var tmp = moment(_this.messages.MessageEmails[0].SentDate).format("DD/MM/YYYY");
                    _this._dateId.text(tmp);
                }
                var subjectName = _this.messages.MessageEmails[0].Subject;
                var messageEmailId = _this.messages.MessageEmails[0].EntityId;
                var url = "../Prot/MessageEmailView.aspx?Type=Prot&MessageEmailId=" + messageEmailId;
                _this._nameId.text(subjectName);
                _this._nameId.attr("disabled", "disabled");
                if (_this.isMessageReadable) {
                    _this._nameId.removeAttr("disabled");
                    _this._nameId.attr("href", url);
                }
                var sender = _this.messages.MessageContacts.filter(function (f) { return f.ContactPosition == MessageContactPosition.Sender; }).map(function (f) { return f.Description; }).join("; ");
                _this._senderId.text(sender);
                var receiver = _this.messages.MessageContacts.filter(function (f) { return f.ContactPosition == MessageContactPosition.Recipient; }).map(function (f) { return f.Description; }).join("; ");
                _this._receiverId.text(receiver);
                if (receiver == "") {
                    _this._layoutRowRecipientId.hide();
                }
                ;
                var receiverbcc = _this.messages.MessageContacts.filter(function (f) { return f.ContactPosition == MessageContactPosition.RecipientBcc; }).map(function (f) { return f.Description; }).join("; ");
                _this._receiverBccId.text(receiverbcc);
                if (receiverbcc == "") {
                    _this._layoutRowRecipientBccId.hide();
                }
                ;
                var status = _this.messages.Status;
                if (status == "Active") {
                    _this._statusId.text("Invio in corso");
                    _this._pecImageId.attr("src", "../Comm/Images/pec-accettazione.gif");
                }
                if (status == "Sent") {
                    _this._statusId.text("Inviato");
                    _this._pecImageId.attr("src", "../Comm/Images/pec-avvenuta-consegna.gif");
                }
                if (status == "Error") {
                    _this._statusId.text("Errore");
                    _this._pecImageId.attr("src", "../Comm/Images/pec-errore-consegna.gif");
                }
                if (status == "Draft") {
                    _this._statusId.text("Bozza");
                    _this._pecImageId.attr("src", "../Comm/Images/pec-non-accettazione.gif");
                }
            });
        };
        return MessageDetails;
    }());
    return MessageDetails;
});
//# sourceMappingURL=MessageDetails.js.map