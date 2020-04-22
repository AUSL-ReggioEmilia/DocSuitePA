using System.ComponentModel;

enum AttualeOccupazione
{
    [Description("dipendente pubblico")]
    pubblico = 1,
    [Description("dipendente privato")]
    privato = 2,
    [Description("imprenditore/libero professionista")]
    professionista = 3,
    [Description("commerciante/artigiano")]
    artigiano = 4,
    [Description("casalinga")]
    casalinga = 5,
    [Description("pensionato")]
    pensionato = 6,
    [Description("studente")]
    studente = 7,
    [Description("in attesa di occupazione")]
    occupazione = 8,
    [Description("altro")]
    altro = 9
}