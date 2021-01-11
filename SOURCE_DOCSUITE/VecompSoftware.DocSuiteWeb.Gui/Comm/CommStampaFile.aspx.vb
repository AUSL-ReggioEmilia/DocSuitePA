
Partial Class CommStampaFile
    Inherits CommBasePage

    Private FileName As String = String.Empty
    Private ViewPrint As String = String.Empty

    Protected WithEvents lblData As System.Web.UI.WebControls.Label
    Protected WithEvents lblAziendaF As System.Web.UI.WebControls.Label
    Protected WithEvents lblTitoloF As System.Web.UI.WebControls.Label
    Protected WithEvents lblAziendaT As System.Web.UI.WebControls.Label
    Protected WithEvents lblTitoloT As System.Web.UI.WebControls.Label
    Protected WithEvents pnlPrintDate As System.Web.UI.WebControls.Panel
    Protected WithEvents pnlNoPrintDate As System.Web.UI.WebControls.Panel
    Protected WithEvents lblOra As System.Web.UI.WebControls.Label

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        FileName = Request.QueryString("FileName")
        Title = Request.QueryString("Titolo")
        ViewPrint = Request.QueryString("ViewPrint")

        Inizializza()
    End Sub

    Private Sub Inizializza()
        If ViewPrint = "False" Then
            pnlPrint.Visible = False
        End If
        tblStampa.Visible = False

        pnlStampa.Attributes("src") = FileName

    End Sub

    Public Function SetType() As String 
        Return Type
    End Function

End Class


