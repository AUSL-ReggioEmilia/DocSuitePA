Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports NHibernate.Event
Imports NHibernate.Persister.Entity

Public Class TenantInsertEventListener
    Implements IPreInsertEventListener

    Public Function OnPreInsert([event] As PreInsertEvent) As Boolean Implements IPreInsertEventListener.OnPreInsert
        If TypeOf [event].Entity Is ISupportTenant Then
            Dim evaluatedEntity As ISupportTenant = DirectCast([event].Entity, ISupportTenant)
            If Not evaluatedEntity.IdTenantAOO.HasValue Then
                Dim callback As Action(Of Object) = EventListenerUtil.CustomEventInstances.SingleOrDefault(Function(x) x.Key = NameOf(TenantInsertEventListener)).Value
                If callback IsNot Nothing Then
                    callback(evaluatedEntity)
                    SetNHProperty([event].Persister, [event].State, NameOf(ISupportTenant.IdTenantAOO), evaluatedEntity.IdTenantAOO)
                End If
            End If
        End If

        Return False
    End Function

    Public Function OnPreInsertAsync([event] As PreInsertEvent, cancellationToken As CancellationToken) As Task(Of Boolean) Implements IPreInsertEventListener.OnPreInsertAsync
        Return Task.Run(Function() OnPreInsert([event]), cancellationToken)
    End Function

    Private Sub SetNHProperty(persister As IEntityPersister, state As Object(), propertyName As String, value As Object)
        Dim index As Integer = Array.IndexOf(persister.PropertyNames, propertyName)
        If index = -1 Then Return
        state(index) = value
    End Sub
End Class