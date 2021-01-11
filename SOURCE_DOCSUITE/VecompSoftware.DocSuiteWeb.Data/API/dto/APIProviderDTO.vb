Imports VecompSoftware.DocSuiteWeb.API

Public Class APIProviderDTO
    Implements IAPIProviderDTO

#Region " Constructors "

    Public Sub New()
    End Sub
    Public Sub New(provider As APIProvider)
        Address = provider.Address
        Code = provider.Code
        Description = provider.Description
        Enabled = provider.Enabled
        Main = provider.Main
        Title = provider.Title
    End Sub

#End Region

#Region " Properties "

    Public Property Address As String Implements IAPIProviderDTO.Address
    Public Property Code As String Implements IAPIProviderDTO.Code
    Public Property Description As String Implements IAPIProviderDTO.Description
    Public Property Enabled As Boolean? Implements IAPIProviderDTO.Enabled
    Public Property Main As Boolean? Implements IAPIProviderDTO.Main
    Public Property Title As String Implements IAPIProviderDTO.Title

#End Region

End Class
