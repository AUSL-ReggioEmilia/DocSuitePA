Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class uscResolutionOC
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _currentResolutionRight As ResolutionRights
    Private _btnDocSupervisoryBoard As Button
    Private _btnDocRegion As Button

#End Region

#Region " Properties "

    Public Property CurrentResolution As Resolution

    Public ReadOnly Property CurrentResolutionRight As ResolutionRights
        Get
            If _currentResolutionRight Is Nothing AndAlso CurrentResolution IsNot Nothing Then
                _currentResolutionRight = New ResolutionRights(CurrentResolution)
            End If
            Return _currentResolutionRight
        End Get
    End Property


    Public Property VisibleOCCollegioSindacaleRegione() As Boolean
        Get
            Return trOCCollegioSindacaleRegione.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCCollegioSindacaleRegione.Visible = value
        End Set
    End Property

    Public Property VisibleOCControlloGestioneCorteConti() As Boolean
        Get
            Return trOCControlloGestioneCorteConti.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCControlloGestioneCorteConti.Visible = value
        End Set
    End Property

    Public Property VisibleOCAltro() As Boolean
        Get
            Return trOCAltro.Visible
        End Get
        Set(ByVal value As Boolean)
            trOCAltro.Visible = value
        End Set
    End Property

    Public Property VisibleOCChkAltro() As Boolean
        Get
            Return chkOther.Visible
        End Get
        Set(ByVal value As Boolean)
            chkOther.Visible = value
        End Set
    End Property

    Public Property VisibleOCChkConfSindaci() As Boolean
        Get
            Return chkConfSindaci.Visible
        End Get
        Set(ByVal value As Boolean)
            chkConfSindaci.Visible = value
        End Set
    End Property

    Public Property VisibleOCRilievoCollegioSindacale() As Boolean
        Get
            Return trSupervisoryBoardOpinion.Visible
        End Get
        Set(ByVal value As Boolean)
            trSupervisoryBoardOpinion.Visible = value
        End Set
    End Property

    Public Property VisibleWaitDate() As Boolean
        Get
            Return tdWaitDateLabel.Visible And tdWaitDateContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdWaitDateLabel.Visible = value
            tdWaitDateContent.Visible = value
        End Set
    End Property

    Public Property VisibleDGR() As Boolean
        Get
            Return tdDGRLabel.Visible And tdDGRContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdDGRLabel.Visible = value
            tdDGRContent.Visible = value
        End Set
    End Property

    Public Property VisibleApprovalNote() As Boolean
        Get
            Return tdApprovalNoteLabel.Visible And tdApprovalNoteContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdApprovalNoteLabel.Visible = value
            tdApprovalNoteContent.Visible = value
        End Set
    End Property

    Public Property VisibleDeclineNote() As Boolean
        Get
            Return tdDeclineNoteLabel.Visible And tdDeclineNoteContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdDeclineNoteLabel.Visible = value
            tdDeclineNoteContent.Visible = value
        End Set
    End Property

    Public Property VisibleConfirmDate() As Boolean
        Get
            Return tdConfirmDateLabel.Visible And tdConfirmDateContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdConfirmDateLabel.Visible = value
            tdConfirmDateContent.Visible = value
        End Set
    End Property

    Public Property VisibleCommentoRegione() As Boolean
        Get
            Return tdCommentoLabel.Visible And tdCommentoContent.Visible
        End Get
        Set(ByVal value As Boolean)
            tdCommentoLabel.Visible = value
            tdCommentoContent.Visible = value
        End Set
    End Property

    Public Property VisibleRegionTitolo() As Boolean
        Get
            Return trRegionTitolo.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionTitolo.Visible = value
        End Set
    End Property

    Public Property VisibleRegionSpedizione() As Boolean
        Get
            Return trRegionSpedizione.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionSpedizione.Visible = value
        End Set
    End Property

    Public Property VisibleRegionRicezioneScadenza() As Boolean
        Get
            Return trRegionRicezioneScadenza.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionRicezioneScadenza.Visible = value
        End Set
    End Property

    Public Property VisibleRegionDGRProtocollo() As Boolean
        Get
            Return trRegionDGRProtocollo.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionDGRProtocollo.Visible = value
        End Set
    End Property

    Public Property VisibleRegionInvioChiarimenti() As Boolean
        Get
            Return trRegionInvioChiarimenti.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionInvioChiarimenti.Visible = value
        End Set
    End Property

    Public Property VisibleRegionCommento() As Boolean
        Get
            Return trRegionCommento.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionCommento.Visible = value
        End Set
    End Property

    Public Property VisibleRegionNote() As Boolean
        Get
            Return trRegionNote.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionNote.Visible = value
        End Set
    End Property

    Public Property VisibleRegionApprovazione() As Boolean
        Get
            Return trRegionApprovazione.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionApprovazione.Visible = value
        End Set
    End Property

    Public Property VisibleRegionDecadimento() As Boolean
        Get
            Return trRegionDecadimento.Visible
        End Get
        Set(ByVal value As Boolean)
            trRegionDecadimento.Visible = value
        End Set
    End Property

    Public Property VisibleResponseProtocolLink() As Boolean
        Get
            Return tdResponseProtocolLink.Visible
        End Get
        Set(ByVal value As Boolean)
            tdResponseProtocolLink.Visible = value
        End Set
    End Property

    Public Property AltroLabel() As String
        Get
            Return tdAltro.InnerHtml
        End Get
        Set(ByVal value As String)
            tdAltro.InnerHtml = value
        End Set
    End Property

    Public Property RispostaDefinitivaRegione() As String
        Get
            Return tdControllerOpinionLabel.InnerText
        End Get
        Set(ByVal value As String)
            tdControllerOpinionLabel.InnerText = value
        End Set
    End Property

    Public Property DataRispostaRegione() As String
        Get
            Return tdResponseLabel.InnerText
        End Get
        Set(ByVal value As String)
            tdResponseLabel.InnerText = value
        End Set
    End Property

    Public Property ButtonSupervisoryBoardFile() As Button
        Get
            Return _btnDocSupervisoryBoard
        End Get
        Set(ByVal value As Button)
            _btnDocSupervisoryBoard = value
        End Set
    End Property

    Public Property ButtonRegionFile() As Button
        Get
            Return _btnDocRegion
        End Get
        Set(ByVal value As Button)
            _btnDocRegion = value
        End Set
    End Property

