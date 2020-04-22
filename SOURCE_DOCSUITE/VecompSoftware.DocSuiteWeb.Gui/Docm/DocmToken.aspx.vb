Public Class DocmToken
    Inherits DocmBasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

        If Not Page.IsPostBack Then
            Initialize()
        End If

    End Sub

    Private Sub Initialize()
        uscDocmToken.Year = CurrentDocumentYear
        uscDocmToken.Number = CurrentDocumentNumber
    End Sub

End Class
