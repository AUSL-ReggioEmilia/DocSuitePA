Imports VecompSoftware.Helpers

<Serializable()> _
Public Class TabWorkflow
    Inherits DomainObject(Of TabWorkflowCompositeKey)

    ''' <summary> Campi gestiti nel <see cref="ManagedWorkflowData"/> </summary>
    ''' <remarks> Inserire solo se implementati in <see cref="ExistWorkflowData()"/> e <see cref="ExtractoWorkflowData()"/>. </remarks>
    Public Enum WorkflowField
        Role
    End Enum

#Region " Fields "

    Private _description As String
    Private _customDescription As String
    Private _fieldDocument As String
    Private _documentImageFile As String
    Private _documentDescription As String
    Private _fieldDate As String
    Private _fieldUser As String
    Private _attachment As String
    Private _fieldPrivacyAttachment As String
    Private _fieldAttachment As String
    Private _biblosFileProperty As String
    Private _operationStep As String
    Private _viewDocumentBitRight As String
    Private _viewAttachmentBitRight As String
    Private _viewOnlyActive As String
    Private _viewPreStep As String
    Private _changeableData As String
    Private _managedWorkflowData As String
    Private _template As String
    Private _fieldFrontalinoRitiro As String
    Private _pubblicaRevocaPage As String
    Private _fieldProtocol As String

#End Region

