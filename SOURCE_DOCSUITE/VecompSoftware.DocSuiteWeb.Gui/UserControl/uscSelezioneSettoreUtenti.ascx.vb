Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Partial Public Class uscSelezioneSettoreUtenti
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Const Pipe As Char = "|"c
    Const SEP As Char = ","c

    Const DIRITTI_GESTIONE As String = "1000000000"
    Const DIRITTI_WORKFLOW As String = "1100000000"
    Const DIRITTI_MANAGER As String = "1110000000"

    Const NODETEXTCOLOR_BLU As String = "#00008B"
    Const NODETEXTCOLOR_DEL As String = "#808080"

    Private _document As Document
    Private _year As Short
    Private _roleS As String = String.Empty
    Private _fullManagerS As String = String.Empty
    Private _stepP As String = String.Empty

#End Region

#Region " Properties "

    Public Property Year() As Short
        Get
            Return _year
        End Get
        Set(ByVal value As Short)
            _year = value
        End Set
    End Property

    Public Property Number() As Integer

    Public Property CurrentDocument() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
        End Set
    End Property

    Public Property RoleS() As String
        Get
            Return _roleS
        End Get
        Set(ByVal value As String)
            _roleS = value
        End Set
    End Property

    Public Property FullManagerS() As String
        Get
            Return _fullManagerS
        End Get
        Set(ByVal value As String)
            _fullManagerS = value
        End Set
    End Property

    Public Property StepP() As String
        Get
            Return _stepP
        End Get
        Set(ByVal value As String)
            _stepP = value
        End Set
    End Property

    Public ReadOnly Property Treeview() As RadTreeView
        Get
            Return Tvw
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Initialize()
    End Sub

    Private Sub ddlToken_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlToken.SelectedIndexChanged
        Utenti()
    End Sub

#End Region

