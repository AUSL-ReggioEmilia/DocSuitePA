Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders

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
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            txtYear.Text = Date.Now.Year.ToString()
            rowContainer.Visible = False

            If DefaultCategoryId.HasValue Then
                Dim currentCategory As Category = Facade.CategoryFacade.GetById(DefaultCategoryId.Value)
                UscClassificatore1.DataSource = New List(Of Category) From {currentCategory}
                UscClassificatore1.DataBind()
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

    Protected Sub OnCategoryAdded(ByVal sender As Object, ByVal e As EventArgs) Handles UscClassificatore1.CategoryAdded
        VisibleCategorySearch = True
        AjaxManager.ResponseScripts.Add("VisibleCategorySearch()")
    End Sub

    Protected Sub OnCategoryRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles UscClassificatore1.CategoryRemoved
        If Not UscClassificatore1.HasSelectedCategories Then
            VisibleCategorySearch = False
            AjaxManager.ResponseScripts.Add("HideCategorySearch()")
        End If
    End Sub

    Protected Sub UscClassificatore1_CategoryAdding(sender As Object, args As EventArgs) Handles UscClassificatore1.CategoryAdding
        UscClassificatore1.Year = Nothing
        If Not String.IsNullOrEmpty(txtYear.Text) Then
            UscClassificatore1.Year = Convert.ToInt32(txtYear.Text)
        End If

        UscClassificatore1.FromDate = Nothing
        If txtStartDateFrom.SelectedDate.HasValue Then
            UscClassificatore1.FromDate = txtStartDateFrom.SelectedDate.Value
        End If

        If txtEndDateFrom.SelectedDate.HasValue Then
            If UscClassificatore1.FromDate Is Nothing OrElse UscClassificatore1.FromDate > txtEndDateFrom.SelectedDate Then
                UscClassificatore1.FromDate = txtEndDateFrom.SelectedDate
            End If
        End If

        UscClassificatore1.ToDate = Nothing
        If txtStartDateTo.SelectedDate.HasValue Then
            UscClassificatore1.ToDate = txtStartDateTo.SelectedDate.Value
        End If

        If txtEndDateTo.SelectedDate.HasValue Then
            If UscClassificatore1.FromDate Is Nothing OrElse UscClassificatore1.ToDate > txtEndDateTo.SelectedDate Then
                UscClassificatore1.ToDate = txtEndDateTo.SelectedDate
            End If
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeContainers()
        Dim containers As ICollection(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("FASCICLE", Convert.ToInt16(True))
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
        finderModel.StartDateTo = txtStartDateTo.SelectedDate
        'Data chiusura
        finderModel.EndDateFrom = txtEndDateFrom.SelectedDate
        finderModel.EndDateTo = txtEndDateTo.SelectedDate

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
        If UscClassificatore1.HasSelectedCategories Then
            finderModel.Classifications = UscClassificatore1.SelectedCategories.First().FullIncrementalPath
            finderModel.IncludeChildClassifications = chbCategoryChild.Checked
        End If

        'Settori
        If uscSettori.HasSelectedRole Then
            finderModel.Roles = uscSettori.RoleListAdded.ToList()
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

        finderModel.ApplySecurity = ProtocolEnv.IsSecurityEnabled OrElse ProtocolEnv.SearchOnlyAuthorizedFasciclesEnabled
        _finder.EnablePaging = False
        _finder.FascicleFinderModel = finderModel
        _finder.FromPostMethod = True
    End Sub

#End Region

End Class