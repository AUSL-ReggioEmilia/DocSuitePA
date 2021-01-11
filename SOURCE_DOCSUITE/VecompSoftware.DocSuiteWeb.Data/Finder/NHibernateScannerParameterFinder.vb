Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager
Imports System.ComponentModel

<Serializable(), DataObject()> _
Public Class NHibernateScannerParameterFinder
    Inherits NHibernateBaseFinder(Of ScannerParameter, ScannerParameter)

#Region "Fields"
    Private _scannerConfigurationId As Integer?
    Private _name As String
    Private _value As String
    Private _description As String
#End Region

#Region "Properties"
    Public Property ScannerConfigurationId() As Integer?
        Get
            Return _scannerConfigurationId
        End Get
        Set(ByVal value As Integer?)
            _scannerConfigurationId = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

#End Region

#Region "Constuctor"
    Public Sub New()
        SessionFactoryName = System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB)
    End Sub
#End Region

#Region " NHibernate Properties "
    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property
#End Region

#Region "Criteria"
    Protected Overrides Function CreateCriteria() As NHibernate.ICriteria
        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType)

        'IdScannerConfiguration
        If Not String.IsNullOrEmpty(ScannerConfigurationId) Then
            criteria.Add(Restrictions.Eq("ScannerConfiguration.Id", ScannerConfigurationId))
        End If

        'Name
        If Not String.IsNullOrEmpty(Name) Then
            criteria.Add(Expression.Like("Name", Name, MatchMode.Anywhere))
        End If

        'Value
        If Not String.IsNullOrEmpty(Value) Then
            criteria.Add(Expression.Like("Value", Value, MatchMode.Anywhere))
        End If

        'Description
        If Not String.IsNullOrEmpty(Value) Then
            criteria.Add(Expression.Like("Description", Description, MatchMode.Anywhere))
        End If

        'Aggancia filtri
        AttachFilterExpressions(criteria)

        Return criteria
    End Function
#End Region

#Region "IFinder DoSearch"
    Public Overloads Overrides Function DoSearch() As System.Collections.Generic.IList(Of ScannerParameter)
        Dim criteria As ICriteria = Me.CreateCriteria()

        If Not AttachSortExpressions(criteria) Then
            AttachSortExpressions(criteria, "Name", SortOrder.Ascending)
        End If

        criteria.SetFirstResult(_startIndex)
        criteria.SetMaxResults(_pageSize)

        Return criteria.List(Of ScannerParameter)()
    End Function
#End Region

End Class
