Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Microsoft.Reporting.WebForms

'' <summary>
'' Stampa del registro giornaliero 
'' </summary>
'' <remarks></remarks>
Public Class ReslJournalDelPrint
    Inherits BasePrintRpt

#Region "Fields"
    Private _omissis As Boolean
    Private _tipologia As Short
    Private _containersName As String = String.Empty
#End Region

#Region "Properties"

    Public Property Omissis() As Boolean
        Get
            Return _omissis
        End Get
        Set(ByVal value As Boolean)
            _omissis = value
        End Set
    End Property

    Public Property Tipologia() As Short
        Get
            Return _tipologia
        End Get
        Set(ByVal value As Short)
            _tipologia = value
        End Set
    End Property

    Public Property ContainersName() As String
        Get
            Return _containersName
        End Get
        Set(ByVal value As String)
            _containersName = value
        End Set
    End Property
#End Region


#Region "Create Rows"
    'Private Sub CreateContainersHeader(ByRef tbl As DSTable)
    '    'crea riga
    '    tbl.CreateEmptyRow()

    '    'crea cella Delibera
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateContenitoriCellStyle(tbl.CurrentRow.CurrentCell)
    '    tbl.CurrentRow.CurrentCell.Text = _containersName

    'End Sub

    'Private Sub CreateHeader(ByRef tbl As DSTable)
    '    'crea riga
    '    tbl.CreateEmptyRow()

    '    'crea cella Delibera
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateDeliberaCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
    '    tbl.CurrentRow.CurrentCell.Text = "DELIBERA"

    '    'crea cella Controllo
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateControlloCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
    '    tbl.CurrentRow.CurrentCell.Text = "CONTROLLO"

    '    'crea cella Oggetto
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
    '    tbl.CurrentRow.CurrentCell.Text = "OGGETTO"

    '    'crea cella Adozione
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateAdozioneCellStyle(tbl.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
    '    tbl.CurrentRow.CurrentCell.Text = "ADOZIONE"

    '    CreateDivindingLine(tbl, 4)
    'End Sub

    'Private Sub CreateRow(ByRef tbl As DSTable, ByVal resl As Resolution)
    '    'crea riga
    '    tbl.CreateEmptyRow()

    '    'crea cella Delibera
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateDeliberaCellStyle(tbl.CurrentRow.CurrentCell)
    '    tbl.CurrentRow.CurrentCell.Text = GetDelibera(resl)

    '    'crea cella Controllo
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateControlloCellStyle(tbl.CurrentRow.CurrentCell)
    '    tbl.CurrentRow.CurrentCell.Text = GetControllo(resl)

    '    'crea cella Oggetto
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateOggettoCellStyle(tbl.CurrentRow.CurrentCell)
    '    tbl.CurrentRow.CurrentCell.Text = ResolutionUtil.OggettoPrivacy(resl, _omissis) & _
    '    If(resl.ImmediatelyExecutive.GetValueOrDefault(False) = True, "<br/>IMMEDIATAMENTE ESEGUIBILE", "")

    '    'crea cella Adozione
    '    tbl.CurrentRow.CreateEmpytCell()
    '    CreateAdozioneCellStyle(tbl.CurrentRow.CurrentCell)
    '    tbl.CurrentRow.CurrentCell.Text = resl.AdoptionDate

    '    CreateDivindingLine(tbl, 4)
    'End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Registro Deliberazioni"

        CreatePrint()
    End Sub
#End Region

#Region "Private Functions"
    Private Sub CreatePrint()
        Dim resls As IList(Of Resolution)
        resls = Finder.DoSearch()
        Dim dsResls As DataSet

        If (resls.Count > 0) Then

            dsResls = New DataSet("Resolutions")
            dsResls.Tables.Add("tblResolution")
            dsResls.Tables(0).Columns.Add("Numero", GetType(System.String))
            dsResls.Tables(0).Columns.Add("Numero2", GetType(System.String))
            dsResls.Tables(0).Columns.Add("Controllo", GetType(System.String))
            dsResls.Tables(0).Columns.Add("Oggetto", GetType(System.String))
            dsResls.Tables(0).Columns.Add("DataAdozione", GetType(System.DateTime))
            dsResls.Tables(0).Columns.Add("ImmediatelyExecutive", GetType(System.Boolean))

            dsResls.Tables.Add("tblContainers")
            dsResls.Tables(1).Columns.Add("Contenitori", GetType(System.String))
            Dim dr As DataRow = dsResls.Tables(1).NewRow()
            dr("Contenitori") = ContainersName
            dsResls.Tables(1).Rows.Add(dr)

            For i As Integer = 0 To resls.Count - 1
                If (Finder.NotNumber Is Nothing OrElse resls(i).Number > 0) Then
                    CreateProtocolRow(dsResls.Tables(0), resls(i), 0)
                End If
            Next

            TablePrint.LocalReport.ReportPath = RdlcPrint
            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(0).TableName, dsResls.Tables(0)))
            TablePrint.LocalReport.DataSources.Add(New ReportDataSource(dsResls.DataSetName & "_" & dsResls.Tables(1).TableName, dsResls.Tables(1)))

        Else
            Throw New Exception("Ricerca Nulla")
        End If

    End Sub

    Private Sub CreateProtocolRow(ByRef tbl As DataTable, ByVal resl As Resolution, ByVal Index As Integer)

        Dim dr As DataRow = tbl.NewRow()
        dr("Numero") = resl.NumberFormat("{0:0000000}")
        dr("Numero2") = GetDelibera(resl)
        dr("Controllo") = GetControllo(resl)
        dr("Oggetto") = ResolutionUtil.OggettoPrivacy(resl, _omissis) &
            If(resl.ImmediatelyExecutive.GetValueOrDefault(False) = True, vbCrLf & "IMMEDIATAMENTE ESEGUIBILE", "")
        dr("ImmediatelyExecutive") = False
        'If resl.ImmediatelyExecutive.HasValue Then dr("ImmediatelyExecutive") = resl.ImmediatelyExecutive.Value
        dr("DataAdozione") = resl.AdoptionDate
        tbl.Rows.Add(dr)

    End Sub

    'Protected Sub CreateDivindingLine(ByRef tbl As DSTable, ByVal colspan As Integer, Optional ByVal size As String = "1")
    '    Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
    '    'stile cella
    '    cellStyle.Width = Unit.Percentage(100)
    '    cellStyle.ColumnSpan = colspan
    '    'crea riga
    '    tbl.CreateEmptyRow()
    '    tbl.CurrentRow.CreateEmpytCell()
    '    tbl.CurrentRow.CurrentCell.AddDividingLineControl("#000000", size)
    '    'stile cella
    '    tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    'End Sub

    Private Function GetDelibera(ByVal resl As Resolution) As String
        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then proposerCode = resl.ResolutionContactProposers.ElementAt(0).Contact.Code
            Return "/" & proposerCode & "/" & resl.Year.ToString
        Else
            Return String.Empty
        End If
    End Function

    Private Function GetControllo(ByVal resl As Resolution) As String
        Dim sControllo As String = ""
        If resl.OCSupervisoryBoard.GetValueOrDefault(False) Then sControllo = "art.14"
        If resl.OCRegion.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "Regione"
        End If
        If resl.OCManagement.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "Controllo Gestione"
        End If

        If resl.OCCorteConti.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= "Corte dei Conti"
        End If
        If resl.OCOther.GetValueOrDefault(False) Then
            If sControllo <> "" Then sControllo &= vbCrLf
            sControllo &= resl.OtherDescription
        End If

        Return sControllo
    End Function
#End Region
End Class
