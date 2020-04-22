Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class ProtocolRoleUserKey
    Inherits Quintet(Of Short, Integer, Short, String, String)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal pYear As Integer, ByVal pNumber As Integer, ByVal pIdRole As Integer, ByVal pGroupName As String, ByVal pUserName As String)
        MyBase.New(pYear, pNumber, pIdRole, pGroupName, pUserName)
    End Sub

    Public Overridable Property Year() As Short
        Get
            Return First
        End Get
        Set(ByVal value As Short)
            First = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return Second
        End Get
        Set(ByVal value As Integer)
            Second = value
        End Set
    End Property

    Public Overridable Property IdRole() As Integer
        Get
            Return Third
        End Get
        Set(ByVal value As Integer)
            Third = value
        End Set
    End Property

    Public Overridable Property GroupName() As String
        Get
            Return Forth
        End Get
        Set(ByVal value As String)
            Forth = value
        End Set
    End Property

    Public Overridable Property UserName() As String
        Get
            Return Fifth
        End Get
        Set(ByVal value As String)
            Fifth = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Dim format As String = "{0}/{1:0000000}/{2}/{3}/{4}"
        Return String.Format(format, Year, Number, IdRole, GroupName, UserName)
    End Function

End Class