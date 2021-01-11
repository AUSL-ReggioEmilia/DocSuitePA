Imports System.Collections.Generic
Imports System.Text
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data

Public Class SecurityGroupsFacade
    Inherits CommonFacade(Of SecurityGroups, Integer, NHibernateSecurityGroupsDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

#End Region

#Region " Overrides "

    Public Overrides Sub Save(ByRef obj As SecurityGroups)
        obj.Id = _dao.GetMaxId() + 1
        obj.IdSecurityGroupTenant = obj.Id
        obj.TenantId = DocSuiteContext.Current.CurrentTenant.TenantId
        CalculateFullIncremental(obj)
        MyBase.Save(obj)
    End Sub

    ''' <summary> Cancella il <see>SecurityGroups</see> controllando che non sia legato ad alcun <see>Container</see>. </summary>
    Public Overrides Function Delete(ByRef securityGroups As SecurityGroups) As Boolean
        Dim daoCompatibleGroupsList As String = GroupsNameList(securityGroups)

        ' Controllo che non sia legato a contenitori
        Dim containers As New List(Of Container)(Factory.ContainerFacade.GetContainerRigths(ProtDB, daoCompatibleGroupsList, 2))
        If DocSuiteContext.Current.IsDocumentEnabled Then
            containers.AddRange(Factory.ContainerFacade.GetContainerRigths(DocmDB, daoCompatibleGroupsList, 2))
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            containers.AddRange(Factory.ContainerFacade.GetContainerRigths(ReslDB, daoCompatibleGroupsList, 2))
        End If

        If (containers.Count > 0) Then
            Dim containerNames As New StringBuilder
            For Each container As Container In containers
                containerNames.AppendFormat("'{0}' ", container.Name)
            Next

            Throw New DocSuiteException("Errore cancellazione gruppo", "Gruppo non cancellabile. Viene usato dai seguenti contenitori: " & containerNames.ToString())
        End If

        ' Controllo che non sia legato a settori
        Dim roles As New List(Of Role)(Factory.RoleFacade.GetRolesBySG(DSWEnvironment.Protocol, {securityGroups}, Nothing, Nothing, String.Empty, Nothing))

        If DocSuiteContext.Current.IsDocumentEnabled Then
            roles.AddRange(Factory.RoleFacade.GetRolesBySG(DSWEnvironment.Document, {securityGroups}, Nothing, Nothing, String.Empty, Nothing))
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            roles.AddRange(Factory.RoleFacade.GetRolesBySG(DSWEnvironment.Resolution, {securityGroups}, Nothing, Nothing, String.Empty, Nothing))
        End If

        If (roles.Count > 0) Then
            Dim roleNames As New StringBuilder
            For Each role As Role In roles
                roleNames.AppendFormat("'{0}' ", role.Name)
            Next

            Throw New DocSuiteException("Errore cancellazione gruppi", "Gruppo non cancellabile. Viene usato dai seguenti settori: " & roleNames.ToString())
        End If

        ' Cancello gli utenti collegati
        Dim securityUserFacade As New SecurityUsersFacade
        For Each user As SecurityUsers In securityGroups.SecurityUsers
            securityUserFacade.Delete(user)
        Next

        Return MyBase.Delete(securityGroups)
    End Function

    ''' <summary>
    ''' Aggiorna i security groups e gli oggetti collegati.
    ''' </summary>
    ''' <remarks>
    ''' ATTENZIONE: Esegue SEMPRE l'aggiornamento di tutti gli oggetti collegati.
    ''' Questo per prevenire errori di sincronizzazione sul nome del gruppo
    ''' che dev'essere compatibile con SecurityGroup non abilitato.
    ''' </remarks>
    Public Overrides Sub UpdateOnly(ByRef group As SecurityGroups)
        ' Aggiorno i settori
        Dim roles As IList(Of RoleGroup) = Factory.RoleGroupFacade.GetBySecurityGroup(group)
        For Each roleGroup As RoleGroup In roles
            Dim newRg As RoleGroup = DirectCast(roleGroup.Clone(), RoleGroup)
            Factory.RoleGroupFacade.Delete(roleGroup)
            newRg.Name = group.GroupName
            Factory.RoleGroupFacade.Update(newRg)
        Next

        ' Aggiorno i contenitori
        Dim containers As IList(Of ContainerGroup) = Factory.ContainerGroupFacade.GetBySecurityGroup(group)
        For Each containerGroup As ContainerGroup In containers
            Dim newCg As ContainerGroup = DirectCast(containerGroup.Clone(), ContainerGroup)
            Factory.ContainerGroupFacade.Delete(containerGroup)
            newCg.Name = group.GroupName
            Factory.ContainerGroupFacade.Update(newCg)
        Next

        MyBase.UpdateOnly(group)

    End Sub

#End Region

    ''' <summary> Calcola e imposta il FullIncrementalPath di un oggetto SecurityGroups. </summary>
    Private Shared Sub CalculateFullIncremental(ByRef obj As SecurityGroups)
        If obj.Parent Is Nothing Then
            obj.FullIncrementalPath = obj.IdSecurityGroupTenant.ToString()
        Else
            obj.FullIncrementalPath = String.Format("{0}|{1}", obj.Parent.FullIncrementalPath, obj.IdSecurityGroupTenant)
        End If
    End Sub

    Public Function GetGroupByName(groupName As String) As SecurityGroups
        Return _dao.GetGroupByName(groupName)
    End Function

    ''' <summary> Restituisce tutti i gruppi ROOT (senza padre). </summary>
    Public Function GetRootGroups(name As String) As IList(Of SecurityGroups)
        Return _dao.GetRootGroups(name)
    End Function

    ''' <summary> Restituisce tutti i gruppi. </summary>
    Public Function GetGroupsFlat(name As String) As IList(Of SecurityGroups)
        Return _dao.GetGroupsFlat(name)
    End Function

    ''' <summary> Converte un <see>SecurityGroups</see> in una stringa con il nome del gruppo utile alla DAO. </summary>
    Public Shared Function GroupsNameList(ByVal group As SecurityGroups) As String
        Dim groups As New List(Of SecurityGroups)
        groups.Add(group)
        Return GroupsNameList(groups)
    End Function

    ''' <summary>
    ''' Converte un'insieme di <see>SecurityGroup</see> o <see>ADGroup</see> in una stringa contenente una lista di nomi gruppo utile alla DAO
    ''' </summary>
    ''' <param name="groups">Lista di <see>SecurityGroup</see> o di <see>ADGroup</see> </param>
    ''' <returns>Se nulla o vuota torna stringa vuota</returns>
    ''' <remarks>
    ''' Non andrebbe qui questo metodo ma dato che sono i SecurityGroups ad essere persistiti, temporaneamente può essere messa qui.
    ''' In mancanza di una visione dei gruppi come servizio, da usare tramite interfaccia comune, i due oggetti devono avere un
    ''' ToString che può andare nella [ContainerGroup].[GroupName].
    ''' </remarks>
    Public Shared Function GroupsNameList(ByVal groups As IList) As String
        Dim founded As New List(Of String)

        If Not groups Is Nothing Then
            For i As Integer = 0 To groups.Count - 1
                founded.Add(String.Format("'{0}'", groups(i).ToString))
            Next
        End If

        Return String.Join(",", founded)
    End Function

    Public Function GetByUser(account As String, domain As String) As IList(Of SecurityGroups)
        Return _dao.GetByUser(account, domain)
    End Function
End Class
