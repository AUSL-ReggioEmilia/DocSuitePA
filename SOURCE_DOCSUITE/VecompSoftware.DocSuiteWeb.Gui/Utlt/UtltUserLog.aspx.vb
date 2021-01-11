Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltUserLog
    Inherits CommonBasePage

#Region " Properties "

    Dim _finder As NHibernateUserLogFinder
    Private ReadOnly Property Finder() As NHibernateUserLogFinder
        Get
            If _finder Is Nothing Then
                _finder = Facade.UserLogFinder
            End If
            Return _finder
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        rdpDateFrom.Focus()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        'popolo il finder con i dati da estrapolare
        Finder.LastOperationDateFrom = rdpDateFrom.SelectedDate
        Finder.LastOperationDateTo = rdpDateTo.SelectedDate
        Finder.SystemServer = txtSystemServer.Text
        Finder.SystemUserContains = txtSystemUser.Text

        Dim count As Integer = Finder.Count()
        lblCounter.Text = count.ToString()

        Finder.PageSize = count
        If count > 0 Then
            gvUserLog.PageSize = count
        End If
        gvUserLog.Finder = Finder
        gvUserLog.DataBindFinder()
        gvUserLog.Visible = True
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(gvUserLog, gvUserLog, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, gvUserLog, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, lblCounter, Nothing)
    End Sub

#End Region
    
End Class
