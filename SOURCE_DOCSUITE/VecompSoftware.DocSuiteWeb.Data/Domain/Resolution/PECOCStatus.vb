''' <summary> Stati della PEC all'Organo di Controllo. </summary>
Public Enum PECOCStatus

    ''' <summary> Aggiunto. </summary>
    Aggiunto
    ''' <summary> Preso in carico dal jeep module. </summary>
    Elaborazione
    ''' <summary> Completata l'elaborazione del jeep module, in attesa dell'invio. </summary>
    Completo
    ''' <summary> Elaborazione del jeep module andata in errore. </summary>
    Errore
    ''' <summary> PEC spedita. </summary>
    Spedito
    ''' <summary> Cancellato dall'utente tramite GUI. </summary>
    Cancellato
    ''' <summary> Creato ma vuoto. </summary>
    Vuoto

End Enum

