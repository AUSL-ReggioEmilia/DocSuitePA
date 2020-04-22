Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ReslElencoPrint
    Inherits BasePrint

#Region "Fields"
    Private _idContainers As String = String.Empty
    Private _tipologia As Short
    Private _adoptionDate As Date?
    Private _gestione As Boolean
    Private _omissis As Boolean
#End Region

#Region "Properties"

    Public Property IdContainers() As String
        Get
            Return _idContainers
        End Get
        Set(ByVal value As String)
            _idContainers = value
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

    Public Property AdoptionDate() As Date?
        Get
            Return _adoptionDate
        End Get
        Set(ByVal value As Date?)
            _adoptionDate = value
        End Set
    End Property

    Public Property Gestione() As Boolean
        Get
            Return _gestione
        End Get
        Set(ByVal value As Boolean)
            _gestione = value
        End Set
    End Property

    Public Property Omissis() As Boolean
        Get
            Return _omissis
        End Get
        Set(ByVal value As Boolean)
            _omissis = value
        End Set
    End Property
#End Region

#Region "Cell Styles"
    'Numero
    Private Sub CreateNumeroCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(15)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = False
        cell.ApplyStyle(cellStyle)
    End Sub

    'Oggetto Delibera
    Private Sub CreateOggettoCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(85)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = True
        cellStyle.LineBox = False
        cell.ApplyStyle(cellStyle)
    End Sub

    'Oggetto Determina
    Private Sub CreateOggettoDetCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean, ByVal align As HorizontalAlign)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(45)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = align
        cellStyle.Wrap = True
        cellStyle.LineBox = False
        cell.ApplyStyle(cellStyle)
    End Sub

    'Data Adozione
    Private Sub CreateDataAdozioneCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(15)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Center
        cellStyle.LineBox = False
        cell.ApplyStyle(cellStyle)
    End Sub

    'Contenitore
    Private Sub CreateContenitoreCellStyle(ByRef cell As DSTableCell, ByVal bold As Boolean)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(25)
        cellStyle.Font.Bold = bold
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.LineBox = False
        cell.ApplyStyle(cellStyle)
    End Sub

    'In Data
    Private Sub CreateColspanCellStyle(ByRef cell As DSTableCell, ByVal align As HorizontalAlign, ByVal colspan As Integer)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = align
        cellStyle.ColumnSpan = colspan
        cellStyle.LineBox = False
        cell.ApplyStyle(cellStyle)
    End Sub
#End Region

#Region "IPrint Implementation"
    Public Overrides Sub DoPrint()
        CreatePrint()
    End Sub
#End Region

