Imports System.Collections.Generic
Imports System.Text
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class uscPrivacyPanel
    Inherits DocSuite2008BaseControl

#Region " Properties "

    Private _currentResolution As Resolution = Nothing
    Public Property CurrentResolution() As Resolution
        Get
            Return _currentResolution
        End Get
        Set(ByVal value As Resolution)
            _currentResolution = value
        End Set
    End Property

    Private _resolutionType As ResolutionType = Nothing
    Public Property ResolutionType As ResolutionType
        Get
            Return If(_resolutionType IsNot Nothing, _resolutionType, CurrentResolution.Type)
        End Get
        Set(value As ResolutionType)
            _resolutionType = value
        End Set
    End Property

    Public WriteOnly Property ResolutionTypeId As Short
        Set(value As Short)
            ResolutionType = Facade.ResolutionTypeFacade.GetById(value)
        End Set
    End Property

    Public Property PrivacyTypeVisible As Boolean
        Get
            Return pnlPrivacyPublishDocumentSelection.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlPrivacyPublishDocumentSelection.Visible = value
        End Set
    End Property

    Public Property PrivacyDocumentVisible As Boolean
        Get
            Return pnlPrivacyPublishDocument.Visible
        End Get
        Set(ByVal value As Boolean)
            pnlPrivacyPublishDocument.Visible = value
        End Set
    End Property

    Public Property PrivacySelectorEnabled As Boolean
        Get
            Return rblSelectPrivacy.Enabled
        End Get
        Set(value As Boolean)
            rblSelectPrivacy.Enabled = value
        End Set
    End Property

    Public Property ValidatorEnabled As Boolean
        Get
            Return rfvSelectPrivacy.Enabled
        End Get
        Set(value As Boolean)
            rfvSelectPrivacy.Enabled = value
        End Set
    End Property

    Public Property GeneratedDocumentUrls As String
        Get
            Return privacyImageToPrint.NavigateUrl
        End Get
        Set(value As String)
            privacyImageToPrint.NavigateUrl = value
            privacyTextToPrint.NavigateUrl = value
        End Set
    End Property

    Private _useResolutionNumberDisplay As Boolean = True
    Public Property UseResolutionNumberDisplay As Boolean
        Get
            Return _useResolutionNumberDisplay
        End Get
        Set(value As Boolean)
            _useResolutionNumberDisplay = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        privacyImageToPrint.ImageUrl = ImagePath.SmallPdf
    End Sub

    Private Sub RblSelectPrivacySelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSelectPrivacy.SelectedIndexChanged
        'Se ho scelto di non attivare la privacy attivo direttamente il controllo
        If Not Boolean.Parse(rblSelectPrivacy.SelectedValue) Then
            CheckPrivacy()
        Else
            'Altrimenti eseguo prima la verifica dei dati
            AjaxManager.ResponseScripts.Add("ExecuteAjaxRequest('checkdatatoinsert');")
        End If
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(rblSelectPrivacy, rblSelectPrivacy)
        AjaxManager.AjaxSettings.AddAjaxSetting(rblSelectPrivacy, pnlPrivacyPublishDocument, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacyPublishDocumentSelection)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlPrivacyPublishDocument, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscPrivacyPublicationDocumentUploader)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, tblResolutionNumber)
    End Sub

    Public Sub CheckPrivacy()
        Try
            If rblSelectPrivacy.SelectedIndex > -1 AndAlso rblSelectPrivacy.SelectedItem IsNot Nothing Then
                If Boolean.Parse(rblSelectPrivacy.SelectedValue) Then
                    Dim msg As New StringBuilder()
                    msg.AppendFormat("Documento Privacy = Si")
                    msg.Append(Environment.NewLine)
                    msg.AppendFormat("Sarà assegnato il numero definitivo alla {0}.", Facade.ResolutionTypeFacade.GetDescription(ResolutionType))
                    msg.Append(Environment.NewLine)
                    msg.AppendFormat("Si dovrà poi procedere alla scansione del documento con omissis.")

                    BasePage.AjaxAlertConfirmAndDeny(msg.ToString(), "ExecuteAjaxRequest('blockinsert');", "ExecuteAjaxRequest('privacyreset');", String.Empty, True)
                Else
                    PrivacyDocumentVisible = False

                    Dim msg As New StringBuilder()
                    msg.AppendFormat("Documento Privacy = No")
                    msg.Append(Environment.NewLine)
                    msg.AppendFormat("Sarà pubblicato integralmente il Documento inserito.")
                    BasePage.AjaxAlertConfirmAndDeny(msg.ToString(), String.Empty, "ExecuteAjaxRequest('privacyreset');", String.Empty, True)
                End If
            End If
        Catch ex As Exception
            rblSelectPrivacy.SelectedIndex = -1
            BasePage.AjaxAlert(String.Format("Impossibile attivare il pannello Privacy.{0}{0}{1}", Environment.NewLine, ex.Message))
        End Try
    End Sub

    Public Sub SavePrivacyDocument(ByVal docTypeToSave As ResolutionFacade.DocType)
        If PrivacyDocuments.Count > 0 Then
            ''Salvo il documento in Biblos
            Dim fileDocumentInfo As DocumentInfo = PrivacyDocuments(0)
            fileDocumentInfo.Signature = Facade.ResolutionFacade.ResolutionSignature(CurrentResolution, ResolutionType.UploadDocumentType.Pubblicazione)
            fileDocumentInfo.Name = "Documento Privacy.pdf"

            Dim savedDocument As BiblosDocumentInfo = fileDocumentInfo.ArchiveInBiblos(CurrentResolution.Location.DocumentServer, CurrentResolution.Location.ReslBiblosDSDB)
            Dim idDocumentoPubblicazionePrivacy As Integer = savedDocument.BiblosChainId

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                FacadeFactory.Instance.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", savedDocument.Name, savedDocument.DocumentId))
            End If

            ''Salvo il documento nella FileResolution
            Facade.ResolutionFacade.SqlResolutionDocumentUpdate(CurrentResolution.Id, idDocumentoPubblicazionePrivacy, docTypeToSave)
        End If
    End Sub

    Public Function UsePrivacy() As Boolean
        Return Boolean.Parse(rblSelectPrivacy.SelectedValue)
    End Function

    Public Sub ResetSelector()
        rblSelectPrivacy.SelectedIndex = -1
        PrivacyDocumentVisible = False
    End Sub

    Public Function PrivacyDocuments() As IList(Of DocumentInfo)
        Return uscPrivacyPublicationDocumentUploader.DocumentInfos
    End Function

    Public Sub PrivacyDocumentPanelShow()
        If rblSelectPrivacy.SelectedItem IsNot Nothing AndAlso Boolean.Parse(rblSelectPrivacy.SelectedValue) Then
            Dim resolutionRegistrationMessage As String
            If UseResolutionNumberDisplay Then
                resolutionRegistrationMessage = String.Format("{0} inserita con numero {1}{2}{2}", Facade.ResolutionTypeFacade.GetDescription(ResolutionType), CurrentResolution.InclusiveNumber, Environment.NewLine)
            End If
            BasePage.AjaxAlert(String.Format("{0}Stampare il documento privacy generato,{1}effettuare le correzioni privacy,{1}quindi ricaricarlo nell'apposita sezione tramite scanner e{1}premere 'COMPLETA INSERIMENTO'.", resolutionRegistrationMessage, Environment.NewLine))
            PrivacyDocumentVisible = True
            uscPrivacyPublicationDocumentUploader.IsDocumentRequired = True

            If UseResolutionNumberDisplay Then
                lblNumberLabel.InnerText = String.Format("{0}: ", Facade.ResolutionTypeFacade.GetDescription(ResolutionType))
                lblNumber.InnerText = CurrentResolution.InclusiveNumber
                tblResolutionNumber.Visible = True
            End If
        End If
    End Sub

    Public Sub GeneratePrivacyDocumentToPrint(ByVal workflowType As String)
        '' Carico lo step di pubblicazione
        Dim publicationWorkstep As TabWorkflow = Facade.TabWorkflowFacade.GetByDescription("Pubblicazione", workflowType)
        If StringHelper.InStrTest(publicationWorkstep.ManagedWorkflowData, "DocTypesToPublish") Then
            ''Se è presente il parametro significa che devo generare il documento secondo le modalità di ASMN
            GeneratedDocumentUrls = Facade.ResolutionWPFacade.GetMergedPublicationDocument(CurrentResolution, Nothing, String.Empty, True)

            'Attivo il pannello dei documenti privacy
            PrivacyDocumentPanelShow()
        End If
    End Sub

#End Region

End Class