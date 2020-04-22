Public Class Test
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            visualizza(Request.QueryString("id"))
        End If
    End Sub

    Protected Sub SendClick(sender As Object, e As EventArgs) Handles send.Click
        visualizza(prova.Text)
    End Sub


    Protected Sub Alert(message As String)
        ClientScript.RegisterStartupScript(Me.GetType(), "customAlert", String.Format("alert('{0}');", message), True)
    End Sub

    Protected Sub Visualizza(arg As String)
        Dim db As New DBAdmin
        Dim atto As Byte()
        Dim numeroPubblicazione As Integer

        If Integer.TryParse(arg, numeroPubblicazione) Then
            atto = db.GetAtto(numeroPubblicazione)

            If atto IsNot Nothing Then
                Response.Write("Ok atto estratto dal db")

                'Prova 
                Response.Clear()
                Response.ContentType = "application/pdf"
                Response.AppendHeader("Content-Disposition", "inline;filename=data.pdf")
                Response.BufferOutput = True
                Response.BinaryWrite(atto)
                Response.End()

            Else
                Alert("errore riprovare")
            End If
        Else
            Alert("inserire un id valido")
        End If
    End Sub

    Protected Sub InserisciClick(sender As Object, e As EventArgs) Handles Inserisci.Click
        Dim service As New Service()

        Dim nPublicazione As Long = service.Inserisci(TipoDocumento.Text, Titolo.Text, DateTime.Now, Oggetto.Text, 0)
        NPubblicazione.Text = nPublicazione.ToString()
        nPublicazione2.Text = nPublicazione.ToString()

        Alert("Inserimento avvenuto con successo NPublicazione " & nPublicazione.ToString())

    End Sub


    Protected Sub PubblicaClick(sender As Object, e As EventArgs) Handles Pubblica.Click
        Dim service As New Service()

        Dim nPublicazione As Long = service.PubblicaPath(AttoPath.Text, Long.Parse(NPubblicazione.Text.ToString()), 0, DateTime.Now, OggettoPubblicazione.Text)
        Alert(String.Format("Pubblicazione avvenuta con successo: id {0}", nPublicazione))
    End Sub

    Protected Sub RevocaClick(sender As Object, e As EventArgs) Handles Revoca.Click
        Dim service As New Service()

        service.Revoca(Long.Parse(nPublicazione2.Text.ToString()))
        Alert("Revoca avvenuta con successo")
    End Sub
End Class