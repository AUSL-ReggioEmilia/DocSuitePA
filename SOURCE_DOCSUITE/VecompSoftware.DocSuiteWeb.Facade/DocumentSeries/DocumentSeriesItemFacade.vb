Imports System.Linq
Imports VecompSoftware.Core.Command
Imports VecompSoftware.Core.Command.CQRS.Commands.Entities.DocumentArchives
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.DocumentSeries
Imports VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.Services.Command.CQRS.Commands.Entities.DocumentArchives
Imports VecompSoftware.Services.Logging
Imports APISeriesItem = VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports APIDocUnit = VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits

<Serializable>
Public Class DocumentSeriesItemFacade
    Inherits BaseProtocolFacade(Of DocumentSeriesItem, Integer, NHibernateDocumentSeriesItemDao)
#Region " Fields "

    Private _commandInsertFacade As CommandFacade(Of ICommandCreateDocumentSeriesItem)
    Private _commandUpdateFacade As CommandFacade(Of ICommandUpdateDocumentSeriesItem)
    Private _mapperCategoryFascicle As MapperCategoryFascicle
    Private _categoryFascicleDao As CategoryFascicleDao
    Private _documentUnitFinder As DocumentUnitFinder

#End Region

#Region " Properties "
    Private ReadOnly Property CommandInsertFacade As CommandFacade(Of ICommandCreateDocumentSeriesItem)
        Get
            If _commandInsertFacade Is Nothing Then
                _commandInsertFacade = New CommandFacade(Of ICommandCreateDocumentSeriesItem)
            End If
            Return _commandInsertFacade
        End Get
    End Property

    Private ReadOnly Property CommandUpdateFacade As CommandFacade(Of ICommandUpdateDocumentSeriesItem)
        Get
            If _commandUpdateFacade Is Nothing Then
                _commandUpdateFacade = New CommandFacade(Of ICommandUpdateDocumentSeriesItem)
            End If
            Return _commandUpdateFacade
        End Get
    End Property

    Public ReadOnly Property MapperCategoryFascicle As MapperCategoryFascicle
        Get
            If _mapperCategoryFascicle Is Nothing Then
                _mapperCategoryFascicle = New MapperCategoryFascicle
            End If
            Return _mapperCategoryFascicle
        End Get
    End Property

    'Si utilizza il Dao per problemi di referenze circolari
    Public ReadOnly Property CategoryFascicleDao As CategoryFascicleDao
        Get
            If _categoryFascicleDao Is Nothing Then
                _categoryFascicleDao = New CategoryFascicleDao(ProtDB)
            End If
            Return _categoryFascicleDao
        End Get
    End Property

    Private ReadOnly Property CurrentDocumentUnitFinder As DocumentUnitFinder
        Get
            If _documentUnitFinder Is Nothing Then
                _documentUnitFinder = New DocumentUnitFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _documentUnitFinder
        End Get
    End Property
#End Region

