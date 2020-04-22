Imports System.Collections.Generic
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibernateResolutionStatusDao
    Inherits BaseNHibernateDao(Of ResolutionStatus)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce un elenco di resolutionStatus il cui Id non è compreso nell'insieme passato. </summary>
    ''' <param name="exceptionList">Array di Id che non devono essere restituiti</param>
    Public Function GetByExceptionList(ByVal exceptionList As Short()) As IList(Of ResolutionStatus)
        criteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Expression.Not(Expression.In("Id", exceptionList)))

        Return criteria.List(Of ResolutionStatus)()
    End Function

End Class
