Imports VecompSoftware.DocSuiteWeb.Data

Public Class ContactAddedEventArgs
    Inherits EventArgs

    Private _contact As ContactDTO

    Public Sub New(ByVal contactDto As ContactDTO)
        Me._contact = contactDto
    End Sub

End Class