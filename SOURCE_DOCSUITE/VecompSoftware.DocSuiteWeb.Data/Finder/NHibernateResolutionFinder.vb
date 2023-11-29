Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports VecompSoftware.Helpers.NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Transformer
Imports System.ComponentModel
Imports NHibernate.Transform
Imports NHibernate.SqlCommand
Imports System.Reflection
Imports System.Linq

<Serializable(), DataObject()>
Public Class NHibernateResolutionFinder
    Inherits NHibernateBaseFinder(Of Resolution, ResolutionHeader)

#Region " Constructors "

    Public Sub New(ByVal DbName As String, Optional EnabledFilter As Boolean = True)
        EagerLog = True
        EnablePaging = True
        Configuration = String.Empty
        SessionFactoryName = DbName
        If EnabledFilter Then
            NHibernateSession.EnableFilter("LogUser").SetParameter("User", DocSuiteContext.Current.User.FullUserName)
        End If
        _EnableStatus = True
    End Sub

#End Region

#Region " Fields "

    Protected _year As Short?
    Protected _provNumber As Integer?
    Protected _idResolution As Integer?
    Protected _resolutionObjectSearch As ObjectSearchType
    Protected _idBidType As Integer?

    'ASL-TO2
    Protected _immediatelyExecutive As Boolean
    Protected _proposerDateFrom As Date?
    Protected _proposerDateTo As Date?
    Protected _adoptionDateFrom As Date?
    Protected _adoptionDateTo As Date?
    Protected _ocSupervisoryBoard As Boolean
    Protected _ocRegion As Boolean
    Protected _ocManagement As Boolean
    Protected _ocCorteConti As Boolean
    Protected _ocOther As Boolean
    Protected _statusCancel As Boolean
    Protected _onlyStatusCancel As Boolean
    Protected _supervisoryBoardProtocolLink As String


    'Configuration
    Protected _print As Boolean
    Protected _webService As Boolean

    'Top
    Protected _topMaxRecords As Integer = 0

    ' Web Publish
    Protected _webPublicationDateFrom As Date?
    Protected _webPublicationDateTo As Date?
    Protected _webRevokeDateFrom As Date?
    Protected _webRevokeDateTo As Date?
    Protected _onlyPublicated As Nullable(Of Boolean)
    Protected _checkPublication As Nullable(Of Boolean)

    ' AUSL
    Protected _isChecked As Nullable(Of Boolean)

    ' Resolution Type
    Protected _resolutionType As ResolutionType
    Protected _hasDocumentalSeriesDraft As Boolean
    Protected _isDocumentalSeriesDraftToComplete As Boolean

#End Region

#Region " Properties "

    Public Property EagerLog() As Boolean

    Public Property EnablePaging As Boolean

    Public Property Roles As String

    Public Property Delibera As Boolean

    Public Property Determina As Boolean

    Public Property Year As String
        Get
            Return _year.ToString()
        End Get
        Set(ByVal value As String)
            _year = ConvertToShort(value)
        End Set
    End Property

    Public Property Number As Integer?

    ''' <summary> Numero da escludere </summary>
    ''' <remarks>Nato per escludere i numeri 0 di test</remarks>
    Public Property NotNumber As Integer?

    ''' <summary> Filtra ServiceNumber per le Determinazioni, Number per le Delibere </summary>
    Public Property AuslPcNumber As String

    Public Property InclusiveNumber As String

    Public Property InclusiveNumbers As ICollection

    Public Property InclusiveNumberFrom As String

    Public Property InclusiveNumberTo As String

    Public Property RegistrationDateFrom As Date?

    Public Property RegistrationDateTo As Date?

    Public Property ServiceNumber As String

    Public Property ServiceNumberEqual As String

    Public Property ServiceNumberEndsWith As String

    Public Property TryNumber() As String
        Get
            Return _provNumber.ToString()
        End Get
        Set(ByVal value As String)
            _provNumber = ConvertToInteger(value)
        End Set
    End Property

    Public Property IdResolution() As String
        Get
            Return _idResolution.ToString()
        End Get
        Set(ByVal value As String)
            _idResolution = ConvertToInteger(value)
        End Set
    End Property

    Public Property IdBidType() As String
        Get
            Return _idBidType.ToString()
        End Get
        Set(ByVal value As String)
            _idBidType = ConvertToInteger(value)
        End Set
    End Property

    Public Property IdResolutionList As IList(Of Integer)

    Public Property DocumentSeriesItemIdentifier As IList(Of Integer)

    Public Property User As String

    ''' <summary> Elenco degli id contenitori separati da virgola </summary>
    Public Property ContainerIds As String = String.Empty

    Public Property SelectedContainerId As Integer?

    Public Property Proposta As Boolean

    Public Property Adottata As Boolean

    Public Property Pubblicata As Boolean

    Public Property Spedizione As Boolean

    Public Property Ricezione As Boolean

    Public Property Scadenza As Boolean

    Public Property Risposta As Boolean

    Public Property Esecutiva As Boolean

    Public Property StepAttivo As Boolean

    Public Property DateFrom As Date?

    Public Property DateTo As Date?

    Public Property ResolutionObject As String

    Public Property ResolutionObjectSearch() As ObjectSearchType
        Get
            Return _resolutionObjectSearch
        End Get
        Set(ByVal value As ObjectSearchType)
            _resolutionObjectSearch = value
        End Set
    End Property

    Public Property Note As String

    Public Property Recipient As String

    Public Property Proposer As String

    Public Property Assignee As String

    Public Property Manager As String

    Public Property InteropRecipients As String

    Public Property InteropProposers As String

    Public Property InteropAssignees As String

    Public Property InteropManagers As String

    Public Property InteropRecipientsChild As Boolean


    Public Property ControllerOpinion As String

    Public Property IdControllerStatus As String

    Public Property Categories As String

    Public Property IncludeChildCategories As Boolean

    Public Property Configuration As String

    Public Property HasServiceNumber As Boolean

    Public Property HasNumber As Boolean

    Public Property HasInclusiveNumber As Boolean

    Public Property IsPrint As Boolean

    Public Property IsWebService As Boolean

    Public Property DescriptionStep As String

    Public Property DescriptionSteps As List(Of String) = New List(Of String)

    Public Property FieldDateNames As List(Of String) = New List(Of String)

    Public Property WorkFlowStep As Short?

    Public Property EnableStatus As Boolean

    Public Property IdStatus As Short?

    Public Overridable Property WebPublicationDateFrom() As Date?
        Get
            Return _webPublicationDateFrom
        End Get
        Set(ByVal value As Date?)
            _webPublicationDateFrom = value
        End Set
    End Property

    Public Overridable Property WebPublicationDateTo() As Date?
        Get
            Return _webPublicationDateTo
        End Get
        Set(ByVal value As Date?)
            _webPublicationDateTo = value
        End Set
    End Property

    ''' <summary>Stato della pubblicazione</summary>
    ''' <remarks>Valido solo se <see>DocSuiteContext.Current.ResolutionEnv.WebPublishEnabled</see> è abilitato.</remarks>
    Public Overridable Property WebState As Resolution.WebStateEnum?

    ''' <summary>
    ''' Indica se includere solo <see>ResolutionHeader</see> con <see>FileResolution.IdFrontalinoRitiro</see> valorizzato
    ''' </summary>
    Public Overridable Property HasFrontalinoRitiro() As Boolean?

    Public Overridable Property WebRevokeDateFrom() As Date?
        Get
            Return _webRevokeDateFrom
        End Get
        Set(ByVal value As Date?)
            _webRevokeDateFrom = value
        End Set
    End Property

    Public Overridable Property WebRevokeDateTo() As Date?
        Get
            Return _webRevokeDateTo
        End Get
        Set(ByVal value As Date?)
            _webRevokeDateTo = value
        End Set
    End Property

    Public Overridable Property CheckPublication() As Nullable(Of Boolean)
        Get
            Return _checkPublication
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _checkPublication = value
        End Set
    End Property

    Public Overridable Property IsChecked() As Nullable(Of Boolean)
        Get
            Return _isChecked
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _isChecked = value
        End Set
    End Property

    Public Overridable Property OnlyPublicated() As Nullable(Of Boolean)
        Get
            Return _onlyPublicated
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _onlyPublicated = value
        End Set
    End Property

    ''' <summary> Consente la ricerca delle proposte che sono state ritenute non conformi. </summary>
    ''' <remarks> AUSL-PC </remarks>
    Public Overridable Property HasCancelMotivation() As Boolean?


    ''' <summary> Consente la ricerca degli atti con bozze di serie documentale da completare. </summary>
    Public Property HasDocumentalSeriesDraft() As Boolean
        Get
            Return _hasDocumentalSeriesDraft
        End Get
        Set(ByVal value As Boolean)
            _hasDocumentalSeriesDraft = value
        End Set
    End Property

    Public Overridable Property ResolutionType As ResolutionType
        Get
            Return _resolutionType
        End Get
        Set(value As ResolutionType)
            _resolutionType = value
        End Set
    End Property

    Public Property TemplateSpecifications As IList(Of ResolutionJournalTemplateSpecification)

    Public IsAdopted As Boolean?
    Public IsEffective As Boolean?

    Public Property PrivacyPublication As String

    Public Property HasPublishingDate As Boolean?

    Public Property ViewAllExecutive As Boolean

    Public Property WorkflowStepsExcluded As ICollection(Of Tuple(Of String, Integer))

    Public Property WorkflowStepsForceVisibility As ICollection(Of Tuple(Of String, Integer))

#End Region

