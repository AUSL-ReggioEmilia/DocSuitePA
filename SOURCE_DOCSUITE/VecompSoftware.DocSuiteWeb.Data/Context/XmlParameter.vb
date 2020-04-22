Imports System.Xml.Serialization

''' <summary>
''' Parametro d'ambiente
''' </summary>
''' <remarks>
''' Classe creata per semplificare l'uso delle parameterEnv
''' Implementa attributi e metodi utili alla serializzazione e all'uso delle lambda.
''' </remarks>
<Serializable(), XmlRoot("Parameter")> _
Public Class XmlParameter
    Implements IEquatable(Of XmlParameter)

#Region "Properties"
    ''' <summary>
    ''' Chiave del parametro
    ''' </summary>
    Public Property Key() As String

    ''' <summary>
    ''' Valore di default
    ''' </summary>
    Public Property Value() As String

    ''' <summary>
    ''' Valore della chiave nell'applicazione
    ''' </summary>
    Public Property AppValue() As String

    ''' <summary>
    ''' Gruppo di visualizzazione
    ''' </summary>
    Public Property Group() As String

    ''' <summary>
    ''' Descrizione
    ''' </summary>
    Public Property Description() As String

    ''' <summary>
    ''' Note varie
    ''' </summary>
    Public Property Note() As String

    ''' <summary>
    ''' Firma che identifica univocamente la proprietà associata all'XML
    ''' </summary>
    Public Property Signature() As String

    ''' <summary>
    ''' Versione nella quale il parametro è stato adottato
    ''' </summary>
    Public Property Version() As String

    ''' <summary>
    ''' Indica se il parametro è associato o meno ad un contenitore
    ''' </summary>
    Public Property Container() As Boolean

    ''' <summary>
    ''' Cliente
    ''' </summary>
    Public Property Customer() As String

    ''' <summary>
    ''' Indica il tipo del parametro
    ''' </summary>
    ''' <returns></returns>
    Public Property ParameterType() As ContainerPropertyType?
#End Region

#Region "IEquitable"

    ''' <summary>
    ''' Confronta due oggetti per chiave
    ''' </summary>
    ''' <remarks>Necessario per usare le lambda</remarks>
    Public Function Equals1(
        ByVal other As XmlParameter
        ) As Boolean Implements IEquatable(Of XmlParameter).Equals

        ' Controllo se nullo.
        If other Is Nothing Then Return False

        ' Controllo se faccio riferimento allo stesso oggetto.
        If Me Is other Then Return True

        ' Controllo se la chiave equivale.
        Return Key.Equals(other.Key)
    End Function

    ''' <summary>
    ''' Calcola l'hash code
    ''' </summary>
    ''' <remarks>Necessario per usare le lambda</remarks>
    Public Overrides Function GetHashCode() As Integer

        ' Calcolo l'hash della chiave.
        Return If(Key Is Nothing, 0, Key.GetHashCode())
    End Function

#End Region

End Class