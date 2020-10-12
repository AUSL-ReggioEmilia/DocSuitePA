Imports System
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()>
Public Class PosteOnLineContactFacade
    Inherits BaseProtocolFacade(Of POLRequestContact, Guid, NHibernatePosteOnlineContactDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetDefaultSender() As POLRequestSender
        Dim sender As New POLRequestSender()
        sender.Id = Guid.NewGuid()

        ' "Name|Address|City|CivicNumber|ZipCode|Province"
        Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.PosteWebDefaultSender.Split("|"c)
        sender.Name = splitted(0).Trim()
        sender.Address = splitted(1).Trim()
        sender.City = splitted(2).Trim()
        sender.CivicNumber = splitted(3).Trim()
        sender.ZipCode = splitted(4).Trim()
        sender.Province = splitted(5).Trim()

        Return sender
    End Function


    ''' <summary> Configura l'indirizzo del mittente con l'indirizzo ricevuto come parametro. </summary>
    Private Sub SetSenderAddress(ByRef sender As POLRequestSender, address As Address)
        Dim currentPlace As String = String.Empty
        If address.PlaceName IsNot Nothing Then
            currentPlace = address.PlaceName.Description.Trim()
        End If
        sender.Address = String.Format("{0} {1}", currentPlace, address.Address).Trim()
        sender.City = String.Format("{0}", address.City).Trim()
        sender.CivicNumber = String.Format("{0}", address.CivicNumber).Trim()
        sender.ZipCode = String.Format("{0}", address.ZipCode).Trim()
        sender.Province = String.Format("{0}", address.CityCode).Trim()
    End Sub

    Public Sub RecursiveSetSenderAddress(ByRef sender As POLRequestSender, contact As Contact)
        If contact Is Nothing Then
            Throw New Exception("Non è stato possibile recuperare un indirizzo valido per il mittente specificato.")
        End If

        If contact IsNot Nothing AndAlso contact.Address IsNot Nothing AndAlso contact.Address.IsValidAddress Then
            SetSenderAddress(sender, contact.Address)
        Else
            If contact.Parent IsNot Nothing AndAlso contact.Parent.Address IsNot Nothing Then
                RecursiveSetSenderAddress(sender, contact.Parent)
            Else
                Throw New Exception("Non è stato possibile recuperare un indirizzo valido per il mittente specificato.")
            End If
        End If
    End Sub

    ''' <summary> Configura l'indirizzo del destinatario con l'indirizzo ricevuto come parametro. </summary>
    Private Sub SetRecipientAddress(ByRef recipient As POLRequestRecipient, address As Address)
        Dim currentPlace As String = String.Empty
        If address.PlaceName IsNot Nothing Then
            currentPlace = address.PlaceName.Description.Trim()
        End If
        recipient.Address = String.Format("{0} {1}", currentPlace, address.Address).Trim()
        recipient.City = String.Format("{0}", address.City).Trim()
        recipient.CivicNumber = String.Format("{0}", address.CivicNumber).Trim()
        recipient.ZipCode = String.Format("{0}", address.ZipCode).Trim()
        recipient.Province = String.Format("{0}", address.CityCode).Trim()
    End Sub

    Public Sub TryRecursiveSetRecipientAddress(ByRef recipient As POLRequestRecipient, contact As Contact)
        If contact IsNot Nothing AndAlso contact.Address IsNot Nothing AndAlso Not String.IsNullOrEmpty(contact.Address.Address) Then
            SetRecipientAddress(recipient, contact.Address)
        ElseIf contact.Parent IsNot Nothing Then
            TryRecursiveSetRecipientAddress(recipient, contact.Parent)
        End If
    End Sub

    Public Sub RecursiveSetRecipientAddress(ByRef recipient As POLRequestRecipient, contact As Contact)
        If contact IsNot Nothing AndAlso contact.Address IsNot Nothing AndAlso Not String.IsNullOrEmpty(contact.Address.Address) Then
            SetRecipientAddress(recipient, contact.Address)
        ElseIf contact.Parent IsNot Nothing Then
            RecursiveSetRecipientAddress(recipient, contact.Parent)
        Else
            'todo: cercare se esiste un contatto in rubrica che ha l'indirizzo mail del contatto manuale e poi in caso genero errore

            Throw New Exception("Non è stato possibile recuperare un indirizzo valido per il destinatario specificato.")
        End If
    End Sub

    Public Function GetRecipientWithRequestId(ByVal requestId As Guid) As POLRequestRecipient
        Return _dao.GetRecipientWithRequestId(requestId)
    End Function

End Class

