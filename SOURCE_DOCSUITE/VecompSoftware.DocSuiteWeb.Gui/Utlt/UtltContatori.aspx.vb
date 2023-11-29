Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltContatori
    Inherits CommonBasePage

#Region " Fields "

    Private _serverYear As Integer = 0
    Private _tableYear As Integer = 0

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If
        InitializeAjaxSettings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnDocmAggiorna_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDocmAggiorna.Click
        Aggiorna("DocmDB")
        Initialize()
    End Sub

    Private Sub btnProtAggiorna_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProtAggiorna.Click
        Aggiorna("ProtDB")
        Initialize()
    End Sub

    Private Sub btnReslAggiorna_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReslAggiorna.Click
        Aggiorna("ReslDB")
        Initialize()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        If DocSuiteContext.Current.IsDocumentEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnDocmAggiorna, phTable)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnDocmAggiorna, btnDocmAggiorna)
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnProtAggiorna, phTable)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnProtAggiorna, btnProtAggiorna)
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnReslAggiorna, phTable)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnReslAggiorna, btnReslAggiorna)
        End If
    End Sub

    Private Sub Initialize()
        Dim table As New DSTable(False)
        table.CSSClass = "datatable"
        Dim paramFacade As ParameterFacade = Nothing

        'abilitazione bottoni a seconda delle sezioni attive
        AbilitaButtons(False)

        'Anno del server
        _serverYear = DateTime.Now.Year
        If DocSuiteContext.Current.IsDocumentEnabled Then
            paramFacade = New ParameterFacade("DocmDB")
            _tableYear = paramFacade.GetCurrent().LastUsedYear
            CreaIntestazione(table, "Pratiche")
            CreaRiga("DocmDB", table, "Anno server", "scuro", 1)
            CreaRiga("DocmDB", table, "Anno in uso", "chiaro", 2)
            CreaRiga("DocmDB", table, "Numero", "chiaro", 3)
            btnDocmAggiorna.Enabled = (_serverYear <> _tableYear)

        End If

        If DocSuiteContext.Current.IsProtocolEnabled Then
            paramFacade = New ParameterFacade("ProtDB")
            _tableYear = paramFacade.GetCurrent().LastUsedYear
            CreaIntestazione(table, "Protocollo")
            CreaRiga("ProtDB", table, "Anno server", "scuro", 1)
            CreaRiga("ProtDB", table, "Anno in uso", "chiaro", 2)
            CreaRiga("ProtDB", table, "Numero", "chiaro", 3)
            btnProtAggiorna.Enabled = (_serverYear <> _tableYear)
        End If

        If DocSuiteContext.Current.IsResolutionEnabled Then
            paramFacade = New ParameterFacade("ReslDB")
            _tableYear = paramFacade.GetCurrent().LastUsedResolutionYear
            CreaIntestazione(table, Facade.TabMasterFacade.TreeViewCaption)
            CreaRiga("ReslDB", table, "Anno server", "scuro", 1)
            CreaRiga("ReslDB", table, "Anno in uso", "chiaro", 2)
            CreaRiga("ReslDB", table, "Numero " & Facade.ResolutionTypeFacade.DeliberaCaption, "chiaro", 3)
            CreaRiga("ReslDB", table, "Numero " & Facade.ResolutionTypeFacade.DeterminaCaption, "chiaro", 4)
            btnReslAggiorna.Text = "Aggiorna " & Facade.TabMasterFacade.TreeViewCaption
            btnReslAggiorna.Enabled = (_serverYear <> _tableYear)
        End If

        table.Width = Unit.Percentage(100)
        phTable.Controls.Add(table.Table)
    End Sub

    Private Sub AbilitaButtons(ByVal bool As Boolean)
        btnDocmAggiorna.Enabled = bool
        btnProtAggiorna.Enabled = bool
        btnReslAggiorna.Enabled = bool

        btnDocmAggiorna.Visible = DocSuiteContext.Current.IsDocumentEnabled
        btnProtAggiorna.Visible = DocSuiteContext.Current.IsProtocolEnabled
        btnReslAggiorna.Visible = DocSuiteContext.Current.IsResolutionEnabled
    End Sub

    Private Sub CreaIntestazione(ByRef tblNew As DSTable, ByVal testo As String)
        Dim cellStyle As New DSTableCellStyle()
        tblNew.CreateEmptyRow("titolo")
        tblNew.CurrentRow.CreateEmpytCell(False)
        tblNew.CurrentRow.CurrentCell.Text = testo

        cellStyle.ColumnSpan = 2
        cellStyle.Font.Bold = True
        cellStyle.Width = Unit.Percentage(30)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left

        tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreaRiga(ByVal dbName As String, ByRef tblNew As DSTable, ByVal testo As String, ByVal css As String, ByVal p As Byte)
        'prima cella
        With tblNew
            .CreateEmptyRow(False)
            .CurrentRow.CreateEmpytCell(False)
            .CurrentRow.CurrentCell.Text = testo
            .CurrentRow.CurrentCell.CSSClass = "label"
        End With

        'seconda cella
        With tblNew
            .CurrentRow.CreateEmpytCell(False)
            Dim paramFacade As New ParameterFacade(dbName)
            Dim parameter As Parameter = paramFacade.GetCurrent()
            Select Case p
                Case 1
                    .CurrentRow.CurrentCell.Text = _serverYear.ToString()
                Case 2
                    .CurrentRow.CurrentCell.Text = GetLastUsedYear(parameter, dbName).ToString()
                Case 3
                    .CurrentRow.CurrentCell.Text = GetLastUsedNumber(parameter, dbName)
                Case 4
                    .CurrentRow.CurrentCell.Text = parameter.LastUsedBillNumber
                Case Else
            End Select
            .CurrentRow.CurrentCell.Width = Unit.Percentage(70)
        End With
    End Sub

    Private Function GetLastUsedYear(ByVal param As Parameter, ByVal dbName As String) As Short
        Select Case dbName
            Case "ProtDB", "DocmDB"
                Return param.LastUsedYear
            Case "ReslDB"
                Return param.LastUsedResolutionYear
        End Select
    End Function

    Private Function GetLastUsedNumber(ByVal param As Parameter, ByVal dbName As String) As Integer
        Select Case dbName
            Case "ProtDB", "DocmDB"
                Return param.LastUsedNumber
            Case "ReslDB"
                Return param.LastUsedResolutionNumber
        End Select
    End Function

    Function Aggiorna(ByVal dbName As String) As Boolean
        Dim paramFacade As New ParameterFacade(dbName)
        Try
            Select Case dbName
                Case "ProtDB"
                    Dim parameter As Parameter = paramFacade.GetCurrent()
                    parameter.LastUsedYear = Convert.ToInt16(DateTime.Now.Year)
                    parameter.LastUsedNumber = 0
                    paramFacade.UpdateSingleInstance(parameter, dbName)

                Case "DocmDB"
                    paramFacade.ResetDocumentNumbers()

                Case "ReslDB"
                    paramFacade.ResetResolutionNumbers()
            End Select
        Catch ex As Exception
            FileLogger.Warn(LoggerName, $"Impossibile aggiornare i contatori su ambiente [{dbName}].", ex)
            Return False
        End Try

        Return True
    End Function

#End Region

End Class
