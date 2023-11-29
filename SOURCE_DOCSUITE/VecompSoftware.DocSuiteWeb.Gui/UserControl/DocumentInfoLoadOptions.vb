Public Class DocumentInfoLoadOptions

#Region " Properties "
    Public Property Selected As Boolean
    Public Property Deletable As Boolean
    Public Property ShowSize As Boolean
#End Region

#Region " Constructor "
    Public Sub New()
        Selected = True
        Deletable = False
        ShowSize = False
    End Sub
#End Region

End Class
