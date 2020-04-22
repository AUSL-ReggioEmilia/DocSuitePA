Imports VecompSoftware.DocSuiteWeb.Data

Public MustInherit Class FinderEventArgs(Of T, THeader)
    Inherits EventArgs

    Protected _finder As NHibernateBaseFinder(Of T, THeader)

    Public MustOverride ReadOnly Property Finder() As NHibernateBaseFinder(Of T, THeader)

    Public Sub New(ByVal finder As NHibernateBaseFinder(Of T, THeader))
        _finder = finder
    End Sub

End Class
