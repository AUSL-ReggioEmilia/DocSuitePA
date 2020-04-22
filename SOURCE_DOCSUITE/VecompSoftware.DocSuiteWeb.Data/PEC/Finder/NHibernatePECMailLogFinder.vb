Imports NHibernate
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports VecompSoftware.NHibernateManager


Public Class NHibernatePECMailLogFinder
    Inherits NHibernateBaseFinder(Of PECMailLog, PECMailLogHeader)

#Region " Properties "

    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean

    Public Property PECMail As PECMail

    Public Property PECMailIDIn As List(Of Integer)

    Public Property DateFrom As DateTime?
    Public Property DateTo As DateTime?
    Public Property LogType As String
    Public Property DescriptionLike() As String

#End Region

#Region " Constructors "

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub

#End Region

#Region " Methods "

    Public Overloads Overrides Function DoSearch() As IList(Of PECMailLog)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)

        Return criteria.List(Of PECMailLog)()
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of PECMailLogHeader)
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)
        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)
        SetProjectionsHeaders(criteria)
        Return criteria.List(Of PECMailLogHeader)()
    End Function

    Private Sub SetProjectionsHeaders(ByRef criteria As ICriteria)
        Dim proj As ProjectionList = Projections.ProjectionList()

        proj.Add(Projections.Property("PML.Id"), "Id") _
            .Add(Projections.Property("PML.Description"), "Description") _
            .Add(Projections.Property("PML.Type"), "Type") _
            .Add(Projections.Property("PML.Date"), "Date") _
            .Add(Projections.Property("PML.SystemComputer"), "SystemComputer") _
            .Add(Projections.Property("PML.SystemUser"), "SystemUser") _
            .Add(Projections.Property("PM.Id"), "MailId") _
            .Add(Projections.Property("PM.MailSubject"), "MailSubject")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(Transformers.AliasToBean(Of PECMailLogHeader))
    End Sub

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of PECMailLog)("PML")
        criteria.CreateAlias("PML.Mail", "PM")


        Return criteria
    End Function

    Private Sub innerDateTimeRange(ByRef rangeFrom As DateTime?, ByRef rangeTo As DateTime?)
        If Not (rangeFrom.HasValue AndAlso rangeTo.HasValue) Then
            Return
        End If
        If rangeTo.Value.AddDays(1).AddSeconds(-1) < rangeFrom Then
            Dim tempFrom As DateTime? = rangeFrom
            rangeFrom = rangeTo
            rangeTo = tempFrom
        End If
    End Sub

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)

        If PECMail IsNot Nothing Then
            criteria.Add(Restrictions.Eq("PML.Mail", PECMail))
        End If

        If PECMailIDIn IsNot Nothing AndAlso PECMailIDIn.Count > 0 Then
            criteria.Add(Restrictions.In("PM.Id", PECMailIDIn))
        End If

        If Not String.IsNullOrEmpty(LogType) Then
            criteria.Add(Restrictions.Eq("PML.Type", LogType))
        End If

        If Not String.IsNullOrEmpty(DescriptionLike) Then
            criteria.Add(Restrictions.Like("PML.Description", DescriptionLike, MatchMode.Anywhere))
        End If

        innerDateTimeRange(DateFrom, DateTo)
        If DateFrom.HasValue AndAlso DateTo.HasValue Then
            criteria.Add(Restrictions.Between("PML.Date", DateFrom, DateTo.Value.AddDays(1).AddSeconds(-1)))
        ElseIf DateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("PML.Date", DateFrom))
        ElseIf DateTo.HasValue Then
            criteria.Add(Restrictions.Le("PML.Date", DateTo.Value.AddDays(1).AddSeconds(-1)))
        End If

    End Sub

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachFilterExpressions(criteria)
        criteria.SetProjection(Projections.CountDistinct("PML.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

#End Region

End Class