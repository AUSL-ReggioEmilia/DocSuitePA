
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ContainerDocTypeFacade
    Inherits CommonFacade(Of ContainerDocType, Integer, NHibernateContainerDocTypeDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

    Public Function DeleteGroup(ByRef obj As ContainerDocType) As Boolean
        Return MyBase.Delete(obj)
    End Function

    Public Function ContainerDocTypeSearch(ByVal p_idContainer As Integer, Optional ByVal p_isActive As Boolean = False) As IList(Of DocumentType)
        Return _dao.ContainerDocTypeSearch(p_idContainer, p_isActive)
    End Function

End Class
