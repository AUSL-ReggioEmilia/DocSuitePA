''' <summary>
''' Ambiente contenente i parametri che ne permettono la configuarizzazione
''' </summary>
''' <remarks>
''' Classe creata per semplificare l'uso delle parameterEnv
''' Implementa attributi e metodi utili alla serializzazione.
''' </remarks>
<Serializable()> _
Public Class XmlEnvironment

#Region "Fields"
    Private _name As String
    Private _parameters As List(Of XmlParameter)
#End Region

#Region " Constructors "
    Public Sub New()
        _parameters = New List(Of XmlParameter)
    End Sub
#End Region

#Region "Properties"
    ''' <summary>
    ''' Nome dell'ambiente
    ''' </summary>
    ''' <remarks>
    ''' <list>
    ''' <item><value>Atti</value></item>
    ''' <item><value>Protocollo</value></item>
    ''' <item><value>Pratiche</value></item>
    ''' </list>
    ''' </remarks>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    ''' <summary>
    ''' Parametri ambientali
    ''' </summary>
    Public Property Parameters() As List(Of XmlParameter)
        Get
            Return _parameters
        End Get
        Set(value As List(Of XmlParameter))
            _parameters = value
        End Set
    End Property
#End Region
    
End Class
