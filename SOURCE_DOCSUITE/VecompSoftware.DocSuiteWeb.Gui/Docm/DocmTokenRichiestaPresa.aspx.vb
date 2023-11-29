Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging

Public Class DocmTokenRichiestaPresa
    Inherits DocmBasePage

#Region "Fields"
    Private _documentTokenListActive As IList(Of DocumentToken)
#End Region

#Region "Properties"
    Private ReadOnly Property IdRoleDestination() As Integer
        Get
            Return Replace(Request.QueryString("txtPIdOwner"), "|", "")
        End Get
    End Property

    Private ReadOnly Property DocumentTokensActive() As IList(Of DocumentToken)
        Get
            If _documentTokenListActive Is Nothing Then
                _documentTokenListActive = Facade.DocumentTokenFacade.GetByYearNumber(CurrentDocumentYear, CurrentDocumentNumber, True)
            End If
            Return _documentTokenListActive
        End Get
    End Property
#End Region

#Region "Page Load"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        'verifica token valido di presa in carico
        If Not Facade.DocumentTokenFacade.VerifyDocumentTokenRoleP(CurrentDocumentYear, CurrentDocumentNumber, IdRoleDestination) Then
            Throw New DocSuiteException("Apertura pagina", "Operazione Effettuata da Altro Utente. Riaprire la Pratica.")
        End If

        If Not Me.IsPostBack Then
            'verifica documenti in check out
            If Facade.DocumentVersioningFacade.GetDocumentVersion(CurrentDocumentYear, CurrentDocumentNumber, 0, "O") IsNot Nothing Then
                AjaxAlert("Ci sono Documenti in Check Out.{0}Non è possibile eseguire una richiesta di Presa in Carico.", Environment.NewLine)
                btnConferma.Enabled = False
                Exit Sub
            End If
            Initialize()
        End If

        'selezione settori con locazione
        uscDocumentToken.ControlSettoreDestinatario.MultiSelect = True
        uscDocumentToken.ControlSettoreDestinatario.RoleRestictions = RoleRestrictions.None
        uscDocumentToken.ControlSettoreDestinatario.Rights = "10000000000000000000"

        'selezione settori con locazione
        Dim selected As String = String.Empty
        Dim docTokenListCC As IList = Facade.DocumentTokenFacade.GetTokenList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"CC"}, True, , False)
        For Each token As DocumentToken In docTokenListCC
            selected &= token.RoleDestination.Id.ToString & "|"
        Next
        Dim docTokenListP As IList = Facade.DocumentTokenFacade.GetDocumentTokenRoleP(CurrentDocumentYear, CurrentDocumentNumber)
        For Each token As DocumentToken In docTokenListP
            selected &= token.RoleDestination.Id.ToString & "|"
        Next

        uscDocumentToken.ControlSettoreCC.SelectedRoles = selected.TrimEnd("|"c)
        uscDocumentToken.ControlSettoreCC.MultiSelect = True
        uscDocumentToken.ControlSettoreCC.RoleRestictions = RoleRestrictions.None
        uscDocumentToken.ControlSettoreCC.Rights = "10000000000000000000"
        Dim documentTokensCC As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True)
        For Each documentToken As DocumentToken In documentTokensCC
            uscDocumentToken.ControlSettoreCC.SourceRoles.Add(documentToken.RoleDestination)
        Next
    End Sub
#End Region

#Region "Initialize"
    Private Sub Initialize()
        uscDocumentToken.TextTokenType = "RC"
        uscDocumentToken.TextTokenName = Facade.DocumentTabTokenFacade.GetById(uscDocumentToken.TextTokenType).Description

        Dim role As Role = Facade.RoleFacade.GetById(IdRoleDestination)
        If role IsNot Nothing Then
            uscDocumentToken.ControlSettoreMittente.SourceRoles.Add(role)
            uscDocumentToken.ControlSettoreMittente.DataBind()
        End If
    End Sub
#End Region

