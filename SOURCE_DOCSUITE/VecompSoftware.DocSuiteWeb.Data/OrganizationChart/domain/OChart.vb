Imports AutoMapper
Imports NHibernate.Collection.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable>
Public Class OChart
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Constructors "

    Public Sub New()
    End Sub
    Public Sub New(header As OChart, userName As String)
        LoadFrom(header, userName)
    End Sub
    Public Sub New(dto As OChartTransformerDTO)
        LoadFrom(dto)
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property Enabled As Boolean?
    Public Overridable Property StartDate As DateTime?
    Public Overridable Property EndDate As DateTime?
    Public Overridable Property Title As String
    Public Overridable Property Description As String
    Public Overridable Property Imported As Boolean?
    Public Overridable Property Items As IList(Of OChartItem)

    Public Overridable ReadOnly Property IsEnabled As Boolean
        Get
            Return Enabled.GetValueOrDefault(False)
        End Get
    End Property
    Public Overridable ReadOnly Property StartDateOrDefault As DateTime
        Get
            Return StartDate.GetValueOrDefault(DateTime.MinValue)
        End Get
    End Property
    Public Overridable ReadOnly Property EndDateOrDefault As DateTime
        Get
            Return EndDate.GetValueOrDefault(DateTime.MaxValue)
        End Get
    End Property
    Public Overridable ReadOnly Property IsEnded As Boolean
        Get
            Return EndDateOrDefault < DateTime.Now
        End Get
    End Property
    Public Overridable ReadOnly Property IsEffective As Boolean
        Get
            If Not IsEnabled Then
                Return False
            End If
            Dim started As Boolean = StartDateOrDefault < DateTime.Now
            Return started AndAlso Not IsEnded
        End Get
    End Property
    Public Overridable ReadOnly Property Duration As Double
        Get
            Return (EndDateOrDefault - StartDateOrDefault).TotalDays
        End Get
    End Property
    Public Overridable ReadOnly Property Expiration As Double
        Get
            If IsEnded Then
                Return 0
            End If
            Return (EndDateOrDefault - DateTime.Now).TotalDays
        End Get
    End Property
    Public Overridable ReadOnly Property IsImported As Boolean
        Get
            Return Imported.GetValueOrDefault(False)
        End Get
    End Property

    Public Overridable ReadOnly Property HasItems As Boolean
        Get
            Return Not Items.IsNullOrEmpty()
        End Get
    End Property
    Public Overridable ReadOnly Property Roots As IEnumerable(Of OChartItem)
        Get
            If Not HasItems Then
                Return Nothing
            End If

            Return Items.Where(Function(i) i.IsRoot).Distinct()
        End Get
    End Property

#End Region

#Region " Methods "

    Public Overridable Function CloneItems(header As OChart, userName As String) As OChart
        If header.HasItems Then
            Items = header.Roots.ReplicateList(Nothing, Me, userName).ToList()
        End If
        Return Me
    End Function

    Public Overridable Sub AddChild(item As OChartItem)
        If item.Parent IsNot Nothing Then
            Throw New InvalidOperationException("Il nodo che stai cercando di aggiungere non è una radice.")
        End If

        item.OrganizationChart = Me
        item.SetCodes()

        If item.HasItems Then
            For Each child As OChartItem In item.Items
                child.OrganizationChart = Me
            Next
        End If
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
        Dim item As New OChartItem(dto)
        AddChild(item)
    End Sub

    Public Overridable Sub RemoveChildren(deletion As IEnumerable(Of OChartItem))
        For Each item As OChartItem In deletion
            Me.Items.Remove(item)
        Next
    End Sub
    Public Overridable Sub ReparentChildren(parents As IEnumerable(Of OChartItem), userName As String)
        For Each item As OChartItem In parents.Where(Function(i) i.HasItems)
            Dim inheritance As IEnumerable(Of OChartItem) = item.Items.ReplicateList(Nothing, Me, userName)
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

    Public Overridable Sub RebuildHierarchy()
        RebuildHierarchy(Items)
    End Sub
    Public Overridable Sub RebuildFlattened()
        Items = GetFlattenedItems(Roots)
    End Sub
    Public Overridable Sub Transform(dto As OChartTransformerDTO)
        If Not dto.ComplyHeader(Me) Then
            Return
        End If

        Select Case True
            Case dto.IsRoot AndAlso Me.HasItems AndAlso dto.Comply(Me.Items)
                Select Case dto.Type
                    Case OChartTransformerDTO.TransformTypes.Delete
                        Dim deletion As IEnumerable(Of OChartItem) = Items.Where(Function(i) dto.Comply(i)).ToList()

                        RemoveChildren(dto)
                        ' Rimuovo i figli dall'albero esploso
                        Dim flattened As List(Of OChartItem) = GetFlattenedItems(deletion)
                        RemoveChildren(flattened)
                    Case Else
                        TransformChildren(dto)
                End Select

            Case dto.IsRoot
                Select Case dto.Type
                    Case OChartTransformerDTO.TransformTypes.SaveOrUpdate, OChartTransformerDTO.TransformTypes.Recode
                        AddChild(dto)
                End Select

            Case Else
                Select Case dto.Type
                    Case OChartTransformerDTO.TransformTypes.Delete
                        TransformChildren(dto)
                        ' Rimuovo i figli dall'albero esploso
                        Dim deletion As IEnumerable(Of OChartItem) = Items.Where(Function(i) dto.Comply(i))
                        Dim flattened As List(Of OChartItem) = GetFlattenedItems(deletion)
                        RemoveChildren(flattened)
                    Case Else
                        TransformChildren(dto)
                End Select
        End Select
    End Sub

    Private Sub LoadFrom(header As OChart, userName As String)
        Enabled = header.Enabled
        StartDate = header.StartDate
        EndDate = header.EndDate
        Title = header.Title
        Description = header.Description
        Imported = header.Imported
        CloneItems(header, userName)
    End Sub
    Private Sub LoadFrom(dto As OChartTransformerDTO)
        StartDate = dto.ReferenceDate
        Title = "Generato dinamicamente"
        Description = Title
        Imported = dto.Imported
    End Sub
    Private Sub RebuildHierarchy(parents As IEnumerable(Of OChartItem))
        For Each item As OChartItem In parents
            Dim children As IEnumerable(Of OChartItem) = Items.Where(Function(i) Not i.IsRoot AndAlso i.Parent.Id.Equals(item.Id))
            item.Items = Nothing
            item.AddChildren(children)
            If item.HasItems Then
                RebuildHierarchy(item.Items)
            End If
        Next
    End Sub
    Private Function GetFlattenedItems(parents As IEnumerable(Of OChartItem)) As List(Of OChartItem)
        Dim list As New List(Of OChartItem)(parents)
        Dim recursion As IEnumerable(Of OChartItem) = list.Where(Function(p) p.HasItems).SelectMany(Function(p) p.Items)
        If Not recursion.IsNullOrEmpty() Then
            list.AddRange(GetFlattenedItems(recursion))
        End If
        Return list
    End Function
    Private Sub TransformChildren(dto As OChartTransformerDTO)
        If Not HasItems Then
            Return
        End If

        For Each item As OChartItem In Items
            item.Transform(dto)
        Next
    End Sub

#End Region

End Class
