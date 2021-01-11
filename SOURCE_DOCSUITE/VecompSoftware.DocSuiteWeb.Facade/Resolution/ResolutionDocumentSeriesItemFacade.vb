Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionDocumentSeriesItemFacade
    Inherits BaseResolutionFacade(Of ResolutionDocumentSeriesItem, Integer, NHibernateResolutionDocumentSeriesItemDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

    ''' <summary> Ritorna le RDS abbinate ad una Resolution. </summary>
    Public Function GetByResolution(idResolution As Integer) As IList(Of ResolutionDocumentSeriesItem)
        Return _dao.GetByIdResolution(idResolution)
    End Function

    ''' <summary> Ritorna TUTTE le RDS. </summary>
    Public Function GetResolutionDocumentSeries() As IList(Of ResolutionDocumentSeriesItem)
        Return _dao.GetResolutionDocumentSeries()
    End Function

    ''' <summary> Ritorna le RDS abbinate ad una Resolution. </summary>
    Public Function GetByResolution(resolution As Resolution) As IList(Of ResolutionDocumentSeriesItem)
        Return GetByResolution(resolution.Id)
    End Function

    ''' <summary> Recupera gli id di DS abbinati ad una Resolution. </summary>
    Public Function GetDocumentSeriesItemIdentifiers(resolution As Resolution) As List(Of Integer)
        Dim resolutionSeries As IList(Of ResolutionDocumentSeriesItem) = GetByResolution(resolution).ToList()
        Return resolutionSeries.Select(Function(rds) rds.IdDocumentSeriesItem.Value).ToList()
    End Function


    ''' <summary> Recupera le DS abbinate ad una Resolution. </summary>
    Public Function GetDocumentSeriesItems(resolution As Resolution) As IList(Of DocumentSeriesItem)
        Dim identifiers As List(Of Integer) = GetDocumentSeriesItemIdentifiers(resolution)
        Return Factory.DocumentSeriesItemFacade.GetByIdentifiers(identifiers)
    End Function

    ''' <summary> Recupera le IDS, non in stato bozze, abbinate ad una Resolution. </summary>
    Public Function GetDocumentSeriesItemsNotDraft(resolution As Resolution) As IList(Of DocumentSeriesItem)
        Dim identifiers As List(Of Integer) = GetDocumentSeriesItemIdentifiers(resolution)
        Return Factory.DocumentSeriesItemFacade.GetItemNotDraftByIdentifiers(identifiers)
    End Function

    ''' <summary> Ritorna le RDS abbinate ad una DS. </summary>
    Public Function GetByDocumentSeriesItem(idDocumentSeriesItem As Integer) As ResolutionDocumentSeriesItem
        Dim documentSeriesItems As IList(Of ResolutionDocumentSeriesItem) = _dao.GetByIdDocumentSeriesItem(idDocumentSeriesItem)
        Try
            Return documentSeriesItems.Single()
        Catch ex As Exception
            Throw New InvalidOperationException("Errore nel caricamento delle serie documentali: sono presenti più serie documentali dello stesso tipo associate all'atto selezionato", ex)
        End Try
    End Function


    ''' <summary> Ritorna le Resolution abbinate ad una DS. </summary>
    Public Function GetResolutions(documentSeriesItem As DocumentSeriesItem) As IList(Of Resolution)
        Return _dao.GetResolutions(documentSeriesItem.Id)
    End Function

    Public Function LinkResolutionToDocumentSeriesItem(resl As Resolution, item As DocumentSeriesItem) As ResolutionDocumentSeriesItem
        Dim rdsi As New ResolutionDocumentSeriesItem() With {
            .IdDocumentSeriesItem = item.Id,
            .UniqueIdDocumentSeriesItem = item.UniqueId,
            .Resolution = resl,
            .UniqueIdResolution = resl.UniqueId
        }
        Save(rdsi)
        Return rdsi
    End Function

    Public Sub RemoveLinkResolutionToDocumentSeriesItem(resl As Resolution, item As DocumentSeriesItem)
        Dim resolutions As IList(Of ResolutionDocumentSeriesItem) = GetByResolution(resl.Id)
        If resolutions.Any() Then
            Dim itemToRemove As ResolutionDocumentSeriesItem = resolutions.SingleOrDefault(Function(x) x.IdDocumentSeriesItem.Equals(item.Id))
            If itemToRemove Is Nothing Then
                Exit Sub
            End If

            Delete(itemToRemove)
        End If
    End Sub


End Class
