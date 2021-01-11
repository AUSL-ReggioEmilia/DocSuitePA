Imports System.Collections.Generic

Public Class ItemControlEventArgs(Of T)
    Inherits EventArgs

    Public Property Group As String
    Public Property Item As T
    Public Property Cancel As Boolean

    Public Sub New()
    End Sub

    Public Sub New(group As String)
        Me.New()
        Me.Group = group
    End Sub

    Public Sub New(item As T, group As String)
        Me.New(group)
        Me.Item = item
    End Sub

End Class