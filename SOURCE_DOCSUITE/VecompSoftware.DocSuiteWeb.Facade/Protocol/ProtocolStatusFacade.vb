Imports System
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ProtocolStatusFacade
    Inherits BaseProtocolFacade(Of ProtocolStatus, String, NHibernateProtocolStatusDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetByIncremental(ByVal incremental As Integer) As IList(Of ProtocolStatus)
        Return _dao.GetByIncremental(incremental)
    End Function

    Public Function GetByDescription(ByVal description As String) As IList(Of ProtocolStatus)
        Return _dao.GetByDescription(description)
    End Function

    Public Function GetByProtocolStatusExclusion(ByVal statusExclusion As String) As IList(Of ProtocolStatus)
        Return _dao.GetByProtocolStatusExclusion(statusExclusion)
    End Function

End Class