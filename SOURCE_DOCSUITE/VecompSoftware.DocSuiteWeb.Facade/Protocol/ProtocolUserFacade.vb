Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods

<DataObject()>
Public Class ProtocolUserFacade
    Inherits BaseProtocolFacade(Of ProtocolUser, Guid, NHibernateProtocolUserDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Sub SetHighlightUser(protocol As Protocol, account As String, note As String)
        If protocol IsNot Nothing AndAlso Not String.IsNullOrEmpty(account) AndAlso Not protocol.Users.Any(Function(x) x.Account.Eq(account) AndAlso x.Type = ProtocolUserType.Highlight) Then
            Dim protocolHighlightUser As ProtocolUser = New ProtocolUser() With {
                    .Account = account,
                    .UniqueIdProtocol = protocol.UniqueId,
                    .Protocol = protocol,
                    .Type = ProtocolUserType.Highlight,
                    .Note = note
                }
            Save(protocolHighlightUser)
            FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PH, String.Format("Protocollo {0} in evidenza a {1}", protocol.FullNumber, account))
        End If
    End Sub

    Public Sub RemoveHighlightUser(protocol As Protocol, account As String)
        If protocol.Users.Any(Function(x) x.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso x.Type = ProtocolUserType.Highlight) Then
            Dim item As ProtocolUser = protocol.Users.Single(Function(x) x.Account.Eq(account) AndAlso x.Type = ProtocolUserType.Highlight)
            protocol.Users.Remove(item)
            Delete(item)
            FacadeFactory.Instance.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PH, String.Format("Rimossa evidenza del protocollo {0} a {1}", protocol.FullNumber, account))
        End If
    End Sub

    Public Sub RemoveHighlightUser(id As YearNumberCompositeKey, account As String)
        Dim protocol As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(id.Year.Value, id.Number.Value)
        RemoveHighlightUser(protocol, account)
    End Sub

    Public Function GetProtocolUsersByProtocolUniqueId(protocolId As Guid, accountUser As String) As ProtocolUser
        Return _dao.GetProtocolUserByProtocolUniqueId(protocolId, accountUser)
    End Function
#End Region
End Class