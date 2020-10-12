Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.DocumentUnits

Public Class DocumentUnitFacade
    Inherits BaseProtocolFacade(Of DocumentUnit, Guid, DocumentUnitDao)

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Methods "
    Public Overrides Function Delete(ByRef obj As DocumentUnit) As Boolean
        Throw New NotImplementedException()
    End Function
    Public Overrides Function Delete(ByRef obj As DocumentUnit, dbName As String, Optional needTransaction As Boolean = True) As Boolean
        Throw New NotImplementedException()
    End Function
    Public Overrides Function DeleteWithoutTransaction(ByRef obj As DocumentUnit) As Boolean
        Throw New NotImplementedException()
    End Function
    Public Overrides Sub Recover(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub Recover(ByRef obj As DocumentUnit, dbName As String)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub Save(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub Save(ByRef obj As DocumentUnit, dbName As String, Optional needTransaction As Boolean = True)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub SaveWithoutTransaction(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub Update(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub Update(ByRef obj As DocumentUnit, dbName As String, Optional needTransaction As Boolean = True)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateNeedAuditable(ByRef obj As DocumentUnit, dbName As String, Optional needAuditable As Boolean = True, Optional needTransaction As Boolean = True)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateNeedAuditable(ByRef obj As DocumentUnit, needAuditable As Boolean)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateNoLastChange(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateNoLastChange(ByRef obj As DocumentUnit, dbName As String)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateOnly(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateOnly(ByRef obj As DocumentUnit, dbName As String, Optional needTransaction As Boolean = True)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateOnlyWithoutTransaction(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
    Public Overrides Sub UpdateWithoutTransaction(ByRef obj As DocumentUnit)
        Throw New NotImplementedException()
    End Sub
#End Region
End Class