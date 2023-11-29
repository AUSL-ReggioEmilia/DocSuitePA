Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class uscFascicleFinder
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Dim _finder As FascicleFinder = New FascicleFinder(DocSuiteContext.Current.Tenants)
    Dim _hasInteropSearch As Boolean
    Dim _hasMetadataSerach As Boolean
    Dim _hasVisibleVisibilityType As Boolean
    Dim _contacts As IList(Of ContactDTO)
#End Region

#Region "Properties"
    Public ReadOnly Property Count() As Long
        Get
            Return _finder.Count()
        End Get
    End Property

    Public Property VisibleInteropSearch() As Boolean
        Get
            Return _hasInteropSearch
        End Get
        Set(ByVal value As Boolean)
            _hasInteropSearch = value
            rowInterop.Visible = value
        End Set
    End Property

    Public Property VisibleCategorySearch() As Boolean
        Get
            If rowCategorySearch.Style("display") = "none" Then
                Return False
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                rowCategorySearch.Style.Add("display", "")
            Else
                rowCategorySearch.Style.Add("display", "none")
            End If
        End Set
    End Property

    Public Property VisibleMetadataSearch() As Boolean
        Get
            Return _hasMetadataSerach
        End Get
        Set(value As Boolean)
            _hasMetadataSerach = value
            pnlMetadata.Visible = value
        End Set
    End Property


    Public Property VisibleVisibilityTypeCheck() As Boolean
        Get
            Return _hasVisibleVisibilityType
        End Get
        Set(value As Boolean)
            _hasVisibleVisibilityType = value
            trVisibility.Visible = value
        End Set
    End Property

    Public Property PageIndex() As Integer
        Get
            Return _finder.PageIndex
        End Get
        Set(ByVal value As Integer)
            _finder.PageIndex = value
        End Set
    End Property

    Public Property PageSize() As Integer
        Get
            Return _finder.PageSize
        End Get
        Set(ByVal value As Integer)
            _finder.PageSize = value
        End Set
    End Property

    Public ReadOnly Property Finder() As FascicleFinder
        Get
            BindData()
            Return _finder
        End Get
    End Property

    Public Property DefaultCategoryId As Integer?

    Public ReadOnly Property UscMetadataRepositorySelId As String
        Get
            Return uscMetadataRepositorySel.PageContentDiv.ClientID
        End Get
    End Property

    Public Property CurrentMetadataValue As String
    Public Property CurrentMetadataValues As ICollection(Of MetadataFinderModel)

    Public Property FinderCategoryId As Integer?
        Get
            If ViewState(String.Format("{0}_FinderCategoryId", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_FinderCategoryId", ID)), Integer)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Integer?)
            ViewState(String.Format("{0}_FinderCategoryId", ID)) = value
        End Set
    End Property
    Public Property FinderProcessId As Guid?
        Get
            If ViewState(String.Format("{0}_FinderProcessId", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_FinderProcessId", ID)), Guid)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Guid?)
            ViewState(String.Format("{0}_FinderProcessId", ID)) = value
        End Set
    End Property

    Public Property FinderProcessName As String
        Get
            If ViewState(String.Format("{0}_FinderProcessName", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_FinderProcessName", ID)), String)
            End If
            Return Nothing
        End Get
        Set(ByVal value As String)
            ViewState(String.Format("{0}_FinderProcessName", ID)) = value
        End Set
    End Property
    Public Property FinderDossierFolderId As Guid?
        Get
            If ViewState(String.Format("{0}_FinderDossierFolderId", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_FinderDossierFolderId", ID)), Guid)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Guid?)
            ViewState(String.Format("{0}_FinderDossierFolderId", ID)) = value
        End Set
    End Property

    Public Property FinderDossierFolderName As String
        Get
            If ViewState(String.Format("{0}_FinderDossierFolderName", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_FinderDossierFolderName", ID)), String)
            End If
            Return Nothing
        End Get
        Set(ByVal value As String)
            ViewState(String.Format("{0}_FinderDossierFolderName", ID)) = value
        End Set
    End Property

    Public Property DynamicMetadataEnabled As Boolean = False
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            txtYear.Text = Date.Now.Year.ToString()
            rowContainer.Visible = False

            uscCategoryRest.ShowProcesses = ProtocolEnv.ProcessEnabled
            uscCategoryRest.HideTitle = True
            uscCategoryRest.ProcessNodeSelectable = True
            uscCategoryRest.AjaxRequestEnabled = True
            uscCategoryRest.IsProcessActive = True
            If ProtocolEnv.ProcessEnabled Then
                uscCategoryRest.FascicleType = Model.Entities.Fascicles.FascicleType.Procedure
                uscCategoryRest.ShowAuthorizedFascicolable = True
            End If
            If DefaultCategoryId.HasValue Then
                FinderCategoryId = DefaultCategoryId.Value
                uscCategoryRest.DefaultCategoryId = DefaultCategoryId.Value
            End If

            If ProtocolEnv.FascicleContainerEnabled Then
                rowContainer.Visible = True
                InitializeContainers()
            End If
        End If
        VisibleInteropSearch = DocSuiteContext.Current.ProtocolEnv.IsInteropEnabled
        VisibleMetadataSearch = DocSuiteContext.Current.ProtocolEnv.MetadataRepositoryEnabled
        VisibleVisibilityTypeCheck = DocSuiteContext.Current.ProtocolEnv.ShowVisibilityTypeInFascicleSearch
        rowCategorySearch.Attributes.Add("style", "display:none")
        uscSettori.RoleRestictions = RoleRestrictions.OnlyMine
        lblRP.Text = DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel
        uscMetadataRepositorySel.SetiContactVisibilityButton = False
    End Sub

    Public Sub Category_EntityAdded(ByVal sender As Object, ByVal args As List(Of String)) Handles uscCategoryRest.EntityAdded
        Dim entityType As String = args(0)
        FinderCategoryId = Nothing
        FinderProcessId = Nothing
        FinderDossierFolderId = Nothing

        Select Case entityType
            Case "Category"
                FinderCategoryId = Integer.Parse(args(1))
            Case "Process"
                FinderProcessId = Guid.Parse(args(1))
                FinderProcessName = args(2)
            Case "DossierFolder"
                FinderDossierFolderId = Guid.Parse(args(1))
                FinderDossierFolderName = args(2)
        End Select
    End Sub

    Public Sub Category_EntityRemoved(ByVal sender As Object, ByVal args As List(Of String)) Handles uscCategoryRest.EntityRemoved
        FinderCategoryId = Nothing
        FinderProcessId = Nothing
        FinderDossierFolderId = Nothing
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeContainers()
        Dim containers As ICollection(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("FASCICLE", True)
        rdlContainers.DataSource = containers
        rdlContainers.DataBind()
        rdlContainers.Items.Insert(0, New DropDownListItem(String.Empty, String.Empty))
    End Sub

    Private Sub BindData()
        Dim finderModel As FascicleFinderModel = New FascicleFinderModel()
        If Not String.IsNullOrEmpty(txtYear.Text) Then
            finderModel.Year = Convert.ToInt16(txtYear.Text.Trim())
        End If

        If Not String.IsNullOrEmpty(txtNumber.Text) Then
            finderModel.Title = txtNumber.Text.Trim
        End If

        'Data Apertura
        finderModel.StartDateFrom = txtStartDateFrom.SelectedDate
        If (txtStartDateTo.SelectedDate.HasValue) Then
            finderModel.StartDateTo = txtStartDateTo.SelectedDate.Value.EndOfTheDay()
        End If
        'Data chiusura

        finderModel.EndDateFrom = txtEndDateFrom.SelectedDate
        If (txtEndDateTo.SelectedDate.HasValue) Then
            finderModel.EndDateTo = txtEndDateTo.SelectedDate.Value.EndOfTheDay()
        End If
        'Oggetto
        finderModel.Subject = txtObjectProtocol.Text.Trim()
        Select Case rblClausola.SelectedValue
            Case "AND"
                finderModel.SubjectSearchStrategy = NHibernateBaseFinder(Of Fascicle, Fascicle).ObjectSearchType.AllWords
            Case "OR"
                finderModel.SubjectSearchStrategy = NHibernateBaseFinder(Of Fascicle, Fascicle).ObjectSearchType.AtLeastWord
        End Select

        Dim rblFascicleStatevalue As Integer = DirectCast([Enum].Parse(GetType(FascicleStatus), rblFascicleState.SelectedValue), FascicleStatus)
        If rblFascicleStatevalue > 0 Then
            finderModel.FascicleStatus = rblFascicleStatevalue
        End If

        finderModel.Note = txtNote.Text.Trim()

        'Visibilità
        If VisibleVisibilityTypeCheck Then
            finderModel.ViewConfidential = Confidential.Checked
            finderModel.ViewAccessible = Accessible.Checked
        End If

        If Not String.IsNullOrEmpty(rdlContainers.SelectedValue) Then
            finderModel.Container = Short.Parse(rdlContainers.SelectedValue)
        End If

        'Classificatore
        If FinderCategoryId.HasValue Then
            finderModel.Classifications = Facade.CategoryFacade.GetById(FinderCategoryId.Value).FullIncrementalPath
        End If

        If FinderProcessId.HasValue Then
            finderModel.IdProcess = FinderProcessId.Value
            finderModel.ProcessLabel = FinderProcessName
        End If


        If FinderDossierFolderId.HasValue Then
            finderModel.IdDossierFolder = FinderDossierFolderId.Value
            finderModel.DossierFolderLabel = FinderDossierFolderName
        End If

        'Settori
        If uscSettori.HasSelectedRole Then
            finderModel.Roles = uscSettori.RoleListAdded.ToList()
        End If

        If uscSettoriResp.HasSelectedRole Then
            finderModel.MasterRole = uscSettoriResp.RoleListAdded.Single()
        End If

        ''INTEROP
        If VisibleInteropSearch Then
            _contacts = UscRiferimentoSel1.GetContacts(False)

            If _contacts.Count > 0 Then
                finderModel.Manager = _contacts(0).Contact.FullIncrementalPath
            End If
        End If

        'Metadati
        If VisibleMetadataSearch Then
            finderModel.IdMetadataRepository = uscMetadataRepositorySel.CurrentMetadataRepository
            finderModel.MetadataValue = CurrentMetadataValue
            finderModel.MetadataValues = CurrentMetadataValues
        End If

        If uscMetadataRepositorySel.CurrentMetadataRepository IsNot Nothing Then
            DynamicMetadataEnabled = True
        End If

        finderModel.ApplySecurity = ProtocolEnv.IsSecurityEnabled OrElse ProtocolEnv.SearchOnlyAuthorizedFasciclesEnabled
        _finder.EnablePaging = False
        _finder.FascicleFinderModel = finderModel
        _finder.FromPostMethod = True
    End Sub

#End Region

End Class