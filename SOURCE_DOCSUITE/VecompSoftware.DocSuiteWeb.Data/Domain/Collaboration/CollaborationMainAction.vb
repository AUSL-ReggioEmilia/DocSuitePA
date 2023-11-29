''' <summary> Codici delle azioni di collaborazione. </summary>
Public Class CollaborationMainAction

#Region " IdStatus di CollaborationStatus "

    ''' <summary> Aggiunto Allegato </summary>
    Public Const AggiuntoAllegato As String = "ALLADD"

    ''' <summary> Rimozione Allegato </summary>
    Public Const RimozioneAllegato As String = "ALLDEL"

    ''' <summary> Documento da Protocollare </summary>
    Public Const DaProtocollareGestire As String = "CD"

    ''' <summary> Inoltro Documento per Visione/Firma </summary>
    Public Const DaVisionareFirmare As String = "CF"

    ''' <summary> Check In Documento </summary>
    Public Const CheckInDocumento As String = "CK"

    ''' <summary> Documento Protocollato </summary>
    Public Const ProtocollatiGestiti As String = "CP"

    ''' <summary> Richiamo Documento per Visione/Firma </summary>
    ''' <remarks> Nel codice sembra esserci confusione tra <see cref="Richiamo"/> e <see cref="CambioResponsabile"/>. </remarks>
    Public Const Richiamo As String = "CR"

    ''' <summary> Cambio Responsabile </summary>
    ''' <remarks> Nel codice sembra esserci confusione tra <see cref="Richiamo"/> e <see cref="CambioResponsabile"/>. </remarks>
    Public Const CambioResponsabile As String = "CResp"

    ''' <summary> Restituzione Documento al Proponente </summary>
    Public Const DaLeggere As String = "DL"

    ''' <summary> Inserimento Documento per Visione/Firma </summary>
    Public Const InserimentoPerVisioneFirma As String = "IN"

    ''' <summary> Modifica Documento per Visione/Firma </summary>
    Public Const Modifica As String = "MD"

    ''' <summary> Avanzamento Collaborazione </summary>
    ''' <remarks> Quando un firmatario firma il documento senza mandare avanti la collaborazione. </remarks>
    Public Const NotificaStep As String = "NotifyStep"

    ''' <summary> Cancellazione Documento/Annullamento documento </summary>
    Public Const CancellazioneDocumento As String = "RM"

    ''' <summary>
    ''' La collaborazione è stata annullata. Il workflow della collaborazione è tornato verso la funzionalità di tavoli.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const AnnullataRitornoAlTavolo As String = "RT"

#End Region

    ''' <summary> Alla Visione/Firma. </summary>
    Public Const AllaVisioneFirma As String = "CI"

    ''' <summary> Al Protocollo/Segreteria. </summary>
    Public Const AlProtocolloSegreteria As String = "CA"

    ''' <summary> Attività in Corso. </summary>
    Public Const AttivitaInCorso As String = "CS"

    ''' <summary> Miei Check Out. </summary>
    Public Const MieiCheckOut As String = "CCheckedOut"

    ''' <summary> Check In multiplo. </summary>
    Public Const CheckInMultiplo As String = "CCheckIn"

    ''' <summary> Inoltro Documento per Visione/Firma </summary>
    Public Const DaFirmareInDelega As String = "DFD"
End Class
