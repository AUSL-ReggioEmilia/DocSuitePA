Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Transformer
Imports NHibernate.Impl
Imports NHibernate.SqlCommand


''' <summary> Classe base che identifica un generico Finder per NHibernate. </summary>
<Serializable(), DataObject()>
Public MustInherit Class NHibernateBaseFinder(Of T, THeader)
    Implements IFinder(Of T)

    ''' <summary> Tipo di ricerca del testo </summary>
    Public Enum ObjectSearchType
        ''' <summary>Tutte le parole</summary>
        AllWords = 1
        ''' <summary>Almeno una parola</summary>
        AtLeastWord = 2
        ''' <summary>Parola esatta</summary>
        ''' <remarks>Usata per SM-SUD</remarks>
        ExactWord = 3
    End Enum

    ''' <summary> Ordinamento </summary>
    ''' <remarks>
    ''' TODO: portare fuori e usare System.Enum.GetName
    ''' </remarks>
    Public Enum SearchOrder
        ASC = 0
        DESC = 1
    End Enum

    Public Enum SortOrder
        Ascending = 0
        Descending = 1
    End Enum

    Public Enum StatusSearchType
        ''' <summary>Anche i Protocolli Annullati</summary>
        EvenStatusCancel = 1
        ''' <summary>Solo i Protocolli Annullati</summary>
        OnlyStatusCancel = 2
        ''' <summary>Solo i Protocolli Attivi</summary>
        OnlyStatusActive = 3

    End Enum

#Region " Fields "

    Private Const SORT_NONE As String = "NONE"

    Protected _filterExpressions As IDictionary(Of String, IFilterExpression)

    Protected _sortExpressions As IDictionary(Of String, String)

    Protected _sqlExpressions As IDictionary(Of String, ISQLExpression)

    Protected _projections As IDictionary(Of String, String)

    Protected _startIndex As Integer = 0

    Protected _pageSize As Integer = 50

    Protected SessionFactoryName As String

    Protected ReadOnly persistentType As System.Type = GetType(T)

    Protected _enableTableJoin As Boolean = True

    Protected _enableFetchMode As Boolean = True
#End Region

#Region "Virtual Functions"
    Protected MustOverride Function CreateCriteria() As ICriteria
    Public MustOverride Function DoSearch() As IList(Of T) Implements IFinder(Of T).DoSearch

    Public Overridable Function DoSearchHeader() As IList(Of THeader)
        Return DoSearch()
    End Function
#End Region

#Region " Properties "

    ''' <summary>Dizionario contenente tutti gli operatori di confronto.</summary>
    ''' <value>
    ''' 1) Proprietà
    ''' 2) Valore
    ''' 3) Expressione
    ''' </value>
    Public ReadOnly Property FilterExpressions() As IDictionary(Of String, IFilterExpression) Implements IFinder(Of T).FilterExpressions
        Get
            If _filterExpressions Is Nothing Then
                _filterExpressions = New Dictionary(Of String, IFilterExpression)
            End If
            Return _filterExpressions
        End Get
    End Property

    ''' <summary>Dizionario di tutti gli ordinamenti</summary>
    Public ReadOnly Property SortExpressions() As IDictionary(Of String, String) Implements IFinder(Of T).SortExpressions
        Get
            If _sortExpressions Is Nothing Then
                _sortExpressions = New Dictionary(Of String, String)
            End If
            Return _sortExpressions
        End Get
    End Property

    ''' <summary>Dizionario di tutte le query SQL assegnate al finder</summary>
    Public ReadOnly Property SQLExpressions() As IDictionary(Of String, ISQLExpression)
        Get
            If _sqlExpressions Is Nothing Then
                _sqlExpressions = New Dictionary(Of String, ISQLExpression)
            End If
            Return _sqlExpressions
        End Get
    End Property

    ''' <summary> Proiezioni. </summary>
    Public ReadOnly Property SelectProjections() As IDictionary(Of String, String)
        Get
            If _projections Is Nothing Then
                _projections = New Dictionary(Of String, String)
            End If
            Return _projections
        End Get
    End Property

    Public Property PageIndex() As Integer Implements IFinder(Of T).PageIndex
        Get
            Return _startIndex
        End Get
        Set(ByVal value As Integer)
            _startIndex = value
        End Set
    End Property

    Public Property CustomPageIndex As Integer Implements IFinder(Of T).CustomPageIndex
        Get
            Return Math.Ceiling(PageIndex / PageSize)
        End Get
        Set(value As Integer)
            PageIndex = value * PageSize
        End Set
    End Property

    Public Property PageSize() As Integer Implements IFinder(Of T).PageSize
        Get
            Return _pageSize
        End Get
        Set(ByVal value As Integer)
            _pageSize = value
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

    Public Property EnableFetchMode() As Boolean
        Get
            Return _enableFetchMode
        End Get
        Set(ByVal value As Boolean)
            _enableFetchMode = value
        End Set
    End Property
