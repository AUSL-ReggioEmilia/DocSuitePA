Public Interface IAliasFinder

    Property Path() As String
    Property [Alias]() As String
    Property JoinType() As NHibernate.SqlCommand.JoinType
    Property JoinAlias() As IList(Of IAliasFinder)

End Interface
