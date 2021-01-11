Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonSelMailRecipients
    Inherits CommonBasePage

#Region "Fields"

#End Region

#Region "Properties"

    Private ReadOnly Property RoleIds As String
        Get
            Return Request.QueryString.GetValueOrDefault("Roles", String.Empty)
        End Get
    End Property

    Private ReadOnly Property AlreadySentRoleIds As String
        Get
            Return Request.QueryString.GetValueOrDefault("Sent", String.Empty)
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll.Click
        uscSettori.CheckAll()
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeselectAll.Click
        uscSettori.UnCheckAll()
    End Sub

    Private Sub btnInvia_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInvia.Click
        If uscSettori.GetCheckedRoles().Count <= 0 Then
            AjaxAlert("Selezionare un Settore")
            Exit Sub
        End If

        Dim mailList As New List(Of Integer)
        For Each idRole As Integer In uscSettori.GetCheckedRoleIds()
            mailList.Add(idRole)
        Next
        Dim closeFunction As String = String.Concat("CloseWindow('", String.Join(";", mailList), "');")
        AjaxManager.ResponseScripts.Add(closeFunction)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInvia, btnInvia)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, uscSettori)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, uscSettori)
    End Sub

    Private Sub Initialize()
        If Not String.IsNullOrEmpty(RoleIds) Then
            Dim splitted As String() = RoleIds.Split("|"c)
            Dim result As Integer
            Dim ids As Integer() = splitted.Where(Function(t) Integer.TryParse(t, result)).Select(Function(r) result).ToArray()
            Dim totalRoles As List(Of Role) = ids.Select(Function(i) Facade.RoleFacade.GetById(i)).ToList()
            uscSettori.SourceRoles = totalRoles
            uscSettori.DataBind()
            If Not String.IsNullOrEmpty(AlreadySentRoleIds) Then
                uscSettori.UnCheckAll()
                Dim sentSplitted As String() = AlreadySentRoleIds.Split("|"c)
                Dim out As Integer
                Dim sentIds As Integer() = sentSplitted.Where(Function(t) Integer.TryParse(t, out)).Select(Function(r) out).ToArray()
                Dim sentRoles As IList(Of Role) = Facade.RoleFacade.GetByIds(sentIds.Select(Function(i) i).ToList())
                For Each role As Role In totalRoles
                    If Not sentRoles.Any(Function(r) r.UniqueId.Equals(role.UniqueId)) Then
                        uscSettori.CheckRole(role)
                    End If
                Next
            End If
        End If
    End Sub

#End Region

End Class
