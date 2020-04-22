Imports VecompSoftware.DocSuiteWeb.Data

Public Class ContactNameFacade
    Inherits CommonFacade(Of ContactName, Integer, NHibernateContactNameDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

#Region " Methods "

    Public Function GetContactNameByIncremental(ByVal Incremental As Integer) As ContactName
        Return _dao.GetContactNameByIncremental(Incremental)
    End Function

    Public Function GetContactNamesByIncremental(ByVal Incremental As Integer) As IList(Of ContactName)
        Return _dao.GetContactNamesByIncremental(Incremental)
    End Function

    Public Function GetContactNamesByIncrementalForDate(ByVal Incremental As Integer) As IList(Of ContactName)
        Return _dao.GetContactNamesByIncrementalForDate(Incremental)
    End Function

    Public Function GetContactNamesByValidDate(ByVal Incremental As Integer, ByVal SelectedDate As DateTime) As ContactName
        Return _dao.GetContactNamesByValidDate(Incremental, SelectedDate)
    End Function

    Public Function GetContactNamesHistoryByValidDate(ByVal Incremental As Integer, ByVal SelectedDate As DateTime) As ContactName
        Return _dao.GetContactNamesHistoryByValidDate(Incremental, SelectedDate)
    End Function

    Public Function GetContactNamesOlder(ByVal Incremental As Integer) As ContactName
        Return _dao.GetContactNamesOlder(Incremental)
    End Function

#End Region


End Class
