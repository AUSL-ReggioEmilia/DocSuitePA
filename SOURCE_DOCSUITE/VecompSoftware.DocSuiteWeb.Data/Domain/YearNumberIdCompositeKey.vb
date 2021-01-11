Imports VecompSoftware.Helpers.NHibernate

<Serializable()> _
Public Class YearNumberIdCompositeKey
    Inherits Triplet(Of Short, Integer, Integer)

#Region " Constructors"
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal first As Short, ByVal second As Integer, ByVal third As Integer)
        MyBase.New(first, second, third)
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
    Public Overridable Property Number() As Integer
        Get
            Return Second()
        End Get
        Set(ByVal value As Integer)
            Second = value
        End Set
    End Property
    Public Overridable Property Id() As Integer
        Get
            Return Third
        End Get
        Set(ByVal value As Integer)
            Third = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return Year.ToString() + "/" + Number.ToString() + "-" + Id.ToString()
    End Function

    Public Shared Function TryParse(ByVal id As String) As YearNumberIdCompositeKey
        Dim tmp() As String = id.Split("-"c)
        If tmp.Length > 0 Then
            Dim incr As Integer
            If Integer.TryParse(tmp(1), incr) Then
                Dim year As Short
                Dim number As Integer
                tmp = tmp(0).Split("/"c)
                If Short.TryParse(tmp(0), year) AndAlso Integer.TryParse(tmp(1), number) Then
                    Return New YearNumberIdCompositeKey(year, number, incr)
                End If
            End If
        End If

        Return Nothing
    End Function

End Class