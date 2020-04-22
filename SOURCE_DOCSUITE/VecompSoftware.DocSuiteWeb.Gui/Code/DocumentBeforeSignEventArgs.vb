Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class DocumentBeforeSignEventArgs
    Inherits EventArgs

    Public Property Cancel As Boolean
    Public Property SourceDocument As DocumentInfo

End Class