#End Region

#Region "Convert Functions"

    ''' <summary> Converte una Stringa in Short. </summary>
    Protected Function ConvertToShort(ByVal value As String) As Nullable(Of Short)
        If Not String.IsNullOrEmpty(value) Then
            Return Convert.ToInt16(value)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary> Converte una stringa in Intero. </summary>
    Protected Function ConvertToInteger(ByVal value As String) As Integer?
        If Not String.IsNullOrEmpty(value) Then
            Return Convert.ToInt32(value)
        Else
            Return Nothing
        End If
    End Function

#End Region

#Region "Util Functions"

    Protected Sub LoadFetchMode(ByRef criteria As ICriteria, ByVal propertyName As String)
        If EnableFetchMode Then
            criteria.SetFetchMode(propertyName, FetchMode.Eager)
        End If
    End Sub

    Protected Function AddJoinAlias(ByRef criteria As ICriteria, ByVal propertyName As String, ByVal [alias] As String, ByVal join As JoinType) As ICriteria
        If EnableTableJoin Then
            Return criteria.CreateAlias(propertyName, [alias], join)
        End If
        Return criteria
    End Function

    ''' <summary>
    ''' Estrae dal <see>CriteriaImpl</see> espressioni, sottocriteri, ordinamenti e proiezioni restituendoli come <see>DetachedCriteria</see>
    ''' </summary>
    Protected Function CreateDetachFromCriteria(ByRef criteria As CriteriaImpl, ByVal persistentType As System.Type) As DetachedCriteria
        Dim dc As DetachedCriteria = Nothing

        If Not criteria Is Nothing Then
            dc = DetachedCriteria.For(persistentType, criteria.Alias)

            'espressioni
            Dim expList As IList = criteria.IterateExpressionEntries()
            If Not expList Is Nothing Then
                For Each exp As CriteriaImpl.CriterionEntry In expList
                    dc.Add(exp.Criterion)
                Next
            End If

            'sottocriteri
            Dim subList As IList = criteria.IterateSubcriteria()
            If Not subList Is Nothing Then
                For Each [sub] As CriteriaImpl.Subcriteria In subList
                    dc.CreateAlias([sub].Path, [sub].Alias, [sub].JoinType)
                Next
            End If

            'ordinamenti
            Dim ordList As IList = criteria.IterateOrderings()
            If Not ordList Is Nothing Then
                For Each ord As CriteriaImpl.OrderEntry In ordList
                    dc.AddOrder(ord.Order)
                Next
            End If

            'proiezioni
            dc.SetProjection(criteria.Projection)
        End If

        Return dc
    End Function

    Protected Shared Function CreateLikeExpressionConjunction(ByVal propertyName As String, ByVal objList As Object(), ByVal matchMode As MatchMode) As Conjunction
        Dim conju As Conjunction = Expression.Conjunction()
        For Each obj As String In objList
            conju.Add(Expression.Like(propertyName, obj, matchMode))
        Next

        Return conju
    End Function

    Protected Shared Function CreateLikeExpressionDisjunction(ByVal propertyName As String, ByVal objList As Object(), ByVal matchMode As MatchMode) As Disjunction
        Dim disju As Disjunction = Expression.Disjunction()
        For Each obj As String In objList
            disju.Add(Expression.Like(propertyName, obj, matchMode))
        Next

        Return disju
    End Function

#End Region

