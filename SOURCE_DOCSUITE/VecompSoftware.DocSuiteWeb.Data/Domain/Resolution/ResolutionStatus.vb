<Serializable()> _
Public Class ResolutionStatus
    Inherits DomainObject(Of Short)

#Region " Fields "

    Private _description As String

#End Region

#Region " Properties "

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

#End Region

End Class

