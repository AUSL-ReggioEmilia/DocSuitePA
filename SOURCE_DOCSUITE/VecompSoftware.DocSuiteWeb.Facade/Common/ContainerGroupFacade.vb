Imports System.Collections.Generic
Imports System.ComponentModel
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.DTO.Commons

<DataObject()>
Public Class ContainerGroupFacade
    Inherits CommonFacade(Of ContainerGroup, Guid, NHibernateContainerGroupDao)
    Implements IGroupFacade(Of ContainerGroup)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#End Region

    ''' <summary> Torna la lista di <see>ContainerGroup</see> inerente il <see>SecurityGroup</see> </summary>
    ''' <param name="securityGroup"> Gruppo da cercare </param>
    Public Function GetBySecurityGroup(ByRef securityGroup As SecurityGroups, Optional ordered As Boolean = False) As IList(Of ContainerGroup)
        Return _dao.GetBySecurityGroup(securityGroup, ordered)
    End Function


    Public Function GetByIdContainer(ByVal idContainer As Integer) As IList(Of ContainerGroup)
        Return _dao.GetByIdContainer(idContainer)
    End Function

    Public Function GetByContainerAndName(idContainer As Integer, groupName As String) As ContainerGroup
        Return _dao.GetByContainerAndName(idContainer, groupName)
    End Function

    Public Function GetByContainerAndGroups(idContainer As Integer, groups As String()) As IList(Of ContainerGroup)
        Return _dao.GetByContainerAndGroups(idContainer, groups)
    End Function

    Public Function GetByContainersAndGroup(idContainers As Integer(), group As String) As IList(Of ContainerGroup)
        Return _dao.GetByContainersAndGroup(idContainers, group)
    End Function

    Public Function GetMaxPrivacyLevel(idContainer As Integer, domain As String, account As String) As Integer
        Return _dao.GetMaxPrivacyLevel(idContainer, domain, account)
    End Function

    Public Function HasContainerRight(idContainer As Integer, domain As String, account As String, right As Integer, env As DSWEnvironment) As Boolean
        Return _dao.HasContainerRight(idContainer, domain, account, right, env)
    End Function

    Public Function DeleteGroup(ByRef obj As ContainerGroup) As Boolean Implements IGroupFacade(Of ContainerGroup).DeleteGroup
        Return Delete(obj)
    End Function

    ''' <summary> Traduce tutti i permessi </summary>
    Public Shared Function TraslitteraDiritti(environment As DSWEnvironment, ByVal group As ContainerGroup) As String
        Dim codedRights As String = Nothing
        Dim enumType As Type = Nothing
        Select Case environment
            Case DSWEnvironment.Protocol
                codedRights = group.ProtocolRightsString
                enumType = GetType(ProtocolContainerRightPositions)
            Case DSWEnvironment.Resolution
                codedRights = group.ResolutionRights
                enumType = GetType(ResolutionRightPositions)
            Case DSWEnvironment.Document
                codedRights = group.DocumentRights
                enumType = GetType(DocumentContainerRightPositions)
            Case DSWEnvironment.DocumentSeries
                codedRights = group.DocumentSeriesRights
                enumType = GetType(DocumentSeriesContainerRightPositions)
        End Select
        If String.IsNullOrEmpty(codedRights) OrElse enumType Is Nothing Then
            Return String.Empty
        End If

        Dim results As New List(Of String)
        For i As Integer = 1 To codedRights.Length
            If codedRights.Chars(i - 1) <> "1" Then
                Continue For
            End If

            Dim enumObject As Object = [Enum].ToObject(enumType, i)
            If enumObject Is Nothing Then
                Continue For
            End If
            results.Add(EnumHelper.GetDescription(CType(enumObject, [Enum])))
        Next i

        Return String.Join(", ", results)
    End Function

    Public Sub DeactivateUD(container As Data.Container)
        Dim groups As IList(Of ContainerGroup) = Factory.ContainerGroupFacade.GetByIdContainer(container.Id)
        If Not groups.IsNullOrEmpty() Then
            For Each group As ContainerGroup In groups
                If group.DocumentSeriesRights IsNot Nothing Then
                    CommonUtil.GetInstance.SetGroupRight(group.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Insert, False)
                    CommonUtil.GetInstance.SetGroupRight(group.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Modify, False)
                    CommonUtil.GetInstance.SetGroupRight(group.DocumentSeriesRights, DocumentSeriesContainerRightPositions.Cancel, False)
                End If
                If group.DocumentRights IsNot Nothing Then
                    CommonUtil.GetInstance.SetGroupRight(group.DocumentRights, DocumentRightPositions.Insert, False)
                    CommonUtil.GetInstance.SetGroupRight(group.DocumentRights, DocumentRightPositions.Modify, False)
                    CommonUtil.GetInstance.SetGroupRight(group.DocumentRights, DocumentRightPositions.Delete, False)
                End If
                If group.ResolutionRights IsNot Nothing Then
                    CommonUtil.GetInstance.SetGroupRight(group.ResolutionRights, ResolutionRightPositions.Insert, False)
                    CommonUtil.GetInstance.SetGroupRight(group.ResolutionRights, ResolutionRightPositions.Cancel, False)
                End If
                If group.ProtocolRightsString IsNot Nothing Then
                    CommonUtil.GetInstance.SetGroupRight(group.ProtocolRightsString, ProtocolContainerRightPositions.Insert, False)
                    CommonUtil.GetInstance.SetGroupRight(group.ProtocolRightsString, ProtocolContainerRightPositions.Modify, False)
                    CommonUtil.GetInstance.SetGroupRight(group.ProtocolRightsString, ProtocolContainerRightPositions.Cancel, False)
                End If
                If group.DeskRights IsNot Nothing Then
                    CommonUtil.GetInstance.SetGroupRight(group.DeskRights, DeskRightPositions.Insert, False)
                End If
                If group.UDSRights IsNot Nothing Then
                    CommonUtil.GetInstance.SetGroupRight(group.UDSRights, UDSRightPositions.Insert, False)
                    CommonUtil.GetInstance.SetGroupRight(group.UDSRights, UDSRightPositions.Modify, False)
                End If
                Update(group)
            Next
        End If
    End Sub
    Public Overrides Sub Save(ByRef entity As ContainerGroup)
        Dim cId As Guid = entity.Container.UniqueId
        Dim gName As String = entity.Name
        Dim right As String = entity.ProtocolRightsString
        Dim reslRight As String = entity.ResolutionRights
        Dim dcmRight As String = entity.DocumentRights
        Dim dcmSeriesRight As String = entity.DocumentSeriesRights
        Dim deskRight As String = entity.DeskRights
        Dim udsRight As String = entity.UDSRights
        Dim privacyLevel As String = entity.PrivacyLevel.ToString()
        FacadeFactory.Instance.TableLogFacade.Insert("ContainerGroup", LogEvent.PR, String.Format("Inserito Gruppo con livello di privacy", privacyLevel), cId)
        FacadeFactory.Instance.TableLogFacade.Insert("ContainerGroup", LogEvent.INS, String.Format("Inserito Gruppo {0} con Rights {1}, ResolutionRights {2}, DocumentRights {3}, DocumentSeriesRights {4}, DeskRights {5}, UDSRights {6}, PrivacyLevel {7}", gName, right, reslRight, dcmRight, dcmSeriesRight, deskRight, udsRight, privacyLevel), cId)
        MyBase.Save(entity)
    End Sub
    Public Overrides Sub UpdateOnly(ByRef entity As ContainerGroup)
        Dim cId As Guid = entity.Container.UniqueId
        Dim gName As String = entity.Name
        Dim right As String = entity.ProtocolRightsString
        Dim reslRight As String = entity.ResolutionRights
        Dim dcmRight As String = entity.DocumentRights
        Dim dcmSeriesRight As String = entity.DocumentSeriesRights
        Dim deskRight As String = entity.DeskRights
        Dim udsRight As String = entity.UDSRights
        Dim privacyLevel As String = entity.PrivacyLevel.ToString()
        FacadeFactory.Instance.TableLogFacade.Insert("ContainerGroup", LogEvent.UP, String.Format("Modificato Gruppo {0} con Rights {1}, ResolutionRights {2}, DocumentRights {3}, DocumentSeriesRights {4}, DeskRights {5}, UDSRights {6}, PrivacyLevel {7}", gName, right, reslRight, dcmRight, dcmSeriesRight, deskRight, udsRight, privacyLevel), cId)
        MyBase.UpdateOnly(entity)
    End Sub


End Class
