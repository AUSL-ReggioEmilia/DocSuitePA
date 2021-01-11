''' <summary> Destinatario. </summary>
''' <remarks> Type 2 </remarks>
Public Class POLRequestRecipient
    Inherits POLRequestContact

#Region " Fields "

    Private _status As POLMessageContactEnum
    Private _statusDescrition As String
    Private _dataSpedizione As DateTime?
    Private _idRicevuta As String
    Private _numero As String
    Private _costo As Double?

#End Region

#Region " Properties "

    Public Overridable Property Status() As POLMessageContactEnum
        Get
            Return _status
        End Get
        Set(ByVal value As POLMessageContactEnum)
            _status = value
        End Set
    End Property

    Public Overridable Property StatusDescrition() As String
        Get
            Return _statusDescrition
        End Get
        Set(ByVal value As String)
            _statusDescrition = value
        End Set
    End Property

    Public Overridable Property DataSpedizione() As DateTime?
        Get
            Return _dataSpedizione
        End Get
        Set(ByVal value As DateTime?)
            _dataSpedizione = value
        End Set
    End Property

    Public Overridable Property IdRicevuta() As String
        Get
            Return _idRicevuta
        End Get
        Set(ByVal value As String)
            _idRicevuta = value
        End Set
    End Property

    Public Overridable Property Numero() As String
        Get
            Return _numero
        End Get
        Set(ByVal value As String)
            _numero = value
        End Set
    End Property

    ''' <summary> Costo unitario della richiesta. </summary>
    ''' <remarks> Derivato dal totale. </remarks>
    Public Overridable Property Costo() As Double?
        Get
            Return _costo
        End Get
        Set(ByVal value As Double?)
            _costo = value
        End Set
    End Property

#End Region

End Class