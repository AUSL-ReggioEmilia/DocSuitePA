Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class UDSViewer
    Inherits UDSBasePage
    Implements ISendMail

#Region "Fields"
    Private _udsSource As UDSDto
    Private _udsKeys As IDictionary(Of Guid, Guid) = Nothing
    Private _udsList As IList(Of UDSDto) = Nothing
    Private _currentSchemaRepositoryModel As UDSModel = Nothing
#End Region

#Region "Properties"
    Public ReadOnly Property CurrentSchemaRepositoryModel() As UDSModel
        Get
            If _currentSchemaRepositoryModel Is Nothing Then
                Dim uds As UDSDto
                If CurrentIdUDSRepository.HasValue AndAlso CurrentIdUDS.HasValue Then
                    uds = GetSource()
                    _currentSchemaRepositoryModel = uds.UDSModel
                End If
            End If

            Return _currentSchemaRepositoryModel
        End Get
    End Property

    Private ReadOnly Property UDSKeys As IDictionary(Of Guid, Guid)
        Get
            If _udsKeys.IsNullOrEmpty() Then
                Dim queryStringValues As String = GetKeyValue(Of String)("UDSIds")
                If CurrentIdUDS.HasValue AndAlso Not CurrentIdUDS = Guid.Empty AndAlso CurrentIdUDSRepository.HasValue AndAlso Not CurrentIdUDSRepository = Guid.Empty Then
                    _udsKeys = New Dictionary(Of Guid, Guid)
                    _udsKeys.Add(CurrentIdUDS.Value, CurrentIdUDSRepository.Value)
                End If
                If _udsKeys.IsNullOrEmpty() AndAlso Not String.IsNullOrEmpty(queryStringValues) Then
                    _udsKeys = JsonConvert.DeserializeObject(Of IDictionary(Of Guid, Guid))(Server.UrlDecode(queryStringValues))
                End If
            End If
            Return _udsKeys
        End Get
    End Property

    Private ReadOnly Property UDSList As IList(Of UDSDto)
        Get
            If Not UDSKeys.IsNullOrEmpty() AndAlso _udsList Is Nothing Then
                Dim udsDto As IList(Of UDSDto) = New List(Of UDSDto)
                Dim uds As UDSDto
                For Each item As KeyValuePair(Of Guid, Guid) In UDSKeys
                    uds = GetSource(item.Value, item.Key)
                    udsDto.Add(uds)
                Next
                _udsList = udsDto
            End If
            Return _udsList
        End Get
    End Property

    Public ReadOnly Property ViewOriginal As Boolean
        Get
            Dim param As String = Request.QueryString("ViewOriginal")
            If String.IsNullOrEmpty(param) Then
                Return False
            End If
            Dim result As Boolean = False
            Boolean.TryParse(param, result)
            Return result
        End Get
    End Property

    Public ReadOnly Property Documents As IList(Of DocumentInfo) Implements ISendMail.Documents
        Get
            Dim list As New List(Of DocumentInfo)
            For Each uds As UDSDto In UDSList
                list.AddRange(UDSFacade.GetAllDocuments(uds.UDSModel))
            Next
            Return list
        End Get
    End Property

    Public ReadOnly Property SenderDescription As String Implements ISendMail.SenderDescription
        Get
            Return CommonInstance.UserDescription
        End Get
    End Property

    Public ReadOnly Property SenderEmail As String Implements ISendMail.SenderEmail
        Get
            Return CommonInstance.UserMail
        End Get
    End Property

    Public ReadOnly Property Recipients As IList(Of ContactDTO) Implements ISendMail.Recipients
        Get
            Return New List(Of ContactDTO)()
        End Get
    End Property

    Public ReadOnly Property Subject As String Implements ISendMail.Subject
        Get
            If UDSList IsNot Nothing AndAlso UDSList.Count = 1 AndAlso CurrentUDSRepository IsNot Nothing Then
                Return UDSFacade.GetUDSMailSubject(UDSList.First())
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Body As String Implements ISendMail.Body
        Get
            If UDSList IsNot Nothing AndAlso UDSList.Count = 1 AndAlso CurrentUDSRepository IsNot Nothing Then
                Return UDSFacade.GetUDSMailBody(UDSList.First())
            End If
            Return String.Empty
        End Get
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ViewerLight.StampaConformeEnabled = CurrentSchemaRepositoryModel Is Nothing OrElse CurrentSchemaRepositoryModel.Model.StampaConformeEnabled
        ViewerLight.DocumentsPreviewEnabled = ViewerLight.StampaConformeEnabled

        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            btnSend.PostBackUrl = "~/MailSenders/UDSMailSender.aspx?recipients=false&overridepreviouspageurl=true&Type=UDS"
            If CurrentIdUDS.HasValue AndAlso CurrentIdUDSRepository.HasValue Then
                btnSend.PostBackUrl = String.Concat(btnSend.PostBackUrl, "&IdUDSRepository=", CurrentIdUDSRepository, "&IdUDS=", CurrentIdUDS)
            End If
            BindViewerLight()
        End If
        ViewerLight.CheckViewableRight = True
        ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("UDSViewer")
    End Sub
#End Region

#Region "Methods"
    Private Sub BindViewerLight()
        If ViewOriginal Then
            ViewerLight.ViewOriginal = True
        End If
        ViewerLight.DataSource = UDSList.Select(Function(u) GetUDSDocuments(u)).ToList()
    End Sub


    Private Function GetUDSDocuments(udsSource As UDSDto) As DocumentInfo
        Return UDSFacade.GetUDSTreeDocuments(udsSource, Sub(document As BiblosDocumentInfo, documentType As UDSDocumentType)
                                                            document.AddAttribute(Viewers.ViewerLight.BIBLOS_ATTRIBUTE_UniqueId, udsSource.Id.ToString())
                                                            document.AddAttribute(Viewers.ViewerLight.BIBLOS_ATTRIBUTE_Environment, DirectCast(DSWEnvironment.UDS, Integer).ToString())
                                                        End Sub)
    End Function

#End Region

End Class