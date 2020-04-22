Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class PackageCompositeKey
    Inherits Duplet(Of Char, Integer)

#Region " Properties "

    Public Overridable Property Origin() As Char
        Get
            Return First
        End Get
        Set(ByVal value As Char)
            First = value
        End Set
    End Property

    Public Overridable Property Package() As Integer
        Get
            Return Second
        End Get
        Set(ByVal value As Integer)
            Second = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal origin As Char, ByVal package As Integer)
        MyBase.New(origin, package)
    End Sub

#End Region

#Region " Methods "

    Public Overrides Function ToString() As String
        Return String.Format("{0}/{1}", Origin, Package)
    End Function

#End Region

End Class
