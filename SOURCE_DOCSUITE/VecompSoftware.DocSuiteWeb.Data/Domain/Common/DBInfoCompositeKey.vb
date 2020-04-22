Imports VecompSoftware.Helpers.NHibernate

Public Class DBInfoCompositeKey
    Inherits Duplet(Of String, String)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal TableName As String, ByVal ColumnName As String)
        MyBase.New(TableName, ColumnName)
    End Sub

    Public Overridable Property TableName() As String
        Get
            Return First
        End Get
        Set(ByVal value As String)
            First = value
        End Set
    End Property

    Public Overridable Property ColumnName() As String
        Get
            Return Second()
        End Get
        Set(ByVal value As String)
            Second = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0}|{1}", TableName, ColumnName)
    End Function
End Class
