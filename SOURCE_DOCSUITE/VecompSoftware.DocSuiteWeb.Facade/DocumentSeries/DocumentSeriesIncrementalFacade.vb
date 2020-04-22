Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentSeriesIncrementalFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesIncremental, Integer, NHibernateDocumentSeriesIncrementalDao)

#Region " Invalid Operations "

    Private invalidOperation As New InvalidOperationException("Operazione non consentita per questo Facade")

    Public Overrides Sub Recover(ByRef obj As DocumentSeriesIncremental)
        Throw invalidOperation
    End Sub
    Public Overrides Sub Recover(ByRef obj As DocumentSeriesIncremental, dbName As String)
        Throw invalidOperation
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef obj As DocumentSeriesIncremental)
        Throw invalidOperation
    End Sub
    Public Overrides Sub UpdateNoLastChange(ByRef obj As DocumentSeriesIncremental, dbName As String)
        Throw invalidOperation
    End Sub
    Public Overrides Sub UpdateOnly(ByRef obj As DocumentSeriesIncremental)
        Throw invalidOperation
    End Sub
    Public Overrides Sub UpdateOnly(ByRef obj As DocumentSeriesIncremental, dbName As String, Optional needTransaction As Boolean = True)
        Throw invalidOperation
    End Sub

    Public Overrides Function Delete(ByRef obj As DocumentSeriesIncremental) As Boolean
        Throw invalidOperation
    End Function
    Public Overrides Function Delete(ByRef obj As DocumentSeriesIncremental, dbName As String, Optional needTransaction As Boolean = True) As Boolean
        Throw invalidOperation
    End Function

#End Region


    Public Function GetNewIncremental(series As DocumentSeries, year As Integer) As DocumentSeriesIncremental
        Return _dao.GetNewIncremental(series, year)
    End Function
    Public Function GetNewIncremental(series As DocumentSeries) As DocumentSeriesIncremental
        Return GetNewIncremental(series, _dao.GetServerDate().Year)
    End Function

    Public Function GetNewIncrementalNumber(series As DocumentSeries, year As Integer) As Integer?
        Return GetNewIncremental(series, year).LastUsedNumber
    End Function
    Public Function GetNewIncrementalNumber(series As DocumentSeries) As Integer?
        Return GetNewIncremental(series).LastUsedNumber
    End Function
    Public Function GetOpenDocumentIncrementalSeries() As IList(Of DocumentSeriesIncremental)
        Return _dao.GetOpenDocumentSeries()
    End Function
    Public Function GetOpenDocumentIncrementalSeries(idSeries As Integer) As IList(Of DocumentSeriesIncremental)
        Return _dao.GetOpenDocumentSeries(idSeries)
    End Function
    Public Function CountOpenDocumentIncrementalSeries(idSeries As Integer) As Integer
        Return _dao.CountOpenDocumentSeries(idSeries)
    End Function
    Public Function CountOpenDocumentIncrementalSeries(idSeries As Integer, year As Integer) As Integer
        Return _dao.CountOpenDocumentSeries(idSeries)
    End Function
    Public Function CountOpenDocumentIncrementalSeriesByYear(idSeries As Integer, year As Integer) As Integer
        Return _dao.CountOpenDocumentIncrementalSeriesByYear(idSeries, year)
    End Function
    Public Function GetDocumentIncrementalSeriesById(idSeries As Integer) As DocumentSeriesIncremental
        Return _dao.GetDocumentIncrementalSeriesById(idSeries)
    End Function
    Public Function GetAllDocumentIncrementalSeries(idSeries As Integer) As IList(Of DocumentSeriesIncremental)
        Return _dao.GetAllDocumentSeries(idSeries)
    End Function
    Public Sub AddDocumentSeriesIncremental(year As Integer, documentSeries As DocumentSeries)
        Dim docSeries As New DocumentSeriesIncremental
        docSeries.DocumentSeries = documentSeries
        docSeries.IsOpen = True
        docSeries.Year = year
        docSeries.LastUsedNumber = 0
        Save(docSeries)
    End Sub
    Public Sub UpdateDocumentSeriesIncremental(docSeriesIncremental As DocumentSeriesIncremental, isOpen As Boolean)
        With docSeriesIncremental
            .IsOpen = isOpen
        End With
        Update(docSeriesIncremental)
    End Sub
    Public Function GetLastDocumentSeriesIncremental(idSeries As Integer) As DocumentSeriesIncremental
        Return _dao.GetLastDocumentSeriesIncremental(idSeries)
    End Function
End Class