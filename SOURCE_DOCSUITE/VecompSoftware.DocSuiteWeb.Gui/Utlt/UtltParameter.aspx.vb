''' <summary>
''' Visualizza tutti i parametri possibili per ogni ambiente,
''' segnala i valori presenti nel DB che secondo l'XML non dovrebbero essere presenti
''' scrive in corsivo i valori che non sono stati settati nell'applicazione
''' scrive in neretto i valori impostati nel DB solo se sono differenti dal default
''' </summary>
Partial Class UtltParameter
    Inherits SuperAdminPage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
        End If
    End Sub

#End Region

End Class
