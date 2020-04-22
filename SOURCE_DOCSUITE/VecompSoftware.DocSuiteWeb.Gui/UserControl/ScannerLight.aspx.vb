Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Telerik.Web.UI

Public Class ScannerLight
    Inherits CommonBasePage

#Region " Fields "

    Private _currentConfiguration As ScannerConfiguration
    Private _fileNameInsert As String
    Private Const CheckDematerialisationComplianceQueryItem As String = "checkcompl"
#End Region

#Region " Properties "

    Protected ReadOnly Property MaxImagesToScan As String
        Get
            Return DocSuiteContext.Current.ProtocolEnv.ScannerBufferMaximum.ToString()
        End Get
    End Property

    Private ReadOnly Property CurrentConfiguration As ScannerConfiguration
        Get
            If _currentConfiguration Is Nothing Then
                _currentConfiguration = Facade.ScannerConfigurationFacade.GetCurrentConfiguration
            End If
            Return _currentConfiguration
        End Get
    End Property

    Private ReadOnly Property DocumentName As String
        Get
            Dim name As String = CType(ViewState("_documentName"), String)
            If String.IsNullOrEmpty(name) Then
                name = String.Format("{0}-Insert-{1:HHmmss}-DaScanner000000.Scan0.pdf", CommonUtil.UserDocumentName, Now())
                ViewState("_documentName") = name
            End If
            Return name
        End Get
    End Property

    Public Property FileNameInsert() As String
        Get
            If String.IsNullOrEmpty(_fileNameInsert) Then
                Dim name As String = fileName.Text
                If String.IsNullOrEmpty(name) Then
                    Return fileName.EmptyMessage
                End If

                If Not FileHelper.IsValidFileName(name) Then
                    Return fileName.EmptyMessage
                End If
                _fileNameInsert = fileName.Text
            End If
            Return _fileNameInsert
        End Get
        Set(value As String)
            _fileNameInsert = value
        End Set
    End Property
    Private ReadOnly Property ChkCheckDematerialisationCompliance As RadButton
        Get
            Dim controlCompliance As RadToolBarButton = DirectCast(ToolBar.FindButtonByCommandName("CheckDematerialisationCompliance"), RadToolBarButton)
            Return DirectCast(controlCompliance.FindControl("chkDematerialisationCompliance"), RadButton)
        End Get
    End Property
    Private ReadOnly Property IsInCompliance As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(CheckDematerialisationComplianceQueryItem, False)
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub DdlScannerConfigurationSelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlScannerConfiguration.SelectedIndexChanged
        InitializeConfiguration(Facade.ScannerConfigurationFacade.GetById(Integer.Parse(ddlScannerConfiguration.SelectedItem.Value)))
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        ' tenere nel code behind questa parte permette di manutenere più facilmente la compatibilità con uscDocumentUpload
        If Not FileHelper.MatchExtension(FileNameInsert, FileHelper.PDF) Then
            FileNameInsert += FileHelper.PDF
        End If
        Dim dict As New Dictionary(Of String, String)()

        dict.Add(DocumentName, FileNameInsert)
        If ChkCheckDematerialisationCompliance.Visible Then
            dict.Add("CheckDematerialisationCompliance", ChkCheckDematerialisationCompliance.Checked.ToString())
        End If
        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", JsonConvert.SerializeObject(dict)))
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        ' Inizializzo i pulsanti
        Dim host As String = Request.Url.Host
        Dim port As String = Request.Url.Port.ToString()
        Dim actionPage As String = Page.ResolveUrl("~/UserControl/ScannerLightSave.aspx")

        btnUpload.OnClientClicking = String.Format("function (button,args){{ btnUpload_onclick(button, args, '{0}', '{1}', '{2}', '{3}', '{4}');}}", host, port, Request.IsSecureConnection, actionPage, DocumentName)

        maxPagesToScan.Text = MaxImagesToScan

        ChkCheckDematerialisationCompliance.Visible = False
        If DocSuiteContext.Current.ProtocolEnv.DematerialisationEnabled AndAlso (IsInCompliance) Then
            ChkCheckDematerialisationCompliance.Visible = True
        End If

        If Not DocSuiteContext.Current.ProtocolEnv.ScannerConfigurationEnabled Then
            divScannerConfiguration.Visible = False
            Exit Sub
        End If

        divScannerConfiguration.Visible = True
        ddlScannerConfiguration.DataSource = Facade.ScannerConfigurationFacade.GetAll()
        ddlScannerConfiguration.DataValueField = "Id"
        ddlScannerConfiguration.DataTextField = "Description"
        ddlScannerConfiguration.DataBind()

        If CurrentConfiguration IsNot Nothing Then
            InitializeConfiguration(CurrentConfiguration)
        End If
    End Sub

    Private Sub InitializeConfiguration(ByVal scannerConfiguration As ScannerConfiguration)
        ddlScannerConfiguration.SelectedValue = CType(scannerConfiguration.Id, String)

        If scannerConfiguration.GetParameterValue("Resolution") IsNot Nothing Then
            ddlResolution.SelectedValue = scannerConfiguration.GetParameterValue("Resolution")
        End If

        If scannerConfiguration.GetParameterValue("Color") IsNot Nothing Then
            rblColor.SelectedValue = scannerConfiguration.GetParameterValue("Color")
        End If

        If scannerConfiguration.GetParameterValue("ShowUI") IsNot Nothing Then
            chkShowUI.Checked = CBool(scannerConfiguration.GetParameterValue("ShowUI"))
        End If

        If scannerConfiguration.GetParameterValue("ADF") IsNot Nothing Then
            chkADF.Checked = CBool(scannerConfiguration.GetParameterValue("ADF"))
        End If

        If scannerConfiguration.GetParameterValue("Duplex") IsNot Nothing Then
            chkDuplex.Checked = CBool(scannerConfiguration.GetParameterValue("Duplex"))
        End If

        If scannerConfiguration.GetParameterValue("DiscardBlank") IsNot Nothing Then
            chkDiscardBlank.Checked = CBool(scannerConfiguration.GetParameterValue("DiscardBlank"))
        End If

        If scannerConfiguration.GetParameterValue("Threshold") IsNot Nothing Then
            txtThreshold.Text = scannerConfiguration.GetParameterValue("Threshold")
        End If
    End Sub

#End Region

End Class