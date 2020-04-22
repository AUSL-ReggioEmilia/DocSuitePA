using System.ComponentModel;

enum ConoscenzaDiQuestaDelSito
{
    [Description("ho trovato l’indirizzo sui mezzi di comunicazione (es. stampa)")]
    mezzi = 1,
    [Description("attraverso motori di ricerca")]
    motori = 2,
    [Description("attraverso un link da un altro sito")]
    link = 3,
    [Description("attraverso indicazione diretta da parte di personale dell’AUSL di Reggio Emilia")]
    personale = 4,
    [Description("attraverso suggerimento di un conoscente")]
    suggerimento = 5,
    [Description("altro")]
    altro = 6
}