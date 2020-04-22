Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos

<Serializable> _
Public Class DocumentSeriesItemLinkFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesItemLink, Integer, NHibernateDocumentSeriesItemLinkDao)

    Public Function LinkResolutionToDocumentSeriesItem(resl As Resolution, item As DocumentSeriesItem) As DocumentSeriesItemLink
        Dim rdsi As New DocumentSeriesItemLink() With {
            .DocumentSeriesItem = item,
            .UniqueIdDocumentSeriesItem = item.UniqueId,
            .Resolution = resl,
            .UniqueIdResolution = resl.UniqueId,
            .LinkType = If(resl.Type.Description.Eq("Atto"), DocumentSeriesItemLinkType.Atto, DocumentSeriesItemLinkType.Delibera)
        }
        Save(rdsi)
        Return rdsi
    End Function

End Class
