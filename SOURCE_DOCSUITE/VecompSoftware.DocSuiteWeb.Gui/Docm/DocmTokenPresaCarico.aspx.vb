Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging

Public Class DocmTokenPresaCarico
    Inherits DocmBasePage

#Region "Properties"

    Public ReadOnly Property RoleRightsWList() As String()
        Get
            Dim _roles As String = Request.QueryString("txtIdRoleRightW").Replace("|", "")
            Return Split(_roles, ",")
        End Get
    End Property

#End Region

#Region "Page Load"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        WebUtils.ObjAttDisplayNone(txtProgrIncremental)
        WebUtils.ExpandOnClientNodeAttachEvent(radTreeSettoriInAttesa)

        InitializeAjaxSettings()
        InitializePage()
        If Not Page.IsPostBack Then
            Initialize()
        Else
            If Not VerifyOperation() Then
                Throw New DocSuiteException("Operazione pratica", "Operazione Effettuata da Altro Utente. Riaprire la Pratica.")
            End If
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Function VerifyOperation() As Boolean
        Dim _documentTokens As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RR", "RP"}, RoleRightsWList)
        If _documentTokens.Count = 0 Then
            Return False
        End If
        Return True
    End Function


    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf DocmTokenPresaCarico_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDistribuzione, uscDistribuzione)
    End Sub

    Private Sub InitializePage()
        'Visualizzazti solo campi Step e Tipologia
        uscDocumentToken.RestituzioneVisible = False
        'Settore Mittente sola lettura
        uscDocumentToken.ControlSettoreMittente.ReadOnly = True
        'Settore Destinatario sola lettura
        uscDocumentToken.ControlSettoreDestinatario.ReadOnly = True
        'Settori Copia Conoscenza non visibili
        uscDocumentToken.PanelSettoriCCVisible = False
        'Etichette pannello dettagli
        'Data scadenza
        uscDocumentToken.ControlDatiRichiesta.DateLabel = "Data scadenza:"
        uscDocumentToken.ControlDatiRichiesta.DateReadOnly = True
        'Oggetto
        uscDocumentToken.ControlDatiRichiesta.ObjectLabel = "Oggetto:"
        uscDocumentToken.ControlDatiRichiesta.ObjectReadOnly = True
        'Motivo
        uscDocumentToken.ControlDatiRichiesta.ReasonLabel = "Motivo:"
        uscDocumentToken.ControlDatiRichiesta.ReasonReadOnly = True
        'Note
        uscDocumentToken.ControlDatiRichiesta.NoteLabel = "Note:"
        uscDocumentToken.ControlDatiRichiesta.NoteReadOnly = True
    End Sub

    Private Sub Initialize()
        Dim _documentTokens As IList(Of DocumentToken)
        Dim _currentToken As DocumentToken


        _documentTokens = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"RC", "RR", "RP"}, RoleRightsWList)
        If _documentTokens.Count = 0 Then
            Throw New DocSuiteException("Inizializzazione pratica", "Errore in apertura tipologia Richiesta")
        Else
            _currentToken = _documentTokens(0)
        End If

        txtProgrIncremental.Text = _currentToken.Incremental

        'Imposta dettaglio token
        uscDocumentToken.TextStep = _currentToken.FullStep
        uscDocumentToken.TextTokenType = _currentToken.DocumentTabToken.Id
        uscDocumentToken.TextTokenName = _currentToken.DocumentTabToken.Description
        uscDocumentToken.ControlDatiRichiesta.DateText = _currentToken.ExpiryDate
        uscDocumentToken.ControlDatiRichiesta.ObjectText = _currentToken.DocObject
        uscDocumentToken.ControlDatiRichiesta.ReasonText = _currentToken.Reason
        uscDocumentToken.ControlDatiRichiesta.NoteText = _currentToken.Note

        'Imposta ruoli mittente/destinatario
        If _currentToken.RoleDestination IsNot Nothing Then
            uscDocumentToken.ControlSettoreMittente.SourceRoles.Add(_currentToken.RoleSource)
            uscDocumentToken.ControlSettoreMittente.DataBind()
        End If
        If _currentToken.RoleSource IsNot Nothing Then
            uscDocumentToken.ControlSettoreDestinatario.SourceRoles.Add(_currentToken.RoleDestination)
            uscDocumentToken.ControlSettoreDestinatario.DataBind()
            uscDistribuzione.Role = _currentToken.RoleDestination
        End If

        'Imposta visualizzazione pannelli in base alla tipologia di token
        Select Case _currentToken.DocumentTabToken.Id
            Case "RC", "RR"
                pnlSettoriInAttesa.Visible = False
                pnlRestituzione.Visible = False
                pnlDistribuzione.Visible = True
                pnlRifiuto.Visible = False
                btnRifiuta.Visible = True
                uscDistribuzione.SetRole(uscDistribuzione.TreeViewControl.Nodes(0), _currentToken.RoleDestination)
                uscDistribuzione.TreeViewControl.Nodes(0).Nodes(0).Selected = True
                btnAccetta.Visible = True
            Case "RP"
                pnlSettoriInAttesa.Visible = SetSettoriInAttesa(_currentToken)
                pnlRestituzione.Visible = True
                txtReasonResponse.Text = _currentToken.ReasonResponse
                pnlDistribuzione.Visible = False
                pnlRifiuto.Visible = False
                btnRifiuta.Visible = False
                btnAccetta.Visible = True
        End Select

        If _currentToken.Response = "N" Then
            pnlRestituzione.Visible = False
            pnlDistribuzione.Visible = False
            pnlRifiuto.Visible = True
            btnRifiuta.Visible = False
            btnAccetta.Visible = False
            txtRefusal.Enabled = False
            btnAccetta.Visible = False
            txtRefusal.Text = _currentToken.ReasonResponse
        End If
    End Sub
