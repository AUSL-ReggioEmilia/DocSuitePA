Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.Services.Biblos.Models

Public Module DocumentDTOEx

    <Extension()>
    Public Function ToDocumentInfos(source As IDocumentDTO) As List(Of DocumentInfo)
        Dim dto As DocumentDTO = DirectCast(source, DocumentDTO)
        If dto.Content IsNot Nothing Then
            Dim memoryDocument As New MemoryDocumentInfo(dto.Content, dto.Name)
            Return New List(Of DocumentInfo) From {memoryDocument}
        End If

        If dto.HasBiblosGuid() Then
            Return New BiblosChainInfo(dto.GetBiblosGuid().Value).Documents.ToList()
        End If

        If dto.HasBiblosId() Then
            Return New BiblosChainInfo(dto.BiblosArchive, dto.BiblosId.Value).Documents.ToList()
        End If

        Dim fi As New FileInfo(dto.FullName)
        Dim fileDocument As New FileDocumentInfo(fi)
        Return New List(Of DocumentInfo) From {fileDocument}
    End Function

End Module
