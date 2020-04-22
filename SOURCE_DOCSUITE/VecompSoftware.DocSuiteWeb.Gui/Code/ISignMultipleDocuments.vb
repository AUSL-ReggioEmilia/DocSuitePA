Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Services.Biblos

''' <summary> Interfaccia per le pagine che chiamano la firma multipla. </summary>
Public Interface ISignMultipleDocuments

    ''' <summary> Documenti da firmare </summary>
    ReadOnly Property DocumentsToSign() As IList(Of MultiSignDocumentInfo)

    ''' <summary> Url al quale andare </summary>
    ''' <remarks> Sostituire questo return url al Request.UrlReferrer, se non c'è bisogno di reindirizzamenti o pagine iniziali fuori dalla docsuite. </remarks>
    ReadOnly Property ReturnUrl As String

End Interface
