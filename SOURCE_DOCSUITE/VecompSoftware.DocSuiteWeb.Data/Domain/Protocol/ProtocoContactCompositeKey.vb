Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class ProtocolContactCompositeKey
    Inherits Quadruplet(Of Short, Integer, Integer, String)

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
            Return Second()
        End Get
        Set(ByVal value As Integer)
            Second = value
        End Set
    End Property
    Public Overridable Property IdContact() As Integer
        Get
            Return Third
        End Get
        Set(ByVal value As Integer)
            Third = value
        End Set
    End Property
    Public Overridable Property ComunicationType() As String
        Get
            Return Forth
        End Get
        Set(ByVal value As String)
            Forth = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0}/{1}-{2}-{3}", Year.ToString(), Number.ToString(), IdContact.ToString(), ComunicationType)
    End Function

End Class