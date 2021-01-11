<Serializable()> _
Public Class FileResolution
    Inherits DomainObject(Of Int32)

#Region " Fields "

    Private _idResolutionFile As Integer?
    Private _idProposalFile As Integer?
    Private _idAttachements As Integer?
    Private _idControllerFile As Integer?
    Private _idPrivacyAttachments As Integer?
    Private _idAssumedProposal As Integer?
    Private _idFrontespizio As Integer?
    Private _idPrivacyPublicationDocument As Integer?
    Private _idFrontalinoRitiro As Integer?
    Private _resolution As Resolution

#Region "ASL-TO2"
    Private _idUltimaPagina As Integer?
    Private _idSupervisoryBoardFile As Integer?

#End Region

#End Region

#Region " Properties "

    Public Overridable Property IdResolutionFile() As Integer?
        Get
            Return _idResolutionFile
        End Get
        Set(ByVal value As Integer?)
            _idResolutionFile = value
        End Set
    End Property

    Public Overridable Property IdProposalFile() As Integer?
        Get
            Return _idProposalFile
        End Get
        Set(ByVal value As Integer?)
            _idProposalFile = value
        End Set
    End Property

    Public Overridable Property IdAttachements() As Integer?
        Get
            Return _idAttachements
        End Get
        Set(ByVal value As Integer?)
            _idAttachements = value
        End Set
    End Property

    Public Overridable Property IdPrivacyAttachments() As Integer?
        Get
            Return _idPrivacyAttachments
        End Get
        Set(ByVal value As Integer?)
            _idPrivacyAttachments = value
        End Set
    End Property

    Public Overridable Property IdControllerFile() As Integer?
        Get
            Return _idControllerFile
        End Get
        Set(ByVal value As Integer?)
            _idControllerFile = value
        End Set
    End Property

    Public Overridable Property IdAssumedProposal() As Integer?
        Get
            Return _idAssumedProposal
        End Get
        Set(ByVal value As Integer?)
            _idAssumedProposal = value
        End Set
    End Property

    Public Overridable Property IdFrontespizio() As Integer?
        Get
            Return _idFrontespizio
        End Get
        Set(ByVal value As Integer?)
            _idFrontespizio = value
        End Set
    End Property

    Public Overridable Property IdPrivacyPublicationDocument() As Integer?
        Get
            Return _idPrivacyPublicationDocument
        End Get
        Set(ByVal value As Integer?)
            _idPrivacyPublicationDocument = value
        End Set
    End Property

    ''' <summary>
    ''' Identificativo catena documento ritirato da firmare
    ''' </summary>
    Public Overridable Property IdFrontalinoRitiro() As Integer?
        Get
            Return _idFrontalinoRitiro
        End Get
        Set(ByVal value As Integer?)
            _idFrontalinoRitiro = value
        End Set
    End Property

    Public Overridable Property Resolution() As Resolution
        Get
            Return _resolution
        End Get
        Set(ByVal value As Resolution)
            _resolution = value
        End Set
    End Property

    ' Guid catena allegati annexed
    Public Overridable Property IdAnnexes As Guid

    'Guid catena documento principale omissis
    Public Overridable Property IdMainDocumentsOmissis As Guid

    'Guid catena allegati omissis
    Public Overridable Property IdAttachmentsOmissis As Guid

    Public Overridable Property DematerialisationChainId As Guid?

    Public Overridable Property UniqueIdResolution As Guid
#End Region

#Region "Properties: ASL-TO2"

    Public Overridable Property IdUltimaPagina() As Integer?
        Get
            Return _idUltimaPagina
        End Get
        Set(ByVal value As Integer?)
            _idUltimaPagina = value
        End Set
    End Property

    Public Overridable Property IdSupervisoryBoardFile() As Integer?
        Get
            Return _idSupervisoryBoardFile
        End Get
        Set(ByVal value As Integer?)
            _idSupervisoryBoardFile = value
        End Set
    End Property
#End Region

#Region "Ctor/init"
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

#Region " Methods "
    ''' <summary>
    ''' Verifica che la FileResolution corrente contenga almeno 1 valore visualizzabile
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function HasDocuments() As Boolean
        Return IdResolutionFile IsNot Nothing OrElse _
            IdAttachements IsNot Nothing OrElse _
            IdPrivacyAttachments IsNot Nothing OrElse _
            IdControllerFile IsNot Nothing OrElse _
            IdPrivacyPublicationDocument IsNot Nothing OrElse _
            IdAnnexes <> Guid.Empty OrElse _
            IdMainDocumentsOmissis <> Guid.Empty OrElse _
            IdAttachmentsOmissis <> Guid.Empty
        'IdAssumedProposal IsNot Nothing OrElse _
    End Function
#End Region

End Class

