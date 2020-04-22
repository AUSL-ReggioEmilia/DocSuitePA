
Imports System.Xml
Imports System.IO

Public Class SignatureDTDResolver
    Inherits XmlUrlResolver

    Private _signatureDTDPath As String
    Private ReadOnly Property SignatureDTDPath() As String
        Get
            Return Me._signatureDTDPath
        End Get
    End Property

    Public Sub New(ByVal signaturaDtdPath As String)
        Me._signatureDTDPath = signaturaDtdPath
    End Sub

    Public Overrides Function GetEntity(ByVal absoluteUri As System.Uri, ByVal role As String, ByVal ofObjectToReturn As System.Type) As Object
        If (File.Exists(Me.SignatureDTDPath)) Then
            Return New FileStream(Me.SignatureDTDPath, FileMode.Open, FileAccess.Read, FileShare.Read)
        End If
        Return MyBase.GetEntity(absoluteUri, role, ofObjectToReturn)
    End Function
End Class
