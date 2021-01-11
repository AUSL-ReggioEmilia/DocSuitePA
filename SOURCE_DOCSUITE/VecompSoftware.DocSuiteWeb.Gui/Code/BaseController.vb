Imports VecompSoftware.DocSuiteWeb.Facade

Public MustInherit Class BaseController

#Region "Facade"
    Private _facadeFactory As FacadeFactory
    Protected Overridable ReadOnly Property Facade() As FacadeFactory
        Get
            If _facadeFactory Is Nothing Then
                _facadeFactory = CreateFactory()
            End If
            Return _facadeFactory
        End Get
    End Property

    Public MustOverride Function CreateFactory() As FacadeFactory
#End Region

End Class
