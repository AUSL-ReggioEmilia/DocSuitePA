Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class FascicleFacade
    Inherits BaseProtocolFacade(Of Fascicle, Guid, NHibernateFascicleDao)

    Public Sub New()
        MyBase.New()
    End Sub


    Public Function GetByYearNumberCategory(year As Short, idCategory As Integer, number As Integer) As Fascicle
        Return _dao.GetByYearCategoryNumber(year, idCategory, number)
    End Function

    Public Function CountFascicleByCategory(category As Category) As Long
        Return _dao.CountFascicleByCategory(category)
    End Function


End Class