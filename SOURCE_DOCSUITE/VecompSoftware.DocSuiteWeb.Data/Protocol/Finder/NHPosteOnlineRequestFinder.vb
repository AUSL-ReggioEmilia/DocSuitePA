Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports NHibernate.SqlCommand
Imports NHibernate.Transform

<Serializable(), DataObject()> _
Public Class NHPosteOnlineRequestFinder
    Inherits NHibernateBaseFinder(Of POLRequestRecipient, POLRequestRecipientHeader)

#Region " Properties "
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property Sent As Boolean?
    Public Property DateSentFrom As DateTime?

    Public Property DateSentTo As DateTime?

    Public Property Type As POLRequestType?

    Public Property Accounts As IList(Of Integer)

    Public Property TaskHeaderIdIn As IEnumerable(Of Integer)

    Protected Property Criteria As ICriteria

#End Region

#Region "Constuctor"

    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

    Public Sub New()
        Me.New("ProtDB")
    End Sub

#End Region

#Region " Methods "

    Public Overloads Overrides Function DoSearch() As IList(Of POLRequestRecipient)
        Dim criteria As ICriteria = CreateCriteria()

        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)

        Return criteria.List(Of POLRequestRecipient)()
    End Function

    Public Overloads Overrides Function DoSearchHeader() As IList(Of POLRequestRecipientHeader)
        Dim criteria As ICriteria = CreateCriteria()

        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        AttachFilterExpressions(criteria)
        SetProjectionHeaders(criteria)

        Return criteria.List(Of POLRequestRecipientHeader)()
    End Function

    Protected Overrides Function CreateCriteria() As ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of POLRequestRecipient)("PRR")
        criteria.CreateCriteria("PRR.Request", "R", JoinType.InnerJoin)

        If Sent.HasValue Then
            If Sent Then
                criteria.Add(Restrictions.IsNotNull("PRR.DataSpedizione"))
            Else
                criteria.Add(Restrictions.IsNull("PRR.DataSpedizione"))
            End If
        End If
        If (Not Sent.HasValue OrElse (Sent.HasValue AndAlso Not Sent.Value)) AndAlso DateSentFrom.HasValue Then
            criteria.Add(Restrictions.Ge("PRR.DataSpedizione", DateSentFrom.Value.BeginOfTheDay()))
        End If
        If (Not Sent.HasValue OrElse (Sent.HasValue AndAlso Not Sent.Value)) AndAlso DateSentTo.HasValue Then
            criteria.Add(Restrictions.Le("PRR.DataSpedizione", DateSentTo.Value.EndOfTheDay()))
        End If
        If Type.HasValue Then
            criteria.Add(Restrictions.Eq("R.Type", Type.Value))
        End If
        If Not Accounts Is Nothing Then
            criteria.Add(Restrictions.In("R.Account.Id", Accounts.ToArray()))
        End If

        If DocSuiteContext.Current.ProtocolEnv.FastProtocolSenderEnabled AndAlso Not TaskHeaderIdIn.IsNullOrEmpty() Then
            Dim dcTaskHeader As DetachedCriteria = DetachedCriteria.For(Of TaskHeaderPOLRequest)("THPQ")
            dcTaskHeader.Add(Restrictions.EqProperty("THPQ.POLRequest.Id", "PRR.Request.Id"))

            dcTaskHeader.Add(Restrictions.In("THPQ.Header.Id", TaskHeaderIdIn.ToArray()))
            dcTaskHeader.SetProjection(Projections.Constant(True))

            criteria.Add(Subqueries.Exists(dcTaskHeader))
        End If

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria
    End Function

    Protected Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)


    End Sub

    Public Shared Sub SetProjectionPOLRequestRecipientHeader(ByRef criteria As ICriteria)
        Dim proj As ProjectionList = Projections.ProjectionList()

        Dim senderCriteria As DetachedCriteria = DetachedCriteria.For(Of POLRequestSender)("PRS")
        With senderCriteria
            .Add(Restrictions.EqProperty("PRR.Request", "PRS.Request"))
            .SetMaxResults(1)
            .SetProjection(Projections.Property("PRS.Name"))
        End With

        Dim LOLRequestCriteria As DetachedCriteria = DetachedCriteria.For(Of LOLRequest)("LOLR")
        With LOLRequestCriteria
            .Add(Restrictions.EqProperty("PRR.Request", "LOLR.Id"))
            .SetMaxResults(1)
            .SetProjection(Projections.Property("LOLR.DocumentName"))
        End With

        Dim ROLRequestCriteria As DetachedCriteria = DetachedCriteria.For(Of ROLRequest)("ROLR")
        With ROLRequestCriteria
            .Add(Restrictions.EqProperty("PRR.Request", "ROLR.Id"))
            .SetMaxResults(1)
            .SetProjection(Projections.Property("ROLR.DocumentName"))
        End With

        Dim TOLRequestCriteria As DetachedCriteria = DetachedCriteria.For(Of TOLRequest)("TOLR")
        With TOLRequestCriteria
            .Add(Restrictions.EqProperty("PRR.Request", "TOLR.Id"))
            .SetMaxResults(1)
            .SetProjection(Projections.Property("TOLR.Testo"))
        End With

        Dim SOLRequestCriteria As DetachedCriteria = DetachedCriteria.For(Of SOLRequest)("SOLR")
        With SOLRequestCriteria
            .Add(Restrictions.EqProperty("PRR.Request", "SOLR.Id"))
            .SetMaxResults(1)
            .SetProjection(Projections.Property("SOLR.DocumentName"))
        End With

        proj.Add(Projections.SubQuery(senderCriteria), "SenderName")
        proj.Add(Projections.SubQuery(LOLRequestCriteria), "LOLRequestDocumentName")
        proj.Add(Projections.SubQuery(ROLRequestCriteria), "ROLRequestDocumentName")
        proj.Add(Projections.SubQuery(TOLRequestCriteria), "TOLRequestTesto")
        proj.Add(Projections.SubQuery(SOLRequestCriteria), "SOLRequestDocumentName")

        proj.Add(Projections.Property("R.GuidPoste"), "GuidPoste")
        proj.Add(Projections.Property("R.ErrorMsg"), "ErrorMsg")
        proj.Add(Projections.Property("R.CostoTotale"), "CostoTotale")
        proj.Add(Projections.Property("R.RegistrationDate"), "RegistrationDate")
        proj.Add(Projections.Property("R.StatusDescrition"), "RequestStatusDescrition")
        proj.Add(Projections.Property("R.Type"), "RequestType")
        proj.Add(Projections.Property("R.ProtocolYear"), "ProtocolYear")
        proj.Add(Projections.Property("R.ProtocolNumber"), "ProtocolNumber")
        proj.Add(Projections.Property("R.IdRichiesta"), "IdRichiesta")
        proj.Add(Projections.Property("R.IdOrdine"), "IdOrdine")

        proj.Add(Projections.Property("Name"), "Name")
        proj.Add(Projections.Property("DataSpedizione"), "DataSpedizione")
        proj.Add(Projections.Property("StatusDescrition"), "StatusDescrition")
        proj.Add(Projections.Property("IdRicevuta"), "IdRicevuta")
        proj.Add(Projections.Property("Costo"), "Costo")

        criteria.SetProjection(Projections.Distinct(proj))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of POLRequestRecipientHeader))
    End Sub

    Protected Overridable Sub SetProjectionHeaders(ByRef criteria As ICriteria)
        SetProjectionPOLRequestRecipientHeader(criteria)
    End Sub


    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        AttachFilterExpressions(criteria)

        criteria.SetProjection(Projections.CountDistinct("PRR.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

#End Region

End Class
