Imports System.Linq
Imports NHibernate.Impl
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateDocumentTokenFinder
    Inherits NHibernateBaseFinder(Of DocumentToken, DocumentToken)

#Region " Fields "

    Private _docYear As Short
    Private _docNumber As Integer
    Private _docType As IList(Of String)

#End Region

#Region " Properties "

    ''' <summary> Ritorna la sessione hibernate corrente </summary>
    Protected Overloads ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

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

    Public Property DocumentType() As IList(Of String)
        Get
            If _docType Is Nothing Then
                _docType = New List(Of String)
            End If
            Return _docType
        End Get
        Set(ByVal value As IList(Of String))
            _docType = value
        End Set
    End Property


#End Region

#Region " Constructors "

    Public Sub New()
        SessionFactoryName = "DocmDB"
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Aggiunta dei filtri anno e numero ai criteri di ricerca comuni </summary>
    ''' <returns> La lista dei criteria necessari per la ricerca dei DocumentToken </returns>
    Protected Overrides Function CreateCriteria() As ICriteria

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "D")

        If DocumentYear <> 0 Then
            criteria.Add(Expression.Like("D.Year", DocumentYear))
        End If

        If DocumentNumber <> 0 Then
            criteria.Add(Expression.Like("D.Number", DocumentNumber))
        End If

        If DocumentType.Count <> 0 Then
            Dim ids() As String
            ReDim ids(DocumentType.Count - 1)
            DocumentType.CopyTo(ids, 0)
            criteria.Add(Expression.In("D.DocumentTabToken.Id", ids))
            criteria.Add(Expression.Gt("D.IsActive", False))
        End If

        AttachFilterExpressions(criteria)

        Return criteria

    End Function

    ''' <summary> Ricerca senza limiti di paginazione  </summary>
    ''' <returns>La lista dei DocumentToken che soddisfano i criteri di ricerca</returns>
    Public Overrides Function DoSearch() As IList(Of DocumentToken)

        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            Dim orderList As IList(Of CriteriaImpl.OrderEntry) = CType(criteria, CriteriaImpl).IterateOrderings().ToList()
            orderList.Clear()
            MyBase.AttachSortExpressions(criteria)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of DocumentToken)()
    End Function

    ''' <summary> Ricerca con ordinamento </summary>
    ''' <param name="SortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    Public Overloads Function DoSearch(ByVal SortExpr As String) As IList(Of DocumentToken)
        MyBase.DoSearch(SortExpr)
        Return DoSearch()
    End Function

    ''' <summary>  Ricerca con ordinamento, riga di partenza e dimensione della paginazione </summary>
    ''' <param name="sortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="startRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="pagsize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    Public Overloads Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pagSize As Integer) As IList(Of DocumentToken)
        Dim criteria As ICriteria = CreateCriteria()
        MyBase.DoSearch(sortExpr, startRow, pagSize)
        Return DoSearch()
    End Function

#End Region

End Class
