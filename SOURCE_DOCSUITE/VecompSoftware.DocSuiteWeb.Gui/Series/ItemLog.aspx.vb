Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ItemLog
    Inherits CommonBasePage

#Region " Fields "
    Private _currentDocumentSeriesItem As DocumentSeriesItem
#End Region

#Region " Properties "
    Private ReadOnly Property CurrentDocumentSeriesItem As DocumentSeriesItem
        Get
            If _currentDocumentSeriesItem Is Nothing Then
                Dim idDsi As Integer? = Request.QueryString.GetValueOrDefault(Of Integer?)("IdDocumentSeriesItem", Nothing)
                If idDsi.HasValue Then
                    _currentDocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(idDsi.Value)
                End If
            End If
            Return _currentDocumentSeriesItem
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Title = String.Format("{0} - {1}/{2}", CurrentDocumentSeriesItem.DocumentSeries.Container.Name, CurrentDocumentSeriesItem.Year, CurrentDocumentSeriesItem.Number)
        InitializeAjaxSettings()
        If Not Page.IsPostBack Then
            Initialize()
        End If

    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        Dim finder As New NHibernateDocumentSeriesItemLogFinder
        finder.DocumentSeriesItem = CurrentDocumentSeriesItem

        GridLog.Finder = finder
        GridLog.PageSize = finder.PageSize
        GridLog.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
        GridLog.DataBindFinder()
    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(GridLog, GridLog, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub
#End Region

End Class