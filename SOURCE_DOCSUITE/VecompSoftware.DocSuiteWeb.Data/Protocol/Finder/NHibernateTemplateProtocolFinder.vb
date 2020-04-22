Imports System.ComponentModel
Imports System.Runtime.Remoting.Channels
Imports NHibernate.Criterion
Imports NHibernate
Imports VecompSoftware.NHibernateManager

<Serializable(), DataObject()>
Public Class NHibernateTemplateProtocolFinder
    Inherits NHibernateBaseFinder(Of TemplateProtocol, TemplateProtocol)

#Region "Fields"
    Private _enablePaging As Boolean
    Private _status As Short?
    Private _default As Boolean?
    Private _object As String
#End Region

#Region " Constructors "

    Public Sub New()
        Me.New(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
    End Sub
    Public Sub New(ByVal DbName As String)
        SessionFactoryName = DbName
    End Sub

#End Region

#Region "Properties"
    Protected ReadOnly Property NHibernateSession As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

    Public Property EnablePaging As Boolean
        Get
            Return _enablePaging
        End Get
        Set(value As Boolean)
            _enablePaging = value
        End Set
    End Property

    Public Property Status As Short?
        Get
            Return _status
        End Get
        Set(value As Short?)
            _status = value
        End Set
    End Property

    Public Property IsDefault As Boolean?
        Get
            Return _default
        End Get
        Set(value As Boolean?)
            _default = value
        End Set
    End Property

    Public Property ProtocolObject As String
        Get
            Return _object
        End Get
        Set(value As String)
            _object = value
        End Set
    End Property

#End Region

    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(Of TemplateProtocol)("TPR")
        Return criteria
    End Function

    Public Overrides Function Count() As Integer
        Dim criteria As ICriteria = CreateCriteria()
        DecorateCriteria(criteria)
        criteria.SetProjection(Projections.CountDistinct("TPR.Id"))
        Return criteria.UniqueResult(Of Integer)()
    End Function

    Public Overloads Overrides Function DoSearch() As IList(Of TemplateProtocol)
        Dim criteria As ICriteria = CreateCriteria()
        SetPaging(criteria)
        DecorateCriteria(criteria)
        AttachSortExpressions(criteria)
        Return criteria.List(Of TemplateProtocol)()
    End Function

    Public Overridable Sub DecorateCriteria(ByRef criteria As ICriteria)
        If Status.HasValue Then
            criteria.Add(Restrictions.Eq("TPR.IdTemplateStatus", Status.Value))
        End If

        If IsDefault.HasValue Then
            criteria.Add(Restrictions.Eq("TPR.IsDefault", IsDefault.Value))
        End If

        If Not String.IsNullOrEmpty(ProtocolObject) Then
            criteria.Add(Restrictions.Like("TPR.ProtocolObject", ProtocolObject))
        End If
    End Sub

    Protected Sub SetPaging(ByRef criteria As ICriteria)
        If Not EnablePaging Then
            Return
        End If

        criteria.SetFirstResult(PageIndex)
        criteria.SetMaxResults(PageSize)
    End Sub

    Protected Overrides Function AttachSortExpressions(ByRef criteria As ICriteria) As Boolean
        If SortExpressions.Count = 0 Then
            SortExpressions.Add("TPR.TemplateName", "ASC")
        End If
        Return MyBase.AttachSortExpressions(criteria)
    End Function
End Class
