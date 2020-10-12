Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.SessionState
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Model.Entities.UDS
Imports VecompSoftware.DocSuiteWeb.UDSDesigner
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web.RadGrid
Imports VecompSoftware.Helpers.XML
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.DocumentsService
Imports VecompSoftware.Services.Logging
Imports FacadeWebAPI = VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS

Public Class DesignerService
    Inherits CommBasePage

#Region "Fields"
    Private Const CONTROL_TITLE As String = "Title"
    Private Const CONTROL_DOCUMENT As String = "Document"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private Shared _currentUDSSchemaRepository As UDSSchemaRepository = Nothing
    Private Shared _udsRepositoryFacade As UDSRepositoryFacade = Nothing
    Private Shared _udsRepositoryWebAPIFacade As FacadeWebAPI.UDSRepositoryFacade = Nothing
#End Region

#Region "Filter Name"

#End Region

#Region "Properties"
    Protected Shared ReadOnly Property CurrentUDSSchemaRepository As UDSSchemaRepository
        Get
            If (_currentUDSSchemaRepository Is Nothing) Then
                _currentUDSSchemaRepository = New UDSSchemaRepositoryFacade(DocSuiteContext.Current.User.FullUserName).GetCurrentSchema()
            End If
            Return _currentUDSSchemaRepository
        End Get
    End Property

    Public Shared ReadOnly Property UDSRepositoryWebAPIFacade As FacadeWebAPI.UDSRepositoryFacade
        Get
            If _udsRepositoryWebAPIFacade Is Nothing Then
                _udsRepositoryWebAPIFacade = New FacadeWebAPI.UDSRepositoryFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
            End If
            Return _udsRepositoryWebAPIFacade
        End Get
    End Property

    Public Shared ReadOnly Property UDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _udsRepositoryFacade Is Nothing Then
                _udsRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _udsRepositoryFacade
        End Get
    End Property

    Public Overloads Shared ReadOnly Property CurrentTenant As Tenant
        Get
            If HttpContext.Current.Session("CurrentTenant") IsNot Nothing Then
                Return DirectCast(HttpContext.Current.Session("CurrentTenant"), Tenant)
            End If
            Return Nothing
        End Get
    End Property
#End Region

#Region "Constructor"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
#End Region

