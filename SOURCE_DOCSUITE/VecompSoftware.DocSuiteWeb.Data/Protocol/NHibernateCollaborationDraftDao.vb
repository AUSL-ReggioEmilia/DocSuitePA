Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateCollaborationDraftDao
    Inherits BaseNHibernateDao(Of CollaborationDraft)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByCollaboration(coll As Collaboration) As CollaborationDraft
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Collaboration.Id", coll.Id))
        Return criteria.UniqueResult(Of CollaborationDraft)()
    End Function

    Public Function GetByIdDocumentUnit(idDocumentUnit As Guid) As CollaborationDraft
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("IdDocumentUnit", idDocumentUnit))
        Return criteria.UniqueResult(Of CollaborationDraft)()
    End Function

End Class
