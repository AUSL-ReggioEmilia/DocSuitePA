Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Public Class DocmAutorizza
    Inherits DocmBasePage

#Region " Fields "

    Private _publication As String

#End Region

#Region " Properties "

    Public Property Publication() As String
        Get
            If _publication Is Nothing Then
                _publication = Request.QueryString("Publication") = "1"
            End If
            Return _publication
        End Get
        Set(ByVal value As String)
            _publication = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        uscDocumentData.CurrentDocument = CurrentDocument
        InitializeAjax()
        If Not Page.IsPostBack Then
            Initialize()
        End If

    End Sub

    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim rolesAdded As ICollection(Of Integer) = uscSettori.RoleListAdded
        Dim rolesRemoved As ICollection(Of Integer) = uscSettori.RoleListRemoved

        If rolesAdded IsNot Nothing Then
            For Each idRole As Integer In rolesAdded
                Dim documentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocument.Year, CurrentDocument.Number)

                With documentToken
                    .IncrementalOrigin = CurrentDocument.DocumentTokens(0).IncrementalOrigin
                    .IsActive = 0
                    .Response = String.Empty
                    .DocStep = CurrentDocument.DocumentTokens(0).DocStep
                    .SubStep = CurrentDocument.DocumentTokens(0).SubStep
                    .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById("CC")
                    .RoleDestination = Facade.RoleFacade.GetById(idRole)
                    .RoleSource = CurrentDocument.DocumentTokens(0).RoleDestination
                    .OperationDate = DateTime.Now
                    .RegistrationUser = DocSuiteContext.Current.User.FullUserName
                    .RegistrationDate = DateTime.Now
                End With

                Facade.DocumentTokenFacade.Save(documentToken)
            Next
        End If

        If rolesRemoved IsNot Nothing Then
            For Each idRole As Integer In rolesRemoved

                Dim tokenTypeCc As String() = {"CC"}
                Dim roleId As String() = {idRole.ToString()}

                Dim documentTokens As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenByTokenType(CurrentDocument.Year, CurrentDocument.Number, tokenTypeCc, roleId, True)
                For Each docToken As DocumentToken In documentTokens
                    Facade.DocumentTokenFacade.UpdateResponse(docToken, "A")
                    Facade.DocumentTokenFacade.Save(docToken)
                Next
            Next
        End If

        'Inserimento log
        If DocumentEnv.IsEnvLogEnabled Then
            Facade.DocumentLogFacade.InsertRoles(CurrentDocument, rolesAdded, "Add")
            Facade.DocumentLogFacade.InsertRoles(CurrentDocument, rolesRemoved, "Del")
        End If

        RegisterFolderRefreshFullScript()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscSettori, uscSettori)
    End Sub

    Private Sub Initialize()

        uscDocumentData.Show()

        uscDocumentData.VisibleAltri = False
        uscDocumentData.VisibleClassificatoreModifica = False
        uscDocumentData.VisibleClassificazione = False
        uscDocumentData.VisibleContatti = False
        uscDocumentData.VisibleDate = False
        uscDocumentData.VisibleDateModifica = False
        uscDocumentData.VisibleDateSovrapposte = False
        uscDocumentData.VisibleDati = True
        uscDocumentData.VisibleDatiModifica = False
        uscDocumentData.VisibleGenerale = False
        uscDocumentData.VisiblePratica = True

        Dim documentTokenListCc As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"CC"}, True, , False)

        Dim roleList As New List(Of Role)
        For Each docToken As DocumentToken In documentTokenListCc
            roleList.Add(docToken.RoleDestination)
        Next

        Dim docTokenListP As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenRoleP(CurrentDocumentYear, CurrentDocumentNumber)

        Dim selected As New StringBuilder
        For Each token As DocumentToken In docTokenListP
            If selected.Length > 0 Then
                selected.Append("|")
            End If
            selected.Append(token.RoleDestination.Id)
        Next

        uscSettori.SelectedRoles = selected.ToString()
        uscSettori.MultiSelect = True
        uscSettori.RoleRestictions = RoleRestrictions.None
        uscSettori.Required = False
        uscSettori.SourceRoles = roleList
        uscSettori.DataBind()

    End Sub

#End Region

End Class