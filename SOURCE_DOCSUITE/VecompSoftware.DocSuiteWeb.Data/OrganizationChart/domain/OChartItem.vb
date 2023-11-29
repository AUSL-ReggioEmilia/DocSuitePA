Imports NHibernate.Collection.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable>
Public Class OChartItem
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Constructors "
    Protected Sub New()

    End Sub

    Public Sub New(userName As String)
        RegistrationUser = userName
        RegistrationDate = DateTimeOffset.UtcNow
    End Sub

    Public Sub New(item As OChartItem, parentItem As OChartItem, header As OChart, userName As String)
        Code = item.Code
        Parent = parentItem
        SetCodes()

        OrganizationChart = header
        Title = item.Title
        Description = item.Description
        Acronym = item.Acronym
        Imported = item.Imported
        Enabled = item.Enabled
        RegistrationUser = userName
        RegistrationDate = DateTimeOffset.UtcNow

        If item.HasContainers Then
            Containers = item.Containers.Distinct().ReplicateList(Me).ToList()
        End If
        If item.HasRoles Then
            Roles = item.Roles.Distinct().ReplicateList(Me).ToList()
        End If
        If item.HasContacts Then
            Contacts = item.Contacts.Distinct().ReplicateList(Me).ToList()
        End If

        If item.HasItems Then
            Items = item.Items.Distinct().ReplicateList(Me, OrganizationChart, userName).ToList()
        End If

    End Sub
    Public Sub New(dto As OChartTransformerDTO)
        Code = dto.Code
        Title = dto.Title
        Description = dto.Description
        Acronym = dto.Acronym
        Imported = dto.Imported
        OrganizationChart = New OChart(dto)

        If Not dto.IsRoot Then
            Parent = New OChartItem(dto.Parent)
            Parent.OrganizationChart = New OChart(dto)
            Parent.Items = New List(Of OChartItem) From {Me}
        End If
        SetCodes()

        AddResources(dto)
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property Enabled As Boolean?
    Public Overridable Property OrganizationChart As OChart
    Public Overridable Property Code As String
    Public Overridable Property FullCode As String
    Public Overridable Property Title As String
    Public Overridable Property Description As String
    Public Overridable Property Acronym As String
    Public Overridable Property Imported As Boolean?
    Public Overridable Property Parent As OChartItem
    Public Overridable Property Items As IList(Of OChartItem)
    Public Overridable Property Contacts As IList(Of OChartItemContact)
    Public Overridable Property Containers As IList(Of OChartItemContainer)
    Public Overridable Property Roles As IList(Of OChartItemRole)

    Public Overridable ReadOnly Property IsEnabled As Boolean
        Get
            Return Enabled.GetValueOrDefault(True)
        End Get
    End Property
    Public Overridable ReadOnly Property IsImported As Boolean
        Get
            Return Imported.GetValueOrDefault(False)
        End Get
    End Property
    Public Overridable ReadOnly Property IsRoot As Boolean
        Get
            Return Parent Is Nothing
        End Get
    End Property
    Public Overridable ReadOnly Property HasItems As Boolean
        Get
            Return Not Items.IsNullOrEmpty()
        End Get
    End Property

    Public Overridable ReadOnly Property HasContacts As Boolean
        Get
            Return Not Contacts.IsNullOrEmpty()
        End Get
    End Property
    Public Overridable ReadOnly Property HasContainers As Boolean
        Get
            Return Not Containers.IsNullOrEmpty()
        End Get
    End Property
    Public Overridable ReadOnly Property HasRoles As Boolean
        Get
            Return Not Roles.IsNullOrEmpty()
        End Get
    End Property

    Public Overridable ReadOnly Property HasResources As Boolean
        Get
            Return HasContacts OrElse HasContainers OrElse HasRoles
        End Get
    End Property

#End Region

