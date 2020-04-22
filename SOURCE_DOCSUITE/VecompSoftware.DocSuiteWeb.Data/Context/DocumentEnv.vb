
Public Class DocumentEnv
    Inherits BaseEnvironment

#Region " Fields "

    Private Const DefaultConnectionStringName As String = "DocmConnection"

#End Region

#Region " Constructors "

    Public Sub New(ByRef pContext As DocSuiteContext)
        MyBase.New(DefaultConnectionStringName, pContext)
    End Sub

    Public Sub New(ByRef pContext As DocSuiteContext, ByRef pParameters As IEnumerable(Of ParameterEnv))
        MyBase.New(DefaultConnectionStringName, pContext, pParameters)
    End Sub

#End Region

    ''' <summary> Numero di giorni da considerare a partire dall'attuale nelle viste per la scrivania. </summary>
    Public ReadOnly Property DesktopDayDiff() As Integer
        Get
            Return GetInteger("DesktopDayDiff", 30)
        End Get
    End Property

    ''' <summary> Numero massimo di record da ritirare. </summary>
    Public ReadOnly Property DesktopMaxRecords() As Integer
        Get
            Return GetInteger("DesktopMaxRecords", 300)
        End Get
    End Property

    Public ReadOnly Property SearchMaxRecords() As Integer
        Get
            Return GetInteger("SearchMaxRecords", 100)
        End Get
    End Property

    Public ReadOnly Property IsMailEnabled() As Boolean
        Get
            Return GetBoolean("MailEnabled")
        End Get
    End Property

    Public ReadOnly Property MailSmtpServer() As String
        Get
            Return GetString("MailSmtpServer")
        End Get
    End Property

    Public ReadOnly Property IsSecurityEnabled() As Boolean
        Get
            Return GetBoolean("Security")
        End Get
    End Property

	Public ReadOnly Property IsInteropEnabled() As Boolean
		Get
			Return GetBoolean("InteropEnabled")
		End Get
	End Property

    Public ReadOnly Property IsEnvLogEnabled() As Boolean
        Get
            Return GetBoolean("LogEnabled")
        End Get
    End Property

    Public ReadOnly Property WorkFlowCapture() As String
        Get
            Return GetString("WorkFlowCapture", "W")
        End Get
    End Property

    Public ReadOnly Property IsPubblicationEnabled() As Boolean
        Get
            Return GetBoolean("PublicationEnabled")
        End Get
    End Property

    Public ReadOnly Property IsVersioningEnabled() As Boolean
        Get
            Return GetBoolean("VersioningEnabled")
        End Get
    End Property

    Public ReadOnly Property Signature() As String
        Get
            Return GetString("SignatureString")
        End Get
    End Property

    Public ReadOnly Property CorporateAcronym() As String
        Get
            Return GetString("CorporateAcronym")
        End Get
    End Property

    Public ReadOnly Property SignatureType() As Integer
        Get
            Return GetInteger("SignatureType")
        End Get
    End Property

    Public ReadOnly Property ObjectMinLength() As Integer
        Get
            Return GetInteger("ObjectMinLength", 1)
        End Get
    End Property

    Public ReadOnly Property ProtSecurity() As String
        Get
            Return GetString("ProtSecurity")
        End Get
    End Property

    ''' <summary> Stringa di sicurezza per agganciare atti </summary>
    ''' <remarks> Posizionale della sequenza dei diritti </remarks>
    Public ReadOnly Property ReslSecurity() As Integer
        Get
            Return GetInteger("ReslSecurity", 3)
        End Get
    End Property

    ''' <summary> Stringa che indica il dominio AD. </summary>
    Public ReadOnly Property Domain() As String
        Get
            Return GetString("Domain")
        End Get
    End Property

    Public ReadOnly Property EnvLinkVerify() As String
        Get
            Return GetString("LinkVerify")
        End Get
    End Property

    Public ReadOnly Property ContactRoot() As String
        Get
            Return GetString("ContactRoot")
        End Get
    End Property

    Public ReadOnly Property IsConservationEnabled() As Boolean
        Get
            Return GetBoolean("ConservationEnabled")
        End Get
    End Property

    ''' <summary>Abilita l'inserimento multiplo di documenti nelle pratiche</summary>
    Public ReadOnly Property MultipleUploadEnabled() As Boolean
        Get
            Return GetBoolean("MultipleUploadEnabled")
        End Get
    End Property

    ' TODO: spostare da qui
    Public Enum ChkAbilitazioni
        Contact = 1
        Category = 2
    End Enum

    Public ReadOnly Property PraticheHiddenColumns() As String
        Get
            Return GetString("PraticheHiddenColumns")
        End Get
    End Property

    Public ReadOnly Property DefaultWindowWidth() As Integer
        Get
            Return GetInteger("DefaultWindowWidth", 700)
        End Get
    End Property

    Public ReadOnly Property DefaultWindowHeight() As Integer
        Get
            Return GetInteger("DefaultWindowHeight", 500)
        End Get
    End Property
    Public ReadOnly Property EnableButtonNuovaPratica As Boolean
        Get
            Return GetBoolean("EnableButtonNuovaPratica", False)
        End Get
    End Property
End Class
