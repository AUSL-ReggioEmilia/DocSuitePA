Imports NHibernate
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.Services.Biblos


<ComponentModel.DataObject()> _
Public Class PosteOnLineRequestFacade
    Inherits BaseProtocolFacade(Of POLRequest, Guid, NHibernatePosteOnlineRequestDao)

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Methods "

    Sub SaveAll(ByVal request As POLRequest, Optional ByVal excludeSender As Boolean = False)
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try
            Me.Save(request, ProtDB, needTransaction:=False)

            If Not excludeSender Then
                'Sending TNotice will not have any information in the sender
                FacadeFactory.Instance.PosteOnLineContactFacade.SaveWithoutTransaction(request.Sender)
            End If

            For Each recipient As POLRequestContact In request.Recipients
                FacadeFactory.Instance.PosteOnLineContactFacade.SaveWithoutTransaction(recipient)
            Next

            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            Throw
        End Try
    End Sub

    Public Function GetByProtocol(idProtocol As Guid) As IList(Of POLRequest)
        Return _dao.GetByProtocol(idProtocol)
    End Function

    Public Function GetRecipientByProtocol(idProtocol As Guid) As IList(Of POLRequestRecipientHeader)
        Return _dao.GetRecipientsByProtocol(idProtocol)
    End Function

    Public Function Costi(ByVal dateFrom As DateTime?, ByVal dateTo As DateTime?) As IList(Of PolDtoCosti)
        Return _dao.Costi(dateFrom, dateTo)
    End Function
    Public Function CostiAccount(ByVal dateFrom As DateTime?, ByVal dateTo As DateTime?) As IList(Of PolDtoCosti)
        Return _dao.CostiAccount(dateFrom, dateTo)
    End Function

    Public Function GetOngoingRaccomandate(account As POLAccount) As IList(Of ROLRequest)
        Return _dao.GetOngoingRaccomandate(account)
    End Function

    Public Function GetConfirmedRaccomandate(account As POLAccount) As IList(Of ROLRequest)
        Return _dao.GetConfirmedRaccomandate(account)
    End Function

    Public Function GetOngoingLettere(account As POLAccount) As IList(Of LOLRequest)
        Return _dao.GetOngoingLettere(account)
    End Function

    Public Function GetOngoingTelegrammi(account As POLAccount) As IList(Of TOLRequest)
        Return _dao.GetOngoingTelegrammi(account)
    End Function

    Public Function GetOngoingSerc(account As POLAccount) As IList(Of SOLRequest)
        Return _dao.GetOngoingSerc(account)
    End Function

    Public Function SendLettera(dto As API.MailDTO, protocolDTO As API.ProtocolDTO) As Guid
        Dim pol As POLRequest = dto.ToPOLRequest(protocolDTO)
        Dim protocol As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(protocolDTO.UniqueId.Value)
        pol.DocumentUnit = FacadeFactory.Instance.DocumentUnitFacade.GetById(protocol.Id)
        pol.RegistrationDate = Me._dao.GetServerDate()
        pol.Sender.RegistrationDate = pol.RegistrationDate
        For Each item As POLRequestRecipient In pol.Recipients
            item.RegistrationDate = pol.RegistrationDate
        Next

        FacadeFactory.Instance.PosteOnLineRequestFacade.SaveAll(pol)
        Return pol.Id
    End Function

    Public Function PairToProtocol(mail As POLRequest, uniqueIdProtocol As Guid) As POLRequest
        If mail.DocumentUnit IsNot Nothing Then
            Dim message As String = "Questa mail ha già un protocollo associato ({0})."
            message = String.Format(message, uniqueIdProtocol)
            Throw New InvalidOperationException(message)
        End If

        Dim protocol As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(uniqueIdProtocol)
        mail.DocumentUnit = FacadeFactory.Instance.DocumentUnitFacade.GetById(protocol.Id)

        Me.Update(mail)
        Return mail
    End Function

    Public Function PairToProtocol(mailDTO As API.MailDTO, protocolDTO As API.ProtocolDTO) As Guid
        If Not DirectCast(mailDTO.Mailbox, API.MailboxDTO).IsPOL() Then
            Throw New InvalidOperationException("MailDTO specificato non è di tipo ""POL"".")
        End If

        Dim polId As New Guid(mailDTO.Id)
        Dim mail As POLRequest = FacadeFactory.Instance.PosteOnLineRequestFacade.GetById(polId)
        Dim updated As POLRequest = Me.PairToProtocol(mail, protocolDTO.UniqueId.Value)
        Return updated.Id
    End Function

#End Region

End Class

