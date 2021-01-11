Public Class TenderHeader
    Inherits DomainObject(Of Guid)
    Implements IAuditable

    Public Overridable Property IdResolution As Integer?

    Public Overridable Property DocumentSeriesItem As DocumentSeriesItem

    Public Overridable Property Title As String

    Public Overridable Property Abstract As String

    Public Overridable Property Year As Integer

    Public Overridable Property Lots As IList(Of TenderLot)

    Public Overridable Sub AddLot(lot As TenderLot)
        If Lots Is Nothing Then
            Lots = New List(Of TenderLot)()
        End If
        lot.Tender = Me
        Lots.Add(lot)
    End Sub

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser


End Class
