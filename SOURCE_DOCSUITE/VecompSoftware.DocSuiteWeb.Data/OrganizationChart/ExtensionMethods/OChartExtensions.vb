Imports System.Linq
Imports System.Runtime.CompilerServices
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module OChartExtensions

    <Extension()>
    Public Function OrderChronologically(ByVal source As IEnumerable(Of OChart)) As IEnumerable(Of OChart)
        Return source.OrderBy(Function(o) o.StartDateOrDefault) _
            .ThenBy(Function(o) o.EndDateOrDefault) _
            .ThenByDescending(Function(o) o.Id.ToSqlGuid())
    End Function

    <Extension()>
    Public Function PreviousOrDefault(ByVal source As IEnumerable(Of OChart), header As OChart) As OChart
        Dim current As New OChart(header, DocSuiteContext.Current.User.FullUserName)
        current.Id = Guid.NewGuid()

        Dim list As New List(Of OChart)(source)
        list.Add(current)
        Dim ordered As IEnumerable(Of OChart) = list.OrderChronologically()
        Dim previous As IEnumerable(Of OChart) = ordered.TakeWhile(Function(o) Not o.Id.Equals(current.Id))
        If previous.IsNullOrEmpty() Then
            Return header
        End If
        Return previous.Last()
    End Function
    <Extension()>
    Public Function EffectiveOrDefault(ByVal source As IEnumerable(Of OChart)) As OChart
        Return source.Where(Function(o) o.IsEffective) _
            .OrderByDescending(Function(o) o.StartDateOrDefault) _
            .ThenBy(Function(o) o.EndDateOrDefault) _
            .ThenByDescending(Function(o) o.Id) _
            .FirstOrDefault()
    End Function

    <Extension()>
    Public Function IsSame(source As OChart, current As OChart) As Boolean
        Return source.StartDateOrDefault = current.StartDateOrDefault AndAlso source.EndDateOrDefault = current.EndDateOrDefault
    End Function
    <Extension()>
    Public Function IsSame(source As IEnumerable(Of OChart), current As OChart) As IEnumerable(Of OChart)
        Return source.Where(Function(o) o.IsSame(current))
    End Function

    <Extension()>
    Public Function IsUpcoming(source As OChart, current As OChart) As Boolean
        Return source.StartDateOrDefault >= current.StartDateOrDefault
    End Function
    <Extension()>
    Public Function IsUpcoming(source As IEnumerable(Of OChart), current As OChart) As IEnumerable(Of OChart)
        Return source.Where(Function(o) o.IsUpcoming(current))
    End Function

    <Extension()>
    Public Function FindByHeader(source As IEnumerable(Of OChart), header As OChart) As OChart
        Dim found As OChart = source.FirstOrDefault(Function(t) t.Id = header.Id)
        If found IsNot Nothing Then
            Return found
        End If

        Dim list As IEnumerable(Of OChart) = source.Where(Function(t) t.IsSame(header))
        Return list.OrderByDescending(Function(t) t.Id.ToSqlGuid()).FirstOrDefault()
    End Function

End Module
