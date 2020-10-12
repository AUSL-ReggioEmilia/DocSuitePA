Imports VecompSoftware.Services.Logging
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Reflection
Imports System.Xml.Serialization
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.Services.Biblos
Imports NHibernate.SqlCommand
Imports System.Web.UI.WebControls
Imports VecompSoftware.Services.Biblos.Models

<Serializable(), DataObject()> _
Public Class DocumentSeriesItemFinder
    Inherits NHibernateDocumentSeriesItemFinder(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))

    Private Enum BiblosFetchMode
        Full = 1
        AttributesOnly = 2
    End Enum

#Region " Constructors "

    Public Sub New()
        Me.New(True, Nothing)
    End Sub
    Public Sub New(impersonating As String)
        Me.New(True, impersonating)
    End Sub
    Public Sub New(security As Boolean, impersonating As String)
        MyBase.New()

        EnableSecurity = security
        ImpersonatingUser = impersonating
        If EnableSecurity Then
            ApplySecurity()
        End If
    End Sub

#End Region

#Region " Fields "

    <NonSerialized()> _
    Private _documentSeriesItemFacade As DocumentSeriesItemFacade
    <NonSerialized()> _
    Private _documentSeriesFacade As DocumentSeriesFacade
    <NonSerialized()> _
    Private _biblosResultDictionary As IDictionary(Of Guid, BiblosChainInfo)
    <NonSerialized()> _
    Private _biblosContentSearchResult As IList(Of BiblosChainInfo)
    Private _viewDraftAssociatedResolution As Boolean = True

#End Region

#Region " Properties "

    <XmlIgnore> _
    Private ReadOnly Property DocumentSeriesItemFacade As DocumentSeriesItemFacade
        Get
            If _documentSeriesItemFacade Is Nothing Then
                _documentSeriesItemFacade = New DocumentSeriesItemFacade()
            End If
            Return _documentSeriesItemFacade
        End Get
    End Property
    <XmlIgnore> _
    Private ReadOnly Property DocumentSeriesFacade As DocumentSeriesFacade
        Get
            If _documentSeriesFacade Is Nothing Then
                _documentSeriesFacade = New DocumentSeriesFacade()
            End If
            Return _documentSeriesFacade
        End Get
    End Property

    Public Property EnableSecurity As Boolean

    Public Property OnlyActive As Boolean

    Public Property ImpersonatingUser As String

    Public Property IncludeSubsections As Boolean

    Public Property ViewDraftAssociatedResolution As Boolean
        Get
            Return _viewDraftAssociatedResolution
        End Get
        Set(value As Boolean)
            _viewDraftAssociatedResolution = value
        End Set
    End Property

    <XmlIgnore> _
    Public Property BiblosContentSearchResult As IList(Of BiblosChainInfo)
        Get
            Return _biblosContentSearchResult
        End Get
        Set(value As IList(Of BiblosChainInfo))
            _biblosContentSearchResult = value
            ResetContentSearchCascading()

            If _biblosContentSearchResult IsNot Nothing Then
                If _biblosContentSearchResult.Count > 0 Then
                    IdMainIn = _biblosContentSearchResult.Select(Function(bci) bci.ID.Value).ToList()
                Else
                    ' Qualora non avessi nessun risultato dalla ricerca in Biblos inserisco un valore fail-safe.
                    IdMainIn = New List(Of Guid)({New Guid("{10000000-0000-0000-0000-000000000001}")})
                End If
            End If
        End Set
    End Property

    <XmlIgnore>
    Private ReadOnly Property BiblosResultDictionary As IDictionary(Of Guid, BiblosChainInfo)
        Get
            If _biblosResultDictionary Is Nothing AndAlso BiblosContentSearchResult IsNot Nothing Then
                Dim dictionary As Dictionary(Of Guid, BiblosChainInfo) = BiblosContentSearchResult.ToDictionary(Of Guid, BiblosChainInfo)(Function(keys) keys.ID.Value, Function(values) values)

                If dictionary.Count > 0 Then
                    _biblosResultDictionary = dictionary
                End If
            End If
            Return _biblosResultDictionary
        End Get
    End Property

    Private Property SecurityIdDocumentSeriesIn As IList(Of Integer)
    ''' <summary>
    ''' Elenco dei settori a cui appartiene l'operatore corrente
    ''' </summary>
    Private Property SecurityRoleFullIncrementalPathIn As List(Of String)
    ''' <summary>
    ''' Tipo di Link a Role che garantisce il diritto di accesso all'Item
    ''' </summary>
    Private Property SecurityLinkTypeIn As List(Of DocumentSeriesItemRoleLinkType)

    Public Property FindByConstraints As Boolean?

    Public Property Constraint As String
