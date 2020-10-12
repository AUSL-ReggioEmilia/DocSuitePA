Public Class EventListenerUtil
    Public Shared Property CustomEventInstances As IDictionary(Of String, Action(Of Object)) = New Dictionary(Of String, Action(Of Object))
End Class
