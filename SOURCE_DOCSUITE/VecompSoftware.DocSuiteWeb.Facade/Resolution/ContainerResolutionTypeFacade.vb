Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class ContainerResolutionTypeFacade
    Inherits BaseResolutionFacade(Of ContainerResolutionType, ContainerResolutionTypeCompositeKey, NHibernateContainerResolutionTypeDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Restituisce una lista di Containers associati ad un determinato ResolutionType. </summary>
    Function GetAllowedContainers(resolutionTypeId As Short, ByVal isActive As Short, ByVal rights As ResolutionRightPositions?, Optional accounting As Boolean? = Nothing) As IList(Of ContainerResolutionType)
        Dim groups As String()
        Dim securityGroups As IList(Of SecurityGroups) = FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
        If securityGroups Is Nothing Then
            Return New List(Of ContainerResolutionType)
        End If

        groups = securityGroups.Select(Function(g) g.Id.ToString()).ToArray()

        If groups Is Nothing OrElse groups.Length = 0 Then
            Return New List(Of ContainerResolutionType)
        End If

        Return _dao.GetAllowedContainers(resolutionTypeId, groups, isActive, rights, accounting)
    End Function

End Class
