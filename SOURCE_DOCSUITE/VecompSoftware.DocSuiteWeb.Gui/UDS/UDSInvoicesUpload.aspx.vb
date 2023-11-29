Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Services.Biblos.Models

Public Class UDSInvoicesUpload
    Inherits UDSBasePage

#Region " Fields "
    Private Const CORRELATED_CHAINID_CALLBACK As String = "SetChainId('{0}', {1});"
    Private Const CORRELATED_IDDOCUMENT_CALLBACK As String = "SetIdDocument('{0}');"
#End Region

#Region " Properties "
    Public ReadOnly Property CurrentUserTenantName As String
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.TenantName
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantId As Guid
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.UniqueId
            End If
            Return Guid.Empty
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantAOOId As Guid
        Get
            If CurrentTenant IsNot Nothing Then
                Return CurrentTenant.TenantAOO.UniqueId
            End If
            Return Guid.Empty
        End Get
    End Property

    Private currentDocumentValue As DocumentInfo
    Public Property CurrentDocument() As DocumentInfo
        Get
            Return currentDocumentValue
        End Get
        Set(ByVal value As DocumentInfo)
            currentDocumentValue = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack() Then
            rgvPreviewDocuments.Style.Add(HtmlTextWriterStyle.Display, "none")
            rgvPreviewDocuments.DataSource = New List(Of String)()
            rdtpDataRicezioneSdi.SelectedDate = Date.UtcNow
        End If
    End Sub

    Private Sub rgvPreviewDocuments_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles rgvPreviewDocuments.ItemDataBound
        If TypeOf e.Item Is GridDataItem Then
            Dim dataItem As GridDataItem = CType(e.Item, GridDataItem)
            Dim chkBox As CheckBox = CType(dataItem("CheckCol").Controls(0), CheckBox)
            chkBox.Enabled = True
        End If
    End Sub
    Private Sub UscUploadDocumentiDocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscDocumentUpload.DocumentUploaded
        btnPreview.Enabled = True
        If Not FileHelper.MatchExtension(e.Document.Name, FileHelper.ZIP) Then
            AjaxAlert("E' possibile processare solo file con estensione .zip")
            uscDocumentUpload.RemoveDocumentInfo(e.Document)
            Return
        End If
        Dim location As Location = FacadeFactory.Instance.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.CollaborationLocation)
        Dim biblosDocumentInfo As BiblosDocumentInfo = e.Document.ArchiveInBiblos(location.ProtBiblosDSDB)
        AjaxManager.ResponseScripts.Add(String.Format(CORRELATED_CHAINID_CALLBACK, biblosDocumentInfo.ChainId.ToString(), "false"))
        AjaxManager.ResponseScripts.Add(String.Format(CORRELATED_IDDOCUMENT_CALLBACK, biblosDocumentInfo.DocumentId.ToString()))
    End Sub

    Protected Sub UDSInvoicesUploadAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch ex As Exception

        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, "GenerateInvoiceZIP") AndAlso ajaxModel IsNot Nothing AndAlso ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 Then
            Try
                Dim uploadedDocument As DocumentInfo = uscDocumentUpload.DocumentInfos.First()
                Dim location As Location = FacadeFactory.Instance.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.CollaborationLocation)
                Dim biblosDocumentInfo As BiblosDocumentInfo = New BiblosDocumentInfo(Guid.Parse(HFcorrelatedIdDocument.Value), Guid.Parse(HFcorrelatedChainId.Value))
                Dim content As List(Of KeyValuePair(Of String, Byte())) = New List(Of KeyValuePair(Of String, Byte()))
                Dim model As List(Of InvoicePreviewModel) = JsonConvert.DeserializeObject(Of List(Of InvoicePreviewModel))(ajaxModel.Value(0))
                Dim zipCompress As ZipCompress = New ZipCompress()
                Dim extractedFiles As List(Of CompressItem) = zipCompress.InMemoryExtract(New MemoryStream(uploadedDocument.Stream))
                For Each file As CompressItem In extractedFiles
                    If model.Any(Function(x) x.InvoiceFilename = file.Filename) Then
                        content.Add(New KeyValuePair(Of String, Byte())(file.Filename, file.Data))
                    End If
                    If model.Any(Function(x) x.InvoiceMetadataFilename = file.Filename) Then
                        content.Add(New KeyValuePair(Of String, Byte())(file.Filename, file.Data))
                    End If
                Next
                biblosDocumentInfo.Stream = zipCompress.InMemoryCompressWithoutPrefix(content)
                biblosDocumentInfo.Update(DocSuiteContext.Current.User.FullUserName)
                AjaxManager.ResponseScripts.Add(String.Format(CORRELATED_CHAINID_CALLBACK, biblosDocumentInfo.ChainId.ToString(), "true"))
            Catch ex As Exception

            End Try
        End If
    End Sub
#End Region

#Region " Methods "
    Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UDSInvoicesUploadAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlContentInvoice, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub
#End Region

End Class