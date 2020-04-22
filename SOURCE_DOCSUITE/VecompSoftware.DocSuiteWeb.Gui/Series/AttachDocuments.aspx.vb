Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Gui.Resl
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Web
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Namespace Series
    Public Class AttachDocuments
        Inherits CommonBasePage

#Region " Fields "

        Private _currentSeries As DocumentSeries

#End Region

#Region " Properties "

        Private ReadOnly Property CurrentSeries As DocumentSeries
            Get
                If _currentSeries Is Nothing AndAlso Not String.IsNullOrEmpty(ddlDocumentSeries.SelectedValue) Then
                    _currentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(CType(ddlDocumentSeries.SelectedValue, Integer))
                End If
                Return _currentSeries
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

        ''' <summary>
        ''' Elenco dei protocolli collegati che han passato i documenti al protocollo in inserimento.
        ''' </summary>
        ''' <remarks> Stringa di ID separata da virgole. </remarks>
        Public Property SessionProtInserimentoLinks As String
            Get
                If Not Session.Item("Series-AttachDocuments") Is Nothing Then
                    Return Session.Item("Series-AttachDocuments").ToString()
                Else
                    Return String.Empty
                End If
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Session.Remove("Series-AttachDocuments")
                Else
                    Session.Item("Series-AttachDocuments") = value
                End If
            End Set
        End Property

        Public Property SessionSeriesNumber As String
            Get
                If Not Session.Item("SeriesNumberAttach") Is Nothing Then
                    Return Session.Item("SeriesNumberAttach").ToString()
                Else
                    Return String.Empty
                End If
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Session.Remove("SeriesNumberAttach")
                Else
                    Session.Item("SeriesNumberAttach") = value
                End If
            End Set
        End Property
#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            MasterDocSuite.TitleVisible = False
            InitializeAjax()
            If Not Page.IsPostBack Then
                Page.Title = "Allega da " & ProtocolEnv.DocumentSeriesName
                Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, DocumentSeriesContainerRightPositions.Insert, True)
                ddlDocumentSeries.DataValueField = "Id"
                ddlDocumentSeries.DataTextField = "Name"
                ddlDocumentSeries.DataSource = availableContainers
                ddlDocumentSeries.DataBind()
                ddlDocumentSeries.Items.Insert(0, "")

                txtYear.Value = DateTime.Today.Year
            End If

        End Sub

        Private Sub AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
            Dim idSeries As Integer = Integer.Parse(e.Argument)
            Dim item As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(idSeries)
            ddlDocumentSeries.SelectedValue = item.DocumentSeries.Container.Id.ToString()
            LoadDocuments(item)
        End Sub

        Private Sub BtnSelezionaClick(sender As Object, e As EventArgs) Handles btnSeleziona.Click
            LoadDocuments(CurrentSeries, Short.Parse(txtYear.Text), Integer.Parse(txtNumber.Text))
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
            fileType.Text = item.Parent.Name

        End Sub

        Private Sub BtnAddClick(sender As Object, e As EventArgs) Handles btnAdd.Click

            ' controllo i documenti selezionati
            Dim docs As IList(Of DocumentInfo) = SelectedDocuments
            If docs.IsNullOrEmpty() Then
                AjaxAlert("Devi selezionare almeno un file.")
                Exit Sub
            End If

            ' prendo tutti i files temporanei generati a partire dalla selezione
            CloseWindowScript(docs)
        End Sub

#End Region

