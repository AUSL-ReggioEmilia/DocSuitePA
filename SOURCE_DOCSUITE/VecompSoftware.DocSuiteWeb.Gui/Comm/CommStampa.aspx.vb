Imports System.IO

Partial Class CommStampa
    Inherits CommBasePage

    Private FileName As String = String.Empty
    Private Titolo As String = String.Empty
    Private ViewPrint As String = String.Empty

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim CorporateName As String = ProtocolEnv.CorporateName
        FileName = Request.QueryString("FileName")
        Titolo = Request.QueryString("Title")
        ViewPrint = Request.QueryString("ViewPrint")
        Dim PrintDate As Boolean = If(UCase(Request.QueryString("PrintDate")) = "FALSE", False, True)
        If PrintDate Then
            lblAziendaT.Text = CorporateName
            lblTitoloT.Text = Replace(Titolo, "|", "<BR>")
            Dim serverDate As Date = DateTime.Now
            lblOra.Text = "Ora<BR>" & String.Format("{0:HH:mm}", serverDate)
            lblData.Text = "Data<BR>" & String.Format("{0:dd/MM/yyyy}", serverDate)
            pnlNoPrintDate.Visible = False
        Else
            lblAziendaF.Text = CorporateName
            lblTitoloF.Text = Replace(Titolo, "|", "<BR>")
            pnlPrintDate.Visible = False
        End If
        Inizializza()
    End Sub

    Private Sub Inizializza()
        Dim fi As New FileInfo(FileName)
        If fi.Exists Then
            If ViewPrint = "False" Then
                pnlPrint.Visible = False
            End If
            tblStampa.Visible = False
            Dim sr As StreamReader = New StreamReader(FileName)
            Do While sr.Peek() >= 0
                Dim s As String = sr.ReadLine
                Dim v As Array = Split(s, "§")
                If Not WebUtils.ObjTableAdd("Prnt", tbl, v(0), v(1), v(2), v(3), Nothing) Then
                    AjaxAlert("Errore dati per la stampa.\n\n" & v(0) & "\n" & v(1))
                    Exit Sub
                End If
            Loop
            sr.Close()
            File.Delete(FileName)
        Else
            AjaxAlert("Pagina non trovata.")
            tblStampa.Visible = True
        End If
    End Sub

    Public Function SetType() As String 
        Return Type
    End Function

End Class


