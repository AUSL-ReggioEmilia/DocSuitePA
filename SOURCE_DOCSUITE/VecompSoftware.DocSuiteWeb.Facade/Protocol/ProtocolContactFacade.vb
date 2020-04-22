Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel

<DataObject()> _
Public Class ProtocolContactFacade
    Inherits BaseProtocolFacade(Of ProtocolContact, ProtocolContactCompositeKey, NHibernateProtocolContactDao)

    Public Sub New()
    End Sub

    ''' <summary> Lista di ProtocolContact filtrati per Year, Number e ComunicationType </summary>
    ''' <param name="year">Anno Protocollo</param>
    ''' <param name="number">Numero Protocollo</param>
    Public Function GetByComunicationType(ByVal year As Short, ByVal number As Integer, ByVal comunicationType As String) As IList(Of ProtocolContact)
        Return _dao.GetByComunicationType(year, number, comunicationType)
    End Function

    Public Function GetCountByProtocol(ByVal year As Short, ByVal number As Integer, ByVal comunicationType As String) As Integer
        Return _dao.GetCountByProtocol(year, number, comunicationType)
    End Function

    Function GetJournalPrint(ByVal idContainers As String, ByVal dateFrom As Date?, ByVal dateTo As Date?, ByVal idStatus As Integer?) As IList(Of ProtocolContactJournalDTO)
        Return _dao.GetJournalPrint(idContainers, dateFrom, dateTo, idStatus)
    End Function

    ''' <summary> Aggiunge un contatto al protocollo </summary>
    ''' <remarks> Verifica integrità dei dati </remarks>
    Public Shared Sub BindContactToProtocol(ByRef protocol As Protocol, ByVal contact As Contact, ByVal comunicationType As Char, ByVal copiaConoscenza As Boolean)
        Dim id As New ProtocolContactCompositeKey
        id.IdContact = contact.Id
        id.Year = protocol.Year
        id.Number = protocol.Number
        id.ComunicationType = comunicationType

        If protocol.Contacts.FirstOrDefault(Function(x) x.Id.Equals(id)) Is Nothing Then
            Dim pc As New ProtocolContact(id)
            pc.Contact = contact
            If copiaConoscenza Then
                pc.Type = "CC"
            End If
            pc.Protocol = protocol
            pc.UniqueIdProtocol = protocol.UniqueId

            protocol.Contacts.Add(pc)
        End If

    End Sub

    ''' <summary>
    ''' Recupera il contatto relativo al Sistema di interscambio
    ''' </summary>
    ''' <returns>Oggetto DTO contenente i dati del Sistema di interscambio</returns>
    Public Shared Function GetSdiContactDto(protocol As Protocol) As ContactDTO

        Dim currentContainer As Data.Container = FacadeFactory.Instance.ContainerFacade.GetById(protocol.Container.Id, False)
        Dim currentContainerEnv As ContainerEnv = New ContainerEnv(DocSuiteContext.Current, currentContainer)

        If String.IsNullOrEmpty(currentContainerEnv.InvoicePAContactSDI) Then
            Return Nothing
        End If

        Dim validEmail As Boolean = RegexHelper.IsValidEmail(currentContainerEnv.InvoicePAContactSDI)
        If Not validEmail Then
            Return Nothing
        End If

        Dim sdicontact As New Contact
        sdicontact.IsActive = 1
        sdicontact.Parent = Nothing
        sdicontact.ContactType = New ContactType(ContactType.Administration)
        sdicontact.Description = "Sistema di Interscambio"
        sdicontact.CertifiedMail = currentContainerEnv.InvoicePAContactSDI
        sdicontact.Role = Nothing

        Dim contactDto As New ContactDTO(sdicontact, ContactDTO.ContactType.Manual)
        Return contactDto
    End Function

    Public Function GetInvoiceContactGroup(container As Data.Container) As Contact
        Dim currentContainer As Data.Container = FacadeFactory.Instance.ContainerFacade.GetById(container.Id, False)
        Dim currentContainerEnv As ContainerEnv = New ContainerEnv(DocSuiteContext.Current, currentContainer)
        Dim groupName As String = currentContainerEnv.ImportInvoiceContactGroupName
        Dim contactsByName As IList(Of Contact) = FacadeFactory.Instance.ContactFacade.GetContactByDescription(groupName, NHibernateContactDao.DescriptionSearchType.Equal, True, New List(Of Integer)())

        If contactsByName.Any() Then
            Return contactsByName.First()
        End If

        Dim group As New Contact()
        group.Description = groupName
        group.ContactType = New ContactType(ContactType.Group)
        group.IsActive = 1
        FacadeFactory.Instance.ContactFacade.Save(group)
        Return group
    End Function

End Class
