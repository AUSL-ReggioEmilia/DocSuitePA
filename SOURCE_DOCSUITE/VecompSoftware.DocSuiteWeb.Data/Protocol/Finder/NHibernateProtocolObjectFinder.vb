Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports NHibernate.Transform

<Serializable(), DataObject()> _
Public Class NHibernateProtocolObjectFinder
    Inherits NHibernateBaseFinder(Of Protocol, ProtocolHeader)

#Region "New"
    Public Sub New()
        SessionFactoryName = "ProtDB"
    End Sub
#End Region

#Region "Private Fields"
    Private _year As Nullable(Of Short)
    Private _numberFrom As Integer?
    Private _numberTo As Integer?
    Private _registrationDateFrom As Date?
    Private _registrationDateTo As Date?
    Private _idContainer As String
    Private _protocolObject As String = String.Empty
    Private _loadProtocolObjectToImport As Boolean = False
#End Region

#Region "Criteria"
    ''' <summary>
    ''' Crea il criteria per NHibernate
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "P")

        'Filtro Anno
        If Not (String.IsNullOrEmpty(Year)) Then
            criteria.Add(Restrictions.Eq("P.Year", _year))
        End If

        'Filtro Numero Da
        If Not (String.IsNullOrEmpty(NumberFrom)) Then
            criteria.Add(Restrictions.Ge("P.Number", _numberFrom))
        End If

        'Filtro Numero A
        If Not (String.IsNullOrEmpty(NumberTo)) Then
            criteria.Add(Restrictions.Le("P.Number", _numberTo))
        End If

        'Filtro Data di Registrazione a partire da
        If RegistrationDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("P.RegistrationDate", New DateTimeOffset(_registrationDateFrom)))
        End If

        'Filtro Data di Registrazione fino a
        If RegistrationDateTo.HasValue Then
            criteria.Add(Restrictions.Le("P.RegistrationDate", New DateTimeOffset(_registrationDateTo)))
        End If

        'Container
        'Se nessun container impostato allora in join con tutti i contenitori
        If Not (String.IsNullOrEmpty(IdContainer)) Then
            criteria.CreateAlias("P.Container", "Container", SqlCommand.JoinType.LeftOuterJoin)
            criteria.Add(Restrictions.Eq("Container.Id", Integer.Parse(IdContainer)))
        End If

        'Object
        If Not (String.IsNullOrEmpty(ProtocolObject)) Then
            criteria.Add(Restrictions.Eq("P.ProtocolObject", ProtocolObject))
        End If


        If LoadOnlyProtocolObjectToImport Then
            criteria.Add(Expression.Sql("CHARINDEX('|',FullIncrementalPath, CHARINDEX('|',FullIncrementalPath,CHARINDEX('|',FullIncrementalPath) + 1) + 1) > 0"))
        Else
            Dim dcCat As DetachedCriteria = DetachedCriteria.For(GetType(Category), "Category")
            dcCat.SetProjection(Projections.Id())
            dcCat.Add(Restrictions.EqProperty("Category.Id", "P.Category.Id"))
            dcCat.Add(Expression.Sql("CHARINDEX('|', FullIncrementalPath, CHARINDEX('|'," & _
                "FullIncrementalPath) + 1) > 0   AND CHARINDEX('|', FullIncrementalPath, CHARINDEX('|', FullIncrementalPath, CHARINDEX('|', FullIncrementalPath) + 1) + 1) = 0"))
            criteria.Add(Subqueries.Exists(dcCat))
        End If

        AttachFilterExpressions(criteria)
        AttachSQLExpressions(criteria)

        Return criteria
    End Function
#End Region

#Region "Count"
    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        Dim countRecords As Integer = 0

        criteria.SetProjection(Projections.Distinct(Projections.ProjectionList() _
                .Add(Projections.Property("P.Year"), "Year") _
                .Add(Projections.Property("P.Number"), "Number")))
        criteria.SetProjection(Projections.Count("P.Year"))

        countRecords = criteria.UniqueResult(Of Integer)()
        Return countRecords
    End Function
