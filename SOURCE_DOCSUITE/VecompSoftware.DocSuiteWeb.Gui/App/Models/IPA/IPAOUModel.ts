import IPABase = require("./IPABase");

interface IPAOUModel extends IPABase {
    cod_uni_ou: string;
    cod_aoo: string;
    des_ou: string;
    tel: string;
    fax: string;
    mail_resp: string;
    tel_resp: string;
}

export = IPAOUModel;