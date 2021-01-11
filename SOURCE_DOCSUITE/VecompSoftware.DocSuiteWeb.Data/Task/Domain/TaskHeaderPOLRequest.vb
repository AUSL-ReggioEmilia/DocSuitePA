Public Class TaskHeaderPOLRequest
    Inherits DomainObject(Of Integer)
    Implements IAuditable

#Region " Properties "

    Public Overridable Property Header As TaskHeader

    Public Overridable Property POLRequest As POLRequest

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate

    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser

    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate

    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

#End Region

End Class
