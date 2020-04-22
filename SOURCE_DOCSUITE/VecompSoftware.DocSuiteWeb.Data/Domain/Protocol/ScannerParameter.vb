<Serializable()> _
Public Class ScannerParameter
    Inherits DomainObject(Of Integer)

    Private _name As String
    Public Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _value As String
    Public Overridable Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

    Private _description As String
    Public Overridable Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Private _scannerConfiguration As ScannerConfiguration
    Public Overridable Property ScannerConfiguration() As ScannerConfiguration
        Get
            Return _scannerConfiguration
        End Get
        Set(ByVal value As ScannerConfiguration)
            _scannerConfiguration = value
        End Set
    End Property

End Class


