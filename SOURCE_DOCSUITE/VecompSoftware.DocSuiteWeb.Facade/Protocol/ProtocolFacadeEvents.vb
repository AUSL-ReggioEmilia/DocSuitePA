Imports VecompSoftware.DocSuiteWeb.Data

Public Delegate Sub ProtocolEventHandler(sender As Object, args As ProtocolEventArgs)

Public Class ProtocolEventArgs
    Inherits EventArgs

    Public Sub New(protocol As Protocol)
        Me.Protocol = protocol
    End Sub

    Public Sub New()

    End Sub

    Public Property Protocol As Protocol
    Public Property Cancel As Boolean
    Public Property Tag As Object

End Class
