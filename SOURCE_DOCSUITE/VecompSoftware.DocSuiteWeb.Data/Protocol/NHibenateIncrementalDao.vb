Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao

Public Class NHibenateIncrementalDao
    Inherits BaseNHibernateDao(Of Incremental)

#Region " Constants "

    Private Const FactoryName As String = "ProtDB"

#End Region

#Region " Methods "

    Public Function GetFor(Of T)() As Incremental
        Dim incremental As Incremental = New Incremental With {.Name = GetType(T).Name, .Incremental = 0}

        Using session As ISession = NHibernateSessionManager.Instance.OpenSession(FactoryName)
            Dim query As IQuery = session.CreateSQLQuery("select * from Incremental with (xlock, rowlock) where Name = :name") _
                                  .AddEntity(incremental.GetType()).SetParameter("name", GetType(T).Name)
            Using tx As ITransaction = session.BeginTransaction(IsolationLevel.Serializable)
                Try
                    Dim found As Incremental = query.UniqueResult(Of Incremental)()
                    If found IsNot Nothing Then
                        session.Refresh(found)
                        incremental = found
                    End If

                    incremental.Incremental += 1
                    session.SaveOrUpdate(incremental)
                    tx.Commit()
                Catch ex As Exception
                    tx.Rollback()
                    Throw
                Finally
                    session.Flush()
                End Try
            End Using
        End Using

        Return incremental
    End Function

#End Region

End Class
