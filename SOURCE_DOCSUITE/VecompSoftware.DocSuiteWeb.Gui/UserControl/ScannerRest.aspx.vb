Public Class ScannerRest
    Inherits System.Web.UI.Page
    Public ReadOnly Property MultipleEnabled As String
        Get
            Return Request.QueryString.Item("multipleEnabled")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class