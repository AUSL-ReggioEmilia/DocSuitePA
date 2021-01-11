using System.ComponentModel;

enum MotivoHaConsultato
{
    [Description("cercavo informazioni")]
    informazioni = 1,
    [Description("cercavo un documento")]
    documento = 2,
    [Description("per stampare modulistica")]
    modulistica = 3,
    [Description("per caso")]
    caso = 4,
    [Description("altro")]
    altro = 5
}