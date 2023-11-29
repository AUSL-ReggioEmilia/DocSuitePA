import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import PECMailService = require("App/Services/PECMails/PECMailService");
import PECMailViewModel = require("App/ViewModels/PECMails/PECMailViewModel");
import PECMailReceiptsModel = require("App/ViewModels/PECMails/PECMailReceiptsModel");
import GenericHelper = require("App/Helpers/GenericHelper");

class PECDetails {
    dateId: string;
    nameId: string;
    senderId: string;
    receiverId: string;
    layoutReceiverId: string;
    layoutMessageId: string;

    pecMails: PECMailViewModel;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _pecMailServiceConfiguration: PECMailService;

    private _dateId: JQuery;
    private _nameId: JQuery;
    private _senderId: JQuery;
    private _receiverId: JQuery;
    private _statusId: JQuery;
    private _pecImageId: JQuery;
    private _dateReceiptId: JQuery;
    private _layoutReceiverId: JQuery;
    private _layoutMessageId: JQuery;


    protected static PECMail_TYPE_NAME = "PECMail";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let pecMailServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECDetails.PECMail_TYPE_NAME);
        this._pecMailServiceConfiguration = new PECMailService(pecMailServiceConfiguration);

        this._dateId = <JQuery>$(`#${this.dateId}`);
        this._nameId = <JQuery>$(`#${this.nameId}`);
        this._senderId = <JQuery>$(`#${this.senderId}`);
        this._receiverId = <JQuery>$(`#${this.receiverId}`);
        this._layoutReceiverId = <JQuery>$(`#${this.layoutReceiverId}`);
        this._layoutMessageId = <JQuery>$(`#${this.layoutMessageId}`);

        let entityId: number = Number(GenericHelper.getUrlParams(window.location.href, "PECEntityId"));
        let directionType: string = GenericHelper.getUrlParams(window.location.href, "Direction");

        if (decodeURIComponent(directionType) == "'Outgoing'") {
            this._layoutReceiverId.show();
            this._layoutMessageId.show();
            this.loadOutGoingData(entityId);
        } else {
            this.loadIncomingData(entityId);
            this.loadOutGoingData(entityId);
            this._layoutReceiverId.show();
            this._layoutMessageId.show();
        }
    }
    
    private loadOutGoingData(entityId: number): void {
        this._pecMailServiceConfiguration.getOutgoingPECMailByEntityId(entityId, (data) => {
            if (!data) return;
            this.pecMails = data[0];

            if (this.pecMails.MailDate) {
                let tmp: string = moment(this.pecMails.MailDate).format("DD/MM/YYYY, HH:mm:ss");
                this._dateId.text(tmp);
            }

            let subjectName: string = this.pecMails.MailSubject;
            let url: string = `../PEC/PECSummary.aspx?Type=Pec&PECId=${this.pecMails.EntityId}`;

            if (!subjectName) {
                subjectName = '----------'
            }

            this._nameId.text(subjectName);
            this._nameId.attr("href", url);

            let sender = this.pecMails.MailSenders;
            this._senderId.text(sender);
            
            let receiver: string = this.pecMails.MailRecipients;
            this._receiverId.text(receiver);

            let numberOfItems: number = this.pecMails.PECMailReceipts.length;
            let pecMailsReceipts: PECMailReceiptsModel[] = this.pecMails.PECMailReceipts;

            let table: HTMLElement = document.getElementById('messageDetails');
            let tr: string = "";

            for (let i = 0; i < numberOfItems; i++) {
                let status: string = pecMailsReceipts[i].ReceiptType;
                
                if (status == "accettazione") {
                    tr = `${tr}<tr><td><img class="rtImg" alt = "" src = "../Comm/Images/pec-accettazione.gif"><td>`;
                    tr = `${tr}accettazione`;
                    tr = `${tr}<td>${(moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, HH:mm:ss"))}</td>`;
                    tr = `${tr}</tr>`;
                }

                if (status == "avvenuta-consegna") {
                    tr = `${tr}<tr><td><img class="rtImg" alt = "" src = "../Comm/Images/pec-avvenuta-consegna.gif" ><td>`;
                    tr = `${tr}avvenuta - consegna`;
                    tr = `${tr}<td>${(moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, HH:mm:ss"))}</td>`;
                    tr = `${tr}</tr>`;
                }

                if (status == "non-accettazione") {
                    tr = `${tr}<tr><td><img class="rtImg" alt = "" src = "../Comm/Images/pec-non-accettazione.gif"><td>`;
                    tr = `${tr}non-accettazione`;
                    tr = `${tr}<td>${(moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, HH:mm:ss"))}</td>`;
                    tr = `${tr}</tr>`;
                }

                if (status == "preavviso-errore-consegna") {
                    tr = `${tr}<tr><td><img class="rtImg" alt = "" src = "../Comm/Images/pec-preavviso-errore-consegna.gif"><td>`;
                    tr = `${tr}preavviso-errore-consegna`;
                    tr = `${tr}<td>${(moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, HH:mm:ss"))}</td>`;
                    tr = `${tr}</tr>`;
                }

                if (status == "errore-consegna") {
                    tr = `${tr}<tr><td><img class="rtImg" alt = "" src = "../Comm/Images/pec-errore-consegna.gif"><td>`;
                    tr = `${tr}errore-consegna`;
                    tr = `${tr}<td>${(moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, HH:mm:ss"))}</td>`;
                    tr = `${tr}</tr>`;
                }
            }
            table.innerHTML = tr;
        })
    }

    private loadIncomingData(entityId: number): void {
        this._pecMailServiceConfiguration.getIncomingPECMailByEntityId(entityId, (data) => {
            if (!data) return;

            this.pecMails = data[0];
            if (this.pecMails.MailDate) {
                let tmp: string = moment(this.pecMails.MailDate).format("DD/MM/YYYY, HH:mm:ss");
                this._dateId.text(tmp);
            }

            let subjectName: string = this.pecMails.MailSubject;
            let url: string = `../PEC/PECSummary.aspx?Type=Pec&PECId=${this.pecMails.EntityId}`;

            if (!subjectName) {
                subjectName = '----------'
            }

            this._nameId.text(subjectName);
            this._nameId.attr("href", url);

            let sender = this.pecMails.MailSenders;
            this._senderId.text(sender);

        })
    }
}

export = PECDetails;