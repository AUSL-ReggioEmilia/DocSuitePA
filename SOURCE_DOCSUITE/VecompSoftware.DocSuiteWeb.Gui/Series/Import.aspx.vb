Imports System.Collections.Generic
Imports System.IO
Imports System.Data.OleDb
Imports System.Linq
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.Services.Biblos.Models

Namespace Series
    Public Class Import
        Inherits CommonBasePage

#Region " Fields "

        Private _selectedDocumentSeries As DocumentSeries
        Private Shared ReadOnly _basicFields As String() = {"Subject", "Subsection", "DOC_Main", "DOC_Annexed", "DOC_UnpublishedAnnexed"}

#End Region

#Region " Properties "

        ''' <summary> Serie documentale selezionata </summary>
        Private ReadOnly Property SelectedDocumentSeries As DocumentSeries
            Get
                If _selectedDocumentSeries Is Nothing Then
                    If Not String.IsNullOrEmpty(ddlDocumentSeries.SelectedValue) Then
                        _selectedDocumentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(CType(ddlDocumentSeries.SelectedValue, Integer))
                    End If
                End If
                Return _selectedDocumentSeries
            End Get
        End Property

        Private ReadOnly Property SelectedArchiveInfo As ArchiveInfo
            Get
                If SelectedDocumentSeries IsNot Nothing Then
                    Return DocumentSeriesFacade.GetArchiveInfo(SelectedDocumentSeries)
                End If
                Return Nothing
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeAjax()

            If Not Page.IsPostBack Then
                Initialize()
            End If
        End Sub

        Private Sub cmdImport_Click(sender As Object, e As EventArgs) Handles cmdImport.Click
            Dim task As TaskHeader = New TaskHeader()
            Dim series As DocumentSeriesItemDTO = New DocumentSeriesItemDTO()
            Dim path As DirectoryInfo = New DirectoryInfo(ProtocolEnv.VersioningShareDocSeriesImporter)
            series.Category = New CategoryDTO()
            series.Document = New DocumentDTO()
            Dim idDocSeries As Integer
            If ddlDocumentSeries.SelectedItem Is Nothing OrElse Not Integer.TryParse(ddlDocumentSeries.SelectedItem.Value, idDocSeries) Or
                Not ItemCategory.HasSelectedCategories OrElse uscDocument.DocumentInfosAdded.Count() <= 0 Then
                AjaxAlert("Parametri non corretti. Selezionare tutte le voci richieste")
                Exit Sub
            End If
            Dim doc As DocumentInfo = uscDocument.DocumentInfosAdded(0)
            If Not Helpers.FileHelper.MatchExtension(doc.Name, Helpers.FileHelper.XLS) OrElse Helpers.FileHelper.MatchExtension(doc.Name, Helpers.FileHelper.XLSX) Then
                Throw New DocSuiteException("Importazione Excel", "Il documento non è .XLS o .XLSX.")
                Exit Sub
            End If

            series.IdDocumentSeries = FacadeFactory.Instance.DocumentSeriesFacade.GetDocumentByContainerId(idDocSeries).Id
            series.Status = DirectCast(IIf(cbDraft.Checked, DocumentSeriesItemStatus.Draft, DocumentSeriesItemStatus.Active), Integer)
            If Me.SelectedDocumentSeries.PublicationEnabled.GetValueOrDefault() Then
                series.PublishingDate = ItemPublishingDate.SelectedDate
            End If
            series.Category.Id = ItemCategory.SelectedCategories.First().Id
            series.Document.FullName = FileHelper.UniqueFileNameFormat(doc.Name)

            doc.SaveToDisk(path, series.Document.FullName)
            series.Document.FullName = path.EnumerateFiles().Single(Function(f) f.Name.Equals(series.Document.FullName)).FullName
            task.Code = JsonConvert.SerializeObject(series)
            task.TaskType = TaskTypeEnum.DocSeriesImporter
            Facade.TaskHeaderFacade.Save(task)

            ' Pulizia dei controlli
            uscDocument.ClearNodes()
            ItemCategory.Clear()
            ItemPublishingDate.SelectedDate = Nothing
            ' Registro il Log e visualizzo messaggio a video
            Dim message As String = "Importazione schedulata con successo. Importazione verrà processata ed eseguita."
            FileLogger.Debug(LoggerName, message)
            AjaxManager.ResponseScripts.Add(String.Format("openConfirmWindow('{0}')", message))
        End Sub

        Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
            Dim columns As New List(Of String)(_basicFields)
            If SelectedArchiveInfo IsNot Nothing Then
                columns.AddRange(From attribute In SelectedArchiveInfo.VisibleChainAttributes Select attribute.Name)
            End If

            lblFields.Text = String.Join(" - ", columns)

            Me.pnlPublishingDate.Visible = Me.SelectedDocumentSeries.PublicationEnabled.GetValueOrDefault()

            ItemCategory.Clear()
            ' Se disponibile più di un contenitore provo a selezionare il contenitore di default della serie documentale
            Dim behaviours As IList(Of DocumentSeriesAttributeBehaviour) = Facade.DocumentSeriesAttributeBehaviourFacade.GetAttributeBehaviours(SelectedDocumentSeries, DocumentSeriesAction.Insert, "DEFAULT")
            Dim defaultCategoryBehaviour As DocumentSeriesAttributeBehaviour = behaviours.FirstOrDefault(Function(b) b.AttributeName.Eq("#" & ItemCategory.ID))
            If defaultCategoryBehaviour IsNot Nothing Then
                ItemCategory.DataSource = {Facade.CategoryFacade.GetById(Integer.Parse(defaultCategoryBehaviour.AttributeValue))}
                ItemCategory.DataBind()
            End If
        End Sub

#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdImport, MainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdImport, cmdImport, MasterDocSuite.AjaxFlatLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, lblFields)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, pnlPublishingDate)
            AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, ItemCategory)
            MasterDocSuite.ScriptManager.AsyncPostBackTimeout = 120
        End Sub

        Private Sub Initialize()
            DocumentSeries.Text = ProtocolEnv.DocumentSeriesName

            ' Carico i contenitori abilitati all'operatore
            Dim rights As List(Of Integer) = New List(Of Integer)({DocumentSeriesContainerRightPositions.Insert, DocumentSeriesContainerRightPositions.Draft})
            Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, rights, True)

            ItemPublishingDate.SelectedDate = Date.Now
            ddlDocumentSeries.DataValueField = "Id"
            ddlDocumentSeries.DataTextField = "Name"
            ddlDocumentSeries.DataSource = availableContainers
            ddlDocumentSeries.DataBind()
            ddlDocumentSeries.Items.Insert(0, "")

            ' Se è disponibile un singolo contenitore lo seleziono di default
            If availableContainers.Count = 1 Then
                ddlDocumentSeries.SelectedValue = availableContainers(0).Id.ToString()
            End If
        End Sub


        Private Function ParseDocumentString(docs As String) As List(Of DocumentInfo)
            Dim tor As New List(Of DocumentInfo)
            For Each token As String In docs.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
                If File.Exists(token) Then
                    tor.Add(New FileDocumentInfo(New FileInfo(token)))
                Else
                    Throw New DocSuiteException("Importazione Excel", String.Format("File [{0}] non trovato.", token))
                End If
            Next
            Return tor
        End Function

#End Region

    End Class
End Namespace