#Region " Methods "

    Public Sub Initialize()
        'verifica manager
        Dim ManagerPC As Boolean
        Dim ManagerPT As Boolean
        Dim roleI As Integer = Integer.Parse(Replace(RoleS, Pipe, String.Empty))
        Dim fullManagerI As String = Replace(FullManagerS, Pipe, String.Empty)
        Dim arrFullManagerS As String() = fullManagerI.Split(SEP)
        Dim tokenTypes As String() = {"PT"}
        Dim docTokenList As IList(Of DocumentToken)

        ManagerPC = FullManagerS.Contains(RoleS)
        docTokenList = Facade.DocumentTokenFacade.GetTokenOperationDateList(Year, Number, tokenTypes, arrFullManagerS)
        ManagerPT = docTokenList.Count > 0

        Dim lstItem As ListItem
        If ManagerPC Then
            lstItem = New ListItem
            lstItem.Text = Replace(StepP, Pipe, "")
            FullRole(lstItem, roleI)
            lstItem.Value = roleI
            ddlToken.Items.Add(lstItem)
        End If
        If ManagerPT Then
            For Each docT As DocumentToken In docTokenList
                lstItem = New ListItem
                lstItem.Text = docT.DocStep
                If docT.SubStep <> 0 Then lstItem.Text &= "." & docT.SubStep
                FullRole(lstItem, docT.RoleDestination.Id)
                lstItem.Value = docT.RoleDestination.Id
                ddlToken.Items.Add(lstItem)
            Next
        End If
        Select Case ddlToken.Items.Count
            Case 0
                Throw New DocSuiteException(
                    DocumentUtil.DocmFull(Year, Number),
                    "L'Utente non ha i diritti di Manager. Riaprire la Pratica.",
                    Request.Url.ToString(),
                    DocSuiteContext.Current.User.FullUserName)
            Case 1
                ddlToken.Enabled = False
        End Select
        ddlToken.SelectedIndex = 0
        Utenti()

    End Sub

    Private Sub FullRole(ByRef itm As ListItem, ByVal idRole As Integer)

        Dim FullRole As String = ""

        Dim role As Role = Facade.RoleFacade.GetById(idRole)
        If Not role Is Nothing Then
            SetRoleString(FullRole, role.FullIncrementalPath)
        Else
            FullRole = "Errore in ricerca Ruolo"
        End If
        itm.Text &= " " & FullRole
    End Sub

    Private Sub SetRoleString(ByRef testo As String, ByVal fullIncremental As String)
        If String.IsNullOrEmpty(fullIncremental) Then
            Exit Sub
        End If

        Dim r As String() = fullIncremental.Split(Pipe)
        Dim role As Role = Facade.RoleFacade.GetById(Integer.Parse(r(0)))

        If role Is Nothing Then
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(testo) Then
            testo &= "/"
        End If
        testo &= role.Name
        If r.Length > 1 Then
            SetRoleString(testo, Mid$(fullIncremental, InStr(fullIncremental, Pipe) + 1))
        End If
    End Sub

    Private Sub Utenti()
        Dim StepI As Integer
        Dim SubStepI As Integer
        Dim tn As RadTreeNode
        Dim tnUser As RadTreeNode
        Dim idRoleDestination As Integer = ddlToken.SelectedItem.Value
        Dim docTokenUserList As IList(Of DocumentTokenUser)
        Dim roleList As IList(Of Role)
        Dim role As Role

        EstraiStep(StepI, SubStepI)

        docTokenUserList = Facade.DocumentTokenUserFacade.DocumentTokenUserSearch(Year, Number, , , idRoleDestination, True)

        'nodo start
        Tvw.Nodes.Clear()
        tn = New RadTreeNode

        tn.Text = "Titolo"
        tn.Value = ""
        tn.Expanded = True
        tn.ImageUrl = ImagePath.SmallRole
        tn.Style.Add("font-weight", "bold")
        tn.Checkable = False
        Tvw.Nodes.Add(tn)

        role = Facade.RoleFacade.GetById(idRoleDestination)

        If Not role Is Nothing And role.IsActive = 1 Then
            roleList = Facade.RoleFacade.GetUserRights(BasePage.Type, role, "1__0000000")
            If roleList.Count > 0 Then
                Tvw.Nodes(0).Text = roleList(0).Name

                For Each roleGroup As RoleGroup In roleList(0).RoleGroups
                    tn = New RadTreeNode
                    Dim Settore As String = roleGroup.Name
                    Dim Rights As String = ""
                    Dim bManager As Boolean = False

                    Select Case roleGroup.DocumentRights
                        Case DIRITTI_GESTIONE
                            Rights = " (Gestione)"
                        Case DIRITTI_WORKFLOW
                            Rights = " (WorkFlow)"
                        Case DIRITTI_MANAGER
                            Rights = " (Manager)"
                            bManager = True
                    End Select
                    tn.Text = Settore & Rights
                    tn.Value = Settore
                    tn.Expanded = True
                    tn.ImageUrl = ImagePath.SmallSubRole
                    tn.Style.Add("font-weight", "bold")
                    tn.Checkable = False
                    Tvw.Nodes(0).Nodes.Add(tn)

                    Dim users As IList(Of AccountModel)
                    Try
                        users = GetUsers(roleGroup)

                        For Each user As AccountModel In users
                            tnUser = New RadTreeNode
                            tnUser.Text = user.Account
                            tnUser.Value = user.GetFullUserName()
                            If Not bManager Then
                                tnUser.Checkable = True
                            Else
                                tnUser.Checkable = False
                            End If
                            tnUser.ImageUrl = "../Comm/Images/User16.gif"
                            tnUser.Style.Add("font-weight", "bold")

                            If docTokenUserList.Count > 0 Then

                                Dim docTokenUserFound As DocumentTokenUser
                                docTokenUserFound = GetRoleFromList(docTokenUserList, Settore, user.Account, idRoleDestination)

                                If docTokenUserFound IsNot Nothing Then
                                    Select Case docTokenUserFound.IsActive
                                        Case 0
                                            tnUser.Style.Add("color", NODETEXTCOLOR_DEL)
                                        Case 1
                                            tnUser.Style.Add("color", NODETEXTCOLOR_BLU)
                                            tnUser.Checked = True
                                    End Select
                                End If
                            End If
                            tn.Nodes.Add(tnUser)
                        Next
                    Catch ex As Exception
                        tn.Style.Add("color", NODETEXTCOLOR_DEL)
                    End Try

                Next
            End If
        End If

    End Sub

    Private Function GetUsers(group As RoleGroup) As IList(Of AccountModel)
        If group.SecurityGroup IsNot Nothing Then
            Return Facade.SecurityUsersFacade.GetUsersByGroup(group.SecurityGroup).Select(Function(f) New AccountModel(f.Account, f.Description, domain:=f.UserDomain)).ToList()
        End If
        Return New List(Of AccountModel)()
    End Function

    Private Function GetRoleFromList(ByVal docTokenUserList As IList(Of DocumentTokenUser), ByVal settore As String, ByVal userAccount As String, ByVal idRoleDestination As Integer) As DocumentTokenUser
        Dim docTokenUserFound As DocumentTokenUser = Nothing
        For Each doctokenUser As DocumentTokenUser In docTokenUserList
            If (doctokenUser.UserRole = settore And doctokenUser.Account = userAccount And doctokenUser.Role.Id = idRoleDestination) Then
                docTokenUserFound = doctokenUser
                Exit For
            End If
        Next
        Return docTokenUserFound
    End Function

    Private Sub EstraiStep(ByRef StepI As Integer, ByRef SubStepI As Integer)
        Dim s As String = ddlToken.SelectedItem.Text
        s = Left$(s, InStr(s, " ") - 1)
        Dim a As Array = Split(s, ".")
        StepI = 0
        SubStepI = 0
        StepI = a(0)
        If a.Length = 2 Then
            SubStepI = a(1)
        End If
    End Sub

    Public Sub TvwCheck(ByVal nodo As RadTreeNode)
        Dim StepI As Integer
        Dim SubStepI As Integer
        If nodo.Nodes.Count = 0 Then Exit Sub

        EstraiStep(StepI, SubStepI)
        Dim idRoleDestination As Integer = ddlToken.SelectedItem.Value

        Dim serverDate As DateTimeOffset = DateTimeOffset.UtcNow
        For Each tn As RadTreeNode In nodo.Nodes
            Dim nParent As RadTreeNode = CType(tn.Parent, RadTreeNode)
            Dim UserRole As String = nParent.Value
            Dim NodeColor As String = tn.Style("color")
            Dim account As String = tn.Value
            Dim userName As String = tn.Text

            If tn.Checkable Then
                Select Case tn.Checked
                    Case True
                        Select Case NodeColor
                            Case NODETEXTCOLOR_DEL
                                Dim docTokenUser As IList(Of DocumentTokenUser) = Facade.DocumentTokenUserFacade.DocumentTokenUserSearch(Year, Number, , , idRoleDestination, , account)
                                If (docTokenUser.Count = 1) Then
                                    With docTokenUser(0)
                                        .LastStep = StepI
                                        .LastSubStep = SubStepI
                                        .IsActive = 1
                                        .LastChangedUser = DocSuiteContext.Current.User.FullUserName
                                        .LastChangedDate = serverDate
                                    End With
                                    Facade.DocumentTokenUserFacade.Update(docTokenUser(0))
                                End If
                            Case Else
                                Dim docTokenUser As DocumentTokenUser = Facade.DocumentTokenUserFacade.CreateDocumentTokenUser(Year, Number)
                                With docTokenUser
                                    .DocStep = StepI
                                    .SubStep = SubStepI
                                    .IsActive = 1
                                    .Role = Facade.RoleFacade.GetById(idRoleDestination)
                                    .UserRole = UserRole
                                    .UserName = userName
                                    .Account = account
                                    .Note = String.Empty
                                    .RegistrationUser = DocSuiteContext.Current.User.FullUserName
                                    .RegistrationDate = serverDate
                                    .LastStep = 0
                                    .LastSubStep = 0
                                End With
                                Facade.DocumentTokenUserFacade.Save(docTokenUser)
                        End Select
                    Case False
                        Select Case NodeColor
                            Case NODETEXTCOLOR_BLU
                                Dim docTokenUser As IList(Of DocumentTokenUser) = Facade.DocumentTokenUserFacade.DocumentTokenUserSearch(Year, Number, , , idRoleDestination, True, account)
                                If (docTokenUser.Count = 1) Then
                                    With docTokenUser(0)
                                        .LastStep = StepI
                                        .LastSubStep = SubStepI
                                        .IsActive = 0
                                        .LastChangedUser = DocSuiteContext.Current.User.FullUserName
                                        .LastChangedDate = serverDate
                                    End With
                                    Facade.DocumentTokenUserFacade.Update(docTokenUser(0))
                                End If
                        End Select
                End Select
            End If
            TvwCheck(tn)
        Next
    End Sub

#End Region

End Class