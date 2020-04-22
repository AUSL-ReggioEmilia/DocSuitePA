Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class CategoryPrint
    Inherits BasePrint

#Region "Fields"
    Private _categories As IList(Of Integer)
#End Region

#Region "Properties"
    Public Property CategoriesID() As IList(Of Integer)
        Get
            If _categories Is Nothing Then
                _categories = New List(Of Integer)
            End If
            Return _categories
        End Get
        Set(ByVal value As IList(Of Integer))
            _categories = value
        End Set
    End Property
#End Region

#Region "DoPrint"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa del classificatore unico aziendale"
        StampaClassificazione()
    End Sub
#End Region

#Region "Creazione Righe"
    Private Sub CreaRigaClassificatore(ByRef tbl As DSTable, ByVal category As Category)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()

        cellStyle.Width = Unit.Percentage(5)
        cellStyle.Font.Bold = True
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = False
        cellStyle.LineBox = True

        '5-B-L-W|95-B-L-W"
        'crea riga
        tbl.CreateEmptyRow("Prnt-Tabella")
        'cella sinistra
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = category.Code
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'cella destra
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = GetName(category)
        cellStyle.Width = Unit.Percentage(95)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Sub CreaRigaSottoclassificatore(ByRef tbl As DSTable, ByVal category As Category)
        Dim cellStyle As DSTableCellStyle = New DSTableCellStyle()

        cellStyle.Width = Unit.Percentage(5)
        cellStyle.Font.Bold = False
        cellStyle.HorizontalAlignment = HorizontalAlign.Left
        cellStyle.Wrap = False

        'crea riga
        tbl.CreateEmptyRow("Prnt-Chiaro")
        'cella sinistra
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = GetCode(category)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
        'cella destra
        tbl.CurrentRow.CreateEmpytCell()
        tbl.CurrentRow.CurrentCell.Text = GetName(category)
        cellStyle.Width = Unit.Percentage(95)
        tbl.CurrentRow.CurrentCell.ApplyStyle(cellStyle)
    End Sub

    Private Function GetCode(ByVal category As Category) As String
        Return Replace(Replace(category.FullCode, "0", ""), "|", ".")
    End Function

    Private Function GetName(ByVal category As Category) As String
        Dim str As String = String.Empty
        Dim p As String = ". . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . "
        str = p.Substring(1, category.Level * 2)
        str &= " " & category.Name
        If CommonUtil.HasGroupAdministratorRight() Then
            str &= " (" & category.Id & ")"
        End If
        Return str
    End Function
#End Region

#Region "Funzioni di stampa"
    Private Sub StampaClassificazione()
        Dim category As Category
        For Each id As Integer In CategoriesID
            category = Facade.CategoryFacade.GetById(id)
            CreaRigaClassificatore(TablePrint, category)
            CreateTreeCategoryPrint(category)
        Next
    End Sub

    Private Sub CreateTreeCategoryPrint(ByVal category As Category)
        Dim categories As IList(Of Category) = Nothing
        Dim strLevel As String = String.Empty
        Dim text As String = String.Empty

        categories = Facade.CategoryFacade.GetCategoryByParentId(category.Id, True)
        If categories.Count > 0 Then
            For Each child As Category In categories
                CreaRigaSottoclassificatore(TablePrint, child)
                CreateTreeCategoryPrint(child)
            Next
        End If
    End Sub
#End Region
    
End Class
