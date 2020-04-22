Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports System.Linq
Imports NHibernate.Impl.CriteriaImpl
Imports NHibernate.Impl

<Serializable(), DataObject()> _
Public Class NHibernateDocumentVersioningFinder
    Inherits NHibernateBaseFinder(Of DocumentVersioning, DocumentVersioning)


#Region " New "

    Public Sub New()
        SessionFactoryName = "DocmDB"
    End Sub

#End Region

#Region " Private members "
    Private _docYear As Short
    Private _docNumber As Integer
    Private _docIncremental As Short
    Private _docCheckStatus As String
    Private _TypeSearch As Integer
#End Region

#Region " Properties "

    ''' <summary>m
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

    ''' <summary>
    ''' Incremento del documento da ricercare
    ''' </summary>
    ''' <value>Short</value>
    ''' <returns></returns>
    ''' <remarks>Short</remarks>
    Public Property DocumentIncr() As Short
        Get
            Return _docIncremental
        End Get
        Set(ByVal value As Short)
            _docIncremental = value
        End Set
    End Property

    ''' <summary>
    ''' Valore che indica la tipologia di ricerca
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns></returns>
    ''' <remarks>Boolean</remarks>
    Public Property TypeSearch() As Integer
        Get
            Return _TypeSearch
        End Get
        Set(ByVal value As Integer)
            _TypeSearch = value
        End Set
    End Property

    ''' <summary>
    ''' Valore che indica lo stato del documento in versioning
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks>String</remarks>
    Public Property DocumentCheckStatus() As String
        Get
            Return _docCheckStatus
        End Get
        Set(ByVal value As String)
            _docCheckStatus = value
        End Set
    End Property

#End Region

#Region " Virtual method overrides"

    ''' <summary>
    ''' Aggiunta dei filtri anno e numero ai criteri di ricerca comuni
    ''' </summary>
    ''' <returns>La lista dei criteria necessari per la ricerca dei DocumentVersioning</returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType)


        Select Case TypeSearch
            Case 0 'Ricerca base 
                If DocumentYear <> 0 Then
                    criteria.Add(Restrictions.Eq("Year", DocumentYear))
                End If

                If DocumentNumber <> 0 Then
                    criteria.Add(Restrictions.Eq("Number", DocumentNumber))
                End If

                If DocumentIncr <> 0 Then
                    criteria.Add(Restrictions.Eq("Incremental", DocumentIncr))
                End If

            Case 1 ' Ricerca con lo stato
                If DocumentYear <> 0 Then
                    criteria.Add(Restrictions.Eq("Year", DocumentYear))
                End If

                If DocumentNumber <> 0 Then
                    criteria.Add(Restrictions.Eq("Number", DocumentNumber))
                End If

                If DocumentIncr <> 0 Then
                    criteria.Add(Restrictions.Eq("Incremental", DocumentIncr))
                End If

                If Not String.IsNullOrEmpty(DocumentCheckStatus) Then
                    criteria.Add(Restrictions.Eq("CheckStatus", DocumentCheckStatus))
                End If

        End Select

        AttachFilterExpressions(criteria)

        Return criteria

    End Function

    ''' <summary>
    ''' Ricerca senza limiti di paginazione 
    ''' </summary>
    ''' <returns>La lista dei DocumentVersioning che soddisfano i criteri di ricerca</returns>
    ''' <remarks></remarks>
    Public Overrides Function DoSearch() As IList(Of DocumentVersioning)

        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            Dim orderList As IList(Of OrderEntry) = CType(criteria, CriteriaImpl).IterateOrderings().ToList()
            orderList.Clear()
            MyBase.AttachSortExpressions(criteria)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of DocumentVersioning)()

    End Function

#End Region

#Region " Virtual methods overload"

    ''' <summary>
    '''  Ricerca con ordinamento, riga di partenza e dimensione della paginazione 
    ''' </summary>
    ''' <param name="SortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="StartRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="PageSize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    ''' <remarks></remarks>
    Public Overloads Function DoSearch(ByVal SortExpr As String, ByVal StartRow As Integer, ByVal PageSize As Integer) As IList(Of DocumentVersioning)
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