#Region "Properties: ASL-TO2"
    Public Property ImmediatelyExecutive() As Boolean
        Get
            Return _immediatelyExecutive
        End Get
        Set(ByVal value As Boolean)
            _immediatelyExecutive = value
        End Set
    End Property
    Public Property ProposerDateFrom() As Date?
        Get
            Return _proposerDateFrom
        End Get
        Set(ByVal value As Date?)
            _proposerDateFrom = value
        End Set
    End Property
    Public Property ProposerDateTo() As Date?
        Get
            Return _proposerDateTo
        End Get
        Set(ByVal value As Date?)
            _proposerDateTo = value
        End Set
    End Property
    Public Property AdoptionDateFrom() As Date?
        Get
            Return _adoptionDateFrom
        End Get
        Set(ByVal value As Date?)
            _adoptionDateFrom = value
        End Set
    End Property
    Public Property AdoptionDateTo() As Date?
        Get
            Return _adoptionDateTo
        End Get
        Set(ByVal value As Date?)
            _adoptionDateTo = value
        End Set
    End Property
    Public Property OCSupervisoryBoard() As Boolean
        Get
            Return _ocSupervisoryBoard
        End Get
        Set(ByVal value As Boolean)
            _ocSupervisoryBoard = value
        End Set
    End Property
    Public Property OCRegion() As Boolean
        Get
            Return _ocRegion
        End Get
        Set(ByVal value As Boolean)
            _ocRegion = value
        End Set
    End Property
    Public Property OCManagement() As Boolean
        Get
            Return _ocManagement
        End Get
        Set(ByVal value As Boolean)
            _ocManagement = value
        End Set
    End Property
    Public Property OCCorteConti() As Boolean
        Get
            Return _ocCorteConti
        End Get
        Set(ByVal value As Boolean)
            _ocCorteConti = value
        End Set
    End Property
    Public Property OCOther() As Boolean
        Get
            Return _ocOther
        End Get
        Set(ByVal value As Boolean)
            _ocOther = value
        End Set
    End Property
    Public Property StatusCancel() As Boolean
        Get
            Return _statusCancel
        End Get
        Set(ByVal value As Boolean)
            _statusCancel = value
        End Set
    End Property
    Public Property OnlyStatusCancel() As Boolean
        Get
            Return _onlyStatusCancel
        End Get
        Set(ByVal value As Boolean)
            _onlyStatusCancel = value
        End Set
    End Property
    Public Property SupervisoryBoardProtocolLink() As String
        Get
            Return _supervisoryBoardProtocolLink
        End Get
        Set(ByVal value As String)
            _supervisoryBoardProtocolLink = value
        End Set
    End Property
    Public Property UserTakeCharge As String
#End Region

