<Serializable()> _
Public Class DistributionList
    Inherits DomainObject(Of Int32)

#Region "private data"
    Private _name As String
    Private _recipients As IList(Of Recipient)

#End Region

#Region "Properties"
    Public Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Overridable Property Recipients() As IList(Of Recipient)
        Get
            Return _recipients
        End Get
        Set(ByVal value As IList(Of Recipient))
            _recipients = value
        End Set
    End Property

#End Region

#Region "Ctor/init"
    Public Sub New()
        _recipients = New List(Of Recipient)
    End Sub
#End Region

End Class

