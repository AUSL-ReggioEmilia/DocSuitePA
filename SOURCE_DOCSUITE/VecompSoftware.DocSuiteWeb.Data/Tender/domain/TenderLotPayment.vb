Public Class TenderLotPayment
    Inherits DomainObject(Of Guid)
    Implements IAuditable

    Public Overridable Property Lot As TenderLot

    Public Overridable Property PaymentKey As String

    Public Overridable Property Amount As Double

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

End Class
