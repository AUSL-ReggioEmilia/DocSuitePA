Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernatePECMailBoxDao
    Inherits BaseNHibernateDao(Of PECMailBox)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetIfIsInterop(ByVal isInterop As Boolean) As IList(Of PECMailBox)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.CreateAlias("Configuration", "Configuration", SqlCommand.JoinType.InnerJoin)
        crit.Add(Restrictions.Eq("IsForInterop", isInterop))
        Return crit.List(Of PECMailBox)()
    End Function

    Public Function GetIncomingMailBoxByIdHost(idHost As Guid, isDefault As Boolean) As IList(Of PECMailBox)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.CreateAlias("Configuration", "Configuration", SqlCommand.JoinType.InnerJoin)
        If isDefault Then
            crit.Add(Expression.Disjunction() _
                     .Add(Restrictions.Eq("IdJeepServiceIncomingHost", idHost)) _
                     .Add(Restrictions.IsNull("IdJeepServiceIncomingHost")))
        Else
            crit.Add(Restrictions.Eq("IdJeepServiceIncomingHost", idHost))
        End If

        crit.Add(Expression.Conjunction() _
                 .Add(Restrictions.Not(Restrictions.Eq("IncomingServerName", ""))) _
                 .Add(Restrictions.IsNotNull("IncomingServerName")))
        Return crit.List(Of PECMailBox)()
    End Function

    Public Function GetOutgoingMailBoxByIdHost(idHost As Guid, isDefault As Boolean) As IList(Of PECMailBox)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.CreateAlias("Configuration", "Configuration", SqlCommand.JoinType.InnerJoin)
        If isDefault Then
            crit.Add(Expression.Disjunction() _
                     .Add(Restrictions.Eq("IdJeepServiceOutgoingHost", idHost)) _
                     .Add(Restrictions.IsNull("IdJeepServiceOutgoingHost")))
        Else
            crit.Add(Restrictions.Eq("IdJeepServiceOutgoingHost", idHost))
        End If

        crit.Add(Expression.Conjunction() _
                 .Add(Restrictions.Not(Restrictions.Eq("OutgoingServerName", ""))) _
                 .Add(Restrictions.IsNotNull("OutgoingServerName")))
        Return crit.List(Of PECMailBox)()
    End Function

    Public Function GetIfIsSendingEnabled(ByVal protocolBoxOnly As Boolean) As IList(Of PECMailBox)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.IsNotNull("OutgoingServerName"))
        crit.Add(Restrictions.Eq("IsProtocolBox", protocolBoxOnly = True))
        Return crit.List(Of PECMailBox)()
    End Function

    Public Function GetByRecipient(ByVal address As String) As PECMailBox
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("MailBoxName", address))
        crit.Add(Restrictions.Eq("IsProtocolBox", False))
        Return crit.UniqueResult(Of PECMailBox)()
    End Function

    Public Function GetByRoles(roles As IList(Of Role)) As IList(Of PECMailBox)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.CreateAlias("MailBoxRoles", "MBR")
        crit.Add(Restrictions.In("MBR.Id.RoleId", roles.Select(Function(r) r.Id).ToArray()))
        crit.Add(Restrictions.Eq("IsProtocolBox", False))
        Return crit.List(Of PECMailBox)()
    End Function

    Public Function GetProtocolBoxes(ByVal protocolBoxOnly As Boolean) As IList(Of PECMailBox)
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("IsProtocolBox", protocolBoxOnly = True))
        Return crit.List(Of PECMailBox)()
    End Function

    Public Function CountManyPECMailsReceived(pecMailBoxId As Integer, fromDate As DateTime) As Integer
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(GetType(PECMail))
        crit.Add(Restrictions.Eq("MailBox.Id", Convert.ToInt16(pecMailBoxId)))
        crit.Add(Restrictions.Eq("Direction", Convert.ToInt16(PECMailDirection.Ingoing)))
        crit.Add(Restrictions.Ge("MailDate", fromDate))
        crit.SetProjection(Projections.RowCount())
        Return CType(crit.UniqueResult(), Integer)
    End Function

    Public Function CountPECMails(pecMailBoxId As Integer) As Integer
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(GetType(PECMail))
        crit.Add(Restrictions.Eq("MailBox.Id", Convert.ToInt16(pecMailBoxId)))
        crit.Add(Restrictions.Eq("IsActive", ActiveType.Cast(ActiveType.PECMailActiveType.Active)))
        crit.SetProjection(Projections.RowCount())
        Return CType(crit.UniqueResult(), Integer)
    End Function

    Public Function CountLoginErrorPECBoxes() As Integer
        Dim crit As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        crit.Add(Restrictions.Eq("LoginError", True))
        crit.SetProjection(Projections.RowCount())
        Return CType(crit.UniqueResult(), Integer)
    End Function
End Class
