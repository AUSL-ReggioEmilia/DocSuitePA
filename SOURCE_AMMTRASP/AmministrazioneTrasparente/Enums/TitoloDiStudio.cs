using System.ComponentModel;

enum TitoloDiStudio
{
    [Description("licenza elementare o nessun titolo")]
    elementare = 1,
    [Description("licenza media")]
    media = 2,
    [Description("diploma scuola media superiore")]
    superiore = 3,
    [Description("diploma universitario")]
    universitario = 4,
    [Description("laurea o laurea con specializzazione")]
    specializzazione = 5
}