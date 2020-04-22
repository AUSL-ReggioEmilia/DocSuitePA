Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Workflows
Imports VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class RequestStatement
    Inherits CommonBasePage

#Region " Fields "
    Private Const DEMATERIALISATION_TITLE As String = "Richiesta attestazione"
    Private Const SECURE_DOCUMENT_TITLE As String = "Richiesta securizzazione"
    Private Const SECURE_PAPER_TITLE As String = "Richiesta contrassegno a stampa"
    Private Const DEMATERIALISATION_TYPE As String = "Dematerialisation"
    Private Const SECUREDOCUMENT_TYPE As String = "SecureDocument"
    Private Const SECUREPAPER_TYPE As String = "SecurePaper"

    Private _currentProtocol As Protocol = Nothing
    Private Event RblWorkflowSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
#End Region

#Region " Properties"


    Public ReadOnly Property UDUniqueId As Guid
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("UniqueId", Guid.Empty)
        End Get
    End Property

    Public ReadOnly Property IdChain As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("IdChain", Guid.Empty)
        End Get
    End Property
    Public ReadOnly Property IdAttachmentsChain As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("IdAttachmentsChain", Guid.Empty)
        End Get
    End Property
    Public ReadOnly Property ArchiveName As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("ArchiveName", String.Empty)
        End Get
    End Property

    Public ReadOnly Property DocumentUnitIds As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("DocumentUnitIds", String.Empty)
        End Get
    End Property

    Public ReadOnly Property FromPageMultiple As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("FromPageMultiple", False)
        End Get
    End Property

    Public ReadOnly Property MultiWorkflowStatement As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("MultiWorkflowStatement", False)
        End Get
    End Property

    Public ReadOnly Property RequestStatementType As String
        Get
            If Not FromPageMultiple AndAlso MultiWorkflowStatement Then
                Select Case ddlWorkflows.SelectedValue
                    Case DEMATERIALISATION_TYPE
                        Return uscRequestStatement.DEMATERIALISATION_STATEMENT
                    Case SECUREDOCUMENT_TYPE
                        Return uscRequestStatement.SECURE_DOCUMENT_STATEMENT
                    Case SECUREPAPER_TYPE
                        Return uscRequestStatement.SECURE_PAPER_STATEMENT
                End Select
            End If
            Return uscRequestStatement.DEMATERIALISATION_STATEMENT
        End Get
    End Property

    Public ReadOnly Property CurrentProtocol As Protocol
        Get
            If _currentProtocol Is Nothing Then
                Dim udUniqueId As Guid = JsonConvert.DeserializeObject(Of List(Of Guid))(DocumentUnitIds).First()
                _currentProtocol = Facade.ProtocolFacade.GetByUniqueId(udUniqueId)
            End If
            Return _currentProtocol
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        Initialize()
        If Not IsPostBack Then
            MasterDocSuite.TitleVisible = False
            wfPanel.Visible = False
            ddlWorkflows.Visible = False
            Select Case RequestStatementType
                Case uscRequestStatement.DEMATERIALISATION_STATEMENT
                    Title = DEMATERIALISATION_TITLE
                Case uscRequestStatement.SECURE_DOCUMENT_STATEMENT
                    Title = SECURE_DOCUMENT_TITLE
                Case uscRequestStatement.SECURE_PAPER_STATEMENT
                    Title = SECURE_PAPER_TITLE
            End Select
            If MultiWorkflowStatement AndAlso Not FromPageMultiple Then
                InitializeWorkflowsSelector()
                Title = "Avvio nuovo flusso di lavoro"
            End If
        End If
    End Sub

    Private Sub btnConfirm_onClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        Dim commandSended As Boolean = uscRequestStatementId.SendCommand()

        If commandSended Then
            AjaxManager.ResponseScripts.Add(String.Format("CloseModal('{0}');", "True"))
        End If
    End Sub

    Private Sub rblWorkflows_selectedIndexChanged(sender As Object, e As EventArgs) Handles ddlWorkflows.SelectedIndexChanged
        uscRequestStatementId.RequestStatementType = RequestStatementType
        uscRequestStatementId.Initialize()

        If RequestStatementType.Equals(uscRequestStatement.DEMATERIALISATION_STATEMENT) _
            AndAlso CurrentProtocol IsNot Nothing AndAlso CurrentProtocol.Container IsNot Nothing Then
            Dim currentContainerEnv As ContainerEnv = New ContainerEnv(DocSuiteContext.Current, CurrentProtocol.Container)
            Dim defaultSigner As String = currentContainerEnv.DefaultDematerialisationSigner
            If Not String.IsNullOrEmpty(defaultSigner) Then
                uscRequestStatementId.LoadADSigner(defaultSigner)
            End If
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        uscRequestStatementId.TenantName = TenantName
        uscRequestStatementId.TenantId = TenantId.ToString()
        uscRequestStatementId.TypeId = Type
        uscRequestStatementId.IdChain = IdChain
        uscRequestStatementId.IdAttachmentsChain = IdAttachmentsChain
        uscRequestStatementId.ArchiveName = ArchiveName
        uscRequestStatementId.DocumentUnitsIds = DocumentUnitIds
        uscRequestStatementId.FromPageMultiple = FromPageMultiple
        uscRequestStatementId.RequestStatementType = RequestStatementType
    End Sub

    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, RequestStatement)(key)
    End Function

    Public Overloads Function GetKeyValueOrDefault(Of T)(key As String, defaultValue As T) As T
        Return Context.Request.QueryString.GetValueOrDefault(key, defaultValue)
    End Function

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, pnlRequestStatementCompliance, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlWorkflows, pnlRequestStatementCompliance, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub InitializeWorkflowsSelector()
        wfPanel.Visible = True
        ddlWorkflows.Visible = True
        Dim udUniqueId As Guid = JsonConvert.DeserializeObject(Of List(Of Guid))(DocumentUnitIds).First()
        Dim pageRights As RequestStatementPageRights = ReadEntityRigths(udUniqueId, Type)
        If Not pageRights.SecureDocumentVisible AndAlso Not pageRights.DematerialisationVisible AndAlso Not pageRights.SecurePaperVisible Then
            ddlWorkflows.Visible = False
            lblNoWorkflow.Visible = True
            uscRequestStatementId.Visible = False
            lblNoWorkflow.Text = "Nessun flusso di lavoro abilitato"
            btnConfirm.Enabled = False
            Exit Sub
        End If

        'TODO: Gestione dinamica dei flussi utilizzabili        
        If pageRights.DematerialisationVisible Then
            ddlWorkflows.Items.Add(New DropDownListItem("Richiesta attestazione", DEMATERIALISATION_TYPE))
        End If
        If pageRights.SecureDocumentVisible Then
            ddlWorkflows.Items.Add(New DropDownListItem("Richiesta securizzazione", SECUREDOCUMENT_TYPE))
        End If
        If pageRights.SecurePaperVisible Then
            ddlWorkflows.Items.Add(New DropDownListItem("Richiesta contrassegno a stampa", SECUREPAPER_TYPE))
        End If

        ddlWorkflows.SelectedIndex = 0
        uscRequestStatementId.RequestStatementType = RequestStatementType
        RaiseEvent RblWorkflowSelectedIndexChanged(Me, New EventArgs())
    End Sub

    Public Function ReadEntityRigths(udUniqueId As Guid, udType As String) As RequestStatementPageRights
        Dim result As RequestStatementPageRights = New RequestStatementPageRights()
        Select Case udType.ToUpperInvariant()
            Case "PROT"
                Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetByUniqueId(udUniqueId)
                Dim currentProtocolRights As ProtocolRights = New ProtocolRights(currentProtocol)
                result.DematerialisationVisible = currentProtocolRights.CanDematerialise
                result.SecureDocumentVisible = currentProtocolRights.CanSecureDocument AndAlso ProtocolEnv.SecureDocumentWorkflowStatementVisibility.DOU
                result.SecurePaperVisible = currentProtocolRights.CanSecureDocument AndAlso ProtocolEnv.SecureDocumentWorkflowStatementVisibility.SecurePaper
        End Select
        Return result
    End Function
#End Region

End Class