#Region "Conferma Button Event"
    Private Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim IncrementalOrigin As Integer

        If DocumentTokensActive.Count = 1 Then
            IncrementalOrigin = DocumentTokensActive(0).Incremental
        Else
            AjaxAlert("Errore in ricerca Step Attivo")
            Exit Sub
        End If

        Dim iStep As Short = Facade.DocumentTokenFacade.GetMaxStep(CurrentDocument.DocumentTokens)
        'mittenti
        If uscDocumentToken.ControlSettoreMittente.Count <> 1 Then
            AjaxAlert("Settore Mittente non valido")
            Exit Sub
        End If

        'destinatari
        If uscDocumentToken.ControlSettoreDestinatario.Count = 0 Then
            AjaxAlert("Inserire un Settore Destinazione")
            Exit Sub
        End If

        'verifica settore destinatario diverso da quello mittente
        Dim _roleSourceList As List(Of Role) = uscDocumentToken.ControlSettoreMittente.GetRoles()
        Dim roleMittente As Role = _roleSourceList(0)

        Dim _roleDestinationList As List(Of Role) = uscDocumentToken.ControlSettoreDestinatario.GetRoles()
        For Each roleDestinatario As Role In _roleDestinationList
            If roleDestinatario.Equals(roleMittente) Then
                AjaxAlert("il settore Destinatario deve essere diverso da quello Mittente")
                Exit Sub
            End If
        Next

        'cambio stato isactive al documenttoken
        Dim originDocumentToken As DocumentToken = Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, IncrementalOrigin)
        Facade.DocumentTokenFacade.UpdateIsActive(originDocumentToken, False)

        Dim idSubStep As Integer = 0
        Dim listRoles As IList(Of Role) = uscDocumentToken.ControlSettoreDestinatario.GetRoles()
        For Each roleDestinatario As Role In listRoles
            Dim newDocumentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
            idSubStep = If(listRoles.Count = 1, 0, idSubStep + 1)
            With newDocumentToken
                .IncrementalOrigin = IncrementalOrigin
                .Response = String.Empty
                .IsActive = True
                .DocStep = iStep + 1S
                .SubStep = idSubStep
                .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById(uscDocumentToken.TextTokenType)
                .RoleDestination = roleDestinatario
                .RoleSource = roleMittente
                .ExpiryDate = uscDocumentToken.ControlDatiRichiesta.DateText
                .DocObject = uscDocumentToken.ControlDatiRichiesta.ObjectText
                .Note = uscDocumentToken.ControlDatiRichiesta.NoteText
                .Reason = uscDocumentToken.ControlDatiRichiesta.ReasonText
                .OperationDate = DateTime.Now
            End With
            Facade.DocumentTokenFacade.Save(newDocumentToken)
            If roleDestinatario IsNot Nothing Then
                Facade.DocumentFolderFacade.InsertFoldersRole(CurrentDocumentYear, CurrentDocumentNumber, roleDestinatario.FullIncrementalPath, Nothing)
            End If
        Next

        'Settori in CC
        Dim _roleCCList As List(Of Role) = uscDocumentToken.ControlSettoreCC.GetRoles()
        For Each roleCC As Role In _roleCCList
            Dim newDocumentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
            With newDocumentToken
                .IncrementalOrigin = IncrementalOrigin
                .Response = String.Empty
                .IsActive = False
                .DocStep = iStep + 1S
                .SubStep = 0
                .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById("CC")
                .RoleDestination = roleCC
                .RoleSource = roleMittente
                .OperationDate = DateTime.Now
            End With
            Facade.DocumentTokenFacade.Save(newDocumentToken)
        Next

        If _roleDestinationList.Count > 0 Then
            Try
                DocUtil.FncSendMail(_roleSourceList, _roleDestinationList, Facade.DocumentTabTokenFacade.GetById(uscDocumentToken.TextTokenType), CurrentDocumentYear, CurrentDocumentNumber, uscDocumentToken.ControlDatiRichiesta.DateText, uscDocumentToken.ControlDatiRichiesta.ObjectText, uscDocumentToken.ControlDatiRichiesta.ReasonText, uscDocumentToken.ControlDatiRichiesta.NoteText, "", "")
            Catch ex As DocSuiteException
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(ex)
            End Try
        End If

        If _roleCCList.Count > 0 Then
            Try
                DocUtil.FncSendMail(_roleSourceList, _roleCCList, Facade.DocumentTabTokenFacade().GetById("CC"), CurrentDocumentYear, CurrentDocumentNumber, Nothing, "", "", "", "", "")
            Catch ex As DocSuiteException
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(ex)
            End Try
        End If

        RegisterFolderRefreshFullScript()
    End Sub
#End Region

End Class
