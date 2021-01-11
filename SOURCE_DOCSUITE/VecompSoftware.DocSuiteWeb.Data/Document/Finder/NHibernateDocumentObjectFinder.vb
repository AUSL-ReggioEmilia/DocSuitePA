Imports NHibernate
Imports System.Linq
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports NHibernate.Criterion
Imports NHibernate.Impl.CriteriaImpl
Imports NHibernate.Impl

<Serializable(), DataObject()> _
Public Class NHibernateDocumentObjectFinder
    Inherits NHibernateBaseFinder(Of DocumentObject, DocumentObject)


#Region " New "

    Public Sub New()
        SessionFactoryName = "DocmDB"
    End Sub

#End Region

#Region " Private members "
    Private _docYear As Short
    Private _docNumber As Integer
    Private _TypeSearch As Integer
#End Region

#Region " Properties "

    ''' <summary> Anno del documento da ricercare </summary>
    Public Property DocumentYear() As Short
        Get
            Return _docYear
        End Get
        Set(ByVal value As Short)
            _docYear = value
        End Set
    End Property

    ''' <summary> Numero del documento da ricercare </summary>
    Public Property DocumentNumber() As Integer
        Get
            Return _docNumber
        End Get
        Set(ByVal value As Integer)
            _docNumber = value
        End Set
    End Property

    ''' <summary> Incremento del documento da ricercare </summary>
    Public Property DocumentIncr() As Short?

    ''' <summary> Valore che indica la tipologia di ricerca </summary>
    Public Property TypeSearch() As Integer
        Get
            Return _TypeSearch
        End Get
        Set(ByVal value As Integer)
            _TypeSearch = value
        End Set
    End Property

#End Region

#Region " Virtual method overrides"

    ''' <summary> Aggiunta dei filtri anno e numero ai criteri di ricerca comuni </summary>
    ''' <returns>La lista dei criteria necessari per la ricerca dei DocumentObject</returns>
    Protected Overrides Function CreateCriteria() As ICriteria

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType)

        Select Case TypeSearch
            Case 0
                If DocumentYear <> 0 Then
                    criteria.Add(Restrictions.Eq("Year", DocumentYear))
                End If

                If DocumentNumber <> 0 Then
                    criteria.Add(Restrictions.Eq("Number", DocumentNumber))
                End If

                If DocumentIncr.HasValue Then
                    criteria.Add(Restrictions.Eq("Incremental", DocumentIncr.Value))
                End If

            Case 1
                If DocumentIncr.HasValue Then
                    criteria.Add(Restrictions.Eq("IncrementalFolder", DocumentIncr.Value))
                End If

                NHibernateSession.EnableFilter("Status").SetParameter("CheckStatus", "A")

                criteria.CreateAlias("DocumentVersionings", "DV", SqlCommand.JoinType.LeftOuterJoin)

                criteria.Add(Restrictions.Eq("id.Year", DocumentYear))
                criteria.Add(Restrictions.Eq("id.Number", DocumentNumber))

                criteria.Add(Restrictions.IsNull("ValidIncremental"))

            Case 2
                criteria.Add(Restrictions.Eq("Id.Year", DocumentYear))
                criteria.Add(Restrictions.Eq("Id.Number", DocumentNumber))

                If DocumentIncr.HasValue Then
                    criteria.Add(Restrictions.Or(Restrictions.Eq("Id.Incremental", DocumentIncr.Value), Restrictions.Eq("ValidIncremental", DocumentIncr.Value)))
                End If

        End Select

        AttachFilterExpressions(criteria)

        Return criteria

    End Function

    ''' <summary> Ricerca senza limiti di paginazione </summary>
    ''' <returns> La lista dei DocumentObject che soddisfano i criteri di ricerca </returns>
    Public Overrides Function DoSearch() As IList(Of DocumentObject)
        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            Dim orderList As IList(Of OrderEntry) = CType(criteria, CriteriaImpl).IterateOrderings().ToList()
            orderList.Clear()
            criteria.AddOrder(Order.Asc("OrdinalPosition"))
            MyBase.AttachSortExpressions(criteria)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of DocumentObject)()
    End Function

#End Region

#Region " Virtual methods overload"

    ''' <summary> Ricerca con ordinamento </summary>
    ''' <param name="SortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    Public Overloads Function DoSearch(ByVal SortExpr As String) As IList(Of DocumentObject)
        MyBase.DoSearch(SortExpr)
        Return DoSearch()
    End Function

    ''' <summary>  Ricerca con ordinamento, riga di partenza e dimensione della paginazione </summary>
    ''' <param name="SortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="StartRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="PageSize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    Public Overloads Function DoSearch(ByVal SortExpr As String, ByVal StartRow As Integer, ByVal PageSize As Integer) As IList(Of DocumentObject)
        Dim criteria As ICriteria = CreateCriteria()
        MyBase.DoSearch(SortExpr, StartRow, PageSize)
        Return DoSearch()
    End Function

#End Region

#Region " NHibernate Properties "

    ''' <summary> Ritorna la sessione hibernate corrente </summary>
    Protected Overloads ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

End Class
