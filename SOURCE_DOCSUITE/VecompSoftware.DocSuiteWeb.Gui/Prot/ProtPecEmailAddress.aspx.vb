Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ProtPecEmailAddress
    Inherits ProtBasePage

#Region "Fields"

    Private _contactsUpdated As Boolean
    Private _sessionSeed As String

#End Region

#Region "Properties"

    Private ReadOnly Property SessionSeed As String
        Get
            If String.IsNullOrEmpty(_sessionSeed) Then
                If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("SessionSeed") IsNot Nothing Then
                    _sessionSeed = Request.QueryString("SessionSeed")
                End If
            End If

            Return _sessionSeed
        End Get
    End Property

    Private ReadOnly Property ContactIds As String()
        Get
            If Not String.IsNullOrEmpty(SessionSeed) Then
                Return CType(Session(SessionSeed), KeyValuePair(Of String, String)).Key.Split("$"c)
            End If

            Return Nothing
        End Get
    End Property

    Private ReadOnly Property ContactNames As String()
        Get
            If Not String.IsNullOrEmpty(SessionSeed) Then
                Return CType(Session(SessionSeed), KeyValuePair(Of String, String)).Value.Split("$"c)
            End If

            Return Nothing
        End Get
    End Property

    Public ReadOnly Property CloseMe() As String
        Get
            If _contactsUpdated Then
                Return "true"
            Else
                Return "false"
            End If
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack And ContactIds IsNot Nothing Then
            ' Scrivo il messaggio
            lblMessage.Text = Request("Message")
            dlstAddresses.DataSource = BuidAddressesList()
            dlstAddresses.DataBind()
        End If
    End Sub

    Protected Sub BtnConfermaClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim contactId As String
        Dim address As String
        Dim addressId As Integer

        For Each item As DataListItem In dlstAddresses.Items
            address = CType(item.FindControl("txtCertifiedMail"), TextBox).Text
            contactId = dlstAddresses.DataKeys()(item.ItemIndex).ToString()

            If Integer.TryParse(contactId, addressId) Then
                ' contatto da rubrica
                _contactsUpdated = UpdateContact(contactId, address, ContactTypeEnum.AddressBook)
            Else
                ' contatto manuale
                _contactsUpdated = UpdateContact(contactId, address, ContactTypeEnum.Manual)
            End If
        Next
    End Sub

#End Region

#Region "Methods"

    Private Function BuidAddressesList() As IList(Of PecContactAddress)
        Dim addresses As New List(Of PecContactAddress)
        For index As Short = 0 To ContactIds.Length - 1
            addresses.Add(New PecContactAddress(ContactIds(index), ContactNames(index)))
        Next index

        Return addresses
    End Function

    Private Function UpdateContact(ByVal idContact As String, ByVal emailAddress As String, ByVal contactType As ContactTypeEnum) As Boolean
        Select Case contactType
            Case ContactTypeEnum.AddressBook
                Dim addressId As Integer = CInt(idContact)
                Try
                    Dim addressCnt As Contact = Facade.ContactFacade.GetById(addressId, True)
                    addressCnt.CertifiedMail = emailAddress
                    Facade.ContactFacade.UpdateOnly(addressCnt)
                    Return True
                Catch ex As Exception
                    lblError.Text = String.Format("Errore durante l'inserimento dell'indirizzo PEC: {0}.", ex.Message)
                    Return False
                End Try
            Case ContactTypeEnum.Manual
                Dim manualId As Guid = Nothing
                Guid.TryParse(idContact, manualId)
                If Not manualId.Equals(Guid.Empty) Then
                    Try
                        Dim manualCnt As ProtocolContactManual = Facade.ProtocolContactManualFacade.GetById(manualId)
                        manualCnt.Contact.CertifiedMail = emailAddress
                        Facade.ProtocolContactManualFacade.UpdateOnly(manualCnt)
                        Return True
                    Catch ex As Exception
                        lblError.Text = String.Format("Errore durante l'inserimento dell'indirizzo PEC: {0}.", ex.Message)
                        Return False
                    End Try
                End If
        End Select

        Return False
    End Function

#End Region

#Region "Classes"

    Private Class PecContactAddress
        Private _id As String

        Public Property Id() As String
            Get
                Return _id
            End Get
            Set(ByVal value As String)
                _id = value
            End Set
        End Property

        Private _name As String

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Sub New(ByVal id As String, ByVal name As String)
            _name = name
            _id = id
        End Sub
    End Class

#End Region
End Class