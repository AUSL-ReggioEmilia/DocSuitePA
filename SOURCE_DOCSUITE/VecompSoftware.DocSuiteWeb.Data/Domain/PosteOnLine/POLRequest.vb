Imports System.Linq

''' <summary> Richiesta generica PosteOnLine. </summary>
Public Class POLRequest

#Region " Fields "

    Private _contacts As IList(Of POLRequestContact)
    Private _id As Guid

    Private _account As POLAccount


    Private _idRichiesta As String
    Private _guidPoste As String

    Private _idOrdine As String

    Private _status As POLRequestStatusEnum
    Private _statusDescrition As String
    Private _errorMsg As String

    Private _costoTotale As Double

    Private _registrationDate As DateTime?
    Private _registrationUser As String
    Private _lastChangedDate As DateTime?
    Private _lastChangedUser As String


    Private _recipients As IList(Of POLRequestRecipient)

    Private _sender As POLRequestSender

    Private _type As POLRequestType

#End Region

#Region " Properties "

    Public Overridable Property Id() As Guid
        Get
            Return _id
        End Get
        Set(ByVal value As Guid)
            _id = value
        End Set
    End Property

    Public Overridable Property Account As POLAccount
        Get
            Return _account
        End Get
        Set(value As POLAccount)
            _account = value
        End Set
    End Property

    ''' <summary> Identificativo univoco del servizio richiesto.  </summary>
    ''' <remarks> Una volta inviata la richiesta sarà assocuato al <see cref="GuidPoste"/>. </remarks>
    Public Overridable Property IdRichiesta() As String
        Get
            Return _idRichiesta
        End Get
        Set(ByVal value As String)
            _idRichiesta = value
        End Set
    End Property

    ''' <summary> Identificativo univoco corrispondente all’IdTransazione di Poste Italiane. </summary>
    ''' <remarks> 
    ''' Può essere utilizzato dall’utente per richiedere informazioni sulla lettera Raccomandata all’Help Desk. 
    ''' Associato ad IdRichiesta. Chiamato anche GuidUtente.
    ''' </remarks>
    Public Overridable Property GuidPoste() As String
        Get
            Return _guidPoste
        End Get
        Set(ByVal value As String)
            _guidPoste = value
        End Set
    End Property

    Public Overridable Property IdOrdine() As String
        Get
            Return _idOrdine
        End Get
        Set(ByVal value As String)
            _idOrdine = value
        End Set
    End Property

    Public Overridable Property Status() As POLRequestStatusEnum
        Get
            Return _status
        End Get
        Set(ByVal value As POLRequestStatusEnum)
            _status = value
        End Set
    End Property

    Public Overridable Property StatusDescrition() As String
        Get
            Return _statusDescrition
        End Get
        Set(ByVal value As String)
            _statusDescrition = value
        End Set
    End Property

    Public Overridable Property ErrorMsg() As String
        Get
            Return _errorMsg
        End Get
        Set(ByVal value As String)
            _errorMsg = value
        End Set
    End Property

    Public Overridable Property CostoTotale() As Double
        Get
            Return _costoTotale
        End Get
        Set(ByVal value As Double)
            _costoTotale = value
        End Set
    End Property

    Public Overridable Property ProtocolYear As Short?

    Public Overridable Property ProtocolNumber As Integer?

    Public Overridable Property RegistrationDate() As DateTime?
        Get
            Return _registrationDate
        End Get
        Set(ByVal value As DateTime?)
            _registrationDate = value
        End Set
    End Property

    Public Overridable Property RegistrationUser() As String
        Get
            Return _registrationUser
        End Get
        Set(ByVal value As String)
            _registrationUser = value
        End Set
    End Property

    Public Overridable Property LastChangedDate As DateTime?
        Get
            Return _lastChangedDate
        End Get
        Set(ByVal value As DateTime?)
            _lastChangedDate = value
        End Set
    End Property

    Public Overridable Property LastChangedUser As String
        Get
            Return _lastChangedUser
        End Get
        Set(ByVal value As String)
            _lastChangedUser = value
        End Set
    End Property

    Protected Friend Overridable Property Contacts() As IList(Of POLRequestContact)
        Get
            Return _contacts
        End Get
        Set(ByVal value As IList(Of POLRequestContact))
            _contacts = value
        End Set
    End Property

    Public Overridable Property Recipients() As IList(Of POLRequestRecipient)
        Get
            If _contacts IsNot Nothing AndAlso _contacts.Count > 0 Then
                _recipients.Clear()
                For Each ct As POLRequestContact In _contacts
                    Dim polRequestRecipient As POLRequestRecipient = TryCast(ct, POLRequestRecipient)
                    If (polRequestRecipient IsNot Nothing) Then
                        _recipients.Add(polRequestRecipient)
                    End If
                Next
            End If
            Return _recipients
        End Get
        Protected Set(ByVal value As IList(Of POLRequestRecipient))
            _recipients = value
        End Set
    End Property

    Public Overridable Property Sender() As POLRequestSender
        Get

            If _contacts IsNot Nothing AndAlso _contacts.Count > 0 Then
                _sender = Nothing
                For Each ct As POLRequestContact In _contacts
                    Dim polRequestContact As POLRequestSender = TryCast(ct, POLRequestSender)
                    If (polRequestContact IsNot Nothing) Then
                        _sender = polRequestContact
                    End If
                Next
            End If
            Return _sender
        End Get
        Set(ByVal value As POLRequestSender)
            _sender = value
        End Set
    End Property

    Public Overridable Property Type() As POLRequestType
        Get
            Return _type
        End Get
        Protected Set(ByVal value As POLRequestType)
            _type = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        Recipients = New List(Of POLRequestRecipient)()
        _contacts = New List(Of POLRequestContact)()
        Sender = New POLRequestSender()
    End Sub

#End Region

#Region " Methods "

    Public Overridable Sub SetError(ByVal msg As String)
        ErrorMsg = msg
        Status = POLRequestStatusEnum.Error
        StatusDescrition = "Invio non possibile!"
    End Sub

    ''' <summary> Indica se la richiesta è stata ricevuta o rifiutata da tutti. </summary>
    Public Overridable Function IsComplete() As Boolean
        Return Recipients.All(Function(ct) ct.Status = POLMessageContactEnum.Received OrElse ct.Status = POLMessageContactEnum.Rejected)
    End Function

#End Region

End Class
