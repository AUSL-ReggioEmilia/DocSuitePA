Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits

Public Module DocumentUnitModelEx

    <Extension()>
    Public Function FullUDNumber(source As DocumentUnitModel) As String
        Return String.Format("{0}/{1:0000000}", source.Year, source.Number)
    End Function

End Module
