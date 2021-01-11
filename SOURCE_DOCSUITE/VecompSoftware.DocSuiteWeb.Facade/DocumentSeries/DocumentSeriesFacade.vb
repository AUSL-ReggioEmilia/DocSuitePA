Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Public Class DocumentSeriesFacade
    Inherits BaseProtocolFacade(Of DocumentSeries, Integer, NHibernateDocumentSeriesDao)

    ''' <summary> Recupera le DS da una lista di identificativi </summary>
    ''' <param name="identifiers">Identificativi di riferimento.</param>
    ''' <remarks>Introdotto per GetDocumentSeries in ResolutionDocumentSeriesFacade.</remarks>
    Public Function GetByIdentifiers(identifiers As Integer()) As IList(Of DocumentSeries)
        Return _dao.GetByIdentifiers(identifiers)
    End Function

    ''' <summary> Recupera le DS da una lista di identificativi </summary>
    ''' <param name="identifiers">Identificativi di riferimento.</param>
    Public Function GetByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeries)
        Return GetByIdentifiers(identifiers.ToArray())
    End Function

    Public Function GetPublicationEnabledDocumentSeries() As IList(Of DocumentSeries)
        Return _dao.GetPublicationEnabledDocumentSeries()
    End Function

    Public Function GetSeries(domain As String, userName As String, rights As DocumentSeriesContainerRightPositions) As IList(Of DocumentSeries)
        Dim containers As IList(Of Container) = Factory.ContainerFacade.GetContainers(domain, userName, DSWEnvironment.DocumentSeries, rights, True)
        If containers IsNot Nothing Then
            Return containers.Select(Function(c) GetDocumentSeries(c)).ToList()
        Else
            Return Nothing
        End If
    End Function

    Public Function GetSeries(rights As DocumentSeriesContainerRightPositions) As IList(Of DocumentSeries)
        FileLogger.Debug(LoggerName, String.Format("Inizio caricamento contenitori per diritto {0}", rights))
        Dim containers As IList(Of Container) = Factory.ContainerFacade.GetContainers(DSWEnvironment.DocumentSeries, rights, True)
        FileLogger.Debug(LoggerName, String.Format("Caricati {0} contenitori", If(containers Is Nothing, 0, containers.Count)))
        If containers IsNot Nothing Then
            Dim tor As IList(Of DocumentSeries) = containers.Select(Function(c) GetDocumentSeries(c.Id)).Where(Function(s) s IsNot Nothing).ToList()
            Return tor
        Else
            Return Nothing
        End If
    End Function

    Public Function GetDocumentByContainerId(containerId As Integer) As DocumentSeries
        Return _dao.GetDocumentByContainerId(containerId)
    End Function

    Public Function GetDocumentSeries(container As Container) As DocumentSeries
        Return _dao.GetByContainer(container)
    End Function

    Public Function GetDocumentSeries(idContainer As Integer) As DocumentSeries
        Return _dao.GetByContainer(idContainer)
    End Function


    Public Function GetArchiveInfo(container As Container) As ArchiveInfo
        Dim series As DocumentSeries = GetDocumentSeries(container)
        Return GetArchiveInfo(series)
    End Function

    Public Shared Function GetArchiveInfo(series As DocumentSeries) As ArchiveInfo
        Return New ArchiveInfo(series.Container.DocumentSeriesLocation.ProtBiblosDSDB)
    End Function

    Public Sub FillFinder(finder As DocumentSeriesItemFinder, conditions As List(Of SearchCondition))
        Dim firstId As Integer = finder.IdDocumentSeriesIn.First()
        Dim series As DocumentSeries = Factory.DocumentSeriesFacade.GetById(firstId)

        Dim chains As List(Of BiblosChainInfo) = Service.SearchChains(series.Container.DocumentSeriesLocation.ProtBiblosDSDB, conditions)

        finder.BiblosContentSearchResult = chains
    End Sub

    Public Function GetName(series As DocumentSeries) As String
        Return series.Container.Name
    End Function

    ''' <summary> Attiva o aggiorna i dati della serie documentale </summary>
    ''' <param name="container">Contenitore a cui la serie è collegata</param>
    Public Function ActivateDocumentSeries(container As Container) As DocumentSeries
        Dim series As DocumentSeries = GetDocumentSeries(container)
        If series Is Nothing Then
            series = New DocumentSeries()
            series.Container = container
            series.Name = container.Name
            series.PublicationEnabled = False
            Save(series)
        End If
        Return series
    End Function

    Public Sub DeactivateDocumentSeries(container As Container)
        Dim series As DocumentSeries = GetDocumentSeries(container)
        If series IsNot Nothing Then
            Delete(series)
        End If
        ' Rimuovo i diritti dai Gruppi
        Dim groups As IList(Of ContainerGroup) = Factory.ContainerGroupFacade.GetByIdContainer(container.Id)
        If Not groups.IsNullOrEmpty() Then
            For Each group As ContainerGroup In groups
                group.DocumentSeriesRights = Nothing
            Next
        End If
    End Sub

    Public Function GetEmptyLogSummaries(ByVal idArchive As Integer?) As IList(Of DocumentSeriesLogSummaryDTO)
        Return _dao.GetEmptyLogSummaries(idArchive)
    End Function

    Public Function GetEmptyLogSummaries() As IList(Of DocumentSeriesLogSummaryDTO)
        Return _dao.GetEmptyLogSummaries(Nothing)
    End Function

    Public Function GetSeriesByFamilyAndArchive(idFamily As Integer, idArchive As Integer) As IList(Of DocumentSeries)
        Return _dao.GetSeriesByFamilyAndArchive(idFamily, idArchive)
    End Function

    Public Function GetSeriesByArchive(idSeries As Integer, idArchive As Integer) As DocumentSeries
        Return _dao.GetSeriesByArchive(idSeries, idArchive)
    End Function
End Class
