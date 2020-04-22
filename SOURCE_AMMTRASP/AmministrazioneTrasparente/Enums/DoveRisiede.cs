using System.ComponentModel;

enum DoveRisiede
{
    [Description("nella provincia di Reggio Emilia")]
    nellaprovincia = 1,
    [Description("in altra provincia dell’Emilia Romagna")]
    altraprovincia = 2,
    [Description("in altra regione italiana")]
    regione = 3,
    [Description("in altro Stato")]
    altro = 4
}