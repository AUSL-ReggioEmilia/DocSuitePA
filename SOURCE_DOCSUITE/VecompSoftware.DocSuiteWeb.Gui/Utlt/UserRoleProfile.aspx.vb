Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UserRoleProfile
    Inherits CommBasePage

#Region " Fields "
    Private Const PAGE_TITLE_FORMAT As String = "Configurazione profilo dell'utente {0}"
#End Region

#Region " Properties "
    Public ReadOnly Property Account As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("RoleUser", String.Empty)
        End Get
    End Property

    Public ReadOnly Property MakeRelaod() As Boolean
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("MakeReload")) Then
                Return Boolean.Parse(Request.QueryString("MakeReload"))
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property SignSaveModalityDisabled() As Boolean
        Get
            If Not String.IsNullOrEmpty(Request.QueryString("SignSaveModalityDisabled")) Then
                Return Boolean.Parse(Request.QueryString("SignSaveModalityDisabled"))
            End If
            Return False
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscUserProfile.Account = Account
        uscUserProfile.MakeReload = MakeRelaod
        uscUserProfile.SignSaveModalityDisabled = SignSaveModalityDisabled
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