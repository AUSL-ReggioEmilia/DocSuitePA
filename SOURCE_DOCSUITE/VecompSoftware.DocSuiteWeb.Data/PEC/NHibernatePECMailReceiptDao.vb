Imports System
Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECMailReceiptDao
    Inherits BaseNHibernateDao(Of PECMailReceipt)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetPECReceipts(pec As PECMail) As IList(Of PECMailReceipt)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("MSGID", pec.XRiferimentoMessageID))
        criteria.AddOrder(Order.Asc("ReceiptDate"))
        Return criteria.List(Of PECMailReceipt)()
    End Function

End Class
