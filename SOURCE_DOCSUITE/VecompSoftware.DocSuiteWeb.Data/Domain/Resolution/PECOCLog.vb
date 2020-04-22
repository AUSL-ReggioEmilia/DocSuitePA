''' <summary> Classe per la gestione semplice di un log delle PEC da inviare agli Organi di Controllo. </summary>
<Serializable()> _
Public Class PECOCLog
    Inherits DomainObject(Of Int32)

#Region " Fields "

    Private _pecOc As PECOC

    Private _description As String
    Private _date As DateTime
    Private _systemComputer As String
    Private _systemUser As String

#End Region

#Region " Constructor "
    Public Sub New()

    End Sub
#End Region

#Region " Properties "

    Public Overridable Property PecOc As PECOC
        Get
            Return _pecOc
        End Get
        Set(ByVal value As PECOC)
            _pecOc = value
        End Set
    End Property

    Public Overridable Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property [Date] As DateTime
        Get
            Return _date
        End Get
        Set(ByVal value As DateTime)
            _date = value
        End Set
    End Property

    Public Overridable Property SystemComputer As String
        Get
            Return _systemComputer
        End Get
        Set(ByVal value As String)
            _systemComputer = value
        End Set
    End Property

    Public Overridable Property SystemUser As String
        Get
            Return _systemUser
        End Get
        Set(ByVal value As String)
            _systemUser = value
        End Set
    End Property

#End Region

End Class