#Region " Properties "

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property CustomDescription() As String
        Get
            Return _customDescription
        End Get
        Set(ByVal value As String)
            _customDescription = value
        End Set
    End Property

    Public Overridable Property FieldDocument() As String
        Get
            Return _fieldDocument
        End Get
        Set(ByVal value As String)
            _fieldDocument = value
        End Set
    End Property

    Public Overridable Property DocumentImageFile() As String
        Get
            Return _documentImageFile
        End Get
        Set(ByVal value As String)
            _documentImageFile = value
        End Set
    End Property

    Public Overridable Property DocumentDescription() As String
        Get
            Return _documentDescription
        End Get
        Set(ByVal value As String)
            _documentDescription = value
        End Set
    End Property

    Public Overridable Property FieldDate() As String
        Get
            Return _fieldDate
        End Get
        Set(ByVal value As String)
            _fieldDate = value
        End Set
    End Property

    Public Overridable Property FieldUser() As String
        Get
            Return _fieldUser
        End Get
        Set(ByVal value As String)
            _fieldUser = value
        End Set
    End Property

    Public Overridable Property Attachment() As String
        Get
            Return _attachment
        End Get
        Set(ByVal value As String)
            _attachment = value
        End Set
    End Property

    Public Overridable Property FieldAttachment() As String
        Get
            Return _fieldAttachment
        End Get
        Set(ByVal value As String)
            _fieldAttachment = value
        End Set
    End Property

    Public Overridable Property FieldPrivacyAttachment() As String
        Get
            Return _fieldPrivacyAttachment
        End Get
        Set(ByVal value As String)
            _fieldPrivacyAttachment = value
        End Set
    End Property

    Public Overridable Property BiblosFileProperty() As String
        Get
            Return _biblosFileProperty
        End Get
        Set(ByVal value As String)
            _biblosFileProperty = value
        End Set
    End Property

    Public Overridable Property OperationStep() As String
        Get
            Return _operationStep
        End Get
        Set(ByVal value As String)
            _operationStep = value
        End Set
    End Property

    Public Overridable Property ViewDocumentBitRight() As String
        Get
            Return _viewDocumentBitRight
        End Get
        Set(ByVal value As String)
            _viewDocumentBitRight = value
        End Set
    End Property

    Public Overridable Property ViewAttachmentBitRight() As String
        Get
            Return _viewAttachmentBitRight
        End Get
        Set(ByVal value As String)
            _viewAttachmentBitRight = value
        End Set
    End Property

    Public Overridable Property ViewOnlyActive() As String
        Get
            Return _viewOnlyActive
        End Get
        Set(ByVal value As String)
            _viewOnlyActive = value
        End Set
    End Property

    Public Overridable Property ViewPreStep() As String
        Get
            Return _viewPreStep
        End Get
        Set(ByVal value As String)
            _viewPreStep = value
        End Set
    End Property

    Public Overridable Property ChangeableData() As String
        Get
            Return _changeableData
        End Get
        Set(ByVal value As String)
            _changeableData = value
        End Set
    End Property

    Public Overridable Property ManagedWorkflowData() As String
        Get
            Return _managedWorkflowData
        End Get
        Set(ByVal value As String)
            _managedWorkflowData = value
        End Set
    End Property

    Public Overridable Property Template() As String
        Get
            Return _template
        End Get
        Set(ByVal value As String)
            _template = value
        End Set
    End Property

    ''' <summary>
    ''' Nome del campo del <see>FileResolution</see> dove impostare il chainId
    ''' </summary>
    Public Overridable Property FieldFrontalinoRitiro() As String
        Get
            Return _FieldFrontalinoRitiro
        End Get
        Set(ByVal value As String)
            _fieldFrontalinoRitiro = value
        End Set
    End Property

    ''' <summary>
    ''' Nome della pagina da richiamare per effettuare una pubblicazione personalizzata (esempio Ausl-pc)
    ''' </summary>
    Public Overridable Property PubblicaRevocaPage() As String
        Get
            Return _pubblicaRevocaPage
        End Get
        Set(ByVal value As String)
            _pubblicaRevocaPage = value
        End Set
    End Property

    ''' <summary>
    ''' Nome del campo del <see>FileResolution</see> doce impostare il Guid della catena Annessi
    ''' </summary>
    Public Overridable Property FieldAnnexed() As String

    Public Overridable Property FieldProtocol() As String
        Get
            Return _fieldProtocol
        End Get
        Set(ByVal value As String)
            _fieldProtocol = value
        End Set
    End Property

    ''' <summary>
    ''' Nome del campo del <see>FileResolution</see> doce impostare il Guid della catena FieldDocumentsOmissis
    ''' </summary>
    Public Overridable Property FieldDocumentsOmissis() As String

    ''' <summary>
    ''' Nome del campo del <see>FileResolution</see> doce impostare il Guid della catena FieldAttachmentsOmissis
    ''' </summary>
    Public Overridable Property FieldAttachmentsOmissis() As String

    ''' <summary>
    ''' Definisce se il documento principale dello step corrente debba essere visualizzato o meno (in aggiunta agli altri criteri di sicurezza)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property ViewCurrentDocument() As Nullable(Of Boolean)

    ''' <summary>
    ''' Definisce se l'allegato principale dello step corrente debba essere visualizzato o meno (in aggiunta agli altri criteri di sicurezza)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Property ViewCurrentAttachment() As Nullable(Of Boolean)
#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Identifica la presenza del campo richiesto nei dati di gestione del workflow. </summary>
    Public Overridable Function ExistWorkflowData(field As WorkflowField) As Boolean
        Return ExistWorkflowData(field.ToString())
    End Function

    ''' <summary> Identifica la presenza del campo richiesto nei dati di gestione del workflow. </summary>
    Public Overridable Function ExistWorkflowData(ByVal field As String) As Boolean
        Return StringHelper.InStrTest(ManagedWorkflowData, field)
    End Function

    Public Overridable Function ExtractoWorkflowData(Of T)(field As WorkflowField) As T
        Dim toReturn As T

        Select Case field
            Case WorkflowField.Role
                Dim startIndex As Integer = ManagedWorkflowData.IndexOf(field.ToString(), StringComparison.Ordinal) + 6
                Dim endIndex As Integer = ManagedWorkflowData.IndexOf(".", startIndex, StringComparison.Ordinal)
                toReturn = CType(Convert.ChangeType(ManagedWorkflowData.Substring(startIndex, endIndex - startIndex), GetType(T)), T)
            Case Else
                Throw New NotImplementedException("Campo non ancora implementato")
        End Select

        Return toReturn
    End Function

#End Region

End Class

