Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports System.IO

<DataObject()> _
Public Class PECMailAttachmentFacade
    Inherits BaseProtocolFacade(Of PECMailAttachment, Integer, NHibernatePECMailAttachmentDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByMail(ByVal idMail As Integer) As IList(Of PECMailAttachment)
        Return _dao.GetByMail(idMail)
    End Function


    Public Function CheckFileName(ByVal fileNameAndPath As String) As Boolean
        Dim fileName As String = String.Empty
        Dim theDirectory As String = fileNameAndPath

        Dim p As Char = Path.DirectorySeparatorChar

        Dim splitPath() As String
        splitPath = fileNameAndPath.Split(p)
        If splitPath.Length > 1 Then
            fileName = splitPath(splitPath.Length - 1)
            theDirectory = String.Join(p, splitPath, 0, splitPath.Length - 1)
        End If

        For Each c As Char In Path.GetInvalidFileNameChars()
            If fileName.Contains(c) Then
                Return False
            End If
        Next

        For Each c As Char In Path.GetInvalidPathChars()
            If theDirectory.Contains(c) Then
                Return False
            End If
        Next
        Return True
    End Function

End Class

