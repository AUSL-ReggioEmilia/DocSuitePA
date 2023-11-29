Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging

Public Class DocmTokenRichiamo
    Inherits DocmBasePage

#Region " Fields "

    Private _docTokenListRole As IList(Of DocumentToken)

#End Region

#Region " Properties "

    Private ReadOnly Property DocumentTokensRoleRList() As IList(Of DocumentToken)
        Get
            If _docTokenListRole Is Nothing Then
                _docTokenListRole = Facade.DocumentTokenFacade.GetDocumentTokenRoleR(CurrentDocumentYear, CurrentDocumentNumber)
            End If
            Return _docTokenListRole
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializePage()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim i As Integer
        Dim _step As Short
        Dim _originalTokenType As DocumentTabToken = Nothing
        Dim _roleSource As Role = Nothing
        Dim _roleSourceList As New List(Of Role)
        Dim _roleDestinationList As New List(Of Role)
        Dim _roleCCList As New List(Of Role)
        Dim documentToken As DocumentToken = Nothing

        If txtReasonResponse.Text = "" Then
            AjaxAlert("Campo Motivo obbligatorio")
            Exit Sub
        End If


        If DocumentTokensRoleRList.Count > 0 Then

            For Each documentToken In DocumentTokensRoleRList
                i += 1
                If i = 1 Then
                    Facade.DocumentTokenFacade.UpdateIsActive(Facade.DocumentTokenFacade.GetById(CurrentDocumentYear, CurrentDocumentNumber, documentToken.IncrementalOrigin), False)
                    _step = documentToken.DocStep
                    _originalTokenType = documentToken.DocumentTabToken
                    _roleSource = documentToken.RoleSource
                    _roleSourceList.Add(documentToken.RoleSource)
                End If
                Facade.DocumentTokenFacade.UpdateIsActive(documentToken, False)
                Facade.DocumentTokenFacade.UpdateResponse(documentToken, "A")
                RemoveToken(documentToken)
                _roleDestinationList.Add(documentToken.RoleDestination)
            Next

            Dim newDocumentToken As DocumentToken = Facade.DocumentTokenFacade.CreateDocumentToken(CurrentDocumentYear, CurrentDocumentNumber)
            With newDocumentToken
                .IncrementalOrigin = 0
                .Response = String.Empty
                .IsActive = True
                .DocStep = _step + 1S
                .SubStep = 0
                .DocumentTabToken = Facade.DocumentTabTokenFacade.GetById("PM")
                .RoleDestination = _roleSource
                .RoleSource = _roleSource
                .OperationDate = DateTime.Now
                .ReasonResponse = txtReasonResponse.Text
            End With
            Facade.DocumentTokenFacade.Save(newDocumentToken)

            'SettoriCC
            Dim _docTokenListRoleCC As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, False, _step)
            If _docTokenListRoleCC.Count > 0 Then
                For Each documentToken In _docTokenListRoleCC
                    Facade.DocumentTokenFacade.UpdateResponse(documentToken, "A")
                    _roleCCList.Add(documentToken.RoleDestination)
                Next

            End If

            'Invio Mail
            If _roleDestinationList.Count > 0 Then
                Try
                    DocUtil.FncSendMail(_roleSourceList, _roleDestinationList, documentToken.DocumentTabToken, CurrentDocumentYear, CurrentDocumentNumber, Nothing, "", "", "", "A", txtReasonResponse.Text)
                Catch ex As DocSuiteException
                    FileLogger.Warn(LoggerName, ex.Message, ex)
                    AjaxAlert(ex)
                End Try
            End If

            If _roleCCList.Count > 0 Then
                Try
                    DocUtil.FncSendMail(_roleSourceList, _roleCCList, Facade.DocumentTabTokenFacade.GetById("CC"), CurrentDocumentYear, CurrentDocumentNumber, Nothing, "", "", "", "A", txtReasonResponse.Text)
                Catch ex As DocSuiteException
                    FileLogger.Warn(LoggerName, ex.Message, ex)
                    AjaxAlert(ex)
                End Try
            End If
        Else
            AjaxAlert("Errore in ricerca Richiesta di Presa in carico")
        End If
        'script per l'aggiornamento del sommario
        RegisterFolderRefreshFullScript()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializePage()
        uscDocumentToken.ControlSettoreCC.ReadOnly = True
        uscDocumentToken.ControlSettoreDestinatario.ReadOnly = True
        uscDocumentToken.ControlSettoreMittente.ReadOnly = True
    End Sub

    Private Sub Initialize()
        Dim hasErrors As Boolean
        Dim i As Integer = 0
        Dim _step As Short = 0
        Dim _incrementalOrigin As Short = 0
        Dim _roleSource As Role = Nothing

        uscDocumentToken.PanelSettoriCCVisible = False

        If DocumentTokensRoleRList.Count > 0 Then
            For Each documentTokenRole As DocumentToken In DocumentTokensRoleRList
                i += 1
                If i = 1 Then
                    _step = documentTokenRole.DocStep
                    _incrementalOrigin = documentTokenRole.IncrementalOrigin
                    _roleSource = documentTokenRole.RoleSource
                    If _roleSource IsNot Nothing Then
                        uscDocumentToken.ControlSettoreMittente.SourceRoles.Add(_roleSource)
                    End If
                End If
                If documentTokenRole.RoleDestination IsNot Nothing Then
                    uscDocumentToken.ControlSettoreDestinatario.SourceRoles.Add(documentTokenRole.RoleDestination)
                End If

                If _step <> documentTokenRole.DocStep Then
                    hasErrors = True
                End If
                If _incrementalOrigin <> documentTokenRole.IncrementalOrigin Then
                    hasErrors = True
                End If
                If _roleSource.Id <> documentTokenRole.RoleSource.Id Then
                    hasErrors = True
                End If
            Next
        Else
            hasErrors = True
        End If

        If hasErrors Then
            pnlRichiamo.Visible = False
            AjaxAlert("Errore in ricerca Richiesta di Presa in carico")
        End If

        pnlRichiamo.Visible = True

        Dim finder As NHibernateDocumentTokenFinder = Facade.DocumentTokenFinder()
        Dim ids As IList(Of String) = New List(Of String)
        ids.Add("RC")
        ids.Add("RP")
        ids.Add("RR")

        finder.DocumentType = ids
        finder.DocumentYear = CurrentDocumentYear
        finder.DocumentNumber = CurrentDocumentNumber

        gvToken.Finder = finder
        gvToken.VirtualItemCount = DocumentTokensRoleRList.Count
        gvToken.DataBindFinder()

        'Bind settore mittente
        uscDocumentToken.ControlSettoreMittente.DataBind()
        'Bind settori destinatario
        uscDocumentToken.ControlSettoreDestinatario.DataBind()

        'SettoriCC
        Dim _docTokenListRoleCC As IList(Of DocumentToken) = Facade.DocumentTokenFacade.GetTokenListCC(CurrentDocumentYear, CurrentDocumentNumber, True, _step)
        If _docTokenListRoleCC.Count > 0 Then
            uscDocumentToken.PanelSettoriCCVisible = True
            For Each documentTokenRoleCC As DocumentToken In _docTokenListRoleCC
                uscDocumentToken.ControlSettoreCC.SourceRoles.Add(documentTokenRoleCC.RoleDestination)
            Next
            uscDocumentToken.ControlSettoreCC.DataBind()
        End If
    End Sub

    Private Sub RemoveToken(ByRef documentToken As DocumentToken)
        Dim _documentFolders As IList(Of DocumentFolder)
        If documentToken IsNot Nothing Then
            _documentFolders = Facade.DocumentFolderFacade.GetByRole(CurrentDocumentYear, CurrentDocumentNumber, documentToken.RoleDestination)
            If _documentFolders.Count = 1 Then
                RemoveFolder(_documentFolders(0))
            End If
        End If
    End Sub

    Private Sub RemoveFolder(ByRef documentFolder As DocumentFolder)
        Dim _documentFolderFather As DocumentFolder = Nothing

        If Facade.DocumentTokenFacade.GetTokenListForAllTokenTypes(CurrentDocumentYear, CurrentDocumentNumber, True, 0, documentFolder.Role).Count > 0 Then
            Exit Sub
        End If
        If documentFolder.HasChildren Or documentFolder.HasObjects Then
            Exit Sub
        End If

        If documentFolder.IncrementalFather.HasValue Then
            _documentFolderFather = Facade.DocumentFolderFacade.GetById(documentFolder.Year, documentFolder.Number, documentFolder.IncrementalFather.Value)
        End If

        'Rimuove document folder di livello più basso
        Facade.DocumentFolderFacade.Delete(documentFolder)

        If _documentFolderFather IsNot Nothing Then
            RemoveFolder(_documentFolderFather)
        End If
    End Sub
#End Region

End Class