#End Region

#Region "AjaxRequest"

    Private Sub DocmTokenPresaCarico_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 2)
        If (arguments(0) = Me.ClientID) Then
            ConfermaRifiuto(arguments(1))
        End If
    End Sub
#End Region

#Region "Accetta Button Event"
    Private Sub btnAccetta_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAccetta.Click
        Dim _documentToken As DocumentToken

        _documentToken = Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, txtProgrIncremental.Text)
        If _documentToken Is Nothing Then
            AjaxAlert("Errore in ricerca Richiesta")
            Exit Sub
        End If
        Select Case _documentToken.DocumentTabToken.Id
            Case "RC", "RR"
                ConfermaRCRR(_documentToken)
            Case "RP"
                ConfermaRP(_documentToken)
        End Select
    End Sub

#Region "Conferma Token RP"
    Private Sub ConfermaRP(ByVal documentToken As DocumentToken)

        Facade.DocumentTokenFacade.UpdateIsActive(documentToken, 0)

        Dim source As New List(Of Role) : source.Add(documentToken.RoleSource)
        Dim destination As New List(Of Role) : destination.Add(documentToken.RoleDestination)

        Dim newDocumentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
        With newDocumentToken
            .IncrementalOrigin = 0
            .IsActive = If(pnlSettoriInAttesa.Visible, 0S, 1S)
            .Response = String.Empty
            .DocStep = documentToken.DocStep
            .SubStep = documentToken.SubStep
            .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById("PR")
            .RoleSource = documentToken.RoleSource
            .RoleDestination = documentToken.RoleDestination
            .OperationDate = DateTime.Now
            .ExpiryDate = documentToken.ExpiryDate
            .DocObject = documentToken.DocObject
            .Reason = documentToken.Reason
            .ReasonResponse = documentToken.ReasonResponse
            .Note = documentToken.Note
        End With
        Facade.DocumentTokenFacade.Save(newDocumentToken)
        Try
            DocUtil.FncSendMail(source, destination, documentToken.DocumentTabToken, documentToken.Year, documentToken.Number, documentToken.ExpiryDate, documentToken.DocObject, documentToken.Reason, documentToken.Note, "", documentToken.ReasonResponse)
        Catch ex As DocSuiteException
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert(ex)
        End Try

        Me.RegisterFolderRefreshFullScript()
    End Sub

#End Region

