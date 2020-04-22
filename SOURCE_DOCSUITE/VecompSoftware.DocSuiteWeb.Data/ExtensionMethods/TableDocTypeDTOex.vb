Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API

Public Module TableDocTypeDTOex

    <Extension>
    Public Function CopyFrom(source As TableDocTypeDTO, tableDocType As DocumentType) As TableDocTypeDTO
        source.Id = tableDocType.Id
        source.Code = tableDocType.Code
        source.Description = tableDocType.Description
        Return source
    End Function
End Module
