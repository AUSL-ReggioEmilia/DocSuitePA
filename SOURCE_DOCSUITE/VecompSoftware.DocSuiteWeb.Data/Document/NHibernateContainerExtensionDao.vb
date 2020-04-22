Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.NHibernateManager.Dao
Imports NHibernate
Imports NHibernate.Criterion

Public Class NHibernateContainerExtensionDao
    Inherits BaseNHibernateDao(Of ContainerExtension)

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetAllAccountingSectionals() As IList(Of ContainerExtension)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        'Da errore di mapping ... da risolvere quando si determia il vero motivo
        'criteria.Add(Expression.IsNotNull("Id.AccountingSectionalNumber"))
        criteria.Add(Restrictions.Eq("Id.KeyType", ContainerExtensionType.SC.ToString()))
        Return criteria.List(Of ContainerExtension)().Where(Function(f) f.AccountingSectionalNumber.HasValue).ToList()
    End Function

    Public Function GetByContainerAndKey(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType) As IList(Of ContainerExtension)
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.idContainer", pIdContainer))
        criteria.Add(Restrictions.Eq("Id.KeyType", pKey.ToString()))
        Return criteria.List(Of ContainerExtension)()
    End Function

    Public Function GetMaxIdByContainerAndKey(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType) As Short
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persitentType)
        criteria.Add(Restrictions.Eq("Id.idContainer", pIdContainer))
        criteria.Add(Restrictions.Eq("Id.KeyType", pKey.ToString()))
        criteria.SetProjection(Projections.Max("Id.Incremental"))
        Dim ret As Short = 0
        Try
            ret = criteria.UniqueResult(Of Short)()
        Catch ex As Exception
        End Try
        Return ret
    End Function

    Public Function DeleteByContainerAndKey(ByVal pIdContainer As Integer, ByVal pKey As ContainerExtensionType, ByRef transaction As ITransaction) As Boolean
        Dim query As String = String.Format("DELETE FROM ContainerExtension WHERE idContainer = {0} AND KeyType = '{1}'", pIdContainer, pKey)

        Dim command As IDbCommand = New SqlClient.SqlCommand()
        command.Connection = NHibernateSession.Connection
        transaction.Enlist(command)

        command.CommandText = query
        Return command.ExecuteNonQuery() > 0
    End Function

End Class
