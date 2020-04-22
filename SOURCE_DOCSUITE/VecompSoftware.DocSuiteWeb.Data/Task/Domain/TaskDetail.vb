Imports System.ComponentModel

Public Class TaskDetail
    Inherits DomainObject(Of Integer)
    Implements IAuditable

    Public Sub New()
        RegistrationDate = DateTime.Now
    End Sub


    Public Overridable Property TaskHeader As TaskHeader

    Public Overridable Property DetailType As DetailTypeEnum

    Public Overridable Property Title As String
    Public Overridable Property Description As String
    Public Overridable Property ErrorDescription As String

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

End Class

Public Enum DetailTypeEnum
    Debug = 0
    <Description("Informazione")>
    Info = 10
    <Description("Avviso")>
    Warn = 20
    <Description("Sommario")>
    Summary = 50
    <Description("Errore")>
    ErrorType = 100
End Enum
