Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.NHibernateManager.Dao


Public Class NHibernateParameterEnvDao
    Inherits BaseNHibernateDao(Of ParameterEnv)

#Region " Fields "

    Private Const SelectQuery As String = "SELECT [KeyValue] FROM [ParameterEnv] WHERE [KeyName]=:key"
    Private Const UpdateQuery As String = "UPDATE [ParameterEnv] SET [KeyValue] = :value, [LastChangedDate] = :lastChangedDate, [LastChangedUser] = :lastChangedUser WHERE [KeyName]=:key"
    Private Const DeleteQuery As String = "DELETE FROM [ParameterEnv] WHERE [KeyName]=:key"
    Private Const InsertQuery As String = "INSERT INTO [ParameterEnv]( [KeyName], [KeyValue], [RegistrationDate], [RegistrationUser]) 
                                            VALUES ( :key, :value, :registrationDate, :registrationUser)"

#End Region

#Region " Constructor "

    Public Sub New(ByVal sessionFactoryName As String)
        MyBase.New(sessionFactoryName)
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    ' TODO: usare veramente il DAO pls
    Public Function SelectByKey(environment As EnvironmentDataCode, ByVal key As String) As IList(Of String)
        Dim nHibernateSession As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), environment))

        Dim sqlQuery As IQuery = nHibernateSession.CreateSQLQuery(SelectQuery).AddScalar("KeyValue", NHibernateUtil.String).SetString("key", key)
        Return sqlQuery.List(Of String)()

    End Function

    ' TODO: usare veramente il DAO pls
    Public Sub SetByKey(environment As EnvironmentDataCode, ByVal key As String, ByVal value As String)
        Dim nHibernateSession As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), environment))

        Dim sqlQuery As IQuery = nHibernateSession.CreateSQLQuery(UpdateQuery).SetString("value", value).SetString("key", key).
            SetDateTimeOffset("lastChangedDate", DateTimeOffset.UtcNow).SetString("lastChangedUser", DocSuiteContext.Current.User.FullUserName)
        sqlQuery.ExecuteUpdate()
    End Sub

    ' TODO: usare veramente il DAO pls
    Public Sub DeleteByKey(ByVal environment As EnvironmentDataCode, ByVal key As String)
        Dim nHibernateSession As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), environment))

        Dim sqlQuery As IQuery = nHibernateSession.CreateSQLQuery(DeleteQuery).SetString("key", key)
        sqlQuery.ExecuteUpdate()
    End Sub

    ' TODO: usare veramente il DAO pls
    Public Sub AddByKey(ByVal environment As EnvironmentDataCode, ByVal key As String, ByVal value As String)
        Dim nHibernateSession As ISession = NHibernateSessionManager.Instance.GetSessionFrom(System.Enum.GetName(GetType(EnvironmentDataCode), environment))

        Dim sqlQuery As IQuery = nHibernateSession.CreateSQLQuery(InsertQuery).SetString("key", key).SetString("value", value).
            SetDateTimeOffset("registrationDate", DateTimeOffset.UtcNow).SetString("registrationUser", DocSuiteContext.Current.User.FullUserName)
        sqlQuery.ExecuteUpdate()
    End Sub

End Class
