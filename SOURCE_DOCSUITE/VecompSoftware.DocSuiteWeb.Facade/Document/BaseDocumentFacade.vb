
Imports VecompSoftware.NHibernateManager.Dao

Public MustInherit Class BaseDocumentFacade(Of T, IdT, DaoType As INHibernateDao(Of T))
    Inherits FacadeNHibernateBase(Of T, IdT, DaoType)

    Private Delegate Sub DaoFunction(ByRef obj As T, ByVal DbName As String)

    Public Sub New()
        MyBase.New(DocmDB)
    End Sub

    Public Sub New(ByVal DBName As String)
        MyBase.New(DBName)
    End Sub

End Class