Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateTableLogFinder
    Inherits NHibernateBaseLogFinder(Of TableLog)

    Private _tableName As String
    Private _entityId As Integer?
    Private _entityUniqueId As Guid?

#Region " New "

    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
    End Sub

#End Region

#Region " Properties "
    ''' <summary>
    ''' Nome della tabella su cui effettuare la ricerca
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property TableName() As String
        Get
            Return _tableName
        End Get
        Set(ByVal value As String)
            _tableName = value
        End Set
    End Property

    ''' <summary>
    '''Id di riferimento
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property EntityId() As Integer?
        Get
            Return _entityId
        End Get
        Set(ByVal value As Integer?)
            _entityId = value
        End Set
    End Property
    Public Property EntityUniqueId() As Guid?
        Get
            Return _entityUniqueId
        End Get
        Set(ByVal value As Guid?)
            _entityUniqueId = value
        End Set
    End Property

#End Region

#Region " Virtual method overrides"

    ''' <summary>
    ''' Aggiunta del criterio TableName ai criteri di ricerca comuni
    ''' </summary>
    ''' <returns>La lista dei criteria necessari per la ricerca in TableLogs</returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = MyBase.CreateCriteria()

        If TableName <> String.Empty Then
            criteria.Add(Expression.Like("D.TableName", TableName))
        End If

        If EntityId.HasValue Then
            criteria.Add(Restrictions.Eq("D.EntityId", EntityId))
        End If

        If EntityUniqueId.HasValue Then
            criteria.Add(Restrictions.Eq("D.EntityUniqueId", EntityUniqueId))
        End If
        Return criteria
    End Function

    ''' <summary>
    ''' Esegue la ricerca senza limiti di paginazione 
    ''' </summary>
    ''' <returns>La lista dei TableLog che soddisfano i criteri di ricerca</returns>
    ''' <remarks></remarks>
    Public Overrides Function DoSearch() As System.Collections.Generic.IList(Of TableLog)

        Dim criteria As ICriteria = CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "RegistrationDate", SortOrder.Descending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of TableLog)()
    End Function

#End Region

#Region " Virtual methods overload"
    ''' <summary>
    ''' Ricerca con ordinamento
    ''' </summary>
    ''' <param name="SortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <returns>Una lista ordinata di TableLog </returns>
    ''' <remarks></remarks>
    Public Overloads Function DoSearch(ByVal SortExpr As String) As System.Collections.Generic.IList(Of TableLog)
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
    Public Overloads Function DoSearch(ByVal SortExpr As String, ByVal StartRow As Integer, ByVal PageSize As Integer) As System.Collections.Generic.IList(Of TableLog)
        MyBase.DoSearch(SortExpr, StartRow, PageSize)
        Return DoSearch()
    End Function
#End Region

#Region " NHibernate Properties"
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
