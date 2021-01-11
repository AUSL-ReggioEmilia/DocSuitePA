Imports Telerik.Web.UI

Public Class DocumentInfoUploadConfiguration
    Inherits AsyncUploadConfiguration

    ''' <summary>
    ''' Nome dell'operatore che esegue l'operazione
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Owner As String

End Class

Public Class BiblosDocumentInfoUploadConfiguration
    Inherits DocumentInfoUploadConfiguration

    Public Property Server As String
    Public Property Archive As String

End Class

Public Class TempFileDocumentInfoUploadConfiguration
    Inherits DocumentInfoUploadConfiguration

    Public Property TempFolder As String

End Class