#Region "Criteria"

    ''' <summary>
    ''' Imposta al criterio specificato le restrizioni a partire dalle specifiche di un ResolutionJournalTemplate.
    ''' </summary>
    ''' <param name="criteria">Criterio di cui impostare le restrizioni.</param>
    ''' <remarks>Colonne in AND, record in OR.</remarks>
    Private Sub DecorateTemplateSpecifications(ByRef criteria As ICriteria)
        If TemplateSpecifications Is Nothing Then
            Exit Sub
        End If

        Dim disj As New Disjunction()
        disj.Add(Expression.Sql("1=0")) ' fail-safe
        For Each spec As ResolutionJournalTemplateSpecification In TemplateSpecifications
            Dim conj As New Conjunction()
            If spec.Container IsNot Nothing Then
                conj.Add(Restrictions.Eq("R.Container", spec.Container))
            End If
            If spec.ReslType IsNot Nothing Then
                conj.Add(Restrictions.Eq("R.Type", spec.ReslType))
            End If
            disj.Add(conj)
        Next
        criteria.Add(disj)
    End Sub

    Private Sub DecorateController(ByRef criteria As ICriteria)
        'ControllerOpinion
        If Not String.IsNullOrEmpty(ControllerOpinion) Then
            criteria.Add(Expression.Like("R.ControllerOpinion", "%" & _ControllerOpinion & "%"))
        End If


        'ControllerStatus
        If Not String.IsNullOrEmpty(IdControllerStatus) Then
            criteria.Add(Restrictions.Eq("ControllerStatus.Id", _IdControllerStatus))
        End If
    End Sub

    Private Sub DecorateOC(ByRef criteria As ICriteria)
        'SupervisoryBoardProtocolLink
        If Not String.IsNullOrEmpty(SupervisoryBoardProtocolLink) Then
            criteria.Add(Restrictions.Eq("R.SupervisoryBoardProtocolLink", _supervisoryBoardProtocolLink))
        End If
        'Organi di controllo
        If OCSupervisoryBoard Then
            criteria.Add(Restrictions.Eq("R.OCSupervisoryBoard", OCSupervisoryBoard))
        End If
        If OCRegion Then
            criteria.Add(Restrictions.Eq("R.OCRegion", OCRegion))
        End If
        If OCManagement Then
            criteria.Add(Restrictions.Eq("R.OCManagement", OCManagement))
        End If
        If OCCorteConti Then
            criteria.Add(Restrictions.Eq("R.OCCorteConti", OCCorteConti))
        End If
        If OCOther Then
            criteria.Add(Restrictions.Eq("R.OCOther", OCOther))
        End If
    End Sub

    Private Sub DecorateSteps(ByRef criteria As ICriteria)
        'StepAdozione
        If Not String.IsNullOrEmpty(DescriptionStep) Then
            AttachWorkflowStepExpression(criteria, DescriptionStep)
        End If
        If WorkFlowStep.HasValue Then
            AttachWorkflowStepExpression(criteria, WorkFlowStep)
        End If

        If DescriptionSteps.Any() Then
            AttachWorkflowStepsExpression(criteria, DescriptionSteps)
        End If

        If FieldDateNames.Any() Then
            For Each fieldDate As String In FieldDateNames
                If DateFrom.HasValue Then
                    criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat(fieldDate, DateFrom.Value)))
                End If
                If DateTo.HasValue Then
                    criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat(fieldDate, DateTo.Value)))
                End If
            Next
        End If

        'Immediatamente esecutiva
        If ImmediatelyExecutive Then
            criteria.Add(Restrictions.Eq("R.ImmediatelyExecutive", ImmediatelyExecutive))
        End If

        'Data Proposta Da
        If ProposerDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("ProposeDate", ProposerDateFrom.Value)))
        End If
        'Data Proposta a
        If ProposerDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("ProposeDate", ProposerDateTo.Value)))
        End If

        'Data Adozione Da
        If AdoptionDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("AdoptionDate", AdoptionDateFrom.Value)))
        End If
        'Data Adozione a
        If AdoptionDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("AdoptionDate", AdoptionDateTo.Value)))
        End If

    End Sub

    Private Sub DecorateStatus(ByRef criteria As ICriteria)
        If IdStatus.HasValue Then
            criteria.Add(Restrictions.Eq("Status.Id", _IdStatus))
        ElseIf EnableStatus Then
            Select Case Configuration.ToUpper()
                Case "AUSL-PC"
                    Select Case True
                        Case OnlyStatusCancel
                            AttachOnlyStatusCancelExpression(criteria)

                        Case StatusCancel
                            AttachStatusExpression(criteria)

                        Case Else
                            criteria.Add(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)))
                    End Select
                Case "ASL3-TO"
                    Select Case True
                        Case OnlyStatusCancel
                            AttachOnlyStatusCancelExpression(criteria)
                        Case StatusCancel
                            AttachStatusExpression(criteria)
                        Case Else
                            criteria.Add(Restrictions.Or(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)), Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Sospeso))))
                    End Select
                Case Else
                    AttachStatusExpression(criteria)
            End Select
        End If
    End Sub

    Private Sub DecorateCategories(ByRef criteria As ICriteria)
        'Classificazione
        If Not String.IsNullOrEmpty(Categories) Then
            criteria.CreateAlias("R.CategoryAPI", "CategoryAPI")

            Dim rootDisjunction As Disjunction = Restrictions.Disjunction()
            If IncludeChildCategories Then
                Dim dis As Disjunction = Restrictions.Disjunction()
                dis.Add(Restrictions.Eq("CategoryAPI.FullIncrementalPath", Categories))
                dis.Add(Restrictions.Like("CategoryAPI.FullIncrementalPath", $"{Categories}|%"))
                rootDisjunction.Add(dis)
            Else
                rootDisjunction.Add(Restrictions.Eq("CategoryAPI.FullIncrementalPath", Categories))
            End If
            criteria.Add(rootDisjunction)
        End If
    End Sub
    Private Sub DecorateRecipients(ByRef criteria As ICriteria)
        'AlternativeRecipient
        If Not String.IsNullOrEmpty(Recipient) Then
            Dim disj As Disjunction = Expression.Disjunction
            Dim s As String = "%" & _Recipient.Replace(" ", "%") & "%"

            'Controllo il campo AlternativeRecipient
            disj.Add(Expression.Like("R.AlternativeRecipient", s))

            'Controllo i Contatti
            If DocSuiteContext.Current.ResolutionEnv.IsInteropEnabled Then
                criteria.SetFetchMode("Destinatari", FetchMode.Eager)

                Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Destinatari")

                Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Dest")
                dcCon.Add(Restrictions.EqProperty("Destinatari.Id.IdContact", "Dest.Id"))
                dcCon.Add(Expression.Like("Dest.Description", s))
                dcCon.SetProjection(Projections.Id())

                reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
                reCon.Add(Restrictions.Eq("Destinatari.Id.ComunicationType", "D"))
                reCon.SetProjection(Projections.Property("Id.IdResolution"))

                disj.Add(Subqueries.PropertyIn("Id", reCon))
            End If

            criteria.Add(disj)
        End If

        'AlternativeProposer
        If Not String.IsNullOrEmpty(Proposer) Then
            Dim disj As Disjunction = Expression.Disjunction
            Dim s As String = "%" & _Proposer.Replace(" ", "%") & "%"

            'Controllo il campo AlternativeProposer
            disj.Add(Expression.Like("R.AlternativeProposer", s))

            'Controllo i Contatti
            If DocSuiteContext.Current.ResolutionEnv.IsInteropEnabled Then
                Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Proponenti")

                Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Prop")
                dcCon.Add(Restrictions.EqProperty("Proponenti.Id.IdContact", "Prop.Id"))
                dcCon.Add(Expression.Like("Prop.Description", s))
                dcCon.SetProjection(Projections.Id())

                reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
                reCon.Add(Restrictions.Eq("Proponenti.Id.ComunicationType", "P"))
                reCon.SetProjection(Projections.Property("Id.IdResolution"))

                disj.Add(Subqueries.PropertyIn("Id", reCon))
            End If

            criteria.Add(disj)
        End If

        'AlternativeAssignee
        If Not String.IsNullOrEmpty(Assignee) Then
            Dim disj As Disjunction = Expression.Disjunction
            Dim s As String = "%" & _Assignee.Replace(" ", "%") & "%"

            'Controllo il campo AlternativeAssignee
            disj.Add(Expression.Like("R.AlternativeAssignee", s))

            'Controllo i Contatti
            If DocSuiteContext.Current.ResolutionEnv.IsInteropEnabled Then

                Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Assignee")

                Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Asse")
                dcCon.Add(Restrictions.EqProperty("Assignee.Id.IdContact", "Asse.Id"))
                dcCon.Add(Expression.Like("Asse.Description", s))
                dcCon.SetProjection(Projections.Id())

                reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
                reCon.Add(Restrictions.Eq("Assignee.Id.ComunicationType", "A"))
                reCon.SetProjection(Projections.Property("Id.IdResolution"))

                disj.Add(Subqueries.PropertyIn("Id", reCon))
            End If

            criteria.Add(disj)
        End If

        'AlternativeManager
        If Not String.IsNullOrEmpty(Manager) Then
            Dim disj As Disjunction = Expression.Disjunction
            Dim s As String = "%" & _Manager.Replace(" ", "%") & "%"

            'Controllo il campo AlternativeManager
            disj.Add(Expression.Like("R.AlternativeManager", s))

            'Controllo i Contatti
            If DocSuiteContext.Current.ResolutionEnv.IsInteropEnabled Then

                Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Manager")

                Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Resp")
                dcCon.Add(Restrictions.EqProperty("Manager.Id.IdContact", "Resp.Id"))
                dcCon.Add(Expression.Like("Resp.Description", s))
                dcCon.SetProjection(Projections.Id())

                reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
                reCon.Add(Restrictions.Eq("Manager.Id.ComunicationType", "R"))
                reCon.SetProjection(Projections.Property("Id.IdResolution"))

                disj.Add(Subqueries.PropertyIn("Id", reCon))
            End If

            criteria.Add(disj)
        End If

        'Contatti
        'InteropRecipients
        If Not String.IsNullOrEmpty(InteropRecipients) Then
            'Criteria per i ResolutionContact
            Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Destinatari")

            'Criteri per i Contact
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Dest")
            dcCon.Add(Restrictions.EqProperty("Destinatari.Id.IdContact", "Dest.Id"))

            'Controllo se la ricerca è anche per i figli
            Dim disj As Disjunction = Expression.Disjunction
            disj.Add(Restrictions.Eq("Dest.FullIncrementalPath", _InteropRecipients))
            If InteropRecipientsChild Then
                disj.Add(Expression.Like("Dest.FullIncrementalPath", _InteropRecipients & "|%"))
            End If
            dcCon.Add(disj)

            'Proiezione sull'id dei Contacts
            dcCon.SetProjection(Projections.Id())

            'Subquery sulla ResolutionContact
            reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
            reCon.Add(Restrictions.Eq("Destinatari.Id.ComunicationType", "D"))
            reCon.SetProjection(Projections.Property("Id.IdResolution")) 'proiezione sull'IdResolution

            'Subquery sulla Resolution
            criteria.Add(Subqueries.PropertyIn("Id", reCon))
        End If

        'InteropProposer
        If Not String.IsNullOrEmpty(InteropProposers) Then
            'Criteria per i ResolutionContact
            Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Proponenti")

            'Criteri per i Contact
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Prop")
            dcCon.Add(Restrictions.EqProperty("Proponenti.Id.IdContact", "Prop.Id"))

            'Controllo se la ricerca è anche per i figli
            Dim disj As Disjunction = Expression.Disjunction
            Dim props As String() = _InteropProposers.Split("§"c)
            For Each p As String In props
                disj.Add(Restrictions.Eq("Prop.FullIncrementalPath", p))
                If InteropRecipientsChild Then
                    disj.Add(Expression.Like("Prop.FullIncrementalPath", p & "|%"))
                End If
            Next
            dcCon.Add(disj)

            'Proiezione sull'id dei Contacts
            dcCon.SetProjection(Projections.Id())

            'Subquery sulla ResolutionContact
            reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
            reCon.Add(Restrictions.Eq("Proponenti.Id.ComunicationType", "P"))
            reCon.SetProjection(Projections.Property("Id.IdResolution")) 'proiezione sull'IdResolution

            'Subquery sulla Resolution
            criteria.Add(Subqueries.PropertyIn("Id", reCon))
        End If

        'InteropAssignee
        If Not String.IsNullOrEmpty(InteropAssignees) Then
            'Criteria per i ResolutionContact
            Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Assegnatari")

            'Criteri per i Contact
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Assi")
            dcCon.Add(Restrictions.EqProperty("Assegnatari.Id.IdContact", "Assi.Id"))

            'Controllo se la ricerca è anche per i figli
            Dim disj As Disjunction = Expression.Disjunction
            disj.Add(Restrictions.Eq("Assi.FullIncrementalPath", _InteropAssignees))
            If InteropRecipientsChild Then
                disj.Add(Expression.Like("Assi.FullIncrementalPath", _InteropAssignees & "|%"))
            End If
            dcCon.Add(disj)

            'Proiezione sull'id dei Contacts
            dcCon.SetProjection(Projections.Id())

            'Subquery sulla ResolutionContact
            reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
            reCon.Add(Restrictions.Eq("Assegnatari.Id.ComunicationType", "A"))
            reCon.SetProjection(Projections.Property("Id.IdResolution")) 'proiezione sull'IdResolution

            'Subquery sulla Resolution
            criteria.Add(Subqueries.PropertyIn("Id", reCon))
        End If

        'InteropManager
        If Not String.IsNullOrEmpty(InteropManagers) Then
            'Criteria per i ResolutionContact
            Dim reCon As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "Responsabili")

            'Criteri per i Contact
            Dim dcCon As DetachedCriteria = DetachedCriteria.For(GetType(Contact), "Resp")
            dcCon.Add(Restrictions.EqProperty("Responsabili.Id.IdContact", "Resp.Id"))

            'Controllo se la ricerca è anche per i figli
            Dim disj As Disjunction = Expression.Disjunction
            disj.Add(Restrictions.Eq("Resp.FullIncrementalPath", _InteropManagers))
            If InteropRecipientsChild Then
                disj.Add(Expression.Like("Resp.FullIncrementalPath", _InteropManagers & "|%"))
            End If
            dcCon.Add(disj)

            'Proiezione sull'id dei Contacts
            dcCon.SetProjection(Projections.Id())

            'Subquery sulla ResolutionContact
            reCon.Add(Subqueries.PropertyIn("Id.IdContact", dcCon))
            reCon.Add(Restrictions.Eq("Responsabili.Id.ComunicationType", "R"))
            reCon.SetProjection(Projections.Property("Id.IdResolution")) 'proiezione sull'IdResolution

            'Subquery sulla Resolution
            criteria.Add(Subqueries.PropertyIn("Id", reCon))
        End If
    End Sub

    Private Sub DecorateObject(ByRef criteria As ICriteria)
        'Object
        If Not (String.IsNullOrEmpty(ResolutionObject)) Then
            Dim words As String() = ResolutionObject.Split(" "c)
            Select Case ResolutionObjectSearch
                Case ObjectSearchType.AtLeastWord
                    Dim disju As Disjunction = Expression.Disjunction()
                    For Each word As String In words
                        disju.Add(Expression.Like("R.ResolutionObject", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(disju)
                Case ObjectSearchType.AllWords
                    Dim conju As Conjunction = Expression.Conjunction()
                    For Each word As String In words
                        conju.Add(Expression.Like("R.ResolutionObject", word, MatchMode.Anywhere))
                    Next
                    criteria.Add(conju)
            End Select
        End If
    End Sub

    Private Sub DecorateSecurities(ByRef criteria As ICriteria)
        If String.IsNullOrEmpty(ContainerIds) AndAlso String.IsNullOrEmpty(Roles) Then
            Return
        End If

        Dim disju As Disjunction = New Disjunction
        disju.Add(Expression.Sql("1=0")) ' fail-safe

        Dim containerIntIds As Integer() = Nothing
        Dim roleIntIds As Integer() = Nothing
        'Filtro Container
        If Not (String.IsNullOrEmpty(ContainerIds)) Then
            containerIntIds = ContainerIds.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()
            disju.Add(Restrictions.In("R.Container.Id", containerIntIds))
        End If

        'Filtro Roles
        Dim disjunctionRightsCriteria As Disjunction = New Disjunction()
        If Not String.IsNullOrEmpty(Roles) Then
            Dim conjunctionRoleCriteria As Conjunction = New Conjunction()
            roleIntIds = Roles.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()

            Dim dcRole As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionRole), "RR")
            dcRole.Add(Restrictions.EqProperty("RR.Resolution.Id", "R.Id"))
            dcRole.Add(Restrictions.In("RR.Role.Id", roleIntIds))
            dcRole.SetProjection(Projections.GroupProperty("RR.Resolution.Id"))
            conjunctionRoleCriteria.Add(Subqueries.Exists(dcRole))

            If User IsNot Nothing Then
                Dim dcReslContact As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionContact), "RC")
                dcReslContact.Add(Restrictions.EqProperty("RC.Resolution.Id", "R.Id"))
                dcReslContact.CreateAlias("RC.Contact", "CRContact", JoinType.InnerJoin)
                dcReslContact.Add(Restrictions.EqProperty("RC.Id.IdContact", "CRContact.Id"))
                dcReslContact.Add(Restrictions.In("CRContact.Role.Id", roleIntIds))
                dcReslContact.Add(Restrictions.Eq("RC.Id.ComunicationType", "P"))
                dcReslContact.SetProjection(Projections.Id())
                conjunctionRoleCriteria.Add(Subqueries.Exists(dcReslContact))
                disjunctionRightsCriteria.Add(Restrictions.Eq("ProposeUser", User))
            End If
            disjunctionRightsCriteria.Add(conjunctionRoleCriteria)
        End If

        If Not WorkflowStepsForceVisibility.IsNullOrEmpty() Then
            Dim stepDisj As Disjunction = Restrictions.Disjunction()
            Dim tmpConj As Conjunction
            For Each stepToExcelude As Tuple(Of String, Integer) In WorkflowStepsForceVisibility
                tmpConj = Restrictions.Conjunction()
                tmpConj.Add(Restrictions.Eq("RW.ResStep", Convert.ToInt16(stepToExcelude.Item2)))
                tmpConj.Add(Restrictions.Eq("WorkflowType", stepToExcelude.Item1))
                stepDisj.Add(tmpConj)
            Next
            disjunctionRightsCriteria.Add(stepDisj)
        End If
        disju.Add(disjunctionRightsCriteria)

        If User IsNot Nothing AndAlso String.IsNullOrEmpty(Roles) Then
            disju.Add(Restrictions.Eq("ProposeUser", User))
        End If

        'Includo anche gli atti esecutivi, anche se non ho diritti su contenitori/settori
        If ViewAllExecutive Then
            disju.Add(Restrictions.IsNotNull("R.EffectivenessDate"))
        End If

        criteria.Add(disju)

        If Not WorkflowStepsExcluded.IsNullOrEmpty() Then
            Dim stepDisj As Disjunction
            Dim tmpConj As Conjunction
            For Each stepToExcelude As Tuple(Of String, Integer) In WorkflowStepsExcluded
                stepDisj = Restrictions.Disjunction()
                tmpConj = Restrictions.Conjunction()
                tmpConj.Add(Restrictions.Not(Restrictions.Eq("RW.ResStep", Convert.ToInt16(stepToExcelude.Item2))))
                tmpConj.Add(Restrictions.Eq("WorkflowType", stepToExcelude.Item1))
                stepDisj.Add(tmpConj)
                stepDisj.Add(Restrictions.Not(Restrictions.Eq("WorkflowType", stepToExcelude.Item1)))
                criteria.Add(stepDisj)
            Next
        End If

        If Not String.IsNullOrEmpty(UserTakeCharge) Then
            Dim tmpExistCriteria As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionWorkflowUser), "RWU")
            tmpExistCriteria.Add(Restrictions.EqProperty("RWU.ResolutionWorkflow.Id", "RW.Id"))
            tmpExistCriteria.Add(Restrictions.Eq("RWU.AuthorizationType", AuthorizationRoleType.Responsible))
            tmpExistCriteria.Add(Restrictions.Eq("RWU.Account", UserTakeCharge))
            tmpExistCriteria.SetMaxResults(1).SetProjection(Projections.Constant(1))
            criteria.Add(Subqueries.Exists(tmpExistCriteria))
        End If
    End Sub

    Private Sub DecoratePassi(ByRef criteria As ICriteria)
        'Filtro Passi
        'Creo un disjunction criteria per mettere in OR i passi selezionati
        Dim passoDisju As Disjunction = Restrictions.Disjunction()
        Dim bAdd As Boolean = False

        'Proposta
        If Proposta Then
            'Creo un conjunction criteria per mettere in AND i criteria del passo
            Dim propConju As Conjunction = Restrictions.Conjunction()

            propConju.Add(Restrictions.IsNotNull("R.ProposeDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(propConju, "ProposeDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(propConju, "ProposeDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                propConju.Add(Restrictions.IsNull("R.AdoptionDate"))
            End If

            passoDisju.Add(propConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Adottata
        If Adottata Then
            'Creo un conjunction criteria per mettere in AND i criteria del passo
            Dim adopConju As Conjunction = Restrictions.Conjunction()

            adopConju.Add(Restrictions.IsNotNull("R.AdoptionDate"))


            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(adopConju, "AdoptionDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(adopConju, "AdoptionDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                adopConju.Add(Restrictions.IsNull("R.PublishingDate"))
            End If

            passoDisju.Add(adopConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Pubblicata
        If Pubblicata Then
            'Creo un conjunction criteria per mettere in AND i criteria del passo
            Dim publConju As Conjunction = Restrictions.Conjunction()

            publConju.Add(Restrictions.IsNotNull("R.PublishingDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(publConju, "PublishingDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(publConju, "PublishingDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                publConju.Add(Restrictions.IsNull("R.ResponseDate"))
                publConju.Add(Restrictions.IsNull("R.EffectivenessDate"))
            End If

            passoDisju.Add(publConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Esecutiva
        If Esecutiva Then
            'Creo un conjunction criteria per mettere in AND i criteria del passo
            Dim effeConju As Conjunction = Restrictions.Conjunction()

            effeConju.Add(Restrictions.IsNotNull("EffectivenessDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(effeConju, "EffectivenessDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(effeConju, "EffectivenessDate", _DateTo.Value)
            End If

            passoDisju.Add(effeConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Filtro Organi di Controllo

        'Spedizione
        If Spedizione Then
            'Creo un conjunction criteria per mettere in AND i criteria dell'organo
            Dim spedConju As Conjunction = Restrictions.Conjunction()

            spedConju.Add(Restrictions.IsNotNull("R.WarningDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(spedConju, "WarningDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(spedConju, "WarningDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                spedConju.Add(Restrictions.IsNull("R.ConfirmDate"))
            End If

            passoDisju.Add(spedConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Ricezione
        If Ricezione Then
            'Creo un conjunction criteria per mettere in AND i criteria dell'organo
            Dim riceConju As Conjunction = Restrictions.Conjunction()

            riceConju.Add(Restrictions.IsNotNull("R.ConfirmDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(riceConju, "ConfirmDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(riceConju, "ConfirmDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                riceConju.Add(Restrictions.IsNull("R.ResponseDate"))
            End If

            passoDisju.Add(riceConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Scadenza
        If Scadenza Then
            'Creo un conjunction criteria per mettere in AND i criteria dell'organo
            Dim scadConju As Conjunction = Restrictions.Conjunction()

            scadConju.Add(Restrictions.IsNotNull("R.WaitDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(scadConju, "WaitDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(scadConju, "WaitDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                scadConju.Add(Restrictions.IsNull("R.ResponseDate"))
            End If

            passoDisju.Add(scadConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        'Risposta
        If Risposta Then
            'Creo un conjunction criteria per mettere in AND i criteria dell'organo
            Dim rispConju As Conjunction = Restrictions.Conjunction()

            rispConju.Add(Restrictions.IsNotNull("R.ResponseDate"))

            'Controllo sulle date
            If _DateFrom.HasValue Then
                CreateDateGeExpression(rispConju, "ResponseDate", _DateFrom.Value)
            End If
            If _DateTo.HasValue Then
                CreateDateLeExpression(rispConju, "ResponseDate", _DateTo.Value)
            End If

            'Controllo che non sia attivo lo step successivo
            If StepAttivo Then
                rispConju.Add(Restrictions.IsNull("R.EffectivenessDate"))
            End If

            passoDisju.Add(rispConju) 'Aggiungo il conjunction criteria al disjunction
            bAdd = True
        End If

        If bAdd Then
            criteria.Add(passoDisju) 'Fine Disjunction organi di controllo
        End If

    End Sub

    Private Sub DecorateRegistrationDate(ByRef criteria As ICriteria)
        If RegistrationDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("RW.RegistrationDate", New DateTimeOffset(RegistrationDateFrom)))
        End If

        'Filtro Data di Registrazione fino a
        If RegistrationDateTo.HasValue Then
            criteria.Add(Restrictions.Le("RW.RegistrationDate", New DateTimeOffset(DateAdd(DateInterval.Day, 1, RegistrationDateTo.Value))))
        End If
    End Sub

    Private Sub DecorateNumber(ByRef criteria As ICriteria)
        ' Filtro per Number per AUSL-PC
        If Not (String.IsNullOrEmpty(AuslPcNumber)) Then
            Dim disju As New Disjunction
            If Delibera Then
                ' Metto il filtro per Number
                Dim numberValue As Integer
                If Integer.TryParse(AuslPcNumber, numberValue) Then
                    Dim conju As New Conjunction
                    conju.Add(Restrictions.Eq("R.Number", numberValue))
                    conju.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDelibera))
                    disju.Add(conju)
                End If
            End If
            If Determina Then
                ' Metto il filtro per ServiceNumber
                Dim conju As New Conjunction
                If IsNumeric(AuslPcNumber) Then
                    AuslPcNumber = AuslPcNumber.PadLeft(4, "0"c)
                    conju.Add(Restrictions.Like("R.ServiceNumber", "%" & AuslPcNumber))
                Else
                    conju.Add(Restrictions.Like("R.ServiceNumber", AuslPcNumber & "%"))
                End If
                conju.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDetermina))
                disju.Add(conju)
            End If
            criteria.Add(disju)
        End If

        If Not String.IsNullOrEmpty(InclusiveNumber) Then
            If Not DocSuiteContext.Current.ResolutionEnv.InclusiveNumberWithProposerCodeEnabled Then
                criteria.Add(Restrictions.Eq("R.InclusiveNumber", InclusiveNumber))
            Else
                'nel caso di Città di Torino controllo che l'inclusive number inizi con il numero indicato
                criteria.Add(Restrictions.Like("R.InclusiveNumber", InclusiveNumber, MatchMode.Start))
                If Delibera Then
                    criteria.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDelibera))
                End If
                If Determina Then
                    criteria.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDetermina))
                End If
            End If

        End If

        If (InclusiveNumbers IsNot Nothing) AndAlso (InclusiveNumbers.Count > 0) Then
            criteria.Add(Restrictions.In(Projections.Property("R.InclusiveNumber"), InclusiveNumbers))
        End If

        If Not String.IsNullOrEmpty(InclusiveNumberFrom) Then
            criteria.Add(Restrictions.Ge(Projections.Property("R.InclusiveNumber"), InclusiveNumberFrom))
            Dim inclusiveNumberParts As String() = InclusiveNumberFrom.Split("/"c)
            If inclusiveNumberParts(0) = "0000" Then
                If inclusiveNumberParts.Length = 3 Then
                    criteria.Add(Restrictions.Ge(Projections.Property("R.ServiceNumber"), String.Format("{0}/{1}", inclusiveNumberParts(1), inclusiveNumberParts(2))))
                Else
                    criteria.Add(Restrictions.Ge(Projections.Property("R.Number"), inclusiveNumberParts(1)))
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(InclusiveNumberTo) Then
            criteria.Add(Restrictions.Le(Projections.Property("R.InclusiveNumber"), InclusiveNumberTo))
            Dim inclusiveNumberParts As String() = InclusiveNumberTo.Split("/"c)
            If inclusiveNumberParts(0) = "9999" Then
                If inclusiveNumberParts.Length = 3 Then
                    criteria.Add(Restrictions.Le(Projections.Property("R.ServiceNumber"), String.Format("{0}/{1}", inclusiveNumberParts(1), inclusiveNumberParts(2))))
                Else
                    criteria.Add(Restrictions.Le(Projections.Property("R.Number"), String.Format("{0}", inclusiveNumberParts(1))))
                End If
            End If
        End If

        'Filtro Anno
        If Not (String.IsNullOrEmpty(Year)) Then
            criteria.Add(Restrictions.Eq("R.Year", _year))
        End If

        'Filtro Numero
        If Number.HasValue Then
            criteria.Add(Restrictions.Eq("R.Number", Number.Value))
        End If

        If NotNumber.HasValue Then
            criteria.Add(Restrictions.Not(Restrictions.Eq("R.Number", NotNumber.Value)))
        End If

        'Filtro ServiceNumber con Like
        If Not (String.IsNullOrEmpty(ServiceNumber)) Then
            criteria.Add(Restrictions.Like("R.ServiceNumber", ServiceNumber, MatchMode.Anywhere))
        End If

        'Filtro ServiceNumber con Equal
        If Not (String.IsNullOrEmpty(ServiceNumberEqual)) Then
            criteria.Add(Restrictions.Eq("R.ServiceNumber", ServiceNumberEqual))
        End If

        'Filtro ServiceNumber con EndsWith
        If Not (String.IsNullOrEmpty(ServiceNumberEndsWith)) Then
            criteria.Add(Restrictions.Like("R.ServiceNumber", ServiceNumberEndsWith, MatchMode.End))
        End If
    End Sub

    Private Sub DecorateDeliberaDetermina(ByRef criteria As ICriteria)
        If Delibera Xor Determina Then
            'Filtro Delibera
            If Delibera Then
                criteria.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDelibera))

                If IsChecked.HasValue Then
                    If IsChecked Then
                        criteria.Add(Restrictions.Eq("R.IsChecked", True))
                    Else
                        Dim disju As New Disjunction
                        disju.Add(Restrictions.Eq("R.IsChecked", False))
                        disju.Add(Restrictions.IsNull("R.IsChecked"))
                        criteria.Add(disju)
                    End If
                End If
            End If

            'Filtro Determina
            If Determina Then
                criteria.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDetermina))
            End If
        Else
            If Delibera Then
                If IsChecked.HasValue Then
                    If IsChecked Then
                        Dim disju As New Disjunction
                        If Determina Then disju.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDetermina))

                        Dim conju As New Conjunction
                        conju.Add(Restrictions.Eq("R.IsChecked", True))
                        conju.Add(Restrictions.Eq("R.Type.Id", 1S))

                        disju.Add(conju)
                        criteria.Add(disju)
                    Else
                        Dim disju As New Disjunction
                        If Determina Then
                            disju.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDetermina))
                        End If

                        Dim conju As New Conjunction
                        Dim disjuInner As New Disjunction
                        disjuInner.Add(Restrictions.Eq("R.IsChecked", False))
                        disjuInner.Add(Restrictions.IsNull("R.IsChecked"))
                        conju.Add(disjuInner)
                        conju.Add(Restrictions.Eq("R.Type.Id", ResolutionType.IdentifierDelibera))

                        disju.Add(conju)
                        criteria.Add(disju)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DecorateForWorkflowStatus(ByRef criteria As ICriteria)
        If IsAdopted.HasValue Then
            If IsAdopted Then
                criteria.Add(Restrictions.And(
                             Restrictions.IsNotNull("R.AdoptionDate"),
                             Restrictions.Le("R.AdoptionDate", DateTime.Today())))
            Else
                criteria.Add(Restrictions.Or(
                             Restrictions.IsNull("R.AdoptionDate"),
                             Restrictions.Lt("R.AdoptionDate", DateTime.Today())))
            End If
        End If

        If IsEffective.HasValue Then
            If IsEffective Then
                criteria.Add(Restrictions.And(
                             Restrictions.IsNotNull("R.EffectivenessDate"),
                             Restrictions.Le("R.EffectivenessDate", DateTime.Today())))
            Else
                criteria.Add(Restrictions.Or(
                             Restrictions.IsNull("R.EffectivenessDate"),
                             Restrictions.Gt("R.EffectivenessDate", DateTime.Today())))
            End If
        End If
    End Sub

    Private Sub DecoratePublication(ByRef criteria As ICriteria)
        If Not String.IsNullOrEmpty(PrivacyPublication) Then
            criteria.SetFetchMode("R.File", FetchMode.Eager)
            criteria.CreateAliasIfNotExists("R.File", "File", JoinType.InnerJoin)
            ' TODO: considerare di rimuovere queste stringhe magiche
            Select Case PrivacyPublication
                Case "privacy"
                    criteria.Add(Expression.IsNotNull("File.IdPrivacyPublicationDocument"))
                Case "noprivacy"
                    criteria.Add(Restrictions.IsNull("File.IdPrivacyPublicationDocument"))
            End Select
        End If

        If HasFrontalinoRitiro.HasValue Then
            criteria.SetFetchMode("R.File", FetchMode.Eager)
            criteria.CreateAliasIfNotExists("R.File", "File", JoinType.InnerJoin)
            If HasFrontalinoRitiro.Value Then
                criteria.Add(Expression.IsNotNull("File.IdFrontalinoRitiro"))
            Else
                criteria.Add(Restrictions.IsNull("File.IdFrontalinoRitiro"))
            End If
        End If

        If CheckPublication.HasValue Then
            criteria.Add(Restrictions.Eq("_checkPublication", If(CheckPublication.Value, "1", "0")))
        End If

        If OnlyPublicated.GetValueOrDefault(False) Then
            criteria.Add(Restrictions.Eq("_webState", Resolution.WebStateEnum.Published))
            criteria.Add(Restrictions.IsNull("WebRevokeDate"))
            criteria.Add(Expression.Between("WebPublicationDate", WebPublicationDateFrom.Value.AddDays(-1), WebPublicationDateTo.Value.AddDays(1)))
            criteria.AddOrder(Order.Asc("Number"))
        End If

    End Sub
    Private Sub DecorateWeb(ByRef criteria As ICriteria)

        ' Verifico se WebPublication è attivo
        If WebPublicationDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("WebPublicationDate", WebPublicationDateFrom.Value)))
        End If
        If WebPublicationDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("WebPublicationDate", WebPublicationDateTo.Value)))
        End If
        If WebRevokeDateFrom.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.GreaterThanOrEqualToDateIsoFormat("WebRevokeDate", WebRevokeDateFrom.Value)))
        End If
        If WebRevokeDateTo.HasValue Then
            criteria.Add(Expression.Sql(NHibernateHelper.LessThanOrEqualToDateIsoFormat("WebRevokeDate", WebRevokeDateTo.Value)))
        End If

        If Not Me.WebState.HasValue Then
            Return
        End If

        Select Case Me.WebState.Value
            Case Resolution.WebStateEnum.None
                criteria.Add(Restrictions.Or(Restrictions.IsNull("_webState"), Restrictions.Eq("_webState", Me.WebState)))
            Case Else
                criteria.Add(Restrictions.Eq("_webState", Me.WebState))
        End Select
    End Sub

    Private Sub DecorateResolutionWithDraftSeries(ByRef criteria As ICriteria)

        Dim rdsi As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionDocumentSeriesItem), "RDSI")
        rdsi.CreateAlias("RDSI.Resolution", "RE")
        rdsi.Add(Restrictions.In("RDSI.IdDocumentSeriesItem", DocumentSeriesItemIdentifier.ToArray()))
        rdsi.Add(Expression.IsNotNull("R.PublishingDate"))
        rdsi.SetProjection(Projections.Property("RDSI.Resolution"))
        criteria.Add(Subqueries.PropertyIn("R.Id", rdsi))

    End Sub

    ''' <summary> Crea il criteria per NHibernate </summary>

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "R")

        criteria.CreateAlias("R.Category", "Category", JoinType.LeftOuterJoin)
        criteria.CreateAlias("R.ControllerStatus", "ControllerStatus", JoinType.LeftOuterJoin)
        criteria.CreateAlias("R.Container", "Container", JoinType.LeftOuterJoin)
        criteria.CreateAlias("R.Location", "Location", JoinType.InnerJoin)
        criteria.CreateAlias("R.Type", "Type", JoinType.LeftOuterJoin)

        If IdResolutionList IsNot Nothing AndAlso IdResolutionList.Count > 0 Then
            Dim idList As Integer()
            ReDim idList(IdResolutionList.Count - 1)
            IdResolutionList.CopyTo(idList, 0)
            criteria.Add(Restrictions.In("R.Id", idList))
        End If

        'Filtro Id
        If Not (String.IsNullOrEmpty(IdResolution)) Then
            criteria.Add(Restrictions.Eq("R.Id", _idResolution))
        End If

        ' Filtro per tipo resolution
        If Not ResolutionType Is Nothing Then
            criteria.Add(Restrictions.Eq("R.Type.Id", ResolutionType.Id))
        End If

        'Filtro per BidType
        If Not (String.IsNullOrEmpty(IdBidType)) Then
            criteria.Add(Restrictions.Eq("R.BidType.Id", _idBidType))
        End If

        'Note
        If Not String.IsNullOrEmpty(Note) Then
            criteria.Add(Restrictions.Like("R.Note", "%" & _Note & "%"))
        End If

        'EF 20120210 Verifica del campo DeclineNote dove viene alloggiata la motivazione di annullamento
        If HasCancelMotivation.GetValueOrDefault(False) Then
            criteria.Add(Restrictions.IsNotNull("DeclineNote"))
        End If

        If HasPublishingDate.HasValue AndAlso HasPublishingDate.Value Then
            criteria.Add(Restrictions.IsNotNull("R.PublishingDate"))
            If RegistrationDateFrom.HasValue Then
                criteria.Add(Restrictions.Ge("R.PublishingDate", RegistrationDateFrom))
            End If
        End If

        DecorateDeliberaDetermina(criteria)

        DecorateForWorkflowStatus(criteria)

        DecorateNumber(criteria)

        DecorateWorkflow(criteria)

        DecorateRegistrationDate(criteria)

        DecorateSecurities(criteria)

        DecoratePassi(criteria)

        DecorateObject(criteria)

        DecorateRecipients(criteria)

        DecorateController(criteria)

        DecorateCategories(criteria)

        DecorateStatus(criteria)

        DecorateSteps(criteria)

        DecorateOC(criteria)

        DecorateWeb(criteria)

        DecoratePublication(criteria)

        DecorateTemplateSpecifications(criteria)

        If SelectedContainerId.HasValue Then
            criteria.Add(Restrictions.Eq("R.Container.Id", SelectedContainerId.Value))
        End If

        AttachFilterExpressions(criteria)

        AttachSQLExpressions(criteria)

        If HasDocumentalSeriesDraft Then
            DecorateResolutionWithDraftSeries(criteria)
        End If


        Return criteria
    End Function

