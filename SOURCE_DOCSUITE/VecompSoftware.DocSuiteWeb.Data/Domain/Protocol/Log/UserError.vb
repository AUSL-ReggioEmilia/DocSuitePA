<Serializable()> _
Public Class UserError
    Inherits DomainObject(Of Int32)

#Region " Properties "

    Public Overridable Property SystemUser() As String

    Public Overridable Property SystemServer() As String

    Public Overridable Property SystemComputer() As String

    Public Overridable Property ModuleName() As String

    Public Overridable Property ErroreLevel() As String

    Public Overridable Property ErrorDate() As DateTime?

    Public Overridable Property ErrorDescription() As String

    Public Overridable ReadOnly Property ErroreDescriptionFormat() As String
        Get
            Dim descr As String = ErrorDescription
            If InStr(UCase$(descr), "<HTML>") <> 0 Then
                descr = Replace(descr, "<", "|")
                descr = Replace(descr, ">", "|")
            End If
            Return descr
        End Get
    End Property

#End Region

#Region " Constructor "

    Public Sub New()

    End Sub

#End Region

End Class


