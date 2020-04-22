
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject>
Public Class ProtocolRoleUserFacade
    Inherits BaseProtocolFacade(Of ProtocolRoleUser, Guid, NHibernateProtocolRoleUserDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Public Function GetByUniqueIdProtocolAndAccount(uniqueIdProtocol As Guid, account As String) As IList(Of ProtocolRoleUser)
        Return _dao.GetByUniqueIdProtocolAndAccount(uniqueIdProtocol, account)
    End Function

#End Region
End Class
