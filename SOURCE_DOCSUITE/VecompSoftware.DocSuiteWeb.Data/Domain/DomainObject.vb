Imports System
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports VecompSoftware.Services.Command

<Serializable()> _
Public MustInherit Class DomainObject(Of TKey)
    Implements ICloneable

    Private _id As TKey = Nothing 'Valore di Default

    Public Overridable Property Id() As TKey
        Get
            Return _id
        End Get
        Set(ByVal value As TKey)
            _id = value
        End Set
    End Property

    Public Overridable Property UniqueId As Guid

    Public Overridable Property Timestamp As Byte()

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As DomainObject(Of TKey) = TryCast(obj, DomainObject(Of TKey))
        Return compareTo IsNot Nothing AndAlso (HasSameNonDefaultIdAs(compareTo) OrElse _
                ((IsTransient() OrElse compareTo.IsTransient()) AndAlso _
                HasSameBusinessSignatureAs(compareTo)))
    End Function

    Public Overridable Function [Default]() As TKey
        Return Nothing
    End Function

    Public Overridable Function IsTransient() As Boolean
        'Return ((Id Is Nothing) Or Id.Equals([Default]))
        Return (Id Is Nothing)
    End Function

    Public Overrides Function GetHashCode() As Int32
        Return Id.GetHashCode()
    End Function

    Private Function HasSameBusinessSignatureAs(ByVal compareTo As DomainObject(Of TKey)) As Boolean
        Return GetHashCode().Equals(compareTo.GetHashCode())
    End Function

    Private Function HasSameNonDefaultIdAs(ByVal compareTo As DomainObject(Of TKey)) As Boolean
        If (Id Is Nothing AndAlso compareTo.Id Is Nothing) Then
            Return True
        ElseIf ((Id Is Nothing And compareTo.Id IsNot Nothing) Or (Id IsNot Nothing And compareTo.Id Is Nothing)) Then
            Return False
            'ElseIf ([Default]() Is Nothing) Then
            '    Return False
            'ElseIf (Not Id.Equals([Default]) And Not compareTo.Id.Equals([Default])) Then
            '    Return True
        Else
            Return Id.Equals(compareTo.Id)
        End If
        'Return (Id Is Nothing And Not Id.Equals([Default])) And _
        '       (compareTo.Id Is Nothing And Not compareTo.Id.Equals([Default])) And _
        '       Id.Equals(compareTo.Id)
    End Function

    Public Overridable Function Clone() As Object Implements ICloneable.Clone

        Dim ms As New MemoryStream
        Dim bf As New BinaryFormatter
        bf.Serialize(ms, Me)
        ms.Position = 0
        Dim obj As Object = bf.Deserialize(ms)
        ms.Close()

        Return obj

    End Function

    ''' <summary> Formatta una data secondo il formato specificato </summary>
    Protected Function FormatDateTime(ByVal [date] As Date?, ByVal format As String) As String
        ' TODO: rimuovere da qui stà roba, possibilmente eliminarla
        If [date].HasValue Then
            Return String.Format(format, [date])
        Else
            Return String.Empty
        End If
    End Function

End Class