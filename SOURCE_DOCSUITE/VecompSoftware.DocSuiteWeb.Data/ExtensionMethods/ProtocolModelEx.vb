
Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Protocols


Public Module ProtocolModelEx

    <Extension()>
    Public Function FullProtocolNumber(source As ProtocolModel) As String
        Return String.Format("{0}/{1:0000000}", source.Year, source.Number)
    End Function

    <Extension()>
    Public Function ShortDescriptionType(source As ProtocolModel) As String
        Select Case source.ProtocolType.EntityShortId
            Case -1
                Return "I"
            Case 0
                Return "I/U"
            Case 1
                Return "U"
            Case Else
                Return String.Empty
        End Select
    End Function

End Module
