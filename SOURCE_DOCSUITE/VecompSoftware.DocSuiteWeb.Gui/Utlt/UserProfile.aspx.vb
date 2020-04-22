Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UserProfile
    Inherits CommBasePage

#Region " Fields "
    Private Const PAGE_TITLE_FORMAT As String = "Configurazione profilo dell'utente {0}"
#End Region

#Region " Properties "
    Public ReadOnly Property Account() As String
        Get
            Return DocSuiteContext.Current.User.FullUserName
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscUserProfile.Account = Account
        uscUserProfile.MakeReload = True
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        Title = String.Format(PAGE_TITLE_FORMAT, Account)
    End Sub
#End Region

End Class
