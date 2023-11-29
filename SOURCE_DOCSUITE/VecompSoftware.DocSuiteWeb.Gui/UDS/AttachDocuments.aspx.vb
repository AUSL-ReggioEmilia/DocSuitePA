Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class AttachDocuments
    Inherits CommonBasePage

#Region " Fields "
    Private _UDSFacade As UDSFacade
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private Const GRID_DOCUMENTS_CALLBACK As String = "attachDocuments.gridDocumentsCallback();"
    Private Const CLOSE_WINDOW_CALLBACK As String = "attachDocuments.closeWindow('{0}');"
#End Region

#Region " Properties "
    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _UDSFacade Is Nothing Then
                _UDSFacade = New UDSFacade()
            End If
            Return _UDSFacade
        End Get
    End Property

    Public ReadOnly Property SelectedDocuments() As List(Of DocumentInfo)
        Get
            Dim selected As New List(Of DocumentInfo)
            For Each item As GridDataItem In DocumentListGrid.MasterTableView.GetSelectedItems()
                Dim pdf As RadButton = DirectCast(item.FindControl("pdf"), RadButton)
                Dim documentInfo As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))
                If pdf.Checked AndAlso TypeOf documentInfo Is BiblosDocumentInfo Then
                    documentInfo = New BiblosPdfDocumentInfo(DirectCast(documentInfo, BiblosDocumentInfo))
                Else
                    documentInfo.Signature = String.Empty
                End If
                selected.Add(documentInfo)
            Next
            Return selected
        End Get
    End Property

    Public Property SessionUDSNumber As String
        Get
            If Not Session.Item("UDSNumberAttach") Is Nothing Then
                Return Session.Item("UDSNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
        Set(value As String)
            If String.IsNullOrEmpty(value) Then
                Session.Remove("UDSNumberAttach")
            Else
                Session.Item("UDSNumberAttach") = value
            End If
        End Set
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjax()
    End Sub

    Private Sub AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Select Case ajaxModel.ActionName
            Case "SetGridDocuments"
                Dim udsRepositoryId As Guid = Guid.Parse(ajaxModel.Value(0))
                Dim udsId As Guid = Guid.Parse(ajaxModel.Value(1))
                Dim uds As UDSDto = GetSource(udsRepositoryId, udsId)
                Dim documents As IList(Of DocumentInfo) = UDSFacade.GetAllDocuments(uds.UDSModel)
                DocumentListGrid.DataSource = documents
                DocumentListGrid.DataBind()
                AjaxManager.ResponseScripts.Add(GRID_DOCUMENTS_CALLBACK)
            Case "AddDocuments"
                Dim udsRepositoryId As Guid = Guid.Parse(ajaxModel.Value(0))
                Dim udsId As Guid = Guid.Parse(ajaxModel.Value(1))
                Dim uds As UDSDto = GetSource(udsRepositoryId, udsId)
                AddDocuments(uds.Year, uds.Number)
        End Select
    End Sub

    Protected Sub DocumentListGridItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)

        Dim fileImage As Image = DirectCast(e.Item.FindControl("fileImage"), Image)
        fileImage.ImageUrl = ImagePath.FromDocumentInfo(item)
        Dim fileName As Label = DirectCast(e.Item.FindControl("fileName"), Label)
        fileName.Text = item.Name
        Dim fileType As Label = DirectCast(e.Item.FindControl("fileType"), Label)
        fileType.Text = If(item.Parent Is Nothing, String.Empty, item.Parent.Name)

        btnAdd.Visible = True
    End Sub

    Private Sub AddDocuments(ByVal year As Integer, ByVal number As Integer)
        Dim docs As IList(Of DocumentInfo) = SelectedDocuments
        If docs.IsNullOrEmpty() Then
            AjaxAlert("Devi selezionare almeno un file.")
            Exit Sub
        End If
        CloseWindowScript(docs, year, number)
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Protected Function GetSource(UDSRepositoryId As Guid, UDSId As Guid) As UDSDto
        Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(UDSRepositoryId)
        Return CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, UDSId))
    End Function

    Private Sub CloseWindowScript(documents As IList(Of DocumentInfo), year As Integer, number As Integer)
        Dim list As New Dictionary(Of String, String)
        For Each item As BiblosDocumentInfo In documents
            list.Add(BiblosFacade.SaveUniqueToTemp(item).Name, item.Name)
        Next
        Dim serialized As String = JsonConvert.SerializeObject(list)
        Dim jsStringEncoded As String = HttpUtility.JavaScriptStringEncode(serialized)
        Dim closeWindow As String = String.Format(CLOSE_WINDOW_CALLBACK, jsStringEncoded)
        MasterDocSuite.AjaxManager.ResponseScripts.Add(closeWindow)

        Dim protocolNumber As String = WebHelper.UploadDocumentRename("UDS", year, number, ProtocolEnv.UploadDocumentSeriesRenameMaxLength)

        If Not SessionUDSNumber.Contains(protocolNumber) Then
            SessionUDSNumber = protocolNumber
        End If
    End Sub
#End Region

End Class