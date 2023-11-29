Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionWorkflowUserFacade
    Inherits BaseResolutionFacade(Of ResolutionWorkflowUser, Guid, NHibernateResolutionWorkflowUserDao)

#Region " Constructors "

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
        Return _dao.GetUsersByResolution(resolution)
    End Function

    ''' <summary>
    ''' Recupera la lista di utenti per uno specifico atto relativamente ad uno specifico step
    ''' </summary>
    ''' <param name="resolution"></param>
    ''' <param name="currentStep"></param>
    ''' <returns></returns>
    Public Function GetUsersByResolutionAndStep(resolution As Resolution, currentStep As Short) As ICollection(Of ResolutionWorkflowUser)
        Return _dao.GetUsersByResolutionAndStep(resolution, currentStep)
    End Function
#End Region

End Class