#Region "Private Methods"
    Private Sub CreatePrint()
        Dim LastDate As Date

        Dim resls As IList(Of Resolution)
        resls = Finder.DoSearch()

        If resls IsNot Nothing AndAlso resls.Count > 0 Then
            Dim resl As Resolution = resls(0)

            'Setto il titolo della stampa
            Select Case Tipologia
                Case ResolutionType.IdentifierDelibera
                    TitlePrint = "ELENCO DELIBERAZIONI ADOTTATE" & " in data " & String.Format("{0:dd/MM/yyyy}", AdoptionDate) & "|" & resl.Container.HeadingLetter
                Case ResolutionType.IdentifierDetermina
                    TitlePrint = "ELENCO DETERMINAZIONI DIRIGENZIALI ADOTTATE"
            End Select

            'crea riga
            TablePrint.CreateEmptyRow()
            Select Case Tipologia
                Case ResolutionType.IdentifierDelibera
                    'crea cella Numero
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateNumeroCellStyle(TablePrint.CurrentRow.CurrentCell, True)
                    TablePrint.CurrentRow.CurrentCell.Text = "NUMERO DELIBERA"

                    'crea cella Oggetto
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateOggettoCellStyle(TablePrint.CurrentRow.CurrentCell, True)
                    TablePrint.CurrentRow.CurrentCell.Text = "OGGETTO"

                Case ResolutionType.IdentifierDetermina
                    'crea cella Numero
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateNumeroCellStyle(TablePrint.CurrentRow.CurrentCell, True)
                    TablePrint.CurrentRow.CurrentCell.Text = "NUMERO DETERMINA"

                    'crea cella Data Adozione
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateDataAdozioneCellStyle(TablePrint.CurrentRow.CurrentCell, True)
                    TablePrint.CurrentRow.CurrentCell.Text = "DATA ADOZIONE"

                    'crea cella Oggetto
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateOggettoDetCellStyle(TablePrint.CurrentRow.CurrentCell, True, HorizontalAlign.Center)
                    TablePrint.CurrentRow.CurrentCell.Text = "OGGETTO"

                    'crea cella Contenitore
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateContenitoreCellStyle(TablePrint.CurrentRow.CurrentCell, True)
                    TablePrint.CurrentRow.CurrentCell.Text = "CONTENITORE"

            End Select

            'creo divisore
            CreateDivindingLine(TablePrint, If(Tipologia = ResolutionType.IdentifierDetermina, 4, 2))

            For Each resl In resls
                If ResolutionType.IdentifierDetermina Then
                    If (resl.AdoptionDate <> LastDate) Then
                        'crea riga
                        TablePrint.CreateEmptyRow()

                        'crea cella In Data
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateColspanCellStyle(TablePrint.CurrentRow.CurrentCell, HorizontalAlign.Center, 4)
                        TablePrint.CurrentRow.CurrentCell.Text = "IN DATA: " & resl.AdoptionDate.Value

                        'creo divisore
                        CreateDivindingLine(TablePrint, 2, 3)

                        LastDate = resl.AdoptionDate
                    End If
                End If

                Dim s As String = ""
                Dim s1 As String = ""
                Facade.ResolutionFacade.ReslFullNumber(resl, Tipologia, s, s1, True)

                'crea riga
                TablePrint.CreateEmptyRow()
                Select Case Tipologia
                    Case ResolutionType.IdentifierDelibera
                        'crea cella Numero
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateNumeroCellStyle(TablePrint.CurrentRow.CurrentCell, False)
                        TablePrint.CurrentRow.CurrentCell.Text = s1

                        'crea cella Oggetto
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateOggettoCellStyle(TablePrint.CurrentRow.CurrentCell, False)
                        TablePrint.CurrentRow.CurrentCell.Text = ResolutionUtil.OggettoPrivacy(resl, _omissis) & _
                        If(resl.ImmediatelyExecutive.GetValueOrDefault(False) = True, "<BR>IMMEDIATAMENTE ESEGUIBILE", "")

                    Case ResolutionType.IdentifierDetermina
                        'crea cella Numero
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateNumeroCellStyle(TablePrint.CurrentRow.CurrentCell, False)
                        TablePrint.CurrentRow.CurrentCell.Text = s1

                        'crea cella Data Adozione
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateDataAdozioneCellStyle(TablePrint.CurrentRow.CurrentCell, False)
                        TablePrint.CurrentRow.CurrentCell.Text = String.Format("{0:dd/MM/yyyy}", resl.AdoptionDate)

                        'crea cella Oggetto
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateOggettoDetCellStyle(TablePrint.CurrentRow.CurrentCell, False, HorizontalAlign.Left)
                        TablePrint.CurrentRow.CurrentCell.Text = ResolutionUtil.OggettoPrivacy(resl, _omissis)

                        'crea cella Oggetto
                        TablePrint.CurrentRow.CreateEmpytCell()
                        CreateContenitoreCellStyle(TablePrint.CurrentRow.CurrentCell, False)
                        TablePrint.CurrentRow.CurrentCell.Text = resl.Container.Name

                End Select

                'OC
                Dim sRow As String = ""
                If (resl.OCSupervisoryBoard.GetValueOrDefault(False) = True) Then sRow = "art.14"
                If (resl.OCRegion.GetValueOrDefault(False) = True) Then
                    If sRow <> "" Then sRow &= "<BR>"
                    sRow &= "Regione"
                End If
                If (resl.OCManagement.GetValueOrDefault(False) = True) Then
                    If sRow <> "" Then sRow &= "<BR>"
                    sRow &= "Controllo Gestione"
                End If
                If (resl.OCCorteConti.GetValueOrDefault(False) = True) Then
                    If sRow <> "" Then sRow &= "<BR>"
                    sRow &= "Corte dei Conti"
                End If
                If (resl.OCOther.GetValueOrDefault(False) = True) Then
                    If sRow <> "" Then sRow &= "<BR>"
                    sRow &= resl.OtherDescription
                End If

                If sRow <> "" Then
                    'crea riga
                    TablePrint.CreateEmptyRow()

                    'crea cella OC
                    TablePrint.CurrentRow.CreateEmpytCell()
                    CreateColspanCellStyle(TablePrint.CurrentRow.CurrentCell, HorizontalAlign.Left, If(Tipologia = ResolutionType.IdentifierDetermina, 4, 2))
                    TablePrint.CurrentRow.CurrentCell.Text = "<BR>" & sRow
                End If

                'creo divisore
                CreateDivindingLine(TablePrint, If(Tipologia = ResolutionType.IdentifierDetermina, 4, 2))
            Next
        End If
    End Sub

    Protected Sub CreateDivindingLine(ByRef tbl As DSTable, ByVal colspan As Integer, Optional ByVal size As String = "1")
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()
        'stile cella
        cellStyle.Width = Unit.Percentage(100)
        cellStyle.ColumnSpan = colspan
        'crea riga
        tbl.CreateEmptyRow()
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.AddDividingLineControl("#000000", size)
        'stile cella
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub
#End Region

End Class
