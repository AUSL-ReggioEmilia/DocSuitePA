Imports System.Collections.Generic
Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate.Transform

Public Class NHibernateResolutionTypeDao
    Inherits BaseNHibernateDao(Of ResolutionType)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary> Elenco con descrizioni personalizzate per la configurazione corrente. </summary>
    Public Function GetResolutionTypeDictionary() As IList(Of ResolutionType)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TabMaster)("tm")
        With criteria
            .Add(Restrictions.Eq("tm.Id.Configuration", DocSuiteContext.Current.ResolutionEnv.Configuration))
            Dim proj As ProjectionList = Projections.ProjectionList()
            proj.Add(Projections.Property("tm.Id.ResolutionType"), "Id")
            proj.Add(Projections.Property("tm.Description"), "Description")
            .SetProjection(proj)
            ' Popolo un oggetto già esistente onde evitare di creare un nuovo dto per 2 property... - FG
            .SetResultTransformer(Transformers.AliasToBean(Of ResolutionType))
        End With
        Dim result As IList(Of ResolutionType)
        Using transaction As ITransaction = NHibernateSession.BeginTransaction(IsolationLevel.ReadUncommitted)
            Try
                result = criteria.List(Of ResolutionType)()
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using
        Return result
    End Function

End Class