#Region " Methods "
    Public Function GetByYearAndNumber(series As DocumentSeries, year As Integer, number As Integer) As DocumentSeriesItem
        Return _dao.GetByYearAndNumber(series, year, number)
    End Function

    Public Function GetByDocumentId(idDocument As Guid) As IList(Of DocumentSeriesItem)
        Return _dao.GetByDocument(idDocument)
    End Function

    Public Function GetByUniqueId(uniqueId As Guid) As DocumentSeriesItem
        Return _dao.GetByUniqueId(uniqueId)
    End Function

    Public Function GetByArchive(idArchive As Integer, topResults As Integer) As IList(Of DocumentSeriesItem)
        Return _dao.GetLastSeriesByArchive(idArchive, topResults)
    End Function

    Public Function SaveDocumentSeriesItem(item As DocumentSeriesItem, chain As BiblosChainInfo, annexed As IList(Of DocumentInfo), unpublishedAnnexed As IList(Of DocumentInfo), registrationUser As String) As DocumentSeriesItem
        Return SaveDocumentSeriesItem(item, Nothing, chain, annexed, unpublishedAnnexed, registrationUser, DocumentSeriesItemStatus.Active, "")
    End Function

    Public Function SaveDocumentSeriesItem(item As DocumentSeriesItem, year As Integer, chain As BiblosChainInfo, annexed As IList(Of DocumentInfo), unpublishedAnnexed As IList(Of DocumentInfo), status As DocumentSeriesItemStatus, logMessage As String) As DocumentSeriesItem
        Return SaveDocumentSeriesItem(item, year, chain, annexed, unpublishedAnnexed, Nothing, status, logMessage)
    End Function

    Public Function SaveDocumentSeriesItem(item As DocumentSeriesItem, chain As BiblosChainInfo, annexed As IList(Of DocumentInfo), unpublishedAnnexed As IList(Of DocumentInfo), status As DocumentSeriesItemStatus, logMessage As String) As DocumentSeriesItem
        Return SaveDocumentSeriesItem(item, Nothing, chain, annexed, unpublishedAnnexed, Nothing, status, logMessage)
    End Function

    Public Function SaveDocumentSeriesItem(item As DocumentSeriesItem, year As Integer, chain As BiblosChainInfo, annexed As IList(Of DocumentInfo), unpublishedAnnexed As IList(Of DocumentInfo), registrationUser As String, status As DocumentSeriesItemStatus, logMessage As String, Optional needTransaction As Boolean = True) As DocumentSeriesItem

        Dim archiviedDocs As New List(Of BiblosDocumentInfo)()
        Dim archiviedAnnexed As New List(Of BiblosDocumentInfo)()
        Dim archiviedUnpublishedAnnexed As New List(Of BiblosDocumentInfo)()

        item.Location = item.DocumentSeries.Container.DocumentSeriesLocation
        item.LocationAnnexed = item.DocumentSeries.Container.DocumentSeriesAnnexedLocation
        item.LocationUnpublishedAnnexed = item.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation

        item.IdMain = chain.ArchiveInBiblos(item.DocumentSeries.Container.DocumentSeriesLocation.ProtBiblosDSDB)
        archiviedDocs = chain.ArchivedDocuments

        item.HasMainDocument = archiviedDocs IsNot Nothing AndAlso archiviedDocs.Count > 0

        Dim archiviedDoc As BiblosDocumentInfo
        ' Salvo gli annessi in Biblos e ne ricavo l'id di catena
        If Not annexed.IsNullOrEmpty() AndAlso Not item.LocationAnnexed Is Nothing Then
            For Each doc As DocumentInfo In annexed
                archiviedDoc = Nothing
                archiviedDoc = doc.ArchiveInBiblos(item.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtBiblosDSDB, item.IdAnnexed)
                item.IdAnnexed = archiviedDoc.ChainId
                archiviedAnnexed.Add(archiviedDoc)
            Next
        End If

        If Not unpublishedAnnexed.IsNullOrEmpty() AndAlso Not item.LocationUnpublishedAnnexed Is Nothing Then
            archiviedDoc = Nothing
            For Each doc As DocumentInfo In unpublishedAnnexed
                archiviedDoc = doc.ArchiveInBiblos(item.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation.ProtBiblosDSDB, item.IdUnpublishedAnnexed)
                item.IdUnpublishedAnnexed = archiviedDoc.ChainId
                archiviedUnpublishedAnnexed.Add(archiviedDoc)
            Next
        End If

        If status <> DocumentSeriesItemStatus.Draft Then
            Dim inc As DocumentSeriesIncremental
            If year.Equals(Nothing) Then
                year = DateTime.Today.Year
            End If
            inc = Factory.DocumentSeriesIncrementalFacade.GetNewIncremental(item.DocumentSeries, year)
            item.Year = inc.Year
            item.Number = inc.LastUsedNumber
        End If


        If item.RetireDate.HasValue AndAlso item.PublishingDate.HasValue AndAlso item.PublishingDate.Value > item.RetireDate.Value Then
            Throw New DocSuiteException("Salvataggio", "Data ritiro deve essere inferiore a Data pubblicazione.")
        End If

        item.Status = status

        Validate(item)

        If String.IsNullOrEmpty(registrationUser) Then
            Save(item, _dbName, needTransaction)
        Else
            ImpersonificatedSave(item, registrationUser, needTransaction)
        End If

        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Insert, logMessage, needTransaction)

        If (Not String.IsNullOrEmpty(item.ConstraintValue)) Then
            Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.SO, String.Concat("Impostato obbligo trasparenza '", item.ConstraintValue, "'"), needTransaction)
        End If

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            Factory.DocumentSeriesItemLogFacade.AddInsertedDocumentPrivacyLevelLog(item, archiviedDocs, "documento")
            Factory.DocumentSeriesItemLogFacade.AddInsertedDocumentPrivacyLevelLog(item, archiviedAnnexed, "annesso")
            Factory.DocumentSeriesItemLogFacade.AddInsertedDocumentPrivacyLevelLog(item, archiviedUnpublishedAnnexed, "annesso da non pubblicare")
        End If

        If status = DocumentSeriesItemStatus.Active Then
            SendInsertDocumentSeriesItemCommand(item)
        End If

        Return item
    End Function

    Public Sub AssignNumber(item As DocumentSeriesItem)
        If item.Status <> DocumentSeriesItemStatus.Draft Then
            Return
        End If

        ' Assegno gli identificativi year e number 
        Dim inc As DocumentSeriesIncremental = Factory.DocumentSeriesIncrementalFacade.GetNewIncremental(item.DocumentSeries, item.Year.GetValueOrDefault(DateTime.Today.Year))
        item.Year = inc.Year
        item.Number = inc.LastUsedNumber

        ' Attivo la documentSeriesItem
        item.Status = DocumentSeriesItemStatus.Active

        Update(item)
        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Edit, $"Attivata la registrazione {item.Year:0000}/{item.Number:0000000}")
    End Sub

    Public Sub AssignNumberByYear(item As DocumentSeriesItem, year As Integer)
        If item.Status <> DocumentSeriesItemStatus.Draft Then
            Return
        End If

        ' Assegno gli identificativi year e number 
        '  Dim inc As DocumentSeriesIncremental = Factory.DocumentSeriesIncrementalFacade.GetNewIncremental(item.DocumentSeries, item.Year.GetValueOrDefault(DateTime.Today.Year))
        Dim inc As DocumentSeriesIncremental = getIncrementalNumber(item, year)
        item.Year = inc.Year
        item.Number = inc.LastUsedNumber

        ' Attivo la documentSeriesItem
        item.Status = DocumentSeriesItemStatus.Active

        Update(item)
        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Edit, $"Attivata la registrazione {item.Year:0000}/{item.Number:0000000}")
    End Sub

    Private Function getIncrementalNumber(item As DocumentSeriesItem, year As Integer) As DocumentSeriesIncremental

        If item.Status <> DocumentSeriesItemStatus.Draft Then
            Return Nothing
        End If

        ' Assegno gli identificativi year e number 
        Dim inc As DocumentSeriesIncremental = Factory.DocumentSeriesIncrementalFacade.GetNewIncremental(item.DocumentSeries, year)

        Return inc

    End Function

    Public Sub ImpersonificatedSave(item As DocumentSeriesItem, registrationUser As String, Optional needTransaction As Boolean = True)
        _dao.ConnectionName = _dbName
        item.RegistrationDate = DateTimeOffset.UtcNow
        item.RegistrationUser = registrationUser
        item.LastChangedDate = Nothing
        item.LastChangedUser = Nothing

        If needTransaction Then
            _dao.Save(item)
        Else
            _dao.SaveWithoutTransaction(item)
        End If
    End Sub

    Public Sub ImpersonificatedUpdate(item As DocumentSeriesItem, registrationUser As String)
        _dao.ConnectionName = _dbName
        item.LastChangedDate = DateTimeOffset.UtcNow
        item.LastChangedUser = registrationUser
        _dao.Update(item)
    End Sub

    Public Sub UpdateDocumentSeriesItem(item As DocumentSeriesItem, chain As BiblosChainInfo, logMessage As String)
        UpdateDocumentSeriesItem(item, chain, String.Empty, logMessage)
    End Sub

    Public Sub UpdateDocumentSeriesItem(item As DocumentSeriesItem, chain As BiblosChainInfo, registrationUser As String, logMessage As String)
        UpdateDocumentSeriesItem(item, chain, New List(Of DocumentInfo), New List(Of DocumentInfo), registrationUser, logMessage)
    End Sub

    Public Sub UpdateDocumentSeriesItem(item As DocumentSeriesItem, chain As BiblosChainInfo, annexes As IList(Of DocumentInfo), unpublishedAnnexed As IList(Of DocumentInfo), logMessage As String)
        UpdateDocumentSeriesItem(item, chain, annexes, unpublishedAnnexed, Nothing, logMessage)
    End Sub

    Public Sub UpdateDocumentSeriesItem(item As DocumentSeriesItem, chain As BiblosChainInfo, annexes As IList(Of DocumentInfo), unpublishedAnnexed As IList(Of DocumentInfo), registrationUser As String, logMessage As String)
        chain.Update("DSW8")
        chain = GetMainChainInfo(item)
        item.HasMainDocument = chain.Documents.Count > 0

        If annexes IsNot Nothing Then
            For Each annex As DocumentInfo In annexes
                AddAnnexed(item, annex)
            Next
        End If

        If unpublishedAnnexed IsNot Nothing Then
            For Each annex As DocumentInfo In unpublishedAnnexed
                AddUnpublishedAnnexed(item, annex)
            Next
        End If

        Validate(item)

        If String.IsNullOrEmpty(registrationUser) Then
            Update(item)
        Else
            ImpersonificatedUpdate(item, registrationUser)
            Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Edit, String.Format("Utente impersonificato: {0}", registrationUser))
        End If
        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Edit, logMessage)

    End Sub

    Public Function GetMainDocuments(item As DocumentSeriesItem, Optional sortResult As Boolean = False) As List(Of BiblosDocumentInfo)
        Dim documents As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(item.IdMain, True)
        If documents.IsNullOrEmpty() AndAlso Not item.DocumentSeries.AllowNoDocument.GetValueOrDefault(False) Then
            Dim notFound As String = String.Format("Documento non trovato in catena {0}", item.IdMain)
            Throw New InvalidOperationException(notFound)
        End If

        If sortResult Then
            documents = BiblosFacade.SortDocuments(documents)
        End If
        Return documents
    End Function

    Public Function GetMainChainInfo(item As DocumentSeriesItem) As BiblosChainInfo
        Return GetBiblosChainInfo(item.IdMain)
    End Function

    Public Function GetMainChainInfo(dto As DocumentSeriesItemDTO(Of BiblosDocumentInfo)) As BiblosChainInfo
        Return GetBiblosChainInfo(dto.IdMain)
    End Function

    Public Function GetAnnexedChainInfo(item As DocumentSeriesItem) As BiblosChainInfo
        Return GetBiblosChainInfo(item.IdAnnexed)
    End Function

    Public Function GetAnnexedUnpublishedChainInfo(item As DocumentSeriesItem) As BiblosChainInfo
        Return GetBiblosChainInfo(item.IdUnpublishedAnnexed)
    End Function

    Public Function GetAnnexedChainInfo(dto As DocumentSeriesItemDTO(Of BiblosDocumentInfo)) As BiblosChainInfo
        Return GetBiblosChainInfo(dto.IdAnnexed)
    End Function

    Public Function GetBiblosChainInfo(idChain As Guid) As BiblosChainInfo
        Return New BiblosChainInfo(idChain)
    End Function

    Public Function GetAnnexedDocuments(item As DocumentSeriesItem, Optional sortResult As Boolean = False) As List(Of BiblosDocumentInfo)
        Dim annexed As List(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)()
        If item.LocationAnnexed IsNot Nothing Then
            annexed = BiblosDocumentInfo.GetDocuments(item.IdAnnexed)
            If sortResult Then
                annexed = BiblosFacade.SortDocuments(annexed)
            End If
        End If
        Return annexed
    End Function

    Public Function GetAnnexedDocuments(item As DocumentSeriesItemDTO(Of BiblosDocumentInfo), Optional sortResult As Boolean = False) As List(Of BiblosDocumentInfo)
        Dim annexed As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(item.IdAnnexed)
        If sortResult Then
            annexed = BiblosFacade.SortDocuments(annexed)
        End If
        Return annexed
    End Function

    Public Sub AddAnnexed(item As DocumentSeriesItem, annexed As DocumentInfo)
        item.IdAnnexed = annexed.ArchiveInBiblos(item.DocumentSeries.Container.DocumentSeriesAnnexedLocation.ProtBiblosDSDB, item.IdAnnexed).ChainId
    End Sub

    Public Function GetUnpublishedAnnexedDocuments(item As DocumentSeriesItem, Optional sortResult As Boolean = False) As List(Of BiblosDocumentInfo)
        If item.LocationUnpublishedAnnexed Is Nothing Then
            Return New List(Of BiblosDocumentInfo)()
        End If
        Dim unpublishedAnnexed As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(item.IdUnpublishedAnnexed)
        If sortResult Then
            unpublishedAnnexed = BiblosFacade.SortDocuments(unpublishedAnnexed)
        End If
        Return unpublishedAnnexed
    End Function

    Public Function GetUnpublishedAnnexedDocuments(item As DocumentSeriesItemDTO(Of BiblosDocumentInfo), Optional sortResult As Boolean = False) As List(Of BiblosDocumentInfo)
        Dim unpublishedAnnexed As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(item.IdUnpublishedAnnexed)
        If sortResult Then
            unpublishedAnnexed = BiblosFacade.SortDocuments(unpublishedAnnexed)
        End If
        Return unpublishedAnnexed
    End Function

    Public Sub AddUnpublishedAnnexed(item As DocumentSeriesItem, unpublishableAnnexed As DocumentInfo)
        item.IdUnpublishedAnnexed = unpublishableAnnexed.ArchiveInBiblos(item.DocumentSeries.Container.DocumentSeriesUnpublishedAnnexedLocation.ProtBiblosDSDB, item.IdUnpublishedAnnexed).ChainId

    End Sub

    Public Sub ChangeDocumentsAttributes(item As DocumentSeriesItem, location As Location, documents As IList(Of BiblosDocumentInfo))
        ChangeDocumentsAttributes(item, location, documents, New Dictionary(Of String, String))
    End Sub

    Public Sub ChangeDocumentsAttributes(item As DocumentSeriesItem, location As Location, documents As IList(Of BiblosDocumentInfo), chainAttributes As IDictionary(Of String, String))
        For Each document As BiblosDocumentInfo In documents
            Try
                Dim biblosDocument As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentInfo(document.DocumentId, Nothing, True).FirstOrDefault()
                If biblosDocument Is Nothing Then
                    FileLogger.Warn(LoggerName, String.Concat("Nessun documento trovato in Biblos con ID ", document.DocumentId))
                    Exit Sub
                End If

                Dim needUpdate As Boolean = Not document.Name.Eq(biblosDocument.Name) OrElse Not document.Attributes.SequenceEqual(biblosDocument.Attributes)
                If needUpdate Then
                    document.AddAttribute(BiblosFacade.FILENAME_ATTRIBUTE, document.Name)
                    If chainAttributes.Count > 0 Then
                        Dim toUpdateAttributes As Dictionary(Of String, String) = chainAttributes.Where(Function(x) Not x.Key.Eq(BiblosFacade.FILENAME_ATTRIBUTE) _
                                                                                                         OrElse Not x.Key.Eq(BiblosFacade.SIGNATURE_ATTRIBUTE) _
                                                                                                         OrElse Not x.Key.Eq(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE)) _
                                                                                                         .ToDictionary(Function(x) x.Key, Function(x) x.Value)
                        document.AddAttributes(toUpdateAttributes)
                    End If
                    Service.UpdateDocument(document, DocSuiteContext.Current.User.FullUserName)
                    Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Edit, $"Modificati attributi documento {document.Name} per la registrazione {item.Year:0000}/{item.Number:0000000}")
                End If
            Catch ex As Exception
                FileLogger.Error(LoggerName, ex.Message, ex)
                Throw ex
            End Try
        Next
    End Sub

    Public Function GetDocumentSeriesItem() As IList(Of Integer)
        Return _dao.GetDocumentSeriesItem()
    End Function

    ''' <summary> Recupera il DataSource del DocumentSeriesItemViewerLight. </summary>
    ''' <param name="item">DSI di riferimento.</param>
    Public Function GetDocumentSeriesItemViewerSource(item As DocumentSeriesItem, captionConfiguration As IDictionary(Of Model.Entities.DocumentUnits.ChainType, String), Optional sortResult As Boolean = False) As DocumentInfo
        ' Creo la struttura ad albero del visualizzatore.
        Dim source As New FolderInfo()
        source.ID = item.Id.ToString()
        source.Name = GetCaption(item)

        Dim mainDocumentCaption As String = String.Empty
        If (captionConfiguration.ContainsKey(Model.Entities.DocumentUnits.ChainType.MainChain)) Then
            mainDocumentCaption = captionConfiguration(Model.Entities.DocumentUnits.ChainType.MainChain)
        End If
        Dim mainDocuments As New FolderInfo() With {.Name = mainDocumentCaption}
        mainDocuments.AddChildren(Factory.DocumentSeriesItemFacade.GetMainDocuments(item, sortResult).Cast(Of DocumentInfo).ToList())

        Dim annexedDocumentCaption As String = String.Empty
        If (captionConfiguration.ContainsKey(Model.Entities.DocumentUnits.ChainType.AnnexedChain)) Then
            annexedDocumentCaption = captionConfiguration(Model.Entities.DocumentUnits.ChainType.AnnexedChain)
        End If
        Dim annexed As New FolderInfo() With {.Name = annexedDocumentCaption}
        annexed.AddChildren(Factory.DocumentSeriesItemFacade.GetAnnexedDocuments(item, sortResult).Cast(Of DocumentInfo).ToList())

        Dim unpublishedAnnexedDocumentCaption As String = String.Empty
        If (captionConfiguration.ContainsKey(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)) Then
            unpublishedAnnexedDocumentCaption = captionConfiguration(Model.Entities.DocumentUnits.ChainType.UnpublishedAnnexedChain)
        End If
        Dim notPub As New FolderInfo() With {.Name = unpublishedAnnexedDocumentCaption}
        notPub.AddChildren(Factory.DocumentSeriesItemFacade.GetUnpublishedAnnexedDocuments(item, sortResult).Cast(Of DocumentInfo).ToList())

        If mainDocuments.Children.Count > 0 Then
            source.AddChild(mainDocuments)
        End If
        If annexed.Children.Count > 0 Then
            source.AddChild(annexed)
        End If
        If notPub.Children.Count > 0 Then
            source.AddChild(notPub)
        End If
        Return source
    End Function

    Public Shared Function GetCaption(item As DocumentSeriesItem) As String
        Return String.Format("{3} {0}/{1:0000000} del {2:dd/MM/yyyy}", item.Year, item.Number, item.RegistrationDate, DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName)
    End Function

    ''' <summary> Controlla il diritto richiesto. </summary>
    ''' <remarks> Implementazione guidata da altre preesistenti. </remarks>
    Function CheckRight(series As DocumentSeries, userGroups As String(), seriesRightsPosition As DocumentSeriesContainerRightPositions) As Boolean
        For Each cg As ContainerGroup In series.Container.ContainerGroups
            Dim hasRight As Boolean = cg.DocumentSeriesRights.Substring(seriesRightsPosition - 1, 1).Eq("1"c)
            ' TODO: gestire assenza flag
            For Each group As String In userGroups
                If group.Eq(cg.Name) AndAlso hasRight Then
                    Return True
                End If
            Next
        Next
        Return False
    End Function

    ''' <summary> Controlla il diritto richiesto nei security groups. </summary>
    ''' <remarks> Implementazione guidata da altre preesistenti. </remarks>
    Function CheckSecurityGroupsRight(series As DocumentSeries, securityGroups As IList(Of SecurityGroups), seriesRightsPosition As DocumentSeriesContainerRightPositions) As Boolean
        If series.Container.ContainerGroups.IsNullOrEmpty() Then
            Return False
        End If

        For Each cg As ContainerGroup In series.Container.ContainerGroups.Where(Function(f) f.SecurityGroup IsNot Nothing AndAlso Not String.IsNullOrEmpty(f.DocumentSeriesRights))
            Dim hasRight As Boolean = cg.DocumentSeriesRights.Substring(seriesRightsPosition - 1, 1).Eq("1"c)
            If (hasRight AndAlso securityGroups.Any(Function(f) f.Id = cg.SecurityGroup.Id)) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary> Elenco degli attributi della SerieDocumentale. </summary>
    ''' <remarks>
    ''' TODO: sostituire chiamata con recupero attributi da Capo Catena
    ''' </remarks>
    Function GetAttributes(item As DocumentSeriesItem) As Dictionary(Of String, String)
        Return GetMainChainInfo(item).Attributes
    End Function

    Function GetByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeriesItem)
        Return _dao.GetByIdentifiers(identifiers)
    End Function

    ''' <summary> Restituisce l'elenco delle DocumentSeriesItem non in stato Draft </summary>
    ''' <param name="identifiers">Array di identificatori delle documentSeriesItem</param>
    Function GetItemNotDraftByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeriesItem)
        Return _dao.GetItemNotDraftByIdentifiers(identifiers)
    End Function

    ''' <summary> Restituisce l'elenco delle DocumentSeriesItem in stato Draft </summary>
    ''' <param name="identifiers">Array di identificatori delle documentSeriesItem</param>
    Function GetItemDraftByIdentifiers(identifiers As List(Of Integer)) As IList(Of DocumentSeriesItem)
        Return _dao.GetItemDraftByIdentifiers(identifiers)
    End Function

    ''' <summary> Valorizza il campo Data pubblicazione a TODAY e registra l'evento nel Log. </summary>
    ''' <param name="item">DocumentSeriesItem da pubblicare</param>
    Public Sub Publish(item As DocumentSeriesItem)
        Publish(item, DateTime.Today)
    End Sub

    ''' <summary> Valorizza il campo Data pubblicazione e registra l'evento nel Log. </summary>
    ''' <param name="item">DocumentSeriesItem da pubblicare</param>
    ''' <param name="publishDate">Date di Pubblicazione</param>
    Public Sub Publish(item As DocumentSeriesItem, publishDate As DateTime)
        item.PublishingDate = publishDate
        If item.PublishingDate.HasValue AndAlso DocSuiteContext.Current.ProtocolEnv.SeriesRetireAuto > 0 Then
            Dim newYear As Integer = item.PublishingDate.Value.AddYears(DocSuiteContext.Current.ProtocolEnv.SeriesRetireAuto + 1).Year
            item.RetireDate = New Date(newYear, 1, 1)
        End If
        Update(item)
        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Publish, $"Pubblicata registrazione {item.Year:0000}/{item.Number:0000000} in data {item.PublishingDate:dd/MM/yyyy}")
    End Sub


    ''' <summary> Valorizza il campo Data ritiro Pubblicazione a TODAY e registra l'evento nel Log </summary>
    ''' <param name="item">DocumentSeriesItem da ritirare</param>
    Public Sub Retire(item As DocumentSeriesItem)
        Retire(item, DateTime.Today)
    End Sub

    ''' <summary> Valorizza il campo Data ritiro Pubblicazione e registra l'evento nel Log </summary>
    ''' <param name="item">DocumentSeriesItem da ritirare</param>
    ''' <param name="retireDate">Date di Ritiro Pubblicazione</param>
    Public Sub Retire(item As DocumentSeriesItem, retireDate As DateTime)
        item.RetireDate = retireDate
        Update(item)
        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Retire, $"Ritirata registrazione {item.Year:0000}/{item.Number:0000000} in data {item.RetireDate:dd/MM/yyyy}")
    End Sub

    Public Sub Cancel(item As DocumentSeriesItem, description As String)
        Select Case item.Status
            Case DocumentSeriesItemStatus.Draft
                item.Status = DocumentSeriesItemStatus.NotActive
            Case Else
                item.Status = DocumentSeriesItemStatus.Canceled
        End Select

        Try
            'Eseguo detach dei documenti se presenti
            Dim mainChain As BiblosDocumentInfo = BiblosDocumentInfo.GetDocuments(item.IdMain, False).FirstOrDefault()
            If mainChain IsNot Nothing Then
                Service.DetachDocument(item.Location.ProtBiblosDSDB, mainChain.BiblosChainId)
            End If
            item.HasMainDocument = False
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in fase Detach del Documento, impossibile eseguire.", ex)
            Throw New DocSuiteException("Errore annullamento", "Errore in fase Detach del Documento, impossibile eseguire.", ex)
        End Try

        Update(item)
        Factory.DocumentSeriesItemLogFacade.AddLog(item, DocumentSeriesItemLogType.Cancel, $"Annullata registrazione {item.Year:0000}/{item.Number:0000000}")
    End Sub

    ''' <summary> Valida la consistenza del <see cref="DocumentSeriesItem"/>  </summary>
    ''' <remarks> Ho come l'impressione che non sia il posto giusto per questa verifica, forse a livello più basso? </remarks>
    Private Shared Sub Validate(item As DocumentSeriesItem)
        If item.RetireDate.HasValue AndAlso item.PublishingDate.HasValue AndAlso item.PublishingDate.Value > item.RetireDate.Value Then
            Throw New DocSuiteException("Errore salvatagggio", "Data ritiro deve essere inferiore a Data pubblicazione")
        End If
    End Sub

    ''' <summary>
    ''' Invia un nuovo comando di inserimento alle web api
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns>Command ID</returns>
    Public Function SendInsertDocumentSeriesItemCommand(item As DocumentSeriesItem) As Guid
        Try
            Dim commandInsert As ICommandCreateDocumentSeriesItem = GetDocumentSeriesItemCommand(Of ICommandCreateDocumentSeriesItem)(item, Function(tenantName, tenantId, tenantAOOId, attributes, identity, apiDocumentSeries, apiCategoryFascicle, documentUnit)
                                                                                                                                                Return New CommandCreateDocumentSeriesItem(tenantName, tenantId, tenantAOOId, attributes, identity, apiDocumentSeries, apiCategoryFascicle, documentUnit)
                                                                                                                                            End Function)
            CommandInsertFacade.Push(commandInsert)
            Return commandInsert.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendInsertDocumentSeriesItemCommand => ", ex.Message), ex)
        End Try
    End Function

    ''' <summary>
    ''' Invia un nuovo comando di modifica alle web api
    ''' </summary>
    ''' <param name="item"></param>
    ''' <returns>Command ID</returns>
    Public Function SendUpdateDocumentSeriesItemCommand(item As DocumentSeriesItem) As Guid
        Try
            Dim commandUpdate As ICommandUpdateDocumentSeriesItem = GetDocumentSeriesItemCommand(Of ICommandUpdateDocumentSeriesItem)(item, Function(tenantName, tenantId, tenantAOOId, attributes, identity, apiDocumentSeries, apiCategoryFascicle, documentUnit)
                                                                                                                                                Return New CommandUpdateDocumentSeriesItem(tenantName, tenantId, tenantAOOId, attributes, identity, apiDocumentSeries, apiCategoryFascicle, documentUnit)
                                                                                                                                            End Function)
            CommandUpdateFacade.Push(commandUpdate)
            Return commandUpdate.Id
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendUpdateDocumentSeriesItemCommand => ", ex.Message), ex)
        End Try
    End Function

    Private Function GetDocumentSeriesItemCommand(Of T As {ICommand})(item As DocumentSeriesItem,
                                                                      commandInitializeFunc As Func(Of String, Guid, Guid, IDictionary(Of String, String), IdentityContext, APISeriesItem.DocumentSeriesItem, APICommons.CategoryFascicle, APIDocUnit.DocumentUnit, T)) As T
        Dim mapper As MapperDocumentSeriesItemEntity = New MapperDocumentSeriesItemEntity()
        Dim apiDocumentSeries As APISeriesItem.DocumentSeriesItem = mapper.MappingDTO(item)
        Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
        Dim tenantName As String = CurrentTenant.TenantName
        Dim tenantId As Guid = CurrentTenant.UniqueId
        Dim tenantAOOId As Guid = CurrentTenant.TenantAOO.UniqueId
        Dim attributes As IDictionary(Of String, String) = New Dictionary(Of String, String)
        Dim path As String = String.Empty

        Dim dswCategoryFascicle As CategoryFascicle = CategoryFascicleDao.GetByIdCategory(item.Category.Id, DSWEnvironment.DocumentSeries)
        Dim apiCategoryFascicle As APICommons.CategoryFascicle = Nothing
        If dswCategoryFascicle IsNot Nothing Then
            apiCategoryFascicle = MapperCategoryFascicle.MappingDTO(dswCategoryFascicle)
        End If

        If item.DocumentSeries IsNot Nothing AndAlso item.DocumentSeries.Container IsNot Nothing AndAlso item.DocumentSeries.Container.Archive IsNot Nothing Then
            path = String.Concat(item.DocumentSeries.Container.Archive.Name)
            attributes.Add("trasparenza_container_archive", item.DocumentSeries.Container.Archive.Name)
        End If

        If item.DocumentSeries IsNot Nothing AndAlso item.DocumentSeries.Family IsNot Nothing Then
            path = String.Concat(path, If(String.IsNullOrEmpty(path), String.Empty, "|"), item.DocumentSeries.Family.Name)
        End If

        If item.DocumentSeriesSubsection IsNot Nothing Then
            path = String.Concat(path, If(String.IsNullOrEmpty(path), String.Empty, "|"), item.DocumentSeriesSubsection.Description)
        End If

        If item.DocumentSeries IsNot Nothing Then
            path = String.Concat(path, If(String.IsNullOrEmpty(path), String.Empty, "|"), item.DocumentSeries.Name)
        End If

        attributes.Add("trasparenza_path", path)
        Return commandInitializeFunc(tenantName, tenantId, tenantAOOId, attributes, identity, apiDocumentSeries, apiCategoryFascicle, Nothing)
    End Function

    Public Function GetDocumentSeriesItemsNotDraft(prot As Protocol) As IList(Of DocumentSeriesItem)
        Return _dao.GetItemNotDraftByProtocol(prot.Year, prot.Number)
    End Function
#End Region
End Class
