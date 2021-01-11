Imports System.IO
Imports System.Xml
Imports System.Linq
Imports System.Text
Imports System.Collections.ObjectModel
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.WebPublication
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Core.Command
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions
Imports VecompSoftware.Core.Command.CQRS.Commands.Models.Resolutions
Imports VecompSoftware.Services.Command.CQRS.Commands.Models.Resolutions
Imports System.Web.Hosting
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports APICommons = VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Commons
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits

Public Class ResolutionFacade
    Inherits BaseResolutionFacade(Of Resolution, Integer, NHibernateResolutionDao)

    Public Enum DocType
        Proposta = 1
        Adozione = 2
        Allegati = 3
        Frontespizio = 4
        Disposizione = 5
        OrganoControllo = 6
        UltimaPagina = 7
        AllegatiRiservati = 8
        FrontespizioRitiro = 9
        Annessi = 10
        PrivacyPublicationDocument = 11
        DocumentoPrincipaleOmissis = 12
        AllegatiOmissis = 13
    End Enum

#Region " Fields "

    Private _customWorkflowXml As XmlDocument
    Private _mapperResolutionModel As MapperResolutionModel
    Private _commandCreateFacade As CommandFacade(Of ICommandCreateResolution)
    Private _commandUpdateFacade As CommandFacade(Of ICommandUpdateResolution)
    Private _fullPrintPath As String
    Private _categoryFascicleDao As CategoryFascicleDao
    Private _mapperCategoryFascicle As MapperCategoryFascicle
    Private _documentUnitFinder As DocumentUnitFinder
    Public Const PathStampeTo As String = "Resl\Stampe\AUSL-TO\"
    Public Const PrintPath As String = "~/Resl/Stampe/"

#End Region

#Region " Properties "

    Private ReadOnly Property CustomWorkflowXml As XmlDocument
        Get
            ' "<?xml version="1.0"?><CustomWorkFlowXml><AVVENUTAPUBBLICAZIONE><![CDATA[ *Avvenuta Pubblicazione ]]></AVVENUTAPUBBLICAZIONE><ESECUTIVA><![CDATA[ *Esecutiva ]]></ESECUTIVA><RISPOSTAOC><![CDATA[ *Risposta OC ]]></RISPOSTAOC><SCADENZAOC><![CDATA[ *Scadenza OC ]]></SCADENZAOC><RICEZIONE><![CDATA[ *Ricezione ]]></RICEZIONE><SPEDIZIONE><![CDATA[ *Spedizione ]]></SPEDIZIONE><PUBBLICATA><![CDATA[ *Pubblicata ]]></PUBBLICATA><ADOTTATA><![CDATA[ *Adottata ]]></ADOTTATA><SPEDCS><![CDATA[ *Sped. CS ]]></SPEDCS><PROPOSTA><![CDATA[ *Proposta ]]></PROPOSTA></CustomWorkFlowXml>"
            If _customWorkflowXml Is Nothing AndAlso Not String.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.CustomWorkflowXml) Then
                _customWorkflowXml = New XmlDocument
                _customWorkflowXml.LoadXml(DocSuiteContext.Current.ResolutionEnv.CustomWorkflowXml)
            End If
            Return _customWorkflowXml
        End Get
    End Property

    Private ReadOnly Property MapperResolutionModel As MapperResolutionModel
        Get
            If _mapperResolutionModel Is Nothing Then
                _mapperResolutionModel = New MapperResolutionModel()
            End If
            Return _mapperResolutionModel
        End Get
    End Property

    Private ReadOnly Property CommandCreateFacade As CommandFacade(Of ICommandCreateResolution)
        Get
            If _commandCreateFacade Is Nothing Then
                _commandCreateFacade = New CommandFacade(Of ICommandCreateResolution)
            End If
            Return _commandCreateFacade
        End Get
    End Property

    Private ReadOnly Property CommandUpdateFacade As CommandFacade(Of ICommandUpdateResolution)
        Get
            If _commandUpdateFacade Is Nothing Then
                _commandUpdateFacade = New CommandFacade(Of ICommandUpdateResolution)
            End If
            Return _commandUpdateFacade
        End Get
    End Property

    Public ReadOnly Property FullPrintPath As String
        Get
            If String.IsNullOrEmpty(_fullPrintPath) Then
                Dim fullPath As String = String.Concat(PrintPath, DocSuiteContext.Current.ResolutionEnv.PrintTemplatePath)
                _fullPrintPath = HostingEnvironment.MapPath(fullPath)
            End If
            Return _fullPrintPath
        End Get
    End Property

    Public ReadOnly Property CategoryFascicleDao As CategoryFascicleDao
        Get
            If _categoryFascicleDao Is Nothing Then
                _categoryFascicleDao = New CategoryFascicleDao(ProtDB)
            End If
            Return _categoryFascicleDao
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

    Private ReadOnly Property CurrentDocumentUnitFinder As DocumentUnitFinder
        Get
            If _documentUnitFinder Is Nothing Then
                _documentUnitFinder = New DocumentUnitFinder(DocSuiteContext.Current.CurrentTenant)
            End If
            Return _documentUnitFinder
        End Get
    End Property