#Region "Create Expressions Functions"

    Private Sub CreateExpression(ByRef criteria As ICriteria, ByVal filter As IFilterExpression, ByVal filterProperty As String, ByVal value As Object)
        Select Case filter.FilterExpression
            Case FilterExpression.FilterType.Contains
                criteria.Add(Expression.Like(filterProperty, filter.FilterValue, MatchMode.Anywhere))
            Case FilterExpression.FilterType.EqualTo
                criteria.Add(Restrictions.Eq(filterProperty, value))
            Case FilterExpression.FilterType.GreaterThan
                criteria.Add(Expression.Gt(filterProperty, value))
            Case FilterExpression.FilterType.GreaterThanOrEqualTo
                criteria.Add(Restrictions.Ge(filterProperty, value))
            Case FilterExpression.FilterType.LessThan
                criteria.Add(Expression.Lt(filterProperty, value))
            Case FilterExpression.FilterType.LessThanOrEqualTo
                criteria.Add(Restrictions.Le(filterProperty, value))
            Case FilterExpression.FilterType.IsNull
                criteria.Add(Restrictions.IsNull(filterProperty))
            Case FilterExpression.FilterType.IsNotNull
                criteria.Add(Expression.IsNotNull(filterProperty))
            Case FilterExpression.FilterType.StartsWith
                criteria.Add(Expression.Like(filterProperty, value, MatchMode.End))
            Case FilterExpression.FilterType.NoFilter
                If FilterExpressions.ContainsKey(filterProperty) Then
                    FilterExpressions.Remove(filterProperty)
                End If
            Case FilterExpression.FilterType.SQL
                criteria.Add(Expression.Sql(filter.SQLExpression))
            Case FilterExpression.FilterType.Criteria
                criteria.Add(filter.CriteriaImpl)
            Case FilterExpression.FilterType.IsEnum
                criteria.Add(Restrictions.Eq(filterProperty, value))
        End Select
    End Sub

    Private Sub CreateDateExpression(ByRef criteria As ICriteria, ByVal filter As IFilterExpression, ByVal filterProperty As String, ByVal value As Date)
        Dim sqlExpression As String = String.Empty
        Select Case filter.FilterExpression
            Case FilterExpression.FilterType.EqualTo
                sqlExpression = NHibernateHelper.EqualToDateIsoFormat(filterProperty, value)
            Case FilterExpression.FilterType.GreaterThan
                sqlExpression = NHibernateHelper.GreaterThanDateIsoFormat(filterProperty, value)
            Case FilterExpression.FilterType.GreaterThanOrEqualTo
                sqlExpression = NHibernateHelper.GreaterThanOrEqualToDateIsoFormat(filterProperty, value)
            Case FilterExpression.FilterType.LessThan
                sqlExpression = NHibernateHelper.LessThanDateIsoFormat(filterProperty, value)
            Case FilterExpression.FilterType.LessThanOrEqualTo
                sqlExpression = NHibernateHelper.LessThanOrEqualToDateIsoFormat(filterProperty, value)
            Case FilterExpression.FilterType.NoFilter
                FilterExpressions.Remove(filterProperty)
            Case FilterExpression.FilterType.SQL
                sqlExpression = filter.SQLExpression
        End Select

        If (Not String.IsNullOrEmpty(sqlExpression)) Then
            criteria.Add(Expression.Sql(sqlExpression))
        End If
    End Sub
#End Region

