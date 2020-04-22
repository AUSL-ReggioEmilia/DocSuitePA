Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class UDSLog
    Inherits UDSBasePage

    Private _isAdmin As Boolean?

    Protected ReadOnly Property HasAdminRight As Boolean
        Get
            If Not _isAdmin.HasValue Then
                _isAdmin = CommonShared.HasGroupAdministratorRight
            End If
            Return _isAdmin.Value
        End Get
    End Property

    Protected ReadOnly Property CurrentUser As String
        Get
            Return DocSuiteContext.Current.User.FullUserName
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

End Class