#End Region

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "
    Public Overloads Sub Update(ByRef obj As Resolution)
        obj.LastChangedDate = DateTimeOffset.UtcNow
        obj.LastChangedUser = DocSuiteContext.Current.User.FullUserName
        MyBase.Update(obj)
    End Sub

    Public Shared Sub ComposeDocument(ByVal changeFields As String, ByVal fileRes As FileResolution, ByRef idCatena As Integer, ByRef loc As Location, ByRef signature As String)
        Dim s As String = Mid(changeFields, InStr(UCase(changeFields), UCase("ComposeDoc")))
        s = Mid(s, InStr(s, "[") + 1)
        s = Mid(s, 1, InStr(s, "]") - 1)

        Dim sFile() As String = Split(s, "+")
        Dim idCatenaTo As Integer = CType(ReflectionHelper.GetPropertyCase(fileRes, sFile(0)), Integer)

        Dim originalSignature As String = signature
        Dim i As Integer
        For i = 1 To sFile.Length - 1
            Dim idCatenaFrom As Integer = CType(ReflectionHelper.GetPropertyCase(fileRes, sFile(i)), Integer)
            If idCatenaFrom = 0 OrElse idCatenaTo = 0 Then
                Continue For
            End If

            Dim location As New UIDLocation(loc.ReslBiblosDSDB)
            Dim [from] As New UIDChain(location, idCatenaFrom)
            Dim [to] As New UIDChain(location, idCatenaTo)
            If sFile(i) = "idAttachements" Then
                signature &= " (Allegato)"
            End If
            Service.CopyDocuments([from], [to], signature)

            signature = originalSignature


        Next i

        FacadeFactory.Instance.ResolutionLogFacade.Log(fileRes.Resolution, ResolutionLogType.RP, String.Format("ATTI.COMPOSE: Catena [{0}] composta correttamente nella [{1}].", idCatena, idCatenaTo))
        idCatena = idCatenaTo
    End Sub

    Public Shared Function GetNumberOrServiceNumber(year As Short?, serviceNumber As String) As String
        Dim sNum As String = "*"
        If year.HasValue Then
            sNum = year.Value & "/"
        Else
            Return sNum
        End If

        If Not String.IsNullOrEmpty(serviceNumber) Then
            sNum &= serviceNumber
        Else
            sNum &= "*"
        End If

        Return sNum
    End Function
    ''' <summary> Restituisce il numero dell'atto  in formato esteso se adottato o il numero di servizio </summary>
    Public Shared Function GetNumberOrServiceNumber(ByVal resolution As Resolution) As String
        Return GetNumberOrServiceNumber(resolution.Year, resolution.ServiceNumber)
    End Function
    ''' <summary> Restituisce il numero dell'atto  in formato esteso se adottato o il numero di servizio </summary>
    Public Shared Function GetNumberOrServiceNumber(ByVal resolution As ResolutionHeader) As String
        Return GetNumberOrServiceNumber(resolution.Year, resolution.ServiceNumber)
    End Function

    Public Shared Function ReslFull(ByVal number As Integer) As String
        Return String.Format("{0:0000000}", number)
    End Function

    Public Function ComposeReslNumber(year As Short?, number As Integer?, serviceNumber As String, id As Integer, type As ResolutionType, adoptionDate As DateTime?, publishingDate As DateTime?, complete As Boolean, Optional tenantName As String = "") As String

        Dim b As Boolean = False
        If IsManagedProperty("idResolution", type.Id) Then
            'Visualizzare
            b = True
            If IsManagedProperty("idResolution", type.Id, "VR-NV") Then
                b = False
            End If
            If IsManagedProperty("idResolution", type.Id, "NA") Then
                'Visualizzare se non adottata
                b = (Not adoptionDate.HasValue)
            End If
            If IsManagedProperty("idResolution", type.Id, "NP") Then
                'Visualizzare se non pubblicata
                b = (Not publishingDate.HasValue)
            End If
        End If

        Dim sNum As String = String.Empty
        If b Then
            sNum = String.Format("{0:0000000}", id)
        Else
            If year.HasValue Then
                sNum &= year.Value & "/"
            End If

            If IsManagedProperty("ServiceNumber", type.Id) Then
                If Not String.IsNullOrEmpty(serviceNumber) Then
                    sNum &= serviceNumber
                Else
                    sNum &= "*"
                End If
            ElseIf IsManagedProperty("Number", type.Id) Then
                If number.HasValue Then
                    sNum &= String.Format("{0:0000000}", number.Value)
                Else
                    sNum &= "*"
                End If
            End If
        End If

        If Not complete Then
            Return sNum
        End If

        Return String.Format("{0} {1} del {2}", Factory.ResolutionTypeFacade.GetDescription(type), sNum, adoptionDate.DefaultString())
    End Function

    Public Function GetResolutionNumber(resolution As Resolution, complete As Boolean) As String
        Dim managedData As String = Factory.TabMasterFacade.GetFieldValue(TabMasterFacade.ManagedDataField, DocSuiteContext.Current.ResolutionEnv.Configuration, resolution.Type.Id)
        Return GetResolutionNumber(resolution, managedData, complete)
    End Function

    Public Function GetResolutionNumber(ByVal header As ResolutionHeader, ByVal complete As Boolean) As String
        Dim resolution As Resolution = Factory.ResolutionFacade.GetById(header.Id)
        Return GetResolutionNumber(resolution, header.ManagedData, complete)
    End Function

    Public Function GetResolutionNumber(ByVal resolution As Resolution, managedData As String, ByVal complete As Boolean) As String
        Dim retval As String = ElaborateResolutionNumber(resolution, managedData)

        ' Bello mescolare la logica di formattazione di un prograssivo con quella di un'etichetta... - FG
        If Not complete Then
            Return retval
        End If

        Select Case resolution.Type.Id
            Case ResolutionType.IdentifierDelibera
                Return Factory.ResolutionTypeFacade.DeliberaCaption
            Case ResolutionType.IdentifierDetermina
                Return String.Format("{0} {1} del {2}", Factory.ResolutionTypeFacade.DeterminaCaption, retval, resolution.AdoptionDate.DefaultString())
            Case Else
                Throw New NotImplementedException("ResolutionType sconosciuto.")
        End Select
    End Function

    Private Function ElaborateResolutionNumber(resolution As Resolution, managedData As String) As String
        If DisplayIdResolution(resolution, managedData) Then
            Return String.Format("{0:0000000}", resolution.Id)
        End If

        Dim retval As String = ""
        If resolution.Year.HasValue Then
            retval = String.Concat(resolution.Year.ToString(), "/")
        End If

        If IsManagedPropertyByString(managedData, "ServiceNumber") Then
            If Not String.IsNullOrEmpty(resolution.ServiceNumber) Then
                Return String.Concat(retval, resolution.ServiceNumber)
            End If
            Return String.Concat(retval, "*")
        End If

        If IsManagedPropertyByString(managedData, "Number") Then
            If resolution.Number.HasValue Then
                If Not DocSuiteContext.Current.ResolutionEnv.InclusiveNumberWithProposerCodeEnabled Then
                    Return String.Format("{0}{1:0000000}", retval, resolution.Number)
                Else
                    Dim proposerCode As String = String.Empty
                    If resolution.ResolutionContactProposers IsNot Nothing AndAlso resolution.ResolutionContactProposers.Count > 0 Then
                        proposerCode = resolution.ResolutionContactProposers(0).Contact.Code
                    End If
                    Return String.Format("{0:0000000}/{1}/{2}", resolution.Number, proposerCode, resolution.Year)
                End If
            End If
            Return String.Concat(retval, "*")
        End If

        Return retval
    End Function

    Public Function ElaborateInclusiveNumber(year As Short, number As String, managedData As String) As String
        If String.IsNullOrEmpty(managedData) Then
            Return String.Empty
        End If

        Dim serviceNumber As String = String.Empty

        If IsManagedPropertyByString(managedData, "ServiceNumber") Then
            Dim items As String() = number.Split("/"c)
            If items.Count > 1 Then
                serviceNumber = String.Format("{0}/{1}", items(0), items(1).PadLeft(4, "0"c))
            Else
                serviceNumber = number.ToString().PadLeft(4, "0"c)
            End If
        End If

        If IsManagedPropertyByString(managedData, "Number") Then
            If Not DocSuiteContext.Current.ResolutionEnv.InclusiveNumberWithProposerCodeEnabled Then
                serviceNumber = String.Format("{0:0000000}", Convert.ToInt32(number))
            Else
                'solo nel caso di Città di Torino (cioè quando il parametro InclusiveNumberWithProposerCodeEnabled true) ritorno il numero 
                Return String.Format("{0:0000000}/", Convert.ToInt32(number))
            End If
        End If

        Return String.Format("{0}/{1}", year, serviceNumber)

    End Function

    Private Function DisplayIdResolution(resolution As Resolution, managedData As String) As Boolean
        If Not IsManagedPropertyByString(managedData, "idResolution") Then
            Return False
        End If

        Dim retval As Boolean = Not IsManagedPropertyByString(managedData, "idResolution", "VR-NV")
        If IsManagedPropertyByString(managedData, "idResolution", "NA") Then
            retval = Not resolution.AdoptionDate.HasValue ' Visualizzare se non adottata
        End If
        If IsManagedPropertyByString(managedData, "idResolution", "NP") Then
            retval = Not resolution.PublishingDate.HasValue ' Visualizzare se non pubblicata
        End If

        Return retval
    End Function

    Public Function GetResolutionLabel(resl As Resolution) As String
        Return Factory.ResolutionTypeFacade.GetDescription(resl.Type)
    End Function

    Public Function GetResolutionNumber(resl As Resolution) As String
        ' TODO: Da rivedere pesantemente tutti questi IF CLIENTE
        Dim fullReslNumber As String = String.Empty

        If IsManagedProperty("ServiceNumber", resl.Type.Id) Then
            fullReslNumber = String.Format("{0}/{1}", resl.Year.ToString(), resl.ServiceNumber)
        End If

        If Not IsManagedProperty("Number", resl.Type.Id) Then
            Return fullReslNumber
        End If

        If Not DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
            Return String.Concat(resl.Year.ToString(), "/", resl.NumberFormat("{0:0000000}"))
        End If

        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
                proposerCode = resl.ResolutionContactProposers(0).Contact.Code
            End If
            fullReslNumber = String.Format("{0:0000000}/{1}/{2}", resl.Number, proposerCode, resl.Year)
        End If

        If fullReslNumber = "/" Then
            Return String.Empty
        End If
        Return fullReslNumber
    End Function

    Public Function SqlResolutionGetNumber(ByVal idResolution As Integer, Optional ByVal year As String = "", Optional ByVal number As String = "", Optional ByVal data As String = "", Optional ByVal complete As Boolean = False, Optional currentResolution As Resolution = Nothing) As String
        Dim resl As Resolution

        If currentResolution Is Nothing Then
            resl = GetById(idResolution)
            If resl Is Nothing Then
                Return String.Empty
            End If
        Else
            resl = currentResolution
        End If

        Dim b As Boolean = False

        If IsManagedProperty("idResolution", resl.Type.Id) Then
            b = True
            If IsManagedProperty("idResolution", resl.Type.Id, "VR-NV") Then
                b = False
            End If
            If IsManagedProperty("idResolution", resl.Type.Id, "NA") Then
                b = Not resl.AdoptionDate.HasValue
            End If
            If IsManagedProperty("idResolution", resl.Type.Id, "NP") Then
                b = Not resl.PublishingDate.HasValue
            End If
        End If

        Dim sNum As String = "*"
        If b Then
            sNum = idResolution.ToString()
        Else

            If Not String.IsNullOrEmpty(year) Then
                sNum = year & "/"
            ElseIf resl.Year.HasValue Then
                sNum = resl.Year.Value & "/"
            Else
                Return sNum
            End If

            If Not String.IsNullOrEmpty(number) Then
                sNum &= number
            ElseIf IsManagedProperty("ServiceNumber", resl.Type.Id) Then
                If Not String.IsNullOrEmpty(resl.ServiceNumber) Then
                    sNum &= resl.ServiceNumber
                Else
                    sNum &= "*"
                End If
            ElseIf IsManagedProperty("Number", resl.Type.Id) Then
                If resl.Number.HasValue Then
                    If (DocSuiteContext.Current.IsResolutionEnabled AndAlso DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO")) Then
                        ReslFullNumber(resl, resl.Type.Id, "0", sNum, False)
                    Else
                        sNum &= Format(CLng(resl.Number), "0000000")
                    End If
                Else
                    sNum &= "*"
                End If
            End If
        End If
        If Not complete Then
            Return sNum
        End If

        '' Provo ad impostare il valore che mi arriva da parametro (AdoptionDate potrebbe non essere ancora stato valorizzato)
        Dim dataAdozione As String = data
        If String.IsNullOrEmpty(dataAdozione) Then
            '' Se non c'era valore allora utilizzo il valore già memorizzato come AdoptionDate
            dataAdozione = resl.AdoptionDate.DefaultString()
        End If
        Return String.Format("{0} {1} del {2}", Factory.ResolutionTypeFacade.GetDescription(resl.Type), sNum, dataAdozione)
    End Function

    Public Overridable Function ReslFullNumber(ByVal resolution As Resolution, ByVal type As Short, ByRef fullReslId As String, ByRef fullReslNumber As String, Optional ByVal bold As Boolean = False) As Boolean
        'Calcola l'ID dell'atto in formato esteso
        fullReslId = CalculateFullId(resolution, type)

        'Calcola il NUMERO dell'atto in formato esteso
        fullReslNumber = CalculateFullNumber(resolution, type, bold)

        'Normalizza il numero dell'atto
        If fullReslNumber = "/" Then
            fullReslNumber = ""
        End If
    End Function

    ''' <summary> Calcola l'ID dell'atto in formato esteso </summary>
    ''' <param name="resolution">Oggetto che identifica l'Atto</param>
    ''' <param name="type">Tipo di atto (ReslType)</param>
    ''' <returns>ID dell'atto in formato esteso</returns>
    Protected Overridable Function CalculateFullId(ByVal resolution As Resolution, ByVal type As Short) As String
        Dim b As Boolean = False
        If IsManagedProperty("idResolution", type) Then
            'Visualizzare
            If IsManagedProperty("idResolution", type, "NA") Then
                'Visualizzare se non adottata
                b = (Not resolution.AdoptionDate.HasValue)
            End If
            If IsManagedProperty("idResolution", type, "NP") Then
                'Visualizzare se non pubblicata
                b = (Not resolution.PublishingDate.HasValue)
            End If
            If IsManagedProperty("idResolution", type, "VI") Then
                'Visualizzare sempre
                b = True
            End If
            If b Then
                Return resolution.IdFull
            End If
        End If
        Return String.Empty
    End Function

    ''' <summary> Calcola il NUMERO dell'atto in formato esteso. </summary>
    ''' <param name="resolution">Oggetto che identifica l'Atto</param>
    ''' <param name="type">Tipo di atto (ReslType)</param>
    ''' <param name="bold">Definisce se deve essere aggiunto il valore html <B></B></param>
    ''' <returns>NUMERO dell'atto in formato esteso</returns>
    Public Overridable Function CalculateFullNumber(ByVal resolution As Resolution, ByVal type As Short, ByVal bold As Boolean) As String
        Dim fullReslNumber As String = ""

        If IsManagedProperty("ServiceNumber", type) Then
            fullReslNumber = String.Format("{0}/{1}", resolution.Year.ToString(), resolution.ServiceNumber)
        End If

        If Not IsManagedProperty("Number", type) Then
            Return fullReslNumber
        End If

        If Not DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
            Return resolution.Year.ToString() & "/" & resolution.NumberFormat("{0:0000000}")
        End If

        If Not resolution.Number.HasValue Then
            Return fullReslNumber
        End If

        Dim proposerCode As String = String.Empty
        If resolution.ResolutionContactProposers IsNot Nothing AndAlso resolution.ResolutionContactProposers.Count > 0 Then
            proposerCode = resolution.ResolutionContactProposers(0).Contact.Code
        End If
        Return String.Format("{0}/{1}/{2}",
                             If(bold, String.Format("<B>{0}</B>", resolution.Number.Value), resolution.NumberFormat("{0:0000000}")),
                             proposerCode,
                             resolution.Year.ToString)

    End Function

    Public Function GetDocuments(resolution As Resolution, field As String, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim id As UIDChain = GetDocumentsUID(resolution, field)
        If id.Id > 0 Then
            Dim docs As BiblosDocumentInfo() = BiblosDocumentInfo.GetDocuments(id).OrderBy(Function(d) d.DateCreated).ToArray()
            If includeUniqueId Then
                For Each doc As BiblosDocumentInfo In docs
                    doc = SetProtocolUniqueIdAttribute(doc, resolution.UniqueId, DSWEnvironment.Resolution)
                Next
            End If
            Return docs
        Else
            Return New BiblosDocumentInfo() {}
        End If
    End Function

    Public Function GetDocuments(resolution As Resolution, incremental As Short, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        FileLogger.Debug(LoggerName, String.Format("GetDocuments({0}, {1})", resolution.Id, incremental))

        Dim id As UIDChain = GetDocumentsUID(resolution, incremental)
        If id.Id > 0 Then
            Dim docs As BiblosDocumentInfo() = BiblosDocumentInfo.GetDocuments(id).OrderBy(Function(d) d.DateCreated).ToArray()
            If includeUniqueId Then
                For Each doc As BiblosDocumentInfo In docs
                    doc = SetProtocolUniqueIdAttribute(doc, resolution.UniqueId, DSWEnvironment.Resolution)
                Next
            End If
            Return docs
        Else
            Return New BiblosDocumentInfo() {}
        End If
    End Function

    Public Function GetDocuments(resolution As Resolution) As BiblosDocumentInfo()
        Dim facade As New ResolutionWorkflowFacade()
        Return GetDocuments(resolution, facade.GetActiveIncremental(resolution.Id, 1))
    End Function

    Public Function GetPrivacyDocuments(resolution As Resolution) As BiblosDocumentInfo()
        Dim files As FileResolution = GetFileResolution(resolution)

        If files.IdPrivacyPublicationDocument.HasValue Then
            Dim biblosChain As UIDChain = New UIDChain(resolution.Location.ReslBiblosDSDB, files.IdPrivacyPublicationDocument.Value)
            Return BiblosDocumentInfo.GetDocuments(biblosChain).ToArray()
        End If

        '' Se non ci sono documenti ritorno vuoto
        Return New BiblosDocumentInfo() {}
    End Function

    Public Function HasDocuments(resolution As Resolution, checkAllFiles As Boolean) As Boolean
        Return HasDocuments(resolution, GetActiveStep(resolution).Id.Incremental, checkAllFiles)
    End Function

    Public Function HasDocuments(resolution As Resolution, incremental As Short, checkAllFiles As Boolean) As Boolean
        Dim files As FileResolution = GetFileResolution(resolution)
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)

        If String.IsNullOrEmpty(tab.FieldDocument) Then
            Return checkAllFiles AndAlso files.HasDocuments()
        End If

        Dim docId As Integer = CType(ReflectionHelper.GetPropertyCase(files, tab.FieldDocument), Integer)
        Return (docId > 0) OrElse (checkAllFiles AndAlso files.HasDocuments())
    End Function

    Public Function GetDocumentsOmissis(resolution As Resolution, incremental As Short, getFromStep As Boolean, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim id As Guid = GetDocumentsOmissisGuid(resolution, incremental, getFromStep)
        If Not id.Equals(Guid.Empty) Then
            Dim docs As BiblosDocumentInfo() = BiblosDocumentInfo.GetDocuments(id).ToArray()
            If includeUniqueId Then
                For Each doc As BiblosDocumentInfo In docs
                    doc = SetProtocolUniqueIdAttribute(doc, resolution.UniqueId, DSWEnvironment.Resolution)
                Next
            End If
            Return docs
        Else
            Return New BiblosDocumentInfo() {}
        End If
    End Function

    Public Function GetDocumentsOmissis(resolution As Resolution) As BiblosDocumentInfo()
        Dim facade As New ResolutionWorkflowFacade()
        Return GetDocumentsOmissis(resolution, facade.GetActiveIncremental(resolution.Id, 1), False)
    End Function

    Public Function GetAttachments(resolution As Resolution, incremental As Short, getFromStep As Boolean, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim id As UIDChain = GetAttachmentsUID(resolution, incremental, getFromStep)
        If id.Id > 0 Then
            Dim docs As BiblosDocumentInfo() = BiblosDocumentInfo.GetDocuments(id).ToArray()
            If includeUniqueId Then
                For Each doc As BiblosDocumentInfo In docs
                    doc = SetProtocolUniqueIdAttribute(doc, resolution.UniqueId, DSWEnvironment.Resolution)
                Next
            End If
            Return docs
        Else
            Return New BiblosDocumentInfo() {}
        End If
    End Function

    Public Function GetAttachments(resolution As Resolution) As BiblosDocumentInfo()
        Dim facade As New ResolutionWorkflowFacade()
        Return GetAttachments(resolution, facade.GetActiveIncremental(resolution.Id, 1), False)
    End Function

    Public Function GetAttachmentsOmissis(resolution As Resolution, incremental As Short, getFromStep As Boolean, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim id As Guid = GetAttachmentsOmissisGuid(resolution, incremental, getFromStep)
        If Not id.Equals(Guid.Empty) Then
            Dim docs As BiblosDocumentInfo() = BiblosDocumentInfo.GetDocuments(id).ToArray()
            If includeUniqueId Then
                For Each doc As BiblosDocumentInfo In docs
                    doc = SetProtocolUniqueIdAttribute(doc, resolution.UniqueId, DSWEnvironment.Resolution)
                Next
            End If
            Return docs
        Else
            Return New BiblosDocumentInfo() {}
        End If
    End Function

    Public Function GetAttachmentsOmissis(resolution As Resolution) As BiblosDocumentInfo()
        Dim facade As New ResolutionWorkflowFacade()
        Return GetAttachmentsOmissis(resolution, facade.GetActiveIncremental(resolution.Id, 1), False)
    End Function

    Public Function HasAttachment(resolution As Resolution) As Boolean
        Return HasAttachment(resolution, GetActiveStep(resolution).Id.Incremental)
    End Function

    Public Function HasAttachment(resolution As Resolution, incremental As Short) As Boolean
        Dim files As FileResolution = GetFileResolution(resolution)
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)

        If String.IsNullOrEmpty(tab.FieldAttachment) Then
            Return False
        End If

        Dim docId As Integer = Integer.Parse(ReflectionHelper.GetPropertyCase(files, tab.FieldAttachment).ToString())

        Return (docId > 0)
    End Function

    ''' <summary> Permette di ritornare una lista di DocumentInfo degli annessi da un rispettivo incrementale </summary>
    ''' <returns>array di BiblosDocumentInfo a rappresentare la catena di annessi</returns>
    Public Function GetAnnexes(ByVal resolution As Resolution, ByVal incremental As Short, Optional includeUniqueId As Boolean = False) As BiblosDocumentInfo()
        Dim guid As Guid = GetAnnexesGuid(resolution, incremental)
        If Not guid.Equals(Guid.Empty) Then
            Dim docs As BiblosDocumentInfo() = BiblosDocumentInfo.GetDocuments(guid).ToArray()
            If includeUniqueId Then
                For Each doc As BiblosDocumentInfo In docs
                    doc = SetProtocolUniqueIdAttribute(doc, resolution.UniqueId, DSWEnvironment.Resolution)
                Next
            End If
            Return docs
        Else
            Return New BiblosDocumentInfo() {}
        End If
    End Function

    Public Function GetDocumentsUID(resolution As Resolution, incremental As Short) As UIDChain
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)
        If (tab Is Nothing) Then
            Return New UIDChain(String.Empty, 0)
        End If
        Dim files As FileResolution = GetFileResolution(resolution)
        FileLogger.Debug(LoggerName, String.Format("GetDocumentsUID: tab.FieldDocument = {0}", tab.FieldDocument))

        Dim docId As Integer = CType(ReflectionHelper.GetPropertyCase(files, tab.FieldDocument), Integer)
        FileLogger.Debug(LoggerName, String.Format("GetDocumentsUID: docId = {0}", docId))

        Return New UIDChain(resolution.Location.ReslBiblosDSDB, docId)
    End Function

    Public Function GetDocumentsUID(resolution As Resolution, field As String) As UIDChain
        Dim files As FileResolution = GetFileResolution(resolution)
        Dim docId As Integer = Integer.Parse(ReflectionHelper.GetPropertyCase(files, field).ToString())
        Return New UIDChain(resolution.Location.ReslBiblosDSDB, docId)
    End Function

    Public Function GetDocumentsOmissisGuid(resolution As Resolution, incremental As Short, getFromStep As Boolean) As Guid
        '' Se voglio il valore dallo step allora lo leggo dalla resolutionWorkflow
        If getFromStep Then
            Return Factory.ResolutionWorkflowFacade.GetById(New ResolutionWorkflowCompositeKey() With {.IdResolution = resolution.Id, .Incremental = incremental}).DocumentsOmissis
        End If

        '' Altrimenti carico come prima dalla FileResolution
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)
        Dim files As FileResolution = GetFileResolution(resolution)

        If String.IsNullOrEmpty(tab.FieldDocumentsOmissis) Then
            Return files.IdMainDocumentsOmissis
        Else
            Return CType(ReflectionHelper.GetPropertyCase(files, tab.FieldDocumentsOmissis), Guid)
        End If
    End Function

    Public Function GetAttachmentsUID(resolution As Resolution, incremental As Short, getFromStep As Boolean) As UIDChain
        '' Se voglio il valore dallo step allora lo leggo dalla resolutionWorkflow
        If getFromStep Then
            'Volontariamente il valore nullo diventa 0, la catena con id 0 viene gestita dallo strato superiore
            Dim idChain As Integer = CType(Factory.ResolutionWorkflowFacade.GetById(New ResolutionWorkflowCompositeKey() With {.IdResolution = resolution.Id, .Incremental = incremental}).Attachment, Integer)
            Return New UIDChain(resolution.Location.ReslBiblosDSDB, idChain)
        End If

        '' Altrimenti carico come prima dalla FileResolution
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)
        Dim files As FileResolution = GetFileResolution(resolution)

        If String.IsNullOrEmpty(tab.FieldAttachment) Then
            Return New UIDChain(resolution.Location.ReslBiblosDSDB, 0)
        Else
            Dim docId As Integer = CType(ReflectionHelper.GetPropertyCase(files, tab.FieldAttachment), Integer)
            Return New UIDChain(resolution.Location.ReslBiblosDSDB, docId)
        End If
    End Function

    Public Function GetAttachmentsOmissisGuid(resolution As Resolution, incremental As Short, getFromStep As Boolean) As Guid
        '' Se voglio il valore dallo step allora lo leggo dalla resolutionWorkflow
        If getFromStep Then
            Return Factory.ResolutionWorkflowFacade.GetById(New ResolutionWorkflowCompositeKey() With {.IdResolution = resolution.Id, .Incremental = incremental}).AttachmentsOmissis
        End If

        '' Altrimenti carico come prima dalla FileResolution
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)
        Dim files As FileResolution = GetFileResolution(resolution)

        If String.IsNullOrEmpty(tab.FieldAttachmentsOmissis) Then
            Return files.IdAttachmentsOmissis
        Else
            Return CType(ReflectionHelper.GetPropertyCase(files, tab.FieldAttachmentsOmissis), Guid)
        End If
    End Function

    ''' <summary> Ritorna il guid della catena annessi data una resolution e il rispettivo incrementale </summary>
    Public Function GetAnnexesGuid(ByVal resolution As Resolution, ByVal incremental As Short) As Guid
        Dim tab As TabWorkflow = Factory.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)
        Dim files As FileResolution = GetFileResolution(resolution)

        If String.IsNullOrEmpty(tab.FieldAnnexed) Then
            Return files.IdAnnexes
        Else
            Return Guid.Parse(ReflectionHelper.GetPropertyCase(files, tab.FieldAnnexed).ToString())
        End If
    End Function

    Public Function GetActiveStep(resolution As Resolution) As ResolutionWorkflow
        Dim incremental As Short = Factory.ResolutionWorkflowFacade.GetActiveIncremental(resolution.Id, 1)
        Return GetStep(resolution, incremental)
    End Function

    Public Function GetStep(resolution As Resolution, incremental As Short) As ResolutionWorkflow
        Return Factory.ResolutionWorkflowFacade.GetById(New ResolutionWorkflowCompositeKey() With {.IdResolution = resolution.Id, .Incremental = incremental})
    End Function

    Public Function GetFileResolution(resolution As Resolution) As FileResolution
        Dim frLst As IList(Of FileResolution) = Factory.FileResolutionFacade.GetByResolution(resolution)
        If frLst.Count <> 1 Then
            Throw New DocSuiteException("Recupero Resolution", "Errore in recupero FileResolution per ResolutionId = " & resolution.Id)
        End If

        Return frLst(0)
    End Function

    Function GetResolutions(ByVal keyList As IList(Of Integer)) As IList(Of Resolution)
        Return _dao.GetResolutions(keyList)
    End Function

    Function GetResolutions(ByVal keyList As IList(Of Integer), ByVal orderfield As String, ByVal asc As Boolean) As IList(Of Resolution)
        Return _dao.GetResolutions(keyList, orderfield, asc)
    End Function

    Function GetResolutionsLetter(ByVal keyList As IList(Of Integer), Optional ByVal GroupBy As String = "", Optional ByVal GroupByAliases As String = "", Optional ByVal OrderByAsc As String = "", Optional ByVal OnlyGestione As Boolean = False) As IList(Of ResolutionLetter)
        Return _dao.GetResolutionsLetter(keyList, GroupBy, GroupByAliases, OrderByAsc, OnlyGestione)
    End Function

    'Il metodo controlla che il FieldName sia presente sia in TabMaster sia in TabWorkflow (changeableData); torna True solo se è presente in entrambe
    Public Function ManagedDataTest(ByRef resolution As Resolution, ByVal FieldName As String, Optional ByVal FieldProperty As String = "", Optional ByVal changeableData As String = "", Optional ByVal FieldTest As String = "") As Boolean
        Dim managedBool As Boolean = IsManagedProperty(FieldName, resolution.Type.Id, FieldProperty)
        If Not String.IsNullOrEmpty(changeableData) Then
            managedBool = managedBool And StringHelper.InStrTest(changeableData, FieldTest)
        End If

        Return managedBool
    End Function

    Public Function GetDocumentLinkedCount(ByVal p_idResolution As Integer) As Integer
        Dim retval As Integer = 0
        If DocSuiteContext.Current.IsDocumentEnabled Then
            retval = New DocumentObjectFacade().GetDocumentObjectLink("LR", p_idResolution.ToString() & "|").Count
        End If
        Return retval
    End Function

    ''' <summary> Verifica dei diritti su Resolution. </summary>
    ''' <param name="resolution">Istanza su cui verificare il diritto</param>
    ''' <param name="right">Posizione del bit all'interno della stringa di dei diritti da vericare</param>
    ''' <returns>true se si hanno diritti, false altrimenti</returns>
    Public Function CheckUserRights(ByVal resolution As Resolution, ByVal right As ResolutionRightPositions) As Boolean
        ' Verifica stati che invalidano qualsiasi richiesta
        If resolution.Status.Id = ResolutionStatusId.Errato OrElse resolution.Status.Id = ResolutionStatusId.Temporaneo Then
            Return False ' NESSUN DIRITTO SU QUESTO TIPO DI RESOLUTION
        End If

        ' Diritto base da Container
        Dim containerRight As Boolean = Factory.ContainerFacade.CheckContainerRight(resolution.Container.Id, DSWEnvironment.Resolution, right, Nothing)

        ' Verifica se il diritto viene aggiunto
        Dim roleAddedRight As Boolean = Factory.ResolutionRoleFacade.CheckRight(resolution, right, True)

        ' Se il diritto non è modificato da nessun ResolutionRole il valore restituito è NULL
        Dim roleRestrictedRight As Boolean? = Factory.ResolutionRoleFacade.CheckRestrictedRight(resolution, right, True)

        ' Se il diritto è stato in qualche modo ristretto solo ad alcuni settori
        If roleRestrictedRight.HasValue Then
            Return (containerRight AndAlso roleRestrictedRight.Value) OrElse roleAddedRight
        End If

        ' Diritto non modificato
        Return containerRight OrElse roleAddedRight

    End Function

    ''' <summary>
    ''' Verifica se esistono ruoli associati all'atto su cui l'utente ha diritti
    ''' </summary>
    ''' <param name="resolution">Atto su cui verificare la sicurezza</param>
    ''' <returns>True se l'utente ha diritti, false altrimenti</returns>
    Protected Function CheckUserRole(ByVal resolution As Resolution, ByVal posRights As Integer) As Boolean
        For Each reslRole As ResolutionRole In resolution.ResolutionRoles

            If reslRole.Role.IsActive <> 1 Then
                ' I Settori disabilitati non agiscono sui diritti
                Continue For
            End If

            If Factory.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Resolution, reslRole.Role) Then
                ' Verifico i diritti sulla base del ResolutionRoleType
                Return True
            End If
        Next

        Return False
    End Function

    Public Function SqlResolutionDeleteStep(ByVal idResolution As Integer, ByVal workflow As TabWorkflow) As Boolean
        Dim resl As Resolution = GetById(idResolution)

        Dim sManage As String = workflow.ManagedWorkflowData
        If TabWorkflowFacade.TestManagedWorkflowDataProperty(sManage, "Date", "DEL", "STEP") Then
            ReflectionHelper.SetProperty(resl, workflow.FieldDate, Nothing)
            ReflectionHelper.SetProperty(resl, workflow.FieldUser, Nothing)
        End If

        If TabWorkflowFacade.TestManagedWorkflowDataProperty(sManage, "ServiceNumber", "DEL", "STEP") Then
            resl.ServiceNumber = Nothing
        End If

        Save(resl)
        Dim b As Boolean = True

        If TabWorkflowFacade.TestManagedWorkflowDataProperty(sManage, "Attachment", "DEL", "STEP") AndAlso b Then
            b = SqlResolutionFileUpdate(idResolution, 0, workflow.FieldAttachment)
        End If
        If TabWorkflowFacade.TestManagedWorkflowDataProperty(sManage, "Annexed", "DEL", "STEP") AndAlso b Then
            b = SqlResolutionFileUpdate(idResolution, Guid.Empty, workflow.FieldAnnexed)
        End If
        If TabWorkflowFacade.TestManagedWorkflowDataProperty(sManage, "Document", "DEL", "STEP") AndAlso b Then
            b = SqlResolutionFileUpdate(idResolution, 0, workflow.FieldDocument)
        End If

        Factory.ResolutionLogFacade.Log(resl, ResolutionLogType.RU, "ATTI.STEP.DEL: Eliminato ultimo step")

        Return b
    End Function

    ''' <summary>
    ''' Aggiorna il <see>FileResolution</see> e opzionalmente la <see>Resolution</see> con le catene indicate.
    ''' </summary>
    ''' <param name="idResolution">Atto</param>
    ''' <param name="ViewReslType">Identificativo del tipo di atto</param>
    ''' <param name="NotSegnature">Se true assegna il numero all'atto</param>
    ''' <param name="workflow">Anagrafica dello step</param>
    ''' <param name="Data">Data di adozione in stringa, cosa strana con la N o stringa vuota</param>
    ''' <param name="ServiceNumber">Nuovo service number, se diverso da N o stringa vuota</param>
    ''' <param name="idAttachment">Catena degli allegati</param>
    ''' <param name="idDocument">Catena del documento</param>
    ''' <param name="idAnnexed">Catena annessi</param>
    ''' <param name="idODG">Non usato</param>
    ''' <param name="tosave">Indica se salvare la <see>Resolution</see> (altrimenti aggiorna solo il <see>FileResolution</see>)</param>
    ''' <returns>Torna vero se è riuscito ad aggiornare l'id catena di allegati e documenti</returns>
    Public Function SqlResolutionUpdateWorkflowData(ByVal idResolution As Integer,
                                                   ByVal viewReslType As Short,
                                                   ByVal notSegnature As Boolean,
                                                   ByVal workflow As TabWorkflow,
                                                   ByVal data As String,
                                                   ByVal serviceNumber As String,
                                                   ByVal idAttachment As Integer,
                                                   ByVal idDocument As Integer,
                                                   ByVal idAnnexed As Guid,
                                                   ByVal idOdg As String,
                                                   ByVal tosave As Boolean,
                                                   ByVal user As String) As Boolean

        Dim year As Short = 0
        Dim number As Integer = 0

        Dim resl As Resolution = GetById(idResolution)

        Dim b As Boolean ' FIGATA! (cit.)

        ' Aggiorno anno e numero dell'atto
        If TestOperationStepProperty(workflow.OperationStep, "S", resl) Then
            If notSegnature Then    'Se non ho già assegnato il numero

                Dim adoptionDate As DateTime = DateTime.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture)
                Dim yearAndNumber As Tuple(Of Short, Integer) = CalculateYearAndNumber(resl.ProposeDate.Value, adoptionDate, viewReslType)
                resl.Year = yearAndNumber.Item1
                resl.Number = yearAndNumber.Item2

                Dim parameterFacade As New ParameterFacade(ReslDB)
                Dim reslParameter As Parameter = parameterFacade.GetCurrentAndRefresh()
                If viewReslType = ResolutionType.IdentifierDelibera Then
                    parameterFacade.UpdateResolutionLastUsedNumber(reslParameter.LastUsedResolutionNumber + 1S)
                Else
                    parameterFacade.UpdateResolutionLastUsedBillNumber(reslParameter.LastUsedBillNumber + 1S)
                End If
            End If
        End If

        ' Aggiorno data e utente dell'atto
        If Not data.Eq("N") Then
            If Not String.IsNullOrEmpty(data) Then
                ReflectionHelper.SetPropertyCase(resl, workflow.FieldDate, DateTime.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture))
                ReflectionHelper.SetPropertyCase(resl, workflow.FieldUser, user)
            Else
                ReflectionHelper.SetPropertyCase(resl, workflow.FieldDate, Nothing)
                ReflectionHelper.SetPropertyCase(resl, workflow.FieldUser, Nothing)
            End If
        End If

        ' Aggiorno il ServiceNumber
        If Not serviceNumber.Eq("N") Then
            resl.ServiceNumber = If(Not String.IsNullOrEmpty(serviceNumber), serviceNumber, Nothing)
        End If

        ' Salvataggio dell'atto
        If tosave Then
            Save(resl)
        End If
        b = True

        ' Aggiorno allegato
        If idAttachment <> -1 Then
            If Not String.IsNullOrEmpty(workflow.FieldAttachment) Then
                If b Then
                    b = SqlResolutionFileUpdate(idResolution, idAttachment, workflow.FieldAttachment)
                End If
            End If
        End If
        ' Aggiorno annessi
        ' TODO: capire perchè Guid.Empty voglia dire "non aggiornare" sarebbe bello
        If idAnnexed.Equals(Guid.Empty) Then
            'todo: Chiedo perdono dell'uso improprio del parametro nella Facade... ma tutto questo metodo è improprio.
            If DocSuiteContext.Current.ResolutionEnv.EnableFlushAnnexed Then
                If Not String.IsNullOrEmpty(workflow.FieldAnnexed) Then
                    If b Then
                        b = SqlResolutionFileUpdate(idResolution, idAnnexed, workflow.FieldAnnexed)
                    End If
                End If
            End If
        Else
            If Not String.IsNullOrEmpty(workflow.FieldAnnexed) Then
                If b Then
                    b = SqlResolutionFileUpdate(idResolution, idAnnexed, workflow.FieldAnnexed)
                End If
            End If
        End If
        ' Aggiorno documenti
        If idDocument <> -1 Then
            If Not String.IsNullOrEmpty(workflow.FieldDocument) Then
                If b Then
                    b = SqlResolutionFileUpdate(idResolution, idDocument, workflow.FieldDocument)
                End If
            End If
        End If

        ' Aggiorno InclusiveNumber e LOG
        resl.InclusiveNumber = GetResolutionNumber(resl, False)

        ' Eseguo il LOG solo se InclusiveNumber è stato valorizzato correttamente e solo una volta
        If (resl.InclusiveNumber IsNot Nothing AndAlso Not resl.InclusiveNumber.Eq("*")) Then
            Factory.ResolutionLogFacade.Log(resl, ResolutionLogType.RD, String.Format("ATTI.NUM: Numerato atto con codice {0}.", resl.InclusiveNumber))
        End If

        Return b
    End Function

    Public Function CalculateYearAndNumber(proposeDate As Date, adoptionDate As DateTime, type As Short) As Tuple(Of Short, Integer)
        Dim year As Short = 0
        Dim number As Integer = 0

        Dim parameterFacade As New ParameterFacade(ReslDB)
        Dim reslParameter As Parameter = parameterFacade.GetCurrentAndRefresh()

        year = reslParameter.LastUsedResolutionYear

        ' Verifico che l'anno della data di proposta e l'anno della data di adozione siano antecedenti l'anno correntemente in gestione.
        If DocSuiteContext.Current.ResolutionEnv.PastYearsEnumerationEnabled AndAlso proposeDate.Year < year AndAlso adoptionDate.Year < year Then
            ' In tal caso uso l'anno della data di adozione come anno numeratore.
            year = adoptionDate.Year

            ' Per quanto riguarda il numero recupero l'ultimo inserito +1 per l'anno della data di adozione.
            ' ATTENZIONE! Il recupero del numero attraverso questo procedimento non è transazionale.
            number = _dao.GetMaxResolutionNumber(year, type) + 1
        Else
            ' Qualora il parametro PastYearsEnumerationEnabled in ParameterEnv di Atti sia disabilitato
            ' o mancante procedo con la numerazione come è sempre avvenuta.
            If type = ResolutionType.IdentifierDelibera Then
                number = reslParameter.LastUsedResolutionNumber
            Else
                number = reslParameter.LastUsedBillNumber
            End If
        End If

        Return New Tuple(Of Short, Integer)(year, number)
    End Function

    ''' <summary> Aggiorna il campo specificato tra i file dell'atto </summary>
    ''' <param name="idResolution">Identificativo atto</param>
    ''' <param name="idCatena">Identificativo catena, se Nothing, Guid.Empty o 0 viene cancellato</param>
    ''' <param name="fieldName">Nome del campo da aggiornare</param>
    Public Function SqlResolutionFileUpdate(ByVal idResolution As Integer, ByVal idCatena As Object, ByVal fieldName As String) As Boolean
        Dim fileResFacade As New FileResolutionFacade("ReslDB")

        Dim fr As FileResolution = fileResFacade.GetById(idResolution)
        If fr Is Nothing Then
            Return False
        End If

        If TypeOf idCatena Is Guid Then
            ReflectionHelper.SetPropertyCase(fr, fieldName, DirectCast(idCatena, Guid))
        Else
            ' Corretto l'uso improprio dei long, forzo tutto a compatibilità con integer
            Dim id As Integer? = Nothing
            Try
                id = CType(idCatena, Integer)
            Catch ex As InvalidCastException
                FileLogger.Warn(LoggerName, String.Format("Errore aggiornamento campo [{0}] su resolution [{1}].", fieldName, idResolution), ex)
            End Try
            '' Attenzione al cast: fondamentale per ottenere davvero il valore "Nothing". Alternativamente ritornerebbe 0
            ReflectionHelper.SetPropertyCase(fr, fieldName, If(id.HasValue AndAlso id.Value <> 0, id.Value, CType(Nothing, Integer?)))
        End If

        fileResFacade.Update(fr)
        Return True
    End Function

    Public Shared Sub SaveBiblosDocuments(ByRef resl As Resolution, ByRef documents As IList(Of DocumentInfo), ByRef attachments As IList(Of DocumentInfo), ByRef signature As String, ByRef idChain As Integer, ByRef idChainAllegati As Integer)
        If resl.Location Is Nothing Then
            Throw New DocSuiteException("Salvataggio atti con biblos", "La registrazione di Atti non è andata a buon fine. Nessuna LOCATION definita.")
        End If

        If Not documents.IsNullOrEmpty() Then
            For Each document As DocumentInfo In documents
                document.Signature = signature
            Next
            idChain = DocumentInfoFactory.ArchiveDocumentsInBiblos(documents, resl.Location.ReslBiblosDSDB, idChain)
        End If

        If Not attachments.IsNullOrEmpty() Then
            For Each document As DocumentInfo In attachments
                document.Signature = signature
            Next
            idChainAllegati = DocumentInfoFactory.ArchiveDocumentsInBiblos(attachments, resl.Location.ReslBiblosDSDB, idChainAllegati)
        End If
    End Sub

    ''' <summary>
    ''' Salva una lista di documenti su biblos caricando per riferimento il guid della catena (e opzionalmente l'id della catena old style)
    ''' </summary>
    ''' <param name="resl">Resolution associata che determina la location</param>
    ''' <param name="documents">Lista di documenti da salvare</param>
    ''' <param name="signature">signature dei documenti da applicare</param>
    ''' <param name="guidChain">Guid della catena padre (per riferimento)</param>
    ''' <param name="idChainOldStyle">id della catena biblos old style (caricato per riferimento se il primo valore è diverso da -1)</param>
    Public Shared Function SaveBiblosDocuments(ByRef resl As Resolution, ByRef documents As IList(Of DocumentInfo), ByRef signature As String, ByRef guidChain As Guid, Optional ByRef idChainOldStyle As Integer = -1) As List(Of BiblosDocumentInfo)
        If resl.Location Is Nothing Then
            Throw New DocSuiteException("Salvataggio atti con biblos", "La registrazione di Atti non è andata a buon fine. Nessuna LOCATION definita.")
        End If

        Dim documentInfos As New List(Of BiblosDocumentInfo)
        If Not documents.IsNullOrEmpty() Then
            For Each document As DocumentInfo In documents
                'Carico la signature
                document.Signature = signature
                'Carico il biblosDocumentInfo
                Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(resl.Location.ReslBiblosDSDB, guidChain)
                'Aggiorno la catena corrente
                guidChain = storedBiblosDocumentInfo.ChainId
                'Aggiungo il documento Biblos alla lista
                documentInfos.Add(storedBiblosDocumentInfo)
                'Verifico se è richiesto l'id biblos old style
                If Not idChainOldStyle.Equals(-1) Then
                    idChainOldStyle = storedBiblosDocumentInfo.BiblosChainId
                End If
            Next
        End If
        Return documentInfos
    End Function

    ''' <summary> Permette di inserire in biblos allegati del tipo annexed. </summary>
    ''' <param name="pResolition">Resolution nel quale si vogliono inserire gli annexed</param>
    ''' <param name="annexes">list di annexed incapsulati nel oggetto DocumentInfo</param>
    ''' <param name="signature">signatura da inserire negli annessi</param>
    Public Shared Function AddAnnexes(ByRef pResolition As Resolution, ByVal annexes As IList(Of DocumentInfo), ByVal signature As String) As Guid
        If pResolition.Location Is Nothing Then
            Throw New DocSuiteException("Salvataggio atti con biblos", "La registrazione di Atti non è andata a buon fine. Nessuna LOCATION definita.")
        End If

        If (annexes Is Nothing) OrElse annexes.Count <= 0 Then
            Return pResolition.File.IdAnnexes
        End If

        For Each annexed As DocumentInfo In annexes
            If Not String.IsNullOrEmpty(signature) Then
                annexed.Signature = signature
            End If
            pResolition.File.IdAnnexes = annexed.ArchiveInBiblos(pResolition.Location.ProtBiblosDSDB, pResolition.File.IdAnnexes).ChainId
        Next

        Return pResolition.File.IdAnnexes
    End Function

    Public Function SqlResolutionDocumentUpdate(ByVal idResolution As Integer, ByVal idDocument As Integer, ByVal type As DocType) As Boolean
        Try
            Dim frf As FileResolutionFacade = New FileResolutionFacade("ReslDB")
            Dim fr As FileResolution = frf.GetById(idResolution)

            If fr Is Nothing Then
                Return False
            End If

            If Not idDocument.Equals(0) Then
                Select Case type
                    Case DocType.Adozione
                        fr.IdAssumedProposal = idDocument
                    Case DocType.Allegati
                        fr.IdAttachements = idDocument
                    Case DocType.Disposizione
                        fr.IdResolutionFile = idDocument
                    Case DocType.Frontespizio
                        fr.IdFrontespizio = idDocument
                    Case DocType.OrganoControllo
                        fr.IdControllerFile = idDocument
                    Case DocType.Proposta
                        fr.IdProposalFile = idDocument
                    Case DocType.UltimaPagina
                        fr.IdUltimaPagina = idDocument
                    Case DocType.AllegatiRiservati
                        fr.IdPrivacyAttachments = idDocument
                    Case DocType.FrontespizioRitiro
                        fr.IdFrontalinoRitiro = idDocument
                    Case DocType.PrivacyPublicationDocument
                        fr.IdPrivacyPublicationDocument = idDocument
                End Select
            End If

            frf.Update(fr)
            Return True
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "[SqlResolutionDocumentUpdate] ", ex)
        End Try

        Return False
    End Function

    Public Function SqlResolutionDocumentUpdate(ByVal idResolution As Integer, ByVal idDocument As Guid, ByVal type As DocType) As Boolean
        Try
            Dim frf As FileResolutionFacade = New FileResolutionFacade("ReslDB")
            Dim fr As FileResolution = frf.GetById(idResolution)

            If fr Is Nothing Then
                Return False
            End If

            If Not idDocument.Equals(Guid.Empty) Then
                Select Case type
                    Case DocType.Annessi
                        fr.IdAnnexes = idDocument
                    Case DocType.DocumentoPrincipaleOmissis
                        fr.IdMainDocumentsOmissis = idDocument
                    Case DocType.AllegatiOmissis
                        fr.IdAttachmentsOmissis = idDocument
                End Select

            End If

            frf.Update(fr)
            Return True
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "[SqlResolutionDocumentUpdate] ", ex)
        End Try

        Return False
    End Function

    Public Function TestOperationStepProperty(ByVal OperationStep As String, ByVal Operation As String, ByVal resl As Resolution) As Boolean
        If String.IsNullOrEmpty(OperationStep) Then
            Exit Function
        End If
        Select Case UCase(Operation)
            Case "M" : Operation = "Modify"
            Case "N" : Operation = "Next"
            Case "D" : Operation = "Delete"
            Case "S" : Operation = "Segnature"
        End Select
        If Not StringHelper.InStrTest(OperationStep, Operation) Then
            Exit Function
        End If
        '--
        Dim s() As String = Split(OperationStep, "|")
        Dim i As Integer = 0
        Dim b As Boolean

        For i = 0 To UBound(s)
            If Not StringHelper.InStrTest(s(i), Operation) Then
                Continue For
            End If

            If InStr(s(i), "[") > 0 Then
                '-- Calcolare il diritto
                Dim Right As String = Mid(s(i), InStr(s(i), "[") + 1)
                Right = Left(Right, InStr(Right, "]") - 1)
                If resl.Id >= 0 Then
                    b = ResolutionRights.CheckRight(resl, CInt(Right))
                End If
            Else
                b = True
            End If
            Exit For

        Next
        Return b
    End Function

    Public Function CheckPrevServiceNumberSequence(ByVal idResl As Integer, ByVal year As Short, ByVal serviceNumber As String, ByVal adoptionDate As Date, resolutionType As Short) As Integer
        Return _dao.CheckPrevServiceNumberSequence(idResl, year, serviceNumber, adoptionDate, resolutionType)
    End Function

    Public Function CheckFollowingServiceNumberSequence(ByVal idResl As Integer, ByVal year As Short, ByVal serviceNumber As String, ByVal adoptionDate As Date, resolutionType As Short) As Integer
        Return _dao.CheckFollowingServiceNumberSequence(idResl, year, serviceNumber, adoptionDate, resolutionType)
    End Function

    Public Function GetNextFreeServiceNumber(idResolution As Integer, year As Integer, roleServiceCode As String, resolutionType As Short?) As Integer
        Return _dao.GetNextFreeServiceNumber(idResolution, year, roleServiceCode, resolutionType)
    End Function

    Public Function GetNextFreeServiceNumber(idResolution As Integer, year As Integer, resolutionType As Short?) As Integer
        Return GetNextFreeServiceNumber(idResolution, year, String.Empty, resolutionType)
    End Function

    Public Function CheckServiceNumber(ByVal year As Short, ByVal serviceNumber As String, ByVal id As Integer, resolutionType As Short) As Boolean
        Return _dao.CheckServiceNumber(year, serviceNumber, id, resolutionType)
    End Function

    Public Function GetByIdOrAdoptionData(ByVal type As String, ByVal id As String, ByVal inclusiveNumber As String, ByVal year As String) As Resolution
        ' TODO: ma per favore rimuovete sto schifio il prima possibile
        Dim pType As Nullable(Of Short) = Nothing
        If Not String.IsNullOrEmpty(type) Then
            pType = Convert.ToInt16(type)
        End If

        Dim pId As Integer? = Nothing
        If Not String.IsNullOrEmpty(id) Then
            pId = Convert.ToInt32(id)
        End If

        Dim pYear As Nullable(Of Short) = Nothing
        If Not String.IsNullOrEmpty(year) Then
            pYear = Convert.ToInt16(year)
        End If

        Return _dao.GetByIdOrAdoptionData(pType, pId, inclusiveNumber, pYear)
    End Function

    ''' <summary> Metodo che ritorna la data minima o massima del passo del flusso. </summary>
    ''' <param name="keyList">Lista di chiavi tra cui cercare</param>
    ''' <param name="property">Nome della proprietà (senza alias)</param>
    ''' <param name="minmax">Stringa che indica il criterio di aggregazione [MIN/MAX]</param>
    Function GetMinMaxWorkflowDate(ByVal keyList As IList(Of Integer), ByVal [property] As String, Optional ByVal minmax As String = "MAX") As Date?
        Return _dao.GetMinMaxWorkflowDate(keyList, [property], minmax)
    End Function

    Function GetResolutionsOrderProposerCode(ByVal keyList As IList(Of Integer)) As IList(Of Resolution)
        Return _dao.GetResolutionsOrderProposerCode(keyList)
    End Function

    Function GetResolutionOrderProposeDate(ByVal keyList As IList(Of Integer)) As IList(Of Resolution)
        Return _dao.GetResolutions(keyList, "ProposeDate", True)
    End Function

    ''' <summary> Recupera le delibere da pubblicare. </summary>
    ''' <returns>Elenco di delibere da pubblicare</returns>
    Public Function GetResolutionToPublicate() As IList(Of Resolution)
        Return _dao.GetResolutionToPublicate()
    End Function

    Public Function GetBySupervisoryBoardDate([from] As DateTime, [to] As DateTime?, type As ResolutionType) As IList(Of Resolution)
        Return _dao.GetBySupervisoryBoardDate([from], [to], type)
    End Function

    Public Function GetBySupervisoryBoardCollaborationId(collaborationId As Integer) As ICollection(Of Resolution)
        Return _dao.GetBySupervisoryBoardCollaborationId(collaborationId)
    End Function

    Public Function GetPublicated(ByVal [from] As DateTime?, ByVal [to] As DateTime, ByVal [adoptionFrom] As DateTime?, ByVal [adoptionTo] As DateTime?, Optional ByVal containerId As Integer? = Nothing, Optional ByVal reslType As Short? = Nothing) As IList(Of Resolution)
        Return _dao.GetPublicated([from], [to], [adoptionFrom], [adoptionTo], containerId, reslType)
    End Function

    ''' <summary> Recupera le delibere da rendere esecutive. </summary>
    ''' <param name="region">True: inviate alla region, False: non inviate alla regione</param>
    ''' <returns>Elenco di delibere da rendere esecutive</returns>
    Public Function GetResolutionToEsecutive(ByVal region As Boolean) As IList(Of Resolution)
        Return _dao.GetResolutionToExecutive(region)
    End Function

    Function UpdateProtocolLink(ByVal protocolLinkType As String, ByVal idResolutionList As String, ByVal protocol As String) As Boolean
        Return _dao.UpdateProtocolLink(protocolLinkType, idResolutionList, protocol)
    End Function

    Function UpdateProposerWarningDate(ByVal idResolutionList As String, ByVal data As Date?, ByVal userConnected As String) As Boolean
        Return _dao.UpdateProposerWarningDate(idResolutionList, data, userConnected)
    End Function

    Function UpdateEffectivenessDate(ByVal idResolutionList As String, ByVal dataEsecutivita As Date?, ByVal userConnected As String) As Boolean
        Return _dao.UpdateEffectivenessDate(idResolutionList, dataEsecutivita, userConnected)
    End Function

    Function UpdatePublishingDate(ByVal idResolutionList As String, ByVal dataPubblicazione As Date?, ByVal userConnected As String, isConfTo As Boolean) As Boolean
        Dim resolution As Resolution
        Dim actionSuccess As Boolean = False
        Dim data As Date? = dataPubblicazione
        For Each item As Integer In idResolutionList.Split(","c).Select(Function(f) Integer.Parse(f))
            resolution = GetById(item)
            data = dataPubblicazione

            If Not (resolution.OCSupervisoryBoard) Then
                data = resolution.SupervisoryBoardWarningDate
            End If

            If isConfTo AndAlso data.HasValue Then
                data = data.Value.AddDays(1)
            End If

            If (Not _dao.UpdatePublishingDate(item.ToString(), data, userConnected)) Then
                Return False
            End If
        Next
        Return True
    End Function

    Function UpdateOC(ByVal idResolutionList As String, ByVal dataSpedCollegio As Date?, ByVal protCollegio As Protocol, ByVal dataSpedRegione As Date?, ByVal protRegione As Protocol, ByVal dataSpedGestione As Date?, ByVal protGestione As Protocol, ByVal userConnected As String) As Boolean
        Return _dao.UpdateOC(idResolutionList, dataSpedCollegio, protCollegio, dataSpedRegione, protRegione, dataSpedGestione, protGestione, userConnected)
    End Function

    Public Function UpdateStatus(ByRef resolution As Resolution, ByVal newIdStatus As Short) As Boolean
        Try
            Dim status As ResolutionStatus = Factory.ResolutionStatusFacade.GetById(newIdStatus)
            If status Is Nothing Then
                Throw New DocSuiteException("Errore update status", "Id status non trovato.")
            End If
            resolution.Status = status
            UpdateOnly(resolution)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore nella modifica dello status resolution " & newIdStatus, ex)
            Return False
        End Try

        Return True
    End Function

    Public Function Duplicate(ByVal idResolution As Integer,
        Optional ByVal duplicateType As Boolean = False,
        Optional ByVal duplicateRecipients As Boolean = False,
        Optional ByVal duplicateProposer As Boolean = False,
        Optional ByVal duplicateObject As Boolean = False,
        Optional ByVal duplicateNote As Boolean = False,
        Optional ByVal duplicateManager As Boolean = False,
        Optional ByVal duplicateContainer As Boolean = False,
        Optional ByVal duplicateCategory As Boolean = False,
        Optional ByVal duplicateAssignee As Boolean = False
        ) As Resolution

        ' TODO: rimuovere tutti gli optional e non fare overloads

        Dim resl As Resolution = GetById(idResolution, False)
        Dim reslDuplicate As Resolution = New Resolution

        'assegno temporaneamente IdResolution al duplicato
        reslDuplicate.Id = resl.Id

        'Type
        If duplicateType Then
            reslDuplicate.Type = resl.Type
        End If

        'Recipients
        If duplicateRecipients Then
            For Each rc As ResolutionContact In resl.ResolutionContactsRecipients
                reslDuplicate.ResolutionContacts.Add(rc)
                reslDuplicate.ResolutionContactsRecipients.Add(rc)
            Next
            reslDuplicate.AlternativeRecipient = resl.AlternativeRecipient
        End If

        'Proposer
        If duplicateProposer Then
            For Each rc As ResolutionContact In resl.ResolutionContactProposers
                reslDuplicate.ResolutionContacts.Add(rc)
                reslDuplicate.ResolutionContactProposers.Add(rc)
            Next
            reslDuplicate.AlternativeProposer = resl.AlternativeProposer
        End If

        'Manager
        If duplicateManager Then
            For Each rc As ResolutionContact In resl.ResolutionContactsManagers
                reslDuplicate.ResolutionContacts.Add(rc)
                reslDuplicate.ResolutionContactsManagers.Add(rc)
            Next
            reslDuplicate.AlternativeManager = resl.AlternativeManager
        End If

        'Assignee
        If duplicateAssignee Then
            For Each rc As ResolutionContact In resl.ResolutionContactsAssignees
                reslDuplicate.ResolutionContacts.Add(rc)
                reslDuplicate.ResolutionContactsAssignees.Add(rc)
            Next
            reslDuplicate.AlternativeAssignee = resl.AlternativeAssignee
        End If

        'Object
        If duplicateObject Then
            reslDuplicate.ResolutionObject = resl.ResolutionObject
        End If

        'Note
        If duplicateNote Then
            reslDuplicate.Note = resl.Note
        End If

        'Container
        If duplicateContainer Then
            reslDuplicate.Container = resl.Container
            reslDuplicate.Location = resl.Container.ReslLocation
        End If

        'Classificatore
        If duplicateCategory Then
            reslDuplicate.Category = resl.Category
            reslDuplicate.SubCategory = resl.SubCategory
        End If

        Return reslDuplicate
    End Function

    Public Function GetResolutionRoles(resl As Resolution) As IList(Of ResolutionRole)
        Return _dao.GetResolutionRoles(resl)
    End Function

    ''' <summary> Tutte le resolution adottate senza DSI associate a AVCP. </summary>
    ''' <remarks> Prima versione fatta di corsa. </remarks>
    Public Function GetAdoptedResolutionNotAvcp(ByVal [from] As DateTime?, ByVal [to] As DateTime?) As IList(Of Resolution)
        If Not DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.HasValue Then
            Throw New DocSuiteException("Errore ricerca", String.Format("Impossibile trovare la {0} di default.", DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName.ToLower()))
        End If

        ' Risoluzioni adottate
        Dim x As New ResolutionFinder2
        x.InStepDateRange = New Range(Of Date)([from], [to])
        Dim adopted As IList(Of Resolution) = x.GetAdoptedResolutions()

        ' ritorno solo le resolution che non sono presenti nelle DSI trovate
        Dim toReturn As New List(Of Resolution)
        For Each resolution As Resolution In adopted
            Dim dsiIds As List(Of Integer) = Factory.ResolutionDocumentSeriesItemFacade.GetByResolution(resolution).Select(Function(r) r.IdDocumentSeriesItem.Value).ToList()
            ' Passo al db di protocollo e tiro su tutte le documentseriesitem
            Dim dSItems As IList(Of DocumentSeriesItem) = Factory.DocumentSeriesItemFacade.GetByIdentifiers(dsiIds)
            ' Nessuna DSI dell'atto deve avere un riferimento a AVCP
            Dim toAdd As Boolean = dSItems.All(Function(documentSeriesItem) documentSeriesItem.DocumentSeries.Id <> DocSuiteContext.Current.ProtocolEnv.AvcpDocumentSeriesId.Value)
            If toAdd Then
                toReturn.Add(resolution)
            End If
        Next

        ' ritorno tutte le resolution così trovate
        Return toReturn
    End Function

    ''' <summary> Aggiorno l'elenco di atti passato dall'xml con i valori passati </summary>
    Public Sub UpdateFromXml(ByVal resolutionXml As ResolutionXML, ByVal sourceProtocol As Protocol)
        For Each resl As Resolution In From rtu In resolutionXml.ResolutionsToUpdate Select GetById(rtu)
            'Verifico se devo aggiornare il protocollo di invio ai Servizi
            If Not String.IsNullOrEmpty(resolutionXml.ProposerProtocolLink) Then
                If String.IsNullOrEmpty(resl.ProposerProtocolLink) Then
                    resl.ProposerProtocolLink = resolutionXml.ProposerProtocolLink
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Collegato l'atto {0} al protocollo corrente in qualità di invio ai Servizi", resl.InclusiveNumber))
                Else
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Avvertenza: Impossibile collegare l'atto {0} al protocollo corrente in qualità di invio ai Servizi in quanto tale atto è già collegato al protocollo {1}", resl.InclusiveNumber, Factory.ProtocolFacade.GetProtocolFromCalculatedLink(resl.ProposerProtocolLink).ToString()))
                End If
            End If

            'Verifico se devo aggiornare la data di invio al collegio sindacale
            If resolutionXml.CollegioSindacaleDate.HasValue Then
                If Not resl.SupervisoryBoardWarningDate.HasValue Then
                    resl.SupervisoryBoardWarningDate = resolutionXml.CollegioSindacaleDate.Value
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Aggiornata la data di invio al Collegio Sindacale dell'atto {0} con la data corrente", resl.InclusiveNumber))
                Else
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Avvertenza: Impossibile aggiornare la data di invio al Collegio Sindacale dell'atto {0} con la data corrente in quanto tale atto è già collegato al protocollo {1}", resl.InclusiveNumber, resl.SupervisoryBoardWarningDate.Value))
                End If
            End If

            'Verifico se devo aggiornare il protocollo di invio al Collegio Sindacale
            If Not String.IsNullOrEmpty(resolutionXml.CollegioSindacaleProtocolLink) Then
                If String.IsNullOrEmpty(resl.SupervisoryBoardProtocolLink) Then
                    resl.SupervisoryBoardProtocolLink = resolutionXml.CollegioSindacaleProtocolLink
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Collegato l'atto {0} al protocollo corrente in qualità di invio al Collegio Sindacale", resl.InclusiveNumber))

                    If DocSuiteContext.Current.ResolutionEnv.IsSendMailEnabled Then
                        Const sLettera As String = "Lettera"
                        Dim sError As String = String.Empty
                        'Recuperare il protocollo della lettera (trasmissione al collegio - tiff)
                        Try
                            Dim lettera As DocumentInfo = ProtocolFacade.RecuperoProtocollo(sourceProtocol, sLettera)
                            Select Case resl.Type.Id
                                Case ResolutionType.IdentifierDelibera
                                    Dim subject As String = String.Format("Invio Elenco {0} Adottate - Data Adozione {1}", Factory.ResolutionTypeFacade.DeliberaCaption, resl.AdoptionDate.DefaultString)
                                    Factory.UserErrorFacade.SmtpLogSendMail(subject, "", lettera, sError, DocSuiteContext.Current.ResolutionEnv.SupervisoryMailTo)
                                Case ResolutionType.IdentifierDetermina
                                    Dim subject As String = String.Format("Invio Elenco {0} Adottate", Factory.ResolutionTypeFacade.DeterminaCaption)
                                    Factory.UserErrorFacade.SmtpLogSendMail(subject, "", lettera, sError, DocSuiteContext.Current.ResolutionEnv.OCDetermMailTo)
                            End Select
                        Catch ex As Exception
                            Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PE, "Errore in Recupero Protocollo: " & ex.Message)
                            FileLogger.Warn(LoggerName, "Errore in Recupero Protocollo: " & ex.Message, ex)
                        End Try
                    End If
                Else
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Avvertenza: Impossibile collegare l'atto {0} al protocollo corrente in qualità di invio al Collegio Sindacale in quanto tale atto è già collegato al protocollo {1}", resl.InclusiveNumber, Factory.ProtocolFacade.GetProtocolFromCalculatedLink(resl.SupervisoryBoardProtocolLink).ToString()))
                End If
            End If

            'Verifico se devo aggiornare la data di invio pubblicazione lettera con firma digitale
            If resolutionXml.InvioPubbLetteraFirmaDigitaleDate.HasValue Then
                If Not resl.SupervisoryBoardWarningDate.HasValue Then
                    resl.SupervisoryBoardWarningDate = resolutionXml.InvioPubbLetteraFirmaDigitaleDate.Value
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Aggiornata la data di invio pubblicazione lettera con firma digitale dell'atto {0} con la data corrente", resl.InclusiveNumber))
                Else
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Avvertenza: Impossibile aggiornare la data di invio pubblicazione lettera con firma digitale dell'atto {0} con la data corrente in quanto tale atto è già collegato al protocollo {1}", resl.InclusiveNumber, resl.SupervisoryBoardWarningDate.Value))
                End If
            End If

            If Not String.IsNullOrEmpty(resolutionXml.InvioPubbLetteraFirmaDigitaleProtocolLink) Then
                '-- Salvataggio del num. Protocollo per le delibere selezionate
                Factory.ResolutionFacade.UpdateProtocolLink("PublishingProtocolLink", String.Join(",", resolutionXml.ResolutionsToUpdate.ToArray()), resolutionXml.InvioPubbLetteraFirmaDigitaleProtocolLink)
            End If

            'Verifico se devo aggiornare il protocollo di invio pubblicazione lettera con firma digitale
            If Not String.IsNullOrEmpty(resolutionXml.InvioPubbLetteraFirmaDigitaleProtocolLink) Then
                If String.IsNullOrEmpty(resl.SupervisoryBoardProtocolLink) Then
                    resl.SupervisoryBoardProtocolLink = resolutionXml.InvioPubbLetteraFirmaDigitaleProtocolLink
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Collegato l'atto {0} al protocollo corrente in qualità di invio pubblicazione lettera con firma digitale", resl.InclusiveNumber))

                    If DocSuiteContext.Current.ResolutionEnv.IsSendMailEnabled Then
                        Const sLettera As String = "Lettera"
                        Dim sError As String = String.Empty
                        'Recuperare il protocollo della lettera
                        Try
                            Dim lettera As DocumentInfo = ProtocolFacade.RecuperoProtocollo(sourceProtocol, sLettera)
                            Select Case resl.Type.Id
                                Case ResolutionType.IdentifierDelibera
                                    Dim subject As String = String.Format("Invio Elenco {0} Adottate - Data Adozione {1}", Factory.ResolutionTypeFacade.DeliberaCaption, resl.AdoptionDate.DefaultString)
                                    Factory.UserErrorFacade.SmtpLogSendMail(subject, "", lettera, sError, DocSuiteContext.Current.ResolutionEnv.SupervisoryMailTo)
                                Case ResolutionType.IdentifierDetermina
                                    Dim subject As String = String.Format("Invio Elenco {0} Adottate", Factory.ResolutionTypeFacade.DeterminaCaption)
                                    Factory.UserErrorFacade.SmtpLogSendMail(subject, "", lettera, sError, DocSuiteContext.Current.ResolutionEnv.OCDetermMailTo)
                            End Select
                        Catch ex As Exception
                            Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PE, "Errore in Recupero Protocollo: " & ex.Message)
                            FileLogger.Warn(LoggerName, "Errore in Recupero Protocollo: " & ex.Message, ex)
                        End Try
                    End If
                Else
                    Factory.ProtocolLogFacade.Insert(sourceProtocol, ProtocolLogEvent.PL, String.Format("Avvertenza: Impossibile collegare l'atto {0} al protocollo corrente in qualità di invio pubblicazione lettera con firma digitale in quanto tale atto è già collegato al protocollo {1}", resl.InclusiveNumber, Factory.ProtocolFacade.GetProtocolFromCalculatedLink(resl.SupervisoryBoardProtocolLink).ToString()))
                End If
            End If

            'Aggiorno effettivamentes l'atto
            Update(resl)
        Next
    End Sub

    Private Function GeneraLetteraGetResolution(ByVal typeDoc As Short, ByVal keyList As IList(Of Integer), ByVal tipoLettera As String,
                ByRef sDataFrom As String, ByRef sDataTo As String) As IList(Of ResolutionLetter)
        Dim resolutions As IList(Of ResolutionLetter)

        Select Case tipoLettera
            Case "E"
                resolutions = Factory.ResolutionFacade.GetResolutionsLetter(keyList, "AdoptionDate,PublishingDate,EffectivenessDate,Cont.HeadingLetter", "AdoptionDate,PublishingDate,EffectivenessDate,HeadingLetter", "AdoptionDate,PublishingDate,EffectivenessDate")

            Case "TAR-B", "TAR-U", "TAR-A", "TAR-P", "AL"
                resolutions = Factory.ResolutionFacade.GetResolutionsLetter(keyList)

            Case "TAD"
                resolutions = Factory.ResolutionFacade.GetResolutionsLetter(keyList, , "Number", OrderByAsc:="Number")

            Case Else
                If tipoLettera = "TAS" And typeDoc = ResolutionType.IdentifierDetermina Then
                    sDataFrom = Factory.ResolutionFacade.GetMinMaxWorkflowDate(keyList, "AdoptionDate", "MIN")
                    sDataTo = Factory.ResolutionFacade.GetMinMaxWorkflowDate(keyList, "AdoptionDate")
                    resolutions = Factory.ResolutionFacade.GetResolutionsLetter(keyList, OrderByAsc:="Number")
                ElseIf tipoLettera = "P" And typeDoc = ResolutionType.IdentifierDetermina Then
                    resolutions = Factory.ResolutionFacade.GetResolutionsLetter(keyList, , "Number")
                Else
                    resolutions = Factory.ResolutionFacade.GetResolutionsLetter(keyList, "AdoptionDate,Cont.HeadingLetter", "AdoptionDate,HeadingLetter", "AdoptionDate")
                End If
        End Select
        If resolutions Is Nothing OrElse resolutions.Count = 0 Then
            If String.IsNullOrEmpty(sDataFrom & sDataTo) Then
                Return Nothing
            End If
        End If

        Return resolutions
    End Function

    Public Function GeneraLetteraToDocumentInfo(ByVal typeDoc As Short, ByVal keyList As IList(Of Integer), ByVal tipoLettera As String,
                ByRef fileTemp As String, Optional ByVal sProt As String = "", Optional ByVal fileSource As String = "",
                Optional ByVal dataStampa As String = "", Optional ByVal ruolo As String = "", Optional ByVal firma As String = "",
                Optional ByVal dataFine As String = "") As DocumentInfo

        Dim doc As DocumentInfo = GeneraLettera(typeDoc, keyList, tipoLettera, fileTemp, sProt, fileSource, dataStampa, ruolo, firma, dataFine)
        fileTemp = String.Empty
        Return doc
    End Function

    Public Function GeneraLetteraToString(ByVal typeDoc As Short, ByVal keyList As IList(Of Integer), ByVal tipoLettera As String,
                ByRef fileTemp As String, Optional ByVal sProt As String = "", Optional ByVal fileSource As String = "",
                Optional ByVal dataStampa As String = "", Optional ByVal ruolo As String = "", Optional ByVal firma As String = "",
                Optional ByVal dataFine As String = "") As Boolean
        If GeneraLettera(typeDoc, keyList, tipoLettera, fileTemp, sProt, fileSource, dataStampa, ruolo, firma, dataFine) IsNot Nothing Then
            Return True
        End If
        Return False
    End Function

    Private Function GeneraLettera(ByVal typeDoc As Short, ByVal keyList As IList(Of Integer), ByVal tipoLettera As String,
                ByRef fileTemp As String, Optional ByVal sProt As String = "", Optional ByVal fileSource As String = "",
                Optional ByVal dataStampa As String = "", Optional ByVal ruolo As String = "", Optional ByVal firma As String = "",
                Optional ByVal dataFine As String = "") As DocumentInfo

        Dim resolutions As IList(Of ResolutionLetter)
        Dim sDataFrom As String = String.Empty
        Dim sDataTo As String = String.Empty

        resolutions = GeneraLetteraGetResolution(typeDoc, keyList, tipoLettera, sDataFrom, sDataTo)
        If resolutions Is Nothing Then
            Return Nothing
        End If

        Dim doc As DocumentInfo = Nothing
        CommonUtil.GetInstance().UserDeleteTemp(TempType.P)
        Try
            FileHelper.CopySafe(CommonUtil.GetInstance().AppPath & PathStampeTo & "logo.jpg", CommonUtil.GetInstance().AppTempPath & "logo.jpg", True)
            FileHelper.CopySafe(CommonUtil.GetInstance().AppPath & PathStampeTo & "logo2.jpg", CommonUtil.GetInstance().AppTempPath & "logo2.jpg", True)
        Catch ex As Exception
            FileLogger.Warn(LogName.FileLog, ex.Message, ex)
        End Try
        fileTemp = CommonUtil.UserDocumentName & "-Print-" & String.Format("{0:HHmmss}", Now()) & ".htm"
        Dim fSource As String = CommonUtil.GetInstance().AppPath & PathStampeTo
        fSource &= GetLettera(typeDoc, tipoLettera, fileSource)
        Dim fDestination As String = CommonUtil.GetInstance().AppTempPath & fileTemp
        ' Lettura file
        Dim fileText As String = ""
        Try

            Using sr As StreamReader = New StreamReader(fSource)
                fileText = sr.ReadToEnd()
                sr.Close()
            End Using
            ' Praparazione file
            Select Case typeDoc
                Case ResolutionType.IdentifierDelibera
                    Select Case tipoLettera
                        Case "TAR-B", "TAR-U", "TAR-A", "TAR-P"
                            DoSostituzioniLettera_RigheMultiple(tipoLettera, resolutions, sDataFrom, sDataTo, sProt, typeDoc, dataStampa, ruolo, firma, fileText, dataFine)
                        Case Else
                            DoSostituzioniLettera(tipoLettera, resolutions, sDataFrom, sDataTo, sProt, typeDoc, dataStampa, ruolo, firma, fileText, dataFine)
                    End Select
                Case ResolutionType.IdentifierDetermina
                    DoSostituzioniLettera(tipoLettera, resolutions, sDataFrom, sDataTo, sProt, typeDoc, dataStampa, ruolo, firma, fileText, dataFine)
            End Select
            ' Scrittura file
            Using sw As StreamWriter = New StreamWriter(fDestination)
                sw.Write(fileText)
                sw.Close()
            End Using
            doc = New MemoryDocumentInfo(GetBytes(fileText), fileTemp)

        Catch ex As FileNotFoundException
            Throw New DocSuiteException("Generazione lettera", String.Format("Impossibile trovare il file [{0}]", ex.FileName), ex)
        Catch ex As Exception
            FileLogger.Warn(LogName.FileLog, fileText)
            Throw
        End Try
        Return doc
    End Function

    Private Function GetBytes(ByVal Str As String) As Byte()
        Dim bytes As Byte() = New Byte(Str.Length * 2 - 1) {}
        System.Buffer.BlockCopy(Str.ToCharArray(), 0, bytes, 0, bytes.Length)
        Return bytes
    End Function

    Private Function GetLettera(ByVal typeDoc As Short, ByVal tipoLettera As String, ByVal fileSource As String) As String
        Dim fSource As String = String.Empty
        Select Case tipoLettera
            Case "TAD" 'Determine - Avvenuta adozione 
                fSource = "LetteraTrasmAvvenutaAdozione.htm"
            Case "TAS"  'Lettera Trasmissione Adozione Servizi
                Select Case typeDoc
                    Case ResolutionType.IdentifierDelibera
                        fSource = "LetteraTrasmAdozServizi.htm"
                    Case ResolutionType.IdentifierDetermina
                        fSource = "LetteraTrasmAdozDeterm.htm"
                End Select
            Case "TACS" 'Lettera Trasmissione Adozione Colleggio Sindacale
                fSource = "LetteraTrasmAdozCollegio.htm"

            Case "TAR-B"    'Lettera Trasmissione Adozione Regione
                Select Case typeDoc
                    Case ResolutionType.IdentifierDelibera
                        fSource = "Regione\LetteraTrasmAdozRegioneBilancio_RigheMultiple.htm"
                    Case ResolutionType.IdentifierDetermina
                        fSource = "Regione\LetteraTrasmAdozRegioneBilancio.htm"
                End Select

            Case "TAR-U"    'Lettera Trasmissione Adozione Regione
                Select Case typeDoc
                    Case ResolutionType.IdentifierDelibera
                        fSource = "Regione\LetteraTrasmAdozRegioneUniversita_RigheMultiple.htm"
                    Case ResolutionType.IdentifierDetermina
                        fSource = "Regione\LetteraTrasmAdozRegioneUniversita.htm"
                End Select

            Case "TAR-A"    'Lettera Trasmissione Adozione Regione
                Select Case typeDoc
                    Case ResolutionType.IdentifierDelibera
                        fSource = "Regione\LetteraTrasmAdozRegioneModifica_RigheMultiple.htm"
                    Case ResolutionType.IdentifierDetermina
                        fSource = "Regione\LetteraTrasmAdozRegioneModifica.htm"
                End Select

            Case "TAR-P"    'Lettera Trasmissione Adozione Regione
                Select Case typeDoc
                    Case ResolutionType.IdentifierDelibera
                        fSource = "Regione\LetteraTrasmAdozRegionePrevisione_RigheMultiple.htm"
                    Case ResolutionType.IdentifierDetermina
                        fSource = "Regione\LetteraTrasmAdozRegionePrevisione.htm"
                End Select

            Case "TAG"  'Lettera Trasmissione Adozione Controllo di Gestione
                fSource = "LetteraTrasmAdozGestione.htm"
            Case "AL"  'Lettera Trasmissione Adozione Controllo di Gestione
                fSource = "Regione\" & fileSource
            Case "P"    'Lettera Affissione Albo
                Select Case typeDoc
                    Case ResolutionType.IdentifierDelibera
                        fSource = "LetteraPubblicazione.htm"
                    Case ResolutionType.IdentifierDetermina
                        fSource = "LetteraPubblicazioneDeterm.htm"
                End Select
            Case "E"    'Lettera Avvenuta Esecutività
                fSource = "LetteraTrasmEsecServizi.htm"
        End Select
        Return fSource
    End Function

    ''' <summary> Boh </summary>
    ''' <remarks>
    ''' <li>Deliberazione n. <b>@NUMERO@/@CODICESERV@/@ANNO@</b> del <b>@DATAADOZIONE@</b> avente per oggetto: <i>“@OGGETTO@”</i></li>
    ''' </remarks>
    Private Sub DoSostituzioniLettera_RigheMultiple(ByVal tipoLettera As String, ByVal resolutions As IList(Of ResolutionLetter), ByVal adoptionDate_From As String, ByVal adoptionDate_To As String,
            ByVal sProt As String, ByVal typeDoc As Short,
            ByVal dataStampa As String, ByVal ruolo As String, ByVal firma As String,
            ByRef fileText As String, ByVal dataFine As String)

        Dim resl As ResolutionLetter = Nothing
        If resolutions IsNot Nothing Then
            resl = resolutions(0)
        Else
            Throw New Exception("ResolutionLetter non trovata.")
        End If

        fileText = fileText.Replace("@CODIFICA@", "<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
        If dataStampa <> "" Then
            fileText = fileText.Replace("@DATASTAMPA@", Format(CDate(dataStampa), "dd/MM/yyyy"))
        End If
        '--
        If (tipoLettera = "E") Then
            fileText = fileText.Replace("@DATAPUBBLICAZIONE@", String.Format("{0:dd MMMM yyyy}", resl.PublishingDate))
            fileText = fileText.Replace("@DATAESECUTIVITA@", String.Format("{0:dd MMMM yyyy}", resl.EffectivenessDate))
        End If
        If InStr(fileText, "@DATAPUBBLICAZIONE@") > 0 Then
            fileText = fileText.Replace("@DATAPUBBLICAZIONE@", String.Format("{0:dd/MM/yyyy}", resl.PublishingDate))
            If dataFine <> "" Then
                fileText = fileText.Replace("@DATAPUBBLICAZIONE+10@", Format(CDate(dataFine), "dd/MM/yyyy"))
            Else
                fileText = fileText.Replace("@DATAPUBBLICAZIONE+10@", String.Format("{0:dd/MM/yyyy}", DateAdd(DateInterval.Day, 10, resl.PublishingDate.Value)))
            End If

        End If

        Dim templateRigheMultiple As String = String.Empty
        Select Case tipoLettera
            Case "TAR-A"
                templateRigheMultiple = "<li>n. <b>@NUMERO@/@CODICESERV@/@ANNO@</b> del <b>@DATAADOZIONE@</b> avente per oggetto: <i>""@OGGETTO@""</i>"
            Case "TAR-B"
                templateRigheMultiple = "<li>Deliberazione n. <b>@NUMERO@/@CODICESERV@/@ANNO@</b> del <b>@DATAADOZIONE@</b> avente per oggetto: <i>""@OGGETTO@""</i>"
            Case "TAR-P"
                templateRigheMultiple = "<li>n. <b>@NUMERO@/@CODICESERV@/@ANNO@</b> del <b>@DATAADOZIONE@</b> avente per oggetto: <i>""@OGGETTO@""</i>"
            Case "TAR-U"
                templateRigheMultiple = "<li>n. <b>@NUMERO@/@CODICESERV@/@ANNO@</b> del <b>@DATAADOZIONE@</b> avente per oggetto: <i>""@OGGETTO@""</i>"
        End Select

        Dim newRow As String = String.Empty
        Dim sb As New StringBuilder
        For Each r As ResolutionLetter In resolutions
            newRow = templateRigheMultiple
            newRow = newRow.Replace("@NUMERO@", String.Format("{0:0000000}", r.Number))
            newRow = newRow.Replace("@CODICESERV@", UCase(r.ProposerCode))
            newRow = newRow.Replace("@ANNO@", r.Year)
            newRow = newRow.Replace("@OGGETTO@", StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(r.ResolutionObject)))
            newRow = newRow.Replace("@DATAADOZIONE@", CDate(r.AdoptionDate).ToLongDateString)

            sb.Append(newRow)
        Next
        fileText = fileText.Replace("@RIGHEMULTIPLE@", sb.ToString)

        If tipoLettera = "TAS" And typeDoc = ResolutionType.IdentifierDetermina Then
            Dim dataFrom As String = If(dataStampa <> "", dataStampa, adoptionDate_From)
            Dim dataTo As String = If(dataFine <> "", dataFine, adoptionDate_To)
            fileText = fileText.Replace("@DATAADOZIONEFROM@", dataFrom)
            fileText = fileText.Replace("@DATAADOZIONETO@", dataTo)
        End If
        If InStr(fileText, "@HEADINGL@") > 0 Then
            Dim sH As String = resl.HeadingLetter
            Select Case True
                Case sH = ""
                Case sH.Substring(0, 1).ToUpper = "U"
                    fileText = fileText.Replace("@LAPOSTROFO@", "l'")
                Case Else
                    fileText = fileText.Replace("@LAPOSTROFO@", " ")
            End Select
            fileText = fileText.Replace("@HEADINGL@", sH)
        Else
            fileText = fileText.Replace("@LAPOSTROFO@", "")
        End If
        fileText = fileText.Replace("@RUOLO@", ruolo)
        fileText = fileText.Replace("@NOMECOGNOME@", firma)
        Select Case typeDoc
            Case ResolutionType.IdentifierDelibera
                fileText = fileText.Replace("@TIPOLOGIA@", "deliberazioni")
            Case ResolutionType.IdentifierDetermina
                fileText = fileText.Replace("@TIPOLOGIA@", "determinazioni dirigenziali")
        End Select

        fileText = fileText.Replace("@FIRMA@", If(sProt = "", "", "FIRMATA IN ORIGINALE"))
        '--
        If sProt <> "" Then
            Dim s() As String = Split(sProt, "|")
            fileText = fileText.Replace("@NUMPROT@", s(0) & "/" & String.Format("{0:0000000}", CInt(s(1))))
            fileText = fileText.Replace("@DATAPROT@", String.Format("{0:dd/MM/yyyy}", CDate(s(3))))
        Else
            fileText = fileText.Replace("@NUMPROT@", If(sProt = "", "", sProt))
            fileText = fileText.Replace("@DATAPROT@", If(sProt = "", "", sProt))
        End If
        '--
        If InStr(fileText, "@RIGHETABLEDET@") > 0 Then Call GeneraTabellaDetermine(resolutions, fileText)
        '--
    End Sub

    Private Shared Sub DoSostituzioniLettera(ByVal tipoLettera As String, ByVal resolutions As IList(Of ResolutionLetter), ByVal adoptionDate_From As String, ByVal adoptionDate_To As String,
            ByVal sProt As String, ByVal typeDoc As Short,
            ByVal dataStampa As String, ByVal ruolo As String, ByVal firma As String,
            ByRef fileText As String, ByVal dataFine As String)

        Dim resl As ResolutionLetter = Nothing
        If resolutions IsNot Nothing Then
            resl = resolutions(0)
        Else
            Throw New Exception("ResolutionLetter non trovata.")
        End If

        fileText = fileText.Replace("@CODIFICA@", "<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
        If dataStampa <> "" Then
            fileText = fileText.Replace("@DATASTAMPA@", Format(CDate(dataStampa), "dd/MM/yyyy"))
        End If
        '--
        If (tipoLettera = "E") Then
            fileText = fileText.Replace("@DATAPUBBLICAZIONE@", String.Format("{0:dd MMMM yyyy}", resl.PublishingDate))
            fileText = fileText.Replace("@DATAESECUTIVITA@", String.Format("{0:dd MMMM yyyy}", resl.EffectivenessDate))
        End If
        If InStr(fileText, "@DATAPUBBLICAZIONE@") > 0 Then
            fileText = fileText.Replace("@DATAPUBBLICAZIONE@", String.Format("{0:dd/MM/yyyy}", resl.PublishingDate))
            If dataFine <> "" Then
                fileText = fileText.Replace("@DATAPUBBLICAZIONE+10@", Format(CDate(dataFine), "dd/MM/yyyy"))
            Else
                fileText = fileText.Replace("@DATAPUBBLICAZIONE+10@", String.Format("{0:dd/MM/yyyy}", DateAdd(DateInterval.Day, 10, resl.PublishingDate.Value)))
            End If

        End If
        If (tipoLettera = "TAR-B") Or (tipoLettera = "TAR-U") Or (tipoLettera = "TAR-A") _
                Or (tipoLettera = "TAR-P") Or (tipoLettera = "AL") Then
            '"TAR-B", "TAR-U", "TAR-A", "TAR-P", "AL"
            fileText = fileText.Replace("@NUMERO@", String.Format("{0:0000000}", resl.Number))
            fileText = fileText.Replace("@CODICESERV@", UCase(resl.ProposerCode))
            fileText = fileText.Replace("@ANNO@", resl.Year)
            Dim ss As String = StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(resl.ResolutionObject))
            fileText = fileText.Replace("@OGGETTO@", ss)
        End If
        If tipoLettera = "TAS" And typeDoc = ResolutionType.IdentifierDetermina Then
            Dim DataFrom As String = If(dataStampa <> "", dataStampa, adoptionDate_From)
            Dim DataTo As String = If(dataFine <> "", dataFine, adoptionDate_To)
            fileText = fileText.Replace("@DATAADOZIONEFROM@", DataFrom)
            fileText = fileText.Replace("@DATAADOZIONETO@", DataTo)
        End If
        If InStr(fileText, "@HEADINGL@") > 0 Then
            Dim sH As String = resl.HeadingLetter
            Select Case True
                Case sH = ""
                Case sH.Substring(0, 1).ToUpper = "U"
                    fileText = fileText.Replace("@LAPOSTROFO@", "l'")
                Case Else
                    fileText = fileText.Replace("@LAPOSTROFO@", " ")
            End Select
            fileText = fileText.Replace("@HEADINGL@", sH)
        Else
            fileText = fileText.Replace("@LAPOSTROFO@", "")
        End If
        fileText = fileText.Replace("@RUOLO@", ruolo)
        fileText = fileText.Replace("@NOMECOGNOME@", firma)
        Select Case typeDoc
            Case ResolutionType.IdentifierDelibera
                fileText = fileText.Replace("@TIPOLOGIA@", "deliberazioni")
            Case ResolutionType.IdentifierDetermina
                fileText = fileText.Replace("@TIPOLOGIA@", "determinazioni dirigenziali")
        End Select
        '--
        If InStr(fileText, "@DATAADOZIONE@") > 0 Then
            Dim sAdozGiorno As String = ""
            Dim sMeseAttuale As String = Format(resl.AdoptionDate, "MMMM")
            Dim sMeseRecordPrecedente As String = sMeseAttuale
            Dim sAnno As String = Format(resl.AdoptionDate, "yyyy")
            For Each r As ResolutionLetter In resolutions
                sMeseAttuale = Format(r.AdoptionDate, "MMMM")
                If sMeseAttuale <> sMeseRecordPrecedente Then
                    sAdozGiorno &= " " & sMeseRecordPrecedente & " e "
                Else
                    If sAdozGiorno <> "" Then sAdozGiorno &= " - "
                End If
                sAdozGiorno &= Format(r.AdoptionDate, "dd")
                '--
                sMeseRecordPrecedente = sMeseAttuale
            Next
            sAdozGiorno &= " " & sMeseAttuale & " " & sAnno
            fileText = fileText.Replace("@DATAADOZIONE@", sAdozGiorno)
        End If
        fileText = fileText.Replace("@FIRMA@", If(sProt = "", "", "FIRMATA IN ORIGINALE"))
        '--
        If sProt <> "" Then
            Dim s() As String = Split(sProt, "|")
            fileText = fileText.Replace("@NUMPROT@", s(0) & "/" & String.Format("{0:0000000}", CInt(s(1))))
            fileText = fileText.Replace("@DATAPROT@", String.Format("{0:dd/MM/yyyy}", CDate(s(3))))
        Else
            fileText = fileText.Replace("@NUMPROT@", If(sProt = "", "", sProt))
            fileText = fileText.Replace("@DATAPROT@", If(sProt = "", "", sProt))
        End If
        ' Data attuale se richiesta
        fileText = fileText.Replace("@DATAATTUALE@", Format(CDate(Date.Now()), "dd/MM/yyyy"))
        '--
        If InStr(fileText, "@RIGHETABLEDET@") > 0 Then Call GeneraTabellaDetermine(resolutions, fileText)
        '--
        'FileText = System.Web.HttpUtility.HtmlEncode(FileText)
    End Sub

    Private Shared Sub GeneraTabellaDetermine(ByVal resolutions As IList(Of ResolutionLetter), ByRef FileText As String)
        Dim Row As String = ""
        Dim sNum As String = ""
        Dim sControllo As String
        For Each resl As ResolutionLetter In resolutions
            sNum = String.Format("{0:0000000}", resl.Number) & "/" & UCase(resl.ProposerCode) & "/" & resl.Year.Value
            sControllo = ""
            If (resl.OCSupervisoryBoard = True) Then
                sControllo = "CS"
            End If
            If (resl.OCRegion = True) Then
                If sControllo <> "" Then sControllo &= " - "
                sControllo &= "R"
            End If
            If (resl.OCManagement = True) Then
                If sControllo <> "" Then sControllo &= " - "
                sControllo &= "CG"
            End If
            If (resl.OCOther = True) Then
                If sControllo <> "" Then sControllo &= " - "
                sControllo &= resl.OtherDescription
            End If
            Row &=
            "<tr style=""font-size: 10.0pt; font-family: Arial;"">" & vbNewLine &
                "<td align=center>" & sNum & "</td>" & vbNewLine &
                "<td align=center>" & resl.AdoptionDate.DefaultString() & "</td>" & vbNewLine &
                "<td>" & StringHelper.ReplaceCrLf("" & resl.ResolutionObject) & "</td>" & vbNewLine &
                "<td align=center>" & sControllo & "</td>" & vbNewLine &
            "</tr>" & vbNewLine
        Next
        FileText = FileText.Replace("@RIGHETABLEDET@", Row)
    End Sub

    Public Function GetPreviousSequenceAdoptionDate(ByVal idResolution As Integer, ByVal roleServiceCode As String, ByVal resolutionType As Short) As Date
        Return _dao.GetPreviousSequenceAdoptionDate(idResolution, roleServiceCode, resolutionType)
    End Function

    Public Function CheckPreviousResolutionAdoptionDateMinus(ByVal resolution As Resolution, ByVal currentAdoptionDate As DateTime, ByVal serviceNumber As String) As Boolean
        If resolution Is Nothing Then
            Exit Function
        End If

        Dim serviceCode As String = String.Empty
        Dim serviceParams() As String = serviceNumber.Split("/"c)
        If serviceParams.Count() > 1 Then
            serviceCode = String.Format("{0}%", serviceParams(0))
        End If

        Dim lastAdoptionDate As DateTime = GetPreviousSequenceAdoptionDate(resolution.Id, serviceCode, resolution.Type.Id)
        If lastAdoptionDate > currentAdoptionDate Then
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' Verifica se la resolution passata, in stato pubblicazione, contiene serie documentali in stato bozza
    ''' con richiesta documenti attiva
    ''' </summary>
    Public Function HasSeriesToComplete(ByVal resolution As Resolution) As Boolean
        If resolution Is Nothing Then
            Exit Function
        End If

        If Not GetSeriesToComplete(resolution).Any() Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Verifica se la resolution passata, in stato adozione e pubblicazione, contiene serie documentali confermate
    ''' </summary>
    Public Function HasSeriesComplete(ByVal resolution As Resolution) As Boolean
        If resolution Is Nothing Then
            Exit Function
        End If

        Return GetSeriesNotComplete(resolution).Any()

    End Function

    ''' <summary>
    ''' Verifica se la resolution passata, in stato pubblicazione, contiene serie documentali in stato bozza
    ''' con richiesta documenti attiva
    ''' </summary>
    ''' ' TODO: metodo da rivedere, nome da modificare
    Public Function GetSeriesToAVCP(ByVal resolution As Resolution) As ICollection(Of DocumentSeriesItem)

        If resolution Is Nothing OrElse resolution.ResolutionKind Is Nothing OrElse Not resolution.ResolutionKind.ResolutionKindDocumentSeries.Any() Then
            Return New Collection(Of DocumentSeriesItem)()
        End If

        Dim reslDocSeries As IList(Of ResolutionDocumentSeriesItem) = FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetByResolution(resolution)
        Dim idSeriesList As List(Of Integer) = reslDocSeries _
            .Where(Function(x) x.IdDocumentSeriesItem.HasValue) _
            .Select(Function(s) s.IdDocumentSeriesItem.Value) _
            .ToList()
        Return FacadeFactory.Instance.DocumentSeriesItemFacade.GetByIdentifiers(idSeriesList)
    End Function

    ''' <summary>
    ''' Verifica se la resolution passata, in stato pubblicazione, contiene serie documentali in stato bozza
    ''' con richiesta documenti attiva
    ''' </summary>
    Public Function GetSeriesToComplete(ByVal resolution As Resolution) As ICollection(Of DocumentSeriesItem)

        If resolution Is Nothing OrElse Not resolution.PublishingDate.HasValue OrElse
            resolution.ResolutionKind Is Nothing OrElse (Not resolution.ResolutionKind.ResolutionKindDocumentSeries.Any() AndAlso Not FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetByResolution(resolution).Any()) Then
            Return New Collection(Of DocumentSeriesItem)()
        End If

        Dim reslDocSeries As IList(Of ResolutionDocumentSeriesItem) = FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetByResolution(resolution)
        Dim idSeriesList As List(Of Integer) = reslDocSeries _
            .Where(Function(x) x.IdDocumentSeriesItem.HasValue) _
            .Select(Function(s) s.IdDocumentSeriesItem.Value) _
            .ToList()
        Return FacadeFactory.Instance.DocumentSeriesItemFacade.GetItemDraftByIdentifiers(idSeriesList)
    End Function

    ''' <summary>
    ''' Verifica se la resolution passata, in stato pubblicazione o adozione, contiene serie documentali confermate
    ''' con richiesta documenti attiva
    ''' </summary>
    Public Function GetSeriesNotComplete(ByVal resolution As Resolution) As ICollection(Of DocumentSeriesItem)

        If resolution Is Nothing OrElse Not resolution.PublishingDate.HasValue OrElse Not resolution.AdoptionDate.HasValue OrElse
            resolution.ResolutionKind Is Nothing OrElse Not resolution.ResolutionKind.ResolutionKindDocumentSeries.Any() Then
            Return New Collection(Of DocumentSeriesItem)()
        End If

        Dim reslDocSeries As IList(Of ResolutionDocumentSeriesItem) = FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetByResolution(resolution)
        Dim idSeriesList As List(Of Integer) = reslDocSeries _
            .Where(Function(x) x.IdDocumentSeriesItem.HasValue) _
            .Select(Function(s) s.IdDocumentSeriesItem.Value) _
            .ToList()
        Return FacadeFactory.Instance.DocumentSeriesItemFacade.GetItemNotDraftByIdentifiers(idSeriesList)
    End Function


    ''' <summary>
    ''' Recupera tutte le serie documentali associate all'atto, sia confermate sia in stato di bozza
    ''' </summary>
    Public Function GetSeriesByResolution(ByVal resolution As Resolution) As ICollection(Of DocumentSeriesItem)

        If resolution Is Nothing Then
            Return New Collection(Of DocumentSeriesItem)()
        End If

        Dim reslDocSeries As IList(Of ResolutionDocumentSeriesItem) = FacadeFactory.Instance.ResolutionDocumentSeriesItemFacade.GetByResolution(resolution)
        If reslDocSeries IsNot Nothing Then
            Dim idSeriesList As List(Of Integer) = reslDocSeries _
            .Where(Function(x) x.IdDocumentSeriesItem.HasValue) _
            .Select(Function(s) s.IdDocumentSeriesItem.Value) _
            .ToList()
            Return FacadeFactory.Instance.DocumentSeriesItemFacade.GetByIdentifiers(idSeriesList)
        Else
            Return New Collection(Of DocumentSeriesItem)()
        End If

    End Function


    ''' <summary>
    ''' Dto dei documenti principali
    ''' </summary>
    Public Function GetResolutionMainDocumentDtos(resolution As Resolution, documentGroup As String, incremental As Short) As ICollection(Of ResolutionDocument)
        Dim tab As TabWorkflow = FacadeFactory.Instance.TabWorkflowFacade.GetTabWorkflow(resolution, incremental)
        Dim docs() As BiblosDocumentInfo = GetDocuments(resolution, incremental)

        If Not docs.Any() Then
            Return New Collection(Of ResolutionDocument)
        End If

        Dim dtos As ICollection(Of ResolutionDocument) = GetResolutionDocumentToDtos(docs, resolution, documentGroup, incremental)
        Return dtos
    End Function

    ''' <summary>
    ''' Dto dei documenti principale omissis
    ''' </summary>
    Public Function GetResolutionDocumentOmissisDtos(resolution As Resolution, documentGroup As String, incremental As Short) As ICollection(Of ResolutionDocument)
        Dim docs() As BiblosDocumentInfo = GetDocumentsOmissis(resolution, incremental, False)

        If Not docs.Any() Then
            Return New Collection(Of ResolutionDocument)
        End If

        Dim dtos As ICollection(Of ResolutionDocument) = GetResolutionDocumentToDtos(docs, resolution, documentGroup, incremental)
        Return dtos
    End Function

    ''' <summary>
    ''' Dto degli allegati
    ''' </summary>
    Public Function GetResolutionAttachmentDtos(resolution As Resolution, documentGroup As String, incremental As Short) As ICollection(Of ResolutionDocument)
        Dim docs() As BiblosDocumentInfo = GetAttachments(resolution, incremental, False)

        If Not docs.Any() Then
            Return New Collection(Of ResolutionDocument)
        End If

        Dim dtos As ICollection(Of ResolutionDocument) = GetResolutionDocumentToDtos(docs, resolution, documentGroup, incremental)
        Return dtos
    End Function

    ''' <summary>
    ''' Dto degli allegati omissis
    ''' </summary>
    Public Function GetResolutionAttachmentOmissisDtos(resolution As Resolution, documentGroup As String, incremental As Short) As ICollection(Of ResolutionDocument)
        Dim docs() As BiblosDocumentInfo = GetAttachmentsOmissis(resolution, incremental, False)

        If Not docs.Any() Then
            Return New Collection(Of ResolutionDocument)
        End If

        Dim dtos As ICollection(Of ResolutionDocument) = GetResolutionDocumentToDtos(docs, resolution, documentGroup, incremental)
        Return dtos
    End Function

    ''' <summary>
    ''' Dto degli annessi
    ''' </summary>
    Public Function GetResolutionAnnexesDtos(resolution As Resolution, documentGroup As String, incremental As Short) As ICollection(Of ResolutionDocument)
        Dim docs() As BiblosDocumentInfo = GetAnnexes(resolution, incremental)

        If Not docs.Any() Then
            Return New Collection(Of ResolutionDocument)
        End If

        Dim dtos As ICollection(Of ResolutionDocument) = GetResolutionDocumentToDtos(docs, resolution, documentGroup, incremental)
        Return dtos
    End Function

    ''' <summary>
    ''' Esegue la mappatura di una nuova collezione ResolutionDocument
    ''' </summary>
    Protected Function GetResolutionDocumentToDtos(docs() As BiblosDocumentInfo, resolution As Resolution, documentGroup As String, incremental As Short) As ICollection(Of ResolutionDocument)
        Dim dtos As ICollection(Of ResolutionDocument) = New Collection(Of ResolutionDocument)
        For Each documentInfo As BiblosDocumentInfo In docs
            Dim dto As ResolutionDocument = New ResolutionDocument()
            dto.BiblosSerializeKey = documentInfo.Serialized
            dto.DocumentGroup = documentGroup
            dto.DocumentName = documentInfo.Name
            dto.IdBiblosDocument = documentInfo.DocumentId
            dto.IdResolution = resolution.Id

            dtos.Add(dto)
        Next

        Return dtos
    End Function

    Public Function GetResolutionLocation(idResolution As Integer) As Location
        Return _dao.GetResolutionLocation(idResolution)
    End Function

    Public Function GetByYearAndNumber(year As Short, number As Integer) As Resolution
        Return _dao.GetByYearAndNumber(year, number)

    End Function

    Public Function GetByUniqueId(uniqueId As Guid) As Resolution
        Return _dao.GetByUniqueId(uniqueId)
    End Function

    Public Function GetByYearNumberType(year As Short, number As Integer, type As Short) As Resolution
        Return _dao.GetByYearNumberType(year, number, type)
    End Function

    Public Sub SendCreateResolutionCommand(resolution As Resolution)
        Try
            Dim commandCreate As ICommandCreateResolution = PrepareResolutionCommand(Of ICommandCreateResolution)(resolution, Function(tenantName, tenantId, tenantAOOId, identity, resolutionModel, apiCategoryFascicle, documentUnit)
                                                                                                                                  Return New CommandCreateResolution(tenantName, tenantId, tenantAOOId, identity, resolutionModel, apiCategoryFascicle, documentUnit)
                                                                                                                              End Function)
            CommandCreateFacade.Push(commandCreate)
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendCreateResolutionCommand => ", ex.Message), ex)
        End Try
    End Sub

    Public Sub SendUpdateResolutionCommand(resolution As Resolution)
        Try
            If resolution IsNot Nothing AndAlso resolution.AdoptionDate.HasValue Then
                Dim commandUpdate As ICommandUpdateResolution = PrepareResolutionCommand(Of ICommandUpdateResolution)(resolution, Function(tenantName, tenantId, tenantAOOId, identity, resolutionModel, apiCategoryFascicle, documentUnit)
                                                                                                                                      Return New CommandUpdateResolution(tenantName, tenantId, tenantAOOId, identity, resolutionModel, apiCategoryFascicle, documentUnit)
                                                                                                                                  End Function)
                CommandUpdateFacade.Push(commandUpdate)
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("SendUpdateResolutionCommand => ", ex.Message), ex)
        End Try
    End Sub

    Public Function PrepareResolutionCommand(Of T As {ICommand})(resolution As Resolution,
                                                                  commandInitializeFunc As Func(Of String, Guid, Guid, IdentityContext, ResolutionModel, APICommons.CategoryFascicle, Entity.DocumentUnits.DocumentUnit, T)) As T

        Dim identity As IdentityContext = New IdentityContext(DocSuiteContext.Current.User.FullUserName)
        Dim tenantName As String = CurrentTenant.TenantName
        Dim tenantId As Guid = CurrentTenant.UniqueId
        Dim tenantAOOId As Guid = CurrentTenant.TenantAOO.UniqueId
        Dim resolutionModel As ResolutionModel = MapperResolutionModel.MappingDTO(resolution)

        If resolution.Type.Id.Equals(ResolutionType.IdentifierDetermina) Then
            resolutionModel.DocumentUnitName = Factory.ResolutionTypeFacade.DeterminaCaption
        ElseIf resolution.Type.Id.Equals(ResolutionType.IdentifierDelibera) Then
            resolutionModel.DocumentUnitName = Factory.ResolutionTypeFacade.DeliberaCaption
        End If

        Dim dswCategoryFascicle As CategoryFascicle = CategoryFascicleDao.GetByIdCategory(resolution.Category.Id, DSWEnvironment.Protocol)
        Dim apiCategoryFascicle As APICommons.CategoryFascicle = Nothing
        If dswCategoryFascicle IsNot Nothing Then
            apiCategoryFascicle = MapperCategoryFascicle.MappingDTO(dswCategoryFascicle)
        End If

        Return commandInitializeFunc(tenantName, tenantId, tenantAOOId, identity, resolutionModel, apiCategoryFascicle, Nothing)
    End Function

    Public Function CountResolutionKind(ReslKind As ResolutionKind) As Integer
        Return _dao.CountResolutionKind(ReslKind)
    End Function

#End Region

#Region " IsManagedProperty Methods "

    ''' <summary> Recupera ManagedData dal tipo atto e verifica se è contenuta la chiave specificata. </summary>
    ''' <param name="fieldName">Nome della chiave.</param>
    ''' <param name="resolutionType">Tipo atto di cui recuperare il ManagedData.</param>
    ''' <remarks>Esegue un accesso al database per recuperare ManagedData.</remarks>
    Public Function IsManagedProperty(ByVal fieldName As String, ByVal resolutionType As Short) As Boolean
        ' TODO: spostare nella tabmasterfacade
        Dim managedData As String = Factory.TabMasterFacade.GetFieldValue(TabMasterFacade.ManagedDataField, DocSuiteContext.Current.ResolutionEnv.Configuration, resolutionType)
        Return IsManagedPropertyByString(managedData, fieldName)
    End Function

    ''' <summary>
    ''' Verifica se nel ManagedData ricevuto come parametro è contenuta la chiave specificata.
    ''' </summary>
    ''' <param name="managedData">ManagedData in cui eseguire la verifica.</param>
    ''' <param name="fieldName">Nome della chiave.</param>
    Public Function IsManagedPropertyByString(managedData As String, fieldName As String) As Boolean
        ' TODO: spostare nella tabmasterfacade
        Return IsManagedPropertyByString(managedData, fieldName, Nothing)
    End Function

    ''' <summary>
    ''' Recupera ManagedData dal tipo atto e verifica se è contenuta la chiave e tutte le proprietà specificate.
    ''' </summary>
    ''' <param name="fieldName">Nome della chiave.</param>
    ''' <param name="resolutionType">Tipo atto di cui recuperare il ManagedData.</param>
    ''' <param name="fieldProperties">Elenco di proprietà.</param>
    ''' <remarks>Esegue un accesso al database per recuperare ManagedData.</remarks>
    Public Function IsManagedProperty(ByVal fieldName As String, ByVal resolutionType As Short, ParamArray fieldProperties() As String) As Boolean
        ' TODO: spostare nella tabmasterfacade
        Dim managedData As String = Factory.TabMasterFacade.GetFieldValue(TabMasterFacade.ManagedDataField, DocSuiteContext.Current.ResolutionEnv.Configuration, resolutionType)
        Return IsManagedPropertyByString(managedData, fieldName, fieldProperties)
    End Function

    ''' <summary> Verifica se nel ManagedData ricevuto come parametro è contenuta la chiave e tutte le proprietà specificate. </summary>
    ''' <param name="managedData">ManagedData in cui eseguire la verifica.</param>
    ''' <param name="fieldName">Nome della chiave.</param>
    ''' <param name="fieldProperties">Elenco di proprietà.</param>
    Public Function IsManagedPropertyByString(managedData As String, fieldName As String, ParamArray fieldProperties() As String) As Boolean
        ' TODO: spostare nella tabmasterfacade
        Dim managedDataList As SortedList(Of String, String) = GetManagedDataList(managedData)
        Dim loweredName As String = fieldName.ToLowerInvariant().Trim()
        If (managedDataList Is Nothing) OrElse Not managedDataList.ContainsKey(loweredName) Then
            ' La lista non contiene la chiave
            Return False
        End If

        If fieldProperties.IsNullOrEmpty() Then
            ' La lista contiene la chiave e non è richiesta nessuna proprietà
            Return True
        End If

        ' La lista contiene la chiave e il valore contiene tutte le proprietà richieste
        Dim fieldValue As String = managedDataList.Item(loweredName)
        Dim retval As Boolean = True
        For Each item As String In fieldProperties
            ' TODO: veramente se ho 4 proprietà e la 5 è vuota torno true? mah
            If String.IsNullOrEmpty(item) Then
                Return True
            End If
            ' Verifica che un valore di una chiave di ManagedData contenga la proprietà specificata
            retval = retval AndAlso (fieldValue.ContainsIgnoreCase(String.Format(".{0}.", item)) OrElse fieldValue.ContainsIgnoreCase(String.Format(".{0}-", item)))
        Next
        Return retval
    End Function

    ''' <summary> Trasforma la stringa ManagedData in una lista. </summary>
    Private Shared Function GetManagedDataList(managedData As String) As SortedList(Of String, String)
        ' TODO: spostare nella tabmasterfacade
        Dim propertyList As New SortedList(Of String, String)
        If String.IsNullOrEmpty(managedData) Then
            Return propertyList
        End If

        Dim dataParts As New List(Of String)(managedData.Split("|"c))
        For Each keyValue As String In dataParts
            Dim keyName As String = keyValue.Split("["c)(0).Trim().ToLowerInvariant()
            propertyList.Add(keyName, keyValue)
        Next
        If propertyList.Count = 0 Then
            Throw New NullReferenceException("Impossibile recuperare ManagedData.")
        End If

        Return propertyList
    End Function

    Public Function HasAnnexed(resolution As Resolution) As Boolean
        If resolution.File Is Nothing Then
            Throw New DocSuiteException(String.Format("Nessun file associato alla resolution {0}", resolution.Id))
        End If

        Dim exists As Boolean = Not resolution.File.IdAnnexes.Equals(Guid.Empty)
        Return exists
    End Function

    Public Sub FlushAnnexed(ByVal resolution As Resolution)
        If Not HasAnnexed(resolution) Then
            FileLogger.Warn(LoggerName, String.Format("Nessun annesso da eliminare per la resolution {0}", resolution.Id))
            Exit Sub
        End If

        resolution.File.IdAnnexes = Guid.Empty
        MyBase.UpdateOnly(resolution)

        Const message As String = "Catena Annessi svuotata."
        FacadeFactory.Instance.ResolutionLogFacade.Insert(resolution, ResolutionLogType.RM, message)
        FacadeFactory.Instance.ResolutionFacade.SendUpdateResolutionCommand(resolution)
    End Sub