#Region " Methods "

        Private Function GetDocuments(item As DocumentSeriesItem) As List(Of DocumentInfo)
            Dim tor As New List(Of DocumentInfo)

            ' Documenti
            Dim docs As List(Of BiblosDocumentInfo) = Facade.DocumentSeriesItemFacade.GetMainDocuments(item)
            If Not docs.IsNullOrEmpty() Then
                tor.AddRange(ToSeries.GetWithDummyParent("Documenti", docs.Cast(Of DocumentInfo).ToList()))
            End If

            ' Annessi
            If item.LocationAnnexed IsNot Nothing Then
                Dim annexedDocs As List(Of BiblosDocumentInfo) = Facade.DocumentSeriesItemFacade.GetAnnexedDocuments(item)
                If Not annexedDocs.IsNullOrEmpty() Then
                    tor.AddRange(ToSeries.GetWithDummyParent("Annessi", annexedDocs.Cast(Of DocumentInfo).ToList()))
                End If
            End If

            Return tor
        End Function

        Private Sub InitializeAjax()
            AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest

            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, HeaderWrapper, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnAdd, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, btnAdd, MasterDocSuite.AjaxFlatLoadingPanel)

            AjaxManager.AjaxSettings.AddAjaxSetting(btnAdd, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)

        End Sub

        Private Sub LoadDocuments(item As DocumentSeriesItem)
            btnAdd.Visible = False
            DocumentListGrid.Visible = False

            If item Is Nothing Then
                AjaxAlert(String.Format("Impossibile trovare [{0}]", ProtocolEnv.DocumentSeriesName))
                Exit Sub
            End If

            hf_selectYear.Value = item.Year
            hf_selectNumber.Value = If(item.Number.HasValue, item.Number.Value, String.Empty)
            Dim rights As New DocumentSeriesItemRights(item)
            If Not rights.IsReadable Then
                ' L'utente non ha i diritti di modifica sul protocollo selezionato
                AjaxAlert("{2} n. {0}/{1} - Mancano i diritti necessari", item.Year, item.Number, ProtocolEnv.DocumentSeriesName)
                Exit Sub
            End If

            DocumentListGrid.DataSource = GetDocuments(item)
            DocumentListGrid.DataBind()

            ItemPreview.Item = item
            ItemPreview.Show()

            btnAdd.Visible = True
            DocumentListGrid.Visible = True
        End Sub

        Private Sub LoadDocuments(series As DocumentSeries, year As Short, number As Integer)
            Dim item As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetByYearAndNumber(series, year, number)
            LoadDocuments(item)
        End Sub

        Private Sub CloseWindowScript(documents As IList(Of DocumentInfo))

            Dim list As List(Of String) = documents.Select(Function(s) HttpUtility.UrlEncode(s.ToQueryString().AsEncodedQueryString())).ToList()
            'Imposto StringEscapeHandling = EscapeHtml per evitare i caratteri che possono generare errore (es. apostrofo)
            Dim serialized As String = JsonConvert.SerializeObject(list)
            Dim jsStringEncoded As String = HttpUtility.JavaScriptStringEncode(serialized)

            Dim closeWindow As String = String.Format("CloseWindow('{0}');", jsStringEncoded)
            MasterDocSuite.AjaxManager.ResponseScripts.Add(closeWindow)

            If String.IsNullOrEmpty(hf_selectYear.Value) OrElse String.IsNullOrEmpty(hf_selectNumber.Value) Then
                Throw New InvalidCastException("Errore di conversione in recupero identificativo Serie.")
            End If

            Dim protocolId As String = String.Format("{0}/{1}/{2}", CurrentSeries.Id, hf_selectYear.Value, hf_selectNumber.Value)
            Dim protocolNumber As String = WebHelper.UploadDocumentRename(CurrentSeries.Name, Int16.Parse(hf_selectYear.Value), Int32.Parse(hf_selectNumber.Value), ProtocolEnv.UploadDocumentSeriesRenameMaxLength)

            If Not SessionProtInserimentoLinks.Contains(protocolId) Then
                SessionProtInserimentoLinks = String.Format("{0}{1},", SessionProtInserimentoLinks, protocolId)
            End If

            If Not SessionSeriesNumber.Contains(protocolNumber) Then
                SessionSeriesNumber = protocolNumber
            End If
        End Sub

#End Region

    End Class
End Namespace