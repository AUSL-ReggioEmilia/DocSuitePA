Imports NHibernate
Imports VecompSoftware.NHibernateManager

Public Class NHCategorySchemaFinder
    Inherits NHibernateBaseFinder(Of CategorySchema, CategorySchema)

#Region " Properties "
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property
#End Region

#Region " Constructors "

    Public Sub New(ByVal dbName As String)
        SessionFactoryName = dbName
    End Sub

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub

#End Region

#Region " Methods "

    Protected Overrides Function CreateCriteria() As ICriteria
        Return NHibernateSession.CreateCriteria(Of CategorySchema)
    End Function

    Public Overrides Function DoSearchHeader() As IList(Of CategorySchema)
        Dim criteria As ICriteria = CreateCriteria()
        AttachSortExpressions(criteria)
        Return criteria.List(Of CategorySchema)()
    End Function

    Protected Overrides Function AttachSortExpressions(ByRef criteria As ICriteria) As Boolean
        SortExpressions.Add("Version", "ASC")
        MyBase.AttachSortExpressions(criteria)
    End Function

    Public Overrides Function DoSearch() As IList(Of CategorySchema)
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of CategorySchema)()
    End Function
#End Region
End Class