#End Region

#Region "Criteria Decorator"
    Private Sub DecorateCriteria(ByRef criteria As ICriteria)
        If Not MyBase.AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Ascending)
        End If
        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)
        'criteria.SetResultTransformer(New Transform.DistinctRootEntityResultTransformer())
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
    End Sub
#End Region

#Region "IFinder Implementation"
    Public Overrides Function DoSearch() As IList(Of Protocol)
        Dim criteria As ICriteria = CreateCriteria()

        'Decora il criterio con Expressioni di ordinamento e modalità Fetch
        DecorateCriteria(criteria)
        'Crea le eventuali proiezioni
        CreateProjections(criteria)

        Return criteria.List(Of Protocol)()
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of ProtocolHeader)
        Dim criteria As ICriteria = CreateCriteria()

        'Decora il criterio con Expressioni di ordinamento e modalità Fetch
        DecorateCriteria(criteria)

        Dim projList As ProjectionList = Projections.ProjectionList()
        projList.Add(Projections.Property("Year"), "Year")
        projList.Add(Projections.Property("Number"), "Number")
        projList.Add(Projections.Property("RegistrationDate"), "RegistrationDate")
        projList.Add(Projections.Property("RegistrationUser"), "RegistrationUser")
        projList.Add(Projections.Property("ProtocolObject"), "ProtocolObject")
        projList.Add(Projections.Property("IdDocument"), "IdDocument")
        projList.Add(Projections.Property("IdAttachments"), "IdAttachments")
        projList.Add(Projections.Property("DocumentCode"), "DocumentCode")
        projList.Add(Projections.Property("Type"), "Type")
        projList.Add(Projections.Property("IdStatus"), "IdStatus")
        projList.Add(Projections.Property("Category"), "Category")
        projList.Add(Projections.Property("Container"), "Container")
        projList.Add(Projections.Property("Location"), "Location")
        projList.Add(Projections.Property("DocumentProtocol"), "DocumentProtocol")

        criteria.SetProjection(projList)

        criteria.SetResultTransformer(New NHibernate.Transform.AliasToBeanResultTransformer(GetType(ProtocolHeader)))

        Return criteria.List(Of ProtocolHeader)()
    End Function
#End Region

#Region "Finder Properties"
    Public Property Year() As String
        Get
            Return _year.ToString()
        End Get
        Set(ByVal value As String)
            _year = ConvertToShort(value)
        End Set
    End Property

    Public Property NumberFrom() As String
        Get
            Return _numberFrom.ToString()
        End Get
        Set(ByVal value As String)
            _numberFrom = ConvertToInteger(value)
        End Set
    End Property

    Public Property NumberTo() As String
        Get
            Return _numberTo.ToString()
        End Get
        Set(ByVal value As String)
            _numberTo = ConvertToInteger(value)
        End Set
    End Property

    Public Property RegistrationDateFrom() As Date?
        Get
            Return _registrationDateFrom
        End Get
        Set(ByVal value As Date?)
            _registrationDateFrom = value
        End Set
    End Property

    Public Property RegistrationDateTo() As Date?
        Get
            Return _registrationDateTo
        End Get
        Set(ByVal value As Date?)
            _registrationDateTo = value
        End Set
    End Property

    Property IdContainer() As String
        Get
            Return _idContainer
        End Get
        Set(ByVal value As String)
            _idContainer = value
        End Set
    End Property

    Property ProtocolObject() As String
        Get
            Return _protocolObject
        End Get
        Set(ByVal value As String)
            _protocolObject = value
        End Set
    End Property

    Property LoadOnlyProtocolObjectToImport() As Boolean
        Get
            Return _loadProtocolObjectToImport
        End Get
        Set(ByVal value As Boolean)
            _loadProtocolObjectToImport = value
        End Set
    End Property
#End Region

#Region "NHibernate Properties"
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property
#End Region

End Class

