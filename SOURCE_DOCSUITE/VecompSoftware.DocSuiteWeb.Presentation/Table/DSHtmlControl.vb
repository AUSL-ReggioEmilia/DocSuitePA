Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' Classe ce identifica un controllo HTML generico
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class DSHtmlControl
    Implements ICloneable

    ''' <summary>
    ''' Clona il controllo
    ''' </summary>
    Public Function Clone() As Object Implements System.ICloneable.Clone
        Dim obj As Object

        Using ms As New MemoryStream
            Dim bf As New BinaryFormatter
            bf.Serialize(ms, Me)
            ms.Position = 0
            obj = bf.Deserialize(ms)
            ms.Close()
        End Using

        Return obj

    End Function
End Class
