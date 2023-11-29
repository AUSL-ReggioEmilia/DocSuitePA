Imports System.Collections.Generic
Imports System.Linq
Imports NHibernate
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager

Public Class UscSearchADUser
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Public ReadOnly Property selectedNode As RadTreeNode
        Get
            Return tvwContactDomain.SelectedNode
        End Get
    End Property

    Public Property ADRestrictionGroups As IList(Of String)
        Get
            If ViewState("ADRestrictionGroups") IsNot Nothing Then
                Return DirectCast(ViewState("ADRestrictionGroups"), IList(Of String))
            End If
            Return New List(Of String)
        End Get
        Set(value As IList(Of String))
            ViewState("ADRestrictionGroups") = value
        End Set
    End Property

    Public ReadOnly Property TreeViewControl As RadTreeView
        Get
            Return tvwContactDomain
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack() Then
            txtFilter.Focus()
            Initialize()
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Dim filter As String = txtFilter.Text.Trim()

        If filter.Contains("/") Then
            BasePage.AjaxAlert("Il filtro di ricerca non può contenere il carattere /", False)
            Exit Sub
        End If

        If filter.Length < 2 Then
            BasePage.AjaxAlert("Il filtro di ricerca deve essere di almeno 2 caratteri.", False)
            Exit Sub
        End If

        InitializeTree()

        filter = Helpers.StringHelper.ReplaceBackSlash(filter)
        Dim currentDomain As String = String.Empty
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled AndAlso lbMultiDomain.SelectedItem IsNot Nothing AndAlso Not lbMultiDomain.SelectedItem.Value.Eq("-") Then
            currentDomain = lbMultiDomain.SelectedItem.Text
        End If

        Dim found As IList(Of AccountModel)
        If Not ADRestrictionGroups.IsNullOrEmpty() Then
            found = CommonAD.FindADUsers(filter, currentDomain, ADRestrictionGroups)
        Else
            found = CommonAD.FindADUsers(filter, currentDomain)
        End If

        If found.IsNullOrEmpty() Then
            BasePage.AjaxAlert("Nessun risultato disponibile.", False)
            Exit Sub
        End If

        Dim result As IEnumerable(Of AccountModel) = found

        If DocSuiteContext.Current.ProtocolEnv.RubricaDomainExistingOnly Then
            result = found.Where(Function(u) Facade.SecurityUsersFacade.ExistsUser(u))
        End If

        If DocSuiteContext.Current.ProtocolEnv.ADUserEmailRestrictionEnabled Then
            result = result.Where(Function(f) Not String.IsNullOrEmpty(f.Email))
        End If

        result.GroupBy(Function(u) u.GetLabel()) _
            .Select(Function(u) u.First()) _
            .OrderBy(Function(u) u.Name).ToList() _
            .ForEach(Sub(r) AddToTree(r))
    End Sub

    Public Event NodeClicked As EventHandler

    Protected Sub CheckNoteInput(ByVal sender As Object, ByVal e As EventArgs) Handles tvwContactDomain.NodeClick
        RaiseEvent NodeClicked(Me, EventArgs.Empty)
    End Sub


#End Region

#Region "Methods"
    Private Sub Initialize()

        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            lbMultiDomain.Visible = True

            Dim tenantModels As IList(Of TenantModel) = DocSuiteContext.Current.Tenants.ToList()
            lbMultiDomain.Items.Add(New ListItem("Tutti domini", "-"))
            For Each tenantModel As TenantModel In tenantModels
                lbMultiDomain.Items.Add(New ListItem(tenantModel.DomainName, tenantModel.DomainAddress))
            Next
            If DocSuiteContext.Current.ProtocolEnv.DefaultTenantEnabled Then
                lbMultiDomain.Items.FindByValue(DocSuiteContext.Current.CurrentTenant.DomainAddress).Selected = True
            End If
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, tvwContactDomain)
    End Sub

    Private Sub InitializeTree()
        tvwContactDomain.Nodes.Clear()

        Dim root As New RadTreeNode() With {.Text = "Contatti", .Expanded = True}
        tvwContactDomain.Nodes.Add(root)
        tvwContactDomain.Nodes(0).Selected = True
    End Sub

    Protected Sub RadToolTipmanager_AjaxUpdate(sender As Object, e As ToolTipUpdateEventArgs)
        Dim input As String = e.Value
        Dim split = input.Split(New String() {"|"},
                        StringSplitOptions.RemoveEmptyEntries)

        For i As Integer = 0 To split.Length - 1
            Dim htmlGenericControl As HtmlGenericControl = New HtmlGenericControl("SPAN")
            htmlGenericControl.InnerText = split(i)
            e.UpdatePanel.ContentTemplateContainer.Controls.Add(htmlGenericControl)
            e.UpdatePanel.ContentTemplateContainer.Controls.Add(New HtmlGenericControl("HR"))
        Next
    End Sub

    Private Sub AddToTree(adUser As AccountModel)
        Dim node As New RadTreeNode()
        If String.IsNullOrEmpty(adUser.Email) Then
            adUser.Email = GetCurrentUserEmail(adUser.Account, adUser.Domain, adUser.Email)
        End If
        node.Text = adUser.GetLabel()
        node.Value = adUser.Email
        node.Attributes.Add("Account", adUser.Account)
        node.Attributes.Add("DisplayName", adUser.GetFullUserName())
        node.Attributes.Add("Description", adUser.Name)
        node.Attributes.Add("Domain", adUser.Domain)
        node.Attributes.Add("TooltipText", GetTooltipText(adUser))
        node.ImageUrl = "../App_Themes/DocSuite2008/imgset16/help.png"
        RadToolTipManager.TargetControls.Add(node.ClientID, True)

        tvwContactDomain.Nodes(0).Nodes.Add(node)
    End Sub

    Private Function GetTooltipText(adUser As AccountModel) As String
        Dim firstName As String = $"Nome: {adUser.FirstName}"
        Dim lastName As String = $"Cognome: {adUser.LastName}"
        Dim email As String = $"Email: {adUser.Email}"
        Dim principale As String = $"Principale: {adUser.UserPrincipalName}"

        Return $"{firstName}|{lastName}|{email}|{principale}"
    End Function

    Private Function GetCurrentUserEmail(account As String, domain As String, currentUserAdMail As String) As String
        Dim mail As String = String.Empty
        Dim currentUserLog As UserLog = Facade.UserLogFacade.GetByUser(account, domain)
        If ProtocolEnv.EnableUserProfile Then
            If currentUserLog Is Nothing Then
                Return currentUserAdMail
            End If

            mail = currentUserLog.UserMail
            If String.IsNullOrEmpty(mail) Then
                ' Recupero da Dominio
                mail = currentUserAdMail
            End If
        Else
            mail = currentUserAdMail
            If String.IsNullOrEmpty(mail) Then
                ' Recupero da UserLog
                mail = Facade.UserLogFacade.EmailOfUser(account, domain, True)
            End If
        End If
        Return mail
    End Function
#End Region


End Class