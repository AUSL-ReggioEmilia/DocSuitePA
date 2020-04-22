Imports System.Xml.Serialization

Public Enum ResolutionPublicationStatus
    Publicated
    [Error]
End Enum

<Serializable()>
Public Class ResolutionMultiPublication
  

#Region " Properties "
    <XmlAttribute()>
    Public Property IdResolution As Integer
    Public Property Year As Short?
    Public Property ServiceNumber As String
    Public Property [Object] As String
    Public Property DocumentName As String
    Public Property FileName As String
    Public Property ResolutionStatus As ResolutionPublicationStatus?

    <XmlIgnore>
    Public ReadOnly Property ResolutionNumber As String
        Get
            Dim sNum As String = "*"
            If Year.HasValue Then
                sNum = Year.Value & "/"
            Else
                Return sNum
            End If
            If Not String.IsNullOrEmpty(ServiceNumber) Then
                sNum &= ServiceNumber
            Else
                sNum &= "*"
            End If
            Return sNum
        End Get
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region

End Class
