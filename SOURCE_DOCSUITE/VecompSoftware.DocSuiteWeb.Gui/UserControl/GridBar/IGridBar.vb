Imports Telerik.Web.UI

Public Interface IGridBar

    Property Grid() As RadGrid
    Property HasWorkflow() As Boolean
    ReadOnly Property SelectButton() As Button
    ReadOnly Property DeselectButton() As Button
    ReadOnly Property PrintButton() As Button
    ReadOnly Property DocumentsButton() As Button
    ReadOnly Property SetReadButton() As Button

    ReadOnly Property LeftPanel() As Panel
    ReadOnly Property RightPanel() As Panel
    ReadOnly Property MiddlePanel() As Panel

    Function GetSelectedItems() As IList

    Sub SelectOrDeselectAll(ByVal Selected As Boolean)
    Sub Show()
    Sub Hide()
    Sub Print()

End Interface
