Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging

Public Class DocmTokenRestituzione
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
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializePage()
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializePage()
        'Pannello Restituzione non visibile
        uscDocumentToken.RestituzioneVisible = False
        'Mittente in sola lettura
        uscDocumentToken.ControlSettoreMittente.ReadOnly = True
        'Destinatario in sola lettura
        uscDocumentToken.ControlSettoreDestinatario.ReadOnly = True
        'Settori Copia Conoscenza non visibile
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
        Dim _documentVersioning As DocumentVersioning

        _documentTokens = Facade.DocumentTokenFacade.GetTokenOperationDateList(CurrentDocumentYear, CurrentDocumentNumber, New String() {"PT"}, RoleRightsWList)
        If _documentTokens.Count = 0 Then
            Throw New DocSuiteException("Inizializzazione pratica", "Errore in apertura tipologia Richiesta")
        End If
        _currentToken = _documentTokens(0)

        txtProgrIncremental.Text = _currentToken.Incremental

        'Imposta dettaglio token
        uscDocumentToken.TextStep = _currentToken.FullStep

        uscDocumentToken.TextTokenType = _currentToken.DocumentTabToken.Id
        uscDocumentToken.TextTokenName = _currentToken.DocumentTabToken.Description

        uscDocumentToken.ControlDatiRichiesta.DateText = _currentToken.ExpiryDate
        uscDocumentToken.ControlDatiRichiesta.ObjectText = _currentToken.DocObject
        uscDocumentToken.ControlDatiRichiesta.ReasonText = _currentToken.Reason
        uscDocumentToken.ControlDatiRichiesta.NoteText = _currentToken.Note

        'inverte mittente/destinatario
        If _currentToken.RoleDestination IsNot Nothing Then
            uscDocumentToken.ControlSettoreMittente.SourceRoles.Add(_currentToken.RoleDestination)
            uscDocumentToken.ControlSettoreMittente.DataBind()
        End If
        If _currentToken.RoleSource IsNot Nothing Then
            uscDocumentToken.ControlSettoreDestinatario.SourceRoles.Add(_currentToken.RoleSource)
            uscDocumentToken.ControlSettoreDestinatario.DataBind()
        End If

        'verifica documenti in check out
        _documentVersioning = Facade.DocumentVersioningFacade.GetDocumentVersion(CurrentDocumentYear, CurrentDocumentNumber, 0, "O")
        If _documentVersioning IsNot Nothing Then
            AjaxAlert("Ci sono Documenti in Check Out.\n\nNon è possibile eseguire una richiesta di Restituzione.")
            btnRestituzione.Enabled = False
            Exit Sub
        End If
    End Sub
#End Region

#Region "Restituzione Button Event"
    Private Sub btnRestituzione_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRestituzione.Click
        Dim _currentDocumentToken As DocumentToken

        _currentDocumentToken = Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, txtProgrIncremental.Text)

        If _currentDocumentToken Is Nothing Then
            AjaxAlert("Errore in ricerca Richiesta")
            Exit Sub
        End If

        Facade.DocumentTokenFacade.UpdateIsActive(_currentDocumentToken, 0)

        Dim newDocumentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
        With newDocumentToken
            .IncrementalOrigin = _currentDocumentToken.Incremental
            .IsActive = 1
            .DocStep = _currentDocumentToken.DocStep + 1S
            .SubStep = _currentDocumentToken.SubStep
            .Response = String.Empty
            .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById("RP")
            .RoleSource = _currentDocumentToken.RoleDestination
            .RoleDestination = _currentDocumentToken.RoleSource
            .OperationDate = DateTime.Now
            .ExpiryDate = _currentDocumentToken.ExpiryDate
            .DocObject = _currentDocumentToken.DocObject
            .Reason = _currentDocumentToken.Reason
            .Note = _currentDocumentToken.Note
            .ReasonResponse = txtResponse.Text
        End With
        Facade.DocumentTokenFacade.Save(newDocumentToken)

        Dim roleSourceList As New List(Of Role)
        roleSourceList.Add(_currentDocumentToken.RoleSource)
        Dim roleDestinationList As New List(Of Role)
        roleDestinationList.Add(_currentDocumentToken.RoleDestination)

        Try
            DocUtil.FncSendMail(roleSourceList, roleDestinationList, _currentDocumentToken.DocumentTabToken, _currentDocumentToken.Year, _currentDocumentToken.Number, _currentDocumentToken.ExpiryDate, _currentDocumentToken.DocObject, _currentDocumentToken.Reason, _currentDocumentToken.Note, "", txtResponse.Text)
        Catch ex As DocSuiteException
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert(ex)
        End Try

        RegisterFolderRefreshFullScript()
    End Sub
#End Region

End Class
