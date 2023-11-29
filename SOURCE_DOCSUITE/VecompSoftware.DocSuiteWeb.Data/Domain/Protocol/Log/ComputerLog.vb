<Serializable()> _
Public Class ComputerLog
    Inherits DomainObject(Of String)

#Region "Properties"
    Public Overridable Property SystemServer As String

    Public Overridable Property SystemUser As String
    Public Overridable Property LastOperationDate As Date?
    Public Overridable Property AccessNumber As Integer
    Public Overridable Property PrevOperationDate As Date?
    Public Overridable Property SessionId As String
    Public Overridable Property AdvancedScanner As Integer
    Public Overridable Property AdvancedViewer As Integer
    Public Overridable Property ZebraPrinter As ZebraPrinter
    Public Overridable Property ScannerConfiguration As ScannerConfiguration

#End Region

#Region "Ctor/init"
    Public Sub New()

    End Sub
#End Region
End Class


