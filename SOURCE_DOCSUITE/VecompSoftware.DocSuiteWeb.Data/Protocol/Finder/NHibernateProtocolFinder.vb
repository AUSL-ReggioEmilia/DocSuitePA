Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.SqlCommand
Imports NHibernate.Transform
Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.NHibernateManager.Transformer
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Helpers.NHibernate


<Serializable(), DataObject()>
Public Class NHibernateProtocolFinder
    Inherits NHibernateBaseFinder(Of Protocol, ProtocolHeader)

#Region " Fields "
    Private _numberFrom As Integer?
    Private _numberTo As Integer?
    Private _numberLike As Integer?
    Private _registrationDateFrom As Date?
    Private _registrationDateTo As Date?
    Private _registrationUser As String
    Private _protocolNotReaded As Boolean

    Private _idType As ICollection(Of Integer)
    Private _idContainer As String
    Private _idDocType As Integer?
    Private _documentProtocol As String
    Private _documentDateFrom As Date?
    Private _documentDateTo As Date?
    Private _protocolObject As String = String.Empty
    Private _protocolObjectSearch As ObjectSearchType
    Private _recipient As String = String.Empty
    Private _enableRecipientContains As Boolean = True
    Private _subject As String = String.Empty
    Private _serviceCategory As String = String.Empty
    Private _classifications As String = String.Empty
    Private _includeChildClassifications As Boolean
    Private _protocolStatusCancel As StatusSearchType
    Private _protocolNoRoles As Boolean
    Private _idattachement As Integer
    Private _iddocument As Integer
    Private _documentName As String
    Private _protocolOnlyLastChangedandRead As Boolean
    Private _lastChangedDateFrom As Date?
    Private _lastChangedDateTo As Date?

    'INVOICE
    Private _enableInvoice As Boolean = False
    Private _invoiceNumber As String
    Private _invoiceDateFrom As Date?
    Private _invoiceDateTo As Date?
    Private _accountingSectional As String = String.Empty
    Private _accountingYear As Nullable(Of Short)
    Private _accountingNumber As Integer?

    'INTEROP
    Private _contacts As String = String.Empty
    Private _includeChildContacts As Boolean

    'PACKAGE
    Private _enablePackage As Boolean = False
    Private _packageOrigin As String = String.Empty
    Private _package As Integer?
    Private _packageLot As Integer?
    Private _packageIncremental As Integer?

    Private _noStatus As Boolean = False
    'ADVANCED STATUS
    Private _advancedStatus As String = String.Empty

    Private _securityEnabled As Boolean
    Private _securityContainers As String
    Private _securityRoles As String
    Private _roleUser As String
    Private _roleCC As Nullable(Of Boolean)
    Private _roleDistributionType As String
    Private _notDistributed As Boolean
    Private _shunted As Boolean
    Private _assignedToWork As Boolean

    Private _securityNonManageableRoles As String

    'ADVANCED PROTOCOL FLAG
    Private flgAdvancedProtocolAlias As Boolean = False
    Private flgProtocolType As Boolean = False
    Private flgContainer As Boolean = False

    'Paginazione
    Private _enablePaging As Boolean = True

    'Top
    Private _topMaxRecords As Integer = 0

    Private _includeIncomplete As Boolean

    Private _idProtocolKind As Short
    Private _applyProtocolKindCriteria As Boolean

    Private _statusList As New List(Of Integer)
    Private _distributionEnable As Boolean

    Private _contactsAssignee As ICollection(Of String)
#End Region

#Region " Properties "

    Public Property Year As Short?
    Public Property Number As Integer?

    Public Property LoadFetchModeFascicleEnabled As Boolean = True

    Public Property LoadFetchModeProtocolLogs As Boolean = True
#End Region

