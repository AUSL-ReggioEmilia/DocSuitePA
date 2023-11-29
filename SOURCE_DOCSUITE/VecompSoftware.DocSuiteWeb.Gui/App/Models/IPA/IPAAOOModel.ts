import IPABase = require("./IPABase");

interface IPAAOOModel extends IPABase {
    cod_aoo: string;
    des_aoo: string;
    tel: string;
    fax: string;
    mail_resp: string;
    tele_resp: string;
}

export = IPAAOOModel;