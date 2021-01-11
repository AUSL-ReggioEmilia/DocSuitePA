<Serializable()> _
Public Class Location
    Inherits DomainObject(Of Int32)


#Region "Private Data"

#End Region

#Region "Properties"

    Overridable Property Name As String
    Overridable Property ProtBiblosDSDB As String
    Overridable Property DocmBiblosDSDB As String
    Overridable Property ReslBiblosDSDB As String
    Overridable Property ConsBiblosDSDB As String
#End Region

#Region "Public Functions"
    Public Overridable Function GetArchiveByType(ByVal type As String) As String
        Select Case type.ToUpper()
            Case "PROT"
                Return Me.ProtBiblosDSDB
            Case "DOCM"
                Return Me.DocmBiblosDSDB
            Case "RESL"
                Return Me.ReslBiblosDSDB
        End Select
        Return String.Empty
    End Function
#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class