#Region "Conferma Token RC - RR"
    Private Sub ConfermaRCRR(ByVal documentToken As DocumentToken)
        Dim aSource As New List(Of Role)
        Dim aDestination As New List(Of Role)
        Dim _isActive As Short = 0
        Dim _type As String = String.Empty
        Dim newDocumentToken As DocumentToken = Nothing
        Dim roleDestination As Role = Nothing

        'Role Master
        Dim roleMaster As Role = Facade.RoleFacade.GetById(uscDistribuzione.TreeViewControl.Nodes(0).Nodes(0).Value)

        'Role Destinazione
        Dim destNode As RadTreeNode = uscDistribuzione.TreeViewControl.SelectedNode
        If destNode Is Nothing OrElse destNode.Value = "" Then
            AjaxAlert("Selezionare un Settore valido per la Distribuzione")
            Exit Sub
        Else
            roleDestination = Facade.RoleFacade.GetById(destNode.Value)
        End If

        Dim Distribution As Boolean = Not roleMaster.Equals(roleDestination)
        If Distribution Then
            'TODO: verificare corretto funzionamento
            Facade.DocumentFolderFacade.InsertFoldersRole(CurrentDocumentYear, CurrentDocumentNumber, roleDestination.FullIncrementalPath, Nothing)
        End If

        Dim originToken As DocumentToken = Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, documentToken.IncrementalOrigin)
        Facade.DocumentTokenFacade.UpdateIsActive(originToken, 0)
        Facade.DocumentTokenFacade.UpdateIsActive(documentToken, 0)

        Dim rDestination As Role = Nothing
        Select Case Distribution
            Case False
                aSource.Add(documentToken.RoleSource)
                aDestination.Add(documentToken.RoleDestination)
                rDestination = documentToken.RoleDestination
                Select Case documentToken.DocumentTabToken.Id
                    Case "RC" : _type = "PC" : _isActive = 1
                    Case "RR" : _type = "PT" : _isActive = 2
                End Select
                newDocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
                SaveDocumentToken(newDocumentToken, documentToken, _isActive, documentToken.RoleSource, documentToken.RoleDestination, _type)

            Case True
                aSource.Add(documentToken.RoleSource)
                aDestination.Add(roleDestination)
                newDocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
                SaveDocumentToken(newDocumentToken, documentToken, 0, roleMaster, roleDestination, "MP")

                rDestination = roleDestination
                Select Case documentToken.DocumentTabToken.Id
                    Case "RC" : _type = "PC" : _isActive = 1
                    Case "RR" : _type = "PT" : _isActive = 2
                End Select
                newDocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
                SaveDocumentToken(newDocumentToken, documentToken, _isActive, documentToken.RoleSource, roleDestination, _type)
        End Select

        'verifica/inserimento assegnazione
        For Each node As RadTreeNode In uscDistribuzione.TreeViewControl.SelectedNode.Nodes
            If node.Attributes(uscSettoriUtente.ATTR_USERNODE) = uscSettoriUtente.TRUE_VALUE Then
                Dim userNode As UserNode = uscDistribuzione.GetUserNodeFromNode(node)
                Dim newDocumentTokenUser As DocumentTokenUser = Facade.DocumentTokenUserFacade.CreateDocumentTokenUser(CurrentDocumentYear, CurrentDocumentNumber)
                With newDocumentTokenUser
                    .DocStep = documentToken.DocStep
                    .SubStep = documentToken.SubStep
                    .Role = rDestination
                    .UserRole = userNode.IdParent
                    .UserName = userNode.Text
                    .Account = userNode.Id
                    .Note = String.Empty
                End With
                Facade.DocumentTokenUserFacade.Save(newDocumentTokenUser)
            End If
        Next

        'verifica/annullamento settori in CC
        Dim _documentTokenList As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetDocumentTokenByTokenType(CurrentDocumentYear, CurrentDocumentNumber, New String() {"CC"}, New String() {rDestination.Id}, True, False)
        If _documentTokenList.Count > 0 Then
            For Each token As DocumentToken In _documentTokenList
                Facade.DocumentTokenFacade.UpdateResponse(token, "A")
            Next
        End If
        If documentToken.Response <> "N" Then
            Try
                DocUtil.FncSendMail(aSource, aDestination, documentToken.DocumentTabToken, documentToken.Year, documentToken.Number, documentToken.ExpiryDate, documentToken.DocObject, documentToken.Reason, documentToken.Note, "N", txtRefusal.Text)
            Catch ex As DocSuiteException
                FileLogger.Warn(LoggerName, ex.Message, ex)
                AjaxAlert(ex)
            End Try
        End If

        Me.RegisterFolderRefreshFullScript()
    End Sub
#End Region

#End Region

