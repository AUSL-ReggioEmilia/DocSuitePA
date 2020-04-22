Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocmInfo
    Inherits DocmBasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load, Me.Load
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub
    
    Private Sub Initialize()
        If CurrentDocument Is Nothing Then
            Throw New DocSuiteException("Informazioni Pratica", "Pratica inesistente")
        End If
        uscDocument.CurrentDocument = CurrentDocument
        uscDocument.Show()
    End Sub

End Class