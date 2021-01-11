Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DocumentSeriesAttributeBehaviourFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesAttributeBehaviour, Integer, NHibernateDocumentSeriesAttributeBehaviourDao)

    Public Function GetAttributeBehaviourGroups(series As DocumentSeries, action As DocumentSeriesAction) As IList(Of String)
        Return _dao.GetAttributeBehaviourGroups(series, action)
    End Function

    Public Function GetAttributeBehaviour(series As DocumentSeries, action As DocumentSeriesAction, group As String, attributeName As String) As DocumentSeriesAttributeBehaviour
        Dim temp As IList(Of DocumentSeriesAttributeBehaviour)
        temp = _dao.GetAttributeBehaviours(series, action, group, attributeName)
        If Not temp.IsNullOrEmpty() AndAlso temp.Count > 1 Then
            Throw New DocSuiteException(String.Format("Trovate {0} DocumentSeriesAttributeBehaviour per ({1})-({2})-({3})", temp.Count, series.Id, group, attributeName))
        End If
        If temp.Count = 0 Then
            Return Nothing
        End If
        Return temp(0)
    End Function

    Public Function GetAttributeBehaviours(series As DocumentSeries, action As DocumentSeriesAction, group As String) As IList(Of DocumentSeriesAttributeBehaviour)
        Return _dao.GetAttributeBehaviours(series, action, group)
    End Function

End Class
