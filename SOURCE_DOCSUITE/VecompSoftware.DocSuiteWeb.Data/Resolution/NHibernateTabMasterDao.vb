Imports System
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateTabMasterDao
    Inherits BaseNHibernateDao(Of TabMaster)
    
    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Save(ByRef entity As TabMaster)
        MyBase.Save(entity)
        NHibernateSession.SessionFactory.Evict(GetType(TabMaster))
    End Sub

    Public Overrides Sub Update(ByRef entity As TabMaster)
        MyBase.Update(entity)
        NHibernateSession.SessionFactory.Evict(GetType(TabMaster))
    End Sub

    Public Overrides Sub UpdateNoLastChange(ByRef entity As TabMaster)
        MyBase.UpdateNoLastChange(entity)
        NHibernateSession.SessionFactory.Evict(GetType(TabMaster))
    End Sub

    Public Overrides Sub UpdateOnly(ByRef entity As TabMaster)
        MyBase.UpdateOnly(entity)
        NHibernateSession.SessionFactory.Evict(GetType(TabMaster))
    End Sub

    Public Overrides Sub Delete(ByRef entity As TabMaster)
        MyBase.Delete(entity)
        NHibernateSession.SessionFactory.Evict(GetType(TabMaster))
    End Sub

    ''' <summary> Restituisce il valore del campo specificato. </summary>
    ''' <param name="fieldName">nome del campo da recuperare</param>
    Public Function GetFieldValue(ByVal fieldName As String, ByVal configuration As String, ByVal reslType As Short) As String
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Configuration", configuration))
        criteria.Add(Restrictions.Eq("Id.ResolutionType", Convert.ToInt16(reslType)))

        criteria.SetProjection(Projections.Property(fieldName))
        criteria.SetMaxResults(1)
        
        Return criteria.UniqueResult(Of String)()
    End Function

    Public Function GetByConfigurationAndType(ByVal configuration As String, ByVal resolutionType As Short) As TabMaster
        criteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Configuration", configuration))
        criteria.Add(Restrictions.Eq("Id.ResolutionType", Convert.ToInt16(resolutionType)))

        criteria.SetMaxResults(1)

        Return criteria.UniqueResult(Of TabMaster)()
    End Function


End Class
