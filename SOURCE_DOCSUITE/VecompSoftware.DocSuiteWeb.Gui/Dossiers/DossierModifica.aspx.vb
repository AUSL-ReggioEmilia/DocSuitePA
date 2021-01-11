Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons

Public Class DossierModifica
    Inherits DossierBasePage

#Region " Fields "
    Private _currentDossierDocumentLocation As Location
    Private _argument As String = String.Empty
#End Region

#Region " Properties "
    Protected ReadOnly Property CurrentDossierDocumentLocation As Location
        Get
            If _currentDossierDocumentLocation Is Nothing Then
                _currentDossierDocumentLocation = Facade.LocationFacade.GetById(ProtocolEnv.DossierMiscellaneaLocation)
            End If
            Return _currentDossierDocumentLocation
        End Get
    End Property

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        uscSetiContact.MetadataEditId = UscDynamicMetadataRest.PageContent.ClientID

        If Not IsPostBack() Then

        End If
    End Sub
#End Region

#Region " Methods "
#End Region

End Class