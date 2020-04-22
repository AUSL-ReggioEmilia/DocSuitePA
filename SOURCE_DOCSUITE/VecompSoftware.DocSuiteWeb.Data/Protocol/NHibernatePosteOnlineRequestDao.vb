Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports NHibernate.SqlCommand
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Transform

Public Class NHibernatePosteOnlineRequestDao
    Inherits BaseNHibernateDao(Of POLRequest)

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByProtocol(ByVal year As Short, ByVal number As Integer) As IList(Of POLRequest)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.SetFetchMode("Contacts", FetchMode.Join)
        criteria.Add(Restrictions.Eq("ProtocolYear", year))
        criteria.Add(Restrictions.Eq("ProtocolNumber", number))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        Return criteria.List(Of POLRequest)()
    End Function

    Public Function GetRecipientsByProtocol(ByVal year As Short, ByVal number As Integer) As IList(Of POLRequestRecipientHeader)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of POLRequestRecipient)("PRR")
        criteria.CreateCriteria("Request", "R", JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("R.ProtocolYear", year))
        criteria.Add(Restrictions.Eq("R.ProtocolNumber", number))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)
        NHPosteOnlineRequestFinder.SetProjectionPOLRequestRecipientHeader(criteria)
        Return criteria.List(Of POLRequestRecipientHeader)()
    End Function

    Public Function Costi(ByVal dateFrom As DateTime?, ByVal dateTo As DateTime?) As IList(Of PolDtoCosti)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of POLRequestRecipient)("C")
        dc.Add(Restrictions.EqProperty("C.Request.Id", "Rq.Id"))
        If dateFrom.HasValue AndAlso dateTo.HasValue Then
            dc.Add(Restrictions.Between("C.DataSpedizione", dateFrom.Value.BeginOfTheDay(), dateTo.Value.EndOfTheDay()))
        ElseIf dateFrom.HasValue Then
            dc.Add(Restrictions.Ge("C.DataSpedizione", dateFrom.Value.BeginOfTheDay()))
        ElseIf dateTo.HasValue Then
            dc.Add(Restrictions.Le("C.DataSpedizione", dateTo.Value.EndOfTheDay()))
        End If
        dc.SetProjection(Projections.Constant(1))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of POLRequest)("Rq")
        criteria.CreateAlias("Account", "A", JoinType.InnerJoin).CreateAlias("A.Roles", "R", JoinType.InnerJoin)

        criteria.Add(Restrictions.IsNotNull("CostoTotale"))
        criteria.Add(Restrictions.Gt("CostoTotale", 0.0R))
        criteria.Add(Subqueries.Exists(dc))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.GroupProperty("R.Name"), "Settore")
        proj.Add(Projections.GroupProperty("Type"), "Tipo")
        proj.Add(Projections.Sum("CostoTotale"), "CostoTotale")
        criteria.SetProjection(proj)

        criteria.SetResultTransformer(Transformers.AliasToBean(Of PolDtoCosti))
        Return criteria.List(Of PolDtoCosti)()
    End Function
    Public Function CostiAccount(ByVal dateFrom As DateTime?, ByVal dateTo As DateTime?) As IList(Of PolDtoCosti)
        Dim dc As DetachedCriteria = DetachedCriteria.For(Of POLRequestRecipient)("C")
        dc.Add(Restrictions.EqProperty("C.Request.Id", "Rq.Id"))
        If dateFrom.HasValue AndAlso dateTo.HasValue Then
            dc.Add(Restrictions.Between("C.DataSpedizione", dateFrom.Value.BeginOfTheDay(), dateTo.Value.EndOfTheDay()))
        ElseIf dateFrom.HasValue Then
            dc.Add(Restrictions.Ge("C.DataSpedizione", dateFrom.Value.BeginOfTheDay()))
        ElseIf dateTo.HasValue Then
            dc.Add(Restrictions.Le("C.DataSpedizione", dateTo.Value.EndOfTheDay()))
        End If
        dc.SetProjection(Projections.Constant(1))

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of POLRequest)("Rq")
        criteria.CreateAlias("Account", "A", JoinType.InnerJoin)

        criteria.Add(Restrictions.IsNotNull("CostoTotale"))
        criteria.Add(Restrictions.Gt("CostoTotale", 0.0R))
        criteria.Add(Subqueries.Exists(dc))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.GroupProperty("A.Name"), "Account")
        proj.Add(Projections.GroupProperty("Type"), "Tipo")
        proj.Add(Projections.Sum("CostoTotale"), "CostoTotale")
        criteria.SetProjection(proj)

        criteria.SetResultTransformer(Transformers.AliasToBean(Of PolDtoCosti))
        Return criteria.List(Of PolDtoCosti)()
    End Function


    Public Function GetOngoingRaccomandate(account As POLAccount) As IList(Of ROLRequest)
        Dim crt As ICriteria = NHibernateSession.CreateCriteria(Of ROLRequest)()
        crt.Add(Restrictions.Eq("Type", POLRequestType.Raccomandata))
        crt.Add(Restrictions.Eq("Account", account))
        crt.Add(Restrictions.In("Status", New Object() {POLRequestStatusEnum.RequestQueued, POLRequestStatusEnum.RequestSent, POLRequestStatusEnum.NeedConfirm}))

        Return crt.List(Of ROLRequest)()
    End Function

    Public Function GetConfirmedRaccomandate(account As POLAccount) As IList(Of ROLRequest)
        Dim crt As ICriteria = NHibernateSession.CreateCriteria(Of ROLRequest)()
        crt.Add(Restrictions.Eq("Type", POLRequestType.Raccomandata))
        crt.Add(Restrictions.Eq("Account", account))
        crt.Add(Restrictions.In("Status", New Object() {POLRequestStatusEnum.Confirmed}))

        Return crt.List(Of ROLRequest)()
    End Function

    Public Function GetOngoingLettere(account As POLAccount) As IList(Of LOLRequest)
        Dim crt As ICriteria = NHibernateSession.CreateCriteria(Of LOLRequest)()
        crt.Add(Restrictions.Eq("Type", POLRequestType.Lettera))
        crt.Add(Restrictions.Eq("Account", account))
        crt.Add(Restrictions.In("Status", New Object() {POLRequestStatusEnum.RequestQueued, POLRequestStatusEnum.RequestSent, POLRequestStatusEnum.NeedConfirm}))

        Return crt.List(Of LOLRequest)()
    End Function

    Public Function GetOngoingTelegrammi(account As POLAccount) As IList(Of TOLRequest)
        Dim crt As ICriteria = NHibernateSession.CreateCriteria(Of TOLRequest)()
        crt.Add(Restrictions.Eq("Type", POLRequestType.Telegramma))
        crt.Add(Restrictions.Eq("Account", account))
        crt.Add(Expression.In("Status", New Object() {POLRequestStatusEnum.RequestQueued, POLRequestStatusEnum.RequestSent}))

        Return crt.List(Of TOLRequest)()
    End Function

    Public Function GetOngoingSerc(account As POLAccount) As IList(Of SOLRequest)
        Dim crt As ICriteria = NHibernateSession.CreateCriteria(Of SOLRequest)()
        crt.Add(Restrictions.Eq("Type", POLRequestType.Serc))
        crt.Add(Restrictions.Eq("Account", account))
        crt.Add(Restrictions.In("Status", New Object() {POLRequestStatusEnum.RequestQueued, POLRequestStatusEnum.RequestSent, POLRequestStatusEnum.NeedConfirm}))

        Return crt.List(Of SOLRequest)()
    End Function
#End Region
    
End Class
