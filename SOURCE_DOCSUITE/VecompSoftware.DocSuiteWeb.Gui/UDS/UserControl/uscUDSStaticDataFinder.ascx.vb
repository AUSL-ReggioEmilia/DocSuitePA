Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.Helpers.UDS

Public Class uscUDSStaticDataFinder
    Inherits DocSuite2008BaseControl
    Implements IUDSFinderStaticData

#Region "Fields"

#End Region

#Region "Properties"
    Public ReadOnly Property IdCategory As Integer? Implements IUDSStaticData.IdCategory
        Get
            If uscSelCategory.SelectedCategories.Any() Then
                Return uscSelCategory.SelectedCategories.First().Id
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Subject As String Implements IUDSStaticData.Subject
        Get
            Return txtSubject.Text
        End Get
    End Property

    Public ReadOnly Property Year As Double? Implements IUDSFinderStaticData.Year
        Get
            If txtYear.SelectedDate.HasValue Then
                Return txtYear.SelectedDate.Value.Year
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Number As Double? Implements IUDSFinderStaticData.Number
        Get
            Return txtNumber.Value
        End Get
    End Property

    Public ReadOnly Property ViewDeletedUDS As Boolean Implements IUDSFinderStaticData.ViewDeletedUDS
        Get
            Return chkStatus.Checked
        End Get
    End Property

    Public ReadOnly Property RegistrationDateFrom As DateTimeOffset? Implements IUDSFinderStaticData.RegistrationDateFrom
        Get
            Return txtRegistrationDateFrom.SelectedDate
        End Get
    End Property

    Public ReadOnly Property RegistrationDateTo As DateTimeOffset? Implements IUDSFinderStaticData.RegistrationDateTo
        Get
            Return txtRegistrationDateTo.SelectedDate
        End Get
    End Property

    Public Property ActionType As String Implements IUDSStaticData.ActionType
        Get
            If ViewState(String.Format("{0}_ActionType", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_ActionType", ID)), String)
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_ActionType", ID)) = value
        End Set
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub uscSelCategory_CategoryAdding(sender As Object, args As EventArgs) Handles uscSelCategory.CategoryAdding
        uscSelCategory.Year = Nothing
        If Year.HasValue Then
            uscSelCategory.Year = Convert.ToInt32(Year.Value)
        End If

        uscSelCategory.FromDate = Nothing
        If RegistrationDateFrom.HasValue Then
            uscSelCategory.FromDate = RegistrationDateFrom.Value.Date
        End If

        uscSelCategory.ToDate = Nothing
        If RegistrationDateTo.HasValue Then
            uscSelCategory.ToDate = RegistrationDateTo.Value.Date
        End If
    End Sub
#End Region

#Region "Methods"

    Private Sub InitializeAjax()
        'todo: per future implementazioni
    End Sub

    Public Sub Initialize()
        txtYear.Focus()
        If DocSuiteContext.Current.ProtocolEnv.IsEnvSearchDefaultEnabled Then
            txtYear.SelectedDate = DateTime.Today
            txtNumber.Focus()
        End If
    End Sub

    Public Sub InitializeControls() Implements IUDSStaticData.InitializeControls
        'todo: per future implementazioni
    End Sub

    Public Sub ResetControls() Implements IUDSStaticData.ResetControls
        'todo: per future implementazioni
    End Sub

    Public Sub SetData(model As UDSModel) Implements IUDSStaticData.SetData
        'todo: per future implementazioni
    End Sub

    Public Sub SetUDSBehaviour(model As UDSModel) Implements IUDSStaticData.SetUDSBehaviour
        rowNumber.Visible = Not model.Model.HideRegistrationIdentifier
        rowYear.Visible = Not model.Model.HideRegistrationIdentifier
    End Sub

    Public Sub InitializeFilters(DetailsSearchModel As UDSRepositoryDetailsSearchModel)
        txtNumber.Value = DetailsSearchModel.Number
        If DetailsSearchModel.DateFrom.HasValue Then
            txtRegistrationDateFrom.SelectedDate = DetailsSearchModel.DateFrom.Value.Date
        End If
        If DetailsSearchModel.DateTo.HasValue Then
            txtRegistrationDateTo.SelectedDate = DetailsSearchModel.DateTo.Value.Date
        End If
        txtSubject.Text = DetailsSearchModel.Subject
        chkStatus.Checked = DetailsSearchModel.IsCancelledArchive
        uscSelCategory.CategoryID = DetailsSearchModel.CategoryId
        uscSelCategory.InitializeRadTreeCategory()
    End Sub

#End Region

End Class