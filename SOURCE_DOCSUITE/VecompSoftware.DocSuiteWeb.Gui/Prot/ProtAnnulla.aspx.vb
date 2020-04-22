Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ProtAnnulla
    Inherits ProtBasePage

#Region " Properties "

    Public Property Dirty() As Boolean
        Get
            Return Me.ViewState.Item("Dirty")
        End Get
        Set(ByVal Value As Boolean)
            Me.ViewState.Item("Dirty") = Value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        InitializeTab()

        If Not IsPostBack Then
            Dirty = False
            Initialize()
        End If
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, UscDocumentUpload1)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, UscDocumentUpload2)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        If Not CurrentProtocol Is Nothing Then
            CancelProtocolDocuments()
            CancelProtocol()
        End If
        Response.Redirect($"~/Prot/ProtVisualizza.aspx?{CommonShared.AppendSecurityCheck($"Year={CurrentProtocolYear}&Number={CurrentProtocolNumber}&StatusCancel=on")}")
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        'documenti
        If CurrentProtocol.IdDocument.HasValue AndAlso CurrentProtocol.IdDocument.Value <> 0 Then
            Try
                UscDocumentUpload1.LoadBiblosDocuments(CurrentProtocol.Location.DocumentServer, CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdDocument.Value, False, True)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento protocollo", "Errore in lettura catena", ex)
            End Try
        End If
        'allegati
        If CurrentProtocol.IdAttachments.HasValue AndAlso CurrentProtocol.IdAttachments.Value <> 0 Then
            Try
                Dim loc As UIDLocation = Facade.ProtocolFacade.GetAttachmentLocation(CurrentProtocol)
                UscDocumentUpload2.LoadBiblosDocuments(loc.Server, loc.Archive, CurrentProtocol.IdAttachments.Value, True, True)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento protocollo", "Errore in lettura catena", ex)
            End Try
        End If
    End Sub

    Private Sub InitializeTab()

        If Not CurrentProtocolRights.IsCancelable Then
            Throw New DocSuiteException($"Protocollo n. {CurrentProtocol.FullNumber}", "Mancano diritti di Annullamento.")
        End If

        uscProtocollo.CurrentProtocol = CurrentProtocol
        uscProtocollo.VisibleProtocollo = True
        uscProtocollo.VisibleMittentiDestinatari = ProtocolEnv.IsInteropEnabled
        uscProtocollo.VisibleOggetto = True
        uscProtocollo.VisibleProtocolloMittente = (CurrentProtocol.Type.Id <> 1)
        uscProtocollo.VisibleAltri = False
        uscProtocollo.VisibleClassificazione = False
        uscProtocollo.VisibleFascicolo = False
        uscProtocollo.VisibleStatoProtocollo = False
        uscProtocollo.VisibleTipoDocumento = False
        uscProtocollo.VisibleFatturazione = False
        uscProtocollo.VisibleStatoProtocollo = False
        uscProtocollo.VisibleAssegnatario = False
        uscProtocollo.VisibleScatolone = False

        chkDisableUnlinkPec.Enabled = ProtocolEnv.PecUnboundMode = 0 OrElse ProtocolEnv.PecUnboundMode = 2
        If ProtocolEnv.PecUnboundMode > 0 Then
            chkDisableUnlinkPec.Checked = True
        End If
    End Sub

    Private Sub CancelProtocolDocuments()
        If CurrentProtocol.IdDocument.HasValue AndAlso CurrentProtocol.IdDocument <> 0 Then
            Try
                Service.DetachDocument(CurrentProtocol.Location.DocumentServer, CurrentProtocol.Location.ProtBiblosDSDB, CurrentProtocol.IdDocument.Value)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento protocollo", "Errore in fase Detach del documento, impossibile eseguire.", ex)
            End Try
        End If

        If CurrentProtocol.IdAttachments.HasValue AndAlso CurrentProtocol.IdAttachments <> 0 Then
            Try
                Dim attachmentLocation As UIDLocation = Facade.ProtocolFacade.GetAttachmentLocation(CurrentProtocol)
                Service.DetachDocument(attachmentLocation.Server, attachmentLocation.Archive, CurrentProtocol.IdAttachments.Value)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento protocollo", "Errore in fase Detach degli allegati, impossibile eseguire.", ex)
            End Try
        End If

        If CurrentProtocol.IdAnnexed <> Guid.Empty Then
            Try
                Service.DetachDocument(CurrentProtocol.Location.DocumentServer, CurrentProtocol.IdAnnexed)
            Catch ex As Exception
                Throw New DocSuiteException("Annullamento protocollo", "Errore in fase Detach degli annessi, impossibile eseguire.", ex)
            End Try
        End If
    End Sub

    Private Sub CancelProtocol()
        Dim protFacade As New ProtocolFacade()
        With CurrentProtocol
            .IdStatus = ProtocolStatusId.Annullato
            .LastChangedReason = txtAnnulla.Text
        End With

        protFacade.Update(CurrentProtocol)
        Facade.ProtocolFacade.RaiseAfterCancel(CurrentProtocol)

        If ProtocolEnv.IsLogEnabled Then
            Facade.ProtocolLogFacade.Insert(CurrentProtocol, ProtocolLogEvent.PA, txtAnnulla.Text)
        End If

        'Gestione PEC
        If chkDisableUnlinkPec.Checked Then
            Facade.ProtocolFacade.PecUnlink(CurrentProtocol)
        End If

        Facade.ProtocolFacade.SendUpdateProtocolCommand(CurrentProtocol)

    End Sub

#End Region


End Class