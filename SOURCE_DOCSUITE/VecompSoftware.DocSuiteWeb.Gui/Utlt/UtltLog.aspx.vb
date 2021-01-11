Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data


Partial Class UtltLog
    Inherits CommonBasePage

    Public _tableName As String = String.Empty
    Public tExtr As Label
    Private _logType As String

#Region " Properties "

    Public Property LogType() As String
        Get
            Return _logType
        End Get
        Set(ByVal value As String)
            _logType = value
        End Set
    End Property

    Public Property TableName() As String
        Get
            Return _tableName
        End Get
        Set(ByVal value As String)
            _tableName = value
        End Set
    End Property
#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, dgLog, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(dgLog, dgLog, MasterDocSuite.AjaxDefaultLoadingPanel)

        pnlInputTblt.Visible = False
        pnlInputProt.Visible = False
        pnlInputResl.Visible = False
        pnlDgTblt.Visible = False
        pnlTabella.Visible = False


        LogType = Request.QueryString("LogType")

        Select Case LogType
            Case "Tblt"
                Me.Title += "Tabelle"
                pnlInputTblt.Visible = True
                pnlDgTblt.Visible = True
                pnlTabella.Visible = True
                TableName = "TableLog"
                tExtr = txtTbltEstratto
                MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, txtTbltEstratto)
                AddSpecializedColumnFor("Tblt")
            Case "Prot"
                Me.Title += "Protocollo"
                pnlInputProt.Visible = True
                pnlDgTblt.Visible = True
                TableName = "ProtocolLog"
                tExtr = txtProtEstratto
                MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, txtProtEstratto)
                AddSpecializedColumnFor("Prot")
            Case "Docm"
                Me.Title += "Pratiche"
                pnlInputProt.Visible = True
                pnlDgTblt.Visible = True
                TableName = "DocumentLog"
                tExtr = txtProtEstratto
                MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, txtProtEstratto)
                AddSpecializedColumnFor("Docm")
            Case "Resl"
                Title += Facade.TabMasterFacade.TreeViewCaption
                pnlInputResl.Visible = True
                pnlDgTblt.Visible = True
                TableName = "ResolutionLog"
                tExtr = txtReslEstratto
                MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, txtReslEstratto)
                AddSpecializedColumnFor("Resl")
        End Select
        If Not IsPostBack Then
            Inizializza()
        End If
    End Sub

    Private Sub LogSearch()

        Dim iTotalCount As Integer
        Dim iSelectedCount As Integer
        Dim dtTest As Date
        Dim logPrint As New LogPrint

        iTotalCount = Integer.Parse(txtTotal.Text)

        Select Case LogType

            Case "Tblt"

                Dim tblLogFinder As NHibernateTableLogFinder

                tblLogFinder = Facade.TableLogFinder

                If txtType.Text <> String.Empty Then
                    tblLogFinder.LogType = txtType.Text
                End If

                If txtComputer.Text <> String.Empty Then
                    tblLogFinder.SystemComputer = txtComputer.Text
                End If

                If txtUser.Text <> String.Empty Then
                    tblLogFinder.SystemUser = txtUser.Text
                End If

                If txtDate_From.SelectedDate.HasValue And DateTime.TryParse(txtDate_From.SelectedDate.ToString, dtTest) Then
                    tblLogFinder.LogDateStart = dtTest
                End If

                dtTest = Nothing
                If txtDate_To.SelectedDate.HasValue And DateTime.TryParse(txtDate_To.SelectedDate.ToString, dtTest) Then
                    tblLogFinder.LogDateEnd = dtTest
                End If

                If txtTableName.Text <> String.Empty Then
                    tblLogFinder.TableName = txtTableName.Text
                End If

                iTotalCount = tblLogFinder.Count

                logPrint.TipoStampa = StampaLog.Tabelle
                logPrint.Finder = tblLogFinder

                dgLog.Finder = tblLogFinder
                dgLog.PageSize = 50
                dgLog.MasterTableView.SortExpressions.AddSortExpression("Id ASC")
                iSelectedCount = tblLogFinder.Count
                dgLog.DataBindFinder()

            Case "Prot"

                Dim protLogFinder As NHibernateProtocolLogFinder
                Dim year As Short
                Dim number As Integer

                protLogFinder = Facade.ProtocolLogFinder

                If txtType.Text <> String.Empty Then
                    protLogFinder.LogType = txtType.Text
                End If

                If txtComputer.Text <> String.Empty Then
                    protLogFinder.SystemComputer = txtComputer.Text
                End If

                If txtUser.Text <> String.Empty Then
                    protLogFinder.SystemUser = txtUser.Text
                End If

                If txtDate_From.SelectedDate.HasValue And DateTime.TryParse(txtDate_From.SelectedDate.ToString, dtTest) Then
                    protLogFinder.LogDateStart = dtTest
                End If

                dtTest = Nothing
                If txtDate_To.SelectedDate.HasValue And DateTime.TryParse(txtDate_To.SelectedDate.ToString, dtTest) Then
                    protLogFinder.LogDateEnd = dtTest
                End If

                If txtProtYear.Text <> String.Empty And Short.TryParse(txtProtYear.Text, year) Then
                    protLogFinder.ProtocolYear = year
                End If

                If txtProtNumber.Text <> String.Empty And Integer.TryParse(txtProtNumber.Text, number) Then
                    protLogFinder.ProtocolNumber = number
                End If

                logPrint.TipoStampa = StampaLog.Protocolli
                logPrint.Finder = protLogFinder

                iTotalCount = protLogFinder.Count

                dgLog.Finder = protLogFinder
                dgLog.PageSize = 50
                dgLog.MasterTableView.SortExpressions.AddSortExpression("Id ASC")
                iSelectedCount = protLogFinder.Count
                dgLog.DataBindFinder()

            Case "Docm"

                Dim docmLogFinder As NHibernateDocumentLogFinder
                Dim year As Short
                Dim number As Integer

                docmLogFinder = Facade.DocumentLogFinder

                If txtType.Text <> String.Empty Then
                    docmLogFinder.LogType = txtType.Text
                End If

                If txtComputer.Text <> String.Empty Then
                    docmLogFinder.SystemComputer = txtComputer.Text
                End If

                If txtUser.Text <> String.Empty Then
                    docmLogFinder.SystemUser = txtUser.Text
                End If

                If txtDate_From.SelectedDate.HasValue And DateTime.TryParse(txtDate_From.SelectedDate.ToString, dtTest) Then
                    docmLogFinder.LogDateStart = dtTest
                End If

                dtTest = Nothing
                If txtDate_To.SelectedDate.HasValue And DateTime.TryParse(txtDate_To.SelectedDate.ToString, dtTest) Then
                    docmLogFinder.LogDateEnd = dtTest
                End If

                If txtProtYear.Text <> String.Empty And Short.TryParse(txtProtYear.Text, year) Then
                    docmLogFinder.DocumentYear = year
                End If

                If txtProtNumber.Text <> String.Empty And Integer.TryParse(txtProtNumber.Text, number) Then
                    docmLogFinder.DocumentNumber = number
                End If

                logPrint.TipoStampa = StampaLog.Pratiche
                logPrint.Finder = docmLogFinder

                iTotalCount = docmLogFinder.Count

                dgLog.Finder = docmLogFinder
                dgLog.PageSize = 50
                dgLog.MasterTableView.SortExpressions.AddSortExpression("Id ASC")
                iSelectedCount = docmLogFinder.Count
                dgLog.DataBindFinder()

            Case "Resl"

                Dim reslLogFinder As NHibernateResolutionLogFinder
                Dim id As Integer

                reslLogFinder = Facade.ResolutionLogFinder

                If txtType.Text <> String.Empty Then
                    reslLogFinder.LogType = txtType.Text
                End If

                If txtComputer.Text <> String.Empty Then
                    reslLogFinder.SystemComputer = txtComputer.Text
                End If

                If txtUser.Text <> String.Empty Then
                    reslLogFinder.SystemUser = txtUser.Text
                End If

                If txtDate_From.SelectedDate.HasValue And DateTime.TryParse(txtDate_From.SelectedDate.ToString, dtTest) Then
                    reslLogFinder.LogDateStart = dtTest
                End If

                dtTest = Nothing
                If txtDate_To.SelectedDate.HasValue And DateTime.TryParse(txtDate_To.SelectedDate.ToString, dtTest) Then
                    reslLogFinder.LogDateEnd = dtTest
                End If

                If txtIdResolution.Text <> String.Empty And Integer.TryParse(txtIdResolution.Text, id) Then
                    reslLogFinder.Id = id
                End If

                logPrint.TipoStampa = StampaLog.Risoluzioni
                logPrint.Finder = reslLogFinder

                iTotalCount = reslLogFinder.Count

                dgLog.Finder = reslLogFinder
                dgLog.PageSize = 50
                dgLog.MasterTableView.SortExpressions.AddSortExpression("Id ASC")
                iSelectedCount = reslLogFinder.Count
                dgLog.DataBindFinder()

        End Select

        txtTotal.Text = iTotalCount
        tExtr.Text = iSelectedCount

        Session("Printer") = logPrint

    End Sub

    Private Sub AddSpecializedColumnFor(ByVal LogType As String)

        Dim boundColumn As GridBoundColumn

        Select Case LogType
            Case "Tblt"
                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("TableName"))
                boundColumn = New GridBoundColumn
                boundColumn.DataField = "TableName"
                boundColumn.UniqueName = "TableName"
                boundColumn.HeaderText = "Tabella"
                boundColumn.SortExpression = "TableName"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("IdRef"))
                boundColumn = New GridBoundColumn
                boundColumn.DataField = "IdRef"
                boundColumn.UniqueName = "IdRef"
                boundColumn.SortExpression = "IdRef"
                boundColumn.DataType = GetType(System.Int32)
                boundColumn.CurrentFilterFunction = GridKnownFunction.EqualTo
                boundColumn.HeaderText = "IdRef"
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("LogType"))
                boundColumn = New GridBoundColumn
                boundColumn.DataField = "LogType"
                boundColumn.UniqueName = "LogType"
                boundColumn.HeaderText = "Tipo"
                boundColumn.SortExpression = "LogType"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                boundColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("LogDescription"))
                boundColumn = New GridBoundColumn
                boundColumn.DataField = "LogDescription"
                boundColumn.UniqueName = "LogDescription"
                boundColumn.SortExpression = "LogDescription"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Descrizione"
                dgLog.MasterTableView.Columns.Add(boundColumn)

            Case "Prot", "Docm"

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("Program"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "Program"
                boundColumn.DataField = "Program"
                boundColumn.SortExpression = "Program"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Prg"
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("Year"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "Year"
                boundColumn.DataField = "Year"
                boundColumn.DataType = GetType(System.Int16)
                boundColumn.SortExpression = "Year"
                boundColumn.CurrentFilterFunction = GridKnownFunction.EqualTo
                boundColumn.HeaderText = "Anno"
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("Number"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "Number"
                boundColumn.DataField = "Number"
                boundColumn.DataType = GetType(System.Int32)
                boundColumn.SortExpression = "Number"
                boundColumn.CurrentFilterFunction = GridKnownFunction.EqualTo
                boundColumn.HeaderText = "Numero"
                boundColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("LogType"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "LogType"
                boundColumn.DataField = "LogType"
                boundColumn.SortExpression = "LogType"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Tipo"
                boundColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                boundColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("LogDescription"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "LogDescription"
                boundColumn.DataField = "LogDescription"
                boundColumn.SortExpression = "LogDescription"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Descrizione"
                dgLog.MasterTableView.Columns.Add(boundColumn)

            Case "Resl"

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("Program"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "Program"
                boundColumn.DataField = "Program"
                boundColumn.SortExpression = "Program"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Prg"
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("IdResolution"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "IdResolution"
                boundColumn.DataField = "IdResolution"
                boundColumn.AllowFiltering = False
                boundColumn.HeaderText = "Id"
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("LogType"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "LogType"
                boundColumn.DataField = "LogType"
                boundColumn.SortExpression = "LogType"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Tipo"
                boundColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                boundColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                dgLog.MasterTableView.Columns.Add(boundColumn)

                dgLog.MasterTableView.Columns.Remove(dgLog.MasterTableView.Columns.FindByUniqueNameSafe("LogDescription"))
                boundColumn = New GridBoundColumn
                boundColumn.UniqueName = "LogDescription"
                boundColumn.DataField = "LogDescription"
                boundColumn.SortExpression = "LogDescriprion"
                boundColumn.CurrentFilterFunction = GridKnownFunction.Contains
                boundColumn.HeaderText = "Descrizione"
                dgLog.MasterTableView.Columns.Add(boundColumn)

        End Select

    End Sub

    Private Sub Inizializza()

        Dim iRecordCount As Integer = 0

        Select Case LogType
            Case "Tblt"
                iRecordCount = Facade.TableLogFinder.Count
            Case "Prot"
                iRecordCount = Facade.ProtocolLogFinder.Count
            Case "Docm"
                iRecordCount = Facade.DocumentLogFinder.Count
            Case "Resl"
                iRecordCount = Facade.ResolutionLogFinder.Count
        End Select

        txtTotal.Text = iRecordCount

    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        LogSearch()
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Response.Redirect("..\Comm\CommPrint.aspx?PrintName=LogPrint")
    End Sub

End Class
