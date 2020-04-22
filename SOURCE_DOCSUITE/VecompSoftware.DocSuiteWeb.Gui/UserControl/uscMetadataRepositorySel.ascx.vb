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

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If ComboBoxAutoWidthEnabled Then
                rcbMetadataRepository.Width = Unit.Percentage(100)
            End If
        End If
    End Sub

End Class