Imports System.Linq
Imports System.Runtime.CompilerServices

Public Module OChartTransformerDTOExtensions

    <Extension()>
    Public Function OrderByPriority(ByVal source As IEnumerable(Of OChartTransformerDTO)) As IEnumerable(Of OChartTransformerDTO)
        Return source.OrderBy(Function(c) c.RegistrationDateOrDefault) _
            .ThenBy(Function(c) c.ReferenceDateOrDefault) _
            .ThenBy(Function(c) c.Type) _
            .ThenBy(Function(c) If(c.IsRoot, String.Empty, c.Parent.PaddedCode)) _
            .ThenBy(Function(c) c.PaddedCode)
    End Function

End Module
