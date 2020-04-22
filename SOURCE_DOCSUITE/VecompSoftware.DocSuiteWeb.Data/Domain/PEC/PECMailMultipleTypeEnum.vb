''' <summary> Enumeratore che definisce il tipo di split atteso per la PEC </summary>
Public Enum PecMailMultipleTypeEnum
    ''' <summary> Non � necessaria clonazione delle PEC </summary>
    NoSplit = -1

    ''' <summary> Clona per destinatario </summary>
    SplitByRecipients = 0

    ''' <summary> Clona fino al raggiungimento della massima grandezza disponibile </summary>
    SplitBySize = 1

    ''' <summary>
    ''' Clona fino al raggiungimento della massima grandezza disponibile
    ''' e poi per ogni destinatario
    ''' Il risultato � #PEC_da_dimensione_allegati x #contatti
    ''' </summary>
    SplitBySizeAndRecipients = 2
End Enum