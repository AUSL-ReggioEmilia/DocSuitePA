''' <summary> Tipi di stato per le resolution. </summary>
''' <remarks> Usare solo se estremamente necessario, dovrebbero essere id dinamici "in teoria" </remarks>
Public Enum ResolutionStatusId As Short
    Sospeso = -12
    Ritirata = -6
    Temporaneo = -5
    Rettificato = -4
    Revocato = -3
    Annullato = -2
    Errato = -1
    Attivo = 0
End Enum
