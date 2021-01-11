Imports VecompSoftware.DocSuiteWeb.Data

Partial Class uscDocumentUnitReferences
    Inherits DocSuite2008BaseControl

    Public Property IdDocumentUnit As String
    Public Property DocumentUnitYear As String
    Public Property DocumentUnitNumber As String
    Public Property ShowArchiveLinks As Boolean ''located in UDS Summary -> call countUDSById
    Public Property ShowArchiveRelationLinks As Boolean ''located in Protocol Summary -> call countUDSByRelationId
    Public Property ShowProtocolRelationLinks As Boolean ''located in Protocol Summary 
    Public Property ShowProtocolLinks As Boolean ''located in UDS Summary
    Public Property ShowFascicleLinks As Boolean
    Public Property ShowProtocolMessageLinks As Boolean
    Public Property ShowProtocolDocumentSeriesLinks As Boolean
    Public Property ShowPECIncoming As Boolean
    Public Property ShowTNotice As Boolean
    Public Property ShowPECOutgoing As Boolean
    Public Property ShowDocumentSeriesMessageLinks As Boolean
    Public Property ShowDocumentSeriesResolutionsLinks As Boolean
    Public Property ShowDocumentSeriesProtocolsLinks As Boolean
    Public Property ShowResolutionlMessageLinks As Boolean
    Public Property ShowResolutionDocumentSeriesLinks As Boolean
    Public Property ShowFasciclesLinks As Boolean
    Public Property ShowDossierLinks As Boolean
    Public Property ShowActiveWorkflowActivities As Boolean
    Public Property ShowDoneWorkflowActivities As Boolean
    Public Property ShowRemoveUDSLinksButton As Boolean

    Private _administrationTrasparenteProtocol As String
    Public ReadOnly Property AdministrationTrasparenteProtocol As String
        Get
            _administrationTrasparenteProtocol = CType(DocSuiteContext.Current.ProtocolEnv.AmministrazioneTrasparenteProtocolEnabled, String)
            Return _administrationTrasparenteProtocol
        End Get
    End Property

    Private _seriesTitle As String
    Public ReadOnly Property SeriesTitle As String
        Get
            _seriesTitle = DocSuiteContext.Current.ProtocolEnv.SeriesTitle
            Return _seriesTitle
        End Get
    End Property

    Public ReadOnly Property ResolutionEnable As Boolean
        Get
            Return DocSuiteContext.Current.IsResolutionEnabled
        End Get
    End Property

    Private _protocolDocumentSeriesButton As String
    Public ReadOnly Property ProtocolDocumentSeriesButtonEnable As String
        Get
            _protocolDocumentSeriesButton = CType(ProtocolEnv.ProtocolDocumentSeriesButtonEnable, String)
            Return _protocolDocumentSeriesButton
        End Get
    End Property

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
    End Sub
#End Region
End Class
