import IPABase = require("./IPABase");

interface IPAAdministrationModel extends IPABase {    
    acronimo: string;
    des_amm: string;
    titolo_resp: string;
    sito_istituzionale: string;
    liv_access: string;
    mail4: string;
    mail5: string;
    tipologia: string;
    categoria: string;
    data_accreditamento: string;
    cf: string;
}

export = IPAAdministrationModel;