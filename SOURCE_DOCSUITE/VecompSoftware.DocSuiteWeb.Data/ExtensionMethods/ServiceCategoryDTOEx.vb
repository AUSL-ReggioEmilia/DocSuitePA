Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API

Public Module ServiceCategoryDTOEx
    <Extension>
    Public Function CopyFrom(source As ServiceCategoryDTO, serviceCategory As ServiceCategory) As ServiceCategoryDTO
        source.Id = serviceCategory.Id
        source.Code = serviceCategory.Code
        source.Description = serviceCategory.Description
        Return source
    End Function
End Module
