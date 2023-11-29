Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel


<Serializable(), DataObject()> _
Public Class NHibernateDocumentFinder
    Inherits NHibernateBaseFinder(Of Document, DocumentHeader)

    Public Enum SearchStatus
        All = 0
        Open = 1
        Closed = 2
        Canceled = 3
        Archived = 4
    End Enum

#Region " Constructors "

    Public Sub New(ByVal dbName As String)
        SessionFactoryName = dbName
        NHibernateSession.EnableFilter("DocmLogUser").SetParameter("User", DocSuiteContext.Current.User.FullUserName)
    End Sub

#End Region

#Region " Fields "

    Private _anno As Short
    Private _numero As Integer
    Private _datainiziofrom As DateTime?
    Private _datainizioto As DateTime?
    Private _enddatefrom As DateTime?
    Private _enddateto As DateTime?
    Private _datascadenzafrom As DateTime?
    Private _datascadenzato As DateTime?
    Private _numeroservizio As String
    Private _nome As String
    Private _Oggetto As String
    Private _documentObjectSearch As ObjectSearchType
    Private _manager As String
    Private _note As String
    Private _idRole As String
    Private _idcontainer As String
    Private _idcategory As String
    Private _includeChildCategories As Boolean
    Private _idcontact As String
    Private _includeChildContacts As Boolean
    Private _status As SearchStatus


    Private _documentDescription As String
    Private _documentDate_From As DateTime?
    Private _documentDate_To As DateTime?
    Private _documentObject As String
    Private _documentReason As String
    Private _documentNote As String
    Private _documentRegDate_From As DateTime?
    Private _documentRegDate_To As DateTime?

    Private _presaInCarico As Boolean = False
    Private _idRoleDestination As IList(Of String) = New List(Of String)()

    Private _securityEnabled As Boolean
    Private _securityContainers As String
    Private _securityRoles As String

    Private _documentLogType As String
    Private _documentNotReaded As Boolean

    Private _documentFolderExpired As Boolean
    Private _loadDocumentFolderInfo As Boolean
    Private _documentFolderExpiredDate As Nullable(Of Date)

    'Configuration
    Protected _webService As Boolean
    Protected _addDcDO As Boolean = False

    'Paginazione
    Protected _enablePaging As Boolean = True

#End Region

