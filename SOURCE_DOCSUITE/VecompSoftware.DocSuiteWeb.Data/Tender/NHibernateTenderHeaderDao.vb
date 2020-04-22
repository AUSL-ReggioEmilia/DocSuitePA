Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports NHibernate.Criterion
Imports NHibernate.Transform
Imports System.Linq

Public Class NHibernateTenderHeaderDao
    Inherits BaseNHibernateDao(Of TenderHeader)

    Private Const FactoryName As String = "ProtDB"

    Public Function GetByCIG(cig As String) As TenderHeader
        'Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
        'Dim criteria As ICriteria = session.CreateCriteria(Of TenderHeader)()
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TenderHeader)()

        criteria.CreateAlias("Lots", "L")
        criteria.CreateAlias("L.Payments", "LP")

        criteria.CreateAlias("Lots", "L")
        criteria.Add(Restrictions.Eq("L.CIG", cig))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        criteria.SetMaxResults(1)

        Return criteria.UniqueResult(Of TenderHeader)()
        'End Using
    End Function

    Public Function SetPayment(header As TenderHeader, cig As String, key As String, importo As Decimal) As TenderHeader
        ' Cerco il lotto
        Dim lotto As TenderLot = header.Lots.FirstOrDefault(Function(l) l.CIG.Trim().Equals(cig.Trim()))

        If lotto Is Nothing Then
            Throw New DocSuiteException("CIG non trovato: " + cig)
        End If

        Dim payment As TenderLotPayment = lotto.Payments.FirstOrDefault(Function(p) p.PaymentKey.Equals(key))

        If payment Is Nothing Then
            payment = New TenderLotPayment()
            payment.PaymentKey = key
            payment.Amount = importo

            payment.RegistrationDate = DateTimeOffset.UtcNow
            payment.RegistrationUser = DocSuiteContext.Current.User.FullUserName

            lotto.AddPayment(payment)
        Else
            ' Aggiorno il valore
            payment.Amount = importo
        End If

        payment.LastChangedDate = DateTimeOffset.UtcNow
        payment.LastChangedUser = DocSuiteContext.Current.User.FullUserName

        Return header
    End Function

    Public Function GetByResolution(resl As Resolution) As TenderHeader

        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
            Dim criteria As ICriteria = session.CreateCriteria(Of TenderHeader)("T")
            criteria.CreateAlias("Lots", "L")
            criteria.CreateAlias("L.Payments", "LP")
            criteria.Add(Restrictions.Eq("IdResolution", resl.Id))
            criteria.SetResultTransformer(Transformers.DistinctRootEntity)

            Return criteria.UniqueResult(Of TenderHeader)()
        End Using
    End Function

    Public Function GetChangedHeaders(dateFrom As DateTime?) As IList(Of TenderHeader)

        'Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TenderHeader)("TH")

        'Dim criteria As ICriteria = session.CreateCriteria(Of TenderHeader)("T")

        criteria.CreateAlias("TH.Lots", "L")
        criteria.CreateAlias("L.Payments", "LP")

        criteria.Add(Restrictions.Ge("TH.LastChangedDate", dateFrom))

        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        Return criteria.List(Of TenderHeader)()

        'End Using

    End Function


    Public Function GetByDocumentSeriesItem(idDocumentSeriesItem As Integer) As TenderHeader

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TenderHeader)()


        criteria.CreateAlias("DocumentSeriesItem", "DSI")
        criteria.Add(Restrictions.Eq("DSI.Id", idDocumentSeriesItem))
        criteria.SetResultTransformer(Transformers.DistinctRootEntity)

        criteria.SetMaxResults(1)

        Return criteria.UniqueResult(Of TenderHeader)()

    End Function


End Class