#Region " Methods "

    Public Overridable Sub SetCodes()
        If IsRoot Then
            FullCode = Code
            Return
        End If
        FullCode = String.Format("{0}|{1}", Parent.FullCode, Code)
    End Sub
    Public Overridable Sub SetCodesRecursively()
        SetCodes()
        If HasItems Then
            For Each child As OChartItem In Items
                child.SetCodesRecursively()
            Next
        End If
    End Sub

    Public Overridable Sub Swap(source As OChartItem)
        Acronym = source.Acronym
        If Not FullCode.EndsWith(source.Code, StringComparison.InvariantCulture) Then
            Code = source.Code
            SetCodesRecursively()
        End If
        Title = source.Title
        Description = source.Description
        Enabled = source.Enabled
    End Sub
    ' Gestione Items

    Public Overridable Sub AddChild(item As OChartItem)
        item.OrganizationChart = OrganizationChart
        item.Parent = Me
        item.SetCodes()
        If Items Is Nothing Then
            Items = New List(Of OChartItem) From {item}
            Return
        End If
        Items.Add(item)
    End Sub
    Public Overridable Sub AddChildren(list As IEnumerable(Of OChartItem))
        For Each item As OChartItem In list
            AddChild(item)
        Next
    End Sub
    Public Overridable Sub AddChild(dto As OChartTransformerDTO)
        AddChild(New OChartItem(dto))
    End Sub

    Public Overridable Sub RemoveChild(resource As OChartItem)
        RemoveChildren((New List(Of OChartItem) From {resource}))
    End Sub

    Public Overridable Sub RemoveChildren(deletion As IEnumerable(Of OChartItem))
        For Each item As OChartItem In deletion
            Me.Items.Remove(item)
        Next
    End Sub
    Public Overridable Sub ReparentChildren(parents As IEnumerable(Of OChartItem), userName As String)
        For Each item As OChartItem In parents.Where(Function(i) i.HasItems)
            Dim inheritance As IEnumerable(Of OChartItem) = item.Items.Distinct().ReplicateList(Me, OrganizationChart, userName)
            Me.AddChildren(inheritance)
        Next
    End Sub
    Public Overridable Sub RemoveChildren(dto As OChartTransformerDTO, userName As String)
        Dim deletion As List(Of OChartItem) = Items.Where(Function(i) dto.Comply(i)).ToList()
        If deletion.IsNullOrEmpty() Then
            Return
        End If

        If dto.IsReparenting Then
            Me.ReparentChildren(deletion, userName)
        End If
        Me.RemoveChildren(deletion)
    End Sub

    Public Overridable Function HasHierarchicalContacts() As Boolean
        If HasContacts Then
            Return True
        End If
        If HasItems Then
            Return Items.Any(Function(i) i.HasHierarchicalContacts())
        End If
        Return False
    End Function
    Public Overridable Function HasHierarchicalContainers() As Boolean
        If HasContainers Then
            Return True
        End If
        If HasItems Then
            Return Items.Any(Function(i) i.HasHierarchicalContainers())
        End If
        Return False
    End Function
    Public Overridable Function HasHierarchicalRoles() As Boolean
        If HasRoles Then
            Return True
        End If
        If HasItems Then
            Return Items.Any(Function(i) i.HasHierarchicalRoles())
        End If
        Return False
    End Function

    Public Overridable Function GetHierarchicalContacts() As List(Of Contact)
        Dim hierarchical As New List(Of Contact)
        If HasContacts Then
            hierarchical.AddRange(Contacts.Select(Function(c) c.Contact))
        End If
        If HasItems Then
            For Each item As OChartItem In Items
                Dim recursion As List(Of Contact) = item.GetHierarchicalContacts()
                Dim missing As IEnumerable(Of Contact) = recursion.Where(Function(r) Not hierarchical.Any(Function(h) h.Id.Equals(r.Id)))
                hierarchical.AddRange(missing)
            Next
        End If
        Return hierarchical
    End Function
    Public Overridable Function GetHierarchicalContainers() As List(Of Container)
        Dim hierarchical As New List(Of Container)
        If HasContainers Then
            hierarchical.AddRange(Containers.Select(Function(c) c.Container))
        End If
        If HasItems Then
            For Each item As OChartItem In Items
                Dim recursion As List(Of Container) = item.GetHierarchicalContainers()
                Dim missing As IEnumerable(Of Container) = recursion.Where(Function(r) Not hierarchical.Any(Function(h) h.Id.Equals(r.Id)))
                hierarchical.AddRange(missing)
            Next
        End If
        Return hierarchical
    End Function

    Public Overridable Function GetHierarchicalRoles() As List(Of Role)
        Dim hierarchical As New List(Of Role)
        If HasContacts Then
            hierarchical.AddRange(Roles.Select(Function(r) r.Role))
        End If
        If HasItems Then
            For Each item As OChartItem In Items
                Dim recursion As List(Of Role) = item.GetHierarchicalRoles()
                Dim missing As IEnumerable(Of Role) = recursion.Where(Function(r) Not hierarchical.Any(Function(h) h.Id.Equals(r.Id)))
                hierarchical.AddRange(missing)
            Next
        End If
        Return hierarchical
    End Function

    ' Gestione risorse Contact

    Public Overridable Sub AddContacts(list As IEnumerable(Of Contact))
        Dim missing As IEnumerable(Of OChartItemContact) = list.Select(Function(c) New OChartItemContact() With {.Item = Me, .Contact = c})
        If HasContacts Then
            missing = missing.Where(Function(m) Not Contacts.Any(Function(c) c.Contact.Id.Equals(m.Contact.Id)))
        End If

        If Contacts Is Nothing Then
            Contacts = New List(Of OChartItemContact)(missing)
            Return
        End If

        For Each resource As OChartItemContact In missing
            Contacts.Add(resource)
        Next
    End Sub
    Public Overridable Sub AddContact(resource As Contact)
        AddContacts(New List(Of Contact) From {resource})
    End Sub
    Public Overridable Sub RemoveContacts(list As IEnumerable(Of Contact))
        If Not HasContacts Then
            Return
        End If

        For Each contact As Contact In list
            Contacts.Remove(Contacts.SingleOrDefault(Function(f) f.Contact.Id = contact.Id))
        Next

    End Sub
    Public Overridable Sub RemoveContact(resource As Contact)
        RemoveContacts(New List(Of Contact) From {resource})
    End Sub

    ' Gestione risorse Container

    Public Overridable Sub AddContainers(list As IEnumerable(Of OChartItemContainer))
        Dim missing As IEnumerable(Of OChartItemContainer) = list.Select(Function(c) New OChartItemContainer() With {.Item = Me, .Container = c.Container, .Master = c.Master, .Rejection = c.Rejection})
        If HasContainers Then
            missing = missing.Where(Function(r) Not Containers.HasResource(r.Container))
        End If

        If Containers Is Nothing Then
            Containers = New List(Of OChartItemContainer)(missing)
            Return
        End If

        Dim updatable As IEnumerable(Of OChartItemContainer) = Containers.Where(Function(c) list.HasResource(c.Container))
        For Each resource As OChartItemContainer In updatable
            Dim current As OChartItemContainer = list.FindByResource(resource.Container).First()
            If current.Master.HasValue Then
                resource.Master = current.Master
            End If
            If current.Rejection.HasValue Then
                resource.Rejection = current.Rejection
            End If
        Next

        For Each resource As OChartItemContainer In missing
            Containers.Add(resource)
        Next
    End Sub

    Public Overridable Sub AddContainer(resource As Container, Optional itemContainer As OChartItemContainer = Nothing)
        If itemContainer Is Nothing Then
            itemContainer = New OChartItemContainer()
        End If
        itemContainer.Container = resource
        AddContainers(New List(Of OChartItemContainer) From {itemContainer})
    End Sub

    Public Overridable Sub RemoveContainers(list As IEnumerable(Of Container))
        If Not HasContainers Then
            Return
        End If

        For Each container As Container In list
            Containers.Remove(Containers.SingleOrDefault(Function(f) f.Container.Id = container.Id))
        Next

    End Sub
    Public Overridable Sub RemoveContainer(resource As Container)
        RemoveContainers(New List(Of Container) From {resource})
    End Sub

    ' Gestione risorse Role

    Public Overridable Sub AddRoles(list As IEnumerable(Of Role))
        Dim missing As IEnumerable(Of OChartItemRole) = list.Select(Function(c) New OChartItemRole() With {.Item = Me, .Role = c})
        If HasRoles Then
            missing = missing.Where(Function(m) Not Roles.Any(Function(c) c.Role.Id.Equals(m.Role.Id)))
        End If

        If Roles Is Nothing Then
            Roles = New List(Of OChartItemRole)(missing)
            Return
        End If

        For Each resource As OChartItemRole In missing
            Roles.Add(resource)
        Next
    End Sub
    Public Overridable Sub AddRole(resource As Role)
        AddRoles(New List(Of Role) From {resource})
    End Sub
    Public Overridable Sub RemoveRoles(list As IEnumerable(Of Role))
        If Not HasRoles Then
            Return
        End If

        For Each role As Role In list
            Roles.Remove(Roles.SingleOrDefault(Function(f) f.Role.Id = role.Id))
        Next
    End Sub
    Public Overridable Sub RemoveRole(resource As Role)
        RemoveRoles(New List(Of Role) From {resource})
    End Sub

    ' Gestione Transformers

    Public Overridable Sub AddResources(dto As OChartTransformerDTO)
        If Not dto.Comply(Me) Then
            Return
        End If

        If dto.HasContacts Then
            Me.AddContacts(dto.Contacts)
        End If
        If dto.HasContainers Then
            Me.AddContainers(dto.Containers)
        End If
        If dto.HasRoles Then
            Me.AddRoles(dto.Roles)
        End If
    End Sub
    Public Overridable Sub RemoveResources(dto As OChartTransformerDTO)
        If Not dto.Comply(Me) Then
            Return
        End If

        If dto.HasContacts Then
            Me.RemoveContacts(dto.Contacts)
        End If
        If dto.HasContainers Then
            Me.RemoveContainers(dto.Containers.Select(Function(c) c.Container))
        End If
        If dto.HasRoles Then
            Me.RemoveRoles(dto.Roles)
        End If
    End Sub

    Public Overridable Sub Transform(dto As OChartTransformerDTO)
        Select Case True
            Case dto.Comply(Me)
                Select Case dto.Type
                    Case OChartTransformerDTO.TransformTypes.Recode
                        If Me.IsImported AndAlso Not dto.IsImported Then
                            Return
                        End If

                        Me.Code = dto.Code
                        Me.SetCodesRecursively()

                    Case OChartTransformerDTO.TransformTypes.SaveOrUpdate
                        If Not String.IsNullOrEmpty(dto.Description) Then
                            Me.Description = dto.Description
                        End If
                        If Not String.IsNullOrEmpty(dto.Acronym) Then
                            Me.Acronym = dto.Acronym
                        End If
                        Me.AddResources(dto)

                        If Not String.IsNullOrEmpty(dto.Title) Then
                            If Me.IsImported AndAlso Not dto.IsImported Then
                                Return
                            End If

                            Me.Title = dto.Title
                        End If

                    Case OChartTransformerDTO.TransformTypes.RemoveResources
                        Me.RemoveResources(dto)
                End Select

            Case Not dto.IsRoot AndAlso dto.Parent.Comply(Me)
                Select Case dto.Type
                    Case OChartTransformerDTO.TransformTypes.SaveOrUpdate
                        If Not Me.HasItems OrElse Not dto.Comply(Me.Items) Then
                            Me.AddChild(dto)
                        End If

                    Case OChartTransformerDTO.TransformTypes.Delete
                        Me.RemoveChildren(dto)
                End Select

            Case Me.HasItems AndAlso dto.ComplyRecursively(Me.Items)
                Me.TransformChildren(dto)

            Case Me.HasItems AndAlso Not dto.IsRoot AndAlso dto.Parent.ComplyRecursively(Me.Items)
                Select Case dto.Type
                    Case OChartTransformerDTO.TransformTypes.SaveOrUpdate
                        Me.TransformChildren(dto)
                End Select
        End Select
    End Sub
    Public Overridable Sub TransformChildren(dto As OChartTransformerDTO)
        If Not HasItems Then
            Return
        End If

        For Each item As OChartItem In Items
            item.Transform(dto)
        Next
    End Sub

#End Region

End Class
