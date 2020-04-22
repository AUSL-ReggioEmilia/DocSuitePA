Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging

Partial Public Class DocmChiusuraApertura
    Inherits DocmBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        pnlChiusura.Visible = False
        Select Case Action
            Case "Close"
                pnlChiusura.Visible = True
                pnlRiapertura.Visible = False
                lblAzione.Text = "Dati Chiusura Pratica"
                Me.Title = "Chiusura Pratica"
                AjaxManager.AjaxSettings.AddAjaxSetting(btnConfermaChiusura, btnConfermaChiusura)
            Case "ReOpen"
                pnlChiusura.Visible = False
                pnlRiapertura.Visible = True
                lblAzione.Text = "Dati Riapertura Pratica"
                Me.Title = "Riapertura Pratica"
                AjaxManager.AjaxSettings.AddAjaxSetting(btnConfermaRiapertura, btnConfermaRiapertura)
        End Select

        InitializeDocument()
        If Not Me.IsPostBack Then
            Initialize()
        End If

    End Sub

    Private Sub btnConfermaChiusura_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfermaChiusura.Click
        Dim documenttokens As IList(Of DocumentToken)

        documenttokens = Facade.DocumentTokenFacade.GetDocumentTokenRoleP(CurrentDocumentYear, CurrentDocumentNumber)
        If documenttokens.Count <> 1 Then
            AjaxAlert("Errore in ricerca movimento di Presa in Carico")
            Exit Sub
        End If

        Try
            Facade.DocumentFacade.Close(CurrentDocument, RadDatePicker1.SelectedDate, txtNote.Text)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in Chiusura della Pratica: " & ex.Message, ex)
            AjaxAlert("Errore in Chiusura della Pratica: " & ex.Message)
        End Try

        'Refresh sommario
        Me.RegisterFolderRefreshFullScript()
    End Sub

    Private Sub btnConfermaRiapertura_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfermaRiapertura.Click
        Dim documenttokens As IList(Of DocumentToken)
        documenttokens = Facade.DocumentTokenFacade.GetDocumentTokenRoleP(CurrentDocumentYear, CurrentDocumentNumber)
        If documenttokens.Count <> 1 Then
            AjaxAlert("Errore in ricerca movimento di Presa in Carico")
            Exit Sub
        End If

        Try
            Facade.DocumentFacade.ReOpen(CurrentDocument, RadDatePicker2.SelectedDate, txtNote.Text)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore in RiApertura della Pratica: " & ex.Message, ex)
            AjaxAlert("Errore in RiApertura della Pratica: " & ex.Message)
        End Try

        'Refresh sommario
        Me.RegisterFolderRefreshFullScript()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> associa la pratica al controllo di visualizzazione </summary>
    Private Sub InitializeDocument()
        'dati completi
        If CurrentDocument Is Nothing Then
            Throw New DocSuiteException("Pratica n. " & DocumentUtil.DocmFull(CurrentDocumentYear, CurrentDocumentNumber), "Pratica Inesistente")
        End If

        'inizializza tabella
        With UscDocument1
            .CurrentDocument = CurrentDocument
            .VisibleAltri = False
            .VisibleClassificatoreModifica = False
            .VisibleClassificazione = False
            .VisibleContatti = False
            .VisibleDateModifica = False
            .VisibleDati = False
            .VisibleDatiModifica = False
            .VisibleDate = False
            .VisibleDateSovrapposte = True
            .VisibleGenerale = True
            .VisiblePratica = True
            .Visible = True
        End With
        UscDocument1.LoadStatus()
    End Sub

    Private Sub Initialize()
        RadDatePicker1.SelectedDate = CurrentDocument.EndDate
        RadDatePicker2.SelectedDate = CurrentDocument.ReStartDate
        txtNote.Text = CurrentDocument.Note
    End Sub

#End Region

End Class