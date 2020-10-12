Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class CheckPecEmailAddress
    Inherits ProtBasePage

#Region " Fields "

    Private _contactsUpdated As Boolean
    Private _sessionSeed As String

#End Region

#Region " Properties "

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

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not IsPostBack And ContactIds IsNot Nothing Then
            ' Scrivo il messaggio
            lblMessage.Text = Request("Message")
            dlstAddresses.DataSource = BuidAddressesList()
            dlstAddresses.DataBind()
        End If
    End Sub

    Protected Sub BtnConfermaClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        For Each item As RadListViewDataItem In dlstAddresses.Items
            Dim address As String = CType(item.FindControl("txtCertifiedMail"), RadTextBox).Text
            Dim tmpId As String = item.GetDataKeyValue("Id").ToString()

            Dim addressId As Integer
            If Integer.TryParse(tmpId, addressId) Then
                ' contatto da rubrica
                _contactsUpdated = UpdateContact(tmpId, address, ContactTypeEnum.AddressBook)
            Else
                ' contatto manuale
                _contactsUpdated = UpdateContact(tmpId, address, ContactTypeEnum.Manual)
            End If
        Next
    End Sub

    Protected Sub RadListView_ItemDataBound(sender As Object, e As RadListViewItemEventArgs) Handles dlstAddresses.ItemDataBound
        If TypeOf e.Item Is RadListViewDataItem Then
            Dim item As RadListViewDataItem = DirectCast(e.Item, RadListViewDataItem)
            Dim dataItem As PecContactAddress = DirectCast(item.DataItem, PecContactAddress)
            CType(item.FindControl("idPecAddressName"), Label).Text = String.Format("Casella PEC di ""{0}""", dataItem.Name)
            CType(item.FindControl("txtCertifiedMail"), RadTextBox).Text = dataItem.FatherAddress
        End If
    End Sub

#End Region

#Region "Methods"
    Private Function BuidAddressesList() As IList(Of PecContactAddress)
        Dim addresses As New List(Of PecContactAddress)

        For index As Short = 0 To ContactIds.Length - 1
            Dim idContact As Integer = Convert.ToInt32(ContactIds(index))

            Dim contact As Contact = Facade.ContactFacade.GetById(idContact)
            If contact IsNot Nothing AndAlso contact.Parent IsNot Nothing Then
                Dim fatherAddr As String = contact.Parent.CertifiedMail
                addresses.Add(New PecContactAddress(contact.Id.ToString(), ContactNames(index), IIf(String.IsNullOrEmpty(fatherAddr), String.Empty, fatherAddr).ToString()))
            End If

        Next index

        Return addresses
    End Function

    Private Function UpdateContact(ByVal contactId As String, ByVal emailAddress As String, ByVal contactType As ContactTypeEnum) As Boolean
        Select Case contactType
            Case ContactTypeEnum.AddressBook
                Dim addressId As Integer = CInt(contactId)
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
                Dim manualId As Guid
                Guid.TryParse(contactId, manualId)
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

        Private _fatherAddress As String
        Public Property FatherAddress() As String
            Get
                Return _fatherAddress
            End Get
            Set(ByVal value As String)
                _fatherAddress = value
            End Set
        End Property

        Public Sub New(ByVal id As String, ByVal name As String, ByVal fatherAddress As String)
            _name = name
            _id = id
            _fatherAddress = fatherAddress
        End Sub
    End Class
#End Region

End Class