#End Region

#Region " Functions: Workflow "

    Private Function GetCustomWorkflowString(ByVal source As String, ByVal defaultValue As String) As String
        If CustomWorkflowXml IsNot Nothing Then
            Try
                Dim nodes As XmlNodeList = CustomWorkflowXml.GetElementsByTagName(source)
                If nodes.Count > 0 Then
                    Return nodes(0).InnerText
                End If
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
            End Try
        End If
        Return defaultValue
    End Function

    Private Function GetWorkflowStringStandard(type As String, header As ResolutionHeader) As String
        Select Case True
            Case header.LeaveDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("AVVENUTAPUBBLICAZIONE", "Avvenuta Pubblicazione")
                End If
                Return header.LeaveDate.ToString()

            Case header.EffectivenessDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("ESECUTIVA", "Esecutiva")
                End If
                Return header.EffectivenessDate.ToString()

            Case header.ResponseDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("RISPOSTAOC", "Risposta OC")
                End If
                Return header.ResponseDate.ToString()

            Case header.WaitDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("SCADENZAOC", "Scadenza OC")
                End If
                Return header.WaitDate.ToString()

            Case header.ConfirmDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("RICEZIONE", "Ricezione")
                End If
                Return header.ConfirmDate.ToString()

            Case header.WarningDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("SPEDIZIONE", "Spedizione")
                End If
                Return header.WarningDate.ToString()

            Case header.PublishingDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("PUBBLICATA", "Pubblicata")
                End If
                Return header.PublishingDate.ToString()

            Case header.AdoptionDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("ADOTTATA", "Adottata")
                End If
                Return header.AdoptionDate.ToString()

            Case header.SupervisoryBoardWarningDate.HasValue
                If type.Equals("W") Then
                    Return GetCustomWorkflowString("SPEDCS", "Sped. CS")
                End If
                Return header.SupervisoryBoardWarningDate.ToString()

            Case Else
                If type.Equals("W") Then
                    If IsManagedPropertyByString(header.ManagedData, "idProposalFile", Nothing) Then
                        Return GetCustomWorkflowString("PROPOSTA", "Proposta")
                    End If
                    If Not header.AdoptionDate.HasValue AndAlso Not DocSuiteContext.Current.ResolutionEnv.Configuration.Equals("ASL3-TO") Then
                        Return GetCustomWorkflowString("PROPOSTA", "Proposta")
                    End If
                    Return Factory.ResolutionTypeFacade.GetDescription(header.Type)
                End If
                If header.ProposeDate.HasValue Then
                    Return header.ProposeDate.ToString()
                End If
        End Select
        Return String.Empty
    End Function

    Private Function GetWorkflowStringTo(type As String, header As ResolutionHeader) As String
        Select Case True
            Case header.EffectivenessDate.HasValue
                If type.Eq("W") Then
                    Return GetCustomWorkflowString("ESECUTIVA", "Esecutiva")
                End If
                Return header.EffectivenessDate.ToString()

            Case header.PublishingDate.HasValue
                If type.Eq("W") Then
                    Return GetCustomWorkflowString("PUBBLICATA", "Pubblicata")
                End If
                Return header.PublishingDate.ToString()

            Case header.SupervisoryBoardWarningDate.HasValue
                If type.Eq("W") Then
                    Return GetCustomWorkflowString("SPEDCS", "Sped. CS")
                End If
                Return header.SupervisoryBoardWarningDate.ToString()

            Case header.AdoptionDate.HasValue
                If type.Eq("W") Then
                    Return GetCustomWorkflowString("ADOTTATA", "Adottata")
                End If
                Return header.AdoptionDate.ToString()

            Case Else
                If type.Eq("W") Then
                    If IsManagedPropertyByString(header.ManagedData, "idProposalFile", Nothing) Then
                        Return GetCustomWorkflowString("PROPOSTA", "Proposta")
                    End If
                    Return Factory.ResolutionTypeFacade.GetDescription(header.Type)
                End If
                If header.ProposeDate.HasValue Then
                    Return header.ProposeDate.ToString()
                End If
        End Select
        Return String.Empty
    End Function

    Public Function GetWorkflowString(type As String, header As ResolutionHeader, useTabWorkflow As Boolean) As String
        If useTabWorkflow Then
            Return Factory.TabWorkflowFacade.GetDescriptionByResolution(header.Id)
        End If
        Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
            Case "ASL3-TO", "AUSL-PC"
                Return GetWorkflowStringTo(type, header)
            Case Else
                Return GetWorkflowStringStandard(type, header)
        End Select
    End Function

    Public Function GetWorkflowString(type As String, header As ResolutionHeader) As String
        Return GetWorkflowString(type, header, False)
    End Function

    Public Function StartLetterProcess(ByVal document As DocumentInfo, ByVal resolutionList As IList(Of Resolution), ByVal signers As IList(Of CollaborationContact), ByVal subject As String, ByVal Action As TOWorkflow) As Integer
        'Creo l'oggetto collaborazioneXML
        Dim xmlColl As New CollaborationXML()

        'Aggiungo il destinatario
        xmlColl.Signers = signers.Select(Function(s) New Signer() With {.UserName = s.Account}).ToList()
        'Definisco che è una collaborazione di protocollo
        xmlColl.Type = CollaborationDocumentType.P.ToString()
        xmlColl.Priority = "N"
        xmlColl.Subject = subject

        'Definisco il comportamento che dovrà avere al termine della protocollazione
        xmlColl.Attributes = New List(Of CollaborationXmlData)(
            {
                New ResolutionXML() With
                {
                    .ResolutionsToUpdate = (From resolution In resolutionList Select resolution.Id).ToList(),
                    .UpdateCollegioSindacaleProtocolLink = Action = TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale,
                    .UpdateInvioPubbLetteraFirmaDigitaleProtocolLink = Action = TOWorkflow.RicercaFlussoPubblicazione
                }
            }
        )

        'Inserisco la collaborazione
        Dim idCollaboration As Integer = Factory.CollaborationFacade.InsertCollaboration(xmlColl, DocSuiteContext.Current.User.FullUserName)
        Factory.CollaborationFacade.BiblosInsert(idCollaboration, document.Stream, document.PDFName, DocSuiteContext.Current.User.FullUserName)
        'Attivo la collaborazione
        Factory.CollaborationFacade.StartCollaboration(idCollaboration)

        '' Aggiorno gli atti con un puntatore alla collaborazione
        For Each resolution As Resolution In resolutionList
            If Action = TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale Then
                resolution.SupervisoryBoardProtocolCollaboration = idCollaboration
            End If
            If Action = TOWorkflow.RicercaFlussoPubblicazione Then
                resolution.SupervisoryBoardProtocolCollaboration = idCollaboration
            End If
            Update(resolution)
        Next

        Return idCollaboration
    End Function

