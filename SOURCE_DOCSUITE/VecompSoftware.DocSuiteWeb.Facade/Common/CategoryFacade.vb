Imports System.Linq
Imports NHibernate
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.NHibernateManager

<ComponentModel.DataObject()>
Public Class CategoryFacade
    Inherits CommonFacade(Of Category, Integer, NHibernateCategoryDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public Overrides Sub Save(ByRef obj As Category)
        Dim pf As New ParameterFacade(_dbName)
        Dim parameter As Parameter = pf.GetAll()(0)
        obj.Id = parameter.LastUsedIdCategory + 1

        If (pf.UpdateReplicateLastIdCategory(parameter.LastUsedIdCategory + 1, parameter.LastUsedIdCategory)) Then
            CalculateFullIncremental(obj)
            'FullCode
            CalculateFullCode(obj)

            MyBase.Save(obj)
        End If
    End Sub

    Private Sub CalculateFullIncremental(ByRef obj As Category)
        If obj.Parent Is Nothing Then
            obj.FullIncrementalPath = obj.Id.ToString()
        Else
            obj.FullIncrementalPath = String.Format("{0}|{1}", obj.Parent.FullIncrementalPath, obj.Id.ToString())
        End If
    End Sub

    Private Sub CalculateFullCode(ByRef obj As Category)
        If obj.Parent Is Nothing OrElse (obj.Parent IsNot Nothing AndAlso String.IsNullOrEmpty(obj.Parent.FullCode)) Then
            obj.FullCode = Format(obj.Code, "0000")
        Else
            obj.FullCode = String.Concat(obj.Parent.FullCode, "|", Format(obj.Code, "0000"))
        End If
    End Sub

    Public Function GetRootAOOCategory(idtenantAOO As Guid) As Category
        Return _dao.GetRootAOOCategory(idtenantAOO)
    End Function

    Public Function GetRootCategory(Optional ByVal isActive As Boolean = False) As IList(Of Category)
        Return _dao.GetRootCategory(isActive)
    End Function

    Public Function GetCategoryByParentId(ByVal ParentId As Integer, Optional ByVal IsActive As Boolean = False) As IList(Of Category)
        Return _dao.GetCategoryByParentId(ParentId, IsActive)
    End Function

    Public Function GetCategoryByDescription(ByVal description As String, ByVal isActive As Short, Optional ByVal idCategory As Integer = 0) As IList(Of Category)
        Return _dao.GetCategoryByDescription(description, isActive, idCategory)
    End Function

    Public Function GetCategoryByFullCode(ByVal fullCode As String, ByVal isActive As Short) As IList(Of Category)
        Return _dao.GetCategoryByFullCode(fullCode, isActive)
    End Function

    Public Function GetCategoryByFullIncrementalPath(ByVal fullIncrementalPath As String, isActive As Short) As Category
        Return _dao.GetCategoryByFullIncrementalPath(fullIncrementalPath, isActive)
    End Function

    Public Function GetStatsCategory(ByVal dateFrom As Nullable(Of Date), ByVal dateTo As Nullable(Of Date), ByVal registrationUser As String, ByVal idType As String, ByVal idContainer As String, ByVal hideEmptyCategories As Boolean, ByVal sortColumn As String, ByVal sortOrder As String) As IList
        Return _dao.GetStatsCategory(dateFrom, dateTo, registrationUser, idType, idContainer, hideEmptyCategories, sortColumn, sortOrder)
    End Function

    Public Overrides Function IsUsed(ByRef obj As Category) As Boolean
        If DocSuiteContext.Current.IsProtocolEnabled Then
            If _dao.CategoryUsedProtocol(obj) Then
                Return True
            End If
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            If obj.Parent Is Nothing Then
                If _dao.CategoryUsedDocument(obj) Then
                    Return True
                End If
            Else
                If _dao.SubCategoryUsedDocument(obj) Then
                    Return True
                End If
            End If
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            If obj.Parent Is Nothing Then
                If _dao.CategoryUsedResolution(obj) Then
                    Return True
                End If
            Else
                If _dao.SubCategoryUsedResolution(obj) Then
                    Return True
                End If
            End If
        End If
        If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
            If obj.Parent Is Nothing Then
                If _dao.CategoryUsedDocumentSeriesItem(obj) Then
                    Return True
                End If
            Else
                If _dao.SubCategoryUsedDocumentSeriesItem(obj) Then
                    Return True
                End If
            End If
        End If
        If DocSuiteContext.Current.ProtocolEnv.FascicleEnabled Then
            If _dao.CategoryUsedFascicle(obj) Then
                Return True
            End If
        End If

        Return False
    End Function

    ''' <summary> Verifica l'esatezza e correge i FullIncrementalPath di tutti i classificatori. </summary>
    ''' <param name="errDescription">Descrizione eventuale errore</param>
    ''' <param name="report">Rapporto dell'esecuzione</param>
    Public Function FullIncrementalUtility(ByRef errDescription As String, ByRef report As String) As Boolean
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try
            Dim categories As IList(Of Category) = Me.GetAll()
            For Each category As Category In categories
                Dim sFullPath As String = ""
                _dao.GetFullIncrementalPath(category.Parent, sFullPath)
                If Not String.IsNullOrEmpty(sFullPath) Then
                    sFullPath = sFullPath & "|"
                End If
                sFullPath = sFullPath & category.Id.ToString()
                If Not category.FullIncrementalPath.Eq(sFullPath) Then
                    category.FullIncrementalPath = sFullPath
                    Update(category)
                    If Not String.IsNullOrEmpty(report) Then
                        report &= WebHelper.Br
                    End If
                    report &= String.Format("{0} ({1})", category.GetFullName(), category.Id)
                End If
            Next
            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            errDescription = ex.Message
            Return False
        End Try

        Return True
    End Function

    ''' <summary> Verifica l'esatezza e correge i FullCode di tutti i classificatori. </summary>
    ''' <param name="errDescription">Descrizione eventuale errore</param>
    ''' <param name="report">Rapporto dell'esecuzione</param>
    Function FullCodeUtility(ByRef errDescription As String, ByRef report As String) As Boolean
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try
            Dim categories As IList(Of Category) = Me.GetAll()
            For Each category As Category In categories
                Dim sFullCode As String = String.Empty
                _dao.GetFullCode(category.Parent, sFullCode)
                If Not String.IsNullOrEmpty(sFullCode) Then
                    sFullCode = sFullCode & "|"
                End If
                sFullCode &= Format(category.Code, "0000")
                If Not category.FullCode.Eq(sFullCode) Then
                    category.FullCode = sFullCode
                    Me.Update(category)
                    If Not String.IsNullOrEmpty(report) Then
                        report &= WebHelper.Br
                    End If
                    report &= String.Format("{0} ({1})", category.GetFullName(), category.Id)
                End If
            Next
            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            errDescription = ex.Message
            Return False
        End Try

        Return True
    End Function
    Public Function CategoryNameFullCodeFilter(ByVal sqlParam As String) As Object
        Return _dao.CategoryNameFullCodeFilter(sqlParam)
    End Function

    ''' <summary> Filtra in base alla categoria inserita. </summary>
    Public Function CategoryFullCodeFilter(ByVal sqlParam As String) As Object
        Return _dao.CategoryFullCodeFilter(sqlParam)
    End Function

    ''' <summary> Formatta il fullcode di un classificatore come gestito su DB(Es: 1.1.1 => 0001|0001|0001). </summary>
    ''' <remarks> Pulisce la stringa da valori non validi. </remarks>
    Public Function FormatCategoryFullCode(ByVal fullCode As String) As String
        Return _dao.FormatCategoryFullCode(fullCode)
    End Function

    Public Function GetFullIncrementalName(category As Category) As String
        If category.Parent Is Nothing OrElse category.Parent.Code.Equals(0) Then
            Return category.GetFullName()
        End If
        Return $"{GetFullIncrementalName(category.Parent)}, {category.GetFullName()}"
    End Function

    Public Function GetCodeDotted(category As Category) As String
        Return GetCodeDotted(category.FullCode)
    End Function

    Public Function GetCodeDotted(fullCode As String) As String
        Dim listOfCode As IEnumerable(Of String) = fullCode.Split("|"c).Select(Function(s) s.TrimStart("0"c))
        Return String.Join("."c, listOfCode)
    End Function

    Public Function GetCategoryString(fullCode As String, name As String) As String
        Dim newFullCode As String = GetFormattedFullCode(fullCode)
        Select Case DocSuiteContext.Current.ProtocolEnv.CategoryView
            Case 1
                Return String.Format("{0} {1}", newFullCode, name)
            Case 2
                Return newFullCode
            Case Else
                Return name
        End Select
    End Function

    Private Function GetFormattedFullCode(fullCode As String) As String
        Select Case DocSuiteContext.Current.ProtocolEnv.CategoryFullCodeFormatType
            Case 1
                'full code default
                Return fullCode
            Case 2
                'full code modificato con rimozione zeri e punto di divisione
                Return GetCodeDotted(fullCode)
            Case Else
                'ritorno sempre il default
                Return fullCode
        End Select
    End Function

    Public Function IsCategoryActive(category As Category) As Boolean
        If (category Is Nothing) Then
            Return False
        End If
        Return category.StartDate.Date <= DateTime.Today AndAlso (category.EndDate Is Nothing OrElse category.EndDate.Value.Date > DateTime.Today)
    End Function

    Public Function GetWithChildren() As IList(Of Integer)
        Return _dao.GetWithChildren()
    End Function

    Public Function IsCategoryCodeUnique(category As Category) As Boolean
        Dim fullCode As String = String.Empty
        _dao.GetFullCode(category, fullCode)
        Return Not _dao.ExistCategoryFullCode(fullCode, category)
    End Function

End Class
