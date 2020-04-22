Public Class TenderLot
    Inherits DomainObject(Of Guid)
    Implements IAuditable

    Public Overridable Property Tender As TenderHeader

    Public Overridable Property Payments As IList(Of TenderLotPayment)

    Public Overridable Property CIG As String

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Sub AddPayment(payment As TenderLotPayment)
        If Payments Is Nothing Then
            Payments = New List(Of TenderLotPayment)()
        End If
        payment.Lot = Me
        payment.RegistrationDate = DateTimeOffset.UtcNow
        payment.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        Payments.Add(payment)
    End Sub

End Class
