''' <summary> Stati della collaborazione durante il processo di WorkFlow </summary>
Public Enum CollaborationStatusType
    ''' <summary> Bozza </summary>
    BZ = 0
    ''' <summary> Inserimento per visione </summary>
    [IN]
    ''' <summary> Da Protocollare </summary>
    DP
    ''' <summary> Finalizzato al protocollo </summary>
    PT
    ''' <summary> Da leggere </summary>
    DL

    ''' <summary>
    ''' Non Previsto
    ''' </summary>
    ''' <remarks>Casistica di errore: indica che la collaborazione si trova in uno stato sconosciuto alla procedura.</remarks>
    NP

    ''' <summary>
    ''' Non Trovata
    ''' </summary>
    ''' <remarks>Casistica di errore: indica che la collaborazione con l'id specificato non è stata trovata.</remarks>
    NT
    ''' <summary>
    ''' Collaborazione speciale
    ''' </summary>
    ''' <remarks>Collaborazione speciale</remarks>
    WM
End Enum