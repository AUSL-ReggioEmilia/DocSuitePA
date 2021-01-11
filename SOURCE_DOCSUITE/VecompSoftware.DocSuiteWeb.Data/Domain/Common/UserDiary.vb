<Serializable()> _
Public Class UserDiary
    Inherits DomainObject(Of Int32)

#Region " Fields "

#End Region

#Region " Properties "

    Public Overridable Property Year As Integer
    Public Overridable Property Number As Integer
    Public Overridable Property [Object] As String
    Public Overridable Property Codice As String
    Public Overridable Property LogDate As DateTime
    Public Overridable Property Type As String
    Public Overridable Property PI As Integer
    Public Overridable Property PS As Integer
    Public Overridable Property PD As Integer
    Public Overridable Property PZ As Integer
    Public Overridable Property PM As Integer
    Public Overridable Property AdoptionDate As Date
    Public Overridable Property IsHandled As Integer
    Public Overridable Property UniqueIdProtocol As Guid

#End Region

End Class
