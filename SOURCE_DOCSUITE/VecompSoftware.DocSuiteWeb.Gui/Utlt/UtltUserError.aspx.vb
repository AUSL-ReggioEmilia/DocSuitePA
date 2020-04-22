
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltUserError
    Inherits CommonBasePage

#Region "Finder"
    Dim _finder As NHibernateUserErrorFinder
    Private ReadOnly Property Finder() As NHibernateUserErrorFinder
        Get
            If _finder Is Nothing Then
                _finder = Facade.UserErrorFinder
            End If
            Return _finder
        End Get
    End Property
#End Region

#Region "Page Load"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        Initialize()
    End Sub
#End Region

#Region "Initialize"
    Private Sub InitializeAjaxSettings()
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(gvUserError, gvUserError, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, gvUserError, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, lblCounter, Nothing)
    End Sub

    Private Sub Initialize()
        rdpErrorDate.Focus()
    End Sub
#End Region

#Region "Private Functions"
    Private Sub BindData()
        Finder.ErrorDate = rdpErrorDate.SelectedDate
        Finder.SystemServer = txtSystemServer.Text
        Finder.SystemUser = txtSystemUser.Text
    End Sub
#End Region

#Region "Buttons Events"
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        'popolo il finder con i dati da estrapolare
        Me.BindData()
        gvUserError.Finder = Finder
        gvUserError.MasterTableView.SortExpressions.AddSortExpression("Id DESC")
        gvUserError.DataBindFinder()
        gvUserError.Visible = True
        lblCounter.Text = Finder.Count
    End Sub
#End Region
    
    
End Class