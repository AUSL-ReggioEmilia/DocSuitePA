Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionWorkflowUserDao
    Inherits BaseNHibernateDao(Of ResolutionWorkflowUser)

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    ''' <summary>
    ''' Recupera la lista di utenti per uno specifico atto relativamente allo step attivo
    ''' </summary>
    ''' <param name="resolution"></param>
    ''' <returns></returns>
    Public Function GetUsersByResolution(resolution As Resolution) As ICollection(Of ResolutionWorkflowUser)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionWorkflowUser)
        criteria.CreateAlias("ResolutionWorkflow", "RW")
        criteria.CreateAlias("RW.Resolution", "R")
        criteria.Add(Restrictions.Eq("RW.IsActive", 1S))
        criteria.Add(Restrictions.Eq("R.Id", resolution.Id))
        Return criteria.List(Of ResolutionWorkflowUser)
    End Function

    ''' <summary>
    ''' Recupera la lista di utenti per uno specifico atto relativamente ad uno specifico step
    ''' </summary>
    ''' <param name="resolution"></param>
    ''' <param name="currentStep"></param>
    ''' <returns></returns>
    Public Function GetUsersByResolutionAndStep(resolution As Resolution, currentStep As Short) As ICollection(Of ResolutionWorkflowUser)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of ResolutionWorkflowUser)
        criteria.CreateAlias("ResolutionWorkflow", "RW")
        criteria.CreateAlias("RW.Resolution", "R")
        criteria.Add(Restrictions.Eq("RW.ResStep", currentStep))
        criteria.Add(Restrictions.Eq("R.Id", resolution.Id))
        Return criteria.List(Of ResolutionWorkflowUser)
    End Function
#End Region

End Class
