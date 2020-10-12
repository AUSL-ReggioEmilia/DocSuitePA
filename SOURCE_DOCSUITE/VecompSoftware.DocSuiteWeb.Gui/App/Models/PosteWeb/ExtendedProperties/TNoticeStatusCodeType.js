define(["require", "exports"], function (require, exports) {
    var TNoticeStatusCodeType;
    (function (TNoticeStatusCodeType) {
        TNoticeStatusCodeType[TNoticeStatusCodeType["Bozza"] = 0] = "Bozza";
        TNoticeStatusCodeType[TNoticeStatusCodeType["AccettataInSpedizione"] = 100] = "AccettataInSpedizione";
        TNoticeStatusCodeType[TNoticeStatusCodeType["InesitataErroreConsegnaDestinatario"] = 200] = "InesitataErroreConsegnaDestinatario";
        TNoticeStatusCodeType[TNoticeStatusCodeType["Spedita"] = 300] = "Spedita";
        TNoticeStatusCodeType[TNoticeStatusCodeType["ConsegnatoPrimoAvviso"] = 400] = "ConsegnatoPrimoAvviso";
        TNoticeStatusCodeType[TNoticeStatusCodeType["ConsegnatoSecondoAvviso"] = 500] = "ConsegnatoSecondoAvviso";
        TNoticeStatusCodeType[TNoticeStatusCodeType["ConsegnatoTerzoAvviso"] = 600] = "ConsegnatoTerzoAvviso";
        TNoticeStatusCodeType[TNoticeStatusCodeType["ErroreTecnicoDelSistema"] = 700] = "ErroreTecnicoDelSistema";
        TNoticeStatusCodeType[TNoticeStatusCodeType["CompiutaGiacenza"] = 800] = "CompiutaGiacenza";
        TNoticeStatusCodeType[TNoticeStatusCodeType["RifiutataDalDestinatario"] = 900] = "RifiutataDalDestinatario";
        TNoticeStatusCodeType[TNoticeStatusCodeType["AccettataDalDestinatario"] = 1000] = "AccettataDalDestinatario";
        TNoticeStatusCodeType[TNoticeStatusCodeType["Ritirata_Firmato"] = 1100] = "Ritirata_Firmato";
        TNoticeStatusCodeType[TNoticeStatusCodeType["Ritirata_Senza_Firma"] = 1500] = "Ritirata_Senza_Firma";
    })(TNoticeStatusCodeType || (TNoticeStatusCodeType = {}));
    return TNoticeStatusCodeType;
});
//# sourceMappingURL=TNoticeStatusCodeType.js.map