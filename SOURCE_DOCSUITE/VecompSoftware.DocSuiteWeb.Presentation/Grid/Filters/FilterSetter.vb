Imports System.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI

Public Class FilterSetter

#Region " Fields "

    Private _column As GridColumn
    Private _finder As Object

#End Region

#Region " Constructor "

    Public Sub New(ByRef column As GridColumn, ByRef finder As Object)
        _column = column
        _finder = finder
    End Sub

#End Region

#Region " Methods "

    Private Sub SetFilter(ByRef column As GridColumn, ByVal filter As IFilterExpression)
        'verifico che il filtro esista già nella lista
        Dim filterExpressions As IDictionary(Of String, IFilterExpression) = ReflectionHelper.GetProperty(_finder, "FilterExpressions")
        FilterHelper.UpdateFilter(filterExpressions, filter)
        'imposta il filtro corrente
        column.CurrentFilterFunction = FilterHelper.ConvertFilterExpressionToGridKnownFunction(filter.FilterExpression)
    End Sub

    Private Sub SimpleFilterSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim textFilter As Object = Nothing
        For Each ctrl As Control In filterItem(command.Second).Controls
            If (TypeOf ctrl Is LiteralControl) Then
                Continue For
            End If

            textFilter = FilterHelper.GetFilterValue(ctrl, _column.UniqueName)
            Exit For
        Next
        CreateAndUpdateFilter(textFilter, command, _column)
    End Sub

    Private Sub YearNumberFilterSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim column As YearNumberBoundColumn = DirectCast(_column, YearNumberBoundColumn)
        'crea il filtro
        Dim textFilter As String = FilterHelper.GetFilterValue(filterItem(command.Second).Controls.Item(0), _column.UniqueName)
        Dim filter As IFilterExpression = New Data.FilterExpression(command.Second, column.DataType, textFilter, Data.FilterExpression.FilterType.SQL)
        filter.SQLExpression = column.GetSQLExpression(textFilter)
        'imposta il filtro
        SetFilter(column, filter)
    End Sub

    Private Sub YearSubCatIncFilterSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim column As YearNumberBoundColumn = DirectCast(_column, YearNumberBoundColumn)
        'crea il filtro
        Dim textFilter As String = FilterHelper.GetFilterValue(filterItem(command.Second).Controls.Item(0), _column.UniqueName)
        Dim filter As IFilterExpression = New Data.FilterExpression(command.Second, column.DataType, textFilter, Data.FilterExpression.FilterType.SQL)
        filter.SQLExpression = column.GetSQLExpression(textFilter)
        'imposta il filtro
        SetFilter(column, filter)
    End Sub

    Private Sub GridDateTimeExSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim column As GridDateTimeColumnEx = DirectCast(_column, GridDateTimeColumnEx)
        'crea il filtro
        Dim textFilter As String = FilterHelper.GetFilterValue(filterItem(command.Second).Controls.Item(0), _column.UniqueName)
        Dim filter As IFilterExpression = New Data.FilterExpression(command.Second, column.DataType, textFilter, Data.FilterExpression.FilterType.SQL)
        filter.SQLExpression = column.GetSQLExpression(textFilter, command.First, filter.PropertyName)
        'imposta il filtro
        SetFilter(column, filter)
    End Sub

    Private Sub CompositeFilterSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim column As CompositeBoundColumn = DirectCast(_column, CompositeBoundColumn)

        Dim textFilter As String = FilterHelper.GetFilterValue(filterItem(command.Second).Controls.Item(0), _column.UniqueName)
        Select Case column.BindingType
            Case CompositeBoundColumn.ColumnBinding.CustomBinding
                Dim filter As IFilterExpression = New Data.FilterExpression(command.Second, column.DataType, textFilter, Data.FilterExpression.FilterType.Criteria)
                filter.CriteriaImpl = column.GetExpression(textFilter)
                SetFilter(column, filter)
            Case CompositeBoundColumn.ColumnBinding.DefaultBinding
                CreateAndUpdateFilter(textFilter, command, _column)
        End Select
    End Sub

    Private Sub CompositeFilterBySqlExpSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim column As CompositeTemplateColumnSqlExpression = DirectCast(_column, CompositeTemplateColumnSqlExpression)

        Dim textFilter As String = FilterHelper.GetFilterValue(filterItem(command.Second).Controls.Item(0), _column.UniqueName)
        Select Case column.BindingType
            Case CompositeTemplateColumnSqlExpression.ColumnBinding.CustomBinding
                Dim filter As IFilterExpression = New Data.FilterExpression(command.Second, column.DataType, textFilter, Data.FilterExpression.FilterType.Criteria)
                filter.CriteriaImpl = column.GetExpression(textFilter)
                SetFilter(column, filter)
            Case CompositeTemplateColumnSqlExpression.ColumnBinding.DefaultBinding
                CreateAndUpdateFilter(textFilter, command, _column)
        End Select
    End Sub

    Private Sub CompositeTemplateFilterSetter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Dim column As CompositeTemplateColumn = DirectCast(_column, CompositeTemplateColumn)
        'crea il filtro
        Dim textFilter As String = FilterHelper.GetFilterValue(filterItem(command.Second).Controls.Item(0), _column.UniqueName)
        Dim filter As IFilterExpression = New Data.FilterExpression(command.Second, column.DataType, textFilter, Data.FilterExpression.FilterType.SQL)
        filter.SQLExpression = column.GetSQLExpression(textFilter)
        'imposta il filtro
        SetFilter(column, filter)
    End Sub

    ''' <summary> Compone il filtro e lo aggiorna </summary>
    Private Sub CreateAndUpdateFilter(ByVal textFilter As String, ByVal command As Pair, ByRef column As GridColumn)
        Dim filter As IFilterExpression = New Data.FilterExpression()
        filter.FilterValue = textFilter
        filter.PropertyType = column.DataType
        If column.DataType.IsEnum Then
            filter.FilterExpression = Data.FilterExpression.FilterType.IsEnum
        Else
            filter.FilterExpression = FilterHelper.ConvertFilterMenuValues(command.First)
        End If
        'filter.PropertyName = column.UniqueName

        If TypeOf (column) Is SuggestFilteringColumn Then
            Dim suggestColumn As SuggestFilteringColumn = DirectCast(column, SuggestFilteringColumn)
            filter.PropertyName = If(String.IsNullOrEmpty(suggestColumn.PropertyBind), command.Second, suggestColumn.PropertyBind)
        Else
            filter.PropertyName = command.Second
        End If

        'verifico che il filtro esista già nella lista
        Dim filterExpressions As IDictionary(Of String, IFilterExpression) = ReflectionHelper.GetProperty(_finder, "FilterExpressions")
        FilterHelper.UpdateFilter(filterExpressions, filter)

        'imposta il filtro corrente
        column.CurrentFilterFunction = FilterHelper.ConvertFilterExpressionToGridKnownFunction(filter.FilterExpression)
    End Sub

    Public Sub SetFilter(ByVal command As Pair, ByVal filterItem As GridFilteringItem)
        Select Case _column.GetType()
            Case GetType(YearNumberBoundColumn)
                YearNumberFilterSetter(command, filterItem)
            Case GetType(YearSubCatIncBoundColumn)
                YearSubCatIncFilterSetter(command, filterItem)
            Case GetType(GridDateTimeColumnEx)
                GridDateTimeExSetter(command, filterItem)
            Case GetType(CompositeBoundColumn)
                CompositeFilterSetter(command, filterItem)
            Case GetType(CompositeTemplateColumnSqlExpression)
                CompositeFilterBySqlExpSetter(command, filterItem)
            Case GetType(CompositeTemplateColumn)
                CompositeTemplateFilterSetter(command, filterItem)
            Case Else
                SimpleFilterSetter(command, filterItem)
        End Select
    End Sub

#End Region

End Class
