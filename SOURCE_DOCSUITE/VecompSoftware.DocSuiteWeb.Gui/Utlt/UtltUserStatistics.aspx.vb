Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Class UtltUserStatistics
    Inherits CommonBasePage

#Region " Properties "

    Public ReadOnly Property CurrentType() As String
        Get
            Return Request.QueryString("LogType")
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, pnlRisultati, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, txtTotal, Nothing)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, lblSelected, Nothing)

        Select Case CurrentType
            Case "Prot"
                Title &= " Protocollo"

            Case "Docm"
                Title &= " " & DocSuiteContext.Current.DossierAndPraticheLabel
                RemoveProtocolColumns()

            Case "Resl"
                Dim titleText As String = "Resolutions"
                If Not String.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.Configuration) Then
                    titleText = Facade.TabMasterFacade.TreeViewCaption
                End If
                Title &= " " & titleText
                RemoveProtocolColumns()

        End Select

        If Not IsPostBack Then
            Select Case CurrentType
                Case "Prot"
                    txtTotal.Text = Facade.ProtocolLogFacade.GetProtocolUsersCount()
                Case "Docm"
                    txtTotal.Text = Facade.DocumentLogFacade.GetDocumentlUsersCount()
                Case "Resl"
                    txtTotal.Text = Facade.ResolutionLogFacade.GetResolutionUsersCount()
            End Select
        End If

    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        Dim user As String = ""
        Dim sDate As String = ""
        Dim eDate As String = ""
        Dim maxOpNumber As String = ""

        Dim dtTest As Date
        If txtDate_From.SelectedDate.HasValue And DateTime.TryParse(txtDate_From.SelectedDate.ToString, dtTest) Then
            sDate = String.Format("{0:yyyyMMdd}", CommonUtil.ConvData(dtTest.ToString()))
        End If

        dtTest = Nothing
        If txtDate_To.SelectedDate.HasValue And DateTime.TryParse(txtDate_To.SelectedDate.ToString, dtTest) Then
            eDate = String.Format("{0:yyyyMMdd}", CommonUtil.ConvData(dtTest.ToString()))
        End If

        If txtUser.Text <> String.Empty Then
            user = txtUser.Text
        End If

        If txtOperations.Text <> String.Empty Then
            maxOpNumber = txtOperations.Text
        End If

        Dim dt As DataTable = Nothing
        Select Case CurrentType
            Case "Prot"
                dt = Facade.ProtocolLogFacade.GetProtocolLogStatisticsTable(user, sDate, eDate, maxOpNumber)
            Case "Docm"
                dt = Facade.DocumentLogFacade.GetDocumentLogStatisticsTable(user, sDate, eDate, maxOpNumber)
            Case "Resl"
                dt = Facade.ResolutionLogFacade.GetResolutionLogStatisticsTable(user, sDate, eDate, maxOpNumber)
        End Select

        lblSelected.Text = dt.Rows.Count.ToString()

        Dim newRow As DataRow = dt.NewRow()
        Dim i As Integer
        For i = 0 To dt.Columns.Count - 1
            If i > 0 Then
                newRow(dt.Columns(i)) = dt.Compute("SUM(" & dt.Columns(i).ColumnName & ")", "")
            Else
                newRow(dt.Columns(i)) = "Totali"
            End If
        Next
        dt.Rows.Add(newRow)

        dgLogStatistics.DataSource = dt
        dgLogStatistics.DataBind()

    End Sub

    Protected Sub dgLogStatistics_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles dgLogStatistics.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim totalRowCount As Integer = CType(e.Item.DataItem, DataRowView).DataView.Count
        If totalRowCount > 0 Then
            If e.Item.ItemIndex = totalRowCount - 1 Then
                Dim item As GridDataItem = CType(e.Item, GridDataItem)
                item.Style.Add("font-weight", "bold")
            End If
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub RemoveProtocolColumns()
        dgLogStatistics.MasterTableView.Columns.FindByUniqueNameSafe("ZCount").Visible = False
        dgLogStatistics.MasterTableView.Columns.FindByUniqueNameSafe("MCount").Visible = False
    End Sub

#End Region

End Class