Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class DocumentSignedEventArgs
    Inherits EventArgs

    ''' <summary> Documento pre firma </summary>
    Public Property SourceDocument As DocumentInfo
    ''' <summary> Documento firmato </summary>
    Public Property DestinationDocument As DocumentInfo

End Class