Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class ProtocolLinkCompositeKey
    Inherits Quadruplet(Of Integer, Integer, Integer, Integer)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal pYear As Integer, ByVal pNumber As Integer, ByVal pYearSon As Integer, ByVal pNumberSon As Integer)
        MyBase.New(pYear, pNumber, pYearSon, pNumberSon)
    End Sub

    Public Overridable Property Year() As Integer
        Get
            Return First
        End Get
        Set(ByVal value As Integer)
            First = value
        End Set
    End Property

    Public Overridable Property Number() As Integer
        Get
            Return Second()
        End Get
        Set(ByVal value As Integer)
            Second = value
        End Set
    End Property

    Public Overridable Property YearSon() As Integer
        Get
            Return Third
        End Get
        Set(ByVal value As Integer)
            Third = value
        End Set
    End Property

    Public Overridable Property NumberSon() As Integer
        Get
            Return Forth
        End Get
        Set(ByVal value As Integer)
            Forth = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return Year.ToString() + "/" + Number.ToString().PadLeft(7) + "-" + YearSon.ToString() + "/" + NumberSon.ToString().PadLeft(7)
    End Function

End Class