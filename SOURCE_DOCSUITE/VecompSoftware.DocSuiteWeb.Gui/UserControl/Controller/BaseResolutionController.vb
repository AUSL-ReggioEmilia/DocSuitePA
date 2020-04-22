Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class BaseResolutionController
    Inherits BaseController

#Region "BaseController Implementation"
    Public Overrides Function CreateFactory() As FacadeFactory
        Return New FacadeFactory("ReslDB")
    End Function
#End Region

#Region "Context"
    Protected ReadOnly Property ResolutionEnv() As ResolutionEnv
        Get
            Return DocSuiteContext.Current.ResolutionEnv
        End Get
    End Property
#End Region

End Class
