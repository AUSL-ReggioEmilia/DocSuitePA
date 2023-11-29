Imports NHibernate
Imports System
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DaoManager

<Serializable()>
Public MustInherit Class CommonFacade(Of T, IdT, DaoType As INHibernateDao(Of T))
    Inherits FacadeNHibernateBase(Of T, IdT, DaoType)

    Private Delegate Sub DaoFunction(ByRef entity As T, ByVal DbName As String)

    Public Sub New()
        MyBase.New()
        _dbName = ProtDB
    End Sub

    Public Sub New(ByVal dbName As String)
        MyBase.New(dbName)
    End Sub

#Region " Metti che questi prima o poi li sposto in un helper... "

    ''' <summary>Cerca di capire in base allo stacktrace su che db devo aggiornare i contatori</summary>
    <Obsolete("Ora che non usiamo più SPRING non ha più senso fatta così")>
    Private Function getDbNameFromStackTrace() As String
        Dim trace As New StackTrace(True)
        For item As Integer = 0 To trace.FrameCount - 1
            Dim fullName As String = trace.GetFrame(item).GetMethod().ReflectedType.FullName
            If fullName.Contains("Prot") OrElse fullName.Contains("Fasc") Then
                Return ProtDB
            ElseIf fullName.Contains("Docm") Then
                Return DocmDB
            ElseIf fullName.Contains("Resl") Then
                Return ReslDB
            End If
        Next

        Throw New Exception("Se passa di qua fatemi un fischio - FG")
        'Return DirectCast(ContextRegistry.GetContext().GetObject("UnitOfWork"), IUnitOfWork).DbName ' Da verificare che fa sta cosa... - FG
    End Function

    Private Sub endSession(session As ISession)
        If session.IsOpen Then
            session.Flush()
            session.Close()
        End If
        session.Dispose()
        session = Nothing
    End Sub
    Protected Sub SafeCommitTransaction(session As ISession)
        If session IsNot Nothing _
            AndAlso session.Transaction IsNot Nothing _
            AndAlso session.Transaction.IsActive Then
            session.Transaction.Commit()
        End If
    End Sub
    Protected Sub SafeRollbackTransaction(session As ISession)
        If session IsNot Nothing _
            AndAlso session.Transaction IsNot Nothing _
            AndAlso session.Transaction.IsActive Then
            session.Transaction.Rollback()
        End If
    End Sub

#End Region

    Public Overrides Sub Save(ByRef entity As T)
        Dim sessionOfProtocol, sessionOfDocument, sessionOfResolution As ISession
        sessionOfProtocol = Nothing
        sessionOfResolution = Nothing
        sessionOfDocument = Nothing
        Dim c_step As String = "init"
        Try
            ' Escludo la tabella parameter perchè nei vari database, pur avendo lo stesso nome, contiene informazioni differenti.
            ' P.S. sono i contatori, non cazzeggiate qua per favore. - FG
            If GetType(T).FullName.Equals("VecompSoftware.DocSuiteWeb.Data.Parameter", StringComparison.InvariantCultureIgnoreCase) Then
                If String.IsNullOrEmpty(_dbName) Then
                    Throw New DocSuiteException("Save") With {.Descrizione = "(CommonFacade.Save) Errore in configurazione Dao. Contattare l'assistenza."}
                End If
                c_step = "VecompSoftware.DocSuiteWeb.Data.Parameter"
                Save(entity, _dbName)
            Else
                Dim uow As New NHibernateUnitOfWork()
                Dim cloneable As ICloneable = entity
                Dim protocolEntity As ICloneable = cloneable.Clone()
                sessionOfProtocol = NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)
                sessionOfProtocol.BeginTransaction(IsolationLevel.ReadCommitted)
                uow.DbName = ProtDB
                uow.Clear()
                c_step = "NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)"
                Save(protocolEntity, ProtDB, False)
                uow.Detach(protocolEntity)
                If DocSuiteContext.Current.IsResolutionEnabled Then
                    Dim resolutionEntity As ICloneable = cloneable.Clone()
                    sessionOfResolution = NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)
                    sessionOfResolution.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = ReslDB
                    uow.Clear()
                    c_step = "NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)"
                    Save(resolutionEntity, ReslDB, False)
                    uow.Detach(resolutionEntity)
                End If
                If DocSuiteContext.Current.IsDocumentEnabled Then
                    Dim documentEntity As ICloneable = cloneable.Clone()
                    sessionOfDocument = NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)
                    sessionOfDocument.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = DocmDB
                    uow.Clear()
                    c_step = "NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)"
                    Save(documentEntity, DocmDB, False)
                    uow.Detach(documentEntity)
                End If
            End If
            c_step = "safeCommitTransaction(sessionOfProtocol)"
            SafeCommitTransaction(sessionOfProtocol)
            c_step = "safeCommitTransaction(sessionOfResolution)"
            SafeCommitTransaction(sessionOfResolution)
            c_step = "safeCommitTransaction(sessionOfDocument)"
            SafeCommitTransaction(sessionOfDocument)
        Catch ex As Exception
            FileLogger.Error(LogName.FileLog, c_step, ex)
            SafeRollbackTransaction(sessionOfProtocol)
            SafeRollbackTransaction(sessionOfResolution)
            SafeRollbackTransaction(sessionOfDocument)
            Throw ex
        End Try
    End Sub

    Public Overrides Sub Update(ByRef entity As T)
        Dim sessionOfProtocol, sessionOfDocument, sessionOfResolution As ISession
        sessionOfProtocol = Nothing
        sessionOfResolution = Nothing
        sessionOfDocument = Nothing

        Try
            ' Escludo la tabella parameter perchè nei vari database, pur avendo lo stesso nome, contiene informazioni differenti.
            If GetType(T).FullName.Equals("VecompSoftware.DocSuiteWeb.Data.Parameter", StringComparison.InvariantCultureIgnoreCase) Then
                If String.IsNullOrEmpty(_dbName) Then
                    Throw New DocSuiteException("Update") With {.Descrizione = "(CommonFacade.Update) Errore di configurazione Dao. Contattare l'assistenza."}
                End If
                Update(entity, _dbName)
            Else
                Dim uow As New NHibernateUnitOfWork()
                Dim cloneable As ICloneable = entity
                Dim protocolEntity As ICloneable = cloneable.Clone()
                sessionOfProtocol = NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)
                sessionOfProtocol.BeginTransaction(IsolationLevel.ReadCommitted)
                uow.DbName = ProtDB
                uow.Clear()
                Update(protocolEntity, ProtDB)
                uow.Detach(protocolEntity)
                If DocSuiteContext.Current.IsResolutionEnabled Then
                    Dim resolutionEntity As ICloneable = cloneable.Clone()
                    sessionOfResolution = NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)
                    sessionOfResolution.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = ReslDB
                    uow.Clear()
                    Update(resolutionEntity, ReslDB)
                    uow.Detach(resolutionEntity)
                End If
                If DocSuiteContext.Current.IsDocumentEnabled Then
                    Dim documentEntity As ICloneable = cloneable.Clone()
                    sessionOfDocument = NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)
                    sessionOfDocument.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = DocmDB
                    uow.Clear()
                    Update(documentEntity, DocmDB)
                    uow.Detach(documentEntity)
                End If
            End If
            SafeCommitTransaction(sessionOfProtocol)
            SafeCommitTransaction(sessionOfResolution)
            SafeCommitTransaction(sessionOfDocument)
        Catch ex As Exception
            SafeRollbackTransaction(sessionOfProtocol)
            SafeRollbackTransaction(sessionOfResolution)
            SafeRollbackTransaction(sessionOfDocument)
            Throw ex
        End Try
    End Sub
    Public Overrides Sub UpdateOnly(ByRef entity As T)
        Dim sessionOfProtocol, sessionOfDocument, sessionOfResolution As ISession
        sessionOfProtocol = Nothing
        sessionOfResolution = Nothing
        sessionOfDocument = Nothing

        Try
            ' La tabella parameter, pur essendo presente in tutti e tre i database, contiene in ciascuno informazioni differenti.
            ' P.S. intendo i progressivi, non cazzeggiate qua per favore. - FG
            If GetType(T).FullName.Equals("VecompSoftware.DocSuiteWeb.Data.Parameter", StringComparison.InvariantCultureIgnoreCase) Then
                If String.IsNullOrEmpty(_dbName) Then
                    Throw New DocSuiteException("UpdateOnly") With {.Descrizione = "(CommonFacade.UpdateOnly) Errore di configurazione Dao. Contattare l'assistenza."}
                End If
                UpdateOnly(entity, _dbName)
                'UpdateOnly(entity, getDbNameFromStackTrace())
            Else
                Dim uow As NHibernateUnitOfWork = New NHibernateUnitOfWork()
                Dim cloneable As ICloneable = entity
                Dim protocolEntity As ICloneable = cloneable.Clone()
                sessionOfProtocol = NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)
                sessionOfProtocol.BeginTransaction(IsolationLevel.ReadCommitted)
                uow.DbName = ProtDB
                uow.Clear()
                UpdateOnly(protocolEntity, ProtDB)
                uow.Detach(protocolEntity)
                If DocSuiteContext.Current.IsResolutionEnabled Then
                    Dim resolutionEntity As ICloneable = cloneable.Clone()
                    sessionOfResolution = NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)
                    sessionOfResolution.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = ReslDB
                    uow.Clear()
                    UpdateOnly(resolutionEntity, ReslDB)
                    uow.Detach(resolutionEntity)
                End If
                If DocSuiteContext.Current.IsDocumentEnabled Then
                    Dim documentEntity As ICloneable = cloneable.Clone()
                    sessionOfDocument = NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)
                    sessionOfDocument.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = DocmDB
                    uow.Clear()
                    UpdateOnly(documentEntity, DocmDB)
                    uow.Detach(documentEntity)
                End If
            End If
            SafeCommitTransaction(sessionOfProtocol)
            SafeCommitTransaction(sessionOfResolution)
            SafeCommitTransaction(sessionOfDocument)
        Catch ex As Exception
            SafeRollbackTransaction(sessionOfProtocol)
            SafeRollbackTransaction(sessionOfResolution)
            SafeRollbackTransaction(sessionOfDocument)
            Throw ex
        End Try
    End Sub
    Public Overrides Function Delete(ByRef entity As T) As Boolean
        Dim retval As Boolean = False
        Dim sessionOfProtocol, sessionOfDocument, sessionOfResolution As ISession
        sessionOfProtocol = Nothing
        sessionOfResolution = Nothing
        sessionOfDocument = Nothing

        Try
            ' La tabella parameter, pur essendo presente in tutti e tre i database, contiene in ciascuno informazioni differenti.
            ' P.S. intendo i progressivi, non cazzeggiate qua per favore. - FG
            If GetType(T).FullName.Equals("VecompSoftware.DocSuiteWeb.Data.Parameter", StringComparison.InvariantCultureIgnoreCase) Then
                If String.IsNullOrEmpty(_dbName) Then
                    Throw New DocSuiteException("Delete") With {.Descrizione = "(CommonFacade.Delete) Errore di configurazione Dao. Contattare l'assistenza."}
                End If
                Delete(entity, _dbName)
                'Delete(entity, getDbNameFromStackTrace())
            Else
                Dim isEntityUsed As Boolean = IsUsed(entity)

                If GetType(T).GetInterface("ISupportShortLogicDelete") IsNot Nothing AndAlso isEntityUsed Then
                    Dim deletion As ISupportShortLogicDelete = entity
                    deletion.IsActive = 0S
                    UpdateOnly(deletion)
                    Return False
                End If

                If GetType(T).GetInterface("ISupportBooleanLogicDelete") IsNot Nothing AndAlso isEntityUsed Then
                    Dim deletion As ISupportBooleanLogicDelete = entity
                    deletion.IsActive = False
                    UpdateOnly(deletion)
                    Return False
                End If

                Dim uow As New NHibernateUnitOfWork()
                Dim cloneable As ICloneable = entity
                Dim protocolEntity As ICloneable = cloneable.Clone()
                sessionOfProtocol = NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)
                sessionOfProtocol.BeginTransaction(IsolationLevel.ReadCommitted)
                uow.DbName = ProtDB
                uow.Clear()
                Delete(protocolEntity, ProtDB)
                uow.Detach(protocolEntity)
                If DocSuiteContext.Current.IsResolutionEnabled Then
                    Dim resolutionEntity As ICloneable = cloneable.Clone()
                    sessionOfResolution = NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)
                    sessionOfResolution.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = ReslDB
                    uow.Clear()
                    Delete(resolutionEntity, ReslDB)
                    uow.Detach(resolutionEntity)
                End If
                If DocSuiteContext.Current.IsDocumentEnabled Then
                    Dim documentEntity As ICloneable = cloneable.Clone()
                    sessionOfDocument = NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)
                    sessionOfDocument.BeginTransaction(IsolationLevel.ReadCommitted)
                    uow.DbName = DocmDB
                    uow.Clear()
                    Delete(documentEntity, DocmDB)
                    uow.Detach(documentEntity)
                End If
            End If
            SafeCommitTransaction(sessionOfProtocol)
            SafeCommitTransaction(sessionOfResolution)
            SafeCommitTransaction(sessionOfDocument)
            retval = True
        Catch ex As Exception
            SafeRollbackTransaction(sessionOfProtocol)
            SafeRollbackTransaction(sessionOfResolution)
            SafeRollbackTransaction(sessionOfDocument)
            Throw
        End Try

        Return retval
    End Function

    Public Overrides Sub SaveWithoutTransaction(ByRef obj As T)
        Save(obj, ProtDB, False)
        NHibernateSessionManager.Instance.GetSessionFrom(ProtDB).Evict(obj)
        If DocSuiteContext.Current.IsResolutionEnabled Then
            Save(obj, ReslDB, False)
            NHibernateSessionManager.Instance.GetSessionFrom(ReslDB).Evict(obj)
        End If
        If DocSuiteContext.Current.IsDocumentEnabled Then
            Save(obj, DocmDB, False)
            NHibernateSessionManager.Instance.GetSessionFrom(DocmDB).Evict(obj)
        End If
        _dao.ConnectionName = ProtDB
    End Sub

    Public Sub CommonTransactionalActions(protocolAction As Action)
        ExecuteCommonTransactionalActions(protocolAction, Nothing, Nothing)
    End Sub

    Public Sub CommonTransactionalActions(protocolAction As Action, resolutionAction As Action)
        ExecuteCommonTransactionalActions(protocolAction, resolutionAction, Nothing)
    End Sub

    Public Sub CommonTransactionalActions(protocolAction As Action, resolutionAction As Action, documentAction As Action)
        ExecuteCommonTransactionalActions(protocolAction, resolutionAction, documentAction)
    End Sub

    Public Sub CommonTransactionalSingleAction(transactionalAction As Action)
        ExecuteCommonTransactionalSingleAction(transactionalAction)
    End Sub

    Private Sub ExecuteCommonTransactionalActions(protocolAction As Action, resolutionAction As Action, documentAction As Action)
        Dim sessionProtocol As ISession = Nothing
        Dim sessionResolution As ISession = Nothing
        Dim sessionDocument As ISession = Nothing
        Try
            If protocolAction IsNot Nothing Then
                sessionProtocol = NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)
                sessionProtocol.BeginTransaction(IsolationLevel.ReadCommitted)
                protocolAction()
            End If
            If DocSuiteContext.Current.IsResolutionEnabled AndAlso resolutionAction IsNot Nothing Then
                sessionResolution = NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)
                sessionResolution.BeginTransaction(IsolationLevel.ReadCommitted)
                resolutionAction()
            End If
            If DocSuiteContext.Current.IsDocumentEnabled AndAlso documentAction IsNot Nothing Then
                sessionDocument = NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)
                sessionDocument.BeginTransaction(IsolationLevel.ReadCommitted)
                documentAction()
            End If
            SafeCommitTransaction(sessionProtocol)
            SafeCommitTransaction(sessionResolution)
            SafeCommitTransaction(sessionDocument)
        Catch ex As Exception
            SafeRollbackTransaction(sessionProtocol)
            SafeRollbackTransaction(sessionResolution)
            SafeRollbackTransaction(sessionDocument)
            FileLogger.Error(LoggerName, "Errore nell'esecuzione dell'attività", ex)
            Throw ex
        End Try
    End Sub

    Private Sub ExecuteCommonTransactionalSingleAction(transactionalAction As Action)
        If transactionalAction Is Nothing Then
            Exit Sub
        End If

        Dim sessionProtocol As ISession = Nothing
        Dim sessionResolution As ISession = Nothing
        Dim sessionDocument As ISession = Nothing
        Try
            sessionProtocol = NHibernateSessionManager.Instance.GetSessionFrom(ProtDB)
            sessionProtocol.BeginTransaction(IsolationLevel.ReadCommitted)
            If DocSuiteContext.Current.IsResolutionEnabled Then
                sessionResolution = NHibernateSessionManager.Instance.GetSessionFrom(ReslDB)
                sessionResolution.BeginTransaction(IsolationLevel.ReadCommitted)
            End If
            If DocSuiteContext.Current.IsDocumentEnabled Then
                sessionDocument = NHibernateSessionManager.Instance.GetSessionFrom(DocmDB)
                sessionDocument.BeginTransaction(IsolationLevel.ReadCommitted)
            End If

            transactionalAction()

            SafeCommitTransaction(sessionProtocol)
            SafeCommitTransaction(sessionResolution)
            SafeCommitTransaction(sessionDocument)
        Catch ex As Exception
            SafeRollbackTransaction(sessionProtocol)
            SafeRollbackTransaction(sessionResolution)
            SafeRollbackTransaction(sessionDocument)
            FileLogger.Error(LoggerName, "Errore nell'esecuzione dell'attività", ex)
            Throw ex
        End Try
    End Sub
End Class