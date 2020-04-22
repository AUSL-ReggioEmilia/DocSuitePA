Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers

Partial Public Class SelUsersGroup
    Inherits CommonBasePage

#Region " Fields "

    Private _currentDSWEnvironment As Lazy(Of DSWEnvironment)

#End Region

#Region " Properties "

    Private ReadOnly Property RoleId() As Integer
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("IdRole", 0)
        End Get
    End Property

    Private ReadOnly Property RoleUserType As RoleUserType
        Get
            Dim rut As String = Request.QueryString.GetValue(Of String)("RoleUserType")
            If Not String.IsNullOrEmpty(rut) Then Return DirectCast([Enum].Parse(GetType(RoleUserType), rut), RoleUserType)
            Return RoleUserType.X
        End Get
    End Property

    Private ReadOnly Property CurrentDSWEnvironment As DSWEnvironment
        Get
            Return Me._currentDSWEnvironment.Value
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        Dim role As Role = Facade.RoleFacade.GetById(RoleId)

        If role Is Nothing Then
            Exit Sub
        End If

        If Not IsPostBack Then
            Dim users As List(Of AccountModel) = New List(Of AccountModel)()
            Dim results As Dictionary(Of String, AccountModel) = New Dictionary(Of String, AccountModel)()
            Dim securityUsers As IEnumerable(Of SecurityUsers) = role.RoleGroups.
                    Where(Function(f) f.SecurityGroup IsNot Nothing AndAlso f.SecurityGroup.SecurityUsers IsNot Nothing _
                    AndAlso (Not ProtocolEnv.CollaborationRightsEnabled OrElse (ProtocolEnv.CollaborationRightsEnabled AndAlso Facade.RoleGroupFacade.CheckGroupRights(f, GetDSWEnvironment())))).
                    SelectMany(Function(f) f.SecurityGroup.SecurityUsers).
                    Where(Function(f) Not String.IsNullOrEmpty(f.Account))

            For Each securityUser As SecurityUsers In securityUsers
                If Not results.ContainsKey(securityUser.DisplayName) Then
                    results.Add(securityUser.DisplayName, New AccountModel(securityUser.Account, securityUser.Description, securityUser.UserDomain))
                End If
            Next
            users = results.Values.ToList()

            If users.Any() Then
                LoadUsers(users)
            End If

        End If
    End Sub

#End Region

#Region " Methods "

    Private Function GetDSWEnvironment() As DSWEnvironment
        Dim value As String = Request.QueryString.GetValueOrDefault(Of String)("DSWEnvironment", Nothing)
        If String.IsNullOrWhiteSpace(value) Then
            Return DSWEnvironment.Any
        End If

        Dim result As DSWEnvironment = DSWEnvironment.Any
        If [Enum].TryParse(value, result) Then
            Return result
        End If

        Dim message As String = "Nessun DSWEnvironment definito per il valore ""{0}""."
        message = String.Format(message, value)
        Throw New ArgumentException(message)
    End Function

    Private Sub LoadUsers(users As IList(Of AccountModel))
        users = RemoveDuplicates(users)
        Dim node As RadTreeNode
        For Each usr As AccountModel In users.OrderBy(Function(f) f.Account)
            node = New RadTreeNode
            node.Text = usr.GetLabel()
            node.Value = usr.GetFullUserName()
            node.AddAttribute("account", usr.Account)
            node.AddAttribute("domain", usr.Domain)
            node.AddAttribute("name", usr.Name)
            node.Checkable = True
            node.ToolTip = "Selezionare il nome per inserirlo nel disegno di collaborazione. Più selezioni inseriscono più utenti contemporaneamente. Utilizzare il checkbox per attivare il settore principale."
            rtvAdUsers.Nodes(0).Nodes.Add(node)
        Next
    End Sub

    Private Function RemoveDuplicates(users As IList(Of AccountModel)) As IList(Of AccountModel)
        Dim currentUser As List(Of AccountModel) = users.ToList()
        Dim currentRoleUsers As IList(Of RoleUser) = Nothing
        Dim userToRemove As AccountModel = Nothing
        If DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled Then
            Dim finder As New NHRoleUserFinder()
            finder.RoleIdIn.Add(Me.RoleId)
            finder.TypeIn.Add(Me.RoleUserType.ToString())
            finder.DSWEnvironmentIn.Add(Me.GetDSWEnvironment())
            currentRoleUsers = finder.List()
        End If
        If (currentRoleUsers Is Nothing) Then
            currentRoleUsers = Facade.RoleUserFacade.GetByRoleIdAndType(RoleId, RoleUserType, Nothing, Nothing)
        End If

        currentUser.RemoveAll(Function(f) currentRoleUsers.Any(Function(r) r.Account.Eq(f.GetFullUserName())))
        Return currentUser
    End Function

#End Region

    Private Sub btnConferma_Click(sender As Object, e As EventArgs) Handles btnConferma.Click

        Dim users As List(Of DomainUser) = rtvAdUsers.CheckedNodes.
            Where(Function(f) Not String.IsNullOrEmpty(f.Value)).
            Select(Function(f) New DomainUser() With {.Account = f.Attributes("account"), .Name = f.Attributes("name"), .Domain = f.Attributes("domain"), .MainRole = True}).
            ToList()

        AjaxManager.ResponseScripts.Add(String.Format("return CloseWindow('{0}');", StringHelper.EncodeJS(JsonConvert.SerializeObject(users))))
    End Sub
End Class