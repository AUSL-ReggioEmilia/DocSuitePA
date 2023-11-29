Imports Telerik.Web.UI

Public Interface IGridBar

    Property Grid() As RadGrid
    Property HasWorkflow() As Boolean
    ReadOnly Property SelectButton() As RadButton
    ReadOnly Property DeselectButton() As RadButton
    ReadOnly Property PrintButton() As RadButton
    ReadOnly Property DocumentsButton() As RadButton
    ReadOnly Property SetReadButton() As RadButton

    ReadOnly Property LeftPanel() As Panel
    ReadOnly Property RightPanel() As Panel
    ReadOnly Property MiddlePanel() As Panel

    Function GetSelectedItems() As IList

    Sub SelectOrDeselectAll(ByVal Selected As Boolean)
    Sub Show()
    Sub Hide()
    Sub Print()

End Interface
