Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class DocumentEventArgs
    Inherits EventArgs

    Public Property Document As DocumentInfo
    Public Property Version As Decimal
End Class