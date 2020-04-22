Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionFinder2
    Inherits NHResolutionFinder2

#Region " Costructor "
    Public Sub New()
        MyBase.New("ReslDB")
        _factory = New FacadeFactory("ReslDB")
    End Sub
#End Region

    Private ReadOnly _factory As FacadeFactory
    Private ReadOnly Property Factory As FacadeFactory
        Get
            Return _factory
        End Get
    End Property
    
    Public Function GetAdoptedResolutions() As IList(Of Resolution)
        Dim tab As IList(Of TabWorkflow) = Factory.TabWorkflowFacade.GetItemsByDescription("Adozione")
        InSteps = tab
        Return DoSearch()
    End Function


End Class
