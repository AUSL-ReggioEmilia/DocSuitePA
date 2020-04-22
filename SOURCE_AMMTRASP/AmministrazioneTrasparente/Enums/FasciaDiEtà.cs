using System.ComponentModel;

enum FasciaDiEtà
{
    [Description("meno di 18 anni")]
    bambino = 1,
    [Description("18 – 30 anni")]
    sottoitrenta = 2,
    [Description("31 – 40 anni")]
    sottoiquaranta = 3,
    [Description("41 – 65 anni")]
    sottoisessantacinque = 4,
    [Description("oltre 65 anni")]
    piùdisessantacinque = 5
}