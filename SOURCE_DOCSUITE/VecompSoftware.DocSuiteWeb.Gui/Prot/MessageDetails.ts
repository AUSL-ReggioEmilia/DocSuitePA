import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import MessageService = require("App/Services/Messages/MessageService");
import MessageModel = require("App/Models/Messages/MessageModel");
import MessageContactPosition = require("App/Models/Messages/MessageContactPosition");
import GenericHelper = require("App/Helpers/GenericHelper");

class MessageDetails {
    dateId: string;
    nameId: string;
    senderId: string;
    receiverId: string;
    receiverBccId: string;
    layoutRowRecipientId: string;
    layoutRowRecipientBccId: string;

    statusId: string;
    pecImageId: string;
    messages: MessageModel;
    isMessageReadable: boolean;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _messageService: MessageService;

    private _dateId: JQuery;
    private _nameId: JQuery;
    private _senderId: JQuery;
    private _receiverId: JQuery;
    private _receiverBccId: JQuery;
    private _statusId: JQuery;
    private _pecImageId: JQuery;

    private _layoutRowRecipientId: JQuery;
    private _layoutRowRecipientBccId: JQuery;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let messageServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Message");
        this._messageService = new MessageService(messageServiceConfiguration);

        this._dateId = <JQuery>$(`#${this.dateId}`);
        this._nameId = <JQuery>$(`#${this.nameId}`);
        this._senderId = <JQuery>$(`#${this.senderId}`);
        this._receiverId = <JQuery>$(`#${this.receiverId}`);
        this._receiverBccId = <JQuery>$(`#${this.receiverBccId}`);
        this._statusId = <JQuery>$(`#${this.statusId}`);
        this._pecImageId = <JQuery>$(`#${this.pecImageId}`);
        this._layoutRowRecipientId = <JQuery>$(`#${this.layoutRowRecipientId}`);
        this._layoutRowRecipientBccId = <JQuery>$(`#${this.layoutRowRecipientBccId}`);

        let entityId: number = Number(GenericHelper.getUrlParams(window.location.href, "MessageId"));

        this.loadData(entityId);
    }

    private loadData(entityId: number): void {
        this._messageService.getProtocolMessagesByShortId(entityId, (data) => {
            if (!data) return;
            this.messages = data;

            if (this.messages.MessageEmails[0].SentDate) {
                let tmp: string = moment(this.messages.MessageEmails[0].SentDate).format("DD/MM/YYYY");
                this._dateId.text(tmp);
            }

            let subjectName: string = this.messages.MessageEmails[0].Subject;
            let messageEmailId: number = this.messages.MessageEmails[0].EntityId
            let url: string = `../Prot/MessageEmailView.aspx?Type=Prot&MessageEmailId=${messageEmailId}`;
            this._nameId.text(subjectName);
            this._nameId.attr("disabled", "disabled");
            if (this.isMessageReadable) {
                this._nameId.removeAttr("disabled");
                this._nameId.attr("href", url);
            }

            let sender = this.messages.MessageContacts.filter(f => f.ContactPosition == MessageContactPosition.Sender).map(f => f.Description).join("; ");
            this._senderId.text(sender);

            let receiver = this.messages.MessageContacts.filter(f => f.ContactPosition == MessageContactPosition.Recipient).map(f => f.Description).join("; ");
            this._receiverId.text(receiver);
            if (receiver == "") { this._layoutRowRecipientId.hide() };

            let receiverbcc = this.messages.MessageContacts.filter(f => f.ContactPosition == MessageContactPosition.RecipientBcc).map(f => f.Description).join("; ");
            this._receiverBccId.text(receiverbcc);
            if (receiverbcc == "") { this._layoutRowRecipientBccId.hide() };

            let status = this.messages.Status;

            if (status == "Active") {
                this._statusId.text("Invio in corso");
                this._pecImageId.attr("src", "../Comm/Images/pec-accettazione.gif");
            }

            if (status == "Sent") {
                this._statusId.text("Inviato");
                this._pecImageId.attr("src", "../Comm/Images/pec-avvenuta-consegna.gif");
            }

            if (status == "Error") {
                this._statusId.text("Errore");
                this._pecImageId.attr("src", "../Comm/Images/pec-errore-consegna.gif");
            }

            if (status == "Draft") {
                this._statusId.text("Bozza");
                this._pecImageId.attr("src", "../Comm/Images/pec-non-accettazione.gif");
            }
        })
    }
}
export = MessageDetails;