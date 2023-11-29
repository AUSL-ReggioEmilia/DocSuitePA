Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateTaskLockDao
    Inherits BaseNHibernateDao(Of TaskLock)

    ''' <summary>
    ''' Estrae i tasks per categoria
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTasksByType(ByVal type As String) As IList(Of TaskLock)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.TaskType", type))

        Return criteria.List(Of TaskLock)()
    End Function

    Public Function GetTasksBySession(ByVal session As Guid) As IList(Of TaskLock)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.Session", session))

        Return criteria.List(Of TaskLock)()
    End Function

    Public Function GetLastTaskWithDataByTaskId(ByVal type As String, ByVal taskId As String) As TaskLock
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Restrictions.Eq("Id.TaskType", type)).Add(Restrictions.Eq("Id.TaskId", taskId)).AddOrder(Order.Desc("Created"))

        Return criteria.UniqueResult(Of TaskLock)()
    End Function

    ''' <summary>
    ''' Cancella i tasks per categoria
    ''' </summary>
    ''' <param name="type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteTasksByType(ByVal type As String) As IList(Of TaskLock)
        Dim failedTasks As IList(Of TaskLock) = New List(Of TaskLock)
        Dim tasksToDelete As IList(Of TaskLock) = Me.GetTasksByType(type)

        Me.NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)

        Try
            For Each task As TaskLock In tasksToDelete
                Try
                    Me.Delete(task)
                Catch ex As Exception
                    failedTasks.Add(task)
                End Try
            Next

            If failedTasks.Count = 0 Then
                Me.NHibernateSession.Transaction.Commit()
            Else
                Me.NHibernateSession.Transaction.Rollback()
            End If
        Finally
            Me.NHibernateSession.Transaction.Dispose()
        End Try

        Return failedTasks
    End Function

    ''' <summary>
    ''' Cancella i tasks diversi dall'id di sessione passato
    ''' </summary>
    ''' <param name="session">L'id di sessione della corrente sessione di importazione</param>
    ''' <returns></returns>
    ''' <remarks>Poiche è permessa l'esecuzione di un solo processo di importazione alla volta, gli id sessione diversi dal corrente sono considerati + vecchi</remarks>
    Public Function DeleteOlderTasks(ByVal session As Guid) As IList(Of TaskLock)
        Dim failedTasks As New List(Of TaskLock)
        Dim tasksToDelete As IList(Of TaskLock)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)

        criteria.Add(Expression.Not(Restrictions.Eq("Id.Session", session)))
        criteria.Add(Restrictions.IsNull("Data"))

        tasksToDelete = criteria.List(Of TaskLock)()

        Me.NHibernateSession.BeginTransaction(IsolationLevel.ReadCommitted)

        Try
            For Each task As TaskLock In tasksToDelete
                Try
                    Me.Delete(task)
                Catch ex As Exception
                    failedTasks.Add(task)
                End Try
            Next

            If failedTasks Is Nothing OrElse failedTasks.Count > 0 Then
                Me.NHibernateSession.Transaction.Commit()
            Else
                Me.NHibernateSession.Transaction.Rollback()
            End If
        Finally
            Me.NHibernateSession.Transaction.Dispose()
        End Try

        Return failedTasks
    End Function

    Public Overrides Sub Save(ByRef entity As TaskLock)
        NHibernateSession.Merge(entity)
        NHibernateSession.Flush()
    End Sub
End Class
