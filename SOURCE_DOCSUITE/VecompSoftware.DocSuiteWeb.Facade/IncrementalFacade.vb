Imports VecompSoftware.DocSuiteWeb.Data

Public Class IncrementalFacade
    Inherits FacadeNHibernateBase(Of Incremental, Guid, NHibenateIncrementalDao)

    Public Function GetFor(Of T)() As Incremental
        Return _dao.GetFor(Of T)()
    End Function

End Class
