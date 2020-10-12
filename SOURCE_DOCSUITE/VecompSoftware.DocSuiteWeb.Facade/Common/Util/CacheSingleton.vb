Imports System.Collections.Concurrent
Imports VecompSoftware.DocSuiteWeb.Data

Public Class CacheSingleton
#Region " Fields "
    Private Shared ReadOnly _lock As Object = New Object()
    Private Shared _instance As CacheSingleton = Nothing
    Private ReadOnly _securityGroupsByUsers As ConcurrentDictionary(Of String, IList(Of SecurityGroups))

#End Region

#Region " Properties "
    Public ReadOnly Property SecurityGroupsByUsers As ConcurrentDictionary(Of String, IList(Of SecurityGroups))
        Get
            Return _securityGroupsByUsers
        End Get
    End Property

    Public Shared ReadOnly Property Instance As CacheSingleton
        Get
            SyncLock _lock
                If _instance Is Nothing Then
                    _instance = New CacheSingleton()
                End If
                Return _instance
            End SyncLock
        End Get
    End Property

#End Region

#Region " Constructors "

    Private Sub New()
        _securityGroupsByUsers = New ConcurrentDictionary(Of String, IList(Of SecurityGroups))
    End Sub

#End Region

#Region " Methods "

    Public Sub ClearSecurityCache()
        SyncLock _lock
            _securityGroupsByUsers.Clear()
        End SyncLock
    End Sub

#End Region

End Class