#Region "IFinder Implementation"

    Protected Sub AttachSortExpressions(ByRef criteria As ICriteria, ByVal propertyName As String, ByVal orderClause As SortOrder)
        Select Case orderClause
            Case SortOrder.Descending
                criteria.AddOrder(Order.Desc(propertyName))
            Case SortOrder.Ascending
                criteria.AddOrder(Order.Asc(propertyName))
        End Select

        If SortExpressions.ContainsKey(propertyName) Then
            SortExpressions(propertyName) = If(orderClause = SearchOrder.ASC, "ASC", "DESC")
        Else
            SortExpressions.Add(propertyName, If(orderClause = SearchOrder.ASC, "ASC", "DESC"))
        End If
    End Sub

    ''' <summary> Aggancia al criteria i criteri di ordinamento. </summary>
    ''' <param name="criteria">Criteria a cui agganciare il criterio</param>
    ''' <returns>True se esistono criteri da agganciare, False altrimenti</returns>
    Protected Overridable Function AttachSortExpressions(ByRef criteria As ICriteria) As Boolean
        Return AttachSortExpressions(criteria, SortExpressions)
    End Function

    ''' <summary> Aggancia al criteria i criteri di ordinamento. </summary>
    ''' <param name="criteria">Criteria a cui agganciare il criterio</param>
    ''' <returns>True se esistono criteri da agganciare, False altrimenti</returns>
    Protected Function AttachSortExpressions(ByRef criteria As ICriteria, defaultSortExpressione As IDictionary(Of String, String)) As Boolean
        Dim attachSort As Boolean = False
        If defaultSortExpressione.Count > 0 Then
            Try
                For Each sortExpr As String In defaultSortExpressione.Keys
                    If defaultSortExpressione(sortExpr).Eq(SORT_NONE) Then
                        Continue For
                    End If

                    criteria.AddOrder(If(defaultSortExpressione(sortExpr) = "ASC", Order.Asc(sortExpr), Order.Desc(sortExpr)))
                    attachSort = True
                Next
            Catch ex As Exception
                Throw New DocSuiteException("Errore ordinamento", "Espressione di ordinamento non corretta", ex)
            End Try
        End If
        Return attachSort
    End Function

    ''' <summary> Aggancia al criteria i criteri di filtraggio. </summary>
    ''' <param name="criteria">Criteria a cui agganciare il criterio</param>
    ''' <returns>True se esistono criteri da agganciare, False altrimenti</returns>
    Protected Function AttachFilterExpressions(ByRef criteria As ICriteria) As Boolean
        If FilterExpressions.Count <= 0 Then
            Return False
        End If

        Dim tempDictionary As New Dictionary(Of String, IFilterExpression)(FilterExpressions)
        For Each filterProperty As String In tempDictionary.Keys
            Dim filter As IFilterExpression = FilterExpressions(filterProperty)
            Try
                Dim value As Object
                If TypeOf filter.FilterValue Is DateTime Then
                    value = filter.FilterValue
                Else
                    Dim converter As TypeConverter = TypeDescriptor.GetConverter(filter.PropertyType)
                    value = converter.ConvertFrom(filter.FilterValue)
                End If

                If TypeOf (value) Is Date Then
                    CreateDateExpression(criteria, filter, filterProperty, CType(value, Date))
                Else
                    CreateExpression(criteria, filter, filterProperty, value)
                End If

            Catch ex As Exception
                Throw New DocSuiteException("Errore filtro", String.Format("Formato del filtro [{0}] non corretto", filter.PropertyName), ex)
            End Try
        Next
        Return True
    End Function

    ''' <summary> Aggancia al criteria delle clausole WHERE SQL </summary>
    ''' <param name="criteria">criterio a cui agganciare le espressioni</param>
    ''' <returns>True se esistono espressioni da agganciare, False altrimenti</returns>
    Protected Function AttachSQLExpressions(ByRef criteria As ICriteria) As Boolean
        If SQLExpressions.Count <= 0 Then
            Return False
        End If

        Try
            For Each key As String In SQLExpressions.Keys
                Dim expr As ISQLExpression = SQLExpressions(key)
                If (expr.Alias IsNot Nothing) AndAlso (Not NHibernateHelper.ExistAlias(criteria, expr.Alias.Alias)) Then
                    Dim aliasCriteria As ICriteria = criteria.CreateCriteria(expr.Alias.Path, expr.Alias.Alias, expr.Alias.JoinType)
                    If expr.Alias.JoinAlias.Count > 0 Then
                        For Each jalias As IAliasFinder In expr.Alias.JoinAlias
                            AttachRecursiveJoinAliases(aliasCriteria, jalias)
                        Next
                    End If
                End If
                If Not String.IsNullOrEmpty(expr.SQLExpr) Then
                    criteria.Add(Expression.Sql(expr.SQLExpr))
                End If
            Next
        Catch ex As Exception
            Throw New DocSuiteException("Errore SQL", "Formato espressione SQL non corretta", ex)
        End Try
        Return True
    End Function

    Private Sub AttachRecursiveJoinAliases(ByRef criteria As ICriteria, ByRef jAlias As IAliasFinder)
        Dim aliasCriteria As ICriteria = criteria.CreateCriteria(jAlias.Path, jAlias.Alias, jAlias.JoinType)
        If jAlias.JoinAlias.Count <= 0 Then
            Exit Sub
        End If

        For Each [alias] As IAliasFinder In jAlias.JoinAlias
            AttachRecursiveJoinAliases(aliasCriteria, [alias])
        Next
    End Sub

    ''' <summary>
    ''' Crea le proiezioni (Proprietà Sorgente - Proprietà Destinazione) da attuare sul criterio.
    ''' </summary>
    ''' <param name="criteria">criterio su cui effettuare le proiezioni</param>
    ''' <returns>True se esistono proiezioni, False altrimenti</returns>
    Protected Overridable Function CreateProjections(ByRef criteria As ICriteria) As Boolean
        If SelectProjections.Count <= 0 Then
            Return False
        End If

        Dim projList As ProjectionList = Projections.ProjectionList()
        Try
            For Each [property] As String In SelectProjections.Keys
                projList.Add(Projections.Property([property]), SelectProjections([property]))
            Next
            criteria.SetProjection(projList)
            criteria.SetResultTransformer(New TupleToPropertyResultTransformer(GetType(T), SelectProjections, True))
        Catch ex As Exception
            Throw New DocSuiteException("Errore proiezioni", "Errore nell'impostazione proiezioni", ex)
        End Try
        Return True
    End Function

    ''' <summary> Ricerca con filtro. </summary>
    ''' <param name="filterProperty">Nome della proprietà su cui filtrare</param>
    ''' <param name="filterValue">Valore del filtro</param>
    ''' <param name="filterExpression"></param>
    Public Overridable Sub DoSearch(ByVal filterProperty As String, ByVal filterValue As String, ByVal filterType As System.Type, ByVal filterExpression As FilterExpression.FilterType)
        If String.IsNullOrEmpty(filterProperty) Or String.IsNullOrEmpty(filterValue) Then
            Exit Sub
        End If

        FilterExpressions.Clear()
        FilterExpressions.Add(filterProperty, New FilterExpression(filterProperty, filterType, filterValue, filterExpression))
    End Sub

    ''' <summary> Ricerca con ordinamento </summary>
    ''' <param name="sortExpr">una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <returns>Una lista ordinata di risultati </returns>
    Public Overridable Function DoSearch(ByVal sortExpr As String) As IList(Of T) Implements IFinder(Of T).DoSearch
        If Not String.IsNullOrEmpty(sortExpr) Then
            Dim aExpr() As String = sortExpr.Split(" "c)
            SortExpressions.Clear()
            If aExpr.Length > 1 Then
                SortExpressions.Add(aExpr(0), If(aExpr(1).Eq("ASC"), "Ascending", "Descending"))
            Else
                SortExpressions.Add(aExpr(0), "Ascending")
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>  Ricerca con ordinamento, riga di partenza e dimensione della paginazione. </summary>
    ''' <param name="sortExpr">Una stringa contenente la sortExpression (NomeCampo DirezioneOrdinamento)</param>
    ''' <param name="StartRow">La riga a partire dalla quale tornare il risultato</param>
    ''' <param name="PageSize">La dimensione della pagina dei risultati</param>
    ''' <returns>Una lista ordinata di risultati limitati a PageSize</returns>
    Public Overridable Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pageSize As Integer) As IList(Of T) Implements IFinder(Of T).DoSearch
        DoSearch(sortExpr)
        PageIndex = startRow
        Me.PageSize = pageSize
        Return Nothing
    End Function

    ''' <summary> Numero dei risultati ottenuti dalla ricerca. </summary>
    ''' <returns>Il numero dei record ottenuti dalla ricerca</returns>
    Public Overridable Function Count() As Integer Implements IFinder(Of T).Count
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

#End Region

    Public Function DoSearch(sortExpr As ICollection(Of System.Linq.Expressions.Expression(Of Func(Of T, Object)))) As ICollection(Of T) Implements IFinder(Of T).DoSearch
        Return Nothing
    End Function

    Public Function DoSearch(sortExpr As ICollection(Of System.Linq.Expressions.Expression(Of Func(Of T, Object))), startRow As Integer, pageSize As Integer) As ICollection(Of T) Implements IFinder(Of T).DoSearch
        Return Nothing
    End Function
End Class
