Imports AutoMapper
Imports VecompSoftware.DocSuiteWeb.API

Public Class APIProvider
    Inherits DomainObject(Of Guid)
    Implements IAuditable

#Region " Constructors "

    Public Sub New()
    End Sub
    Public Sub New(source As APIProvider)
        Enabled = source.Enabled
        Code = source.Code
        Title = source.Title
        Description = source.Description
        Address = source.Address
        Main = source.Main
    End Sub
    Public Sub New(source As IAPIProviderDTO)
        Mapper.Initialize(Function(cfg) cfg.CreateMap(Of IAPIProviderDTO, APIProvider)())
        Mapper.Map(source, Me)
    End Sub

#End Region

#Region " Properties "

    Public Overridable Property LastChangedDate As DateTimeOffset? Implements IAuditable.LastChangedDate
    Public Overridable Property LastChangedUser As String Implements IAuditable.LastChangedUser
    Public Overridable Property RegistrationDate As DateTimeOffset Implements IAuditable.RegistrationDate
    Public Overridable Property RegistrationUser As String Implements IAuditable.RegistrationUser

    Public Overridable Property Enabled As Boolean?
    Public Overridable Property Code As String
    Public Overridable Property Title As String
    Public Overridable Property Description As String
    Public Overridable Property Address As String
    Public Overridable Property Main As Boolean?
    Public Overridable Property Preserved As Boolean?

    Public Overridable ReadOnly Property IsEnabled As Boolean
        Get
            Return Enabled.GetValueOrDefault(True)
        End Get
    End Property
    Public Overridable ReadOnly Property IsMain As Boolean
        Get
            Return Main.GetValueOrDefault(False)
        End Get
    End Property
    Public Overridable ReadOnly Property IsPreserved As Boolean
        Get
            Return Preserved.GetValueOrDefault(False)
        End Get
    End Property

#End Region

End Class