#End Region

#Region " Methods "

    ''' <summary>  </summary>
    ''' <remarks> Versione riveduta e corretta del vecchio metodo </remarks>
    Public Sub InitializeOCSupervisoryBoard()
        tblOCSupervisoryBoard.Visible = True
        If Not String.IsNullOrEmpty(CurrentResolution.SupervisoryBoardProtocolLink) Then
            AttachOpenLinkEvent(imgSupervisoryBoardLink, CurrentResolution.SupervisoryBoardProtocolLink)
        ElseIf CurrentResolution.SupervisoryBoardProtocolCollaboration.HasValue Then
            AttachOpenCollaborationLinkEvent(imgSupervisoryBoardLink, CurrentResolution.SupervisoryBoardProtocolCollaboration.Value)
        End If
        ButtonSupervisoryBoardFile.Text = "Risposta Collegio"
        ButtonSupervisoryBoardFile.Attributes.Clear()

        If CurrentResolution.File IsNot Nothing AndAlso CurrentResolution.File.IdSupervisoryBoardFile.HasValue AndAlso CurrentResolutionRight.IsViewable Then
            ' Verifico sia presente il documento e abbia i permessi di visualizzazione
            Dim viewerScript As String = getFileResolutionViewerScript("IdSupervisoryBoardFile", "Risposta Organo Controllo")
            lblSupervisoryBoardFile.Text = "Documento:"
            imgSupervisoryBoardFile.Visible = True
            imgSupervisoryBoardFile.Attributes.Add("onclick", viewerScript)
            ButtonSupervisoryBoardFile.Visible = True
            ButtonSupervisoryBoardFile.Attributes.Add("onclick", viewerScript)
        End If
    End Sub

    ''' <summary>  </summary>
    ''' <remarks> Versione riveduta e corretta del vecchio metodo </remarks>
    Public Sub InitializeOCRegion()
        tblOCRegion.Visible = True
        AttachOpenLinkEvent(imgRegionProtocolLink, CurrentResolution.RegionProtocolLink)

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            AttachOpenLinkEvent(imgRegionResponseProtocolLink, CurrentResolution.ResponseProtocol)
            AttachOpenLinkEvent(imgRegionInvioCommentoRegioneLink, CurrentResolution.ConfirmProtocol)
        Else
            ButtonRegionFile.Text = "Risposta Regione"
            ButtonRegionFile.Attributes.Clear()

            If CurrentResolution.File IsNot Nothing AndAlso CurrentResolution.File.IdControllerFile.HasValue AndAlso CurrentResolutionRight.IsViewable Then
                ' Verifico sia presente il documento e abbia i permessi di visualizzazione
                Dim viewerScript As String = getFileResolutionViewerScript("idControllerFile", "Risposta Organo Controllo")
                lblRegionFile.Text = "Documento:"
                imgRegionFile.Visible = True
                imgRegionFile.Attributes.Add("onclick", viewerScript)
                ButtonRegionFile.Visible = True
                ButtonRegionFile.Attributes.Add("onclick", viewerScript)
            End If
        End If
    End Sub

    Private Function getFileResolutionViewerScript(fieldName As String, description As String) As String
        Dim queryString As String = String.Format("idResolution={0}&field={1}&description={2}", CurrentResolution.Id, fieldName, Server.UrlEncode(description))
        Dim hrefValue As String = String.Format("{0}/viewers/FileResolutionViewer.aspx?", DocSuiteContext.Current.CurrentTenant.DSWUrl) & CommonShared.AppendSecurityCheck(queryString)
        Return String.Format("window.location.href='{0}'; return false;", hrefValue)
    End Function

    Public Sub LoadOCList()
        tblODC.Visible = True
        chkSupervisoryBoard.Checked = CurrentResolution.OCSupervisoryBoard.GetValueOrDefault(False)
        chkRegion.Checked = CurrentResolution.OCRegion.GetValueOrDefault(False)
        chkManagement.Checked = CurrentResolution.OCManagement.GetValueOrDefault(False)
        chkCorteConti.Checked = CurrentResolution.OCCorteConti.GetValueOrDefault(False)
        chkOther.Checked = CurrentResolution.OCOther.GetValueOrDefault(False)
        chkConfSindaci.Checked = CurrentResolution.OCManagement.GetValueOrDefault(False)
    End Sub

    Public Sub LoadOCConfSindaci()
        tblOCConfSindaci.Style.Remove("display")
        tblOCConfSindaci.Visible = True
        AttachOpenLinkEvent(imgConfSindaciProtocolLink, CurrentResolution.ManagementProtocolLink)
    End Sub

    Public Sub LoadOCCorteDeiConti()
        tblOCCorteDeiConti.Visible = True
        AttachOpenLinkEvent(imgCorteDeiContiProtocolLink, CurrentResolution.CorteDeiContiProtocolLink)
    End Sub

    Public Sub LoadOCManagement()
        tblOCManagement.Visible = True
        AttachOpenLinkEvent(imgManagmentProtocolLink, CurrentResolution.ManagementProtocolLink)
    End Sub

    Public Sub LoadOCOther()
        tblOCOther.Visible = True
    End Sub

    Public Sub UnLoadCS()
        chkSupervisoryBoard.Visible = False
        tdCollegioSindacale.Visible = False
    End Sub

    Public Sub UnloadCONFSIND()
        chkConfSindaci.Visible = False
        tdAltro.Visible = False
    End Sub

    Public Sub UnloadREG()
        chkRegion.Visible = False
        tdRegione.Visible = False
    End Sub

    Private Sub AttachOpenLinkEvent(ByRef imgButton As ImageButton, ByVal link As String)
        If String.IsNullOrEmpty(link) Then
            Exit Sub
        End If

        Dim url As String = "../Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Year={0}&Number={1}", Resolution.FormatProtocolLink(link, "Y"), Resolution.FormatProtocolLink(link, "N")))
        imgButton.Attributes.Add("onclick", "document.location='" & url & "'; return false;")
        imgButton.Visible = True
    End Sub

    Private Sub AttachOpenCollaborationLinkEvent(ByRef imgButton As ImageButton, ByVal idCollaboration As Integer)
        Dim cmd As String = String.Format("document.location='../User/UserCollGestione.aspx?Type=Prot&Titolo=Visualizzazione&Action={0}&idCollaboration={1}&Action2={2}&Title2=Visualizzazione", CollaborationSubAction.ProtocollatiGestiti, idCollaboration, CollaborationMainAction.ProtocollatiGestiti)
        imgButton.Attributes.Add("onclick", cmd & "'; return false;")
        imgButton.AlternateText = "Collegamento alla collaborazione"
        imgButton.Visible = True
    End Sub

    Public Function GetControllerDescripton() As String
        If CurrentResolution.ControllerStatus IsNot Nothing Then
            Return CurrentResolution.ControllerStatus.Description
        End If
        Return String.Empty
    End Function

#End Region

End Class