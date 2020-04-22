<Serializable()> _
Public Class ScannerConfiguration
    Inherits DomainObject(Of Integer)

#Region " Fields "

    Private _description As String
    Private _isDefault As Boolean
    Private _computerLogs As IList(Of ComputerLog)
    Private _scannerParameters As IList(Of ScannerParameter)
    Private _parameterDictionary As IDictionary(Of String, String)
    Private _parameterConcat As String

#End Region

#Region " Properties "

    Public Overridable Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property IsDefault As Boolean
        Get
            Return _isDefault
        End Get
        Set(ByVal value As Boolean)
            _isDefault = value
        End Set
    End Property

    Public Overridable Property ComputerLogs() As IList(Of ComputerLog)
        Get
            Return _computerLogs
        End Get
        Set(ByVal value As IList(Of ComputerLog))
            _computerLogs = value
        End Set
    End Property

    Public Overridable Property ScannerParameters() As IList(Of ScannerParameter)
        Get
            Return _scannerParameters
        End Get
        Set(ByVal value As IList(Of ScannerParameter))
            _scannerParameters = value
            _parameterDictionary = Nothing
            _parameterConcat = Nothing
        End Set
    End Property

    Private ReadOnly Property ParameterDictionary As IDictionary(Of String, String)
        Get
            If _parameterDictionary Is Nothing Then
                _parameterDictionary = New Dictionary(Of String, String)
                For Each sp As ScannerParameter In ScannerParameters
                    If Not String.IsNullOrEmpty(sp.Name) Then
                        _parameterDictionary.Add(sp.Name, sp.Value)
                    End If
                Next
            End If
            Return _parameterDictionary
        End Get
    End Property

    Public Overridable ReadOnly Property ParameterConcat As String
        Get
            If _parameterConcat Is Nothing Then
                Dim sb As New Text.StringBuilder
                For Each sp As ScannerParameter In ScannerParameters
                    If Not String.IsNullOrEmpty(sp.Name) Then
                        sb.AppendFormat("{0}:{1};", sp.Name, sp.Value)
                    End If
                Next
                _parameterConcat = sb.ToString()
            End If
            Return _parameterConcat
        End Get
    End Property

#End Region

#Region " Methods "

    Public Overridable Function GetParameterValue(ByVal name As String) As String
        Dim retval As String = Nothing
        If ParameterDictionary.ContainsKey(name) Then
            retval = ParameterDictionary(name)
        End If
        Return retval
    End Function

#End Region

End Class


