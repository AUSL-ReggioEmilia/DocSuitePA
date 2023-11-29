Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports System.Text
Imports NHibernate.SqlCommand

Public Class NHibernateCategoryDao
    Inherits BaseNHibernateDao(Of Category)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Save(ByRef entity As Category)
        MyBase.Save(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(Category))
    End Sub

    Public Overrides Sub Update(ByRef entity As Category)
        MyBase.Update(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(Category))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As Category)
        MyBase.UpdateNoLastChange(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(Category))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As Category)
        MyBase.UpdateOnly(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(Category))
    End Sub

    Public Overrides Sub Delete(ByRef entity As Category)
        MyBase.Delete(entity)
        NHibernateSession.SessionFactory.Evict(GetType(Category))
    End Sub

    Public Overrides Function GetAll() As IList(Of Category)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        Return criteria.List(Of Category)()
    End Function

    Public Overrides Function GetAll(ByVal orderedExpression As String) As IList(Of Category)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        Dim expressions As String() = orderedExpression.Split(" "c)
        Select Case expressions(1).ToUpper()
            Case "DESC"
                criteria.AddOrder(Order.Desc(expressions(0)))
            Case Else
                criteria.AddOrder(Order.Asc(expressions(0)))
        End Select

        Return criteria.List(Of Category)()
    End Function

    Public Function CategoryUsedProtocol(ByVal Category As Category) As Boolean
        Dim _protDao As New NHibernateProtocolDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Return _protDao.GetCountByCategory(Category) > 0
    End Function

    Public Function CategoryUsedDocument(ByVal Category As Category) As Boolean
        Dim _docmDao As New NHibernateDocumentDao("DocmDB")
        Return _docmDao.GetCountByCategory(Category) > 0
    End Function

    Public Function CategoryUsedResolution(ByVal Category As Category) As Boolean
        Dim _reslDao As New NHibernateResolutionDao("ReslDB")
        Return _reslDao.GetCountByCategory(Category) > 0
    End Function

    Public Function CategoryUsedDocumentSeriesItem(ByVal Category As Category) As Boolean
        Dim _docSeriesDao As New NHibernateDocumentSeriesItemDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Return _docSeriesDao.GetCountByCategory(Category) > 0
    End Function

    Public Function CategoryUsedFascicle(ByVal Category As Category) As Boolean
        Dim fascicleDao As New NHibernateFascicleDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Return fascicleDao.CountFascicleByCategory(Category) > 0
    End Function

    Public Function SubCategoryUsedDocumentSeriesItem(ByVal Category As Category) As Boolean
        Dim _docSeriesItemDao As New NHibernateDocumentSeriesItemDao(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
        Return _docSeriesItemDao.CountBySubCategory(Category) > 0
    End Function

    Public Function SubCategoryUsedDocument(ByVal Category As Category) As Boolean
        Dim _docmDao As New NHibernateDocumentDao("DocmDB")
        Return _docmDao.GetCountBySubCategory(Category) > 0
    End Function

    Public Function SubCategoryUsedResolution(ByVal Category As Category) As Boolean
        Dim _reslDao As New NHibernateResolutionDao("ReslDB")
        Return _reslDao.GetCountBySubCategory(Category) > 0
    End Function

    Public Function GetRootAOOCategory(idtenantAOO As Guid) As Category
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.IsNull("Parent"))
        criteria.Add(Restrictions.Eq("Code", 0))
        criteria.AddOrder(Order.Asc("Code"))
        criteria.Add(Restrictions.Eq("IsActive", True))
        criteria.Add(Restrictions.Eq("IdTenantAOO", idtenantAOO))

        Return criteria.UniqueResult(Of Category)()
    End Function

    Public Function GetRootCategory(Optional ByVal IsActive As Boolean = False) As IList(Of Category)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.IsNull("Parent"))
        criteria.AddOrder(Order.Asc("Code"))
        If IsActive Then
            criteria.Add(Restrictions.Eq("IsActive", True))
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of Category)()
    End Function

    Public Function GetCategoryByParentId(ByVal ParentId As Integer, Optional ByVal IsActive As Boolean = False) As IList(Of Category)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Parent", "Parent", SqlCommand.JoinType.LeftOuterJoin)
        criteria.Add(Restrictions.Eq("Parent.Id", ParentId))
        If IsActive Then
            criteria.Add(Restrictions.Eq("IsActive", True))
        End If
        criteria.AddOrder(Order.Asc("Code"))

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of Category)()
    End Function

    Public Function GetCategoryByDescription(ByVal description As String, ByVal isActive As Boolean, Optional ByVal idCategory As Integer = 0) As IList(Of Category)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        'descrizione da cercare
        Dim words As String() = description.Split(" "c)
        Dim disju As Disjunction = Expression.Disjunction()
        For Each word As String In words
            disju.Add(Expression.Like("Name", word, MatchMode.Anywhere))
        Next
        criteria.Add(disju)
        criteria.Add(Restrictions.Eq("IsActive", isActive))

        'Contenitore di default
        If idCategory <> 0 Then
            criteria.Add(Expression.Like("FullIncrementalPath", idCategory.ToString(), MatchMode.Start))
        End If

        criteria.AddOrder(Order.Asc("FullCode"))

        Return criteria.List(Of Category)()
    End Function

    Function GetCategoryByFullCode(ByVal FullCode As String, ByVal IsActive As Boolean?) As IList(Of Category)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        'FullCode
        criteria.Add(Restrictions.Eq("FullCode", FullCode))

        'IsActive
        If IsActive IsNot Nothing Then
            criteria.Add(Restrictions.Eq("IsActive", IsActive))
        End If

        Return criteria.List(Of Category)()
    End Function

    Function GetCategoryByFullIncrementalPath(ByVal fullIncrementalPath As String, ByVal isActive As Boolean) As Category
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        'FullIncrementalpath
        criteria.Add(Restrictions.Eq("FullIncrementalPath", fullIncrementalPath))
        criteria.Add(Restrictions.Eq("IsActive", isActive))

        Return criteria.UniqueResult(Of Category)()
    End Function

    Function GetStatsCategory(ByVal dateFrom As Nullable(Of Date), ByVal dateTo As Nullable(Of Date), ByVal registrationUser As String, ByVal idType As String, ByVal idContainer As String, ByVal hideEmptyCategories As Boolean, ByVal sortColumn As String, ByVal sortOrder As String) As IList
        Dim sort As String = String.Empty
        Dim where As String = String.Empty
        Dim condition As String = String.Empty
        ' Filtreo Data inizio protocollo
        If dateFrom.HasValue Then
            condition = "P.RegistrationDate >= '" & String.Format("{0:yyyyMMdd}", dateFrom) & "'"
            If where <> "" Then
                where = where & " AND " & condition
            Else
                where = condition
            End If
        End If
        ' Filtro data fine
        If dateTo.HasValue Then
            condition = "P.RegistrationDate <= '" & String.Format("{0:yyyyMMdd}", dateTo) & "'"
            If where <> "" Then
                where = where & " AND " & condition
            Else
                where = condition
            End If
        End If
        ' Filtro Utente
        If Not String.IsNullOrEmpty(registrationUser) Then
            condition = "P.RegistrationUser LIKE '" & registrationUser & "'"
            If where <> "" Then
                where = where & " AND " & condition
            Else
                where = condition
            End If
        End If
        ''Filtro Tipologia protcollo (ingresso\uscita\ingresso e uscita)
        If Not String.IsNullOrEmpty(idType) AndAlso Integer.Parse(idType) > Integer.MaxValue Then
            condition = "P.idType = " & idType
            If where <> "" Then
                where = where & " AND " & condition
            Else
                where = condition
            End If
        End If
        ''Filtro contenitore
        If Not (String.IsNullOrEmpty(idContainer)) Then
            condition = "P.idContainer IN (" & idContainer & ")"
            If where <> "" Then
                where = where & " AND " & condition
            Else
                where = condition
            End If
        End If
        ' Sort 
        If Not String.IsNullOrEmpty(sortColumn) Then
            If sortColumn.Equals("Protocols") Then
                sort = sortColumn
            Else
                sort = "C." & sortColumn
            End If
        Else
            sort = "C.FullCode"
        End If
        If Not String.IsNullOrEmpty(sortOrder) Then
            sort = sort & " " & sortOrder
        Else
            sort = sort & " ASC"
        End If
        Dim sql As String = String.Format(
            "select *,ISNULL (P1.Tot,0) as Protocols FROM Category AS C LEFT JOIN (SELECT P.IdCategoryAPI, count(*) as Tot FROM Protocol AS P WHERE {0} GROUP BY IdCategoryAPI) as P1 ON C.idCategory =  P1.IdCategoryAPI WHERE (C.isActive = 1) {1} ORDER BY {2}",
            where, If(hideEmptyCategories, " AND P1.Tot>0 ", ""), sort)
        Dim query As IQuery = NHibernateSession.CreateSQLQuery(sql) _
        .AddEntity("Category", GetType(Category)) _
        .AddScalar("Protocols", NHibernateUtil.Int32)
        Return query.List()
    End Function

    ''' <summary>
    ''' Costruisce il FullIncrementalPath di un classificatore
    ''' </summary>
    ''' <param name="category">Category</param>
    ''' <param name="sFull">variabile che conterrà il FullIncrementalPath</param>
    ''' <remarks></remarks>
    Public Sub GetFullIncrementalPath(ByRef category As Category, ByRef sFull As String)
        Dim father As Category = Nothing

        If category Is Nothing Then
            sFull = "" & sFull
        Else
            If sFull <> "" Then
                sFull = "|" & sFull
            End If
            sFull = category.Id.ToString() & sFull

            father = category.Parent
            If father IsNot Nothing Then
                GetFullIncrementalPath(father, sFull)
            End If
        End If
    End Sub

    ''' <summary> Costruisce il FullCode di un classificatore. </summary>
    Public Sub GetFullCode(ByRef category As Category, ByRef fullCode As String)
        If category Is Nothing Then
            fullCode = "" & fullCode
        Else
            If Not String.IsNullOrEmpty(fullCode) Then
                fullCode = "|" & fullCode
            End If
            fullCode = Format(category.Code, "0000") & fullCode

            Dim father As Category = category.Parent
            If father IsNot Nothing Then
                GetFullCode(father, fullCode)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Esegue il filtro della Grid per la colonna Category che è composta da FullCode e Name
    ''' </summary>
    ''' <param name="sqlParam">sqlParam</param>
    ''' <remarks></remarks>
    Public Function CategoryNameFullCodeFilter(ByVal sqlParam As String) As Object

        Dim conjunction As New Conjunction

        'split di ogni parola
        Dim params() As String = Split(sqlParam, " ")

        For Each par As String In params

            Dim disjuCheckName As New Disjunction

            Dim formatCode As String = FormatCategoryFullCode(par)

            Dim detachName As DetachedCriteria = DetachedCriteria.For(GetType(Category))
            detachName.Add(Restrictions.Like("FullSearchComputed", par, MatchMode.Anywhere))
            detachName.SetProjection(Projections.Id)

            Dim conjuName As New Conjunction
            conjuName.Add(Restrictions.Like("Category.FullSearchComputed", par, MatchMode.Anywhere))
            conjuName.Add(Restrictions.IsNull("P.Category.Id"))

            Dim disjuSubCategoryName As New Disjunction
            disjuSubCategoryName.Add(Subqueries.PropertyIn("P.Category.Id", detachName))

            disjuCheckName.Add(Restrictions.Or(conjuName, disjuSubCategoryName))

            If String.IsNullOrEmpty(formatCode) Then
                conjunction.Add(disjuCheckName)
                Continue For
            End If

            Dim detachCode As DetachedCriteria = DetachedCriteria.For(GetType(Category))
            detachCode.Add(Expression.Like("FullSearchComputed", formatCode, MatchMode.Anywhere))
            detachCode.SetProjection(Projections.Id)

            Dim disjuCode As New Disjunction
            disjuCode.Add(Restrictions.Like("Category.FullSearchComputed", formatCode, MatchMode.Anywhere))
            disjuCode.Add(Subqueries.PropertyIn("P.Category.Id", detachCode))

            conjunction.Add(Restrictions.Or(disjuCheckName, disjuCode))
        Next

        Return conjunction
    End Function



    ''' <summary>
    ''' Filtra in base alla categoria inserita
    ''' </summary>
    Public Function CategoryFullCodeFilter(ByVal sqlParam As String) As Object
        Dim formatCode As String = FormatCategoryFullCode(sqlParam)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(Category))
        detach.Add(Restrictions.Like("FullCode", formatCode, MatchMode.Anywhere))
        detach.SetProjection(Projections.Id)

        Dim disju As New Disjunction
        disju.Add(Restrictions.Like("Category.FullCode", formatCode, MatchMode.Anywhere))
        disju.Add(Subqueries.PropertyIn("P.Category.Id", detach))
        Return disju
    End Function

    ''' <summary> Formatta il fullcode di un classificatore come gestito su DB(Es: 1.1.1 => 0001|0001|0001). </summary>
    ''' <remarks> Pulisce la stringa da valori non validi. </remarks>
    Public Function FormatCategoryFullCode(ByVal fullCode As String) As String
        Dim sFull As New StringBuilder()
        Dim v As String() = fullCode.Split("."c)
        For i As Short = 0 To v.Length - 1
            Dim code As Short
            If Not Short.TryParse(v(i), code) Then
                Continue For
            End If
            sFull.AppendFormat("{0:0000}|", code)
        Next i
        If sFull.Length > 0 Then
            sFull.Remove(sFull.Length - 1, 1)
        End If
        Return sFull.ToString()
    End Function

    Public Function GetWithChildren() As IList(Of Integer)
        Dim detach As DetachedCriteria = DetachedCriteria.For(GetType(Category), "CC")
        detach.Add(Restrictions.IsNotNull("CC.Parent"))
        detach.Add(Restrictions.EqProperty("CC.Parent.Id", "C.Id"))
        detach.SetProjection(Projections.Id())

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "C")
        criteria.Add(Subqueries.Exists(detach))
        criteria.SetProjection(Projections.Id())
        Return criteria.List(Of Integer)()
    End Function

    Function ExistCategoryFullCode(fullCode As String, category As Category) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("FullCode", fullCode))
        criteria.Add(Restrictions.Not(Restrictions.Eq("Id", category.Id)))
        criteria.Add(Restrictions.Eq("IsActive", True))
        criteria.SetProjection(Projections.RowCountInt64())
        Return criteria.UniqueResult(Of Long)() > 0
    End Function

End Class