#Region "Properties"

    Public Property Anno() As Short
        Get
            Return _anno

        End Get
        Set(ByVal value As Short)
            _anno = value
        End Set
    End Property

    Public Property Numero() As Integer
        Get
            Return _numero
        End Get
        Set(ByVal value As Integer)
            _numero = value
        End Set
    End Property


    Public Property DataInizioFrom() As DateTime?
        Get
            Return _datainiziofrom

        End Get
        Set(ByVal value As DateTime?)
            _datainiziofrom = value
        End Set
    End Property

    Public Property DataInizioTo() As DateTime?
        Get
            Return _datainizioto
        End Get
        Set(ByVal value As DateTime?)
            _datainizioto = value
        End Set
    End Property

    Public Property EndDateFrom() As DateTime?
        Get
            Return _enddatefrom
        End Get
        Set(ByVal value As DateTime?)
            _enddatefrom = value
        End Set
    End Property

    Public Property EndDateTo() As DateTime?
        Get
            Return _enddateto
        End Get
        Set(ByVal value As DateTime?)
            _enddateto = value
        End Set
    End Property

    Public Property DataScadenzaFrom() As DateTime?
        Get
            Return _datascadenzafrom
        End Get
        Set(ByVal value As DateTime?)
            _datascadenzafrom = value
        End Set
    End Property

    Public Property DataScadenzaTo() As DateTime?
        Get
            Return _datascadenzato
        End Get
        Set(ByVal value As DateTime?)
            _datascadenzato = value
        End Set
    End Property

    Public Property NumeroServizio() As String
        Get
            Return _numeroservizio
        End Get
        Set(ByVal value As String)
            _numeroservizio = value
        End Set
    End Property

    Public Property Nome() As String
        Get
            Return _nome
        End Get
        Set(ByVal value As String)
            _nome = value
        End Set
    End Property

    Public Property IDCategory() As String
        Get
            Return _idcategory
        End Get
        Set(ByVal value As String)
            _idcategory = value
        End Set
    End Property

    Property IncludeChildCategories() As Boolean
        Get
            Return _includeChildCategories
        End Get
        Set(ByVal value As Boolean)
            _includeChildCategories = value
        End Set
    End Property

    Public Property IDContact() As String
        Get
            Return _idcontact
        End Get
        Set(ByVal value As String)
            _idcontact = value
        End Set
    End Property

    Public Property DocumentContactIds() As IEnumerable(Of Integer)

    Property IncludeChildContacts() As Boolean
        Get
            Return _includeChildContacts
        End Get
        Set(ByVal value As Boolean)
            _includeChildContacts = value
        End Set
    End Property

    Public Property IDRole() As String
        Get
            Return _idRole
        End Get
        Set(ByVal value As String)
            _idRole = value
        End Set
    End Property

    Public Property IDContainer() As String
        Get
            Return _idcontainer
        End Get
        Set(ByVal value As String)
            _idcontainer = value
        End Set
    End Property

    Public Property IdRoleDestination() As IList(Of String)
        Get
            Return _idRoleDestination
        End Get
        Set(ByVal value As IList(Of String))
            _idRoleDestination = value
        End Set
    End Property

    Public Property Manager() As String
        Get
            Return _manager
        End Get
        Set(ByVal value As String)
            _manager = value
        End Set
    End Property

    Public Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property


    Public Property Oggetto() As String
        Get
            Return _Oggetto
        End Get
        Set(ByVal value As String)
            _Oggetto = value
        End Set
    End Property

    Property DocumentObjectSearch() As ObjectSearchType
        Get
            Return _documentObjectSearch
        End Get
        Set(ByVal value As ObjectSearchType)
            _documentObjectSearch = value
        End Set
    End Property


    Public Property DocumentDescription() As String
        Get
            Return _documentDescription
        End Get
        Set(ByVal value As String)
            _documentDescription = value
        End Set
    End Property

    Public Property DocumentDate_From() As DateTime?
        Get
            Return _documentDate_From

        End Get
        Set(ByVal value As DateTime?)
            _documentDate_From = value
        End Set
    End Property

    Public Property DocumentDate_To() As DateTime?
        Get
            Return _documentDate_To

        End Get
        Set(ByVal value As DateTime?)
            _documentDate_To = value
        End Set
    End Property

    Public Property DocumentObject() As String
        Get
            Return _documentObject
        End Get
        Set(ByVal value As String)
            _documentObject = value
        End Set
    End Property

    Public Property DocumentReason() As String
        Get
            Return _documentReason
        End Get
        Set(ByVal value As String)
            _documentReason = value
        End Set
    End Property

    Public Property DocumentNote() As String
        Get
            Return _documentNote
        End Get
        Set(ByVal value As String)
            _documentNote = value
        End Set
    End Property

    Public Property DocumentRegDate_From() As DateTime?
        Get
            Return _documentRegDate_From

        End Get
        Set(ByVal value As DateTime?)
            _documentRegDate_From = value
        End Set
    End Property

    Public Property DocumentRegDate_To() As DateTime?
        Get
            Return _documentRegDate_To

        End Get
        Set(ByVal value As DateTime?)
            _documentRegDate_To = value
        End Set
    End Property

    Public Property PresaInCarico() As Boolean
        Get
            Return _presaInCarico
        End Get
        Set(ByVal value As Boolean)
            _presaInCarico = value
        End Set
    End Property

    Public Property SecurityEnabled() As Boolean
        Get
            Return _securityEnabled
        End Get
        Set(ByVal value As Boolean)
            _securityEnabled = value
        End Set
    End Property

    Public Property SecurityContainers() As String
        Get
            Return _securityContainers
        End Get
        Set(ByVal value As String)
            _securityContainers = value
        End Set
    End Property

    Public Property SecurityRoles() As String
        Get
            Return _securityRoles
        End Get
        Set(ByVal value As String)
            _securityRoles = value
        End Set
    End Property

    Property DocumentLogType() As String
        Get
            Return _documentLogType
        End Get
        Set(ByVal value As String)
            _documentLogType = value
        End Set
    End Property

    Property DocumentNotReaded() As Boolean
        Get
            Return _documentNotReaded
        End Get
        Set(ByVal value As Boolean)
            _documentNotReaded = value
        End Set
    End Property

    Property DocumentFolderExpired() As Boolean
        Get
            Return _documentFolderExpired
        End Get
        Set(ByVal value As Boolean)
            _documentFolderExpired = value
        End Set
    End Property

    Property DocumentFolderExpiredDate() As Nullable(Of Date)
        Get
            Return _documentFolderExpiredDate
        End Get
        Set(ByVal value As Nullable(Of Date))
            _documentFolderExpiredDate = value
        End Set
    End Property

    Property LoadDocumentFolderInfo() As Boolean
        Get
            Return _loadDocumentFolderInfo
        End Get
        Set(ByVal value As Boolean)
            _loadDocumentFolderInfo = value
        End Set
    End Property

    Public Property Status() As SearchStatus
        Get
            Return _status
        End Get
        Set(ByVal value As SearchStatus)
            _status = value
        End Set
    End Property

    Public Property IsWebService() As Boolean
        Get
            Return _webService
        End Get
        Set(ByVal value As Boolean)
            _webService = value
        End Set
    End Property

    Public Property EnablePaging() As Boolean
        Get
            Return _enablePaging
        End Get
        Set(ByVal value As Boolean)
            _enablePaging = value
        End Set
    End Property

    Public ReadOnly Property UsedDocumentObject() As Boolean
        Get
            Return _addDcDO
        End Get
    End Property

