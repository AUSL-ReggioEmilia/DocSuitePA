<Serializable()> _
Public Class ChangeSignerDTO

#Region "Fields"

#End Region

#Region "Properties"
    Public Property RoleId As Integer

    Public Property Origin As String

    Public Property Destination As String
#End Region

#Region "Constructors"
    Public Sub New()
    End Sub

    Public Sub New(ByVal idRole As Integer, ByVal origin As String, ByVal destination As String)
        RoleId = idRole
        _Origin = origin
        _Destination = destination
    End Sub
#End Region


End Class