#Region "Finder Properties"

    Public Property EnablePaging() As Boolean
        Get
            Return _enablePaging
        End Get
        Set(ByVal value As Boolean)
            _enablePaging = value
        End Set
    End Property


    Public Property NumberFrom As String
        Get
            Return _numberFrom.ToString
        End Get
        Set(ByVal value As String)
            _numberFrom = ConvertToInteger(value)
        End Set
    End Property

    Public Property NumberTo As String
        Get
            Return _numberTo.ToString
        End Get
        Set(ByVal value As String)
            _numberTo = ConvertToInteger(value)
        End Set
    End Property

    Public Property NumberLike As String
        Get
            Return _numberLike.ToString
        End Get
        Set(ByVal value As String)
            _numberLike = ConvertToInteger(value)
        End Set
    End Property

    Public Property ProtocolLogType As String

    Public Property RegistrationDateFrom() As Date?
        Get
            Return _registrationDateFrom
        End Get
        Set(ByVal value As Date?)
            _registrationDateFrom = value
        End Set
    End Property

    Public Property RegistrationUser() As String
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
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

    Property ProtocolNotReaded() As Boolean
        Get
            Return _protocolNotReaded
        End Get
        Set(ByVal value As Boolean)
            _protocolNotReaded = value
        End Set
    End Property

    Property ProtocolOnlyLastChangedandRead() As Boolean
        Get
            Return _protocolOnlyLastChangedandRead
        End Get
        Set(ByVal value As Boolean)
            _protocolOnlyLastChangedandRead = value
        End Set
    End Property

    Property IdTypes As ICollection(Of Integer)
        Get
            Return _idType
        End Get
        Set(ByVal value As ICollection(Of Integer))
            _idType = value
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

    Property IdDocType() As String
        Get
            Return _idDocType.ToString()
        End Get
        Set(ByVal value As String)
            _idDocType = ConvertToInteger(value)
        End Set
    End Property

    Property DocumentProtocol() As String
        Get
            Return _documentProtocol
        End Get
        Set(ByVal value As String)
            _documentProtocol = value
        End Set
    End Property

    Property DocumentDateFrom() As Date?
        Get
            Return _documentDateFrom
        End Get
        Set(ByVal value As Date?)
            _documentDateFrom = value
        End Set
    End Property

    Property DocumentDateTo() As Date?
        Get
            Return _documentDateTo
        End Get
        Set(ByVal value As Date?)
            _documentDateTo = value
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

    Property ProtocolObjectSearch() As ObjectSearchType
        Get
            Return _protocolObjectSearch
        End Get
        Set(ByVal value As ObjectSearchType)
            _protocolObjectSearch = value
        End Set
    End Property

    Property Note As String

    Property Recipient() As String
        Get
            Return _recipient.Replace("_", " ")
        End Get
        Set(ByVal value As String)
            _recipient = value.Replace(" ", "_")
        End Set
    End Property

    Property RestrictionOnlyRoles As Boolean

    Property EnableRecipientContains() As Boolean
        Get
            Return _enableRecipientContains
        End Get
        Set(ByVal value As Boolean)
            _enableRecipientContains = value
        End Set
    End Property

    Property Subject() As String
        Get
            Return _subject
        End Get
        Set(ByVal value As String)
            _subject = value
        End Set
    End Property

    Property ServiceCategory() As String
        Get
            Return _serviceCategory
        End Get
        Set(ByVal value As String)
            _serviceCategory = value
        End Set
    End Property

    Property Classifications() As String
        Get
            Return _classifications
        End Get
        Set(ByVal value As String)
            _classifications = value
        End Set
    End Property

    Property IncludeChildClassifications() As Boolean
        Get
            Return _includeChildClassifications
        End Get
        Set(ByVal value As Boolean)
            _includeChildClassifications = value
        End Set
    End Property

    Property IncludeChildRoles As Boolean

    Property ProtocolStatusCancel() As StatusSearchType
        Get
            Return _protocolStatusCancel
        End Get
        Set(ByVal value As StatusSearchType)
            _protocolStatusCancel = value
        End Set
    End Property
    Property ProtocolNoRoles() As Boolean
        Get
            Return _protocolNoRoles
        End Get
        Set(ByVal value As Boolean)
            _protocolNoRoles = value
        End Set
    End Property

    'INVOICE
    Property EnableInvoiceSearch() As Boolean
        Get
            Return _enableInvoice
        End Get
        Set(ByVal value As Boolean)
            _enableInvoice = value
        End Set
    End Property

    Property InvoiceNumber() As String
        Get
            Return _invoiceNumber
        End Get
        Set(ByVal value As String)
            _invoiceNumber = value
        End Set
    End Property

    Property InvoiceDateFrom() As Date?
        Get
            Return _invoiceDateFrom
        End Get
        Set(ByVal value As Date?)
            _invoiceDateFrom = value
        End Set
    End Property

    Property InvoiceDateTo() As Date?
        Get
            Return _invoiceDateTo
        End Get
        Set(ByVal value As Date?)
            _invoiceDateTo = value
        End Set
    End Property

    Property AccountingSectional() As String
        Get
            Return _accountingSectional
        End Get
        Set(ByVal value As String)
            _accountingSectional = value
        End Set
    End Property

    Property AccountingYear() As Nullable(Of Short)
        Get
            Return _accountingYear
        End Get
        Set(ByVal value As Nullable(Of Short))
            _accountingYear = value
        End Set
    End Property

    Property AccountingNumber() As Integer?
        Get
            Return _accountingNumber
        End Get
        Set(ByVal value As Integer?)
            _accountingNumber = value
        End Set
    End Property

    'PACKAGE
    Property EnablePackageSearch() As Boolean
        Get
            Return _enablePackage
        End Get
        Set(ByVal value As Boolean)
            _enablePackage = value
        End Set
    End Property

    Property PackageOrigin() As String
        Get
            Return _packageOrigin
        End Get
        Set(ByVal value As String)
            _packageOrigin = value
        End Set
    End Property

    Property Package() As String
        Get
            Return _package.ToString()
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) Then
                _package = Nothing
            Else
                _package = Convert.ToInt32(value)
            End If
        End Set
    End Property

    Property PackageLot() As String
        Get
            Return _packageLot.ToString()
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) Then
                _packageLot = Nothing
            Else
                _packageLot = Convert.ToInt32(value)
            End If
        End Set
    End Property

    Property PackageIncremental() As String
        Get
            Return _packageIncremental.ToString()
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) Then
                _packageIncremental = Nothing
            Else
                _packageIncremental = Convert.ToInt32(value)
            End If
        End Set
    End Property

    'INTEROP
    Property Contacts() As String
        Get
            Return _contacts
        End Get
        Set(ByVal value As String)
            _contacts = value
        End Set
    End Property

    Property IncludeChildContacts() As Boolean
        Get
            Return _includeChildContacts
        End Get
        Set(ByVal value As Boolean)
            _includeChildContacts = value
        End Set
    End Property

    Property IncludeIncomplete() As Boolean
        Get
            Return _includeIncomplete
        End Get
        Set(ByVal value As Boolean)
            _includeIncomplete = value
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

    Public Property SecurityNonManageableRoles() As String
        Get
            Return _securityNonManageableRoles
        End Get
        Set(ByVal value As String)
            _securityNonManageableRoles = value
        End Set
    End Property

    Public Property RoleUser() As String
        Get
            Return _roleUser
        End Get
        Set(ByVal value As String)
            _roleUser = value
        End Set
    End Property

    Public Property RoleCC() As Nullable(Of Boolean)
        Get
            Return _roleCC
        End Get
        Set(value As Nullable(Of Boolean))
            _roleCC = value
        End Set
    End Property

    Public Property RoleDistributionType() As String
        Get
            Return _roleDistributionType
        End Get
        Set(value As String)
            _roleDistributionType = value
        End Set
    End Property

    Public Property NotDistributed As Boolean
        Get
            Return _notDistributed
        End Get
        Set(value As Boolean)
            _notDistributed = value
        End Set
    End Property

    Public Property Shunted As Boolean
        Get
            Return _shunted
        End Get
        Set(value As Boolean)
            _shunted = value
        End Set
    End Property

    Public Property AssignedToWork As Boolean
        Get
            Return _assignedToWork
        End Get
        Set(value As Boolean)
            _assignedToWork = value
        End Set
    End Property

    'STATUS
    Property NoStatus() As Boolean
        Get
            Return _noStatus
        End Get
        Set(ByVal value As Boolean)
            _noStatus = value
        End Set
    End Property

    ''' <summary>  </summary>
    ''' <remarks>
    ''' TODO: nel db è uno short
    ''' </remarks>
    Property IdStatus() As Integer?

    Property AdvancedStatus() As String
        Get
            Return _advancedStatus
        End Get
        Set(ByVal value As String)
            _advancedStatus = value
        End Set
    End Property

    Property IdAttachement() As Integer
        Get
            Return _idattachement
        End Get
        Set(ByVal value As Integer)
            _idattachement = value
        End Set
    End Property
    Property IdDocument() As Integer
        Get
            Return _iddocument
        End Get
        Set(ByVal value As Integer)
            _iddocument = value
        End Set
    End Property

    Public Property DocumentName() As String
        Get
            Return _documentName
        End Get
        Set(ByVal value As String)
            _documentName = value
        End Set
    End Property

    ''' <summary> Reclami </summary>
    Public Property IsClaim() As Nullable(Of Boolean)

    Public Property HasJournalLog As Boolean?

    Public Property HasIngoingPecMails As Boolean?

    Public Property ApplyProtocolKindCriteria As Boolean
        Get
            Return _applyProtocolKindCriteria
        End Get
        Set(value As Boolean)
            _applyProtocolKindCriteria = value
        End Set
    End Property

    Public Property IdProtocolKind As Short
        Get
            Return _idProtocolKind
        End Get
        Set(value As Short)
            _idProtocolKind = value
        End Set
    End Property

    Public Property StatusList As List(Of Integer)
        Get
            Return _statusList
        End Get
        Set(value As List(Of Integer))
            _statusList = value
        End Set
    End Property

    Public Property DistributionEnable As Boolean
        Get
            Return _distributionEnable
        End Get
        Set(value As Boolean)
            _distributionEnable = value
        End Set
    End Property

    Public Property ProtocolHighlightToMe As Boolean

    Public Property OnlyExplicitRoles As Boolean

    Public Property ProtocolRoleStatus As ProtocolRoleStatus?

    Public Property IsInRefusedProtocolRoleGroup As Boolean

    Public Property DistributionRestrictionRoles As ICollection(Of Integer)

    Public Property AssignedToMe As Boolean = False
    Public Property AssignedToMeCC As Boolean = False
    Public Property IsFascicolated As Boolean?
    Public Property Status As Short
    Public Property LastChangedDateFrom() As Date?
        Get
            Return _lastChangedDateFrom
        End Get
        Set(ByVal value As Date?)
            _lastChangedDateFrom = value
        End Set
    End Property
    Public Property LastChangedDateTo() As Date?
        Get
            Return _lastChangedDateTo
        End Get
        Set(ByVal value As Date?)
            _lastChangedDateTo = value
        End Set
    End Property

    Property ContactsAssignee() As ICollection(Of String)
        Get
            Return _contactsAssignee
        End Get
        Set(ByVal value As ICollection(Of String))
            _contactsAssignee = value
        End Set
    End Property

    Public Property IdTenantAOO As Guid?

    Public Property IncludeCountRead As Boolean = False

    Public Property NeverDistributed As Boolean
#End Region

#Region "NHibernate Properties"

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property TopMaxRecords() As Integer
        Get
            Return _topMaxRecords
        End Get
        Set(ByVal value As Integer)
            _topMaxRecords = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
        RestrictionOnlyRoles = False
        Note = String.Empty
    End Sub

#End Region

#Region "Criteria"

    ''' <summary> Crea il criteria per NHibernate </summary>
    Protected Overrides Function CreateCriteria() As ICriteria

        flgAdvancedProtocolAlias = False
        flgContainer = False
        flgProtocolType = False

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Protocol)("P")

        'Filtro Anno
        If Year.HasValue Then
            criteria.Add(Restrictions.Eq("P.Year", Year.Value))
        End If

        'Filtro Numero
        If Number.HasValue Then
            criteria.Add(Restrictions.Eq("P.Number", Number.Value))
        End If

        'Filtro Numero Da
        If Not (String.IsNullOrEmpty(NumberFrom)) Then
            criteria.Add(Restrictions.Ge("P.Number", _numberFrom))
        End If

        'Filtro Numero A
        If Not (String.IsNullOrEmpty(NumberTo)) Then
            criteria.Add(Restrictions.Le("P.Number", _numberTo))
        End If

        'Filtro Number like
        If Not (String.IsNullOrEmpty(NumberLike)) Then
            'criteria.Add(Expression.Like("P.Number", String.Format("%{0}%", _numberLike)))
            criteria.Add(Restrictions.Like(Projections.Cast(NHibernateUtil.String, Projections.Property("P.Number")), _numberLike.ToString, MatchMode.Anywhere))
        End If

        'Filtro Data di Registrazione a partire da
        If RegistrationDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("RegistrationDate", _registrationDateFrom.Value)))
        End If

        'Filtro Data di Registrazione fino a
        If RegistrationDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("RegistrationDate", _registrationDateTo.Value)))
        End If

        ' Filtro per registrazione utente
        If Not (String.IsNullOrEmpty(RegistrationUser)) AndAlso (Not RestrictionOnlyRoles OrElse (RestrictionOnlyRoles AndAlso SecurityEnabled AndAlso String.IsNullOrEmpty(SecurityRoles))) Then
            criteria.Add(Restrictions.Like("RegistrationUser", RegistrationUser, MatchMode.Anywhere))
        End If

        If IdTenantAOO.HasValue Then
            criteria.Add(Restrictions.Eq("IdTenantAOO", IdTenantAOO.Value))
        End If

        ' Protocolli non letti
        If ProtocolNotReaded Then
            If DocSuiteContext.Current.ProtocolEnv.DisableProtocolLogHasBeenRead Then
                criteria.Add(Subqueries.NotExists(GetByLogTypes("P1", "PS")))
            Else
                criteria.Add(Subqueries.Eq(False, HasBeenRead()))
            End If
        ElseIf Not String.IsNullOrEmpty(ProtocolLogType) Then
            criteria.Add(Subqueries.Exists(GetByLogTypes(ProtocolLogType)))
        End If

        If ProtocolOnlyLastChangedandRead Then
            criteria.Add(Restrictions.IsNotNull("LastChangedDate"))
            'Filtro Data ultima modifica da
            If LastChangedDateFrom.HasValue Then
                criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("LastChangedDate", _lastChangedDateFrom.Value)))
            End If
            'Filtro Data ultima modifica fino a
            If LastChangedDateTo.HasValue Then
                criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("LastChangedDate", _lastChangedDateTo.Value)))
            End If
        End If

        'Filtro Tipologia protcollo (ingresso\uscita\ingresso e uscita)
        If Not IdTypes.IsNullOrEmpty() Then
            criteria.CreateAlias("P.Type", "Type", JoinType.LeftOuterJoin)
            ' Alias con la classe type creato, setta il flag di controllo a true
            flgProtocolType = True
            criteria.Add(Restrictions.In("Type.Id", IdTypes.ToList()))
        Else
            MyBase.AddJoinAlias(criteria, "P.Type", "Type", JoinType.LeftOuterJoin)
        End If

        'Container
        'Se nessun container impostato allora in join con tutti i contenitori
        If Not String.IsNullOrEmpty(IdContainer) Then
            Dim words As String() = IdContainer.Split(","c)
            criteria.CreateAlias("P.Container", "Container", JoinType.InnerJoin)
            ' Alias con la classe container creato, setta il flag di controllo a true
            flgContainer = True
            Dim disju As New Disjunction
            For Each word As String In words
                disju.Add(Restrictions.Eq("Container.Id", Integer.Parse(word)))
            Next
            criteria.Add(disju)
        Else
            MyBase.AddJoinAlias(criteria, "P.Container", "Container", JoinType.InnerJoin)
        End If

        'DocType
        If Not (String.IsNullOrEmpty(IdDocType)) Then
            criteria.CreateAlias("P.DocumentType", "DocumentType", JoinType.LeftOuterJoin)
            criteria.Add(Restrictions.Eq("DocumentType.Id", _idDocType))
        End If

        'Filtro Data Documento a partire da
        If DocumentDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("DocumentDate", _documentDateFrom.Value)))
        End If

        'Filtro Data Documento fino a
        If DocumentDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("DocumentDate", _documentDateTo.Value)))
        End If

        'Filtro nome documento
        If DocumentName <> String.Empty Then
            criteria.Add(Restrictions.Like("P.DocumentCode", "%" & _documentName & "%"))
        End If

        'Filtro Numero protocollo del mittente
        If Not (String.IsNullOrEmpty(DocumentProtocol)) Then
            criteria.Add(Restrictions.Like("P.DocumentProtocol", "%" & _documentProtocol & "%"))
        End If

        'Object
        If Not (String.IsNullOrEmpty(ProtocolObject)) Then
            Dim words As String() = ProtocolObject.Split(" "c)
            Select Case ProtocolObjectSearch
                Case ObjectSearchType.AtLeastWord
                    Dim disju As Disjunction = Restrictions.Disjunction()
                    For Each word As String In words
                        disju.Add(Restrictions.Like("P.ProtocolObject", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(disju)
                Case ObjectSearchType.AllWords
                    Dim conju As Conjunction = Restrictions.Conjunction()
                    For Each word As String In words
                        conju.Add(Restrictions.Like("P.ProtocolObject", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(conju)
                Case ObjectSearchType.ExactWord
                    criteria.Add(Restrictions.Like("P.ProtocolObject", ProtocolObject, MatchMode.Anywhere))
            End Select
        End If

        'Filtro Note (sia Note del campo Oggetto che Note di Settore)
        If Not String.IsNullOrEmpty(Note) Then
            Dim dcRole As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole))
            dcRole.Add(Restrictions.Like("Note", "%" & Note & "%"))
            dcRole.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
            dcRole.SetProjection(Projections.Id())

            AddAvancedProtocolCriteria(criteria)
            criteria.Add(Restrictions.Disjunction() _
            .Add(Restrictions.Like("AP.Note", "%" & Note & "%")) _
                .Add(Subqueries.Exists(dcRole))
            )
        End If

        If DocSuiteContext.Current.ProtocolEnv.SearchRecipientCustom Then
            Try
                ' Cerco il mittente/destinatario per gruppi di tabelle
                If Not String.IsNullOrEmpty(Recipient) Then
                    Dim dcDis As Disjunction = Restrictions.Disjunction()

                    ' Elenco di contatti che corrispondono alla ricerca fatta, eseguo una ricerca
                    Dim contactfinder As New NHibernateContactFinder(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                    contactfinder.Description = Recipient
                    contactfinder.DescriptionContain = EnableRecipientContains
                    Dim contacts As IList(Of Contact) = contactfinder.DoSearch()

                    ' Elenco di ProtocolContact collegati ai contatti di cui sopra
                    If contacts.Count > 0 Then
                        Dim pcfinder As New NHibernateProtocolContactFinder(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                        Dim ids As New List(Of Integer)
                        For Each item As Contact In contacts
                            ids.Add(item.Id)
                        Next
                        pcfinder.IdContacts = ids
                        'Filtro Anno
                        pcfinder.Year = Year
                        'Filtro Numero
                        pcfinder.Number = Number

                        Dim protocolcontacts As IList(Of ProtocolContact) = pcfinder.DoSearch()

                        If protocolcontacts.Count > 0 Then
                            ' Eseguo filtro per numero protocolli
                            For Each item As ProtocolContact In protocolcontacts
                                dcDis = DisjunctionProtContCompositeKey(item, dcDis)
                            Next
                        End If
                    End If

                    ' Elenco di ProtocolContactManual per descrizione
                    Dim PCMfinder As New NHibernateProtocolContactManualFinder(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                    PCMfinder.Description = Recipient
                    PCMfinder.DescriptionContain = EnableRecipientContains
                    'Filtro Anno
                    PCMfinder.Year = Year
                    'Filtro Numero
                    PCMfinder.Number = Number
                    Dim pcms As IList(Of ProtocolContactManual) = PCMfinder.DoSearch()

                    If pcms.Count > 0 Then
                        ' Eseguo filtro per numero protocolli
                        For Each item As ProtocolContactManual In pcms
                            dcDis = DisjunctionProtContManualCompositeKey(item, dcDis)
                        Next
                    End If
                    criteria.Add(dcDis)
                End If
            Catch ex As Exception
                Throw New DocSuiteException("Errore in fase di ricerca Custom", ex)
            End Try
        Else
            If DocSuiteContext.Current.ProtocolEnv.SearchProtocolRecipientChildren Then
                Try
                    ' Cerco il mittente/destinatario per gruppi di tabelle
                    If Not String.IsNullOrEmpty(Recipient) Then

                        Dim dcDis As Disjunction = Restrictions.Disjunction()

                        ' Elenco di contatti che corrispondono alla ricerca fatta, eseguo una ricerca
                        Dim contactfinder As New NHibernateContactFinder(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                        ' Abilito la ricerca nei fogli dei contatti individuati. 
                        contactfinder.SearchDescendantOf = True
                        contactfinder.Description = Recipient
                        contactfinder.DescriptionContain = EnableRecipientContains
                        Dim contacts As IList(Of Contact) = contactfinder.DoSearch()

                        ' Elenco di ProtocolContact collegati ai contatti di cui sopra
                        If contacts.Count > 0 Then
                            Dim pcfinder As New NHibernateProtocolContactFinder(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                            Dim ids As New List(Of Integer)
                            For Each item As Contact In contacts
                                ids.Add(item.Id)
                            Next
                            pcfinder.IdContacts = ids
                            'Filtro Anno
                            pcfinder.Year = Year
                            'Filtro Numero
                            pcfinder.Number = Number

                            Dim protocolcontacts As IList(Of ProtocolContact) = pcfinder.DoSearch()

                            If protocolcontacts.Count > 0 Then
                                ' Eseguo filtro per numero protocolli
                                For Each item As ProtocolContact In protocolcontacts
                                    dcDis = DisjunctionProtContCompositeKey(item, dcDis)
                                Next
                            End If
                        End If

                        ' Elenco di ProtocolContactManual per descrizione
                        Dim PCMfinder As New NHibernateProtocolContactManualFinder(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
                        PCMfinder.Description = Recipient
                        PCMfinder.DescriptionContain = EnableRecipientContains
                        'Filtro Anno
                        PCMfinder.Year = Year
                        'Filtro Numero
                        PCMfinder.Number = Number
                        Dim pcms As IList(Of ProtocolContactManual) = PCMfinder.DoSearch()

                        If pcms.Count > 0 Then
                            ' Eseguo filtro per numero protocolli
                            For Each item As ProtocolContactManual In pcms
                                dcDis = DisjunctionProtContManualCompositeKey(item, dcDis)
                            Next
                        End If
                        criteria.Add(dcDis)
                    End If
                Catch ex As Exception
                    Throw New DocSuiteException("Errore in fase di ricerca gerarchica all'interno dei contatti", ex)
                End Try
            Else
                If Not String.IsNullOrEmpty(Recipient) Then

                    Dim recipientMatchMode As MatchMode = If(EnableRecipientContains, MatchMode.Anywhere, MatchMode.Start)

                    Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact))
                    dcCon.CreateCriteria("Protocols").Add([Property].ForName("Protocol.Id").EqProperty("P.Id"))
                    Dim orClause As Disjunction = New Disjunction()
                    orClause.Add(Restrictions.Like("Description", _recipient, recipientMatchMode))
                    dcCon.Add(orClause)
                    dcCon.SetProjection(Projections.Id())

                    Dim dcConMan As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolContactManual))
                    dcConMan.Add(Restrictions.Like("Contact.Description", _recipient, recipientMatchMode))
                    dcConMan.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
                    dcConMan.SetProjection(Projections.Id())

                    Dim dcConIss As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolContactIssue))
                    dcConIss.CreateCriteria("Contact", "C")
                    dcConIss.Add(Restrictions.Like("C.Description", _recipient, recipientMatchMode))
                    dcConIss.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
                    dcConIss.SetProjection(Projections.Id())

                    Dim disj As Disjunction = Restrictions.Disjunction()
                    disj.Add(Restrictions.Like("P.AlternativeRecipient", _recipient, recipientMatchMode))
                    disj.Add(Subqueries.Exists(dcCon))
                    disj.Add(Subqueries.Exists(dcConMan))

                    If DocSuiteContext.Current.ProtocolEnv.IsIssueEnabled Then
                        disj.Add(Subqueries.Exists(dcConIss))
                    End If
                    criteria.Add(disj)
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(Contacts) Then
            'ProtocolContacts
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact))
            dcCon.CreateCriteria("Protocols").Add([Property].ForName("Protocol.Id").EqProperty("P.Id"))
            dcCon.SetProjection(Projections.Id())
            Dim disjunction As Disjunction = Restrictions.Disjunction()
            disjunction.Add(Restrictions.Eq("FullIncrementalPath", _contacts))
            If IncludeChildContacts Then
                disjunction.Add(Restrictions.Like("FullIncrementalPath", _contacts & "|%"))
            End If
            dcCon.Add(disjunction)

            'ProtocolManualContacts
            Dim dcContact As DetachedCriteria = DetachedCriteria.For(GetType(Contact))
            If IncludeChildContacts Then
                dcContact.Add(Expression.Like("FullIncrementalPath", _contacts & "%"))
            Else
                dcContact.Add(Restrictions.Eq("FullIncrementalPath", _contacts))
            End If
            dcContact.SetProjection(Projections.SqlProjection("REPLACE(Description,'|','_') AS CDESCR", New String() {"CDESCR"}, New NHibernate.Type.StringType() {NHibernateUtil.String}))
            Dim contactDescr As String = String.Empty
            Try
                contactDescr = dcContact.GetExecutableCriteria(NHibernateSession).UniqueResult(Of String)()
            Catch ex As Exception
                If TypeOf ex Is HibernateException Then
                    Dim contactsDescr As IList(Of String) = dcContact.GetExecutableCriteria(NHibernateSession).List(Of String)()
                    contactDescr = contactsDescr(0)
                End If
            End Try

            Dim dcConMan As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolContactManual))
            dcConMan.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
            dcConMan.Add(Restrictions.Like("Contact.Description", contactDescr, MatchMode.Start))
            dcConMan.SetProjection(Projections.Id)

            'ContactIssue
            Dim dcConIss As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolContactIssue))
            dcConIss.CreateCriteria("Contact", "CIssue")
            If IncludeChildContacts Then
                dcConIss.Add(Restrictions.Like("CIssue.FullIncrementalPath", _contacts & "%"))
            Else
                dcConIss.Add(Restrictions.Eq("CIssue.FullIncrementalPath", _contacts))
            End If
            dcConIss.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
            dcConIss.SetProjection(Projections.Id())

            Dim disj As Disjunction = Restrictions.Disjunction()
            disj.Add(Subqueries.Exists(dcCon))
            If DocSuiteContext.Current.ProtocolEnv.IsIssueEnabled Then
                disj.Add(Subqueries.Exists(dcConIss))
                disj.Add(Subqueries.Exists(dcConMan))
            End If
            criteria.Add(disj)
        End If

        If Not String.IsNullOrEmpty(Subject) Then
            AddAvancedProtocolCriteria(criteria)
            criteria.Add(Restrictions.Like("AP.Subject", "%" & _subject & "%"))
        End If

        If Not String.IsNullOrEmpty(ServiceCategory) Then
            AddAvancedProtocolCriteria(criteria)
            criteria.Add(Restrictions.Like("AP.ServiceCategory", "%" & _serviceCategory & "%"))
        End If

        If Not String.IsNullOrEmpty(Classifications) Then
            criteria.CreateAliasIfNotExists("Category", "Category", JoinType.InnerJoin)
            Dim cl() As String = Classifications.Split("|"c)

            'Ho selezionato una categoria
            Dim disjCriteria As Disjunction = Expression.Disjunction()
            Dim conj As Conjunction = Expression.Conjunction()
            conj.Add(Restrictions.Eq("Category.FullIncrementalPath", Classifications))
            disjCriteria.Add(conj)

            If IncludeChildClassifications Then
                disjCriteria.Add(Expression.Like("Category.FullIncrementalPath", Classifications & "%"))
            End If
            criteria.Add(disjCriteria)
        Else
            MyBase.AddJoinAlias(criteria, "P.Category", "Category", JoinType.InnerJoin)
        End If

        Dim statusArray As List(Of Integer) = New List(Of Integer)

        Select Case ProtocolStatusCancel
            Case StatusSearchType.EvenStatusCancel
                statusArray.Add(ProtocolStatusId.Attivo)
                StatusList.Add(ProtocolStatusId.Annullato)
            Case StatusSearchType.OnlyStatusCancel
                statusArray.Add(ProtocolStatusId.Annullato)
            Case StatusSearchType.OnlyStatusActive
                statusArray.Add(ProtocolStatusId.Attivo)
        End Select

        If IncludeIncomplete Then
            statusArray.Add(ProtocolStatusId.Incompleto)
        End If

        If StatusList.Any() Then
            statusArray.AddRange(StatusList)
        End If

        If NoStatus OrElse statusArray.Contains(ProtocolStatusId.Attivo) Then
            For i As Integer = ProtocolStatusId.Attivo To ProtocolStatusId.PAInvoiceRefused
                statusArray.Add(i)
            Next
        End If

        If Not statusArray.Any() AndAlso Not NoStatus AndAlso Not IdStatus.HasValue Then
            criteria.Add(Restrictions.Ge("IdStatus", Convert.ToInt32(ProtocolStatusId.Attivo)))
        End If

        If IdStatus.HasValue Then
            statusArray.Add(IdStatus.Value)
        End If

        If statusArray.Any() Then
            criteria.Add(Restrictions.In("IdStatus", statusArray.Distinct().ToArray()))
        End If

        If ApplyProtocolKindCriteria Then
            criteria.Add(Restrictions.Eq("IdProtocolKind", IdProtocolKind))
        End If


        If ProtocolNoRoles Then
            Dim dcRole As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole))
            dcRole.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
            dcRole.SetProjection(Projections.Id())
            criteria.Add(Restrictions.Disjunction() _
                .Add(Subqueries.NotExists(dcRole))
            )
        End If

        If SecurityEnabled Then
            Dim disju As New Disjunction
            ' Aggiungo la condizione di default della serie di condizioni in OR.
            ' Se non vengono aggiunte condizioni la Disjuction risulterà FALSE e non mostrerà nessun record.
            disju.Add(Expression.Sql("1=0"))

            If Not String.IsNullOrEmpty(SecurityContainers) AndAlso Not RestrictionOnlyRoles AndAlso Not OnlyExplicitRoles Then
                If (String.IsNullOrEmpty(IdContainer)) Then
                    If (MyBase.EnableTableJoin = False) Then
                        criteria.CreateAlias("P.Container", "Container", JoinType.InnerJoin)
                    End If
                End If

                disju.Add(Expression.In("Container.Id", SecurityContainers.Split(","c)))
            End If

            If DistributionEnable Then
                If Not String.IsNullOrEmpty(SecurityRoles) Then
                    disju.Add(Subqueries.Exists(ManagerAuthorizedCriteria(MatchMode.Anywhere)))
                End If

                If Not String.IsNullOrEmpty(SecurityNonManageableRoles) Then
                    Dim conjNonManageableCc As New Conjunction
                    Dim dcNonManageableRoles As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole), "PR")
                    dcNonManageableRoles.Add(Restrictions.EqProperty("PR.Protocol.Id", "P.Id"))
                    dcNonManageableRoles.Add(Expression.In("PR.Role.Id", SecurityNonManageableRoles.Split(","c)))

                    If RoleCC.HasValue Then
                        dcNonManageableRoles.Add(Restrictions.Eq("PR.Type", "CC"))
                    End If
                    dcNonManageableRoles.SetProjection(Projections.Id)

                    If RoleCC.HasValue Then
                        If RoleCC.Value Then
                            conjNonManageableCc.Add(Subqueries.Exists(dcNonManageableRoles))
                        Else
                            conjNonManageableCc.Add(Subqueries.NotExists(dcNonManageableRoles))
                        End If
                    End If

                    Dim dcCurrentUserInRoleUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRoleUser), "PRU")
                    dcCurrentUserInRoleUser.Add(Restrictions.EqProperty("PRU.Protocol.Id", "P.Id"))
                    dcCurrentUserInRoleUser.Add(Restrictions.In("PRU.Role.Id", SecurityNonManageableRoles.Split(","c)))
                    dcCurrentUserInRoleUser.Add(Restrictions.Eq("PRU.Account", RoleUser))
                    dcCurrentUserInRoleUser.SetProjection(Projections.Id)
                    conjNonManageableCc.Add(Subqueries.Exists(dcCurrentUserInRoleUser))

                    disju.Add(conjNonManageableCc)
                End If

                If DistributionRestrictionRoles IsNot Nothing AndAlso DistributionRestrictionRoles.Count > 0 Then
                    Dim dcDistributionRoles As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole), "PR")
                    dcDistributionRoles.Add(Restrictions.EqProperty("PR.Protocol.Id", "P.Id"))
                    dcDistributionRoles.Add(Restrictions.Eq("PR.DistributionType", Explicit))
                    If IncludeChildRoles AndAlso DistributionRestrictionRoles.Count() = 1 Then
                        dcDistributionRoles.CreateAlias("Role", "R", JoinType.InnerJoin)
                        dcDistributionRoles.Add(Restrictions.Like("R.FullIncrementalPath", DistributionRestrictionRoles.First().ToString(), MatchMode.Anywhere))
                    Else
                        dcDistributionRoles.Add(Restrictions.InG("PR.Role.Id", DistributionRestrictionRoles))
                    End If
                    dcDistributionRoles.SetProjection(Projections.Id)
                    criteria.Add(Subqueries.Exists(dcDistributionRoles))
                End If
            Else
                If Not String.IsNullOrEmpty(SecurityRoles) Then
                    Dim conjProtocolRole As New Conjunction()

                    Dim dcProtocolRole As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole), "PR")
                    dcProtocolRole.Add(Restrictions.EqProperty("PR.Protocol.Id", "P.Id"))
                    dcProtocolRole.Add(Restrictions.In("Role.Id", SecurityRoles.Split(","c)))
                    If DocSuiteContext.Current.ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso ProtocolRoleStatus.HasValue Then
                        dcProtocolRole.Add(Restrictions.Eq("PR.Status", ProtocolRoleStatus.Value))
                    End If
                    dcProtocolRole.SetProjection(Projections.Id)

                    conjProtocolRole.Add(Subqueries.Exists(dcProtocolRole))
                    disju.Add(conjProtocolRole)

                End If
            End If

            If DocSuiteContext.Current.SimplifiedPrivacyEnabled Then
                Dim dcProtocolUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolUser), "PU")
                dcProtocolUser.Add(Restrictions.EqProperty("PU.Protocol.Id", "P.Id"))
                dcProtocolUser.Add(Restrictions.Eq("PU.Account", DocSuiteContext.Current.User.FullUserName))
                dcProtocolUser.Add(Restrictions.Eq("PU.Type", ProtocolUserType.Authorization))
                dcProtocolUser.SetProjection(Projections.Id)
                disju.Add(Subqueries.Exists(dcProtocolUser))
            End If

            If DocSuiteContext.Current.ProtocolEnv.CorporateAcronym.Contains("AUSL-PC") Then
                ' Se per AUSL-PC rendo visibili anche i protocolli inseriti da me.
                disju.Add(Restrictions.Eq("P.RegistrationUser", DocSuiteContext.Current.User.FullUserName))
            End If

            criteria.Add(disju)
        Else
            ' Security disabilitata: cerco per utente specifico
            If Not String.IsNullOrEmpty(RoleUser) Then
                Dim dcRoleUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRoleUser))
                dcRoleUser.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
                dcRoleUser.Add(Restrictions.Eq("Account", RoleUser))
                ' Se filtro per settori
                If Not String.IsNullOrEmpty(SecurityRoles) Then
                    dcRoleUser.Add(Restrictions.In("Role.Id", SecurityRoles.Split(","c)))
                End If
                dcRoleUser.SetProjection(Projections.Id())

                criteria.Add(Subqueries.Exists(dcRoleUser))
            End If
        End If

        If DocSuiteContext.Current.ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso IsInRefusedProtocolRoleGroup AndAlso ProtocolRoleStatus.HasValue Then
            Dim conjProtocolRejectedRole As New Conjunction()
            Dim dcProtocolRejectedRole As DetachedCriteria = Nothing
            If ProtocolRoleStatus.Value.Equals(DocSuiteWeb.Data.ProtocolRoleStatus.Refused) Then
                dcProtocolRejectedRole = DetachedCriteria.For(GetType(ProtocolRejectedRole), "PRR")
            Else
                dcProtocolRejectedRole = DetachedCriteria.For(GetType(ProtocolRole), "PRR")
            End If
            dcProtocolRejectedRole.Add(Restrictions.EqProperty("PRR.Protocol.Id", "P.Id"))
            dcProtocolRejectedRole.Add(Restrictions.Eq("PRR.Status", ProtocolRoleStatus.Value))
            dcProtocolRejectedRole.SetProjection(Projections.Id)
            conjProtocolRejectedRole.Add(Subqueries.Exists(dcProtocolRejectedRole))
            criteria.Add(conjProtocolRejectedRole)
        End If

        If NotDistributed Then
            criteria.Add(CriteriaToDitribute())
            'criteria.Add(CriteriaUserCanDistributeByContainers)
        End If

        If Shunted Then
            criteria.Add(CriteriaShunted())
        End If

        If AssignedToWork Then
            criteria.Add(CriteriaAssignedToWork())
        End If

        If Not ContactsAssignee.IsNullOrEmpty() Then
            Dim dcRoleUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRoleUser))
            dcRoleUser.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
            dcRoleUser.Add(Restrictions.Eq("Account", ContactsAssignee.First))
            dcRoleUser.SetProjection(Projections.Id())
            criteria.Add(Subqueries.Exists(dcRoleUser))
        End If
        ' Ricerca solo DA DISTRIBUIRE


        'PACKAGE
        If EnablePackageSearch Then
            AddAvancedProtocolCriteria(criteria)
            CreatePackageCriteria(criteria)
        End If

        'INVOICE
        If EnableInvoiceSearch Then
            AddAvancedProtocolCriteria(criteria)
            CreateInvoiceCriteria(criteria)
        End If

        If Not String.IsNullOrEmpty(AdvancedStatus) Then
            AddAvancedProtocolCriteria(criteria)
            criteria.Add(Restrictions.Eq("AP.Status.Id", AdvancedStatus))
        End If

        If IdAttachement <> 0 And IdDocument <> 0 Then
            criteria.Add(Restrictions.Or(Restrictions.Eq("IdAttachments", IdAttachement), Restrictions.Eq("IdDocument", IdDocument)))
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsClaimEnabled AndAlso IsClaim.HasValue Then
            If IsClaim.Value Then
                criteria.Add(Restrictions.Eq("AP.IsClaim", True))
            Else
                Dim disju As New Disjunction
                disju.Add(Restrictions.Eq("AP.IsClaim", False))
                disju.Add(Restrictions.IsNull("AP.IsClaim"))
                criteria.Add(disju)
            End If
        End If

        If HasIngoingPecMails.GetValueOrDefault(False) Then
            criteria.CreateAliasIfNotExists("PecMails", "PM", JoinType.InnerJoin)
            criteria.Add(Restrictions.Eq("PM.Direction", Convert.ToInt16(PECMailDirection.Ingoing)))
        End If

        If ProtocolHighlightToMe Then
            criteria.CreateAlias("Users", "U", JoinType.InnerJoin)
            criteria.Add(Restrictions.Eq("U.Account", DocSuiteContext.Current.User.FullUserName))
            criteria.Add(Restrictions.Eq("U.Type", ProtocolUserType.Highlight))
        End If

        If IsFascicolated Then
            Dim dcFascicleDocumentUnit As DetachedCriteria = DetachedCriteria.For(GetType(FascicleDocumentUnit), "FDU")
            dcFascicleDocumentUnit.Add(Restrictions.EqProperty("FDU.IdDocumentUnit", "P.Id"))
            dcFascicleDocumentUnit.SetProjection(Projections.Id)
            criteria.Add(Subqueries.Exists(dcFascicleDocumentUnit))
        End If

        If (NeverDistributed) Then
            Dim dcProtocolRoleUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRoleUser), "PRU")
            dcProtocolRoleUser.Add(Restrictions.EqProperty("PRU.Protocol.Id", "P.Id"))
            dcProtocolRoleUser.SetProjection(Projections.Id)
            criteria.Add(Subqueries.NotExists(dcProtocolRoleUser))
        End If

        AddAvancedProtocolCriteria(criteria)
        AttachFilterExpressions(criteria)
        AttachSQLExpressions(criteria)
        NHibernateSession.EnableFilter("LogUser").SetParameter("User", DocSuiteContext.Current.User.FullUserName)
        NHibernateSession.EnableFilter("LogType").SetParameter("Type", "P1")

        Return criteria
    End Function
    ' Creazione condizione di disjunction  (YEAR = 'YYYY' AND NUMBER = 'XX') OR (YEAR = 'YYYY' AND NUMBER = 'XX')
    Private Function DisjunctionProtContCompositeKey(item As ProtocolContact, dcDis As Disjunction) As Disjunction
        Dim dcCon As Conjunction = Restrictions.Conjunction()
        dcCon.Add(Restrictions.Eq("P.Id", item.Protocol.Id))
        dcDis.Add(dcCon)
        Return dcDis
    End Function

    Private Function DisjunctionProtContManualCompositeKey(item As ProtocolContactManual, dcDis As Disjunction) As Disjunction
        Dim dcCon As Conjunction = Restrictions.Conjunction()
        dcCon.Add(Restrictions.Eq("P.Id", item.Protocol.Id))
        dcDis.Add(dcCon)
        Return dcDis
    End Function


    Private Function OnlyInSecurityContainer() As AbstractCriterion
        Return Restrictions.In("Container.Id", SecurityContainers.Split(","c))
    End Function

    Private Function ManagableRole() As Disjunction
        Dim manageableRoleDisj As New Disjunction
        For Each manageableIdRole As String In SecurityRoles.Split(","c)
            manageableRoleDisj.Add(Restrictions.Like("R.FullIncrementalPath", manageableIdRole, MatchMode.Anywhere))
        Next
        Return manageableRoleDisj
    End Function

    ''' <summary>
    ''' DetachedCriteria che individua i Settori Autorizzati per i protocolli
    ''' </summary>
    ''' <param name="onlyLegacy">Restringe il campo ai soli miei sotto settori</param>
    ''' <param name="excludeMyOwn">Esclude dall'elenco i settori in cui sono Manager</param>
    Private Function SubQueryProtocolRole(onlyLegacy As Boolean, excludeMyOwn As Boolean) As DetachedCriteria
        Dim dcRole As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole), "PR")
        dcRole.Add(Restrictions.EqProperty("PR.Protocol.Id", "P.Id"))
        If onlyLegacy Then
            ' Solo quelle nel mio Sotto albero
            dcRole.CreateAlias("Role", "R", JoinType.InnerJoin)
            dcRole.Add(Restrictions.EqProperty("PR.Role.Id", "R.Id"))
            dcRole.Add(ManagableRole)
        End If
        If excludeMyOwn Then
            ' Escludo le autorizzazioni proprie
            dcRole.Add(Restrictions.Not(Restrictions.In("R.Id", SecurityRoles.Split(","c))))
        End If
        dcRole.SetProjection(Projections.Id())
        Return dcRole
    End Function

    Private Function SubQueryProtocolRoleUser(onlyLegacy As Boolean) As DetachedCriteria

        Dim dcRoleUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRoleUser), "PRU")
        dcRoleUser.Add(Restrictions.EqProperty("PRU.Protocol.Id", "P.Id"))
        If onlyLegacy Then
            dcRoleUser.CreateAlias("Role", "R", JoinType.InnerJoin)
            dcRoleUser.Add(Restrictions.EqProperty("PRU.Role.Id", "R.Id"))
            dcRoleUser.Add(ManagableRole)

        End If
        dcRoleUser.SetProjection(Projections.Id())

        Return dcRoleUser

    End Function

    Private Function CriteriaShunted() As ICriterion
        Dim conju As New Conjunction
        ' Nessun operatore autorizzato
        conju.Add(Subqueries.NotExists(SubQueryProtocolRoleUser(Not String.IsNullOrEmpty(SecurityRoles))))

        Dim disju As New Disjunction()
        disju.Add(Expression.Sql("1=0")) 'Se nessuna delle prossime condizioni è vera allora l'intera Disjuction deve essere FALSA

        ' Se ho diritto sul contenitore ALMENO un Settore autorizzato
        If Not String.IsNullOrEmpty(SecurityContainers) Then
            Dim conjContainer As New Conjunction
            ' Diritto sul contenitore
            conjContainer.Add(OnlyInSecurityContainer)
            conjContainer.Add(Subqueries.Exists(SubQueryProtocolRole(False, False)))
            disju.Add(conjContainer)
        End If

        ' Se sono Manager almeno un Sottosettore nei miei rami (esclusi i miei settori)
        If Not String.IsNullOrEmpty(SecurityRoles) Then
            ' Deve esistere almento un Settore
            disju.Add(Subqueries.Exists(SubQueryProtocolRole(True, True)))
        End If

        conju.Add(disju)

        Return conju
    End Function

    Private Function CriteriaAssignedToWork() As ICriterion
        Dim conju As New Conjunction
        Dim dcRoleUser As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRoleUser))
        dcRoleUser.Add(Restrictions.EqProperty("Protocol.Id", "P.Id"))
        dcRoleUser.Add(Restrictions.Eq("Account", RoleUser))
        dcRoleUser.Add(Restrictions.Eq("Status", Status))
        ' Se filtro per settori
        If Not String.IsNullOrEmpty(SecurityRoles) Then
            dcRoleUser.Add(Restrictions.In("Role.Id", SecurityRoles.Split(","c)))
        End If
        dcRoleUser.SetProjection(Projections.Id())
        conju.Add(Subqueries.Exists(dcRoleUser))
        Return conju
    End Function

    Private Function ManagerAuthorizedCriteria(machMode As MatchMode) As DetachedCriteria
        Dim dcRole As DetachedCriteria = DetachedCriteria.For(GetType(ProtocolRole), "PR")
        dcRole.Add(Restrictions.EqProperty("PR.Protocol.Id", "P.Id"))

        ' Aggiungo la verifica si tratti di un  abbinamento ProtocolRole per i quali ho permessi da manager di distribuzione.
        ' Ricreo la join fra ProtocolRole e Role perchè per qualche motivo a me sconosciuto l'entità ProtocolRole non aggancia il provider di Role.
        dcRole.CreateAlias("Role", "R", JoinType.InnerJoin)
        dcRole.Add(Restrictions.EqProperty("PR.Role.Id", "R.Id"))

        If OnlyExplicitRoles Then
            ' Verifico di essere manager del role corrente. (autorizzazione esplicita)
            Dim roleIds As Integer() = SecurityRoles.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()
            dcRole.Add(Restrictions.In("R.Id", roleIds))
        Else
            Dim manageableRoleDisj As New Disjunction
            ' Verifico che il Role corrente sia figlio di un Role del quale sono manager. (autorizzazione implicita)
            For Each manageableIdRole As String In SecurityRoles.Split(","c)
                manageableRoleDisj.Add(Restrictions.Like("R.FullIncrementalPath", manageableIdRole, machMode))
            Next
            dcRole.Add(manageableRoleDisj)
        End If

        ' Settori in copia conoscenza.
        If RoleCC.HasValue Then
            If RoleCC.Value Then
                dcRole.Add(Restrictions.Eq("PR.Type", "CC"))
            Else
                dcRole.Add(Restrictions.IsNull("PR.Type"))
            End If
        End If

        ' Settori autorizzati Implicitamente/Esplicitamente.
        If Not String.IsNullOrEmpty(RoleDistributionType) Then
            dcRole.Add(Restrictions.Eq("DistributionType", RoleDistributionType))
        End If
        dcRole.SetProjection(Projections.Id())

        Return dcRole

    End Function

    Private Function CriteriaToDitribute() As ICriterion
        Dim disju As New Disjunction
        disju.Add(Expression.Sql("1=0")) 'Se nessuna delle prossime condizioni è vera allora l'intera Disjuction deve essere FALSA

        ' Se ho diritti sul contenitore allora non deve esserci nessuna autorizzazione
        If Not String.IsNullOrEmpty(SecurityContainers) Then
            Dim conj As New Conjunction()
            conj.Add(OnlyInSecurityContainer)
            conj.Add(Subqueries.NotExists(SubQueryProtocolRole(False, False)))
            disju.Add(conj)
        End If

        ' Se sono manager non deve esserci nessun sotto-settore autorizzato e nessun operatore autorizzato
        If Not String.IsNullOrEmpty(SecurityRoles) Then
            Dim conj As New Conjunction()
            conj.Add(Subqueries.Exists(ManagerAuthorizedCriteria(MatchMode.Anywhere)))
            conj.Add(Subqueries.NotExists(SubQueryProtocolRole(True, True)))
            conj.Add(Subqueries.NotExists(SubQueryProtocolRoleUser(True)))
            disju.Add(conj)
        End If

        Return disju

    End Function

#End Region

#Region "IFinder Implementation"
    Protected Overrides Function CreateProjections(ByRef criteria As ICriteria) As Boolean
        If SelectProjections.Count <= 0 Then
            Return False
        End If

        Dim projList As ProjectionList = Projections.ProjectionList()
        Try
            For Each [property] As String In SelectProjections.Keys
                projList.Add(Projections.Property([property]), SelectProjections([property]))
            Next
            criteria.SetProjection(Projections.Distinct(projList))
            criteria.SetResultTransformer(New TupleToPropertyResultTransformer(persistentType, SelectProjections, True))
        Catch ex As Exception
            Throw New DocSuiteException("Errore proiezioni", "Errore nell'impostazione proiezioni", ex)
        End Try
        Return True
    End Function

    Public Overrides Function DoSearch(ByVal sortExpr As String) As IList(Of Protocol)
        MyBase.DoSearch(sortExpr)
        Return DoSearch()
    End Function

    Public Overrides Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pageSize As Integer) As IList(Of Protocol)
        MyBase.DoSearch(sortExpr)
        PageIndex = startRow
        Me.PageSize = pageSize
        Return DoSearch()
    End Function

#End Region

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        criteria.SetProjection(Projections.RowCount())

        Dim countRecords As Integer
        Using transaction As ITransaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadUncommitted)
            Try
                countRecords = criteria.UniqueResult(Of Integer)()
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        If TopMaxRecords > 0 Then
            Return Math.Min(countRecords, TopMaxRecords)
        End If
        Return countRecords
    End Function

    Protected Overrides Function AttachSortExpressions(ByRef criteria As ICriteria) As Boolean
        If SortExpressions.Count = 0 Then
            If DocSuiteContext.Current.ProtocolEnv.ForceDescendingOrderElements Then
                SortExpressions.Add("Year", "DESC")
                SortExpressions.Add("Number", "DESC")
            Else
                For Each keyValuePair As KeyValuePair(Of String, String) In DocSuiteContext.Current.ProtocolEnv.DefaultProtocolSortExpression
                    SortExpressions.Add(keyValuePair)
                Next
            End If
        Else
            If SortExpressions.ContainsKey("Id") Then
                SortExpressions.Add("Year", SortExpressions("Id"))
                SortExpressions.Add("Number", SortExpressions("Id"))
                SortExpressions.Remove("Id")
            End If
        End If
        MyBase.AttachSortExpressions(criteria)
    End Function

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)

        If EnablePaging Then
            criteria.SetFirstResult(_startIndex)
            criteria.SetMaxResults(_pageSize)
        End If

        'Recupera i fascicoli con una sottoquery solo se abilitati
        If DocSuiteContext.Current.ProtocolEnv.FascicleEnabled AndAlso LoadFetchModeFascicleEnabled Then
            LoadFetchMode(criteria, "FascicleDocumentUnits")
        End If

        If LoadFetchModeProtocolLogs Then
            LoadFetchMode(criteria, "ProtocolLogs")
        End If

        If HasJournalLog.HasValue Then
            If HasJournalLog Then
                criteria.Add(Restrictions.Not(Restrictions.Eq("JournalLog.Id", 0)))
                criteria.Add(Restrictions.Not(Restrictions.IsNull("JournalLog.Id")))
            Else
                Dim disjJournalLog As New Disjunction
                disjJournalLog.Add(Expression.Sql("1=0"))
                disjJournalLog.Add(Restrictions.Eq("JournalLog.Id", 0))
                disjJournalLog.Add(Restrictions.IsNull("JournalLog"))

                criteria.Add(disjJournalLog)
            End If
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        If TopMaxRecords > 0 Then
            criteria.SetResultTransformer(New TopRecordsResultTransformer(TopMaxRecords))
        End If
    End Sub

    Public Overrides Function DoSearch() As IList(Of Protocol)
        Dim criteria As ICriteria = CreateCriteria()

        AttachSortExpressions(criteria)
        'Decora il criterio con Expressioni di ordinamento e modalità Fetch
        DecorateCriteria(criteria)
        'Crea le eventuali proiezioni
        CreateProjections(criteria)
        Dim result As IList(Of Protocol)
        Using transaction As ITransaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadUncommitted)
            Try
                result = criteria.List(Of Protocol)()
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result
    End Function

    ''' <summary> Decora un DetachedCriteria per conteggiare i collegamenti per protocollo. </summary>
    Private Function detachedProtocolLinkRowCount() As DetachedCriteria
        Dim disj As New Disjunction()
        Dim conj1 As New Conjunction()
        conj1.Add(Restrictions.EqProperty("P.Id", "pl_Links.Protocol.Id"))
        disj.Add(conj1)
        Dim conj2 As New Conjunction()
        conj2.Add(Restrictions.EqProperty("P.Id", "pl_Links.ProtocolLinked.Id"))
        disj.Add(conj2)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ProtocolLink)("pl_Links")
        dc.Add(disj)
        dc.SetProjection(Projections.RowCount)
        Return dc
    End Function

    ''' <summary> Decora un DetachedCriteria per conteggiare le letture per protocollo. </summary>
    Private Function detachedReadRowCount() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ProtocolLog)("pl_readCount")
        dc.Add(Restrictions.EqProperty("P.Year", "pl_readCount.Year"))
        dc.Add(Restrictions.EqProperty("P.Number", "pl_readCount.Number"))
        dc.Add(Restrictions.Eq("pl_readCount.SystemUser", DocSuiteContext.Current.User.FullUserName))
        dc.Add(Restrictions.In("pl_readCount.LogType", New String() {"P1", "PS"}))
        dc.SetProjection(Projections.RowCount)
        Return dc
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of ProtocolHeader)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        SetProjectionHeaders(criteria)
        AttachSortExpressions(criteria)
        Dim result As IList(Of ProtocolHeader)

        Using transaction As ITransaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadUncommitted)
            Try
                result = criteria.List(Of ProtocolHeader)()
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Dim conversion As IList(Of ProtocolHeader) = New List(Of ProtocolHeader)(result.Count)
        For Each a As ProtocolHeader In result
            a.RegistrationDate = a.RegistrationDate.ToLocalTime()
            conversion.Add(a)
        Next

        Return conversion
    End Function

#Region "Statistics"
    Public Function DoStat(ByVal groupBy As String) As IList
        Dim criteria As ICriteria = CreateCriteria()
        If Not String.IsNullOrEmpty(groupBy) Then
            If groupBy.Eq("status") Then
                criteria.SetProjection(Projections.ProjectionList() _
                    .Add(Projections.GroupProperty("IdStatus"), "groupedByIdStatus") _
                    .Add(Projections.RowCount(), "rows")
                )
            ElseIf groupBy.Eq("type") Then
                ' Se non esiste l'alias lo crea
                If flgProtocolType = False Then
                    criteria.CreateAliasIfNotExists("P.Type", "Type", JoinType.LeftOuterJoin)
                End If
                criteria.SetProjection(Projections.ProjectionList() _
                   .Add(Projections.GroupProperty("Type.Description")) _
                   .Add(Projections.GroupProperty("Type")) _
                   .Add(Projections.RowCount())
               )
            ElseIf groupBy.Eq("container") Then
                If flgContainer = False Then
                    criteria.CreateAliasIfNotExists("P.Container", "Container", JoinType.InnerJoin)
                End If
                criteria.SetProjection(Projections.ProjectionList() _
                   .Add(Projections.GroupProperty("Container.Id")) _
                   .Add(Projections.GroupProperty("Container.Name")) _
                   .Add(Projections.RowCount())
               )
            ElseIf groupBy.Eq("user") Then
                criteria.SetProjection(Projections.ProjectionList() _
                    .Add(Projections.GroupProperty("RegistrationUser"), "groupedByUser") _
                    .Add(Projections.RowCount(), "rows")
                )
            End If
        Else
            criteria.SetProjection(Projections.ProjectionList() _
                       .Add(Projections.RowCount(), "rows")
                   )
        End If
        Return criteria.List()
    End Function

#End Region

#Region "Private Functions"
    Private Sub CreateInvoiceCriteria(ByRef criteria As ICriteria)
        'Numero fattura
        If Not String.IsNullOrEmpty(InvoiceNumber) Then
            criteria.Add(Restrictions.Eq("AP.InvoiceNumber", InvoiceNumber))
        End If
        'Data fattura da
        If InvoiceDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("AP.InvoiceDate", InvoiceDateFrom))
        End If
        'Data fattura a
        If InvoiceDateTo.HasValue Then
            criteria.Add(Restrictions.Le("AP.InvoiceDate", InvoiceDateTo))
        End If
        'Accounting Sectional
        If Not String.IsNullOrEmpty(AccountingSectional) Then
            criteria.Add(Expression.Like("AP.AccountingSectional", AccountingSectional, MatchMode.Anywhere))
        End If
        'Accounting Year
        If AccountingYear.HasValue Then
            criteria.Add(Restrictions.Eq("AP.AccountingYear", AccountingYear.Value))
        End If
        'Accounting Number
        If AccountingNumber.HasValue Then
            criteria.Add(Restrictions.Eq("AP.AccountingNumber", AccountingNumber.Value))
        End If
    End Sub

    Private Sub CreatePackageCriteria(ByRef criteria As ICriteria)
        'Tiplogia Package
        If Not String.IsNullOrEmpty(_packageOrigin) Then
            criteria.Add(Restrictions.Eq("AP.PackageOrigin", Convert.ToChar(_packageOrigin)))
        End If
        'Scatolone Package
        If _package.HasValue Then
            criteria.Add(Restrictions.Eq("AP.Package", _package.Value))
        End If
        'Lotto Package
        If _packageLot.HasValue Then
            criteria.Add(Restrictions.Eq("AP.PackageLot", _packageLot.Value))
        End If
        'Progressivo Package
        If _packageIncremental.HasValue Then
            criteria.Add(Restrictions.Eq("AP.PackageIncremental", _packageIncremental.Value))
        End If
    End Sub

    ''' <summary> Ritorna una costante se il protocollo ha almeno un log di uno dei tipi specificati </summary>
    ''' <param name="types">Tipi di log di cui verificare la presenza.</param>
    Private Function GetByLogTypes(ByVal ParamArray types() As String) As DetachedCriteria
        Dim result As DetachedCriteria = DetachedCriteria.For(Of ProtocolLog)("PL")
        With result
            .SetMaxResults(1)
            .Add(Restrictions.EqProperty("PL.Protocol.Id", "P.Id"))
            .Add(Restrictions.Eq("PL.SystemUser", DocSuiteContext.Current.User.FullUserName))
            .Add(Restrictions.In("PL.LogType", types.ToArray()))
            .SetProjection(Projections.Constant(True))
        End With
        Return result
    End Function

    ''' <summary> Ritorna una sottoquery che indica con un booleano se il protocollo è stato letto </summary>
    Private Function HasBeenRead() As DetachedCriteria
        Dim readTypes As String() = DocSuiteContext.Current.ProtocolEnv.ProtocolLogReadTypes.ToArray()
        Dim editTypes As String() = DocSuiteContext.Current.ProtocolEnv.ProtocolLogEditTypes.ToArray()

        ' FG20131004: Considero l'elenco di tutte le mie sessioni di lettura.
        Dim readByMe As New Conjunction()
        With readByMe
            .Add(Restrictions.Eq("PL.SystemUser", DocSuiteContext.Current.User.FullUserName))
            .AddRestrictionsIn("PL.LogType", readTypes)
        End With

        ' FG20131004: Considero l'elenco di tutte le altrui sessioni di modifica.
        Dim editedByOthers As New Conjunction()
        With editedByOthers
            .Add(Restrictions.Not(Restrictions.Eq("PL.SystemUser", DocSuiteContext.Current.User.FullUserName)))
            .AddRestrictionsIn("PL.LogType", editTypes)

            ' FG20131004: Se configurato considero le sole notifiche di modifica
            If DocSuiteContext.Current.ProtocolEnv.ProtocolLogEditDaysThreshold > 0 Then
                Dim threshold As Date = Date.Today.AddDays(-1 * DocSuiteContext.Current.ProtocolEnv.ProtocolLogEditDaysThreshold)
                .Add(Restrictions.Ge("PL.LogDate", threshold))
            End If
        End With

        Dim disj As New Disjunction()
        With disj
            .Add(editedByOthers)
            .Add(readByMe)
        End With

        ' FG20131004: Se l'ultimo log inserito è di lettura considero il protocollo come letto.
        Dim result As DetachedCriteria = DetachedCriteria.For(Of ProtocolLog)("PL")
        With result
            .SetMaxResults(1)
            .Add(disj)
            .Add(Restrictions.EqProperty("PL.Protocol.Id", "P.Id"))
            .SetProjection(Projections.Conditional(Restrictions.In("PL.LogType", readTypes), Projections.Constant(True), Projections.Constant(False)))
            .AddOrder(Order.Desc("PL.LogDate"))
        End With

        Return result
    End Function

    Private Sub AddAvancedProtocolCriteria(ByRef criteria As ICriteria)
        If Not flgAdvancedProtocolAlias Then
            Dim advancedProtocolCriteria As ICriteria = criteria.CreateAlias("AdvanceProtocol", "AP", JoinType.LeftOuterJoin)
            If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
                advancedProtocolCriteria.CreateAlias("AP.Status", "Status", JoinType.LeftOuterJoin)
            End If
            flgAdvancedProtocolAlias = True
        End If
    End Sub
