Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Data.SqlTypes
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module OChartItemExtensions

    <Extension()>
    Public Function OrderChronologically(ByVal source As IEnumerable(Of OChartItem)) As IEnumerable(Of OChartItem)
        Return source.OrderBy(Function(i) i.OrganizationChart.StartDateOrDefault) _
            .ThenBy(Function(i) i.OrganizationChart.EndDateOrDefault) _
            .ThenByDescending(Function(i) i.OrganizationChart.Id) _
            .ThenByDescending(Function(i) i.Id.ToSqlGuid())
    End Function

    <Extension()>
    Public Function EffectiveOrDefault(source As IEnumerable(Of OChartItem)) As OChartItem
        Return source.Where(Function(i) i.OrganizationChart.IsEffective) _
            .OrderByDescending(Function(i) i.OrganizationChart.StartDateOrDefault) _
            .ThenBy(Function(i) i.OrganizationChart.EndDateOrDefault) _
            .ThenByDescending(Function(i) i.Id.ToSqlGuid()) _
            .FirstOrDefault()
    End Function

    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItem), parentItem As OChartItem, oChart As OChart, userName As String) As IEnumerable(Of OChartItem)

        Return source.Select(Function(i) New OChartItem(i, parentItem, oChart, userName))
    End Function
    <Extension()>
    Public Function ReplicateList(ByVal source As IEnumerable(Of OChartItem), parentItem As OChartItem, userName As String) As IEnumerable(Of OChartItem)
        Return source.ReplicateList(parentItem, Nothing, userName)
    End Function

    <Extension()>
    Public Function ReplicateListAsDTO(ByVal source As IEnumerable(Of OChartItem), parentDTO As OChartItemDTO) As IEnumerable(Of OChartItemDTO)
        Return source.Select(Function(i) New OChartItemDTO(i) With {.Parent = parentDTO})
    End Function

    <Extension()>
    Public Function ToMailAddressString(ByVal source As OChartItem, separator As String) As String
        If Not source.HasMailboxes Then
            Return Nothing
        End If
        Dim addresses As IEnumerable(Of String) = source.Mailboxes.Select(Function(m) m.Mailbox.MailBoxName).OrderBy(Function(r) r)
        Return String.Join(separator, addresses.Where(Function(a) Not String.IsNullOrEmpty(a)))
    End Function
    <Extension()>
    Public Function ToMailAddressString(ByVal source As OChartItem) As String
        Return source.ToMailAddressString("; ")
    End Function

    <Extension()>
    Public Function FindByResource(source As IEnumerable(Of OChartItem), resource As Contact) As IEnumerable(Of OChartItem)
        Return source.Where(Function(i) i.HasContacts AndAlso i.Contacts.Any(Function(c) c.Contact.Id = resource.Id)).Distinct()
    End Function
    <Extension()>
    Public Function FindByResource(source As IEnumerable(Of OChartItem), resource As Container) As IEnumerable(Of OChartItem)
        Return source.Where(Function(i) i.HasContainers AndAlso i.Containers.Any(Function(c) c.Container.Id = resource.Id)).Distinct()
    End Function
    <Extension()>
    Public Function FindByResource(source As IEnumerable(Of OChartItem), resource As PECMailBox) As IEnumerable(Of OChartItem)
        Return source.Where(Function(i) i.HasMailboxes AndAlso i.Mailboxes.Any(Function(c) c.Mailbox.Id = resource.Id)).Distinct()
    End Function
    <Extension()>
    Public Function FindByResource(source As IEnumerable(Of OChartItem), resource As Role) As IEnumerable(Of OChartItem)
        Return source.Where(Function(i) i.HasRoles AndAlso i.Roles.Any(Function(c) c.Role.Id = resource.Id)).Distinct()
    End Function

    <Extension()>
    Public Function FindResourceMaster(source As IEnumerable(Of OChartItem), resource As Container) As OChartItem
        Return source.SelectMany(Function(i) i.Containers) _
            .FindByResource(resource).Where(Function(c) c.IsMaster).Select(Function(c) c.Item).FirstOrDefault()
    End Function
    <Extension()>
    Public Function FindResourceSharers(source As IEnumerable(Of OChartItem), resource As Container) As IEnumerable(Of OChartItem)
        Return source.SelectMany(Function(i) i.Containers) _
            .FindByResource(resource).Where(Function(c) Not c.IsMaster).Select(Function(c) c.Item)
    End Function

    <Extension()>
    Public Function GetMasterContainers(source As IEnumerable(Of OChartItem), resource As Container) As IEnumerable(Of OChartItemContainer)
        Return source.SelectMany(Function(i) i.Containers.Where(Function(c) c.Container.Id = resource.Id AndAlso c.Master.HasValue AndAlso c.Master.Value))
    End Function


    <Extension()>
    Public Function GetContainerRejection(source As IEnumerable(Of OChartItem), resource As Container) As Boolean?
        Return source.SelectMany(Function(i) i.Containers) _
            .FindByResource(resource).Any(Function(c) c.IsRejection)
    End Function

    <Extension()>
    Public Function HasResource(source As IEnumerable(Of OChartItem), resource As Contact) As Boolean
        Return source.Any(Function(i) i.HasContacts AndAlso i.Contacts.Any(Function(c) c.Contact.Id = resource.Id))
    End Function
    <Extension()>
    Public Function HasResource(source As IEnumerable(Of OChartItem), resource As Container) As Boolean
        Return source.Any(Function(i) i.HasContainers AndAlso i.Containers.Any(Function(c) c.Container.Id = resource.Id))
    End Function
    <Extension()>
    Public Function HasResource(source As IEnumerable(Of OChartItem), resource As PECMailBox) As Boolean
        Return source.Any(Function(i) i.HasMailboxes AndAlso i.Mailboxes.Any(Function(c) c.Mailbox.Id = resource.Id))
    End Function
    <Extension()>
    Public Function HasResource(source As IEnumerable(Of OChartItem), resource As Role) As Boolean
        Return source.Any(Function(i) i.HasRoles AndAlso i.Roles.Any(Function(c) c.Role.Id = resource.Id))
    End Function

    <Extension()>
    Public Function IsOwner(source As IEnumerable(Of OChartItem), item As OChartItem, container As Container) As Boolean?
        Dim found As OChartItem = source.FindById(item.Id)
        If found Is Nothing Then
            Return Nothing
        End If
        Dim itemContainer As OChartItemContainer = found.Containers.FirstOrDefault(Function(c) c.Id.Equals(container.Id))
        If itemContainer Is Nothing Then
            Return Nothing
        End If

        Return itemContainer.IsMaster
    End Function

    <Extension()>
    Public Function FindById(source As IEnumerable(Of OChartItem), id As Guid) As OChartItem
        Return source.FirstOrDefault(Function(i) i.Id.Equals(id))
    End Function

    <Extension()>
    Public Function FindByCode(source As IEnumerable(Of OChartItem), code As String) As OChartItem
        Return source.FirstOrDefault(Function(i) i.Code.Equals(code))
    End Function
    <Extension()>
    Public Function FindByFullCode(source As IEnumerable(Of OChartItem), fullCode As String) As OChartItem
        Return source.FirstOrDefault(Function(i) i.FullCode.Equals(fullCode))
    End Function

End Module
