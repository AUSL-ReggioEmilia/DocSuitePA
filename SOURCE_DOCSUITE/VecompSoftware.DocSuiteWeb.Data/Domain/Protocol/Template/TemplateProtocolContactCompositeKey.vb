Imports VecompSoftware.Helpers.NHibernate

Public Class TemplateProtocolContactCompositeKey
    Inherits Triplet(Of Integer, Integer, String)

    Public Overridable Property IdTemplateProtocol As Integer
        Get
            Return First
        End Get
        Set(value As Integer)
            First = value
        End Set
    End Property

    Public Overridable Property IdContact As Integer
        Get
            Return Second
        End Get
        Set(value As Integer)
            Second = value
        End Set
    End Property

    Public Overridable Property ComunicationType As String
        Get
            Return Third
        End Get
        Set(value As String)
            Third = value
        End Set
    End Property
End Class