#Region "Rifiuta"
    Private Sub ConfermaRifiuto(ByVal refuseReason As String)
        Dim _currentToken As DocumentToken
        Dim _originToken As DocumentToken

        _currentToken = Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, txtProgrIncremental.Text)
        If _currentToken Is Nothing Then
            AjaxAlert("Errore in ricerca Richiesta")
            Exit Sub
        End If

        'Campo Motivo del rifiuto obbligatorio
        If refuseReason = String.Empty Then
            AjaxAlert("Campo Motivo del Rifiuto Obbligatorio")
            Exit Sub
        End If

        'token di origine non attivo
        _originToken = Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, _currentToken.IncrementalOrigin)
        Facade.DocumentTokenFacade.UpdateIsActive(_originToken, 0)
        'token corrente non attivo
        Facade.DocumentTokenFacade.UpdateIsActive(_currentToken, 0)

        Dim _roleSourceList As New List(Of Role) : _roleSourceList.Add(_currentToken.RoleSource)
        Dim _roleDestinationList As New List(Of Role) : _roleDestinationList.Add(_currentToken.RoleDestination)
        Dim _tokenType As String = String.Empty
        Select Case _currentToken.DocumentTabToken.Id
            Case "RC" : _tokenType = "RC"
            Case "RR" : _tokenType = "RP"
        End Select
        Dim newDocumentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
        With newDocumentToken
            .IncrementalOrigin = _currentToken.IncrementalOrigin
            .IsActive = 1
            .Response = "N"
            .DocStep = _currentToken.DocStep + 1
            .SubStep = _currentToken.SubStep
            .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById(_tokenType)
            .RoleDestination = _currentToken.RoleSource
            .RoleSource = _currentToken.RoleDestination
            .OperationDate = DateTime.Now
            .ExpiryDate = _currentToken.ExpiryDate
            .DocObject = _currentToken.DocObject
            .Reason = _currentToken.Reason
            .ReasonResponse = refuseReason
            .Note = _currentToken.Note
        End With
        Facade.DocumentTokenFacade.Save(newDocumentToken)
        Try
            DocUtil.FncSendMail(_roleSourceList, _roleDestinationList, _currentToken.DocumentTabToken, _currentToken.Year, _currentToken.Number, _currentToken.ExpiryDate, _currentToken.DocObject, _currentToken.Reason, _currentToken.Note, "N", txtRefusal.Text)
        Catch ex As DocSuiteException
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert(ex)
        End Try

        Me.RegisterFolderRefreshFullScript()
    End Sub
#End Region

#Region "Private Function"
    Private Sub SaveDocumentToken(ByRef newToken As DocumentToken, ByVal currentToken As DocumentToken, ByVal isActive As Short, ByVal RoleSource As Role, ByVal RoleDestination As Role, ByVal Type As String)
        With newToken
            .IncrementalOrigin = 0
            .IsActive = isActive
            .Response = String.Empty
            .DocStep = currentToken.DocStep
            .SubStep = currentToken.SubStep
            .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById(Type)
            .RoleSource = RoleSource
            .RoleDestination = RoleDestination
            .OperationDate = DateTime.Now
            .ExpiryDate = currentToken.ExpiryDate
            .DocObject = currentToken.DocObject
            .Reason = currentToken.Reason
            .ReasonResponse = currentToken.ReasonResponse
            .Note = currentToken.Note
        End With
        Facade.DocumentTokenFacade.Save(newToken)
    End Sub

    Private Function SetSettoriInAttesa(ByVal currentToken As DocumentToken) As Boolean
        Dim _documentTokenSuspendList As IList(Of DocumentToken) = Nothing
        Dim _documentToken As DocumentToken = Nothing
        Dim node As RadTreeNode = Nothing

        'RR,PT
        _documentTokenSuspendList = Facade.DocumentTokenFacade.GetTokenSuspend(CurrentDocumentYear, CurrentDocumentNumber, currentToken.DocStep, New String() {"RR", "PT"})
        If _documentTokenSuspendList.Count > 0 Then
            For Each _documentToken In _documentTokenSuspendList
                node = New RadTreeNode()
                node.Text = _documentToken.FullStep & " " & _documentToken.RoleDestination.Name
                node.ImageUrl = "images/RoleOnP.gif"
                radTreeSettoriInAttesa.Nodes(0).Nodes.Add(node)
            Next
        End If

        'RP
        _documentTokenSuspendList = Facade.DocumentTokenFacade.GetTokenSuspend(CurrentDocumentYear, CurrentDocumentNumber, currentToken.DocStep, New String() {"RP"})
        If _documentTokenSuspendList.Count > 0 Then
            For Each _documentToken In _documentTokenSuspendList
                If _documentToken.Incremental <> currentToken.Incremental Then
                    node = New RadTreeNode()
                    node.Text = _documentToken.FullStep & " " & _documentToken.RoleSource.Name
                    node.ImageUrl = "images/RoleOnR.gif"
                    radTreeSettoriInAttesa.Nodes(0).Nodes.Add(node)
                End If
            Next
        End If

        Return (radTreeSettoriInAttesa.Nodes(0).Nodes.Count > 0)
    End Function
#End Region


End Class
