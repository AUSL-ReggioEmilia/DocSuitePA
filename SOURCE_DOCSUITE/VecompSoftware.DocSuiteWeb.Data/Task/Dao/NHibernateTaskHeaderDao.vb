Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Transform

Public Class NHibernateTaskHeaderDao
    Inherits BaseNHibernateDao(Of TaskHeader)

    Private Const FactoryName As String = "ProtDB"

    Public Function GetByTypeAndStatus(taskType As TaskTypeEnum, status As TaskStatusEnum) As IList(Of TaskHeader)

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("TaskType", taskType))
        criteria.Add(Restrictions.Eq("Status", status))

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of TaskHeader)()
    End Function

    Public Sub UpdateHeader(header As TaskHeader)
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
            Using transaction As ITransaction = session.BeginTransaction()
                session.SaveOrUpdate(header)
                transaction.Commit()
            End Using
        End Using
    End Sub

    Public Function GetRecentFastMergeCodes(maxResults As Integer) As IList(Of String)
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
            Dim criteria As ICriteria = session.CreateCriteria(Of TaskHeader)()
            criteria.Add(Restrictions.Eq("TaskType", TaskTypeEnum.FastProtocolSender))

            criteria.SetProjection(Projections.GroupProperty("Code"))
            criteria.SetMaxResults(maxResults)
            criteria.AddOrder(Order.Desc(Projections.Max("RegistrationDate")))
            Return criteria.List(Of String)()
        End Using
    End Function

    Public Function GetProtocolsKey(ids As IEnumerable(Of Integer)) As IList(Of YearNumberCompositeKey)
        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
            Dim criteria As ICriteria = session.CreateCriteria(Of TaskHeaderProtocol)()
            criteria.Add(Restrictions.In("Header.Id", ids.ToArray()))
            criteria.SetProjection(Projections.Distinct(Projections.Property("Protocol.Id")))
            Return criteria.List(Of YearNumberCompositeKey)()
        End Using
    End Function

    Public Function GetByPOL(request As POLRequest) As TaskHeader
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("POLRequests", "TPOL")
        criteria.CreateAlias("TPOL.POLRequest", "POL")
        criteria.Add(Restrictions.Eq("POL.Id", request.Id))
        Return criteria.UniqueResult(Of TaskHeader)()
    End Function

    Public Function GetByPEC(pecMail As PECMail) As TaskHeader
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("PECMails", "TPEC")
        criteria.CreateAlias("TPEC.PECMail", "PEC")
        criteria.Add(Restrictions.Eq("PEC.Id", pecMail.Id))
        Return criteria.UniqueResult(Of TaskHeader)()
    End Function

    Public Function HasTaskSendingElementsToComplete(taskHeader As TaskHeader) As Boolean
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType, "T")
        Dim disjuction As Disjunction = New Disjunction()
        disjuction.Add(Subqueries.Exists(HasPendingPECMailsCriteria()))
        disjuction.Add(Subqueries.Exists(HasPendingPOLRequestCriteria()))

        criteria.Add(disjuction)
        criteria.Add(Restrictions.Eq("T.Id", taskHeader.Id))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)() > 0
    End Function

    Private Function HasPendingPECMailsCriteria() As DetachedCriteria
        Dim detachedCriteria As DetachedCriteria = DetachedCriteria.For(Of TaskHeaderPECMail)("THPM")
        detachedCriteria.CreateAlias("THPM.Header", "TH")
        detachedCriteria.CreateAlias("THPM.PECMail", "PM")
        detachedCriteria.Add(Restrictions.EqProperty("TH.Id", "T.Id"))
        detachedCriteria.Add(Restrictions.Eq("PM.Direction", Convert.ToInt16(PECMailDirection.Outgoing)))
        detachedCriteria.Add(Restrictions.IsNull("PM.MailDate"))
        detachedCriteria.Add(Restrictions.In("PM.IsActive", New List(Of Short) From {ActiveType.PECMailActiveType.Active, ActiveType.PECMailActiveType.Processing}))
        detachedCriteria.SetProjection(Projections.Constant(1))
        detachedCriteria.SetMaxResults(1)
        Return detachedCriteria
    End Function

    Private Function HasPendingPOLRequestCriteria() As DetachedCriteria
        Dim detachedCriteria As DetachedCriteria = DetachedCriteria.For(Of TaskHeaderPOLRequest)("THPOL")
        detachedCriteria.CreateAlias("THPOL.Header", "TH")
        detachedCriteria.CreateAlias("THPOL.POLRequest", "POL")
        detachedCriteria.Add(Restrictions.EqProperty("TH.Id", "T.Id"))
        detachedCriteria.Add(Restrictions.In("POL.Status", New List(Of POLRequestStatusEnum) From {POLRequestStatusEnum.RequestQueued,
                                             POLRequestStatusEnum.RequestSent,
                                             POLRequestStatusEnum.NeedConfirm,
                                             POLRequestStatusEnum.Confirmed}))
        detachedCriteria.SetProjection(Projections.Constant(1))
        detachedCriteria.SetMaxResults(1)
        Return detachedCriteria
    End Function
End Class
