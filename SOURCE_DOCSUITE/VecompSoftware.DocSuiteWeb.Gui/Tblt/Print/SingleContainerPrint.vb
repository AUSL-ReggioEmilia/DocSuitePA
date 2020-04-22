Imports VecompSoftware.DocSuiteWeb.Data

Public Class SingleContainerPrint
    Inherits ContainerSecurityPrint

#Region "Fields"
    Private _container As Container = Nothing
#End Region

#Region "Properties"
    Public Property Container() As Container
        Get
            Return _container
        End Get
        Set(ByVal value As Container)
            _container = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByVal idContainer As Integer)
        MyBase.New()
        _container = Facade.ContainerFacade.GetById(idContainer)
    End Sub
#End Region

#Region "DoPrint"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa Contenitore con Sicurezza"

        Dim reslCaption As String = String.Empty

        If DocSuiteContext.Current.IsResolutionEnabled Then
            'TODO: atti non implementati
        End If
        MyBase.PrintContainer(Container, reslCaption)
    End Sub
#End Region

End Class
