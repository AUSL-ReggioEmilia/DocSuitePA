''' <summary> Classe d'appoggio per il rendering dei documenti legati alle pubblicazioni degli atti </summary>
Public Class WebDoc
#Region " Fields "
    Private ReadOnly _documentGuid As Guid
    Private _name As String
    Private ReadOnly _selected As Boolean
#End Region

#Region " Properties "
    Public ReadOnly Property DocumentGuid As Guid
        Get
            Return _documentGuid
        End Get
    End Property

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    Public ReadOnly Property Id As String
        Get
            Return String.Format("{0}|{1}", DocumentGuid, If(Selected, 1, 0))
        End Get
    End Property

    Public ReadOnly Property Selected As Boolean
        Get
            Return _selected
        End Get
    End Property
#End Region

#Region " Constuctor "
    Public Sub New(ByVal guid As Guid, ByVal name As String, ByVal selected As Boolean)
        _documentGuid = guid
        _name = name
        _selected = selected
    End Sub
#End Region
End Class
