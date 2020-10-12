Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.WebAPI

Public Class UDSBasePage
    Inherits CommonBasePage

#Region "Fields"
    ''' <summary><see cref="CommonBasePage.Type"/> da usare come default</summary>
    Public Const DefaultType As String = "UDS"
    Public Const UDS_ADDRESS_NAME As String = "API-UDSAddress"
    Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
    Private _UDSRepositoryFacade As UDSRepositoryFacade
    Private _UDSFacade As UDSFacade
    Private _UDSRepository As UDSRepository
#End Region

#Region "Properties"

    Public ReadOnly Property Callback As String
        Get
            Return GetKeyValue(Of String)("Callback")
        End Get
    End Property

    Public ReadOnly Property CurrentIdUDS As Guid?
        Get
            Return GetKeyValue(Of Guid?)("IdUDS")
        End Get
    End Property

    Public ReadOnly Property CurrentIdUDSRepository As Guid?
        Get
            Return GetKeyValue(Of Guid?)("IdUDSRepository")
        End Get
    End Property

    Public ReadOnly Property CurrentUDSRepository As UDSRepository
        Get
            If _UDSRepository Is Nothing AndAlso CurrentIdUDSRepository.HasValue Then
                _UDSRepository = CurrentUDSRepositoryFacade.GetById(CurrentIdUDSRepository.Value)
            End If
            Return _UDSRepository
        End Get
    End Property

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _UDSFacade Is Nothing Then
                _UDSFacade = New UDSFacade()
            End If
            Return _UDSFacade
        End Get
    End Property

    Protected ReadOnly Property SignalRServerAddress As String
        Get
            Return DocSuiteContext.SignalRServerAddress
        End Get
    End Property

#End Region

#Region "Contructor"
    Public Sub New()

    End Sub
#End Region

#Region "Methods"
    Public Overloads Function GetKeyValue(Of T)(key As String) As T
        Return GetKeyValue(Of T, UDSBasePage)(key)
    End Function

    Protected Function GetSource() As UDSDto
        Return GetSource(CurrentIdUDSRepository.Value, CurrentIdUDS.Value)
    End Function

    Protected Function GetSource(UDSRepositoryId As Guid, UDSId As Guid) As UDSDto
        Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(UDSRepositoryId)
        Return CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, UDSId))
    End Function
#End Region

End Class
