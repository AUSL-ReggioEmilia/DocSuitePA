Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonSelContactDomain
    Inherits CommonBasePage

#Region " Fields "
#End Region

#Region " Properties "
    Public ReadOnly Property ExceptDeskUser As String
        Get
            Return Request.QueryString("ExceptDeskUser").GetValueOrDefault(String.Empty)
        End Get
    End Property


    Public ReadOnly Property CallerId() As String
        Get
            Return Request.QueryString("ParentID")
        End Get
    End Property

    Public ReadOnly Property CheckEmailAddress As Boolean
        Get
            Return Request.QueryString("CheckEmailAddress").GetValueOrDefault(True)
        End Get
    End Property

    Public ReadOnly Property ButtonConfermaNuovoVisible As Boolean
        Get
            Return Request.QueryString("ConfermaNuovoVisible").GetValueOrDefault(True)
        End Get
    End Property

    Public ReadOnly Property IsIP4DRestriction As Boolean
        Get
            Return Request.QueryString("IP4DRestriction").GetValueOrDefault(False)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            MasterDocSuite.TitleVisible = False
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        btnConfermaNuovo.Visible = ButtonConfermaNuovoVisible
        If IsIP4DRestriction AndAlso Not ProtocolEnv.IP4DGroups.IsNullOrEmpty() Then
            uscUserSearch.ADRestrictionGroups = ProtocolEnv.IP4DGroups
        End If
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click

        If uscUserSearch.TreeViewControl.SelectedNode Is Nothing OrElse (uscUserSearch.TreeViewControl.SelectedNode.Index = 0 AndAlso uscUserSearch.TreeViewControl.SelectedNode.Text.Eq("Contatti")) Then
            AjaxAlert("Selezionare un contatto valido.")
            Return
        End If

        GetSelectedContact(True)
    End Sub

    Private Sub btnConfermaNuovo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfermaNuovo.Click
        GetSelectedContact(False)
    End Sub

#End Region

#Region " Methods "

    Private Sub GetSelectedContact(ByVal close As Boolean)
        Dim actionCallBack As String = String.Format("{0}(jsonRes);", If(close, "ReturnValuesJson", "ReturnValuesJsonAndNew"))
        GetContact(actionCallBack, uscUserSearch.TreeViewControl.SelectedNode)
    End Sub

    Private Sub GetContact(jsActionCallBack As String, contactNode As RadTreeNode)
        If contactNode Is Nothing Then
            Return
        End If

        If DocSuiteContext.Current.ProtocolEnv.ADUserEmailRestrictionEnabled AndAlso CheckEmailAddress AndAlso String.IsNullOrEmpty(contactNode.Value) Then
            AjaxAlert("Il contatto selezionato non ha un indirizzo email valido.")
            Return
        End If

        Dim currentFullUserName As String = Regex.Replace(contactNode.Attributes("DisplayName"), "\\", String.Empty)

        If String.Compare(ExceptDeskUser, currentFullUserName) = 0 Then
            AjaxAlert("L'utente selezionato è già proprietario del tavolo.")
            Return
        End If

        Dim dto As New Contact() With {.ContactType = New ContactType(ContactType.Aoo)}
        dto.Code = contactNode.Attributes("DisplayName")
        dto.Description = contactNode.Attributes("Description")
        dto.SearchCode = contactNode.Attributes("Account")
        If String.IsNullOrWhiteSpace(dto.Description) Then
            dto.Description = contactNode.Text
        End If
        dto.EmailAddress = contactNode.Value

        Dim serialized As String = StringHelper.EncodeJS(JsonConvert.SerializeObject(dto))
        Dim jsScript As String = String.Format("var jsonRes= '{0}'; {1}", serialized, jsActionCallBack)
        AjaxManager.ResponseScripts.Add(jsScript)
    End Sub



#End Region

End Class