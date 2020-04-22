
Public MustInherit Class BaseGridBarController
    Implements IGridBarController

#Region "Fields"
    Protected _gridBar As IGridBar
#End Region

#Region "Constructors"
    Public Sub New(ByRef gridBar As IGridBar)
        _gridBar = gridBar
    End Sub
#End Region

#Region "Sub"
    Public Overridable Sub BindGrid(ByRef grid As BaseGrid) Implements IGridBarController.BindGrid
        _gridBar.Grid = grid
    End Sub

    Public Overridable Sub Hide() Implements IGridBarController.Hide
        _gridBar.Hide()
    End Sub

    Public Overridable Sub Show() Implements IGridBarController.Show
        _gridBar.LeftPanel.Visible = EnableLeft
        _gridBar.MiddlePanel.Visible = EnableMiddle
        _gridBar.RightPanel.Visible = EnableRight
        _gridBar.Show()
    End Sub
#End Region

#Region "Virtual Sub"
    Public MustOverride Sub LoadConfiguration(ByVal config As String) Implements IGridBarController.LoadConfiguration
#End Region

#Region "Properties"
    Private _left As Boolean = False
    Public Property EnableLeft() As Boolean Implements IGridBarController.EnableLeft
        Get
            Return _left
        End Get
        Set(ByVal value As Boolean)
            _left = value
        End Set
    End Property

    Private _middle As Boolean = False
    Public Property EnableMiddle() As Boolean Implements IGridBarController.EnableMiddle
        Get
            Return _middle
        End Get
        Set(ByVal value As Boolean)
            _middle = value
        End Set
    End Property

    Private _right As Boolean = False
    Public Property EnableRight() As Boolean Implements IGridBarController.EnableRight
        Get
            Return _right
        End Get
        Set(ByVal value As Boolean)
            _right = value
        End Set
    End Property

    Public ReadOnly Property HasWorkflow() As Boolean Implements IGridBarController.HasWorkflow
        Get
            Return _gridBar.HasWorkflow
        End Get
    End Property
#End Region

End Class
