<Serializable()> _
Public Class ZebraPrinter
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

    Private _description As String
    Public Overridable Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Private _documentTemplate As String
    Public Overridable Property DocumentTemplate As String
        Get
            Return _documentTemplate
        End Get
        Set(ByVal value As String)
            _documentTemplate = value
        End Set
    End Property

    Private _attachmentTemplate As String
    Public Overridable Property AttachmentTemplate As String
        Get
            Return _attachmentTemplate
        End Get
        Set(ByVal value As String)
            _attachmentTemplate = value
        End Set
    End Property

    Private _computerLogs As IList(Of ComputerLog)
    Public Overridable Property ComputerLogs() As IList(Of ComputerLog)
        Get
            Return _computerLogs
        End Get
        Set(ByVal value As IList(Of ComputerLog))
            _computerLogs = value
        End Set
    End Property

End Class