#End Region

#Region " Web Publications "
    ''' <summary>
    ''' Metodo di inserimento di nuovo record con definizione degli status da ignorare
    ''' </summary>
    Public Function GetLastOrNewPublicationNumber(resl As Resolution, statusToIgnore As List(Of Integer)) As String
        Dim wpf As New WebPublicationFacade()
        If wpf.Exists(resl, statusToIgnore) Then
            Return wpf.GetByResolution(resl, statusToIgnore)(0).ExternalKey.ToString()
        End If

        ' titolo atto
        Dim titolo As String = String.Format("{0} {1} del {2:d/M/yyyy}", resl.Type.Description, resl.InclusiveNumber, resl.AdoptionDate)

        ' Inserimento Atto nel Web Service Pubblicazione
        Dim tipoDoc As String = If(resl.Type.Id = 1, "A", "B")
        Dim result As Long = ServiceWebPublication.Inserisci(tipoDoc, titolo, resl.AdoptionDate.Value, If(resl.ResolutionObjectPrivacy IsNot Nothing, resl.ResolutionObjectPrivacy, resl.ResolutionObject), 0)
        Dim wp As New WebPublication
        wp.Resolution = resl
        wp.Status = 0
        wp.ExternalKey = result.ToString()
        wpf.Save(wp)
        Return result.ToString()
    End Function

    ''' <summary>
    ''' Metodo standard di inserimento di nuovo record di pubblicazione
    ''' </summary>
    Public Function GetLastOrNewPublicationNumber(resl As Resolution) As Long
        Dim statusToIgnore As New List(Of Integer)
        statusToIgnore.Add(2)
        Return Long.Parse(GetLastOrNewPublicationNumber(resl, statusToIgnore))
    End Function

    Public Function GetLastRevokedNumber(resl As Resolution) As String
        Dim wpf As New WebPublicationFacade()
        Return wpf.GetLastRevokedNumber(resl)
    End Function

    ''' <summary> Ritorna il documento da pubblicare utilizzabile per la stampa </summary>
    ''' <param name="resl">Atto</param>
    ''' <returns>Nome del file nella temp da pubblicare</returns>
    ''' <remarks>Nato per stampare il documento a cui apporre gli omissis</remarks>
    Public Function GetPublicationDocument(resl As Resolution, ByVal addSignature As Boolean) As FileInfo
        Dim fileFacade As New FileResolutionFacade()
        Dim fileResolution As FileResolution = fileFacade.GetByResolution(resl)(0)

        Dim doc As DocumentInfo = New BiblosDocumentInfo(resl.Location.ReslBiblosDSDB, fileResolution.IdFrontespizio.Value, 0)
        Dim frontespizioInfo As FileInfo = BiblosFacade.SaveUniquePdfToTemp(doc)

        Return GetPublicationDocument(resl, New FileDocumentInfo(frontespizioInfo), addSignature)
    End Function

    ''' <summary> Ritorna il documento da pubblicare utilizzabile per la stampa. </summary>
    ''' <param name="resl">Atto</param>
    ''' <param name="frontespizio">Nome del file del frontespizio nella temp</param>
    ''' <returns>Nome del file nella temp da pubblicare</returns>
    ''' <remarks>Nato per stampare il documento a cui apporre gli omissis</remarks>
    Public Function GetPublicationDocument(resl As Resolution, frontespizio As DocumentInfo, ByVal addSignature As Boolean) As FileInfo
        Dim fr As FileResolution = Factory.FileResolutionFacade.GetByResolution(resl)(0)
        If Not fr.IdResolutionFile.HasValue OrElse Not fr.IdAssumedProposal.HasValue OrElse frontespizio Is Nothing Then
            Throw New ArgumentException("Impossibile creare il documento di pubblicazione.")
        End If

        ' Recupero i documenti da fondere.
        Dim documents As New List(Of DocumentInfo)
        '' Aggiungo la relata di adozione
        documents.Add(New BiblosDocumentInfo(resl.Location.ReslBiblosDSDB, fr.IdResolutionFile.Value, 0))
        '' Aggiungo il documento adottato
        documents.Add(New BiblosDocumentInfo(resl.Location.ReslBiblosDSDB, fr.IdAssumedProposal.Value, 0))
        '' Aggiungo gli allegati (se presenti)
        If fr.IdAttachements.HasValue AndAlso fr.IdAttachements > 0 Then
            documents.AddRange(BiblosDocumentInfo.GetDocuments(resl.Location.ReslBiblosDSDB, fr.IdAttachements.Value))
        End If
        '' Aggiungo il frontalino di pubblicazione
        documents.Add(frontespizio)

        ' Fondo in un unico documento le catene senza aggiungere 
        Dim pdfManager As New PDF.PdfMerge()
        For Each item As DocumentInfo In documents
            pdfManager.AddDocument(item.GetPdfStreamNoSignature())
        Next
        Dim mergedFileName As String = FileHelper.UniqueFileNameFormat("DocumentoDaPubblicare.pdf", DocSuiteContext.Current.User.UserName)
        Dim mergedTempPath As String = CommonUtil.GetInstance().AppTempPath & mergedFileName
        pdfManager.Merge(mergedTempPath)

        Dim mergedDocumentNoSignature As FileInfo = New FileInfo(mergedTempPath)

        If Not addSignature Then Return mergedDocumentNoSignature

        Dim fdi As New FileDocumentInfo(mergedDocumentNoSignature)
        fdi.Signature = ComposeWebPublicationSignature(resl)

        Return fdi.SavePdf(New DirectoryInfo(CommonUtil.GetInstance().AppTempPath), String.Empty)
    End Function

    ''' <summary> Pubblicazione Web. </summary>
    ''' <param name="resl">Atto da pubblicare</param>
    Public Sub DoWebPublication(ByVal resl As Resolution, ByVal addWatermark As Boolean, ByVal publishingDocument As DocumentInfo, ByVal isPrivacy As Boolean)
        Dim watermark As String = If(addWatermark, DocSuiteContext.Current.ResolutionEnv.WebPublishWatermark, String.Empty)
        publishingDocument.Signature = ComposeWebPublicationSignature(resl)
        Dim finalDocument As FileInfo = publishingDocument.SavePdf(New DirectoryInfo(DocSuiteContext.Current.ProtocolEnv.VersioningShare), FileHelper.UniqueFileNameFormat("PubblicazioneDoc.pdf", DocSuiteContext.Current.User.UserName), watermark)

        Dim l As Long = GetLastOrNewPublicationNumber(resl)
        'Copio il file su di una directory apposita
        If finalDocument.Exists Then
            ' Pubblicazione vera e propria del documento passando il relativop path al web service
            ServiceWebPublication.PubblicaPath(finalDocument.FullName, l, 0, DateTime.Now, If(resl.ResolutionObjectPrivacy IsNot Nothing, resl.ResolutionObjectPrivacy, resl.ResolutionObject))

            ' Salvo i dati in DSW
            Dim statusToExclude As New List(Of Integer)
            statusToExclude.Add(1)
            statusToExclude.Add(2)
            Dim wpf As New WebPublicationFacade()
            Dim savedDocument As BiblosDocumentInfo
            For Each wp As WebPublication In wpf.GetByResolution(resl, statusToExclude)
                wp.Status = 1
                wp.Location = resl.Location

                Dim doc As New FileDocumentInfo(finalDocument)
                doc.Name = "DocumentoPubblicato.pdf"
                doc.Signature = doc.Name
                doc.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, "0")
                savedDocument = doc.ArchiveInBiblos(resl.Location.ReslBiblosDSDB)
                wp.IDDocument = savedDocument.BiblosChainId

                If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                    FacadeFactory.Instance.ResolutionLogFacade.Insert(resl, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", savedDocument.Name, savedDocument.DocumentId))
                End If

                wp.EnumDocument = 0
                wp.Descrizione = "Documento pubblicato"
                wp.IsPrivacy = isPrivacy
                wpf.Update(wp)
            Next

            'Elimino il file pubblicato
            finalDocument.Delete()
        End If
    End Sub

    ''' <summary> Genera il documento da pubblicare e lo pubblica. </summary>
    ''' <param name="resl">Atto</param>
    ''' <param name="frontespizio">Nome del file del frontespizio nella temp</param>
    ''' <remarks>Nato per stampare il documento a cui apporre gli omissis</remarks>
    Public Sub DoWebPublication(resl As Resolution, frontespizio As DocumentInfo, revoca As Boolean)
        '' Effettua la pubblicazione
        If revoca Then
            DoWebPublication(resl, True, frontespizio, False)
        Else
            DoWebPublication(resl, True, New FileDocumentInfo(GetPublicationDocument(resl, frontespizio, False)), False)
        End If

    End Sub

    Public Sub DoWebRevoca(resl As Resolution)
        EndWebRevoca(resl, StartWebRevoca(resl))
    End Sub

    Public Function StartWebRevoca(resl As Resolution) As Long
        'Effettuo soltanto la prenotazione di un eventuale nuovo numero di pubblicazione
        Return Long.Parse(GetLastOrNewPublicationNumber(resl, New List(Of Integer)(New Integer() {1, 2})))
    End Function

    Public Sub EndWebRevoca(resl As Resolution, idReservedPublication As Long)
        ServiceWebPublication.Revoca(idReservedPublication)

        ' Salvo i dati in DSW
        Dim wpf As New WebPublicationFacade()
        Dim wp As WebPublication = wpf.GetByResolution(resl)(0)
        wp.Status = 2
        wpf.Update(wp)
    End Sub

    ''' <summary> Compongo signature/watermark/overprint </summary>
    ''' <param name="resl">Atto con cui comporre</param>
    ''' <remarks>
    ''' Copiata dal metodo ParseString di VecompSoftware.DocSuiteWeb.Gui\Resl\ReslPubblicaWeb.aspx.vb
    ''' TODO: valutare se possibile usare questa anche li
    ''' </remarks>
    Public Function ComposeWebPublicationSignature(ByVal resl As Resolution) As String
        Return String.Format("{0} {1} del {2}", resl.Type.Description, GetResolutionNumber(resl, False), resl.AdoptionDate.DefaultString())
    End Function

    Public Sub DoGenericWebPublication(resl As Resolution, publicationNumber As String, isPrivacy As Boolean)
        DoGenericWebPublication(resl, publicationNumber, 0, 0, isPrivacy)
    End Sub

    ''' <summary>
    ''' Pubblicazione Web Generica, richiede la presenza della tabella WebPublication in Atti
    ''' </summary>
    ''' <param name="resl">Atto da pubblicare</param>
    Public Sub DoGenericWebPublication(resl As Resolution, publicationNumber As String, idBiblos As Integer, enumBiblos As Integer, isPrivacy As Boolean)
        ' Salvo i dati in DSW
        Dim wpf As New WebPublicationFacade()
        ' Creo un nuovo record
        Dim wp As WebPublication = New WebPublication()
        wp.Resolution = resl
        ' Numero di pubblicazione
        wp.ExternalKey = publicationNumber
        wp.Status = 1 ' Pubblicato
        wp.Location = resl.Location
        wp.RegistrationDate = DateTimeOffset.UtcNow
        ' Utente corrente
        wp.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        'Registro l'idBiblos
        wp.IDDocument = idBiblos
        wp.EnumDocument = enumBiblos
        wp.IsPrivacy = isPrivacy
        wpf.Save(wp)
    End Sub

    Public Function IsPublicated(resl As Resolution) As Boolean
        Dim wpf As New WebPublicationFacade()
        Dim wp As IList(Of WebPublication) = wpf.GetByResolution(resl)
        Dim retVal As Boolean = wp IsNot Nothing AndAlso wp.Count > 0
        Return retVal
    End Function

    ''' <summary> Metodo che calcola la signature di un Atto. Usato soprattutto per ASMN-RE </summary>
    Public Function ResolutionSignature(resolution As Resolution, documentType As ResolutionType.UploadDocumentType) As String
        Dim signature As String = "*"
        signature = Factory.ResolutionTypeFacade.GetDescription(resolution.Type)
        If resolution.Type.Id = 1 Then
            signature &= String.Format(" {0}/{1} del {2}", resolution.Year, Format(CLng(resolution.ServiceNumber), "0000000"), resolution.AdoptionDate.DefaultString())
        Else
            signature &= String.Format(" {0}/{1} del {2}", resolution.Year, Format(CLng(resolution.Number), "0000000"), resolution.AdoptionDate.DefaultString())
        End If

        Select Case documentType
            Case ResolutionType.UploadDocumentType.Allegato
                signature &= " (Allegato)"
            Case ResolutionType.UploadDocumentType.AllegatoRiservato
                signature &= " (Allegato Riservato)"
            Case ResolutionType.UploadDocumentType.Frontespizio
                signature &= " (Frontespizio)"
            Case ResolutionType.UploadDocumentType.Pubblicazione
                signature &= " " & DocSuiteContext.Current.ResolutionEnv.PubblicazioneOnLineSignature
        End Select

        Return signature
    End Function

    ''' <summary>
    ''' Metodo per effettuare l'avanzamento automatico con generazione di frontalino di ritiro per il workflow atti ASMN-RE
    ''' </summary>
    ''' <param name="resl">Resolution corrente</param>
    ''' <param name="retireDate">Data di ritiro</param>
    ''' <param name="actualStep">Step corrente in cui si trova l'atto (ovvero quello precedente al ritiro)</param>
    Public Function DoRetirePublicationStep(resl As Resolution, retireDate As Date, actualStep As TabWorkflow) As DocumentInfo
        ''Genero il frontalino di ritiro
        Dim retireStep As TabWorkflow = Nothing
        Factory.TabWorkflowFacade.GetByStep(resl.WorkflowType, CType((actualStep.Id.ResStep + 1), Short), retireStep)

        Dim fi As FileInfo = ResolutionUtil.GeneraFrontalino(retireDate, resl, retireStep.Description, actualStep.Id.ResStep, String.Empty)
        Dim frontalinoFileDocumentInfo As New FileDocumentInfo(fi)

        Dim signature As String = Factory.ResolutionTypeFacade.GetDescription(resl.Type)
        signature &= String.Format(" {0} del {1} (Frontespizio)", resl.InclusiveNumber, resl.AdoptionDate.DefaultString())
        frontalinoFileDocumentInfo.Signature = signature

        frontalinoFileDocumentInfo.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, "0")

        'Aggiungo il frontalino a Biblos e aggiorno la tabella FileResolution
        Dim frontalinoBiblos As BiblosDocumentInfo = frontalinoFileDocumentInfo.ArchiveInBiblos(resl.Location.ReslBiblosDSDB)
        Dim idFrontalino As Integer = frontalinoBiblos.BiblosChainId

        If DocSuiteContext.Current.PrivacyLevelsEnabled Then
            FacadeFactory.Instance.ResolutionLogFacade.Insert(resl, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", frontalinoBiblos.Name, frontalinoBiblos.DocumentId))
        End If

        SqlResolutionDocumentUpdate(resl.Id, idFrontalino, DirectCast([Enum].Parse(GetType(DocType), retireStep.FieldFrontalinoRitiro), DocType))

        'Avanzamento di step
        SqlResolutionUpdateWorkflowData(resl.Id, resl.Type.Id, Not resl.Number.HasValue, retireStep, retireDate.ToString("dd/MM/yyyy"), "N", -1, -1, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)
        Factory.ResolutionWorkflowFacade.InsertNextStep(resl.Id, actualStep.Id.ResStep, 0, 0, 0, Guid.Empty, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)

        'Invio comando di aggiornamento Resolution alle WebApi
        Factory.ResolutionFacade.SendUpdateResolutionCommand(resl)
        Return frontalinoFileDocumentInfo
    End Function


    Public Sub PublicateResolution(currentResl As Resolution, ByVal signedDocInfos As IList(Of DocumentInfo), ByVal NumServ As String, ByVal userAction As String, Optional tempPath As String = "")
        ' Recupero lo step corrente
        Dim currentStep As TabWorkflow = FacadeFactory.Instance.TabWorkflowFacade.GetByResolution(currentResl.Id)

        Select Case currentStep.Description
            Case WorkflowStep.ADOZIONE
                Dim nextStep As TabWorkflow = Nothing
                'Genero lo step di Pubblicazione ed Esecutività
                Factory.TabWorkflowFacade.GetByStep(currentResl.WorkflowType, currentStep.Id.ResStep + 1S, nextStep)
                AuslREResolutionFacade.ForwardWorkflow(currentResl, nextStep, DateTime.Now, NumServ, signedDocInfos, Nothing, Nothing, userAction, tempPath)

                Dim nextStep2 As TabWorkflow = Nothing
                Factory.TabWorkflowFacade.GetByStep(currentResl.WorkflowType, nextStep.Id.ResStep + 1S, nextStep2)
                AuslREResolutionFacade.ForwardWorkflow(currentResl, nextStep2, DateTime.Now, NumServ, Nothing, Nothing, Nothing, userAction, tempPath)

            Case WorkflowStep.PUBBLICAZIONE
                'Genero lo step di Esecutività
                Factory.TabWorkflowFacade.GetByStep(currentResl.WorkflowType, currentStep.Id.ResStep + 1S, currentStep)
                AuslREResolutionFacade.ForwardWorkflow(currentResl, currentStep, DateTime.Now, NumServ, Nothing, Nothing, Nothing, userAction, tempPath)
            Case WorkflowStep.ESECUTIVA
                'Significa che la resolution processata è già stata pubblicata
                Exit Sub
            Case Else
                Throw New Exception(String.Format("SinglePublicate - Tipologia di workflow non riconosciuta per la resolution [{0}].", currentResl.Id))
        End Select
    End Sub

    Public Function GetPresentDocumentSigners(resolution As Resolution, tabWorkflow As TabWorkflow, idDocument As Integer) As IList(Of String)
        Dim idDoc As Integer = IIf(idDocument <> -1, idDocument, CType((0 & ReflectionHelper.GetPropertyCase(resolution.File, tabWorkflow.FieldDocument)), Integer))
        Dim dc As BiblosDocumentInfo = BiblosDocumentInfo.GetDocuments(resolution.Location.ReslBiblosDSDB, idDoc).Where(Function(t) Not False OrElse t.IsSigned).ToList().FirstOrDefault()

        Dim documentSigners As IDictionary(Of String, String) = dc.GetDocumentSignersSerialNumbers()

        Dim presentSigners As IList(Of String) = New List(Of String)

        For Each signer As KeyValuePair(Of String, String) In documentSigners
            If DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Any(Function(m) m.Value.SignCertificate.Eq(signer.Key)) AndAlso
                    DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Where(Function(m) m.Value.SignCertificate.Eq(signer.Key)).FirstOrDefault().Value.SignUser.Eq(signer.Value) Then
                Dim manager As String = DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Where(Function(m) m.Value.SignCertificate.Eq(signer.Key)).FirstOrDefault().Value.Account
                presentSigners.Add(manager)
            End If
        Next

        Return presentSigners
    End Function


    Public Function GetPubblicationDocuments(ByVal location As Location, ByVal fileResolution As FileResolution, ByVal fieldName As String, ByVal forcedName As String, setAsSelected As Boolean) As IList(Of WebDoc)
        Dim documents As New List(Of BiblosDocumentInfo)
        Dim chainId As Object = ReflectionHelper.GetPropertyCase(fileResolution, fieldName)
        If chainId IsNot Nothing Then
            Select Case chainId.GetType
                Case GetType(Guid)
                    If Not chainId.Equals(Guid.Empty) Then
                        documents = BiblosDocumentInfo.GetDocuments(CType(chainId, Guid))
                    End If
                Case GetType(Integer)
                    If chainId > 0 Then
                        documents = BiblosDocumentInfo.GetDocuments(location.ReslBiblosDSDB, CType(chainId, Integer))
                    End If
            End Select
            'In tutti gli altri casi non carico documenti e restituisco quindi lista vuota

            Return ConvertBiblosDocumentInfosToWebDoc(documents, forcedName, setAsSelected)
        End If
        Return New List(Of WebDoc)
    End Function

    Public Function ConvertBiblosDocumentInfosToWebDoc(ByVal biblosDocumentInfoList As IList(Of BiblosDocumentInfo), ByVal name As String, ByVal setAsSelected As Boolean) As IList(Of WebDoc)
        Dim webDocuments As New List(Of WebDoc)
        Dim counter As Integer = 0
        For Each doc As BiblosDocumentInfo In biblosDocumentInfoList
            Dim wd As New WebDoc(doc.DocumentId, doc.Name, setAsSelected)
            If Not String.IsNullOrEmpty(name) Then
                counter += 1
                wd.Name = String.Format("{0}_{1}", name, counter)
            End If
            webDocuments.Add(wd)
        Next
        Return webDocuments
    End Function

#End Region

#Region "Resolution Viewer"

    ''' <summary> Genera il DataSource per la resolution. </summary>
    ''' <returns>Restituisce il Group relativao alla resolution corrente</returns>
    ''' <remarks>Se icremental è uguale o minore a zero viene preso lo step attualmente attivo.</remarks>
    Public Function GetResolutionDataSource(idResolution As Integer, resolutionIncremental As Short, showDocuments As Boolean, showDocumentsOmissis As Boolean, showDocumentsOmissisFromStep As Boolean,
                                            showAttachments As Boolean, showAttachmentsFromStep As Boolean, showAttachmentsOmissis As Boolean, showAttachmentsOmissisFromStep As Boolean, showAnnexes As Boolean, previousIncremental As String) As DocumentInfo
        Dim resl As Resolution = GetById(idResolution)
        Dim main As New FolderInfo() With {
            .Name = $"{FacadeFactory.Instance.ResolutionTypeFacade.GetDescription(resl.Type)} {resl.InclusiveNumber}",
            .ID = idResolution.ToString()
        }

        Dim incr As Short = resolutionIncremental
        If incr <= 0 Then
            incr = FacadeFactory.Instance.ResolutionWorkflowFacade.GetActiveIncremental(idResolution, 1)
        End If

        If showDocuments Then
            Dim folder As FolderInfo = CreateDocumentFolder(resl, incr, includeUniqueId:=True)
            If folder IsNot Nothing Then
                main.AddChild(folder)
            End If
        End If

        Dim prev As Short? = GetPreviousIncremental(resl, incr, previousIncremental)
        If prev.HasValue Then
            Dim folder As FolderInfo = CreateDocumentFolder(resl, prev.Value, includeUniqueId:=True)
            If folder IsNot Nothing Then
                main.AddChild(folder)
            End If
        End If

        ' Documenti Omissis
        If showDocumentsOmissis OrElse showDocumentsOmissisFromStep Then
            Dim folder As FolderInfo = CreateDocumentsOmissisFolder(resl, incr, showDocumentsOmissisFromStep, includeUniqueId:=True)
            If folder IsNot Nothing Then
                main.AddChild(folder)
            End If
        End If

        ' Allegati
        If showAttachments OrElse showAttachmentsFromStep Then
            Dim folder As FolderInfo = CreateAttachmentsFolder(resl, incr, showAttachmentsFromStep, includeUniqueId:=True)
            If folder IsNot Nothing Then
                main.AddChild(folder)
            End If
        End If

        ' Allegati Omissis
        If showAttachmentsOmissis OrElse showAttachmentsOmissisFromStep Then
            Dim folder As FolderInfo = CreateAttachmentsOmissisFolder(resl, incr, showAttachmentsOmissisFromStep, includeUniqueId:=True)
            If folder IsNot Nothing Then
                main.AddChild(folder)
            End If
        End If

        ' Annessi
        If showAnnexes Then
            Dim folder As FolderInfo = CreateAnnexesFolder(resl, incr, includeUniqueId:=True)
            If folder IsNot Nothing Then
                main.AddChild(folder)
            End If
        End If

        Return main
    End Function

    ''' <summary> Indica se per lo step indicato sia necessario visualizzare i documenti dello step precedente. </summary>
    ''' <param name="resl">Resolution di riferimento</param>
    ''' <param name="incr">Identificativo dello step</param>
    ''' <returns>Restituisce nothing nel caso non sia da visualizzare, altrimenti l'identificativo dello step da visualizzare.</returns>
    Public Function GetPreviousIncremental(resl As Resolution, incr As Short, previousIncremental As String) As Short?
        If previousIncremental.Eq("none") Then
            Return Nothing
        End If

        Dim tab As TabWorkflow = FacadeFactory.Instance.TabWorkflowFacade.GetTabWorkflow(resl, incr)
        Dim wf As ResolutionWorkflow = FacadeFactory.Instance.ResolutionWorkflowFacade.GetById(New ResolutionWorkflowCompositeKey() With {.IdResolution = resl.Id, .Incremental = incr})

        If previousIncremental.Eq("show") OrElse (previousIncremental.Eq("conditional") AndAlso (tab.ViewPreStep.Eq("1"))) Then
            Return wf.IncrementalFather
        End If

        Return Nothing
    End Function


    Public Function CreateDocumentFolder(resl As Resolution, incr As Short, Optional includeUniqueId As Boolean = False) As FolderInfo
        Dim tab As TabWorkflow = FacadeFactory.Instance.TabWorkflowFacade.GetTabWorkflow(resl, incr)
        Dim docs() As BiblosDocumentInfo = GetDocuments(resl, incr, includeUniqueId)

        If docs.Length <= 0 Then
            Return Nothing
        End If

        Dim folder As New FolderInfo() With {.Name = tab.DocumentDescription}
        folder.AddChildren(docs)
        Return folder
    End Function

    Public Function CreateDocumentsOmissisFolder(resl As Resolution, incr As Short, getFromStep As Boolean, Optional includeUniqueId As Boolean = False) As FolderInfo
        Dim docs() As BiblosDocumentInfo = GetDocumentsOmissis(resl, incr, getFromStep, includeUniqueId)
        If docs.Length <= 0 Then
            Return Nothing
        End If

        Dim folder As New FolderInfo() With {.Name = "Documenti Omissis"}
        folder.AddChildren(docs)
        Return folder
    End Function

    Public Function CreateAttachmentsFolder(resl As Resolution, incr As Short, getFromStep As Boolean, Optional includeUniqueId As Boolean = False) As FolderInfo
        If Not (New ResolutionRights(resl)).IsAttachmentViewable Then
            Return Nothing
        End If

        Dim docs() As BiblosDocumentInfo = GetAttachments(resl, incr, getFromStep, includeUniqueId)
        If docs.Length <= 0 Then
            Return Nothing
        End If

        Dim folder As New FolderInfo() With {.Name = "Allegati (parte integrante)"}
        folder.AddChildren(docs)
        Return folder
    End Function

    Public Function CreateAttachmentsOmissisFolder(resl As Resolution, incr As Short, getFromStep As Boolean, Optional includeUniqueId As Boolean = False) As FolderInfo
        Dim docs() As BiblosDocumentInfo = GetAttachmentsOmissis(resl, incr, getFromStep, includeUniqueId)
        If docs.Length <= 0 Then
            Return Nothing
        End If

        Dim folder As New FolderInfo() With {.Name = "Allegati Omissis (parte integrante)"}
        folder.AddChildren(docs)
        Return folder
    End Function

    Public Function CreateAnnexesFolder(resl As Resolution, incr As Short, Optional includeUniqueId As Boolean = False) As FolderInfo
        Dim docs() As BiblosDocumentInfo = GetAnnexes(resl, incr, includeUniqueId)
        If docs.Length <= 0 Then
            Return Nothing
        End If

        Dim folder As New FolderInfo() With {.Name = "Annessi (non parte integrante)"}
        folder.AddChildren(docs)
        Return folder
    End Function

    Public Function GetCategoryDescription(resolution As Resolution) As String
        Dim description As String
        If resolution.SubCategory IsNot Nothing Then
            description = Factory.CategoryFacade.GetFullIncrementalName(resolution.SubCategory)
        Else
            description = Factory.CategoryFacade.GetFullIncrementalName(resolution.Category)
        End If
        Return description
    End Function

    Public Function GetActualToPublicate(serviceNumber As String, type As Short, workflowType As String) As Resolution
        Return _dao.GetActualToPublicate(serviceNumber, type, workflowType)
    End Function

#End Region

    Public Sub ResolutionInsertedDocumentPrivacyLevel(resolution As Resolution, doc As BiblosDocumentInfo, typeDoc As String)
        Dim type As String = "documento"
        Select Case typeDoc
            Case "AR"
                type = "allegato riservato"
                Exit Select
            Case "A"
                type = "allegato"
                Exit Select
            Case "AN"
                type = "annesso"
                Exit Select
            Case "DO"
                type = "documento omissis"
                Exit Select
            Case "AO"
                type = "allegato omesso"
                Exit Select
        End Select
        If doc.Attributes.Any(Function(k) k.Key.Equals(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)) Then
            FacadeFactory.Instance.ResolutionLogFacade.Insert(resolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", doc.Attributes.Item(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE), type, doc.Name, doc.DocumentId))
        End If
    End Sub

    Public Function UpdateResolutionFromCollaboration(collaboration As Collaboration) As Boolean
        Dim isCollaborationeFromResolution As Boolean = False
        If DocSuiteContext.Current.ProtocolEnv.CheckResolutionCollaborationOriginEnabled Then
            Dim draft As CollaborationXmlData = FacadeFactory.Instance.ProtocolDraftFacade.GetDataFromCollaboration(collaboration)
            If draft IsNot Nothing AndAlso draft.GetType() = GetType(ResolutionXML) Then
                Dim resolutionXML As ResolutionXML = CType(draft, ResolutionXML)
                Dim documentsDictionary As IDictionary(Of Guid, BiblosDocumentInfo)
                Dim idDocumento As Integer = 0
                Dim idAllegati As Integer = 0
                Dim idDocumentiOmissis As Guid = Guid.Empty
                Dim idAllegatiOmissis As Guid = Guid.Empty
                Dim idAnnexes As Guid = Guid.Empty

                If (resolutionXML IsNot Nothing) Then
                    Dim idResolution As Integer = resolutionXML.ResolutionsToUpdate()(0)
                    Dim resolution As Resolution = GetById(idResolution)
                    If resolution.Type.Id = ResolutionType.IdentifierDelibera AndAlso GetActiveStep(resolution).ResStep = 2 Then
                        Dim signature As String = "*"

                        'documento principale
                        documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.MainDocument)
                        If documentsDictionary.Count > 0 Then
                            SaveBiblosDocuments(resolution, New List(Of DocumentInfo)(documentsDictionary.Values), signature, Guid.Empty, idDocumento)
                            Factory.ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, idDocumento, ResolutionFacade.DocType.Proposta)
                        End If

                        'documento omissis
                        documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.MainDocumentOmissis)
                        If documentsDictionary IsNot Nothing AndAlso documentsDictionary.Count > 0 Then
                            SaveBiblosDocuments(resolution, New List(Of DocumentInfo)(documentsDictionary.Values), signature, idDocumentiOmissis)
                            Factory.ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, idDocumentiOmissis, ResolutionFacade.DocType.DocumentoPrincipaleOmissis)
                        End If

                        'allegati
                        documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.Attachment)
                        If documentsDictionary IsNot Nothing AndAlso documentsDictionary.Count > 0 Then
                            SaveBiblosDocuments(resolution, New List(Of DocumentInfo)(documentsDictionary.Values), signature, Guid.Empty, idAllegati)
                            Factory.ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, idAllegati, ResolutionFacade.DocType.Allegati)
                        End If

                        'allegati omissis
                        documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.AttachmentOmissis)
                        If documentsDictionary IsNot Nothing AndAlso documentsDictionary.Count > 0 Then
                            SaveBiblosDocuments(resolution, New List(Of DocumentInfo)(documentsDictionary.Values), signature, idAllegatiOmissis)
                            Factory.ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, idAllegatiOmissis, ResolutionFacade.DocType.AllegatiOmissis)
                        End If

                        'annessi
                        documentsDictionary = Factory.CollaborationVersioningFacade.GetLastVersionDocuments(collaboration, VersioningDocumentGroup.Annexed)
                        If documentsDictionary IsNot Nothing AndAlso documentsDictionary.Count > 0 Then
                            SaveBiblosDocuments(resolution, New List(Of DocumentInfo)(documentsDictionary.Values), signature, idAnnexes)
                            Factory.ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, idAnnexes, ResolutionFacade.DocType.Annessi)
                        End If

                        Dim workStep As TabWorkflow = Nothing
                        Dim workNextStep As TabWorkflow = Nothing
                        Dim actualStepId As Short = GetActiveStep(resolution).ResStep
                        Dim nextStepId As Short = actualStepId + 1S
                        Factory.TabWorkflowFacade.GetByStep(resolution.WorkflowType, actualStepId, workStep)
                        Factory.TabWorkflowFacade.GetByStep(resolution.WorkflowType, nextStepId, workNextStep)

                        Dim data As DateTime = Date.Today()

                        Factory.ResolutionWorkflowFacade.InsertNextStep(idResolution, 2, idDocumento, idAllegati, 0, idAnnexes, idDocumentiOmissis, idAllegatiOmissis, DocSuiteContext.Current.User.FullUserName)
                        resolution.Status = Factory.ResolutionStatusFacade.GetById(ResolutionStatusId.Attivo)
                        Factory.ResolutionFacade.UpdateOnly(resolution)
                        Factory.ResolutionFacade.SqlResolutionUpdateWorkflowData(idResolution, resolution.Type.Id, Not resolution.Number.HasValue, workNextStep, String.Format("{0:dd/MM/yyyy}", data), "N", -1, idDocumento, idAnnexes, "N", True, DocSuiteContext.Current.User.FullUserName)

                        isCollaborationeFromResolution = True
                    End If
                End If
            End If
        End If
        Return isCollaborationeFromResolution
    End Function


End Class