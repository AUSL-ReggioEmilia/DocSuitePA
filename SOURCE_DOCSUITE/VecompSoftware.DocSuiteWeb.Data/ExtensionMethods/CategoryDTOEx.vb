Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data

Public Module CategoryDTOEx

    <Extension()>
    Public Function CopyFrom(source As CategoryDTO, category As Category) As CategoryDTO
        source.Id = category.Id
        source.Name = category.Name
        Return source
    End Function

End Module