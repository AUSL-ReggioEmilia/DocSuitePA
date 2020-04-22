Imports VecompSoftware.Helpers.NHibernate

Public Class FascicleCompositeKey
    Inherits Triplet(Of Short, Short, Short)

#Region " Constructors"
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal pYear As Short, ByVal pIdSubCategory As Short, ByVal pIncremental As Short)
        MyBase.New(pYear, pIdSubCategory, pIncremental)
    End Sub
#End Region

    Public Overridable Property Year() As Short
        Get
            Return First
        End Get
        Set(ByVal value As Short)
            First = value
        End Set
    End Property

    Public Overridable Property IdSubCategory() As Short
        Get
            Return Second()
        End Get
        Set(ByVal value As Short)
            Second = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short
        Get
            Return Third
        End Get
        Set(ByVal value As Short)
            Third = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return Year.ToString() + "/" + IdSubCategory.ToString() + "/" + Incremental.ToString()
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

End Class