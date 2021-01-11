Imports System
Imports System.Collections.Generic
Imports NHibernate.Transform
Imports VecompSoftware.Helpers
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolContactManualDao
    Inherits BaseNHibernateDao(Of ProtocolContactManual)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByComunicationType(uniqueIdProtocol As Guid, ByVal type As String) As IList(Of ProtocolContactManual)
        criteria = NHibernateSession.CreateCriteria(Of ProtocolContactManual)()
        criteria.CreateAlias("Protocol", "P", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("P.Id", uniqueIdProtocol))
        If Not String.IsNullOrEmpty(type) Then
            criteria.Add(Restrictions.Eq("ComunicationType", type))
        End If
        criteria.AddOrder(Order.Asc("Contact.FullIncrementalPath"))
        criteria.AddOrder(Order.Asc("Contact.Description"))

        Return criteria.List(Of ProtocolContactManual)()
    End Function

    Public Function GetCountByProtocol(uniqueIdProtocol As Guid, ByVal comunicationType As String) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("P.Id", uniqueIdProtocol))
        If (Not String.IsNullOrEmpty(comunicationType)) Then
            criteria.Add(Restrictions.Eq("ComunicationType", comunicationType))
        End If
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Function GetCountByContactTitle(ByVal contactTitle As ContactTitle) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Contact.StudyTitle.Id", contactTitle.Id))
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of ProtocolContactJournalDTO)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P", SqlCommand.JoinType.InnerJoin)

        If dateFrom.HasValue Then
            criteria.Add(Restrictions.Ge("P.RegistrationDate", New DateTimeOffset(dateFrom.Value)))
        End If

        If dateTo.HasValue Then
            criteria.Add(Restrictions.Le("P.RegistrationDate", New DateTimeOffset(dateTo.Value)))
        End If

        If idStatus.HasValue Then
            criteria.Add(Restrictions.Eq("P.IdStatus", idStatus.Value))
        End If

        criteria.Add(Expression.In("P.Container.Id", StringHelper.ConvertStringToList(Of Integer)(idContainers, ","c)))

        criteria.AddOrder(Order.Asc("P.RegistrationDate"))
        criteria.AddOrder(Order.Asc("P.Number"))

        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("P.Year"), "Year")
        proj.Add(Projections.Property("P.Number"), "Number")
        proj.Add(Projections.Property("Contact.Description"), "ContactDescription")
        proj.Add(Projections.Property("Contact.FullIncrementalPath"), "ContactFullPath")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(New AliasToBeanResultTransformer(GetType(ProtocolContactJournalDTO)))

        Return criteria.List(Of ProtocolContactJournalDTO)()
    End Function

    Public Function GetByProtocolAndIncremental(uniqueIdProtocol As Guid, incremental As Integer) As ProtocolContactManual
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P", SqlCommand.JoinType.InnerJoin)
        criteria.Add(Restrictions.Eq("P.Id", uniqueIdProtocol))
        criteria.Add(Restrictions.Eq("ComunicationType", incremental))
        Return criteria.UniqueResult(Of ProtocolContactManual)()
    End Function
End Class
