Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports System.Linq
Imports VecompSoftware.Services.Logging

<DataObject()>
Public Class CategorySchemaFacade
    Inherits CommonFacade(Of CategorySchema, Guid, NHibernateCategorySchemaDao)

#Region "Fields"
    Private ReadOnly _userName As String
#End Region

#Region "Properties"

#End Region

#Region "Constructor"

    Public Sub New(ByVal dbName As String, ByVal userName As String)
        MyBase.New(dbName)
        _userName = userName
    End Sub


#End Region

#Region "Methods"

    ''' <summary>
    ''' Ritorna il CategorySchema ad oggi attivo
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCurrentCategorySchema() As CategorySchema
        Return GetActiveCategorySchema(DateTimeOffset.UtcNow)
    End Function

    ''' <summary>
    ''' Ritorna il CategorySchema attivo in base alla StartDate passata
    ''' </summary>
    ''' <param name="date"></param>
    ''' <returns></returns>
    Public Function GetActiveCategorySchema([date] As DateTimeOffset) As CategorySchema
        Return _dao.GetActiveCategorySchema([date])
    End Function

    ''' <summary>
    ''' Ritorna la lista di CategorySchema che è possibile gestire (presente e passati)
    ''' </summary>
    ''' <param name="[date]"></param>
    ''' <returns></returns>
    Public Function GetManageableCategorySchemas([date] As DateTimeOffset) As ICollection(Of CategorySchema)
        Return _dao.GetManageableCategorySchemas([date])
    End Function


    Public Function GetMaxVersion() As Short
        Return _dao.GetMaxVersion()
    End Function

    Public Function GetCategorySchemaByVersion(version As Short) As CategorySchema
        Return _dao.GetCategorySchemaByVersion(version)
    End Function

    'TODO: E' il caso di prevedere un manager per le StatelessSession?
    Public Sub CreateSchema(ByRef categorySchema As CategorySchema)
        Dim protSession As IStatelessSession = Nothing
        Dim reslSession As IStatelessSession = Nothing
        Dim docmSession As IStatelessSession = Nothing
        Try
            protSession = NHibernateSessionManager.Instance.OpenStatelessSession(ProtDB)
            Dim protTransaction As ITransaction = protSession.BeginTransaction(IsolationLevel.ReadCommitted)

            Dim reslTransaction As ITransaction = Nothing
            Dim docmTransaction As ITransaction = Nothing
            If DocSuiteContext.Current.IsResolutionEnabled Then
                reslSession = NHibernateSessionManager.Instance.OpenStatelessSession(ReslDB)
                reslTransaction = reslSession.BeginTransaction(IsolationLevel.ReadCommitted)
            End If
            If DocSuiteContext.Current.IsDocumentEnabled Then
                docmSession = NHibernateSessionManager.Instance.OpenStatelessSession(DocmDB)
                docmTransaction = docmSession.BeginTransaction(IsolationLevel.ReadCommitted)
            End If

            Try
                categorySchema.Version = GetMaxVersion() + 2S
                Dim previousSchema As CategorySchema = GetCategorySchemaByVersion(categorySchema.Version - 1S)
                categorySchema.RegistrationDate = DateTimeOffset.UtcNow
                categorySchema.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                protSession.Insert(categorySchema)

                If reslSession IsNot Nothing Then
                    reslSession.Insert(categorySchema)
                End If

                If docmSession IsNot Nothing Then
                    docmSession.Insert(categorySchema)
                End If

                If previousSchema IsNot Nothing Then
                    previousSchema.EndDate = categorySchema.StartDate
                    previousSchema.LastChangedDate = DateTimeOffset.UtcNow
                    previousSchema.LastChangedUser = DocSuiteContext.Current.User.FullUserName

                    protSession.Update(previousSchema)
                    If reslSession IsNot Nothing Then
                        reslSession.Update(previousSchema)
                    End If

                    If docmSession IsNot Nothing Then
                        docmSession.Update(previousSchema)
                    End If

                    For Each category As Category In previousSchema.Categories.Where(Function(x) Not x.EndDate.HasValue)
                        category.EndDate = previousSchema.EndDate
                        category.LastChangedDate = DateTimeOffset.UtcNow
                        category.LastChangedUser = DocSuiteContext.Current.User.FullUserName
                        protSession.Update(category)

                        If docmSession IsNot Nothing Then
                            docmSession.Update(category)
                        End If

                        If reslSession IsNot Nothing Then
                            reslSession.Update(category)
                        End If

                    Next
                End If

                protTransaction.Commit()
                If reslTransaction IsNot Nothing Then
                    reslTransaction.Commit()
                End If
                If docmTransaction IsNot Nothing Then
                    docmTransaction.Commit()
                End If
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                protTransaction.Rollback()
                If reslTransaction IsNot Nothing Then
                    reslTransaction.Rollback()
                End If

                If docmTransaction IsNot Nothing Then
                    docmTransaction.Rollback()
                End If
                Throw ex
            End Try
        Finally
            If protSession IsNot Nothing AndAlso protSession.IsOpen Then
                protSession.Dispose()
            End If
            If reslSession IsNot Nothing AndAlso reslSession.IsOpen Then
                reslSession.Dispose()
            End If
            If docmSession IsNot Nothing AndAlso docmSession.IsOpen Then
                docmSession.Dispose()
            End If
        End Try
    End Sub

    Public Sub DeleteSchema(ByRef categorySchema As CategorySchema)
        Dim protSession As IStatelessSession = Nothing
        Dim reslSession As IStatelessSession = Nothing
        Dim docmSession As IStatelessSession = Nothing
        Try
            protSession = NHibernateSessionManager.Instance.OpenStatelessSession(ProtDB)
            Dim protTransaction As ITransaction = protSession.BeginTransaction(IsolationLevel.ReadCommitted)

            Dim reslTransaction As ITransaction = Nothing
            Dim docmTransaction As ITransaction = Nothing
            If DocSuiteContext.Current.IsResolutionEnabled Then
                reslSession = NHibernateSessionManager.Instance.OpenStatelessSession(ReslDB)
                reslTransaction = reslSession.BeginTransaction(IsolationLevel.ReadCommitted)
            End If
            If DocSuiteContext.Current.IsDocumentEnabled Then
                docmSession = NHibernateSessionManager.Instance.OpenStatelessSession(DocmDB)
                docmTransaction = docmSession.BeginTransaction(IsolationLevel.ReadCommitted)
            End If

            Try
                Dim previousSchema As CategorySchema = GetCategorySchemaByVersion(categorySchema.Version - 1S)
                protSession.Delete(categorySchema)

                If reslSession IsNot Nothing Then
                    reslSession.Delete(categorySchema)
                End If

                If docmSession IsNot Nothing Then
                    docmSession.Delete(categorySchema)
                End If

                Dim previousSchemaEndDate As DateTimeOffset? = Nothing
                If previousSchema IsNot Nothing Then
                    previousSchemaEndDate = previousSchema.EndDate
                    previousSchema.EndDate = Nothing
                    previousSchema.LastChangedDate = DateTimeOffset.UtcNow
                    previousSchema.LastChangedUser = DocSuiteContext.Current.User.FullUserName

                    protSession.Update(previousSchema)
                    If reslSession IsNot Nothing Then
                        reslSession.Update(previousSchema)
                    End If

                    If docmSession IsNot Nothing Then
                        docmSession.Update(previousSchema)
                    End If

                    For Each category As Category In previousSchema.Categories.Where(Function(x) x.EndDate.HasValue AndAlso x.EndDate.Value = previousSchemaEndDate.Value)
                        category.EndDate = Nothing
                        category.LastChangedDate = DateTimeOffset.UtcNow
                        category.LastChangedUser = DocSuiteContext.Current.User.FullUserName

                        protSession.Update(category)
                        If reslSession IsNot Nothing Then
                            reslSession.Update(category)
                        End If

                        If docmSession IsNot Nothing Then
                            docmSession.Update(category)
                        End If
                    Next
                End If

                protTransaction.Commit()
                If reslTransaction IsNot Nothing Then
                    reslTransaction.Commit()
                End If

                If docmTransaction IsNot Nothing Then
                    docmTransaction.Commit()
                End If
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                protTransaction.Rollback()
                If reslTransaction IsNot Nothing Then
                    reslTransaction.Rollback()
                End If

                If docmTransaction IsNot Nothing Then
                    docmTransaction.Rollback()
                End If
                Throw ex
            End Try
        Finally
            If protSession IsNot Nothing AndAlso protSession.IsOpen Then
                protSession.Dispose()
            End If
            If reslSession IsNot Nothing AndAlso reslSession.IsOpen Then
                reslSession.Dispose()
            End If
            If docmSession IsNot Nothing AndAlso docmSession.IsOpen Then
                docmSession.Dispose()
            End If
        End Try
    End Sub
#End Region

End Class

