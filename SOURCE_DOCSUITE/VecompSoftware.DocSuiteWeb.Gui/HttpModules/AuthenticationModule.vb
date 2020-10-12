Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Shibboleth
Imports VecompSoftware.Services.Logging

Public Class AuthenticationModule
    Implements IHttpModule

    Public Sub New()
    End Sub

    Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
        AddHandler context.PostAuthenticateRequest, AddressOf Application_PostAuthenticateRequest
    End Sub

    Private Sub Application_PostAuthenticateRequest(source As Object, e As EventArgs)
        If DocSuiteContext.Current.ProtocolEnv.ShibbolethEnabled Then
            Dim shibbolethPrincipal As ShibbolethServerVariablePrincipal = New ShibbolethServerVariablePrincipal()
            HttpContext.Current.User = shibbolethPrincipal
        End If
    End Sub

    Public Sub Dispose() Implements IHttpModule.Dispose

    End Sub
End Class