#End Region

#Region "Criteria"

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "D")

        'Year
        If Anno <> 0 Then
            criteria.Add(Restrictions.Eq("D.Year", Anno))
        End If

        'Number
        If Numero <> 0 Then
            criteria.Add(Restrictions.Eq("D.Number", Numero))
        End If

        'StartDate
        If DataInizioFrom.HasValue Then
            criteria.Add(Restrictions.Ge("D.StartDate", DataInizioFrom))
        End If
        If DataInizioTo.HasValue Then
            criteria.Add(Restrictions.Le("D.StartDate", DataInizioTo))
        End If


        'ExpiryDate
        If DataScadenzaFrom.HasValue Then
            criteria.Add(Restrictions.Ge("D.ExpiryDate", DataScadenzaFrom))
        End If
        If DataScadenzaTo.HasValue Then
            criteria.Add(Restrictions.Le("D.ExpiryDate", DataScadenzaTo))
        End If

        'ExpiryDate
        If EndDateFrom.HasValue Then
            Dim disju As Disjunction = Expression.Disjunction()
            disju.Add(Restrictions.Ge("D.EndDate", EndDateFrom))
            disju.Add(Restrictions.IsNull("D.EndDate"))
            criteria.Add(disju)
        End If
        If EndDateTo.HasValue Then
            criteria.Add(Restrictions.Le("D.ExpiryDate", EndDateTo))
        End If

        'ServiceNumber
        If Not String.IsNullOrEmpty(NumeroServizio) Then
            criteria.Add(Expression.Like("D.ServiceNumber", "%" & NumeroServizio & "%"))
        End If

        'Name
        If Not String.IsNullOrEmpty(Nome) Then
            criteria.Add(Expression.Like("D.Name", "%" & Nome & "%"))
        End If

        'DocumentObject
        If Not (String.IsNullOrEmpty(Oggetto)) Then
            Dim words As String() = Oggetto.Split(" "c)
            Select Case DocumentObjectSearch
                Case ObjectSearchType.AtLeastWord
                    Dim disju As Disjunction = Expression.Disjunction()
                    For Each word As String In words
                        disju.Add(Expression.Like("D.DocumentObject", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(disju)
                Case ObjectSearchType.AllWords
                    Dim conju As Conjunction = Expression.Conjunction()
                    For Each word As String In words
                        conju.Add(Expression.Like("D.DocumentObject", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(conju)
            End Select
        End If

        'Manager
        If Not String.IsNullOrEmpty(Manager) Then
            criteria.Add(Expression.Like("D.Manager", "%" & Manager & "%"))
        End If

        'Note
        If Not String.IsNullOrEmpty(Note) Then
            criteria.Add(Expression.Like("D.Note", "%" & Note & "%"))
        End If

        'Roles
        If Not String.IsNullOrEmpty(IDRole) Then
            Dim words As String()
            Dim disju As New Disjunction
            words = IDRole.Split(","c)
            criteria.CreateAlias("D.Role", "Role", SqlCommand.JoinType.LeftOuterJoin)

            For Each word As String In words
                disju.Add(Restrictions.Eq("Role.Id", Integer.Parse(word)))
            Next
            criteria.Add(disju)
        Else
            MyBase.AddJoinAlias(criteria, "D.Role", "Role", SqlCommand.JoinType.LeftOuterJoin)
        End If

        'Container
        If Not String.IsNullOrEmpty(IDContainer) Then
            Dim words As String()
            Dim disju As New Disjunction
            words = IDContainer.Split(","c)
            criteria.CreateAlias("D.Container", "Container", SqlCommand.JoinType.LeftOuterJoin)

            For Each word As String In words
                disju.Add(Restrictions.Eq("Container.Id", Integer.Parse(word)))
            Next
            criteria.Add(disju)
        Else
            MyBase.AddJoinAlias(criteria, "D.Container", "Container", SqlCommand.JoinType.LeftOuterJoin)
        End If

        'Category
        If Not String.IsNullOrEmpty(IDCategory) Then
            Dim words As String()
            Dim disju As New Disjunction
            words = IDCategory.Split(","c)
            criteria.CreateAlias("D.Category", "Category", SqlCommand.JoinType.LeftOuterJoin)
            criteria.CreateAlias("D.SubCategory", "SubCategory", SqlCommand.JoinType.LeftOuterJoin)

            For Each word As String In words
                If IncludeChildCategories Then
                    Dim disju2 As Disjunction = New Disjunction
                    disju2.Add(Expression.Like("SubCategory.FullIncrementalPath", word & "%"))
                    disju2.Add(Restrictions.Eq("Category.FullIncrementalPath", word))
                    disju.Add(disju2)
                Else
                    Dim disju2 As Disjunction = New Disjunction
                    disju2.Add(Restrictions.Eq("SubCategory.FullIncrementalPath", word))
                    Dim conju2 As Conjunction = New Conjunction
                    conju2.Add(Restrictions.Eq("Category.FullIncrementalPath", word))
                    conju2.Add(Restrictions.IsNull("SubCategory"))
                    disju2.Add(conju2)

                    disju.Add(disju2)
                End If
            Next
            criteria.Add(disju)
        Else
            MyBase.AddJoinAlias(criteria, "D.Category", "Category", SqlCommand.JoinType.LeftOuterJoin)
        End If

        If PresaInCarico Then
            Dim dcDocToken As DetachedCriteria = DetachedCriteria.For(GetType(DocumentToken))
            dcDocToken.Add([Property].ForName("Document.Id").EqProperty("D.Id"))
            dcDocToken.SetProjection(Projections.Id())
            dcDocToken.Add(Expression.In("DocumentTabToken.Id", New String() {"RC", "RR", "RP"}))
            dcDocToken.Add(Restrictions.Eq("IsActive", True))
            Dim aIdRoleDestination(IdRoleDestination.Count - 1) As String
            IdRoleDestination.CopyTo(aIdRoleDestination, 0)
            dcDocToken.Add(Expression.In("RoleDestination.Id", aIdRoleDestination))
            criteria.Add(Subqueries.Exists(dcDocToken))
        End If

        If Not String.IsNullOrEmpty(IDContact) Then
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact))
            dcCon.CreateCriteria("Documents").Add([Property].ForName("Document.Id").EqProperty("D.Id"))
            dcCon.SetProjection(Projections.Id())

            Dim words As String()
            Dim disju As New Disjunction
            words = IDContact.Split(","c)
            For Each word As String In words
                If IncludeChildContacts Then
                    disju.Add(Expression.Like("FullIncrementalPath", "%" & word & "%"))
                Else
                    disju.Add(Expression.Like("FullIncrementalPath", "%" & word))
                End If
            Next
            dcCon.Add(disju)
            criteria.Add(Subqueries.Exists(dcCon))
        End If

        criteria.CreateAlias("Contacts", "DocumentContact", JoinType.LeftOuterJoin)
        criteria.CreateAlias("DocumentContact.Contact", "Contact", JoinType.LeftOuterJoin)
        If Not DocumentContactIds.IsNullOrEmpty() Then
            Dim disju As New Disjunction()
            disju.Add(Restrictions.In("DocumentContact.Id.Id", DocumentContactIds.ToArray()))
            criteria.Add(disju)
        End If

        If Not String.IsNullOrEmpty(DocumentDescription) Then
            criteria.Add(Expression.Like("DO.Description", DocumentDescription, MatchMode.Anywhere))
            _addDcDO = True
        End If

        If DocumentDate_From.HasValue Then
            criteria.Add(Restrictions.Ge("DO.DocumentDate", DocumentDate_From))
            _addDcDO = True
        End If

        If DocumentDate_To.HasValue Then
            criteria.Add(Restrictions.Le("DO.DocumentDate", DocumentDate_To))
            _addDcDO = True
        End If


        If Not String.IsNullOrEmpty(DocumentObject) Then
            criteria.Add(Expression.Like("DO.DocObject", "%" & DocumentObject & "%"))
            _addDcDO = True
        End If

        If Not String.IsNullOrEmpty(DocumentReason) Then
            criteria.Add(Expression.Like("DO.Reason", "%" & DocumentReason & "%"))
            _addDcDO = True
        End If

        If Not String.IsNullOrEmpty(DocumentNote) Then
            criteria.Add(Expression.Like("DO.Note", "%" & DocumentNote & "%"))
            _addDcDO = True
        End If


        If DocumentRegDate_From.HasValue Then
            criteria.Add(Restrictions.Ge("DO.RegistrationDate", New DateTimeOffset(DocumentRegDate_From)))
            _addDcDO = True
        End If

        If DocumentRegDate_To.HasValue Then
            criteria.Add(Restrictions.Le("DO.RegistrationDate", New DateTimeOffset(DocumentRegDate_To)))
            _addDcDO = True
        End If

        If _addDcDO Then
            criteria.CreateAlias("Objects", "DO", SqlCommand.JoinType.LeftOuterJoin)
        End If

        'Stato Pratica
        Select Case _status
            Case SearchStatus.Open
                criteria.Add(Expression.Or(Expression.In("Status.Id", New String() {"AP", "RP"}), Restrictions.IsNull("Status.Id")))
            Case SearchStatus.Closed
                criteria.Add(Restrictions.Eq("Status.Id", "CP"))
            Case SearchStatus.Canceled
                criteria.Add(Restrictions.Eq("Status.Id", "PA"))
            Case SearchStatus.Archived
                criteria.Add(Restrictions.Eq("Status.Id", "AR"))
        End Select

        'Pratiche non lette
        If _documentNotReaded Then
            _documentLogType = "DS"
            criteria.Add(Subqueries.NotExists(GetDocumentLogCriteria()))
        ElseIf Not String.IsNullOrEmpty(DocumentLogType) Then
            criteria.Add(Subqueries.Exists(GetDocumentLogCriteria()))
        End If

        'Sicurezza
        If SecurityEnabled Then
            Dim disju As New Disjunction

            If Not String.IsNullOrEmpty(SecurityContainers) Then
                If (String.IsNullOrEmpty(IDContainer)) Then
                    If (MyBase.EnableTableJoin = False) Then
                        criteria.CreateAlias("D.Container", "Container", SqlCommand.JoinType.LeftOuterJoin)
                    End If
                End If
                disju.Add(Expression.In("Container.Id", SecurityContainers.Split(","c)))
            End If

            If Not String.IsNullOrEmpty(SecurityRoles) Then
                Dim disju2 As New Disjunction
                Dim dc As DetachedCriteria = DetachedCriteria.For(GetType(DocumentToken), "DT")
                dc.Add(Restrictions.EqProperty("D.Id", "DT.Id"))

                Dim conjDT1 As New Conjunction
                conjDT1.Add(Expression.In("DT.DocumentTabToken.Id", New String() {"PA", "PC", "PT"}))
                conjDT1.Add(Expression.In("DT.RoleDestination.Id", SecurityRoles.Split(","c)))
                disju2.Add(conjDT1)

                Dim conjDT2 As New Conjunction
                conjDT2.Add(Expression.Not(Restrictions.Eq("DT.Response", "A")))
                conjDT2.Add(Expression.In("DT.DocumentTabToken.Id", New String() {"RR", "RC", "CC"}))
                conjDT2.Add(Expression.In("DT.RoleDestination.Id", SecurityRoles.Split(","c)))
                disju2.Add(conjDT2)

                Dim conjDT3 As New Conjunction
                conjDT3.Add(Expression.In("DT.DocumentTabToken.Id", New String() {"MP"}))
                conjDT3.Add(Expression.In("DT.RoleDestination.Id", SecurityRoles.Split(","c)))
                disju2.Add(conjDT3)

                dc.Add(disju2)
                dc.SetProjection(Projections.Id())
                disju.Add(Subqueries.Exists(dc))
            End If

            criteria.Add(disju)
        End If

        'Cartelle scadute
        If DocumentFolderExpired Then
            LoadDocumentFolderInfo = True
            criteria.CreateAlias("DocumentFolders", "DocumentFolders", SqlCommand.JoinType.LeftOuterJoin)
            criteria.Add(Restrictions.IsNull("DocumentFolders.Role.Id"))
            criteria.Add(Expression.IsNotNull("DocumentFolders.ExpiryDate"))
            If DocumentFolderExpiredDate.HasValue Then
                criteria.Add(Restrictions.Le("DocumentFolders.ExpiryDate", DocumentFolderExpiredDate.Value))
            Else
                criteria.Add(Restrictions.Le("DocumentFolders.ExpiryDate", Date.Now()))
            End If
        End If

        AttachFilterExpressions(criteria)
        AttachSQLExpressions(criteria)

        Return criteria
    End Function

#End Region

#Region "Private Functions"
    Private Function GetDocumentLogCriteria() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(GetType(DocumentLog))
        dc.Add(Restrictions.Eq("LogType", _documentLogType))
        dc.Add(Restrictions.Eq("SystemUser", DocSuiteContext.Current.User.FullUserName))
        dc.Add([Property].ForName("Document.Id").EqProperty("D.Id"))
        dc.SetProjection(Projections.Id())
        Return dc
    End Function
#End Region

#Region "NHibernate Properties"

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetProjection(Projections.SqlProjection("COUNT(DISTINCT convert(varchar,{alias}.Year)+'/'+right('0000000'+convert(varchar,{alias}.Number),7)) as DOCMCOUNT", New String() {"DocmCount"}, New NHibernate.Type.Int32Type() {NHibernateUtil.Int32}))
        Return criteria.UniqueResult(Of Integer)
    End Function

    Public Overrides Function DoSearch() As IList(Of Document)

        Dim criteria As ICriteria = CreateCriteria()

        If Not MyBase.AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Ascending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of Document)()

    End Function

    Public Overrides Function DoSearchHeader() As IList(Of DocumentHeader)
        Dim criteria As ICriteria = CreateCriteria()

        If Not MyBase.AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Id", SortOrder.Ascending)
        End If

        If _enablePaging Then
            criteria.SetFirstResult(_startIndex)
            criteria.SetMaxResults(_pageSize)
        End If

        Dim projList As ProjectionList = Projections.ProjectionList()
        projList.Add(Projections.Property("Year"), "Year")
        projList.Add(Projections.Property("Number"), "Number")
        projList.Add(Projections.Property("StartDate"), "StartDate")
        projList.Add(Projections.Property("ServiceNumber"), "ServiceNumber")
        projList.Add(Projections.Property("Name"), "Name")
        projList.Add(Projections.Property("DocumentObject"), "DocumentObject")
        projList.Add(Projections.Property("Manager"), "Manager")
        projList.Add(Projections.Property("Note"), "Note")
        projList.Add(Projections.Property("Category"), "Category")
        projList.Add(Projections.Property("Category.Name"), "CategoryName")
        projList.Add(Projections.Property("SubCategory"), "SubCategory")
        projList.Add(Projections.Property("Container"), "Container")
        projList.Add(Projections.Property("Container.Name"), "ContainerName")
        projList.Add(Projections.Property("Role"), "Role")
        projList.Add(Projections.Property("Role.Name"), "RoleName")
        projList.Add(Projections.Property("Status"), "Status")
        projList.Add(Projections.SqlProjection(GetContactSQLString(), New String() {"CONTACTDESCRIPTION"}, New NHibernate.Type.StringType() {NHibernateUtil.String}), "ContactDescription")

        'ricerca con alcuni campi del documento
        If _addDcDO Then
            projList.Add(Projections.Property("DO.Description"), "DocumentObjectDescription")
        End If

        If LoadDocumentFolderInfo Then
            projList.Add(Projections.Property("DocumentFolders.Id.Incremental"), "FolderIncremental")
            projList.Add(Projections.Property("DocumentFolders.FolderName"), "FolderName")
            projList.Add(Projections.Property("DocumentFolders.ExpiryDate"), "FolderExpiryDate")
            projList.Add(Projections.Property("DocumentFolders.Description"), "FolderExpiryDescription")
        End If

        'Configurazione WebService
        If _webService Then
            projList.Add(Projections.Property("Location"), "Location")
        End If

        'Private _contactDescription As String
        criteria.SetProjection(Projections.Distinct(projList))
        criteria.SetResultTransformer(New NHibernate.Transform.AliasToBeanResultTransformer(GetType(DocumentHeader)))

        Return criteria.List(Of DocumentHeader)()
    End Function

    Public Overrides Function DoSearch(ByVal sortExpr As String) As IList(Of Document)
        MyBase.DoSearch(sortExpr)
        Return DoSearch()
    End Function

    Public Overrides Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pagSize As Integer) As IList(Of Document)
        MyBase.DoSearch(sortExpr, startRow, pagSize)
        Return DoSearch()
    End Function

    Private Function GetContactSQLString() As String
        Return "(SELECT TOP 1 Description" & _
               " FROM DocumentContact D" & _
               " LEFT JOIN Contact C ON D.IdContact = C.Incremental" & _
               " WHERE (Year = {alias}.Year AND Number = {alias}.Number)) as CONTACTDESCRIPTION"
    End Function

End Class
