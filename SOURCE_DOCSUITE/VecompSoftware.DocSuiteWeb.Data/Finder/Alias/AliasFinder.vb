<Serializable()> _
Public Class AliasFinder
    Implements IAliasFinder

#Region "Fields"
    Private _path As String
    Private _alias As String
    Private _join As NHibernate.SqlCommand.JoinType
    Private _joinAlias As IList(Of IAliasFinder)
#End Region

#Region "Properties"
    Public Property Path() As String Implements IAliasFinder.Path
        Get
            Return _path
        End Get
        Set(ByVal value As String)
            _path = value
        End Set
    End Property

    Public Property [Alias]() As String Implements IAliasFinder.Alias
        Get
            Return _alias
        End Get
        Set(ByVal value As String)
            _alias = value
        End Set
    End Property

    Public Property JoinType() As NHibernate.SqlCommand.JoinType Implements IAliasFinder.JoinType
        Get
            Return _join
        End Get
        Set(ByVal value As NHibernate.SqlCommand.JoinType)
            _join = value
        End Set
    End Property

    Public Property JoinAliases() As IList(Of IAliasFinder) Implements IAliasFinder.JoinAlias
        Get
            Return _joinAlias
        End Get
        Set(ByVal value As IList(Of IAliasFinder))
            _joinAlias = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        _path = String.Empty
        _alias = String.Empty
        _join = NHibernate.SqlCommand.JoinType.None
        _joinAlias = New List(Of IAliasFinder)
    End Sub

    Public Sub New(ByVal path As String, ByVal [alias] As String, ByVal joinType As NHibernate.SqlCommand.JoinType)
        Me.New()
        _path = path
        _alias = [alias]
        _join = joinType
    End Sub

    ''' <summary>
    ''' Il costruttore di default prende un left outer join
    ''' </summary>
    Public Sub New(ByVal path As String, ByVal [alias] As String)
        Me.New()
        _path = path
        _alias = [alias]
        _join = NHibernate.SqlCommand.JoinType.LeftOuterJoin
    End Sub
#End Region
End Class
