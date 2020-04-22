using System.ComponentModel;

enum FrequenzaConsulta
{
    [Description("tutti i giorni")]
    giorni = 1,
    [Description("una o più volte a settimana")]
    settimana = 2,
    [Description("una o più volte al mese")]
    mese = 3,
    [Description("sporadicamente")]
    sporadicamente = 4,
    [Description("è la prima volta")]
    prima = 5
}