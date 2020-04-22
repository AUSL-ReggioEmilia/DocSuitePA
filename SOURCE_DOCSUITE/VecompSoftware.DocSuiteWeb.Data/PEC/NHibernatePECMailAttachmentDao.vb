Imports System.Collections.Generic
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernatePECMailAttachmentDao
    Inherits BaseNHibernateDao(Of PECMailAttachment)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByMail(ByVal idMail As Integer) As IList(Of PECMailAttachment)
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Mail.Id", idMail))

        Return criteria.List(Of PECMailAttachment)()
    End Function

End Class
