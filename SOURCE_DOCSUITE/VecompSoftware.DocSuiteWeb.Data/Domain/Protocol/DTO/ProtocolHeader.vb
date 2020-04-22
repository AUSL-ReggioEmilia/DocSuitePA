Public Class ProtocolHeader

#Region " Fields "

    Private _proxiedCategory As Category
    Private _proxiedContainer As Container
    Private _proxiedLocation As Location
    Private _proxiedProtocolParer As ProtocolParer

#End Region

#Region " Properties "

    Public Property Year As Short?
    Public Property Number As Integer?
    Public Property RegistrationUser As String
    Public Property RegistrationDate As DateTimeOffset
    Public Property ProtocolObject As String
    Public Property IdDocument As Integer?
    Public Property IdAttachments As Integer?
    Public Property DocumentCode As String
    Public Property Type As ProtocolType
    Public Property IdStatus As Integer?
    Public Property Links As Integer?
    Public Property ProtocolStatus As ProtocolStatus
    Public Property UniqueId As Guid
    Public Property AlternativeRecipient As String
    Public Property LastChangedDate As DateTimeOffset
    Public Property LastChangedReason As String
    Public Property DocumentDate As Date?
    Public Property DocumentProtocol As String

    Public Property ReadCount As Integer?
    Public Property AttachLocation As Location

    Public Property CountPECInPECStatus As Integer?
    Public Property CountPECInAnomaliaStatus As Integer?
    Public Property CountPECInReceiptStatus As Integer?
    Public Property CountPECInNotificaStatus As Integer?
    Public Property CountPECSegnatura As Integer?
    Public Property CountPECOutgoingStatus As Integer?
    Public Property CountToEvaluateRoles As Integer?
    Public Property AccountingSectional As String
    Public Property InvoiceYear As Integer?
    Public Property InvoiceNumber As String
    Public Property AccountingNumber As Integer?
#End Region

#Region " Proxied Properties "

    Public Property CategoryId As Integer
    Public Property CategoryCode As Integer
    Public Property CategoryName As String
    Public Property CategoryFullCode As String
    Public ReadOnly Property ProxiedCategory As Category
        Get
            If Not CategoryId.Equals(0) AndAlso _proxiedCategory Is Nothing Then
                _proxiedCategory = New Category()
                With _proxiedCategory
                    .Id = CategoryId
                    .Code = CategoryCodeProjection
                    .FullCode = CategoryFullCode
                    .Name = CategoryName
                End With
            End If
            Return _proxiedCategory
        End Get
    End Property

    Public Property ContainerId As Integer
    Public Property ContainerName As String
    Public ReadOnly Property ProxiedContainer As Container
        Get
            If Not ContainerId.Equals(0) AndAlso _proxiedContainer Is Nothing Then
                _proxiedContainer = New Container()
                With _proxiedContainer
                    .Id = ContainerId
                    .Name = ContainerName
                End With
            End If
            Return _proxiedContainer
        End Get
    End Property

    Public Property LocationId As Integer
    Public Property LocationDocumentServer As String
    Public Property LocationProtBiblosDSDB As String
    Public ReadOnly Property ProxiedLocation As Location
        Get
            If Not LocationId.Equals(0) AndAlso _proxiedLocation Is Nothing Then
                _proxiedLocation = New Location()
                _proxiedLocation.Id = LocationId
                _proxiedLocation.DocumentServer = LocationDocumentServer
                _proxiedLocation.ProtBiblosDSDB = LocationProtBiblosDSDB
            End If
            Return _proxiedLocation
        End Get
    End Property

    Public Property ParerUri As String
    Public Property ParerHasError As Boolean?
    Public Property ParerLastError As String
    Public ReadOnly Property ProxiedProtocolParer As ProtocolParer
        Get
            If HasParer AndAlso _proxiedProtocolParer Is Nothing Then
                _proxiedProtocolParer = New ProtocolParer()
                With _proxiedProtocolParer
                    .HasError = ParerHasError.GetValueOrDefault(False)
                    .LastError = ParerLastError
                    .ParerUri = ParerUri
                End With
            End If
            Return _proxiedProtocolParer
        End Get
    End Property

    Public Property Subject As String
    ''' <summary> Identificativo della PEC che ha originato il protocollo </summary>
    Public Property IngoingPecId As Integer?

    Public ReadOnly Property HasPECInPECStatus As Boolean
        Get
            If CountPECInPECStatus.HasValue AndAlso CountPECInPECStatus.Value > 0 Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasPECInAnomaliaStatus As Boolean
        Get
            If CountPECInAnomaliaStatus.HasValue AndAlso CountPECInAnomaliaStatus.Value > 0 Then
                Return True
            End If
            Return False
        End Get
    End Property
    Public ReadOnly Property HasPECOutgoing As Boolean
        Get
            If CountPECOutgoingStatus.HasValue AndAlso CountPECOutgoingStatus.Value > 0 Then
                Return True
            End If
            Return False
        End Get
    End Property


    Public ReadOnly Property HasPECInReceiptStatus As Boolean
        Get
            If CountPECInReceiptStatus.HasValue AndAlso CountPECInReceiptStatus.Value > 0 Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasPECInNotificaStatus As Boolean
        Get
            If CountPECInNotificaStatus.HasValue AndAlso CountPECInNotificaStatus.Value > 0 Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property HasPECSegnatura As Boolean
        Get
            If CountPECSegnatura.HasValue AndAlso CountPECSegnatura.Value > 0 Then
                Return True
            End If
            Return False
        End Get
    End Property

#End Region

#Region " TopMedia Properties "

    Public Property TDIdDocument As Integer?
    Public Property TDError As String

#End Region

#Region " Calculated Properties "

    Public ReadOnly Property ProtocolCompositeKey As YearNumberCompositeKey
        Get
            Return New YearNumberCompositeKey(Year, Number)
        End Get
    End Property
    Public ReadOnly Property FullProtocolNumber As String
        Get
            Return String.Format("{0}/{1:0000000}", Year, Number)
        End Get
    End Property
    Public ReadOnly Property HasRead As Nullable(Of Boolean)
        Get
            Return ReadCount.HasValue AndAlso ReadCount > 0
        End Get
    End Property
    Public ReadOnly Property HasParer As Nullable(Of Boolean)
        Get
            Return ParerHasError.HasValue
        End Get
    End Property

#End Region

#Region " Retro-Compatible Properties "

    Public ReadOnly Property Protocol As String
        Get
            Return FullProtocolNumber
        End Get
    End Property
    Public ReadOnly Property TypeDescription As String
        Get
            Return Type.Description
        End Get
    End Property
    Public ReadOnly Property StatusDescription As String
        Get
            Return Status.Description
        End Get
    End Property

    Public Overridable ReadOnly Property CategoryCodeProjection() As String
        Get
            Dim temp As String = ProxiedCategory.FullCode.Replace("0", String.Empty)
            Return temp.Replace("|", ".")
        End Get
    End Property
    Public Overridable ReadOnly Property CalculatedLink() As String
        Get
            Return String.Format("{0}|{1:0000000}|{2}|{3:dd/MM/yyyy}", Year, Number, ProxiedLocation.Id, RegistrationDate)
        End Get
    End Property
    Public ReadOnly Property LogType As String
        Get
            If HasRead Then
                Return "P1"
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property Status As ProtocolStatus
        Get
            Return ProtocolStatus
        End Get
    End Property

#End Region

End Class
