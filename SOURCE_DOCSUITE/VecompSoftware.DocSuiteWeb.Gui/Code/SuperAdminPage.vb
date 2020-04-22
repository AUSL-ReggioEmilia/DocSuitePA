Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Classe base per le pagine consultabili da utenti SuperAdmin. </summary>
Public Class SuperAdminPage
    Inherits CommonBasePage

    Protected Overrides Sub OnInit(e As System.EventArgs)
        MyBase.OnInit(e)

        ' controllo se autenticato
        If Not SuperAdminAuthored Then
            Throw New DocSuiteException("Errore in apertura pagina per Super Admin", "Non ti sei autenticato come Super Admin.", Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

    End Sub
End Class
