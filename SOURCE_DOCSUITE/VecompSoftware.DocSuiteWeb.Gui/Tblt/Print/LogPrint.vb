Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Stampa dei Log </summary>
Public Class LogPrint
    Inherits BasePrint

    Private _tipoStampa As StampaLog
    Private _finder As Object

    Public Property TipoStampa() As StampaLog
        Get
            Return _tipoStampa
        End Get
        Set(ByVal value As StampaLog)
            _tipoStampa = value
        End Set
    End Property

    Public Property Finder() As Object
        Get
            Return _finder
        End Get
        Set(ByVal value As Object)
            _finder = value
        End Set
    End Property

    Public Overrides Sub DoPrint()

        Dim logList As IList
        Dim tbl As DSTable = MyBase.TablePrint
        Dim testLog As ILog = Nothing

        Select Case TipoStampa
            Case StampaLog.Pratiche
                Dim docmLogFinder As NHibernateDocumentLogFinder
                docmLogFinder = CType(Finder, NHibernateDocumentLogFinder)
                logList = docmLogFinder.DoSearch()
                testLog = logList(0)

                If docmLogFinder.LogDateStart.HasValue Then
                    CreaRigaIntestazione(tbl, "Da Data: " & docmLogFinder.LogDateStart.ToString, 9)
                End If
                If docmLogFinder.LogDateEnd.HasValue Then
                    CreaRigaIntestazione(tbl, "A Data: " & docmLogFinder.LogDateEnd.ToString, 9)
                End If
                If docmLogFinder.SystemComputer <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Computer: " & docmLogFinder.SystemComputer, 9)
                End If
                If docmLogFinder.SystemUser <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Utente: " & docmLogFinder.SystemUser, 9)
                End If
                If docmLogFinder.LogType <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Tipo: " & docmLogFinder.LogType, 9)
                End If
                If docmLogFinder.DocumentYear <> 0 Then
                    CreaRigaIntestazione(tbl, "Anno: " & docmLogFinder.DocumentYear, 9)
                End If
                If docmLogFinder.DocumentNumber <> 0 Then
                    CreaRigaIntestazione(tbl, "Numero: " & docmLogFinder.DocumentNumber, 9)
                End If

            Case StampaLog.Protocolli
                Dim protLogFinder As NHibernateProtocolLogFinder
                protLogFinder = CType(Finder, NHibernateProtocolLogFinder)
                logList = protLogFinder.DoSearch()
                testLog = logList(0)

                If protLogFinder.LogDateStart.HasValue Then
                    CreaRigaIntestazione(tbl, "Da Data: " & protLogFinder.LogDateStart.ToString, 9)
                End If
                If protLogFinder.LogDateEnd.HasValue Then
                    CreaRigaIntestazione(tbl, "A Data: " & protLogFinder.LogDateEnd.ToString, 9)
                End If
                If protLogFinder.SystemComputer <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Computer: " & protLogFinder.SystemComputer, 9)
                End If
                If protLogFinder.SystemUser <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Utente: " & protLogFinder.SystemUser, 9)
                End If
                If protLogFinder.LogType <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Tipo: " & protLogFinder.LogType, 9)
                End If
                If protLogFinder.ProtocolYear.HasValue Then
                    CreaRigaIntestazione(tbl, "Anno: " & protLogFinder.ProtocolYear, 9)
                End If
                If protLogFinder.ProtocolNumber.HasValue Then
                    CreaRigaIntestazione(tbl, "Numero: " & protLogFinder.ProtocolNumber, 9)
                End If

            Case StampaLog.Risoluzioni
                Dim reslLogFinder As NHibernateResolutionLogFinder
                reslLogFinder = CType(Finder, NHibernateResolutionLogFinder)
                logList = reslLogFinder.DoSearch()
                testLog = logList(0)

                If reslLogFinder.LogDateStart.HasValue Then
                    CreaRigaIntestazione(tbl, "Da Data: " & reslLogFinder.LogDateStart.ToString, 9)
                End If
                If reslLogFinder.LogDateEnd.HasValue Then
                    CreaRigaIntestazione(tbl, "A Data: " & reslLogFinder.LogDateEnd.ToString, 9)
                End If
                If reslLogFinder.SystemComputer <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Computer: " & reslLogFinder.SystemComputer, 9)
                End If
                If reslLogFinder.SystemUser <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Utente: " & reslLogFinder.SystemUser, 9)
                End If
                If reslLogFinder.LogType <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Tipo: " & reslLogFinder.LogType, 9)
                End If

            Case StampaLog.Tabelle
                Dim tblLogFinder As NHibernateTableLogFinder
                tblLogFinder = CType(Finder, NHibernateTableLogFinder)
                logList = tblLogFinder.DoSearch()
                testLog = logList(0)

                If tblLogFinder.LogDateStart.HasValue Then
                    CreaRigaIntestazione(tbl, "Da Data: " & tblLogFinder.LogDateStart.ToString, 9)
                End If
                If tblLogFinder.LogDateEnd.HasValue Then
                    CreaRigaIntestazione(tbl, "A Data: " & tblLogFinder.LogDateEnd.ToString, 9)
                End If
                If tblLogFinder.SystemComputer <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Computer: " & tblLogFinder.SystemComputer, 9)
                End If
                If tblLogFinder.SystemUser <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Utente: " & tblLogFinder.SystemUser, 9)
                End If
                If tblLogFinder.LogType <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Tipo: " & tblLogFinder.LogType, 9)
                End If
                If tblLogFinder.TableName <> String.Empty Then
                    CreaRigaIntestazione(tbl, "Tabella: " & tblLogFinder.TableName, 9)
                End If

        End Select

        Dim id As String = "", data As String = "", comp As String = "", ut As String = "", prg As String = "", anno As String = "", num As String = "", tipo As String = "", desc As String = ""


        id = "Id"


        If testLog.LogDate.ToString <> String.Empty Then
            data = "Data"
        End If

        If testLog.SystemComputer <> String.Empty Then
            comp = "Computer"
        End If

        If testLog.SystemUser <> String.Empty Then
            ut = "Utente"
        End If

        If testLog.Program <> String.Empty Then
            prg = "Prg"
        End If

        If testLog.Year <> 0 Then
            anno = "Anno"
        End If

        If testLog.Number <> 0 Then
            num = "Number"
        End If

        If testLog.LogType <> String.Empty Then
            tipo = "Tipo"
        End If

        If testLog.LogDescription <> String.Empty Then
            desc = "Descrizione"
        End If

        CreaRigaDettaglio(tbl, id, data, comp, ut, prg, anno, num, tipo, desc, True, "tabella")

        For Each log As ILog In logList
            CreaRigaDettaglio(tbl, log.Id, log.LogDate, log.SystemComputer, log.SystemUser, log.Program, log.Year, log.Number, log.LogType, log.LogDescription, False)
        Next

    End Sub

    Private Sub CreaRigaIntestazione(ByRef tblNew As DSTable, ByVal testo As String, ByVal colSpan As Integer)

        Dim cellStyle As New DSTableCellStyle()

        tblNew.Width = Unit.Percentage(100)
        tblNew.CreateEmptyRow("tabella")
        tblNew.CurrentRow.CreateEmpytCell()
        tblNew.CurrentRow.CurrentCell.Text = testo

        cellStyle.ColumnSpan = colSpan
        cellStyle.Font.Bold = True
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.HorizontalAlignment = HorizontalAlign.Left

        tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)

    End Sub

    Private Sub CreaRigaDettaglio(ByRef tblNew As DSTable, ByVal testoCella1 As String, ByVal testoCella2 As String, _
                                ByVal testoCella3 As String, ByVal testoCella4 As String, ByVal testoCella5 As String, _
                                ByVal testoCella6 As String, ByVal testoCella7 As String, ByVal testoCella8 As String, _
                                ByVal testoCella9 As String, ByVal fontBold As Boolean, Optional ByVal cssRowClass As String = "")

        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()

        '    Row = "Id|Data|Computer|Utente|Prg|Anno|Numero|Tipo|Descrizione"
        '    Type = "5-B-L|15-B-L|11-B-L|15-B-L|5-B-L|8-B-L|8-B-L|5-B-L|28-B-L"

        tblNew.Width = Unit.Percentage(100)

        If cssRowClass <> String.Empty Then
            tblNew.CreateEmptyRow(cssRowClass)
        Else
            tblNew.CreateEmptyRow()
        End If

        If testoCella1 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella1
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(5)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella2 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella2
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(15)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella3 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella3
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(11)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella4 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella4
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(15)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella5 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella5
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(5)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella6 <> String.Empty And testoCella6 <> "0" Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella6
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(8)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella7 <> String.Empty And testoCella6 <> "0" Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella7
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(8)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella8 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella8
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(5)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If

        If testoCella9 <> String.Empty Then
            tblNew.CurrentRow.CreateEmpytCell()
            tblNew.CurrentRow.CurrentCell.Text = testoCella9
            cellStyle.Font.Bold = fontBold
            cellStyle.Width = Unit.Percentage(28)
            cellStyle.HorizontalAlignment = HorizontalAlign.Left
            tblNew.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        End If
    End Sub

End Class
