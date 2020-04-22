Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports NHibernate.Criterion

<Serializable(), DataObject()> _
Public Class NHibernateDocumentTokenUserFinder
    Inherits NHibernateBaseFinder(Of DocumentTokenUser, DocumentTokenUser)


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
    ''' <returns>La lista dei criteria necessari per la ricerca dei DocumentTokenUser</returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As ICriteria

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "D")

        If DocumentYear <> 0 Then
            criteria.Add(Expression.Like("D.Year", DocumentYear))
        End If

        If DocumentNumber <> 0 Then
            criteria.Add(Expression.Like("D.Number", DocumentNumber))
        End If

        AttachFilterExpressions(criteria)

        Return criteria

    End Function

    ''' <summary>
    ''' Ricerca senza limiti di paginazione 
    ''' </summary>
    ''' <returns>La lista dei DocumentTokenUser che soddisfano i criteri di ricerca</returns>
    ''' <remarks></remarks>
    Public Overrides Function DoSearch() As IList(Of DocumentTokenUser)

        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            Dim orderList As IList = CType(criteria, NHibernate.Impl.CriteriaImpl).IterateOrderings()
            orderList.Clear()
            AttachSortExpressions(criteria)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of DocumentTokenUser)()

    End Function

#End Region

#Region " Virtual methods overload"
    ''' <summary>
    ''' Ricerca con ordinamento
    ''' </summary>
    ''' <param name="SortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <returns>Una lista ordinata di DocumentTokenUser </returns>
    ''' <remarks></remarks>
    Public Overloads Function DoSearch(ByVal SortExpr As String) As IList(Of DocumentTokenUser)
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
    Public Overloads Function DoSearch(ByVal SortExpr As String, ByVal StartRow As Integer, ByVal PageSize As Integer) As IList(Of DocumentTokenUser)
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
