Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltUserGroup
    Inherits CommonBasePage

#Region " Fields "

    Private _currentSecurityGroups As IList(Of SecurityGroups) = Nothing

#End Region

#Region " Properties "

    Private ReadOnly Property CurrentSecurityGroups As IList(Of SecurityGroups)
        Get
            If _currentSecurityGroups Is Nothing Then
                _currentSecurityGroups = Facade.SecurityUsersFacade.GetGroupsByAccount(String.Concat(txtDomain.Text, "\", txtAccount.Text))
            End If
            Return _currentSecurityGroups
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Search()
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, tblRicerca, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub Initialize()
        txtAccount.Focus()
    End Sub

    ''' <summary> Cerca i gruppi appartenenti ad un utente </summary>
    Private Sub Search()
        tblRicerca.Rows.Clear()
        tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {"Informazioni utente"}, {"head"})
        Dim user As AccountModel = CommonAD.GetAccount(String.Concat(txtDomain.Text, "\", txtAccount.Text))
        If user IsNot Nothing Then
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("Account {0} [{1}]", user.Account, user.GetFullUserName())}, Nothing)
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("DisplayName {0} [{1}]", user.DisplayName, user.GetLabel())}, Nothing)
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("Email {0}", user.Email)}, Nothing)
        End If
        ' Gruppi
        Dim groups As New List(Of String)
        tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {"Gruppi (Security Group)"}, {"head"})
        Dim list As IList(Of SecurityGroups) = CurrentSecurityGroups
        For Each sg As SecurityGroups In list.OrderBy(Function(item) item.GroupName)
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("{0} [{1}]", sg.GroupName, sg.FullIncrementalPath)}, Nothing)
            groups.Add(sg.GroupName)
        Next
        If groups.IsNullOrEmpty() Then
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {"nessun elemento trovato"}, {"label"})
        End If

        ' Contenitori
        If DocSuiteContext.Current.IsDocumentEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {$"Contenitori - {DocSuiteContext.Current.DossierAndPraticheLabel}", "Gruppo", "Diritti"}, {"head", "head", "head"})
            BindContainers(DSWEnvironment.Document)
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {"Contenitori - Protocollo", "Gruppo", "Diritti"}, {"head", "head", "head"})
            BindContainers(DSWEnvironment.Protocol)
        End If
        If DocSuiteContext.Current.IsProtocolEnabled AndAlso ProtocolEnv.DocumentSeriesEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {"Contenitori - " & ProtocolEnv.DocumentSeriesName, "Gruppo", "Diritti"}, {"head", "head", "head"})
            BindContainers(DSWEnvironment.DocumentSeries)
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {"Contenitori - " & Facade.TabMasterFacade.TreeViewCaption, "Gruppo", "Diritti"}, {"head", "head", "head"})
            BindContainers(DSWEnvironment.Resolution)
        End If

        ' Settori
        If DocSuiteContext.Current.IsDocumentEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {$"Settori - {DocSuiteContext.Current.DossierAndPraticheLabel}", "Gruppo", "Diritti"}, {"head", "head", "head"})
            PrintSettori(DSWEnvironment.Document, Nothing)
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {"Settori - Protocollo", "Gruppo", "Diritti"}, {"head", "head", "head"})
            PrintSettori(DSWEnvironment.Protocol, Nothing)
        End If
        If DocSuiteContext.Current.IsProtocolEnabled AndAlso ProtocolEnv.DocumentSeriesEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {"Settori - " & ProtocolEnv.DocumentSeriesName, "Gruppo", "Diritti"}, {"head", "head", "head"})
            PrintSettori(DSWEnvironment.DocumentSeries, Nothing)
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {"Settori - " & Facade.TabMasterFacade.TreeViewCaption, "Gruppo", "Diritti"}, {"head", "head", "head"})
            PrintSettori(DSWEnvironment.Resolution, Nothing)
        End If

        Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactByGroups(groups)
        If contacts.IsNullOrEmpty() Then
            Exit Sub
        End If

        ' Rubrica
        tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {"Rubrica"}, {"head"})
        For Each contact As Contact In contacts
            Dim name As String = ""
            Dim group As String = ""

            If Not String.IsNullOrEmpty(contact.FullIncrementalPath) Then
                Dim treeS As String = Facade.ContactFacade.GetFullPath(contact.FullIncrementalPath)
                Dim s As String() = treeS.Split("|"c)
                name = s(0)
                If s.Length > 1 Then
                    group = s(1)
                End If
            End If
            tblRicerca.Rows.AddRaw(Nothing, {3}, {50, 50}, {name, group, ""}, {"label"})
        Next
    End Sub

    Private Sub BindContainers(environment As DSWEnvironment)
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(txtDomain.Text, txtAccount.Text, environment, Nothing, Nothing)
        If containers.IsNullOrEmpty() Then
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {"nessun elemento trovato"}, {"label"})
            Exit Sub
        End If
        For Each container As Container In containers
            For Each group As ContainerGroup In container.ContainerGroups
                If Not CommonShared.UserBelongsTo(DocSuiteContext.Current.CurrentDomainName, CurrentSecurityGroups, New List(Of ContainerGroup)({group})) Then
                    Continue For
                End If
                Dim rights As String = ContainerGroupFacade.TraslitteraDiritti(environment, group)
                If String.IsNullOrEmpty(rights) Then
                    Continue For
                End If
                tblRicerca.Rows.AddRaw(Nothing, {2}, {20, 20, 60}, {container.Name, group.SecurityGroup.GroupName, rights}, {"label"})
            Next
        Next
    End Sub

    Private Sub PrintSettori(environment As DSWEnvironment, ByVal role As Role)
        Dim roles As IList(Of Role) = Facade.RoleFacade.GetUserRoles(txtDomain.Text, txtAccount.Text, environment, 1, True, "", False, role)
        If roles.IsNullOrEmpty() Then
            tblRicerca.Rows.AddRaw(Nothing, {4}, Nothing, {"nessun elemento trovato"}, {"label"})
            Exit Sub
        End If
        For Each child As Role In roles.OrderBy(Function(r) r.FullIncrementalPath)
            For Each group As RoleGroup In child.RoleGroups
                If Not CurrentSecurityGroups.Contains(group.SecurityGroup) Then
                    Continue For
                End If

                Dim rightsDescription As String = RoleGroupFacade.TraslitteraDiritti(environment, group)
                tblRicerca.Rows.AddRaw(Nothing, Nothing, {12, 8, 20, 60}, {child.FullIncrementalPath, child.Name, group.Name, rightsDescription}, {"label", "label"})

            Next
        Next
    End Sub

#End Region

End Class
