Imports VecompSoftware.DocSuiteWeb.Model.ExternalModels

Public Class IP4DSentEventArgs
    Inherits EventArgs

    Public IP4DModel As ExternalViewerModel

    Public Sub New(ip4dModel As ExternalViewerModel)
        ip4dModel = ip4dModel
    End Sub
End Class
