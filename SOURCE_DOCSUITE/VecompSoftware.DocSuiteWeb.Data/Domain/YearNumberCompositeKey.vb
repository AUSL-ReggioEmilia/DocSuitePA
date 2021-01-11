Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class YearNumberCompositeKey
    Inherits Duplet(Of Nullable(Of Short), Integer?)

#Region " Properties "

    Public Overridable Property Year() As Nullable(Of Short)
        Get
            Return First
        End Get
        Set(ByVal value As Nullable(Of Short))
            First = value
        End Set
    End Property

    Public Overridable Property Number() As Integer?
        Get
            Return Second
        End Get
        Set(ByVal value As Integer?)
            Second = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal pYear As Nullable(Of Short), ByVal pNumber As Integer?)
        MyBase.New(pYear, pNumber)
    End Sub

#End Region

#Region " Methods "
    
    Public Overrides Function ToString() As String
        Return String.Format("{0}/{1:0000000}", Year, Number)
    End Function

#End Region

End Class