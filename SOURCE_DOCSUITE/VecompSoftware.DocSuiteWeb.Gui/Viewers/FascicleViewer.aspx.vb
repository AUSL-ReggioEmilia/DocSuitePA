Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports System.Diagnostics
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI
Imports System.IO

Namespace Viewers
    Public Class FascicleViewer
        Inherits FascBasePage
        Implements ISendMail

#Region " Fields "

        Private _fascicleKeys As List(Of Guid) = Nothing
        Private _currentFascicleFinder As Data.WebAPI.Finder.Fascicles.FascicleFinder = Nothing
        Private _currentDocumentUnitChainFacade As DocumentUnitChainFacade = Nothing
        Private _currentDocumentUnitChainFinder As Data.WebAPI.Finder.DocumentUnits.DocumentUnitChainFinder = Nothing
        Private _fascicleList As IList(Of Entity.Fascicles.Fascicle) = Nothing
        Private _currentInsertsLocation As Location = Nothing
        Private _currentFascicleFolderFinder As FascicleFolderFinder = Nothing
        Private _currentFascicleDocumentUnitFinder As FascicleDocumentUnitFinder
        Public Const GET_WF_VISIBILITY_VALUE As String = "getWfVisibilityValue();"

#End Region

#Region " Properties "

        Private ReadOnly Property FascicleKeys As IList(Of Guid)
            Get
                If _fascicleKeys.IsNullOrEmpty() Then
                    Dim queryStringValues As String = GetKeyValue(Of String)("FascicleIds")
                    If Not IdFascicle = Guid.Empty Then
                        _fascicleKeys = New List(Of Guid)
                        _fascicleKeys.Add(IdFascicle)
                    ElseIf Not String.IsNullOrEmpty(queryStringValues) Then
                        _fascicleKeys = JsonConvert.DeserializeObject(Of List(Of Guid))(Server.UrlDecode(queryStringValues))
                    End If
                End If
                Return _fascicleKeys
            End Get
        End Property

        Private ReadOnly Property FascicleList As IList(Of Entity.Fascicles.Fascicle)
            Get
                If Not FascicleKeys.IsNullOrEmpty() AndAlso _fascicleList Is Nothing Then
                    Dim fasciclesDto As ICollection(Of WebAPIDto(Of Entity.Fascicles.Fascicle)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFascicleFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.ExpandProperties = True
                                finder.EnablePaging = False
                                finder.FascicleIds = FascicleKeys
                                Return finder.DoSearch()
                            End Function)

                    Dim fascicles As IList(Of Entity.Fascicles.Fascicle) = New List(Of Entity.Fascicles.Fascicle)()
                    For Each fascicleDto As WebAPIDto(Of Entity.Fascicles.Fascicle) In fasciclesDto
                        fascicles.Add(fascicleDto.Entity)
                    Next
                    _fascicleList = fascicles
                End If
                Return _fascicleList
            End Get
        End Property

        Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
            Get
                Return CommonInstance.UserDescription
            End Get
        End Property

        Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
            Get
                Return CommonInstance.UserMail
            End Get
        End Property

        Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
            Get
                Return New List(Of ContactDTO)()
            End Get
        End Property

        Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
            Get
                Dim toCheckDocuments As IList(Of DocumentInfo) = ViewerLight.CheckedDocuments
                If toCheckDocuments.Count = 0 Then
                    toCheckDocuments = ViewerLight.AllDocuments
                End If

                Dim toSendDocuments As IList(Of DocumentInfo) = New List(Of DocumentInfo)
                Dim hasDocumentAttributesRight As Boolean = False
                Dim documentUniqueIdAttribute As Guid? = Nothing
                For Each document As DocumentInfo In toCheckDocuments
                    document.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_FascicleId, IdFascicle.ToString())
                    hasDocumentAttributesRight = document.Attributes.Any(Function(x) (x.Key = ViewerLight.BIBLOS_ATTRIBUTE_Miscellanea OrElse x.Key = ViewerLight.BIBLOS_ATTRIBUTE_UserVisibilityAuthorized) AndAlso x.Value.Eq(True.ToString()))
                    documentUniqueIdAttribute = document.Attributes.Where(Function(x) x.Key = ViewerLight.BIBLOS_ATTRIBUTE_UniqueId).Select(Function(s) Guid.Parse(s.Value)).SingleOrDefault()
                    If Not hasDocumentAttributesRight AndAlso documentUniqueIdAttribute.HasValue Then
                        If Not CurrentODataFacade.HasViewableRight(documentUniqueIdAttribute.Value, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain) Then
                            Continue For
                        End If
                    End If
                    toSendDocuments.Add(document)
                Next
                Return toSendDocuments
            End Get
        End Property
        Public ReadOnly Property Subject() As String Implements ISendMail.Subject
            Get
                Return MailFacade.GetFascicleListSubject(FascicleList)
            End Get
        End Property

        Public ReadOnly Property Body() As String Implements ISendMail.Body
            Get
                Return MailFacade.GetFascicleListBody(FascicleList, Documents)
            End Get
        End Property
        Public ReadOnly Property CurrentInsertsLocation() As Location
            Get
                If _currentInsertsLocation Is Nothing Then
                    _currentInsertsLocation = Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation)
                End If
                Return _currentInsertsLocation
            End Get
        End Property

        Public ReadOnly Property CurrentFascicleFinder() As Data.WebAPI.Finder.Fascicles.FascicleFinder
            Get
                If _currentFascicleFinder Is Nothing Then
                    _currentFascicleFinder = New Data.WebAPI.Finder.Fascicles.FascicleFinder(DocSuiteContext.Current.CurrentTenant)
                    Return _currentFascicleFinder
                Else
                    Return _currentFascicleFinder
                End If
            End Get
        End Property
        Public ReadOnly Property CurrentDocumentUnitChainFinder() As Data.WebAPI.Finder.DocumentUnits.DocumentUnitChainFinder
            Get
                If _currentDocumentUnitChainFinder Is Nothing Then
                    _currentDocumentUnitChainFinder = New Data.WebAPI.Finder.DocumentUnits.DocumentUnitChainFinder(DocSuiteContext.Current.CurrentTenant)
                    Return _currentDocumentUnitChainFinder
                Else
                    Return _currentDocumentUnitChainFinder
                End If
            End Get
        End Property
        Public ReadOnly Property CurrentDocumentUnitChainFacade() As DocumentUnitChainFacade
            Get
                If _currentDocumentUnitChainFacade Is Nothing Then
                    _currentDocumentUnitChainFacade = New DocumentUnitChainFacade(DocSuiteContext.Current.Tenants, CurrentTenant)
                    Return _currentDocumentUnitChainFacade
                End If
                Return _currentDocumentUnitChainFacade
            End Get
        End Property
        Public ReadOnly Property CurrentFascicleFolderFinder As FascicleFolderFinder
            Get
                If _currentFascicleFolderFinder Is Nothing Then
                    _currentFascicleFolderFinder = New FascicleFolderFinder(DocSuiteContext.Current.CurrentTenant)
                End If
                Return _currentFascicleFolderFinder
            End Get
        End Property
        Public ReadOnly Property CurrentFascicleDocumentUnitFinder As FascicleDocumentUnitFinder
            Get
                If _currentFascicleDocumentUnitFinder Is Nothing Then
                    _currentFascicleDocumentUnitFinder = New FascicleDocumentUnitFinder(DocSuiteContext.Current.CurrentTenant)
                End If
                Return _currentFascicleDocumentUnitFinder
            End Get
        End Property
