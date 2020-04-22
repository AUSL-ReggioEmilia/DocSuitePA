Imports System.IO
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Public Class GridSettingsPersister

    'this method should be called on Render
    Public Shared Function SaveSettings(ByRef gridInstance As BindGrid) As String
        Dim gridSettings() As Object = New Object((4) - 1) {}
        'Save groupBy
        Dim groupByExpressions As GridGroupByExpressionCollection = gridInstance.MasterTableView.GroupByExpressions
        Dim groupExpressions() As Object = New Object((groupByExpressions.Count) - 1) {}
        Dim count As Integer = 0
        For Each expression As GridGroupByExpression In groupByExpressions
            groupExpressions(count) = CType(expression, IStateManager).SaveViewState
            count = (count + 1)
        Next
        gridSettings(0) = groupExpressions
        'Save sort expressions
        gridSettings(1) = CType(gridInstance.MasterTableView.SortExpressions, IStateManager).SaveViewState
        'Save page index
        gridSettings(2) = gridInstance.CustomPageIndex
        'Save filter expression
        gridSettings(3) = gridInstance.Finder.FilterExpressions

        Dim formatter As LosFormatter = New LosFormatter
        Dim writer As StringWriter = New StringWriter
        formatter.Serialize(writer, gridSettings)

        Return writer.ToString()
    End Function

    'this method should be called on PageInit
    Public Shared Sub LoadSettings(ByRef gridInstance As BindGrid, ByVal settings As String)
        If Not String.IsNullOrEmpty(settings) Then
            Dim formatter As LosFormatter = New LosFormatter
            Dim reader As StringReader = New StringReader(settings)
            Dim gridSettings() As Object = CType(formatter.Deserialize(reader), Object())
            'Load groupBy
            Dim groupByExpressions As GridGroupByExpressionCollection = gridInstance.MasterTableView.GroupByExpressions
            groupByExpressions.Clear()
            Dim groupExpressionsState() As Object = CType(gridSettings(0), Object())
            Dim count As Integer = 0
            For Each obj As Object In groupExpressionsState
                Dim expression As GridGroupByExpression = New GridGroupByExpression
                CType(expression, IStateManager).LoadViewState(obj)
                groupByExpressions.Add(expression)
                count = (count + 1)
            Next
            'Load sort expressions
            gridInstance.MasterTableView.SortExpressions.Clear()
            CType(gridInstance.MasterTableView.SortExpressions, IStateManager).LoadViewState(gridSettings(1))
            'Load page index
            gridInstance.CustomPageIndex = gridSettings(2)
            'Load filter expression
            If gridInstance.Finder IsNot Nothing Then
                Dim dic As IDictionary(Of String, IFilterExpression) = CType(gridSettings(3), IDictionary(Of String, IFilterExpression))
                For Each key As String In dic.Keys
                    gridInstance.Finder.FilterExpressions.Add(key, dic(key))
                Next
            End If
        End If
    End Sub
End Class
