Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()>
Public Class UnifiedDiaryFacade
    Inherits FacadeNHibernateBase(Of UnifiedDiary, Int32, NHibernateUnifiedDiaryDao)
    Implements IFinder

#Region "Constructor"
    Public Sub New()
        MyBase.New()
        _dbName = ProtDB
        _unitOfWork = New NHibernateUnitOfWork(_dbName)
    End Sub

    Public Sub New(factory As FacadeFactory)
        MyBase.New(factory)
    End Sub

    Public Sub New(ByVal dbName As String, factory As FacadeFactory)
        MyBase.New(dbName, factory)
    End Sub
#End Region

#Region "Methods"

    Public Function GetEntitiesLastLogs(ByVal fromDate As Date, ByVal toDate As Date, ByVal userName As String, currentTenantAOOId As Guid, Optional type As Integer? = Nothing, Optional subject As String = "") As IList(Of UnifiedDiary)
        _lastResult = _dao.GetLastLogsEntities(type, fromDate, toDate.AddDays(1).AddSeconds(-1), userName, currentTenantAOOId, subject)
        Dim cc As String
        'ATTENZIONE SERVE PER IMPEDIRE L'ERRORE DI NO-SESSION/LASY LOADING ERROR DURANTE LA PAGINAZIONE DELLA GRID.
        For Each log As UnifiedDiary In _lastResult
            If (log.Message IsNot Nothing) Then
                For Each recipient As MessageContact In log.Message.MessageContacts
                    For Each email As MessageContactEmail In recipient.ContactEmails
                        cc = email.Email
                    Next
                Next
                For Each message As MessageEmail In log.Message.Emails
                    cc = message.Subject
                Next
            End If
            If (log.DocumentSeriesItem IsNot Nothing) Then
                cc = log.DocumentSeriesItem.DocumentSeries.Name
            End If
            If (log.Document IsNot Nothing) Then
                cc = log.Document.Category.Name
            End If
        Next
        Return _lastResult
    End Function

    Public Function GetEntityLogDetails(ByVal type As Integer, ByVal fromDate As Date, ByVal toDate As Date, ByVal userName As String, ByVal idEntity As Integer, udsId As Guid?, currentTenantAOOId As Guid) As IList(Of UnifiedDiary)
        Return GetEntityLogDetails(type, fromDate, toDate.AddDays(1).AddSeconds(-1), userName, idEntity, Nothing, udsId, currentTenantAOOId)
    End Function

    Public Function GetEntityLogDetails(ByVal type As Integer, ByVal fromDate As Date, ByVal toDate As Date, ByVal userName As String, ByVal year As Integer, ByVal number As Integer?, udsId As Guid?, currentTenantAOOId As Guid) As IList(Of UnifiedDiary)
        _lastResult = _dao.GetLogDetailsByEntity(type, fromDate, toDate.AddDays(1).AddSeconds(-1), userName, year, number, udsId, currentTenantAOOId)
        Return _lastResult
    End Function

#End Region

#Region " Finder Implementations "

    Protected _lastResult As IList(Of UnifiedDiary) = New List(Of UnifiedDiary)()

    Protected _startIndex As Integer = 0

    Protected _pageSize As Integer = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords

    Protected _count As Integer = 0

    Protected _enableTableJoin As Boolean = True

    Protected _enableFetchMode As Boolean = True

    Public Property PageIndex() As Integer Implements IFinder.PageIndex
        Get
            Return _startIndex
        End Get
        Set(ByVal value As Integer)
            _startIndex = value
        End Set
    End Property

    Public Property CustomPageIndex As Integer Implements IFinder.CustomPageIndex
        Get
            Return Math.Ceiling(PageIndex / PageSize)
        End Get
        Set(value As Integer)
            PageIndex = value * PageSize
        End Set
    End Property

    Public Property PageSize() As Integer Implements IFinder.PageSize
        Get
            Return _pageSize
        End Get
        Set(ByVal value As Integer)
            _pageSize = value
        End Set
    End Property

    ''' <summary> Numero dei risultati ottenuti dalla ricerca. </summary>
    ''' <returns>Il numero dei record ottenuti dalla ricerca</returns>
    Public Overridable Function Count() As Integer Implements IFinder.Count
        Return If(_lastResult Is Nothing, 0, _lastResult.Count)
    End Function

    Public ReadOnly Property FilterExpressions() As IDictionary(Of String, IFilterExpression) Implements IFinder.FilterExpressions
        Get
            Return New Dictionary(Of String, IFilterExpression)
        End Get
    End Property

    ''' <summary>Dizionario di tutti gli ordinamenti</summary>
    Public ReadOnly Property SortExpressions() As IDictionary(Of String, String) Implements IFinder.SortExpressions
        Get
            Return New Dictionary(Of String, String)
        End Get
    End Property


    Public Property EnableFetchMode() As Boolean
        Get
            Return _enableFetchMode
        End Get
        Set(ByVal value As Boolean)
            _enableFetchMode = value
        End Set
    End Property

    Public Property EnableTableJoin() As Boolean
        Get
            Return _enableTableJoin
        End Get
        Set(ByVal value As Boolean)
            _enableTableJoin = value
        End Set
    End Property

    Public Function DoSearchHeader() As IList(Of UnifiedDiary)
        Dim partialResults As IEnumerable(Of UnifiedDiary) = _lastResult.Skip(PageIndex).Take(PageSize)
        Return partialResults.ToList()
    End Function
#End Region

End Class
