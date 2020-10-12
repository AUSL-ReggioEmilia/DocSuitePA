Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtocolDocumentSeriesItemFacade
    Inherits BaseProtocolFacade(Of ProtocolDocumentSeriesItem, Guid, NHibernateProtocolDocumentSeriesItemDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Function GetByProtocol(idProtocol As Guid) As IList(Of ProtocolDocumentSeriesItem)
        Return _dao.GetByProtocol(idProtocol)
    End Function

    ''' <summary> Ritorna le PDS abbinate ad una Protocol. </summary>
    Public Function GetByProtocol(protocol As Protocol) As IList(Of ProtocolDocumentSeriesItem)
        Return GetByProtocol(protocol.Id)
    End Function

    ''' <summary> Ritorna i Protocolli abbinate ad una DS. </summary>
    Public Function GetProtocols(documentSeriesItem As DocumentSeriesItem) As IList(Of Protocol)
        Return _dao.GetProtocols(documentSeriesItem.Id)
    End Function

    Public Function GetDocumentSeriesItemIdentifiers(prot As Protocol) As List(Of Integer)
        Dim protocolSeries As IList(Of ProtocolDocumentSeriesItem) = GetByProtocol(prot).ToList()
        Return protocolSeries.Select(Function(pds) pds.DocumentSeriesItem.Id).ToList()
    End Function

    Public Function LinkProtocolToDocumentSeriesItem(prot As Protocol, item As DocumentSeriesItem) As ProtocolDocumentSeriesItem
        Dim pdsi As ProtocolDocumentSeriesItem = New ProtocolDocumentSeriesItem() With {
            .DocumentSeriesItem = item,
            .Protocol = prot,
            .UniqueIdDocumentSeriesItem = item.UniqueId
        }
        Save(pdsi)
        Return pdsi
    End Function

    Public Sub RemoveLinkProtocolToDocumentSeriesItem(idProtocol As Guid, item As DocumentSeriesItem)
        Dim protocols As IList(Of ProtocolDocumentSeriesItem) = GetByProtocol(idProtocol)
        If protocols.Any() Then
            Dim itemToRemove As ProtocolDocumentSeriesItem = protocols.SingleOrDefault(Function(x) x.DocumentSeriesItem.Id.Equals(item.Id))
            If itemToRemove Is Nothing Then
                Exit Sub
            End If

            Delete(itemToRemove)
        End If
    End Sub
#End Region

End Class
