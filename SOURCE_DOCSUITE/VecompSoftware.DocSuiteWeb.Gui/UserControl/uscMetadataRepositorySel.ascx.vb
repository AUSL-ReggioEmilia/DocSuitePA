Imports Telerik.Web.UI

Public Class uscMetadataRepositorySel
    Inherits DocSuite2008BaseControl

    Private _currentMetadataRepository As Guid?

    Public ReadOnly Property MaxNumberDropdownElements As Integer
        Get
            Return ProtocolEnv.MaxNumberDropdownElements
        End Get
    End Property

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return metadataPageContent
        End Get
    End Property

    Public ReadOnly Property CurrentMetadataRepository As Guid?
        Get
            If Not String.IsNullOrEmpty(rcbMetadataRepository.SelectedValue) Then
                _currentMetadataRepository = Guid.Parse(rcbMetadataRepository.SelectedValue)
            End If
            Return _currentMetadataRepository
        End Get
    End Property

    Public Property ComboBoxAutoWidthEnabled As Boolean

    Public Property FascicleInsertCommonIdEvent As String
    Public Property SetiContactVisibilityButton As Boolean = True
    Public Property EnableAdvancedMetadataSearch As Boolean = False

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If ComboBoxAutoWidthEnabled Then
                rcbMetadataRepository.Width = Unit.Percentage(100)
            End If
        End If
        uscSetiContactSel.FascicleInsertCommonIdEvent = FascicleInsertCommonIdEvent
        uscSetiContactSel.MetadataAddId = metadataPageContent.ClientID

        enableAdvancedMetadataSearchBtn.Visible = EnableAdvancedMetadataSearch AndAlso ProtocolEnv.MetadataRepositoryEnabled
        uscAdvancedSearchDynamicMetadataRest.Visible = EnableAdvancedMetadataSearch AndAlso ProtocolEnv.MetadataRepositoryEnabled
        txtMetadataValue.Visible = EnableAdvancedMetadataSearch AndAlso ProtocolEnv.MetadataRepositoryEnabled
    End Sub

End Class