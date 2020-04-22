Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class UscToSeries
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private _containerChainTypes As List(Of ChainType) = Nothing

    Private _allDocuments As List(Of DocumentInfo)

    Private _currentDocumentSeriesId As String = String.Empty

    Public Delegate Sub OnNeedDocumentsSourceEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Event NeedDocumentsSource As OnNeedDocumentsSourceEventHandler
    Public Enum ChainType
        <Description("Documenti")>
        Document
        <Description("Annessi")>
        Annexed
        <Description("Annessi da non pubblicare")>
        UnpublishedAnnexed
    End Enum
#End Region

#Region " Properties "

    Public Property DocumentSource As List(Of DocumentInfo)

    Public ReadOnly Property SelectedDocuments() As Dictionary(Of DocumentInfo, ChainType)
        Get
            Dim selected As New Dictionary(Of DocumentInfo, ChainType)
            Dim documentInfo As DocumentInfo = Nothing
            For Each item As GridDataItem In documentListGrid.MasterTableView.GetSelectedItems()
                documentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))
                If DirectCast(item.FindControl("pdf"), RadButton).Checked AndAlso TypeOf DocumentInfo Is BiblosDocumentInfo Then
                    DocumentInfo = New BiblosPdfDocumentInfo(DirectCast(DocumentInfo, BiblosDocumentInfo))
                Else
                    DocumentInfo.Signature = String.Empty
                End If
                Dim chainType As ChainType = DirectCast([Enum].Parse(GetType(ChainType), DirectCast(item.FindControl("chainTypes"), DropDownList).SelectedValue), ChainType)
                selected.Add(DocumentInfo, chainType)
            Next
            Return selected
        End Get
    End Property

    Public ReadOnly Property SelectedDocumentSeriesId As String
        Get
            If String.IsNullOrEmpty(_currentDocumentSeriesId) Then
                _currentDocumentSeriesId = ddlDocumentSeries.SelectedValue
            End If
            Return _currentDocumentSeriesId
        End Get
    End Property

    '' <summary> Tipi di catene nelle quali inserire i documenti per il container selezionato </summary>
    Private ReadOnly Property ContainerChainTypes As List(Of ChainType)
        Get
            If _containerChainTypes Is Nothing AndAlso Not String.IsNullOrEmpty(SelectedDocumentSeriesId) Then
                _containerChainTypes = New List(Of ChainType)()
                _containerChainTypes.Add(ChainType.Document)
                Dim container As Data.Container = Facade.ContainerFacade.GetById(Integer.Parse(SelectedDocumentSeriesId))
                If container.DocumentSeriesAnnexedLocation IsNot Nothing Then
                    _containerChainTypes.Add(ChainType.Annexed)
                End If
                If container.DocumentSeriesUnpublishedAnnexedLocation IsNot Nothing Then
                    _containerChainTypes.Add(ChainType.UnpublishedAnnexed)
                End If
            End If

            Return _containerChainTypes
        End Get
    End Property
#End Region

#Region " Events"

    Protected Sub uscToSeries_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            DocumentSeries.Text = String.Concat(ProtocolEnv.DocumentSeriesName, ":")
            rfvDocumentSeries.Text = "Selezionare un elemento dall'elenco"
        End If
    End Sub

    Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
        RaiseEvent NeedDocumentsSource(Me, New EventArgs())
    End Sub

    Protected Sub DocumentListGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles documentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)

        With DirectCast(e.Item.FindControl("fileImage"), RadButton)
            .Image.ImageUrl = ImagePath.FromDocumentInfo(item)
            .OnClientClicked = "OpenGenericWindow"
            .CommandArgument = String.Format("../Viewers/DocumentInfoViewer.aspx?{0}", item.ToQueryString().AsEncodedQueryString())
        End With

        ' Carico tutti i tipi di catena documentale disponibili nel contenitore
        With DirectCast(e.Item.FindControl("chainTypes"), DropDownList)
            .Visible = False
            .Items.Clear()
            If ContainerChainTypes IsNot Nothing Then
                .Visible = True
                For Each chainType As ChainType In ContainerChainTypes
                    .Items.Add(New ListItem(GetSeriesChainDescription(chainType), chainType.ToString("D")))
                Next
            End If
        End With

        Dim fileName As Label = DirectCast(e.Item.FindControl("fileName"), Label)
        fileName.Text = item.Name
        Dim fileType As Label = DirectCast(e.Item.FindControl("fileType"), Label)
        fileType.Text = item.Parent.Name

    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, documentListGrid, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(documentListGrid, documentListGrid, BasePage.MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Public Sub BindDocuments()
        documentListGrid.DataSource = DocumentSource
        documentListGrid.DataBind()
    End Sub

    Public Sub BindContainers(filters As IList(Of DocumentSeriesItem))
        ' caricamento 
        ' Contenitori su cui l'operatore ha diritti di inserimento
        Dim availableContainers As IList(Of Data.Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, DocumentSeriesContainerRightPositions.Insert, True)

        If filters IsNot Nothing AndAlso filters.Count > 0 Then
            availableContainers = availableContainers.Where(Function(f) Not filters.Any(Function(r) r.DocumentSeries.Container.Id = f.Id)).ToList()
        End If

        ddlDocumentSeries.DataValueField = "Id"
        ddlDocumentSeries.DataTextField = "Name"
        ddlDocumentSeries.DataSource = availableContainers
        ddlDocumentSeries.DataBind()
        ' Rendo necessaria la selezione di un atto
        ddlDocumentSeries.Items.Insert(0, New ListItem("Scegliere un archivio...", ""))
        ddlDocumentSeries.SelectedIndex = 0
        ddlDocumentSeries.SelectedValue = SelectedDocumentSeriesId
    End Sub

    Public Shared Function GetWithDummyParent(ByVal parentName As String, ByVal documents As IList(Of DocumentInfo)) As List(Of DocumentInfo)
        Dim parent As New FolderInfo(Guid.NewGuid.ToString(), parentName)

        Dim list As New List(Of DocumentInfo)(documents.Count)
        For Each item As DocumentInfo In documents
            item.Parent = parent
            list.Add(item)
        Next
        Return list
    End Function

    Public Function GetSeriesChainDescription(chain As ChainType) As String
        Dim chainDescription As String = String.Empty
        Select Case chain
            Case ChainType.Document
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain)) Then
                    chainDescription = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.MainChain)
                End If

            Case ChainType.Annexed
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain)) Then
                    chainDescription = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
                End If

            Case ChainType.UnpublishedAnnexed
                If (ProtocolEnv.DocumentSeriesDocumentsLabel.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)) Then
                    chainDescription = ProtocolEnv.DocumentSeriesDocumentsLabel(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
                End If
        End Select
        Return chainDescription
    End Function
#End Region

End Class