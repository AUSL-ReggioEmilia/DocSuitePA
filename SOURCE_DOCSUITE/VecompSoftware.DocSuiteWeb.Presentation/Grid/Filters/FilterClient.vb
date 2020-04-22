
<Serializable()>
Public Class FilterClient

#Region " Fields "

    Private _column As String
    Private _clientid As String
    Private _value As String

#End Region

#Region " Properties "

    ''' <summary> Nome della colonna dove applicare il filtro </summary>
    Public Property Column() As String
        Get
            Return _column
        End Get
        Set(ByVal value As String)
            _column = value
        End Set
    End Property

    ''' <summary> ID-Client da utilizzare nel javascript </summary>
    Public Property ClientID() As String
        Get
            Return _clientid
        End Get
        Set(ByVal value As String)
            _clientid = value
        End Set
    End Property

    ''' <summary> Valore da assegnare al controllo client </summary>
    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

#End Region

#Region " Constructors "

    ''' <summary> Costruttore con parametri </summary>
    ''' <param name="Column">ome della colonna dove applicare il filtro</param>
    ''' <param name="ClientID">ID Client da utilizzare nel javascript</param>
    ''' <param name="Value">Valore da assegnare al controllo client</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal Column As String, ByVal ClientID As String, ByVal Value As String)
        _column = Column
        _clientid = ClientID
        _value = Value
    End Sub

    ''' <summary> Costruttore default </summary>
    ''' <remarks> Serve per la serializzazione in Session </remarks>
    Public Sub New()
    End Sub

#End Region

#Region " Methods "

    Public Overrides Function ToString() As String
        Return String.Format("<FILTERSTATE column=""{0}"" clientid=""{1}"" value=""{2}"" />", _column, _clientid, _value)
    End Function

#End Region

End Class
