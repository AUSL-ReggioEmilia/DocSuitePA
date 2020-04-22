Public Interface IGridBarController
    Sub Show()
    Sub Hide()
    Sub BindGrid(ByRef grid As BaseGrid)

    Property EnableLeft() As Boolean
    Property EnableRight() As Boolean
    Property EnableMiddle() As Boolean

    ReadOnly Property HasWorkflow() As Boolean

    Sub LoadConfiguration(ByVal config As String)
End Interface
