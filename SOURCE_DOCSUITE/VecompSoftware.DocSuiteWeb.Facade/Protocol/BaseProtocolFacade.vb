Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao

Public MustInherit Class BaseProtocolFacade(Of T, TIdT, TDaoType As INHibernateDao(Of T))
    Inherits FacadeNHibernateBase(Of T, TIdT, TDaoType)

    Public Sub New()
        MyBase.New()
        _dbName = ProtDB
        _unitOfWork = New NHibernateUnitOfWork(_dbName)
    End Sub

    Public Sub New(factory As FacadeFactory)
        MyBase.New(factory)
    End Sub

    Public Sub New(ByVal dbName As String, factory As FacadeFactory)
        MyBase.New(dbName, factory)
    End Sub

End Class