#End Region

#Region " Methods "

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overridable Sub SetProjectionHeaders(ByRef criteria As ICriteria)
        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("P.Year"), "Year")
        proj.Add(Projections.Property("P.Number"), "Number")
        proj.Add(Projections.Property("P.RegistrationDate"), "RegistrationDate")
        proj.Add(Projections.Property("P.RegistrationUser"), "RegistrationUser")
        proj.Add(Projections.Property("P.LastChangedDate"), "LastChangedDate")
        proj.Add(Projections.Property("P.ProtocolObject"), "ProtocolObject")
        proj.Add(Projections.Property("P.IdDocument"), "IdDocument")
        proj.Add(Projections.Property("P.IdAttachments"), "IdAttachments")
        proj.Add(Projections.Property("P.DocumentCode"), "DocumentCode")
        proj.Add(Projections.Property("P.Type"), "Type")
        proj.Add(Projections.Property("P.IdStatus"), "IdStatus")
        proj.Add(Projections.Property("P.Id"), "UniqueId")

        proj.Add(Projections.Property("Container.Id"), "ContainerId")
        proj.Add(Projections.Property("Container.Name"), "ContainerName")

        proj.Add(Projections.Property("Category.Id"), "CategoryId")
        proj.Add(Projections.Property("Category.Code"), "CategoryCode")
        proj.Add(Projections.Property("Category.FullCode"), "CategoryFullCode")
        proj.Add(Projections.Property("Category.Name"), "CategoryName")

        proj.Add(Projections.Property("P.DocumentProtocol"), "DocumentProtocol")

        If DocSuiteContext.Current.ProtocolEnv.IsStatusEnabled Then
            proj.Add(Projections.Property("AP.Status"), "ProtocolStatus")
        End If

        proj.Add(Projections.Property("AP.Subject"), "Subject")

        'Conteggia i collegamenti per protocollo.
        proj.Add(Projections.SubQuery(detachedProtocolLinkRowCount), "Links")
        If DocSuiteContext.Current.ProtocolEnv.IsLogStatusEnabled AndAlso IncludeCountRead Then
            ' Conteggia le letture per protocollo.
            proj.Add(Projections.SubQuery(detachedReadRowCount), "ReadCount")
        End If
        If DocSuiteContext.Current.ProtocolEnv.IsProtocolAttachLocationEnabled Then
            proj.Add(Projections.Property("P.AttachLocation"), "AttachLocation")
        End If
        If DocSuiteContext.Current.ProtocolEnv.IsPECEnabled Then
            Dim dcIngoingPecId As DetachedCriteria = DetachedCriteria.For(Of PECMail)("SPM")
            dcIngoingPecId.CreateAlias("SPM.DocumentUnit", "SPMDU")
            With dcIngoingPecId
                .Add(Restrictions.EqProperty("P.Id", "SPMDU.Id"))
                .Add(Restrictions.Eq("SPMDU.Environment", DirectCast(DSWEnvironment.Protocol, Integer)))
                .SetProjection(Projections.Max(Projections.Property("SPM.Id")))
            End With
            proj.Add(Projections.SubQuery(dcIngoingPecId), "IngoingPecId")
        End If

        proj.Add(Projections.SubQuery(HasPecsType(PECMailType.PEC, PECMailDirection.Ingoing)), "CountPECInPECStatus")
        proj.Add(Projections.SubQuery(HasPecsType(PECMailType.Anomalia, PECMailDirection.Ingoing)), "CountPECInAnomaliaStatus")
        proj.Add(Projections.SubQuery(HasPecsType(PECMailType.Receipt, PECMailDirection.Ingoing)), "CountPECInReceiptStatus")
        proj.Add(Projections.SubQuery(HasPecsType(PECMailType.Notifica, PECMailDirection.Ingoing)), "CountPECInNotificaStatus")
        proj.Add(Projections.SubQuery(HasPecsType(PECMailDirection.Outgoing)), "CountPECOutgoingStatus")

        HasSignaturesType(proj)

        If DocSuiteContext.Current.ProtocolEnv.RefusedProtocolAuthorizationEnabled AndAlso IsInRefusedProtocolRoleGroup AndAlso ProtocolRoleStatus.Equals(Data.ProtocolRoleStatus.ToEvaluate) Then
            ToEvaluateRolesCount(proj)
        End If

        If DocSuiteContext.Current.ProtocolEnv.IsInvoiceDataGridResultEnabled Then
            proj.Add(Projections.Property("AP.AccountingSectional"), "AccountingSectional")
            proj.Add(Projections.Property("AP.InvoiceYear"), "InvoiceYear")
            proj.Add(Projections.Property("AP.InvoiceNumber"), "InvoiceNumber")
            proj.Add(Projections.Property("AP.AccountingNumber"), "AccountingNumber")
        End If

        criteria.SetProjection(Projections.Distinct(proj))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ProtocolHeader))
    End Sub

    Private Function HasPecsType(pecType As PECMailType, direction As PECMailDirection) As DetachedCriteria
        Dim tbAlias As String = String.Concat("SPM_", DirectCast(pecType, Integer))
        Dim enumName As String = [Enum].GetName(GetType(PECMailType), pecType)
        Dim enumValue As Integer = DirectCast(pecType, Integer)
        Dim dcHasPecType As DetachedCriteria = DetachedCriteria.For(Of PECMail)(tbAlias)
        dcHasPecType.CreateAlias($"{tbAlias}.DocumentUnit", "SPMDU")

        With dcHasPecType
            .Add(Restrictions.EqProperty("P.Id", "SPMDU.Id"))
            .Add(Restrictions.Eq("SPMDU.Environment", DirectCast(DSWEnvironment.Protocol, Integer)))
            .Add(Restrictions.Eq(String.Concat(tbAlias, ".Direction"), Convert.ToInt16(direction)))
            .Add(Restrictions.Eq(String.Concat(tbAlias, ".IsActive"), ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
            .SetProjection(
            Projections.Sum(
                Projections.Conditional(
                    Restrictions.Eq(String.Concat(tbAlias, ".PECType"), pecType),
                                    Projections.Constant(1),
                                    Projections.Constant(0)
                    )
                )
            )
        End With
        Return dcHasPecType
    End Function

    Private Function HasPecsType(direction As PECMailDirection) As DetachedCriteria
        Dim dcHasPecType As DetachedCriteria = DetachedCriteria.For(Of PECMail)("SPM_I")
        dcHasPecType.CreateAlias("SPM_I.DocumentUnit", "SPM_I_DU")

        With dcHasPecType
            .Add(Restrictions.EqProperty("P.Id", "SPM_I_DU.Id"))
            .Add(Restrictions.Eq("SPM_I_DU.Environment", DirectCast(DSWEnvironment.Protocol, Integer)))
            .Add(Restrictions.Eq("SPM_I.IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
            .Add(Restrictions.Eq("SPM_I.Direction", Convert.ToInt16(direction)))
            .SetProjection(Projections.CountDistinct("SPM_I.Id"))
        End With
        Return dcHasPecType
    End Function
    Private Function HasSignaturesType(proj As ProjectionList) As DetachedCriteria
        Dim dcHasPECSignature As DetachedCriteria = DetachedCriteria.For(Of PECMail)("SPM_Segnatura")
        dcHasPECSignature.CreateAlias("SPM_Segnatura.DocumentUnit", "SPM_Segnatura_DU")

        With dcHasPECSignature
            .Add(Restrictions.EqProperty("P.Id", "SPM_Segnatura_DU.Id"))
            .Add(Restrictions.Eq("SPM_Segnatura_DU.Environment", DirectCast(DSWEnvironment.Protocol, Integer)))
            .SetProjection(
            Projections.Sum(
                Projections.Conditional(
                    Restrictions.And(Restrictions.IsNotNull("SPM_Segnatura.Segnatura"), (Restrictions.Not(Restrictions.Eq("SPM_Segnatura.Segnatura", String.Empty)))),
                                     Projections.Constant(1),
                                     Projections.Constant(0)
                    )
                )
            )
        End With
        proj.Add(Projections.SubQuery(dcHasPECSignature), "CountPECSegnatura")
    End Function

    Private Sub ToEvaluateRolesCount(proj As ProjectionList)
        Dim toEvaluateRoles As DetachedCriteria = DetachedCriteria.For(Of ProtocolRole)("PR")

        With toEvaluateRoles
            .Add(Restrictions.EqProperty("P.Id", "PR.Protocol.Id"))
            .Add(Restrictions.Eq("PR.Status", Data.ProtocolRoleStatus.ToEvaluate))
            .SetProjection(Projections.Count("PR.Id"))
        End With
        proj.Add(Projections.SubQuery(toEvaluateRoles), "CountToEvaluateRoles")
    End Sub

#End Region

End Class
