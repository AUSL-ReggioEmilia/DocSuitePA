''' <summary>
''' Ordinamento di default delle pecmailbox. Prima le gestite poi le non gestite, in ordine alfabetico.
''' </summary>
Public Class PecMailBoxComparer
    Implements IComparer(Of PECMailBox)


    Public Function Compare(x As PECMailBox, y As PECMailBox) As Integer Implements IComparer(Of PECMailBox).Compare
        Dim comparison As Integer
        If x Is Nothing Then
            If y Is Nothing Then
                comparison = 0
            Else
                comparison = -1
            End If
        Else
            If y Is Nothing Then
                comparison = 1
            Else
                comparison = y.HasServers().CompareTo(x.HasServers())
                If comparison = 0 Then
                    comparison = String.Compare(x.MailBoxName, y.MailBoxName, StringComparison.OrdinalIgnoreCase)
                End If
            End If
        End If
        Return comparison
    End Function
End Class