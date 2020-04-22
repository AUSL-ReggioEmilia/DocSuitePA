Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Web

Public Class SuperAdmin
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        setVisibility()
    End Sub

    ''' <summary>Verifica la parola chiave e autorizza il cliente</summary>
    ''' <remarks>Dal 05/06/2012 si ricarica da solo!</remarks>
    Protected Sub activateSuperUser(ByVal sender As Object, ByVal e As EventArgs)
        Dim errorMessage As String = ""

        ' Controllo l'autorizzazione
        If CommonUtil.IsSuperAdmin(errorMessage) Then
            SuperAdminAuthored = True
            AjaxManager.ResponseScripts.Add(String.Format("return doRedirect('{0}');", HttpContext.Current.Request.ApplicationPath))
        Else
            SuperAdminAuthored = False
            AjaxAlert(String.Format("Autenticazione non riuscita{0}{1}", Environment.NewLine, errorMessage))
        End If

        setVisibility()
    End Sub

    Protected Sub deactivateSuperUser(ByVal sender As Object, ByVal e As EventArgs)
        ' tolgo l'autorizzazione
        SuperAdminAuthored = False
        setVisibility()
        AjaxManager.ResponseScripts.Add(String.Format("return doRedirect('{0}');", HttpContext.Current.Request.ApplicationPath))
    End Sub

    ''' <summary>
    ''' Imposto la visibilità a seconda dell'autorizzazione
    ''' </summary>
    Private Sub setVisibility()
        If Me.SuperAdminAuthored Then
            signOut.Visible = True
            signIn.Visible = False
        Else
            signIn.Visible = True
            signOut.Visible = False
        End If
    End Sub

End Class