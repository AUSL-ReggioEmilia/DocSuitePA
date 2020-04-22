
Imports System.ComponentModel
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Signer.Security

<DataObject()> _
Public Class TableLogFacade
    Inherits BaseProtocolFacade(Of TableLog, Integer, NHibernateTableLogDao)


#Region " Fields "
    Private _hashHelper As HashGenerator
#End Region

#Region " Properties "
    Private ReadOnly Property HashHelper As Helpers.Signer.Security.HashGenerator
        Get
            If _hashHelper Is Nothing Then
                _hashHelper = New Helpers.Signer.Security.HashGenerator()
            End If
            Return _hashHelper
        End Get
    End Property
#End Region
    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Save(ByRef obj As TableLog)
        Dim str As String = String.Concat(obj.SystemUser, "|", obj.LogType, "|", obj.LogDescription, "|", obj.Id, "|", obj.RegistrationDate.ToString("yyyyMMddHHmmss"))
        obj.Hash = HashHelper.GenerateHash(str)
        MyBase.Save(obj)
    End Sub

    Public Overrides Sub SaveWithoutTransaction(ByRef obj As TableLog)
        Dim str As String = String.Concat(obj.SystemUser, "|", obj.LogType, "|", obj.LogDescription, "|", obj.Id, "|", obj.RegistrationDate.ToString("yyyyMMddHHmmss"))
        obj.Hash = HashHelper.GenerateHash(str)
        MyBase.SaveWithoutTransaction(obj)
    End Sub
    Public Overridable Sub Insert(ByVal tableName As String, ByVal logType As Short, ByVal description As String, ByVal entityUniqueId As Guid)
        Dim table As TableLog = Insert(tableName, logType, description)
        table.EntityUniqueId = entityUniqueId
        Save(table)
    End Sub
    Public Overridable Sub Insert(ByVal tableName As String, ByVal logType As Short, ByVal description As String, ByVal entityId As Integer)
        Dim table As TableLog = Insert(tableName, logType, description)
        table.EntityId = entityId
        Save(table)
    End Sub
    Public Overridable Function Insert(ByVal tableName As String, ByVal logType As Short, ByVal description As String) As TableLog
        Dim table As New TableLog()
        table.LogDescription = description
        table.TableName = tableName
        table.LogType = logType
        table.SystemComputer = DocSuiteContext.Current.UserComputer
        table.SystemUser = DocSuiteContext.Current.User.FullUserName
        table.RegistrationDate = DateTimeOffset.UtcNow
        Return table
    End Function

End Class