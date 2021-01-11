Imports System.ComponentModel
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.PEC.Finder

<Serializable(), DataObject()> _
Public Class PECMailLogFinder
    Inherits NHibernatePECMailLogFinder


    Public Overrides Function Count() As Integer
        Return MyBase.Count()
    End Function

End Class
