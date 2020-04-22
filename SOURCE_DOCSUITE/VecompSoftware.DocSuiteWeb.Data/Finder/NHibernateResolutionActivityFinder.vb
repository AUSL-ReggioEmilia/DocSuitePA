Imports System.ComponentModel
Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.NHibernateManager

<Serializable(), DataObject()>
Public Class NHibernateResolutionActivityFinder
    Inherits NHibernateBaseFinder(Of ResolutionActivity, ResolutionActivityHeader)

#Region " Properties "
    Public Property EnablePaging As Boolean
    Public Property ActivityType As ResolutionActivityType?
    Public Property Status As ResolutionActivityStatus?
    Public Property Username As String
    Public Property Domain As String
    Public Property CheckExecutiveRight As Boolean
    Public Property OCSupervisoryBoard As Boolean
    Public Property OCRegion As Boolean
    Public Property OCCorteConti As Boolean
    Public Property OCOther As Boolean

#End Region

#Region " NHibernate Properties "

    Public Property ActivityDateFrom As DateTimeOffset?

    Public Property ActivityDateTo As DateTimeOffset?

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

#Region "Constuctor"
    Public Sub New()
        SessionFactoryName = "ReslDB"
        EnablePaging = True
    End Sub

#End Region

#Region "Criteria"
    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionActivity)("RA")
        'Filtro Data di Attività a partire da
        If ActivityDateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("RA.ActivityDate", ActivityDateFrom.Value))
        End If

        'Filtro Data di Attività fino a
        If ActivityDateTo.HasValue Then
            criteria.Add(Restrictions.Le("RA.ActivityDate", ActivityDateTo.Value))
        End If

        If ActivityType.HasValue Then
            criteria.Add(Restrictions.Eq("RA.ActivityType", ActivityType.Value))
        End If

        If Status.HasValue Then
            criteria.Add(Restrictions.Eq("RA.Status", Status.Value))
        End If

        criteria.CreateAlias("RA.Resolution", "R")
        If OCSupervisoryBoard Then
            criteria.Add(Restrictions.Eq("R.OCSupervisoryBoard", OCSupervisoryBoard))
        End If

        If OCRegion Then
            criteria.Add(Restrictions.Eq("R.OCRegion", OCRegion))
        End If

        If OCCorteConti Then
            criteria.Add(Restrictions.Eq("R.OCCorteConti", OCCorteConti))
        End If

        If OCOther Then
            criteria.Add(Restrictions.Eq("R.OCOther", OCOther))
        End If

        Return criteria
    End Function

#End Region

#Region " Methods "
    Public Overloads Overrides Function DoSearch() As IList(Of ResolutionActivity)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        CreateOrderClause(criteria)
        AttachFilterExpressions(criteria)
        Return criteria.List(Of ResolutionActivity)()
    End Function

    Protected Sub CreateOrderClause(ByRef criteria As ICriteria)
        If AttachSortExpressions(criteria) Then
            Exit Sub
        End If
        AttachSortExpressions(criteria, "RA.ActivityDate", SortOrder.Descending)

    End Sub

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overridable Sub SetProjectionHeaders(ByRef criteria As ICriteria)
        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("RA.Id"), "Id")
        proj.Add(Projections.Property("RA.Description"), "Description")
        proj.Add(Projections.Property("RA.Status"), "Status")
        proj.Add(Projections.Property("RA.ActivityDate"), "ActivityDate")
        proj.Add(Projections.Property("RA.ActivityType"), "Type")
        proj.Add(Projections.Property("R.Id"), "ResolutionId")
        proj.Add(Projections.Property("R.UniqueId"), "ResolutionUniqueId")
        proj.Add(Projections.Property("R.ResolutionObject"), "ResolutionObject")
        proj.Add(Projections.Property("R.Status.Id"), "ResolutionStatusId")
        proj.Add(Projections.Property("R.Type.Id"), "ResolutionTypeId")
        proj.Add(Projections.Property("R.InclusiveNumber"), "ResolutionInclusiveNumber")
        proj.Add(Projections.Property("R.EffectivenessDate"), "ResolutionEffectivenessDate")
        proj.Add(Projections.Property("R.OCSupervisoryBoard"), "ResolutionOCSupervisoryBoard")
        proj.Add(Projections.Property("R.OCRegion"), "ResolutionOCRegion")
        proj.Add(Projections.Property("R.OCManagement"), "ResolutionOCManagement")
        proj.Add(Projections.Property("R.OCOther"), "ResolutionOCOther")
        proj.Add(Projections.Property("R.OCCorteConti"), "ResolutionOCCorteConti")

        If CheckExecutiveRight Then
            proj.Add(Projections.SubQuery(HasExecutiveRight()), "ResolutionExecutiveRight")
        End If

        criteria.SetProjection(Projections.Distinct(proj))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ResolutionActivityHeader))
    End Sub

    Private Function HasExecutiveRight() As DetachedCriteria
        Dim rightsCriteria As DetachedCriteria = DetachedCriteria.For(Of SecurityGroups)("SG")
        rightsCriteria.CreateAlias("SG.SecurityUsers", "SU")
        rightsCriteria.Add(Restrictions.Eq("SU.Account", Username))
        rightsCriteria.Add(Restrictions.Eq("SU.UserDomain", Domain))
        rightsCriteria.CreateAlias("SG.ContainerGroup", "CG")
        rightsCriteria.Add(Restrictions.Like("CG._resolutionRights", "_1", MatchMode.Start))
        rightsCriteria.CreateAlias("CG.Container", "C")
        rightsCriteria.Add(Restrictions.EqProperty("C.Id", "R.Container.Id"))
        rightsCriteria.SetProjection(Projections.RowCount())
        Return rightsCriteria
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of ResolutionActivityHeader)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        CreateOrderClause(criteria)
        AttachFilterExpressions(criteria)
        SetProjectionHeaders(criteria)
        Return criteria.List(Of ResolutionActivityHeader)()
    End Function

#End Region

End Class
