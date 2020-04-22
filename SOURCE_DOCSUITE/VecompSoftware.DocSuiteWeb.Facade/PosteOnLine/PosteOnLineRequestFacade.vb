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

    Sub SaveAll(ByVal request As POLRequest)
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom("ProtDB").BeginTransaction()
        Try
            Me.Save(request, ProtDB, needTransaction:=False)
            FacadeFactory.Instance.PosteOnLineContactFacade.SaveWithoutTransaction(request.Sender)
            For Each recipient As POLRequestContact In request.Recipients
                FacadeFactory.Instance.PosteOnLineContactFacade.SaveWithoutTransaction(recipient)
            Next

            transaction.Commit()
        Catch ex As Exception
            transaction.Rollback()
            Throw
        End Try
    End Sub

    Public Function GetByProtocol(ByVal year As Short, ByVal number As Integer) As IList(Of POLRequest)
        Return _dao.GetByProtocol(year, number)
    End Function

    Public Function GetRecipientByProtocol(ByVal year As Short, ByVal number As Integer) As IList(Of POLRequestRecipientHeader)
        Return _dao.GetRecipientsByProtocol(year, number)
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
        pol.ProtocolYear = protocolDTO.Year
        pol.ProtocolNumber = protocolDTO.Number
        pol.RegistrationDate = Me._dao.GetServerDate()
        pol.Sender.RegistrationDate = pol.RegistrationDate
        For Each item As POLRequestRecipient In pol.Recipients
            item.RegistrationDate = pol.RegistrationDate
        Next

        FacadeFactory.Instance.PosteOnLineRequestFacade.SaveAll(pol)
        Return pol.Id
    End Function

    Public Function PairToProtocol(mail As POLRequest, year As Short, number As Integer) As POLRequest
        If mail.ProtocolYear.HasValue AndAlso mail.ProtocolNumber.HasValue Then
            Dim message As String = "Questa mail ha già un protocollo associato ({0})."
            message = String.Format(message, ProtocolFacade.ProtocolFullNumber(year, number))
            Throw New InvalidOperationException(message)
        End If

        mail.ProtocolYear = year
        mail.ProtocolNumber = number

        Me.Update(mail)
        Return mail
    End Function

    Public Function PairToProtocol(mailDTO As API.MailDTO, protocolDTO As API.ProtocolDTO) As Guid
        If Not DirectCast(mailDTO.Mailbox, API.MailboxDTO).IsPOL() Then
            Throw New InvalidOperationException("MailDTO specificato non è di tipo ""POL"".")
        End If

        Dim polId As New Guid(mailDTO.Id)
        Dim mail As POLRequest = FacadeFactory.Instance.PosteOnLineRequestFacade.GetById(polId)
        Dim updated As POLRequest = Me.PairToProtocol(mail, protocolDTO.Year.Value, protocolDTO.Number.Value)
        Return updated.Id
    End Function

#End Region

End Class

