Imports System
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class ComputerLogFacade
    Inherits BaseProtocolFacade(Of ComputerLog, String, NHibernateComputerLogDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetCurrent() As ComputerLog
        Return GetById(CommonShared.DSUserComputer)
    End Function

    Public Overloads Sub Update(ByVal systemComputer As String, ByVal value As Integer)
        Dim finder As New NHibernateComputerLogFinder()
        finder.SystemComputer = systemComputer
        Dim logs As IList(Of ComputerLog) = finder.DoSearch()
        ' Salvo il valore impostato
        If (logs Is Nothing) OrElse logs.Count <> 0 Then
            Dim item As ComputerLog = DirectCast(logs(0), ComputerLog)
            item.AdvancedScanner = value
            Update(item)
        End If
    End Sub


End Class