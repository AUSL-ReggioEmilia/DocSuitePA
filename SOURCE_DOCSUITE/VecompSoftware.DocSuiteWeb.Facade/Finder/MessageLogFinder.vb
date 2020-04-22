Imports System.ComponentModel
Imports VecompSoftware.DocSuiteWeb.Data

<Serializable(), DataObject()> _
Public Class MessageLogFinder
    Inherits NHMessageLogFinder


    Public Overrides Function Count() As Integer
        Return MyBase.Count()
    End Function

End Class