#Region "Web Methods"
    <WebMethod>
    Public Shared Function LoadContactTypes() As Object
        Dim UDS As UDSConverter = New UDSConverter()
        Dim contactTypeLabels As ICollection(Of String) = UDS.GetContactTypeDescriptionLabels()
        Return contactTypeLabels
    End Function

    <WebMethod>
    Public Shared Function LoadStatuses(context As RadComboBoxContext) As RadComboBoxData
        Dim results As IList(Of RadComboBoxItemData) = New List(Of RadComboBoxItemData)
        results.Add(New RadComboBoxItemData() With {.Text = "Bozza", .Value = "1"})
        results.Add(New RadComboBoxItemData() With {.Text = "Confermata", .Value = "2"})
        Dim comboResult As RadComboBoxData = New RadComboBoxData()
        comboResult.Items = results.ToArray()
        Return comboResult
    End Function

    <WebMethod>
    Public Shared Function LoadBiblosArchives() As Object
        Dim archives As Archive() = Service.GetArchives()
        Return archives.Select(Function(s) s.Name).OrderBy(Function(o) o).ToArray()
    End Function

    <WebMethod>
    Public Shared Function LoadRepositories(startRowIndex As Integer, maximumRows As Integer, sortExpression As ICollection(Of GridSortExpression),
                                            filterExpression As ICollection(Of GridFilterExpression)) As Object
        Dim repositoryFinder As UDSRepositoryFinder = New UDSRepositoryFinder(New UDSRepositoryModelMapper(), DocSuiteContext.Current.User.FullUserName)
        Dim expressionFilterHelper As RadGridExpressionFilterHelper(Of UDSRepository) = New RadGridExpressionFilterHelper(Of UDSRepository)
        Dim criteriaFilterHelper As RadGridCriteriaFilterHelper(Of UDSRepository) = New RadGridCriteriaFilterHelper(Of UDSRepository)
        Dim sortHelper As RadGridSortExpressionHelper = New RadGridSortExpressionHelper()
        Dim repositoryModelMapper As UDSRepositoryModelMapper = New UDSRepositoryModelMapper()
        Dim filterResult As Object
        For Each filter As GridFilterExpression In filterExpression
            Select Case filter.FilterFunction
                Case RadGridFilterHelper.CONTAINS, RadGridFilterHelper.ENDS_WITH, RadGridFilterHelper.NOT_CONTAINS, RadGridFilterHelper.STARTS_WITH
                    filterResult = criteriaFilterHelper.GetFilter(filter)
                Case Else
                    If filter.ColumnUniqueName.Eq("Status") Then
                        filterResult = criteriaFilterHelper.GetFilter(filter)
                    Else
                        filterResult = expressionFilterHelper.GetFilter(filter)
                    End If
            End Select
            If filterResult Is Nothing Then
                Continue For
            End If
            repositoryFinder.AddFilterExpression(filterResult)
        Next

        repositoryFinder.SortExpressionsClear()
        For Each sort As GridSortExpression In sortExpression
            If sort.SortOrder.Equals(GridSortOrder.Ascending) Then
                repositoryFinder.SortExpressions.Add(New SortExpression(Of UDSRepository) With {.Direction = SortDirection.Ascending, .Expression = sortHelper.GetSortExpression(Of UDSRepository)(sort)})
            Else
                repositoryFinder.SortExpressions.Add(New SortExpression(Of UDSRepository) With {.Direction = SortDirection.Descending, .Expression = sortHelper.GetSortExpression(Of UDSRepository)(sort)})
            End If
        Next

        If Not sortExpression.Count > 0 Then
            repositoryFinder.SortExpressions.Add(New SortExpression(Of UDSRepository) With {.Direction = SortDirection.Ascending,
                                                 .Expression = sortHelper.GetSortExpression(Of UDSRepository)(New GridSortExpression() With {.FieldName = "Name", .SortOrder = GridSortOrder.Ascending})})
        End If

        Dim remainder As Integer
        repositoryFinder.PageSize = 20
        repositoryFinder.CustomPageIndex = Math.DivRem(startRowIndex, 20, remainder)
        repositoryFinder.EnablePaging = True
        Dim repositories As ICollection(Of UDSRepositoryModel) = repositoryFinder.DoSearchHeader()
        Dim countElements As Integer = repositoryFinder.Count()
        Return New With {.Count = countElements, .Data = repositories}
    End Function

    <WebMethod>
    Public Shared Function LoadRepositoryById(idRepository As Guid) As Object
        Dim repository As UDSRepository = UDSRepositoryFacade.GetById(idRepository)
        Dim udsModel As UDSModel = UDSModel.LoadXml(repository.ModuleXML)
        Dim converter As UDSConverter = New UDSConverter()
        Dim jsModel As JsModel = converter.ConvertToJs(udsModel)
        Dim titleCtrl As Element = jsModel.elements.SingleOrDefault(Function(x) x.ctrlType.Equals(CONTROL_TITLE))
        If titleCtrl Is Nothing Then
            Throw New DocSuiteException("VecompSoftware.DocSuiteWeb.Gui.UdsDesigner.DesignerService [ERROR]: UDSModel non corretto")
        End If

        titleCtrl.titleReadOnly = repository.Version > 0
        titleCtrl.idRepository = repository.Id
        titleCtrl.DSWEnvironment = repository.DSWEnvironment
        If titleCtrl.idCategory IsNot Nothing AndAlso Not String.IsNullOrEmpty(titleCtrl.idCategory) Then
            Dim category As Data.Category = FacadeFactory.Instance.CategoryFacade.GetById(Integer.Parse(titleCtrl.idCategory))
            titleCtrl.categoryName = category.GetFullName()
        End If

        titleCtrl.containerReadOnly = repository.Version > 0 AndAlso titleCtrl.idContainer IsNot Nothing
        If titleCtrl.idContainer IsNot Nothing AndAlso Not String.IsNullOrEmpty(titleCtrl.idContainer) Then
            Dim container As Data.Container = FacadeFactory.Instance.ContainerFacade.GetById(Integer.Parse(titleCtrl.idContainer))
            titleCtrl.containerName = container.Name
        End If

        Dim documentCtrls As ICollection(Of Element) = jsModel.elements.Where(Function(x) x.ctrlType.Eq(CONTROL_DOCUMENT)).ToList()
        For Each ctrl As Element In documentCtrls
            ctrl.archiveReadOnly = repository.Version > 0 AndAlso Not ctrl.archive.Eq("Archivio")
        Next
        Return jsModel
    End Function

    <WebMethod>
    Public Shared Function SaveModel(jsModel As JsModel, activeDate As DateTimeOffset, publish As Boolean) As String
        Try
            Dim converter As UDSConverter = New UDSConverter()
            Dim udsModel As UDSModel = converter.ConvertFromJson(jsModel)
            Dim titleCtrl As Element = jsModel.elements.SingleOrDefault(Function(x) x.ctrlType.Equals(CONTROL_TITLE))
            If titleCtrl Is Nothing Then
                Throw New DocSuiteException("VecompSoftware.DocSuiteWeb.Gui.UdsDesigner.DesignerService [ERROR]: UDSModel non corretto")
            End If

            Dim udsRepository As UDSRepository = UDSRepositoryFacade.GetMaxVersionByName(titleCtrl.label)
            If udsRepository IsNot Nothing AndAlso udsRepository.DSWEnvironment <> titleCtrl.DSWEnvironment Then
                Throw New DocSuiteException("VecompSoftware.DocSuiteWeb.Gui.UdsDesigner.DesignerService [ERROR]: Repository già presente con il Titolo passato")
            End If

            If String.IsNullOrEmpty(titleCtrl.idContainer) AndAlso Not titleCtrl.createContainer Then
                Throw New DocSuiteException("VecompSoftware.DocSuiteWeb.Gui.UdsDesigner.DesignerService [ERROR]: Deve essere inserito un contenitore valido")
            End If

            UDSRepositoryWebAPIFacade.SaveRepository(udsModel, activeDate, titleCtrl.idRepository, publish)
            Return "ok"
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Throw New Exception(ex.Message)
        End Try
    End Function

    <WebMethod>
    Public Shared Function FindCategoryById(idCategory As Integer) As Object
        Dim category As Data.Category = FacadeFactory.Instance.CategoryFacade.GetById(idCategory)
        If category Is Nothing Then
            Throw New DocSuiteException("VecompSoftware.DocSuiteWeb.Gui.UdsDesigner.DesignerService [ERROR]: Nessun classificatore trovato per l'ID passato")
        End If

        Return New With {
            .Id = category.Id,
            .Name = category.Name
        }
    End Function

    <WebMethod>
    Public Shared Function FindCategoryByDescription(description As String) As Object
        If String.IsNullOrEmpty(description) Then
            Return LoadRootCategories()
        End If
        Dim categories As IList(Of Data.Category) = FacadeFactory.Instance.CategoryFacade.GetCategoryByDescription(description, 1)
        If Not categories.Any() Then
            Return String.Empty
        End If

        Dim udsFacade As UDSTreeViewFacade = New UDSTreeViewFacade()
        Dim state As UDSTreeViewState = New UDSTreeViewState() With {
            .Checked = False,
            .Disabled = False,
            .Opened = True,
            .Selected = False
        }

        Dim results As ICollection(Of UDSTreeViewDto) = udsFacade.GetCategoryTreeView(categories, state, False)
        Return JsonConvert.SerializeObject(results, Formatting.Indented)
    End Function

    <WebMethod>
    Public Shared Function LoadRootCategories() As Object
        Dim categoryFinder As CategoryFinder = New CategoryFinder(New MapperCategoryModel(), DocSuiteContext.Current.User.FullUserName) With {
            .EnablePaging = False,
            .IsActive = True,
            .IncludeZeroLevel = False
        }
        Dim categories As ICollection(Of Data.Category) = categoryFinder.DoSearch()
        If Not categories.Any() Then
            Return String.Empty
        End If

        Dim udsFacade As UDSTreeViewFacade = New UDSTreeViewFacade()
        Dim results As ICollection(Of UDSTreeViewDto) = udsFacade.GetCategoryTreeView(categories)
        Return JsonConvert.SerializeObject(results, Formatting.Indented)
    End Function

    <WebMethod>
    Public Shared Function LoadChildCategories(idCategory As Integer) As Object
        Dim categories As IList(Of Data.Category) = FacadeFactory.Instance.CategoryFacade.GetCategoryByParentId(idCategory)
        If Not categories.Any() Then
            Return String.Empty
        End If

        Dim udsFacade As UDSTreeViewFacade = New UDSTreeViewFacade()
        Dim results As ICollection(Of UDSTreeViewDto) = udsFacade.GetCategoryTreeView(categories)
        Return JsonConvert.SerializeObject(results, Formatting.Indented)
    End Function

    <WebMethod>
    Public Shared Function FindContainerById(idContainer As Integer) As Object
        Dim container As Data.Container = FacadeFactory.Instance.ContainerFacade.GetById(idContainer)
        If container Is Nothing Then
            Throw New DocSuiteException("VecompSoftware.DocSuiteWeb.Gui.UdsDesigner.DesignerService [ERROR]: Nessun contenitore trovato per l'ID passato")
        End If

        Return New With {
            .Id = container.Id,
            .Name = container.Name
        }
    End Function

    <WebMethod>
    Public Shared Function FindContainerByDescription(description As String) As Object
        If String.IsNullOrEmpty(description) Then
            Return LoadContainers()
        End If
        Dim containers As IList(Of Data.Container) = FacadeFactory.Instance.ContainerFacade.GetAllRightsDistinct("UDS", 1)
        containers = FacadeFactory.Instance.ContainerFacade.FilterContainers(containers, description)
        If Not containers.Any() Then
            Return String.Empty
        End If

        Dim udsFacade As UDSTreeViewFacade = New UDSTreeViewFacade()
        Dim state As UDSTreeViewState = New UDSTreeViewState() With {
            .Checked = False,
            .Disabled = False,
            .Opened = True,
            .Selected = False
        }

        Dim results As ICollection(Of UDSTreeViewDto) = udsFacade.GetContainerTreeView(containers, state, False)
        Return JsonConvert.SerializeObject(results, Formatting.Indented)
    End Function

    <WebMethod>
    Public Shared Function LoadContainers() As Object
        Dim containers As IList(Of Data.Container) = FacadeFactory.Instance.ContainerFacade.GetAllRightsDistinct("UDS", 1)
        If Not containers.Any() Then
            Return String.Empty
        End If

        Dim udsFacade As UDSTreeViewFacade = New UDSTreeViewFacade()
        Dim results As ICollection(Of UDSTreeViewDto) = udsFacade.GetContainerTreeView(containers)
        Return JsonConvert.SerializeObject(results, Formatting.Indented)
    End Function

    <WebMethod>
    Public Shared Function LoadUDSRepositories() As Object
        Dim repositories As IList(Of UDSRepository) = UDSRepositoryFacade.GetActiveRepositories()
        Return repositories.Select(Function(s) s.Name).OrderBy(Function(o) o).ToArray()
    End Function

    <WebMethod>
    Public Shared Function LoadUDSFields(udsRepositoryName As String) As Object
        Dim repository As UDSRepository = UDSRepositoryFacade.GetMaxVersionByName(udsRepositoryName.Trim())
        If repository IsNot Nothing Then
            Dim udsModel As UDSModel = UDSModel.LoadXml(repository.ModuleXML)
            Return udsModel.Model.Metadata.Select(Function(s) s.Items.Select(Function(t) t.ColumnName)).OrderBy(Function(o) o).ToArray()
        End If
        Return Nothing
    End Function

    <WebMethod>
    Public Shared Function LoadTempModel() As Object
        Dim session As HttpSessionState = HttpContext.Current.Session
        If session IsNot Nothing Then
            Return session("tempModel")
        End If
        Return "error: Session is not working"
    End Function


    <WebMethod>
    Public Shared Function SaveTempModel(jsModel As JsModel) As String
        Dim session As HttpSessionState = HttpContext.Current.Session
        If session IsNot Nothing Then
            session("tempModel") = jsModel
            Return "ok"
        End If

        Return "error: Session is not working"
    End Function

    Private Shared Function ErrorFactoryMessage(err As String) As String
        If String.IsNullOrEmpty(err) Then
            Return String.Empty
        End If
        If err.Contains("http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd:Title") Then
            Return "E' necessario specificare un valore valido per la proprietà [TITOLO], ovvero una stringa di almeno tre caratteri ma non più grande di quaranta caretteri"
        End If
        If err.Contains("has invalid child element 'Alias' in namespace") Then
            Return "E' necessario specificare un valore valido per la proprietà [ALIAS], ovvero una stringa di almeno due caratteri ma non più grande di quattro caretteri"
        End If
        If err.Contains("http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd:Alias") Then
            Return "E' necessario specificare un valore valido per la proprietà [ALIAS], ovvero una stringa di almeno due caratteri ma non più grande di quattro caretteri"
        End If
        If err.Contains("The required attribute 'IdCategory' is missing") Then
            Return "E' necessario specificare un classificatore per l'archivio"
        End If
        If err.Contains("Attributo richiesto ""IdCategory"" mancante") Then
            Return "E' necessario specificare un classificatore per l'archivio"
        End If
        If err.Contains("The required attribute 'IdContainer' is missing") Then
            Return "E' necessario specificare un contenitore per l'archivio"
        End If
        If err.Contains("Attributo richiesto ""IdContainer"" mancante") Then
            Return "E' necessario specificare un contenitore per l'archivio"
        End If
        If err.Contains("The required attribute 'IconPath' is missing") Then
            Return "E' necessario specificare percorso di immagine per il valore nell'elemento 'Stato'"
        End If
        If err.Contains("Attributo richiesto ""IconPath"" mancante") Then
            Return "E' necessario specificare percorso di immagine per il valore nell'elemento 'Stato'"
        End If
        If err.Contains("expected: 'Documents'") OrElse err.Contains("expected: 'Document'") Then
            Return "E' necessario inserire un elemento 'Documento' della tipologia 'Documenti principali' per l'archivio"
        End If
        If err.Contains("elementi previsti: ""Documents""") OrElse err.Contains("elementi previsti: ""Document""") Then
            Return "E' necessario inserire un elemento 'Documento' della tipologia 'Documenti principali' per l'archivio"
        End If
        If err.Contains("expected: 'Option'") Then
            Return "E' necessario inserire dei valori nell'elemento 'Scelta multipla'"
        End If
        If err.Contains("expected: ""Option""") Then
            Return "E' necessario inserire dei valori nell'elemento 'Scelta multipla'"
        End If
        If err.Contains("expected: 'Options'") Then
            Return "E' necessario inserire dei valori nell'elemento 'Stato'"
        End If
        If err.Contains("expected: ""Options""") Then
            Return "E' necessario inserire dei valori nell'elemento 'Stato'"
        End If
        If err.Contains("Section_LabelUnique' key or unique identity constraint.") OrElse err.Contains("Section_ColumnNameUnique' key or unique identity constraint.") _
            OrElse err.Contains("Section_LabelUnique' o per il vincolo di identità") OrElse err.Contains("Section_ColumnNameUnique' o per il vincolo di identità") Then
            Dim str_search As String = "There is a duplicate key sequence '"
            Dim str_end As String = "' for the"
            Dim pos As Int32 = err.IndexOf(str_search)
            If (pos = -1) Then
                str_search = "Sequenza di chiave duplicata '"
                pos = err.IndexOf(str_search)
                str_end = "' per la chiave"
            End If
            Dim off As Integer = err.IndexOf(str_end)
            Dim fieldName As String = String.Empty
            If (pos >= 0 AndAlso off > pos) Then
                pos += str_search.Length
                fieldName = err.Substring(pos, off - pos)
            End If

            Return String.Concat("Sono stati riscontrati elementi con la stessa etichetta '", fieldName, "'. Ogni etichetta deve essere unica nell'archivio")
        End If

        Return err
    End Function



    <WebMethod>
    Public Shared Function ValidateModel(jsModel As JsModel) As Object
        Try
            Dim errors As List(Of String) = New List(Of String)()
            Dim converter As UDSConverter = New UDSConverter()
            Dim xml As String = converter.JsToXml(jsModel, errors)
            Dim hasError As Boolean
            Dim validator As XmlValidator = New XmlValidator()
            If (CurrentUDSSchemaRepository IsNot Nothing) Then
                Dim xmlSchema As String = CurrentUDSSchemaRepository.SchemaXML
                hasError = Not validator.ValidateXml(xml, xmlSchema, Utils.UDSNamespace)
                validator.Errors.AddRange(errors)
            Else
                validator.Errors.Add("Il modulo Archivi non è configurato correttamente a livello di installazione. E' neccessario contattare l'assistenza affinché procedano con l'adeguamento della tabella 'SchemaRepository'")
            End If
            Dim model As UDSModel = UDSModel.LoadXml(xml)
            If model.Model.Metadata IsNot Nothing Then
                Dim numberField As NumberField
                For Each medatada As Section In model.Model.Metadata
                    If medatada.Items IsNot Nothing Then
                        For Each item As FieldBaseType In medatada.Items
                            numberField = TryCast(item, NumberField)
                            If numberField IsNot Nothing AndAlso (numberField.MaxValueSpecified AndAlso numberField.MinValueSpecified AndAlso numberField.MaxValue < numberField.MinValue) Then
                                hasError = True
                                validator.Errors.Add($"Il valore massimo dell'elemento {item.Label} non può essere inferiore al valore minimo")
                            End If
                        Next
                    End If
                Next
            End If
            Return New With {
                Key .[error] = hasError,
                Key .messages = validator.Errors.Select(Function(f) ErrorFactoryMessage(f)).ToArray()
            }
        Catch ex As Exception
            Return New With {
                Key .[error] = True,
                Key .messages = New String() {ex.Message}
            }
        End Try
    End Function

    <WebMethod>
    Public Shared Function LoadRepositoryVersionByRepositoryId(idRepository As Guid) As Object
        Dim repository As UDSRepository = UDSRepositoryFacade.GetById(idRepository)
        Return New With {
            Key .Status = repository.Status,
            Key .Version = repository.Version
            }
    End Function

#End Region

End Class