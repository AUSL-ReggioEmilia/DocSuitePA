<Serializable()> _
Public Class Location
    Inherits DomainObject(Of Int32)


#Region "Private Data"
    Private _name As String
    Private _documentServer As String
    Private _protBiblosDSDB As String
    Private _docmBiblosDSDB As String
    Private _reslBiblosDSDB As String
    Private _conservationServer As String
    Private _consBiblosDSDB As String
#End Region

#Region "Properties"
    Overridable ReadOnly Property FullName() As String
        Get
            Return _name & " (" & _documentServer & ")"
        End Get
    End Property

    Overridable ReadOnly Property FullNameDB() As String
        Get
            Dim s As String = _name & " (" & _documentServer
            Select Case True
                Case Not String.IsNullOrEmpty(_protBiblosDSDB)
                    s &= "-" & _protBiblosDSDB
                Case Not String.IsNullOrEmpty(_docmBiblosDSDB)
                    s &= "-" & _docmBiblosDSDB
                Case Not String.IsNullOrEmpty(_reslBiblosDSDB)
                    s &= "-" & _reslBiblosDSDB
            End Select
            Return s & ")"
        End Get
    End Property

    Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Overridable Property DocumentServer() As String
        Get
            Return _documentServer
        End Get
        Set(ByVal value As String)
            _documentServer = value
        End Set
    End Property

    Overridable Property ProtBiblosDSDB() As String
        Get
            Return _protBiblosDSDB
        End Get
        Set(ByVal value As String)
            _protBiblosDSDB = value
        End Set
    End Property

    Overridable Property DocmBiblosDSDB() As String
        Get
            Return _docmBiblosDSDB
        End Get
        Set(ByVal value As String)
            _docmBiblosDSDB = value
        End Set
    End Property

    Overridable Property ReslBiblosDSDB() As String
        Get
            Return _reslBiblosDSDB
        End Get
        Set(ByVal value As String)
            _reslBiblosDSDB = value
        End Set
    End Property

    Overridable Property ConservationServer() As String
        Get
            Return _conservationServer
        End Get
        Set(ByVal value As String)
            _conservationServer = value
        End Set
    End Property

    Overridable Property ConsBiblosDSDB() As String
        Get
            Return _consBiblosDSDB
        End Get
        Set(ByVal value As String)
            _consBiblosDSDB = value
        End Set
    End Property
#End Region

#Region "Public Functions"
    Public Overridable Function GetArchiveByType(ByVal type As String) As String
        Select Case type.ToUpper()
            Case "PROT"
                Return Me.ProtBiblosDSDB
            Case "DOCM"
                Return Me.DocmBiblosDSDB
            Case "RESL"
                Return Me.ReslBiblosDSDB
        End Select
        Return String.Empty
    End Function
#End Region

#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

End Class
