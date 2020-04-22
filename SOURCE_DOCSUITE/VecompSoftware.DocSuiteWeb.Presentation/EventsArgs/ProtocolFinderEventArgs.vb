Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtocolFinderEventArgs
    Inherits FinderEventArgs(Of Protocol, ProtocolHeader)


    Public Sub New(ByVal finder As NHibernateBaseFinder(Of Protocol, ProtocolHeader))
        MyBase.New(finder)
    End Sub

    Public Sub New(ByVal finder As NHibernateProtocolFinder)
        MyBase.New(finder)
    End Sub

    Public Overrides ReadOnly Property Finder() As NHibernateBaseFinder(Of Protocol, ProtocolHeader)
        Get
            Return CType(_finder, NHibernateProtocolFinder)
        End Get
    End Property
End Class
