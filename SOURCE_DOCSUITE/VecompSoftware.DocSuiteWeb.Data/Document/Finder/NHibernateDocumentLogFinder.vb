Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateDocumentLogFinder
    Inherits NHibernateBaseLogFinder(Of DocumentLog)


#Region " New "

    Public Sub New()
        SessionFactoryName = "DocmDB"
    End Sub

#End Region

#Region " Private members "
    Private _docYear As Short
    Private _docNumber As Integer
#End Region

#Region " Properties "

    ''' <summary>
    ''' Anno del documento da ricercare
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns></returns>
    ''' <remarks>Integer</remarks>
    Public Property DocumentYear() As Short
        Get
            Return _docYear
        End Get
        Set(ByVal value As Short)
            _docYear = value
        End Set
    End Property

    ''' <summary>
    ''' Numero del documento da ricercare
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns></returns>
    ''' <remarks>Integer</remarks>
    Public Property DocumentNumber() As Integer
        Get
            Return _docNumber
        End Get
        Set(ByVal value As Integer)
            _docNumber = value
        End Set
    End Property

#End Region

#Region " Virtual method overrides"

    ''' <summary>
    ''' Aggiunta dei filtri anno e numero ai criteri di ricerca comuni
    ''' </summary>
    ''' <returns>La lista dei criteria necessari per la ricerca dei DocumentLogs</returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria

        Dim criteria As ICriteria = MyBase.CreateCriteria()

        If DocumentYear <> 0 Then
            criteria.Add(Expression.Like("D.Year", DocumentYear))
        End If

        If DocumentNumber <> 0 Then
            criteria.Add(Expression.Like("D.Number", DocumentNumber))
        End If

        Return criteria

    End Function

    ''' <summary>
    ''' Ricerca senza limiti di paginazione 
    ''' </summary>
    ''' <returns>La lista dei DocumentLog che soddisfano i criteri di ricerca</returns>
    ''' <remarks></remarks>
    Public Overrides Function DoSearch() As System.Collections.Generic.IList(Of DocumentLog)

        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "LogDate", SortOrder.Descending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of DocumentLog)()

    End Function

#End Region

#Region " Virtual methods overload"
    ''' <summary>
    ''' Ricerca con ordinamento
    ''' </summary>
    ''' <param name="SortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <returns>Una lista ordinata di DocumentLog </returns>
    ''' <remarks></remarks>
    Public Overloads Function DoSearch(ByVal SortExpr As String) As System.Collections.Generic.IList(Of DocumentLog)
        MyBase.DoSearch(SortExpr)
        Return DoSearch()
    End Function

    ''' <summary>
    '''  Ricerca con ordinamento, riga di partenza e dimensione della paginazione 
    ''' </summary>
    ''' <param name="SortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="StartRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="PageSize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    ''' <remarks></remarks>
    Public Overloads Function DoSearch(ByVal SortExpr As String, ByVal StartRow As Integer, ByVal PageSize As Integer) As System.Collections.Generic.IList(Of DocumentLog)
        Dim criteria As ICriteria = CreateCriteria()
        MyBase.DoSearch(SortExpr, StartRow, PageSize)
        Return DoSearch()
    End Function
#End Region

#Region " NHibernate Properties "
    ''' <summary>
    ''' Ritorna la sessione hibernate corrente
    ''' </summary>
    ''' <returns>ISession</returns>
    ''' <remarks>ReadOnly</remarks>
    Protected Overloads ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

End Class
