Public Class FilterItem
    Inherits DomainObject(Of Guid)

#Region " Constructors "

    Public Sub New()
    End Sub
    Public Sub New(ynck As YearNumberCompositeKey)
        Year = ynck.Year
        Number = ynck.Number
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property RegistrationDate As DateTime?
    Public Overridable Property SessionId As Guid?
    Public Overridable Property Year As Short?
    Public Overridable Property Number As Integer?

    Public Overridable Function GetYCNK() As YearNumberCompositeKey
        If Year.HasValue AndAlso Number.HasValue Then
            Return New YearNumberCompositeKey(Year, Number)
        End If
        Return Nothing
    End Function

#End Region

End Class
