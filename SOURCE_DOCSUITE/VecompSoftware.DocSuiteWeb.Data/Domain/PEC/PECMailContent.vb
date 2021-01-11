<Serializable()> _
Public Class PECMailContent
    Inherits DomainObject(Of Int32)


#Region "Fields"
#End Region

    Public Overridable Property IdPECMail() As Integer

    Public Overridable Property MailContent() As String

    'Public Overridable Property IsActive As Short Implements ISupportLogicDelete.IsActive
    '    Get
    '        Return _isActive
    '    End Get
    '    Set(value As Short)
    '        _isActive = value
    '    End Set
    'End Property
End Class
