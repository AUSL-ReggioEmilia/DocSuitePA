Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltServiceLog
    Inherits CommonBasePage

    Public tExtr As Label

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tExtr = txtTbltEstratto
        InitializeAjaxSettings()
        If Not Me.IsPostBack Then
            Initialize()
        End If

    End Sub

#Region "Finder"
    Dim _finder As NHibernateServiceLogFinder
    Private ReadOnly Property Finder() As NHibernateServiceLogFinder
        Get
            If _finder Is Nothing Then
                _finder = Facade.ServiceLogFinder
            End If
            Return _finder
        End Get
    End Property
#End Region

#Region "Initialize"
    Private Sub InitializeAjaxSettings()
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(gvServiceLog, gvServiceLog, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, gvServiceLog, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, txtTbltEstratto, Nothing)
    End Sub
#End Region

    Private Sub BindData()
        Finder.DateFrom = rdpDate_From.SelectedDate
        Finder.DateTo = rdpDate_To.SelectedDate
        If String.IsNullOrEmpty(ddlLevel.SelectedValue) Then
            Finder.Level = Nothing
        Else
            Finder.Level = Int16.Parse(ddlLevel.SelectedValue)
        End If
        Finder.Session = txtSession.Text
        Finder.Text = txtText.Text
    End Sub

    Private Sub Initialize()
        txtTotal.Text = Facade.ServiceLogFinder.CountAll()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        'popolo il finder con i dati da estrapolare
        Me.BindData()
        txtTbltEstratto.Text = Finder.Count
        gvServiceLog.Finder = Finder
        gvServiceLog.DataBindFinder()
        gvServiceLog.Visible = True
    End Sub
End Class
