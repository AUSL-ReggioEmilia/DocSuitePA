Imports System.Runtime.CompilerServices
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

Namespace ExtensionMethods

    Public Module PecMailEx

        ''' <summary>
        ''' Calcola la grandezza totale degli allegati di una PEC
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function TotalAttachmentSize(ByRef pecMail As PECMail) As Long
            Return pecMail.Attachments.Sum(Function(attach) attach.GetSize())
        End Function

    End Module
End Namespace