Public Class UserDesktopNodeAction

#Region "Constructor"
    Public Sub New()

    End Sub

    Public Sub New(title As String, actionType As String, nodeType As String, pageName As String, documentType As String, imageUrl As String)
        Me.Title = title
        Me.ActionType = actionType
        Me.NodeType = nodeType
        Me.PageName = pageName
        Me.DocumentType = documentType
        Me.ImageUrl = imageUrl
    End Sub

    Public Sub New(title As String, actionType As String, pageName As String, documentType As String)
        Me.Title = title
        Me.ActionType = actionType
        Me.PageName = pageName
        Me.DocumentType = documentType
    End Sub

    Public Sub New(title As String, actionType As String, pageName As String)
        Me.Title = title
        Me.ActionType = actionType
        Me.PageName = pageName
    End Sub
#End Region

#Region "Properties"
    Private _title As String
    Public Property Title As String
        Get
            Return _title
        End Get
        Set(value As String)
            _title = value
        End Set
    End Property

    Private _actionType As String
    Public Property ActionType As String
        Get
            Return _actionType
        End Get
        Set(value As String)
            _actionType = value
        End Set
    End Property

    Private _nodeType As String
    Public Property NodeType As String
        Get
            Return _nodeType
        End Get
        Set(value As String)
            _nodeType = value
        End Set
    End Property

    Private _pageName As String
    Public Property PageName As String
        Get
            Return _pageName
        End Get
        Set(value As String)
            _pageName = value
        End Set
    End Property

    Private _documentType As String
    Public Property DocumentType As String
        Get
            Return _documentType
        End Get
        Set(value As String)
            _documentType = value
        End Set
    End Property

    Private _imageUrl As String
    Public Property ImageUrl As String
        Get
            Return _imageUrl
        End Get
        Set(value As String)
            _imageUrl = value
        End Set
    End Property
#End Region

End Class
