Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel
Imports VecompSoftware.Helpers.ExtensionMethods
Imports NHibernate.Transform
Imports System.Linq
Imports NHibernate.SqlCommand

<Serializable(), DataObject()> _
Public Class NHibernateProtocolHeaderFinderTD
    Inherits NHibernateBaseFinder(Of ProtocolHeader, ProtocolHeader)

#Region "Private Fields"
    Private _YearFrom As Short
    Private _Containers As String
    Private _Roles As String
    Private _ProtocolStatus As Integer
#End Region

#Region "Public Properties"
    Public Property YearFrom() As Short
        Get
            Return _YearFrom
        End Get
        Set(ByVal value As Short)
            _YearFrom = value
        End Set
    End Property

    Public Property Containers() As String
        Get
            Return _Containers
        End Get
        Set(ByVal value As String)
            _Containers = value
        End Set
    End Property

    Public Property Roles() As String
        Get
            Return _Roles
        End Get
        Set(ByVal value As String)
            _Roles = value
        End Set
    End Property

    Public Property ProtocolStatus As Integer
        Get
            Return _ProtocolStatus
        End Get
        Set(ByVal value As Integer)
            _ProtocolStatus = value
        End Set
    End Property

    Public Property IdContainerIn As List(Of Integer)
    Public Property IdRoleIn As List(Of Integer)

    Public Property EnablePaging As Boolean

#End Region

#Region "IFinder Implementation"
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of Protocol)("P")

        criteria.CreateAlias("Type", "Type", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateAlias("Location", "Location", SqlCommand.JoinType.InnerJoin)
        criteria.CreateAlias("Container", "Container", SqlCommand.JoinType.LeftOuterJoin)
        criteria.CreateAlias("Category", "Category", SqlCommand.JoinType.LeftOuterJoin)

        criteria.Add(Restrictions.Ge("Id.Year", _YearFrom))
        criteria.Add(Restrictions.Eq("IdStatus", _ProtocolStatus))
        criteria.Add(Restrictions.IsNull("TDIdDocument"))

        Dim disj As New Disjunction()
        disj.Add(Expression.Sql("1=0"))
        If Not IdContainerIn.IsNullOrEmpty() Then
            disj.Add(Restrictions.In("Container.Id", IdContainerIn.ToArray()))
        End If
        If Not IdRoleIn.IsNullOrEmpty() Then
            criteria.CreateAlias("Roles", "PR", JoinType.LeftOuterJoin)
            disj.Add(Restrictions.In("PR.Role.Id", IdRoleIn.ToArray()))
        End If
        criteria.Add(disj)

        'Dim sqlExpr As String = ("({alias}.IdContainer IN (" & Containers & ") OR EXISTS (Select * From ProtocolRole AS PR Where {alias}.Year = PR.Year AND {alias}.Number = PR.Number AND PR.IdRole IN (" & Roles & ")))")
        'criteria.Add(Expression.Sql(sqlExpr))

        'crea Proiezioni
        Dim proj As ProjectionList = Projections.ProjectionList()
        proj.Add(Projections.Property("Year"), "Year")
        proj.Add(Projections.Property("Number"), "Number")
        proj.Add(Projections.Property("RegistrationDate"), "RegistrationDate")
        proj.Add(Projections.Property("RegistrationUser"), "RegistrationUser")
        proj.Add(Projections.Property("ProtocolObject"), "ProtocolObject")
        proj.Add(Projections.Property("IdDocument"), "IdDocument")
        proj.Add(Projections.Property("IdAttachments"), "IdAttachments")
        proj.Add(Projections.Property("DocumentCode"), "DocumentCode")
        proj.Add(Projections.Property("Type"), "Type")
        proj.Add(Projections.Property("IdStatus"), "IdStatus")

        proj.Add(Projections.Property("Container.Id"), "ContainerId")
        proj.Add(Projections.Property("Container.Name"), "ContainerName")

        proj.Add(Projections.Property("Category.Id"), "CategoryId")
        proj.Add(Projections.Property("Category.Code"), "CategoryCode")
        proj.Add(Projections.Property("Category.FullCode"), "CategoryFullCode")
        proj.Add(Projections.Property("Category.Name"), "CategoryName")

        proj.Add(Projections.Property("Location.Id"), "LocationId")
        proj.Add(Projections.Property("Location.ProtBiblosDSDB"), "LocationProtBiblosDSDB")
        proj.Add(Projections.Property("DocumentProtocol"), "DocumentProtocol")

        proj.Add(Projections.Property("TDError"), "TDError")
        proj.Add(Projections.Property("TDIdDocument"), "TDIdDocument")

        AttachFilterExpressions(criteria)
        AttachSQLExpressions(criteria)
        criteria.SetProjection(Projections.Distinct(proj))
        criteria.SetResultTransformer(Transformers.AliasToBean(Of ProtocolHeader))

        Return criteria
    End Function

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        Return criteria.List(Of ProtocolHeader)().Count
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of ProtocolHeader)
        Dim criteria As ICriteria = CreateCriteria()

        If SortExpressions.Count > 0 Then
            Dim orderList As IList = CType(criteria, NHibernate.Impl.CriteriaImpl).IterateOrderings()
            orderList.Clear()
            MyBase.AttachSortExpressions(criteria)
        End If

        If EnablePaging Then
            criteria.SetFirstResult(PageIndex)
            criteria.SetMaxResults(PageSize)
        End If

        Return criteria.List(Of ProtocolHeader)()

    End Function
#End Region

#Region "NHibernate Properties"
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom("ProtDB")
        End Get
    End Property
#End Region
End Class
