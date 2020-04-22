Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data


Public Class ProtocolContactFinder
    Inherits NHibernateProtocolContactFinder2


#Region " Properties "

    Public IncludeManuals As Boolean?

#End Region

#Region " Methods "

    Public Overrides Function GetProtocolKeys() As IList(Of YearNumberCompositeKey)
        If Not IncludeManuals.GetValueOrDefault(False) Then
            Return MyBase.GetProtocolKeys()
        End If

        Dim commonResult As IList(Of YearNumberCompositeKey) = MyBase.GetProtocolKeys()
        Dim manualFinder As New ProtocolContactManualFinder() With {
            .IdContactIn = IdContactIn,
            .Year = Year,
            .IdProtocolIn = IdProtocolIn,
            .Description = Description,
            .DescriptionSearchBehaviour = DescriptionSearchBehaviour
        }
        Dim manualResult As IList(Of YearNumberCompositeKey) = manualFinder.GetProtocolKeys()
        Dim merged As New List(Of YearNumberCompositeKey)(commonResult)
        merged.AddRange(manualResult)
        Return merged.Distinct().ToList()
    End Function

#End Region

End Class
