Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class FascDocumentsInsert
    Inherits FascBasePage

#Region " Fields "
    Private _fascMiscellaneaLocation As Location = Nothing
#End Region

#Region " Properties "
    Public ReadOnly Property FascMiscellaneaLocation() As Location
        Get
            If _fascMiscellaneaLocation Is Nothing Then
                _fascMiscellaneaLocation = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
            End If
            Return _fascMiscellaneaLocation
        End Get
    End Property

    Public ReadOnly Property ArchiveName() As String
        Get
            Return FascMiscellaneaLocation.ProtBiblosDSDB
        End Get
    End Property

    Protected ReadOnly Property OnlySignEnabled As Boolean
        Get
            Return GetKeyValueOrDefault(Of Boolean)("OnlySignEnabled", False)
        End Get
    End Property

    Protected ReadOnly Property FilterByArchiveDocumentId As Guid?
        Get
            Return GetKeyValueOrDefault(Of Guid?)("FilterByArchiveDocumentId", Nothing)
        End Get
    End Property

#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then

        End If

        If OnlySignEnabled Then
            radPane.Visible = False
            UDInsertView.Visible = False
            MiscellaneaInsertView.Visible = True
            rmpPages.SelectedIndex = 1
            uscFascInsertMiscellanea.OnlySignEnabled = OnlySignEnabled
            uscFascInsertMiscellanea.FilterByArchiveDocumentId = FilterByArchiveDocumentId
        End If

    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()

    End Sub

#End Region

End Class

