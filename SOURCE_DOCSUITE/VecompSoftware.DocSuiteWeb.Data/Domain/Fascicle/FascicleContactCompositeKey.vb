Imports VecompSoftware.Helpers.NHibernate

Public Class FascicleContactCompositeKey
    Inherits Quadruplet(Of Short, Short, Short, Integer)

    Public Overridable Property IdSubCategory() As Short
        Get
            Return First
        End Get
        Set(ByVal value As Short)
            First = value
        End Set
    End Property
    Public Overridable Property Year() As Short
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
    Public Overridable Property IdContact() As Integer
        Get
            Return Forth
        End Get
        Set(ByVal value As Integer)
            Forth = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return Year.ToString() + "/" + IdSubCategory.ToString() + "/" + Incremental.ToString() + "/" + IdContact.ToString()
    End Function


End Class