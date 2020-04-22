Imports NHibernate
Imports NHibernate.Criterion
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.NHibernate
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao


Public Class NHibernateAPIProviderDao
    Inherits BaseNHibernateDao(Of APIProvider)

#Region " Constants "
#End Region

#Region " Methods "

    Public Function GetEnabled() As IList(Of APIProvider)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of APIProvider)()
        criteria.Add(Restrictions.Eq("Enabled", True))
        Return criteria.List(Of APIProvider)()
    End Function
    Public Function GetByCode(code As String) As APIProvider
        Dim enabled As IList(Of APIProvider) = GetAll()
        Return enabled.FirstOrDefault(Function(p) p.IsEnabled AndAlso p.Code.Eq(code))
    End Function

    Public Function Discard(providers As IEnumerable(Of APIProvider)) As Integer
        Dim codes As String() = providers.Select(Function(p) p.Code).ToArray()
        Using stateless As IStatelessSession = NHibernateSessionManager.Instance.OpenStatelessSession(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
            Dim queryString As String = String.Format("delete from {0} m where m.Code in (:codes) and (m.Preserved is null or not m.Preserved = 1)", GetType(APIProvider).Name)
            stateless.CreateQuery(queryString) _
                .SetParameterList("codes", codes) _
                .ExecuteUpdate()
        End Using
    End Function
    Public Sub Renew(providers As IEnumerable(Of APIProvider))
        Dim preserved As IEnumerable(Of APIProvider) = GetAll().Where(Function(p) p.IsPreserved)
        Dim renewable As IEnumerable(Of APIProvider) = providers.Where(Function(r) Not preserved.Any(Function(p) p.Code = r.Code))

        Discard(renewable)
        Using stateless As IStatelessSession = NHibernateSessionManager.Instance.OpenStatelessSession(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
            stateless.SetBatchSize(renewable.Count())
            Using transaction As ITransaction = stateless.BeginTransaction()
                For Each item As APIProvider In renewable
                    stateless.Insert(New APIProvider(item))
                Next
                transaction.Commit()
            End Using
        End Using
    End Sub

#End Region

End Class
