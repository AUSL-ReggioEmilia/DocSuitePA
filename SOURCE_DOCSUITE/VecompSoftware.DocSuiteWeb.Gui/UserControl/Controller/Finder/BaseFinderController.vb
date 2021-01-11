Imports VecompSoftware.DocSuiteWeb.Facade

Public MustInherit Class BaseFinderController(Of FinderType)
    Inherits BaseController
    Implements IFinderController

#Region "Constructor"
    Private _control As Control
    Public Sub New(ByRef control As Control)
        _control = control
    End Sub
#End Region

    Public MustOverride Overrides Function CreateFactory() As FacadeFactory
    Public MustOverride Sub Initialize() Implements IFinderController.Initialize
    Public MustOverride Function LoadFinder() As FinderType
    Public MustOverride Sub BindControls() Implements IFinderController.BindControls

    Public Sub Hide() Implements IFinderController.Hide
        _control.Visible = False
    End Sub
    Public Sub Show() Implements IFinderController.Show
        _control.Visible = True
    End Sub
End Class