#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                MasterDocSuite.TitleVisible = False
                If Not FascicleKeys.IsNullOrEmpty() AndAlso IdFascicle = Guid.Empty Then
                    btnSend.Enabled = False
                Else
                    btnSend.PostBackUrl = String.Format("{0}?Type=Fasc&IdFascicle={1}", btnSend.PostBackUrl, CurrentFascicle.Id)
                End If

                BindViewerLight()
            End If
            ViewerLight.Button_StartWorklow = btnWorkflow.ClientID
            ViewerLight.CheckViewableRight = True
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("FascicleViewer")
            AjaxManager.ResponseScripts.Add(GET_WF_VISIBILITY_VALUE)
        End Sub

#End Region

#Region " Methods "
        Private Sub BindViewerLight()
            Dim datasource As List(Of DocumentInfo) = FascicleList.Select(Function(k) GetFascicleUDDocuments(k)).ToList()
            ViewerLight.DataSource = datasource
            FileLogger.Debug(LoggerName, String.Format("{0} - BindViewerLight", Request.RawUrl))
        End Sub


        Friend Function GetFascicleUDDocuments(fascicle As Entity.Fascicles.Fascicle) As DocumentInfo
            Dim mainFolder As FolderInfo = New FolderInfo()
            mainFolder.Name = String.Format("Fascicolo {0} del {1:dd/MM/yyyy}", fascicle.Title, fascicle.RegistrationDate)
            mainFolder.ID = fascicle.UniqueId.ToString()

            Dim userIsAuthorized As Boolean = False
            If fascicle.VisibilityType.Equals(Entity.Fascicles.VisibilityType.Accessible) Then
                userIsAuthorized = CurrentODataFacade.HasFascicleDocumentViewableRight(fascicle.UniqueId, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            End If

            Dim rootFolders As ICollection(Of FascicleFolder) = FindFolderChildren(New FascicleFolder() With {.UniqueId = fascicle.UniqueId})
            Dim fascicleDocumentUnits As ICollection(Of Entity.Fascicles.FascicleDocumentUnit) = GetDocumentUnitsByFascicle(fascicle)
            Dim fascicleInserts As ICollection(Of Tuple(Of Guid, ICollection(Of BiblosDocumentInfo))) = GetFolderMiscellaneaByFascicle(fascicle)
            For Each folder As FascicleFolder In rootFolders
                FillFascicleFolders(mainFolder, fascicle, folder, userIsAuthorized, fascicleDocumentUnits, fascicleInserts)
            Next
            Return mainFolder
        End Function

        Private Sub FillFascicleFolders(folder As FolderInfo, fascicle As Entity.Fascicles.Fascicle, fascicleFolder As FascicleFolder, userIsAuthorized As Boolean,
                                        fascicleDocumentUnits As ICollection(Of Entity.Fascicles.FascicleDocumentUnit), fascicleInserts As ICollection(Of Tuple(Of Guid, ICollection(Of BiblosDocumentInfo))))
            Dim UDguids As IEnumerable(Of Guid) = fascicleDocumentUnits.Where(Function(x) x.FascicleFolder.UniqueId = fascicleFolder.UniqueId).Select(Function(f) f.DocumentUnit.UniqueId)
            Dim fascicleFolderInfo As FolderInfo = New FolderInfo With {
                .Name = fascicleFolder.Name,
                .ID = fascicleFolder.UniqueId.ToString()
            }

            Dim results As ICollection(Of WebAPIDto(Of Entity.DocumentUnits.DocumentUnitChain)) = Nothing
            Dim documents As DocumentInfo = Nothing
            For Each guid As Guid In UDguids
                results = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentDocumentUnitChainFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.IdDocumentUnit = guid
                                finder.EnablePaging = False
                                finder.ExpandProperties = True
                                Return finder.DoSearch()
                            End Function).OrderByDescending(Function(docUnitChain) docUnitChain.Entity.RegistrationDate).ToList()

                If results IsNot Nothing Then
                    documents = CurrentDocumentUnitChainFacade.GetDocumentUnitChainsDocuments(results.Select(Function(f) f.Entity).ToList(), ProtocolEnv.DocumentSeriesDocumentsLabel, userIsAuthorized)
                    If documents IsNot Nothing Then
                        fascicleFolderInfo.AddChild(documents)
                    End If
                End If
            Next

            Dim inserts As IList(Of BiblosDocumentInfo) = fascicleInserts.Where(Function(x) x.Item1 = fascicleFolder.UniqueId).SelectMany(Function(s) s.Item2).ToList()
            If Not inserts.IsNullOrEmpty() Then
                Dim insertsFolder As FolderInfo = New FolderInfo()
                insertsFolder.Name = "Inserti"
                insertsFolder.ID = fascicle.UniqueId.ToString()
                For Each doc As BiblosDocumentInfo In inserts
                    doc.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_UserVisibilityAuthorized, True.ToString())
                    doc.AddAttribute(DocumentUnitChainFacade.BIBLOS_ATTRIBUTE_Miscellanea, True.ToString())
                    insertsFolder.AddChild(doc)
                Next
                fascicleFolderInfo.AddChild(insertsFolder)
            End If

            Dim children As ICollection(Of FascicleFolder) = FindFolderChildren(fascicleFolder)
            If children IsNot Nothing AndAlso children.Count > 0 Then
                For Each child As FascicleFolder In children
                    FillFascicleFolders(fascicleFolderInfo, fascicle, child, userIsAuthorized, fascicleDocumentUnits, fascicleInserts)
                Next
            End If
            folder.AddChild(fascicleFolderInfo)
        End Sub

        Private Function GetFolderMiscellaneaByFascicle(fascicle As Entity.Fascicles.Fascicle) As ICollection(Of Tuple(Of Guid, ICollection(Of BiblosDocumentInfo)))
            Dim insertsDocuments As ICollection(Of FascicleDocument) = fascicle.FascicleDocuments.Where(Function(x) x.ChainType = Entity.DocumentUnits.ChainType.Miscellanea).ToList()
            Dim insertsDocs As ICollection(Of Tuple(Of Guid, ICollection(Of BiblosDocumentInfo))) = New List(Of Tuple(Of Guid, ICollection(Of BiblosDocumentInfo)))()
            If CurrentInsertsLocation IsNot Nothing Then
                insertsDocs = insertsDocuments.Select(Function(s) New Tuple(Of Guid, ICollection(Of BiblosDocumentInfo))(s.FascicleFolder.UniqueId,
                                                                                                                         BiblosDocumentInfo.GetDocumentsLatestVersion(s.IdArchiveChain).OrderByDescending(Function(doc) doc.DateCreated).ToList())).ToList()
            End If
            Return insertsDocs
        End Function

        Private Function FindFolderChildren(parentFolder As FascicleFolder) As ICollection(Of FascicleFolder)
            Dim results As ICollection(Of WebAPIDto(Of FascicleFolder)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFascicleFolderFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.EnablePaging = False
                                finder.ReadChildren = True
                                finder.UniqueId = parentFolder.UniqueId
                                Return finder.DoSearch()
                            End Function)

            Return results.Select(Function(s) s.Entity).ToList()
        End Function

        Private Function GetDocumentUnitsByFascicle(fascicle As Entity.Fascicles.Fascicle) As ICollection(Of Entity.Fascicles.FascicleDocumentUnit)
            Dim results As ICollection(Of WebAPIDto(Of Entity.Fascicles.FascicleDocumentUnit)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentFascicleDocumentUnitFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.EnablePaging = False
                                finder.ExpandProperties = True
                                finder.IdFascicle = fascicle.UniqueId
                                If Not ProtocolEnv.MultiAOOFascicleEnabled Then
                                    finder.IdDocumentUnitTenantAOO = CurrentTenant.TenantAOO.UniqueId
                                End If
                                Return finder.DoSearch()
                            End Function)

            Return results.Select(Function(s) s.Entity).OrderByDescending(Function(fdi) fdi.DocumentUnit.RegistrationDate).ToList()
        End Function

#End Region

    End Class
End Namespace