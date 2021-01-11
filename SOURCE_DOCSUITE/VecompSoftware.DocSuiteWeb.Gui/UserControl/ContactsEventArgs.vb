Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ContactsEventArgs
    Inherits EventArgs

    Public Contacts As IList(Of Contact)
    
End Class