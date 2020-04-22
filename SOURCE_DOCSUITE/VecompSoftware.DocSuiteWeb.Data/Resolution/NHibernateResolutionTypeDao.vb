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
        Return criteria.List(Of ResolutionType)()
    End Function

End Class
