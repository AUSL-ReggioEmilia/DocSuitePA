define(["require", "exports", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/PECMails/PECMailService", "App/Helpers/GenericHelper"], function (require, exports, EnumHelper, ServiceConfigurationHelper, PECMailService, GenericHelper) {
    var PECDetails = /** @class */ (function () {
        function PECDetails(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        PECDetails.prototype.initialize = function () {
            var pecMailServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECDetails.PECMail_TYPE_NAME);
            this._pecMailServiceConfiguration = new PECMailService(pecMailServiceConfiguration);
            this._dateId = $("#" + this.dateId);
            this._nameId = $("#" + this.nameId);
            this._senderId = $("#" + this.senderId);
            this._receiverId = $("#" + this.receiverId);
            this._layoutReceiverId = $("#" + this.layoutReceiverId);
            this._layoutMessageId = $("#" + this.layoutMessageId);
            var entityId = Number(GenericHelper.getUrlParams(window.location.href, "PECEntityId"));
            var directionType = GenericHelper.getUrlParams(window.location.href, "Direction");
            if (decodeURIComponent(directionType) == "'Outgoing'") {
                this._layoutReceiverId.show();
                this._layoutMessageId.show();
                this.loadOutGoingData(entityId);
            }
            else {
                this.loadIncomingData(entityId);
                this.loadOutGoingData(entityId);
                this._layoutReceiverId.show();
                this._layoutMessageId.show();
            }
        };
        PECDetails.prototype.loadOutGoingData = function (entityId) {
            var _this = this;
            this._pecMailServiceConfiguration.getOutgoingPECMailByEntityId(entityId, function (data) {
                if (!data)
                    return;
                _this.pecMails = data[0];
                if (_this.pecMails.MailDate) {
                    var tmp = moment(_this.pecMails.MailDate).format("DD/MM/YYYY, h:mm:ss");
                    _this._dateId.text(tmp);
                }
                var subjectName = _this.pecMails.MailSubject;
                var url = "../PEC/PECSummary.aspx?Type=Pec&PECId=" + _this.pecMails.EntityId;
                _this._nameId.text(subjectName);
                _this._nameId.attr("href", url);
                var sender = _this.pecMails.MailSenders;
                _this._senderId.text(sender);
                var regex = /\S+[a-z0-9]@[a-z0-9\.]+/img;
                var receiver = _this.pecMails.MailRecipients.replace(/[<>]/g, '').match(regex).join(';');
                _this._receiverId.text(receiver);
                var numberOfItems = _this.pecMails.PECMailReceipts.length;
                var pecMailsReceipts = _this.pecMails.PECMailReceipts;
                var table = document.getElementById('messageDetails');
                var tr = "";
                for (var i = 0; i < numberOfItems; i++) {
                    var status_1 = pecMailsReceipts[i].ReceiptType;
                    if (status_1 == "accettazione") {
                        tr = tr + "<tr><td><img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-accettazione.gif\"><td>";
                        tr = tr + "accettazione";
                        tr = tr + "<td>" + (moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, h:mm:ss")) + "</td>";
                        tr = tr + "</tr>";
                    }
                    if (status_1 == "avvenuta-consegna") {
                        tr = tr + "<tr><td><img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-avvenuta-consegna.gif\" ><td>";
                        tr = tr + "avvenuta - consegna";
                        tr = tr + "<td>" + (moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, h:mm:ss")) + "</td>";
                        tr = tr + "</tr>";
                    }
                    if (status_1 == "non-accettazione") {
                        tr = tr + "<tr><td><img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-non-accettazione.gif\"><td>";
                        tr = tr + "non-accettazione";
                        tr = tr + "<td>" + (moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, h:mm:ss")) + "</td>";
                        tr = tr + "</tr>";
                    }
                    if (status_1 == "preavviso-errore-consegna") {
                        tr = tr + "<tr><td><img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-preavviso-errore-consegna.gif\"><td>";
                        tr = tr + "preavviso-errore-consegna";
                        tr = tr + "<td>" + (moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, h:mm:ss")) + "</td>";
                        tr = tr + "</tr>";
                    }
                    if (status_1 == "errore-consegna") {
                        tr = tr + "<tr><td><img class=\"rtImg\" alt = \"\" src = \"../Comm/Images/pec-errore-consegna.gif\"><td>";
                        tr = tr + "errore-consegna";
                        tr = tr + "<td>" + (moment(pecMailsReceipts[i].ReceiptDate).format("DD/MM/YYYY, h:mm:ss")) + "</td>";
                        tr = tr + "</tr>";
                    }
                }
                table.innerHTML = tr;
            });
        };
        PECDetails.prototype.loadIncomingData = function (entityId) {
            var _this = this;
            this._pecMailServiceConfiguration.getIncomingPECMailByEntityId(entityId, function (data) {
                if (!data)
                    return;
                _this.pecMails = data[0];
                if (_this.pecMails.MailDate) {
                    var tmp = moment(_this.pecMails.MailDate).format("DD/MM/YYYY, h:mm:ss");
                    _this._dateId.text(tmp);
                }
                var subjectName = _this.pecMails.MailSubject;
                var url = "../PEC/PECSummary.aspx?Type=Pec&PECId=" + _this.pecMails.EntityId;
                _this._nameId.text(subjectName);
                _this._nameId.attr("href", url);
                var sender = _this.pecMails.MailSenders;
                _this._senderId.text(sender);
            });
        };
        PECDetails.PECMail_TYPE_NAME = "PECMail";
        return PECDetails;
    }());
    return PECDetails;
});
//# sourceMappingURL=PECDetails.js.map