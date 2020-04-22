Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateContactTypeDao
    Inherits BaseNHibernateDao(Of ContactType)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Save(ByRef entity As ContactType)
        MyBase.Save(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(ContactType))
    End Sub

    Public Overrides Sub Update(ByRef entity As ContactType)
        MyBase.Update(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(ContactType))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As ContactType)
        MyBase.UpdateNoLastChange(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(ContactType))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As ContactType)
        MyBase.UpdateOnly(entity)
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(ContactType))
    End Sub

    Public Overrides Sub Delete(ByRef entity As ContactType)
        MyBase.Delete(entity)
        'MyBase.NHibernateSession.SessionFactory.Evict(entity.GetType())
        MyBase.NHibernateSession.SessionFactory.Evict(GetType(ContactType))
    End Sub

End Class
