Public Class ResolutionMessage
    Inherits DomainObject(Of Integer)


#Region " Properties "
    Public Overridable Property Resolution As Resolution
    Public Overridable Property Message As DSWMessage
    Public Overridable Property UniqueIdResolution As Guid

#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

    Public Sub New(ByRef resolution As Resolution, ByRef message As DSWMessage)
        Me.Resolution = resolution
        UniqueIdResolution = resolution.UniqueId
        Me.Message = message
        UniqueId = Guid.NewGuid()
    End Sub

#End Region
End Class
