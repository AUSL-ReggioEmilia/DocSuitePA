Imports VecompSoftware.Helpers.NHibernate

Public Class TemplateProtocolRoleUserCompositeKey
    Inherits Quadruplet(Of Integer, Integer, String, String)

    Public Overridable Property IdTemplateProtocol As Integer
        Get
            Return First
        End Get
        Set(value As Integer)
            First = value
        End Set
    End Property

    Public Overridable Property IdRole As Integer
        Get
            Return Second
        End Get
        Set(value As Integer)
            Second = value
        End Set
    End Property

    Public Overridable Property GroupName As String
        Get
            Return Third
        End Get
        Set(value As String)
            Third = value
        End Set
    End Property

    Public Overridable Property UserName As String
        Get
            Return Forth
        End Get
        Set(value As String)
            Forth = value
        End Set
    End Property
End Class
