Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ProtIncompleti
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            txtYear.Text = DateTime.Today.Year.ToString()
            DoSearch()
        End If
    End Sub

    Private Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        If gvProtocols.DataSource IsNot Nothing Then
            Title = String.Format("Protocolli incompleti - Risultati ({0}/{1})", gvProtocols.DataSource.Count, gvProtocols.VirtualItemCount)
        Else
            Title = "Protocolli incompleti - Nessun Risultato"
        End If
        MasterDocSuite.Title = Title
        MasterDocSuite.HistoryTitle = Title
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        DoSearch()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, MasterDocSuite.TitleContainer)

        AddHandler uscProtocolGrid.Grid.DataBound, AddressOf DataSourceChanged
    End Sub

    Private Sub DoSearch()
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        Dim finder As NHibernateProtocolFinder = New NHibernateProtocolFinder("ProtDB")
        finder.IdStatus = ProtocolStatusId.Incompleto
        CommonInstance.ApplyProtocolFinderSecurity(finder, SecurityType.Read, CurrentTenant.TenantAOO.UniqueId, True)

        Dim year As Short
        If Short.TryParse(txtYear.Text, year) Then
            finder.Year = year
        End If

        gvProtocols.PageSize = finder.PageSize
        gvProtocols.MasterTableView.SortExpressions.Clear()

        If ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
            gvProtocols.MasterTableView.SortExpressions.AddSortExpression("RegistrationDate ASC")
            gvProtocols.MasterTableView.AllowMultiColumnSorting = True
        End If

        gvProtocols.Finder = finder
        gvProtocols.DataBindFinder()
        gvProtocols.Visible = True
        gvProtocols.MasterTableView.AllowMultiColumnSorting = False
    End Sub

    Protected Overridable Sub SelectOrDeselectAll(ByVal selected As Boolean)
        For Each item As GridDataItem In uscProtocolGrid.Grid.Items
            With DirectCast(item.FindControl("cbSelect"), CheckBox)
                If .Enabled AndAlso (Not selected OrElse item.Visible) Then
                    .Checked = selected
                End If
            End With
        Next
    End Sub

#End Region

End Class