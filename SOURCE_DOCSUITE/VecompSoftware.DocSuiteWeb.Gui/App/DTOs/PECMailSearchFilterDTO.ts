import PECMailDirection = require("App/Models/PECMails/PECMailDirection");

class PECMailSearchFilterDTO {
  dateFrom: string;
  dateTo: string;
  pecMailBox: string;
  mailSenders: string;
  mailRecipients: string;
  invoiceStatus: string;
  invoiceType: string;
  direction: PECMailDirection;
}

export = PECMailSearchFilterDTO;