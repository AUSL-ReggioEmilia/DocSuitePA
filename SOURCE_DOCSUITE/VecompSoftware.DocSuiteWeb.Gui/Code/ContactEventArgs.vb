Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic

Public Class ContactEventArgs
    Inherits EventArgs

    Public Property Description As String = String.Empty
    Public Property ContactSource As ICollection(Of Contact)
    Public Property ContactTarget As ICollection(Of Contact)

End Class
