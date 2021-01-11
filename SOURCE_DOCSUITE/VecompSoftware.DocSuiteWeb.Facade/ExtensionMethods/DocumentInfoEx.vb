Imports System.IO
Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Namespace ExtensionMethods

    Public Module DocumentInfoEx

        <Extension()>
        Public Function SaveUniqueToTemp(ByVal doc As DocumentInfo, ByVal customFileName As String) As FileInfo
            Dim owner As String = DocSuiteContext.Current.User.UserName
            Return doc.SaveToDisk(CommonUtil.GetInstance().TempDirectory, FileHelper.UniqueFileNameFormat(customFileName, owner))
        End Function

        <Extension()>
        Public Function SaveUniqueToTemp(ByVal doc As DocumentInfo) As FileInfo
            Return doc.SaveUniqueToTemp(doc.Name)
        End Function

    End Module
End Namespace