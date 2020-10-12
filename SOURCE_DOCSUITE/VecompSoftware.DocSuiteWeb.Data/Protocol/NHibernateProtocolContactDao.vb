Imports System.Collections.Generic
Imports NHibernate.Transform
Imports VecompSoftware.Helpers
Imports NHibernate.Criterion
Imports NHibernate
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateProtocolContactDao
    Inherits BaseNHibernateDao(Of ProtocolContact)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    ''' <summary> Restituisce una lista di ProtocolContact filtrati per Year, Number e ComunicationType </summary>
    Public Function GetByComunicationType(uniqueIdProtocol As Guid, ByVal comunicationType As String) As IList(Of ProtocolContact)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.CreateAlias("Contact", "Contact")
        criteria.CreateAlias("Protocol", "Protocol")

        criteria.Add(Restrictions.Eq("Protocol.Id", uniqueIdProtocol))

        If (Not String.IsNullOrEmpty(comunicationType)) Then
            criteria.Add(Restrictions.Eq("ComunicationType", comunicationType))
        End If

        criteria.AddOrder(Order.Asc("Contact.Description"))

        Return criteria.List(Of ProtocolContact)()
    End Function

    Public Function GetCountByProtocol(uniqueIdProtocol As Guid, ByVal comunicationType As String) As Integer
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "Protocol")
        criteria.Add(Restrictions.Eq("Protocol.Id", uniqueIdProtocol))
        If (Not String.IsNullOrEmpty(comunicationType)) Then
            criteria.Add(Restrictions.Eq("ComunicationType", comunicationType))
        End If
        criteria.SetProjection(Projections.RowCount())
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of ProtocolContactJournalDTO)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.CreateAlias("Protocol", "P", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("Contact", "C", SqlCommand.JoinType.InnerJoin)

        If dateFrom.HasValue Then
            ' mi assicuro che parta dal primo tick della giornata
            Dim [from] As New Date(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day)
            criteria.Add(Restrictions.Ge("P.RegistrationDate", New DateTimeOffset([from])))
        End If

        If dateTo.HasValue Then
            ' mi assicuro che arrivi all'ultimo tick del giorno
            Dim [to] As New Date(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day)
            [to] = [to].AddDays(1).AddTicks(-1)
            criteria.Add(Restrictions.Le("P.RegistrationDate", New DateTimeOffset([to])))
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
        proj.Add(Projections.Property("C.Id"), "ContactId")
        proj.Add(Projections.Property("C.Description"), "ContactDescription")
        proj.Add(Projections.Property("C.FullIncrementalPath"), "ContactFullPath")

        criteria.SetProjection(proj)
        criteria.SetResultTransformer(New AliasToBeanResultTransformer(GetType(ProtocolContactJournalDTO)))

        Return criteria.List(Of ProtocolContactJournalDTO)()
    End Function

End Class
