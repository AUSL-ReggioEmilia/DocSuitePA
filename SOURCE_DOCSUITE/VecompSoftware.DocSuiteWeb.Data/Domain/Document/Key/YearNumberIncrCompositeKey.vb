Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class YearNumberIncrCompositeKey
    Inherits Triplet(Of Nullable(Of Short), Integer?, Nullable(Of Short))

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
            Return Second()
        End Get
        Set(ByVal value As Integer?)
            Second = value
        End Set
    End Property

    Public Overridable Property Incremental() As Nullable(Of Short)
        Get
            Return Third
        End Get
        Set(ByVal value As Nullable(Of Short))
            Third = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal year As Nullable(Of Short), ByVal number As Integer?, ByVal incremental As Nullable(Of Short))
        MyBase.New(year, number, incremental)
    End Sub

#End Region

#Region " Methods "

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As YearNumberIncrCompositeKey = DirectCast(obj, YearNumberIncrCompositeKey)
        Return Me.Year.Equals(compareTo.Year) And Me.Number.Equals(compareTo.Number) And Me.Incremental.Equals(compareTo.Incremental)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return Year.ToString() + "/" + Number.ToString() + "/" + Incremental.ToString()
    End Function

#End Region

End Class