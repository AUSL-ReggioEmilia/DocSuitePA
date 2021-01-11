Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Interface ISendMail

    ''' <summary> Descrizione del from </summary>
    ''' <remarks>
    ''' TODO: prevedere eliminazione o astrazione, è sempre quello
    ''' </remarks>
    ReadOnly Property SenderDescription As String

    ''' <summary> Email del from </summary>
    ''' <remarks>
    ''' TODO: prevedere eliminazione o astrazione, è sempre quello
    ''' </remarks>
    ReadOnly Property SenderEmail As String

    ''' <summary> Destinatari </summary>
    ReadOnly Property Recipients As IList(Of ContactDTO)

    ''' <summary> Documenti da includere </summary>
    ReadOnly Property Documents As IList(Of DocumentInfo)

    ''' <summary> Oggetto </summary>
    ReadOnly Property Subject As String

    ''' <summary> Testo della mail </summary>
    ReadOnly Property Body As String

End Interface