#End Region

    Private Sub ApplySecurity()
        Dim roles As IList(Of Role)
        Dim series As IList(Of DocumentSeries)

        ' Elenco delle DocumentSeries su cui ho diritto di View
        If Not String.IsNullOrEmpty(ImpersonatingUser) Then
            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Inizio Caricamento Serie utente impersonificato: {0}", ImpersonatingUser))
            series = FacadeFactory.Instance.DocumentSeriesFacade.GetSeries(DocSuiteContext.Current.CurrentDomainName, ImpersonatingUser, DocumentSeriesContainerRightPositions.Preview)
            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Trovati {0} serie", If(series Is Nothing, 0, series.Count)))

            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Inizio Caricamento Settori utente impersonificato: {0}", ImpersonatingUser))
            roles = FacadeFactory.Instance.RoleFacade.GetUserRoles(DocSuiteContext.Current.CurrentDomainName, ImpersonatingUser, DSWEnvironment.DocumentSeries, 1, Nothing)
            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Trovati {0} settori", If(roles Is Nothing, 0, roles.Count)))
        Else
            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Inizio Caricamento Serie utente corrente"))
            series = FacadeFactory.Instance.DocumentSeriesFacade.GetSeries(DocumentSeriesContainerRightPositions.Preview)
            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Trovati {0} serie", If(series Is Nothing, 0, series.Count)))

            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Inizio Caricamento Settori utente corrente"))
            roles = FacadeFactory.Instance.RoleFacade.GetUserRoles(DSWEnvironment.DocumentSeries, 1, Nothing)
            FileLogger.Debug(FacadeFactory.Instance.DocumentSeriesFacade.LoggerName, String.Format("Trovati {0} settori", If(roles Is Nothing, 0, roles.Count)))
        End If

        If Not roles.IsNullOrEmpty() Then
            SecurityRoleFullIncrementalPathIn = roles.Select(Function(r) r.FullIncrementalPath).ToList()
            FileLogger.Debug(DocumentSeriesFacade.LoggerName, String.Format("SecurityRoleFullIncrementalPathIn = {0}", If(SecurityRoleFullIncrementalPathIn Is Nothing, String.Empty, SecurityRoleFullIncrementalPathIn.Count.ToString())))

        End If

        If Not series.IsNullOrEmpty() Then
            SecurityIdDocumentSeriesIn = series.Where(Function(s) s IsNot Nothing).Select(Function(s) s.Id).ToList()
            FileLogger.Debug(DocumentSeriesFacade.LoggerName, String.Format("SecurityIdDocumentSeriesIn = {0}", If(SecurityIdDocumentSeriesIn Is Nothing, String.Empty, SecurityIdDocumentSeriesIn.Count.ToString())))
        End If

        SecurityLinkTypeIn = New List(Of DocumentSeriesItemRoleLinkType) From {DocumentSeriesItemRoleLinkType.Owner, DocumentSeriesItemRoleLinkType.Authorization}
    End Sub
    Private Sub ResetContentSearchCascading()
        _biblosResultDictionary = Nothing
        _biblosResultDictionary = Nothing
        IdMainIn = Nothing
    End Sub

    Protected Overrides Sub DecorateCriteria(ByRef criteria As ICriteria)
        If (OnlyActive) Then
            criteria.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Active))
        End If
        If EnableSecurity Then
            ' Insieme delle verifiche che mi danno diritto di visualizzazione all'Item
            Dim disj As New Disjunction()

            ' Fail-safe.
            disj.Add(Expression.Sql("1=0"))

            If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesIsMineRightEnabled Then
                ' Inserita da me. Valido anche per Draft.
                If String.IsNullOrEmpty(ImpersonatingUser) Then
                    disj.Add(Restrictions.Eq("DSI.RegistrationUser", DocSuiteContext.Current.User.FullUserName))
                Else
                    ' Qualora chiami da un servizio esterno.
                    disj.Add(Restrictions.Eq("DSI.RegistrationUser", ImpersonatingUser))
                End If
            End If

            ' La serie documentale è abilitata alla pubblicazione E l'item si trova in stato di pubblicazione.
            criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
            Dim conjIsPublished As New Conjunction()
            conjIsPublished.Add(Restrictions.Eq("DS.PublicationEnabled", True))
            conjIsPublished.Add(Me.GetIsPublishedConjunction())
            disj.Add(conjIsPublished)

            ' Si trova in un contenitore in cui ho diritto di visualizzazione.
            If Not SecurityIdDocumentSeriesIn.IsNullOrEmpty() Then
                criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")

                Dim conj As New Conjunction()
                conj.Add(IsNotDraftDisjunction())
                conj.Add(Restrictions.In("DS.Id", SecurityIdDocumentSeriesIn.ToArray()))

                disj.Add(conj)
            End If

            ' Collegamento di una determinata tipologia ad almeno un settore.
            If Not SecurityRoleFullIncrementalPathIn.IsNullOrEmpty() Then
                ' I settori in Authorize danno diritto solo se in stato Active
                Dim dcAuthorize As DetachedCriteria = GetDetachedHasRolesByType(SecurityRoleFullIncrementalPathIn, SecurityLinkTypeIn)
                Dim conjAuthorize As New Conjunction()
                conjAuthorize.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Active))
                conjAuthorize.Add(Subqueries.Exists(dcAuthorize))
                disj.Add(conjAuthorize)

                ' I settori in Owner danno diritto anche se in stato Draft
                Dim ownerLinkTypeIn As New List(Of DocumentSeriesItemRoleLinkType) From {DocumentSeriesItemRoleLinkType.Owner}
                Dim dcOwner As DetachedCriteria = GetDetachedHasRolesByType(SecurityRoleFullIncrementalPathIn, ownerLinkTypeIn)
                Dim conjOwner As New Conjunction()
                conjOwner.Add(Restrictions.Eq("DSI.Status", DocumentSeriesItemStatus.Draft))
                conjOwner.Add(Subqueries.Exists(dcOwner))
                disj.Add(conjOwner)
            End If

            If Not ViewDraftAssociatedResolution Then
                GetDetachedDraftNotAssociatedResolution(criteria)
            End If

            criteria.Add(disj)
        End If

        If (FindByConstraints.HasValue AndAlso FindByConstraints.Value) Then
            If (String.IsNullOrEmpty(Constraint)) Then
                criteria.Add(Restrictions.Or(Restrictions.IsNull("DSI.ConstraintValue"), Restrictions.Eq("DSI.ConstraintValue", String.Empty)))
            Else
                criteria.Add(Restrictions.Eq("DSI.ConstraintValue", Constraint))
            End If
        End If

        MyBase.DecorateCriteria(criteria)
    End Sub
    Private Function GetDetachedHasRolesByType(fullIncrementalPaths As IEnumerable(Of String), linkTypes As IEnumerable(Of DocumentSeriesItemRoleLinkType)) As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of DocumentSeriesItemRole)("DSIR")
        dc.Add(Restrictions.EqProperty("DSIR.Item.Id", "DSI.Id"))
        dc.Add(Restrictions.In("DSIR.LinkType", linkTypes.ToArray()))
        dc.CreateAlias("DSIR.Role", "R", JoinType.InnerJoin)
        Dim disj As New Disjunction()
        fullIncrementalPaths.ToList().ForEach(Sub(f) disj.Add(Restrictions.Like("R.FullIncrementalPath", f, MatchMode.Start)))
        dc.Add(disj)
        dc.SetProjection(Projections.Constant(True))
        dc.SetMaxResults(1)

        Return dc
    End Function

    Private Sub SetProjectionHeaders(ByRef criteria As ICriteria)
        Dim proj As ProjectionList = Projections.ProjectionList()

        Dim belle As New List(Of Type)() From {GetType(DocumentSeriesItemStatus), GetType(Integer), GetType(Integer?), GetType(Boolean), GetType(Boolean?), GetType(String), GetType(Guid), GetType(Guid?), GetType(Date), GetType(Date?), GetType(DateTime), GetType(DateTime?), GetType(DateTimeOffset), GetType(DateTimeOffset?)}
        For Each info As PropertyInfo In GetType(DocumentSeriesItem).GetProperties()
            If belle.Any(Function(t) t = info.PropertyType) Then
                If Not info.Name.Eq("UniqueId") Then
                    proj.Add(Projections.Property("DSI." & info.Name), info.Name)
                End If
            End If
        Next
        criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
        proj.Add(Projections.Property("DS.Id"), "IdDocumentSeries")

        If IncludeSubsections Then
            criteria.CreateAliasIfNotExists("DSI.DocumentSeriesSubsection", "DSS", JoinType.LeftOuterJoin)
            proj.Add(Projections.Property("DSS.Id"), "IdDocumentSeriesSubsection")
        End If

        criteria.CreateAliasIfNotExists("DS.Container", "DISCO", JoinType.InnerJoin)
        proj.Add(Projections.Property("DISCO.Id"), "IdContainer")
        proj.Add(Projections.Property("DISCO.Name"), "ContainerName")

        criteria.CreateAliasIfNotExists("DSI.Location", "L", JoinType.InnerJoin)
        proj.Add(Projections.Property("L.Id"), "IdLocation")

        criteria.CreateAliasIfNotExists("DSI.LocationAnnexed", "LA", JoinType.LeftOuterJoin)
        proj.Add(Projections.Property("LA.Id"), "IdLocationAnnexed")

        If LastModifiedSortingView.HasValue AndAlso LastModifiedSortingView.Value Then
            proj.Add(Projections.Conditional(
            Restrictions.Or(Restrictions.GtProperty("PublishingDate", "LastChangedDate"), Restrictions.IsNull("LastChangedDate")),
            Projections.Cast(NHibernateUtil.DateTimeOffset, Projections.Property("PublishingDate")), Projections.Property("LastChangedDate")))
        End If

        Dim distinctProj As IProjection = Projections.Distinct(proj)
        criteria.SetProjection(distinctProj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)))
    End Sub

    Private Function GetDetachedAvailableRowCount() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of DocumentSeriesItem)("AvailableRowCount")
        dc.Add(Restrictions.EqProperty("AvailableRowCount.DocumentSeries.Id", "DS.Id"))
        Dim canceled As DocumentSeriesItemStatus() = New DocumentSeriesItemStatus() {DocumentSeriesItemStatus.NotActive, DocumentSeriesItemStatus.Canceled}
        dc.Add(Restrictions.Not(Restrictions.In("AvailableRowCount.Status", canceled)))
        dc.SetProjection(Projections.RowCount())

        Return dc
    End Function

    Private Function GetDetachedDraftedRowCount() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of DocumentSeriesItem)("DraftedRowCount")
        dc.Add(Restrictions.EqProperty("DraftedRowCount.DocumentSeries.Id", "DS.Id"))
        dc.Add(Restrictions.Eq("DraftedRowCount.Status", DocumentSeriesItemStatus.Draft))
        dc.SetProjection(Projections.RowCount())

        Return dc
    End Function

    Public Sub SetProjectionLogSummary(ByRef criteria As ICriteria)
        Dim proj As ProjectionList = Projections.ProjectionList()

        criteria.CreateAliasIfNotExists("DSI.DocumentSeries", "DS")
        proj.Add(Projections.GroupProperty("DS.Id"), "Id")
        proj.Add(Projections.GroupProperty("DS.Name"), "Name")

        proj.Add(Projections.SubQuery(GetDetachedAvailableRowCount()), "Total")
        proj.Add(Projections.SubQuery(GetDetachedDraftedRowCount()), "Drafted")

        criteria.CreateAliasIfNotExists("DSI.Logs", "DSIL")
        proj.Add(Projections.Sum(Projections.Conditional(Restrictions.Eq("DSIL.LogType", DocumentSeriesItemLogType.Insert), Projections.Constant(1), Projections.Constant(0))), "Added")
        proj.Add(Projections.Sum(Projections.Conditional(Restrictions.Eq("DSIL.LogType", DocumentSeriesItemLogType.Edit), Projections.Constant(1), Projections.Constant(0))), "Edits")
        proj.Add(Projections.Sum(Projections.Conditional(Restrictions.Eq("DSIL.LogType", DocumentSeriesItemLogType.Publish), Projections.Constant(1), Projections.Constant(0))), "Published")
        proj.Add(Projections.Sum(Projections.Conditional(Restrictions.Eq("DSIL.LogType", DocumentSeriesItemLogType.Retire), Projections.Constant(1), Projections.Constant(0))), "Retired")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of DocumentSeriesLogSummaryDTO))
    End Sub

    Private Function MergeResults(dto As DocumentSeriesItemDTO(Of BiblosDocumentInfo), fetchMode As BiblosFetchMode) As DocumentSeriesItemDTO(Of BiblosDocumentInfo)
        Dim merged As DocumentSeriesItemDTO(Of BiblosDocumentInfo) = dto
        If BiblosResultDictionary IsNot Nothing Then
            If BiblosResultDictionary.ContainsKey(dto.IdMain) Then
                Select Case fetchMode
                    Case BiblosFetchMode.AttributesOnly
                        merged.Attributes = BiblosResultDictionary(dto.IdMain).Attributes
                    Case Else
                        merged.Attributes = BiblosResultDictionary(dto.IdMain).Attributes
                        merged.MainDocuments = BiblosResultDictionary(dto.IdMain).Documents.Cast(Of BiblosDocumentInfo).ToList()
                        If dto.IdLocationAnnexed > 0 Then
                            merged.Annexed = DocumentSeriesItemFacade.GetAnnexedDocuments(dto)
                        End If
                End Select
                Return merged
            End If
            Return Nothing
        End If

        ' Qualora non abbia a disposizione un dizionario con i risultati di ricerca per metadati.
        Dim chain As BiblosChainInfo = DocumentSeriesItemFacade.GetMainChainInfo(dto)
        Select Case fetchMode
            Case BiblosFetchMode.AttributesOnly
                merged.Attributes = chain.Attributes
            Case Else
                merged.Attributes = chain.Attributes
                merged.MainDocuments = chain.Documents.Cast(Of BiblosDocumentInfo).ToList()
                If dto.IdLocationAnnexed > 0 Then
                    merged.Annexed = DocumentSeriesItemFacade.GetAnnexedDocuments(dto)
                End If
        End Select
        Return merged
    End Function
    Private Function MergeResults(dto As DocumentSeriesItemDTO(Of BiblosDocumentInfo)) As DocumentSeriesItemDTO(Of BiblosDocumentInfo)
        Return MergeResults(dto, BiblosFetchMode.Full)
    End Function

    Public Overrides Function Count() As Integer
        Return MyBase.Count()
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        SetProjectionHeaders(criteria)
        AttachSortExpressions(criteria)
        Dim results As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
        results = criteria.List(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))()
        Return results
    End Function

    Public Function DoSearchCommon() As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
        Return DirectCast(DoSearchHeader(), IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)))
    End Function
    Public Function DoSearchFull() As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
        Dim result As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)) = DoSearchCommon()
        Dim merged As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)) = result.Select(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))(Function(dto) MergeResults(dto)).ToList()
        Return merged
    End Function
    Public Function DoSearchAttributesOnly() As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))
        Dim result As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)) = DoSearchCommon()
        Dim merged As IList(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo)) = result.Select(Of DocumentSeriesItemDTO(Of BiblosDocumentInfo))(Function(dto) MergeResults(dto, BiblosFetchMode.AttributesOnly)).ToList()
        Return merged
    End Function

    Public Function DoSearchConstraints() As IList(Of String)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        criteria.SetProjection(Projections.GroupProperty("ConstraintValue"))
        criteria.AddOrder(Order.Asc("ConstraintValue"))
        Dim results As IList(Of String) = criteria.List(Of String)
        Return results
    End Function

    Public Function DoSearchLogSummary() As IList(Of DocumentSeriesLogSummaryDTO)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        SetProjectionLogSummary(criteria)
        criteria.AddOrder(Order.Asc("Name")).AddOrder(Order.Asc("Id"))
        Return criteria.List(Of DocumentSeriesLogSummaryDTO)()
    End Function

    ''' <summary>
    ''' Crea una subquery per recuperare le sole serie documentali che non sono già associate
    ''' ad altri atti.
    ''' </summary>
    Private Sub GetDetachedDraftNotAssociatedResolution(ByRef criteria As ICriteria)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ResolutionDocumentSeriesItem)("RDSI")
        dc.SetProjection(Projections.Property("RDSI.IdDocumentSeriesItem"))
        criteria.Add(Subqueries.PropertyNotIn("DSI.Id", dc))
    End Sub

End Class