#End Region

    ''' <summary> Aggancia criteri di ordinamento. </summary>
    Protected Sub CreateOrderClause(ByRef criteria As ICriteria)
        If Not AttachSortExpressions(criteria) Then
            Select Case True
                Case HasServiceNumber
                    AttachSortExpressions(criteria, "R.AdoptionDate", SortOrder.Ascending)
                    AttachSortExpressions(criteria, "R.ServiceNumber", SortOrder.Ascending)
                    AttachSortExpressions(criteria, "R.Id", SortOrder.Ascending)
                Case HasNumber
                    AttachSortExpressions(criteria, "R.AdoptionDate", SortOrder.Ascending)
                    AttachSortExpressions(criteria, "R.Number", SortOrder.Ascending)
                    AttachSortExpressions(criteria, "R.Id", SortOrder.Ascending)
                Case IsPrint
                    AttachSortExpressions(criteria, "R.Year", SortOrder.Ascending)
                    AttachSortExpressions(criteria, "R.Number", SortOrder.Ascending)
                Case HasInclusiveNumber
                    AttachSortExpressions(criteria, "R.AdoptionDate", SortOrder.Ascending)
                    AttachSortExpressions(criteria, "R.InclusiveNumber", SortOrder.Ascending)
                Case Else
                    AttachSortExpressions(criteria, "R.Id", SortOrder.Ascending)
            End Select
        End If
    End Sub

    Protected Sub DecorateCriteria(ByRef criteria As ICriteria)
        CreateOrderClause(criteria)

        If EnablePaging Then
            criteria.SetFirstResult(_startIndex)
            criteria.SetMaxResults(_pageSize)
        End If

        'Recupera le informazioni sui Log
        If (DocSuiteContext.Current.ResolutionEnv.IsLogEnabled AndAlso EagerLog) Then
            LoadFetchMode(criteria, "ResolutionLogs")
        End If

        'criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        If TopMaxRecords > 0 Then
            criteria.SetResultTransformer(New TopRecordsResultTransformer(TopMaxRecords))
        End If
    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.CountDistinct("Id"))
        criteria.SetProjection(proj)
        Dim result As Integer = criteria.UniqueResult(Of Integer)()

        If TopMaxRecords > 0 Then
            Return Math.Min(result, TopMaxRecords)
        End If
        Return result
    End Function

    Public Overrides Function DoSearch() As IList(Of Resolution)
        Dim criteria As ICriteria = CreateCriteria()

        'Decora il criterio con Expressioni di ordinamento e modalità Fetch
        DecorateCriteria(criteria)

        'Crea le eventuali proiezioni
        CreateProjections(criteria)

        Return criteria.List(Of Resolution)()
    End Function

    Private Function detachedManagedData() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of TabMaster)("tm_managedData")
        dc.Add(Restrictions.Eq("tm_managedData.Id.Configuration", DocSuiteContext.Current.ResolutionEnv.Configuration))
        dc.Add(Restrictions.EqProperty("tm_managedData.Id.ResolutionType", "R.Type.Id"))
        dc.SetProjection(Projections.Property("tm_managedData.ManagedData"))
        Return dc
    End Function
    Private Function detachedResolutionTypeCaption() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of TabMaster)("tm_resolutionTypeCaption")
        dc.Add(Restrictions.Eq("tm_resolutionTypeCaption.Id.Configuration", DocSuiteContext.Current.ResolutionEnv.Configuration))
        dc.Add(Restrictions.EqProperty("tm_resolutionTypeCaption.Id.ResolutionType", "R.Type.Id"))
        dc.SetProjection(Projections.Property("tm_resolutionTypeCaption.Description"))
        Return dc
    End Function
    Private Function detachedAdoptedDocumentName() As DetachedCriteria
        Dim dcAdoptionResStep As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("tw_AdoptionResStep")
        dcAdoptionResStep.Add(Restrictions.EqProperty("tw_AdoptionResStep.Id.WorkflowType", "R.WorkflowType"))
        dcAdoptionResStep.Add(Restrictions.Eq("tw_AdoptionResStep.Description", Data.WorkflowStep.ADOZIONE)) ' C'è già un'altra proprietà con lo stesso nome... - FG
        dcAdoptionResStep.SetProjection(Projections.Property("tw_AdoptionResStep.Id.ResStep"))

        ' FG20131023: ATTENZIONE, il max è una fix a dati incoerenti.
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ResolutionWorkflow)("rw_adoptedDocumentName")
        dc.Add(Restrictions.EqProperty("rw_adoptedDocumentName.Id.IdResolution", "R.Id"))
        dc.Add(Restrictions.Not(Restrictions.Eq("rw_adoptedDocumentName.IsActive", 2S)))
        dc.Add(Subqueries.PropertyEq("rw_adoptedDocumentName.ResStep", dcAdoptionResStep))
        dc.SetProjection(Projections.Max(Projections.Property("rw_adoptedDocumentName.DocumentName")))
        Return dc
    End Function

    Private Function detachedWebDocFieldDocumentName() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("tw_webDocFieldDocumentName")
        dc.Add(Restrictions.EqProperty("tw_webDocFieldDocumentName.Id.WorkflowType", "R.WorkflowType"))
        dc.Add(Restrictions.Eq("tw_webDocFieldDocumentName.Id.ResStep", DocSuiteContext.Current.ResolutionEnv.WebDocumentSourceWorkflowStep))
        dc.SetProjection(Projections.Property("tw_webDocFieldDocumentName.FieldDocument"))
        Return dc
    End Function
    Private Function detachedWebDocFieldAttachmentName() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("tw_webDocFieldAttachmentName")
        dc.Add(Restrictions.EqProperty("tw_webDocFieldAttachmentName.Id.WorkflowType", "R.WorkflowType"))
        dc.Add(Restrictions.Eq("tw_webDocFieldAttachmentName.Id.ResStep", DocSuiteContext.Current.ResolutionEnv.WebDocumentSourceWorkflowStep))
        dc.SetProjection(Projections.Property("tw_webDocFieldAttachmentName.FieldAttachment"))
        Return dc
    End Function

    Private Function detachedWebDocFieldDocumentOmissisName() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("tw_webDocFieldDocumentOmissisName")
        dc.Add(Restrictions.EqProperty("tw_webDocFieldDocumentOmissisName.Id.WorkflowType", "R.WorkflowType"))
        dc.Add(Restrictions.Eq("tw_webDocFieldDocumentOmissisName.Id.ResStep", DocSuiteContext.Current.ResolutionEnv.WebDocumentSourceWorkflowStep))
        dc.SetProjection(Projections.Property("tw_webDocFieldDocumentOmissisName.FieldDocumentsOmissis"))
        Return dc
    End Function
    Private Function detachedWebDocFieldAttachmentOmissisName() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of TabWorkflow)("tw_webDocFieldAttachmentOmissisName")
        dc.Add(Restrictions.EqProperty("tw_webDocFieldAttachmentOmissisName.Id.WorkflowType", "R.WorkflowType"))
        dc.Add(Restrictions.Eq("tw_webDocFieldAttachmentOmissisName.Id.ResStep", DocSuiteContext.Current.ResolutionEnv.WebDocumentSourceWorkflowStep))
        dc.SetProjection(Projections.Property("tw_webDocFieldAttachmentOmissisName.FieldAttachmentsOmissis"))
        Return dc
    End Function
    Private Function detachedReturnFromCollaboration() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ResolutionLog)("tw_ResolutionLog")
        dc.Add(Restrictions.EqProperty("tw_ResolutionLog.IdResolution", "R.Id"))
        dc.Add(Restrictions.Eq("tw_ResolutionLog.LogType", ResolutionLogType.AC.ToString()))
        dc.SetMaxResults(1).SetProjection(Projections.Constant(1))
        Return dc
    End Function
    Private Function detachedConfirmViewBy() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ResolutionLog)("tw_ResolutionLog")
        dc.Add(Restrictions.EqProperty("tw_ResolutionLog.IdResolution", "R.Id"))
        dc.Add(Restrictions.Eq("tw_ResolutionLog.LogType", ResolutionLogType.CV.ToString()))
        dc.AddOrder(Order.Desc("tw_ResolutionLog.LogDate"))
        dc.SetMaxResults(1).SetProjection(Projections.Property("tw_ResolutionLog.SystemUser"))
        Return dc
    End Function
    Private Function detachedReturnFromRetroStep() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ResolutionLog)("tw_ResolutionLog_ru")
        dc.Add(Restrictions.EqProperty("tw_ResolutionLog_ru.IdResolution", "R.Id"))
        dc.Add(Restrictions.Eq("tw_ResolutionLog_ru.LogType", ResolutionLogType.RU.ToString()))
        dc.SetMaxResults(1).SetProjection(Projections.Constant(1))
        Return dc
    End Function
    Private Function DetachedUserTakeCharge() As DetachedCriteria
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of ResolutionWorkflowUser)("tw_ResolutionWorkflowUser")
        dc.Add(Restrictions.EqProperty("tw_ResolutionWorkflowUser.ResolutionWorkflow.Id", "RW.Id"))
        dc.Add(Restrictions.Eq("tw_ResolutionWorkflowUser.AuthorizationType", AuthorizationRoleType.Responsible))
        dc.SetMaxResults(1).SetProjection(Projections.Property("tw_ResolutionWorkflowUser.Account"))
        Return dc
    End Function
    Protected Function GetProjectionForResolutionHeader(includeResolutionWorkflow As Boolean) As ProjectionList
        Dim proj As ProjectionList = Projections.ProjectionList()

        proj.Add(Projections.Property("Id"), "Id")
        proj.Add(Projections.Property("Year"), "Year")
        proj.Add(Projections.Property("Number"), "Number")
        proj.Add(Projections.Property("InclusiveNumber"), "InclusiveNumber")
        proj.Add(Projections.Property("ServiceNumber"), "ServiceNumber")
        proj.Add(Projections.Property("ProposeDate"), "ProposeDate")
        proj.Add(Projections.Property("LeaveDate"), "LeaveDate")
        proj.Add(Projections.Property("EffectivenessDate"), "EffectivenessDate")
        proj.Add(Projections.Property("ResponseDate"), "ResponseDate")
        proj.Add(Projections.Property("WaitDate"), "WaitDate")
        proj.Add(Projections.Property("ConfirmDate"), "ConfirmDate")
        proj.Add(Projections.Property("WarningDate"), "WarningDate")
        proj.Add(Projections.Property("PublishingDate"), "PublishingDate")
        proj.Add(Projections.Property("AdoptionDate"), "AdoptionDate")
        proj.Add(Projections.Property("ResolutionObject"), "ResolutionObject")
        proj.Add(Projections.Property("WorkflowType"), "WorkflowType")
        proj.Add(Projections.Property("DeclineNote"), "DeclineNote")
        proj.Add(Projections.Property("Note"), "Note")

        proj.Add(Projections.Property("ControllerStatus"), "ControllerStatus")
        proj.Add(Projections.Property("ControllerStatus.Acronym"), "ControllerStatusAcronym")
        proj.Add(Projections.Property("Type"), "Type")
        proj.Add(Projections.Property("Type.Id"), "TypeId")
        proj.Add(Projections.Property("Status"), "Status")
        proj.Add(Projections.Property("Status.Id"), "StatusId")
        proj.Add(Projections.Property("Category"), "Category")
        proj.Add(Projections.Property("Category.Name"), "CategoryName")

        proj.Add(Projections.Property("Container.Id"), "ContainerId")
        proj.Add(Projections.Property("Container.Name"), "ContainerName")

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-To") OrElse DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            proj.Add(Projections.Property("Container.Privacy"), "ContainerPrivacy")
        End If

        proj.Add(Projections.Property("Location.Id"), "LocationId")
        proj.Add(Projections.Property("Location.ReslBiblosDSDB"), "LocationReslBiblosDSDB")

        proj.Add(Projections.Property("AlternativeProposer"), "AlternativeProposer")
        proj.Add(Projections.Property("AlternativeManager"), "AlternativeManager")
        proj.Add(Projections.Property("AlternativeAssignee"), "AlternativeAssignee")
        proj.Add(Projections.Property("AlternativeRecipient"), "AlternativeRecipient")

        ' todo: spalare cacca
        If Not String.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.ConservationFieldDocument) Then
            Dim sqlIdDocument As String = String.Format("(SELECT TOP 1 [{0}] FROM FileResolution WHERE idResolution = {1}) AS DOC", DocSuiteContext.Current.ResolutionEnv.ConservationFieldDocument, "{alias}.idResolution")
            proj.Add(Projections.SqlProjection(sqlIdDocument, New String() {"DOC"}, New Type.Int32Type() {NHibernateUtil.Int32}), "IdDocument")

            Dim sqlIdAttachments As String = String.Format("(SELECT TOP 1 [{0}] FROM FileResolution WHERE idResolution = {1}) AS ATT", DocSuiteContext.Current.ResolutionEnv.ConservationFieldAttachment, "{alias}.idResolution")
            proj.Add(Projections.SqlProjection(sqlIdAttachments, New String() {"ATT"}, New Type.Int32Type() {NHibernateUtil.Int32}), "IdAttachments")
        End If

        ' ritiro frontalino (non è messo in configurazione ausl-re perchè non si vuole più creare tagli di nodi)
        Const sqlIdFrontalinoRitiro As String = "(SELECT TOP 1 idFrontalinoRitiro FROM FileResolution WHERE idResolution = {alias}.idResolution) AS RIT"
        proj.Add(Projections.SqlProjection(sqlIdFrontalinoRitiro, New String() {"RIT"}, New Type.Int32Type() {NHibernateUtil.Int32}), "IdFrontalinoRitiro")

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") OrElse DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            proj.Add(Projections.Property("SupervisoryBoardWarningDate"), "SupervisoryBoardWarningDate")
            proj.Add(Projections.Property("OCSupervisoryBoard"), "OCSupervisoryBoard")
            proj.Add(Projections.Property("OCRegion"), "OCRegion")
            proj.Add(Projections.Property("OCManagement"), "OCManagement")
            proj.Add(Projections.Property("OCCorteConti"), "OCCorteConti")
            proj.Add(Projections.Property("OCOther"), "OCOther")
            Const sqlProposerCode As String = "(SELECT STUFF((SELECT ', ' + Code FROM Contact WHERE Incremental IN (SELECT idContact FROM ResolutionContact WHERE idResolution = {alias}.idResolution AND ComunicationType='P') FOR XML PATH('')), 1, 2, '')) as ProposerCode"
            proj.Add(Projections.SqlProjection(sqlProposerCode, New String() {"ProposerCode"}, New Type.StringType() {NHibernateUtil.String}), "ProposerCode")
            Const sqlResolutionFile As String = "(SELECT idResolutionFile FROM FileResolution WHERE idResolution = {alias}.idResolution) as IdResolutionFile"
            proj.Add(Projections.SqlProjection(sqlResolutionFile, New String() {"IdResolutionFile"}, New Type.Int32Type() {NHibernateUtil.Int32}), "IdResolutionFile")
        End If

        proj.Add(Projections.SubQuery(detachedManagedData()), "ManagedData")
        proj.Add(Projections.SubQuery(detachedResolutionTypeCaption()), "ResolutionTypeCaption")
        proj.Add(Projections.SubQuery(detachedAdoptedDocumentName()), "AdoptedDocumentName")

        proj.Add(Projections.SubQuery(detachedWebDocFieldDocumentName()), "WebDocFieldDocumentName")
        proj.Add(Projections.SubQuery(detachedWebDocFieldAttachmentName()), "WebDocFieldAttachmentName")
        proj.Add(Projections.SubQuery(detachedWebDocFieldDocumentOmissisName()), "WebDocFieldDocumentOmissisName")
        proj.Add(Projections.SubQuery(detachedWebDocFieldAttachmentOmissisName()), "WebDocFieldAttachmentOmissisName")

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
            proj.Add(Projections.SubQuery(detachedReturnFromCollaboration()), "ReturnFromCollaboration")
            proj.Add(Projections.SubQuery(detachedConfirmViewBy()), "ConfirmViewBy")
            proj.Add(Projections.SubQuery(detachedReturnFromRetroStep()), "ReturnFromRetroStep")
            proj.Add(Projections.SubQuery(DetachedUserTakeCharge()), "CurrentUserTakeCharge")
        End If

        If includeResolutionWorkflow Then
            proj.Add(Projections.Property("RW.RegistrationDate"), "WorkflowStepDate")
        End If

        Return proj
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of ResolutionHeader)
        Dim criteria As ICriteria = CreateCriteria()

        CreateOrderClause(criteria)

        If EnablePaging Then
            criteria.SetFirstResult(PageIndex)
            criteria.SetMaxResults(PageSize)
        End If

        Dim proj As ProjectionList = GetProjectionForResolutionHeader(True)

        If DocSuiteContext.Current.ResolutionEnv.IsLogEnabled Then
            LoadFetchMode(criteria, "ResolutionLogs")
        End If

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") Then
            criteria.SetProjection(proj)
        Else
            criteria.SetProjection(Projections.Distinct(proj))
        End If
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ResolutionHeader))

        If TopMaxRecords > 0 Then
            criteria.SetResultTransformer(New TopRecordsResultTransformer(TopMaxRecords))
        End If


        Return criteria.List(Of ResolutionHeader)()
    End Function

#Region "IFinder Implementation"
    Public Overrides Function DoSearch(ByVal sortExpr As String) As IList(Of Resolution)
        MyBase.DoSearch(sortExpr)
        Return DoSearch()
    End Function

    Public Overrides Function DoSearch(ByVal sortExpr As String, ByVal startRow As Integer, ByVal pagSize As Integer) As IList(Of Resolution)
        MyBase.DoSearch(sortExpr)
        PageIndex = startRow
        PageSize = pagSize
        Return DoSearch()
    End Function
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

#Region "Private Methods"
    Protected Sub CreateDateGeExpression(ByRef expr As Conjunction, ByVal filterProperty As String, ByVal value As Date)
        Dim sqlExpression As String = NHibernateHelper.GreaterThanOrEqualToDateIsoFormat(filterProperty, value)

        If (Not String.IsNullOrEmpty(sqlExpression)) Then
            expr.Add(Expression.Sql(sqlExpression))
        End If
    End Sub

    Protected Sub CreateDateLeExpression(ByRef expr As Conjunction, ByVal filterProperty As String, ByVal value As Date)
        Dim sqlExpression As String = NHibernateHelper.LessThanOrEqualToDateIsoFormat(filterProperty, value)

        If (Not String.IsNullOrEmpty(sqlExpression)) Then
            expr.Add(Expression.Sql(sqlExpression))
        End If
    End Sub

    Protected Sub CreateDateEqExpression(ByRef expr As Conjunction, ByVal filterProperty As String, ByVal value As Date)
        Dim sqlExpression As String = NHibernateHelper.EqualToDateIsoFormat(filterProperty, value)

        If (Not String.IsNullOrEmpty(sqlExpression)) Then
            expr.Add(Expression.Sql(sqlExpression))
        End If
    End Sub

    Protected Sub AttachStatusExpression(ByRef criteria As ICriteria)
        '(idStatus=0 OR idStatus BETWEEN -4 AND -2)
        Dim statDisju As Disjunction = Expression.Disjunction()
        statDisju.Add(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)))
        statDisju.Add(Expression.Between("Status.Id", CShort(ResolutionStatusId.Rettificato), CShort(ResolutionStatusId.Annullato)))
        criteria.Add(statDisju)
    End Sub

    Protected Sub AttachOnlyStatusCancelExpression(ByRef criteria As ICriteria)
        Dim d As Disjunction = Expression.Disjunction
        d.Add(Expression.Not(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo))))
        criteria.Add(d)
    End Sub
#End Region

#Region "Private Methods: Criteria Workflow Step"
    Private Sub AttachWorkflowStepExpression(ByRef criteria As ICriteria, description As String)
        Dim existStepCriteria As DetachedCriteria = DetachedCriteria.For(GetType(TabWorkflow), "TW")
        existStepCriteria.Add(Restrictions.Eq("TW.Description", description))
        existStepCriteria.Add(Restrictions.EqProperty("TW.Id.WorkflowType", "R.WorkflowType"))
        existStepCriteria.Add(Restrictions.EqProperty("TW.Id.ResStep", "RW.ResStep"))
        existStepCriteria.SetProjection(Projections.Constant(1))
        criteria.Add(Subqueries.Exists(existStepCriteria))

        DecorateWorkflow(criteria)
    End Sub

    Private Sub AttachWorkflowStepsExpression(ByRef criteria As ICriteria, descriptions As List(Of String))
        Dim existStepCriteria As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionWorkflow), "RWW")
        existStepCriteria.Add(Restrictions.EqProperty("RWW.Resolution.Id", "R.Id"))

        Dim existTabWorkflow As DetachedCriteria = DetachedCriteria.For(GetType(TabWorkflow), "TWW")
        existTabWorkflow.Add(Restrictions.In("TWW.Description", descriptions))
        existTabWorkflow.Add(Restrictions.EqProperty("TWW.Id.WorkflowType", "R.WorkflowType"))
        existTabWorkflow.Add(Restrictions.EqProperty("TWW.Id.ResStep", "RWW.ResStep"))
        existTabWorkflow.SetProjection(Projections.Constant(1))

        existStepCriteria.Add(Subqueries.Exists(existTabWorkflow))

        If StepAttivo Then
            existStepCriteria.Add(Restrictions.Eq("RWW.IsActive", 1S))
        End If

        existStepCriteria.SetProjection(Projections.Constant(1))
        criteria.Add(Subqueries.Exists(existStepCriteria))
    End Sub

    Private Sub AttachWorkflowStepExpression(ByRef criteria As ICriteria, ByVal [step] As Short?)
        'Criteria per i ResolutionWorkflow
        Dim dcRw As DetachedCriteria = DetachedCriteria.For(GetType(ResolutionWorkflow), "RW")
        dcRw.Add(Restrictions.Eq("ResStep", [step].Value))
        dcRw.Add(Restrictions.Eq("IsActive", 1S))
        dcRw.SetProjection(Projections.Property("Id.IdResolution")) 'proiezione sull'IdResolution

        'Subquery sulla Resolution
        criteria.Add(Subqueries.PropertyIn("Id", dcRw))
    End Sub

    Private Sub DecorateWorkflow(ByRef criteria As ICriteria)
        criteria.CreateAliasIfNotExists("ResolutionWorkflows", "RW", JoinType.LeftOuterJoin)
        Dim isActDisju As Disjunction = Restrictions.Disjunction()
        isActDisju.Add(Restrictions.Eq("RW.IsActive", 1S))
        isActDisju.Add(Restrictions.IsNull("RW.IsActive"))
        criteria.Add(isActDisju)
    End Sub

#End Region

#Region "Statistics"
    Public Function DoStat(ByVal groupBy As String) As ArrayList
        Dim criteria As ICriteria = CreateCriteria()

        If Not String.IsNullOrEmpty(groupBy) Then
            If groupBy.Eq("active") Then
                criteria.Add(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)))
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.RowCount(), "rows"))

            ElseIf groupBy.Eq("activeregistrationerror") Then
                criteria.Add(Restrictions.Not(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo))))
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.RowCount()))

            ElseIf groupBy.Eq("type") Then
                criteria.Add(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)))
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Type.Id")).
                                       Add(Projections.GroupProperty("Type.Description")).
                                       Add(Projections.RowCount()))
                criteria.AddOrder(Order.Desc("Type.Description"))

            ElseIf groupBy.Eq("typeregistrationerror") Then
                criteria.Add(Restrictions.Not(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo))))
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Type.Id")).
                                       Add(Projections.GroupProperty("Type.Description")).
                                       Add(Projections.RowCount()))
                criteria.AddOrder(Order.Desc("Type.Description"))

            ElseIf groupBy.Eq("container") Then
                criteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Container.Id")).
                                       Add(Projections.GroupProperty("Container.Name")).
                                       Add(Projections.RowCount()))
                criteria.AddOrder(Order.Asc("Container.Name"))

            ElseIf groupBy.Eq("ocregion") Then
                CreateOCStatistic(criteria, "R.OCRegion")

            ElseIf groupBy.Eq("ocsupervisoryboard") Then
                CreateOCStatistic(criteria, "R.OCSupervisoryBoard")

            ElseIf groupBy.Eq("ocmanagement") Then
                CreateOCStatistic(criteria, "R.OCManagement")

            ElseIf groupBy.Eq("occorteconti") Then
                CreateOCStatistic(criteria, "R.OCCorteConti")

            ElseIf groupBy.Eq("ocother") Then
                CreateOCStatistic(criteria, "R.OCOther")

            ElseIf groupBy.Eq("proposer") Then
                Dim result As IList(Of ContactsStatisticsDTO) = createContactsStatistic(criteria, "P")
                Return dtoToArrayList(result.ToList())

            ElseIf groupBy.Eq("recipient") Then
                Dim result As IList(Of ContactsStatisticsDTO) = createContactsStatistic(criteria, "D")
                Return dtoToArrayList(result.ToList())

            End If

        Else
            criteria.SetProjection(Projections.ProjectionList().Add(Projections.RowCount(), "rows"))
        End If
        Return New ArrayList(criteria.List())
    End Function

    Private Sub CreateOCStatistic(ByRef criteria As ICriteria, ByVal OCProperty As String)
        criteria.Add(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)))
        criteria.Add(Restrictions.Eq(OCProperty, True))
        criteria.SetProjection(Projections.ProjectionList() _
            .Add(Projections.GroupProperty(OCProperty)) _
            .Add(Projections.RowCount())
)
    End Sub

    ''' <summary> Imposta il criterio per l'estrazione delle statistiche per contatto. </summary>
    ''' <param name="criteria">Criterio di riferimento.</param>
    ''' <param name="comunicationType">Filtro per tipo di contatto.</param>
    Private Function createContactsStatistic(ByRef criteria As ICriteria, comunicationType As String) As IList(Of ContactsStatisticsDTO)
        With criteria
            .CreateAlias("ResolutionContacts", "RC")
            .CreateAlias("RC.Contact", "C")
            .Add(Restrictions.Eq("Status.Id", CShort(ResolutionStatusId.Attivo)))
            .Add(Restrictions.Eq("RC.Id.ComunicationType", comunicationType))
        End With
        Dim proj As ProjectionList = Projections.ProjectionList()
        With proj
            .Add(Projections.Count("Id"), "Total")
            .Add(Projections.Property("C.Description"), "Description")
            .Add(Projections.Property("Type.Id"), "ReslType")
            .Add(Projections.GroupProperty("C.Description"))
            .Add(Projections.GroupProperty("Type.Id"))
            .Add(Projections.GroupProperty("Type.Description"))
        End With
        criteria.SetProjection(proj)
        criteria.AddOrder(Order.Desc("Type.Description"))
        criteria.AddOrder(Order.Asc("C.Description"))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ContactsStatisticsDTO))
        Return criteria.List(Of ContactsStatisticsDTO)()
    End Function

    ''' <summary> Trasforma un DTO in un ArrayList di ArrayList. </summary>
    ''' <param name="dtoList">DTO da trasformare.</param>
    ''' <remarks>Lo so, fa schifo ma per il momento va bene così visto che altrimenti toccava stravolgere tutto... - FG</remarks>
    Private Function dtoToArrayList(dtoList As IList) As ArrayList
        Dim listed As New ArrayList
        For Each dto As Object In dtoList
            Dim item As New ArrayList
            For Each p As PropertyInfo In dto.GetType().GetProperties()
                Dim value As Object = p.GetValue(dto, BindingFlags.GetProperty, Nothing, Nothing, Nothing)
                item.Add(value)
            Next
            listed.Add(item)
        Next
        Return listed
    End Function

#End Region

End Class