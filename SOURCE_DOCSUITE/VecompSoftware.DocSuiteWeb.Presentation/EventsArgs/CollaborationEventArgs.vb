Public Class CollaborationEventArgs

    Protected _idCollaboration As Integer

    Public Sub New(ByVal idCollaboration As Integer)
        Me._idCollaboration = idCollaboration
    End Sub

    Public ReadOnly Property IdCollaboration As Integer
        Get
            Return Me._idCollaboration
        End Get
    End Property

    Public Property DocumentType As String

End Class
