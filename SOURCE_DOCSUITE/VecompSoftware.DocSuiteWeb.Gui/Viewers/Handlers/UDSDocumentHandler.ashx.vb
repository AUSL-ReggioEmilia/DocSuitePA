Imports System.Web
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Web.SessionState
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports WebApiFacade = VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS
Imports WebApientity = VecompSoftware.DocSuiteWeb.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI

Namespace Viewers.Handlers

    Public Class UDSDocumentHandler
        Inherits DocumentHandler
        Implements IRequiresSessionState

        Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"
        Private _UDSFacade As UDSFacade
        Private _UDSRepositoryFacade As UDSRepositoryFacade
        Private _udsLogFacade As WebApiFacade.UDSLogFacade
        Private _udsUserFinder As UDSUserFinder

        Public ReadOnly Property CurrentUDSFacade As UDSFacade
            Get
                If _UDSFacade Is Nothing Then
                    _UDSFacade = New UDSFacade()
                End If
                Return _UDSFacade
            End Get
        End Property

        Public ReadOnly Property UDSLogFacade As WebApiFacade.UDSLogFacade
            Get
                If _udsLogFacade Is Nothing Then
                    _udsLogFacade = New WebApiFacade.UDSLogFacade(DocSuiteContext.Current.Tenants)
                End If
                Return _udsLogFacade
            End Get
        End Property

        Public ReadOnly Property UDSUserFinder As UDSUserFinder
            Get
                If _udsUserFinder Is Nothing Then
                    _udsUserFinder = New UDSUserFinder(DocSuiteContext.Current.Tenants)
                End If
                Return _udsUserFinder
            End Get
        End Property

        Public ReadOnly Property CurrentUDSRepositoryFacade As UDSRepositoryFacade
            Get
                If _UDSRepositoryFacade Is Nothing Then
                    _UDSRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
                End If
                Return _UDSRepositoryFacade
            End Get
        End Property

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            Dim parent As String = context.Request.QueryString.GetValue(Of String)("parent")
            Dim name As String = context.Request.QueryString.GetValue(Of String)("Name")
            CheckRightAndLogView(parent, name)
            ElaborateDocument(context)
        End Sub

        Private Sub CheckRightAndLogView(parent As String, documentName As String)
            Dim keysModel As KeyValuePair(Of String, Guid) = JsonConvert.DeserializeObject(Of KeyValuePair(Of String, Guid))(parent)
            Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetActiveRepositories(keysModel.Key).FirstOrDefault()
            Dim dto As UDSDto = CurrentUDSFacade.GetUDSSource(repository, String.Format(ODATA_EQUAL_UDSID, keysModel.Value))
            Dim currentRepositoryRights As UDSRepositoryRightsUtil = New UDSRepositoryRightsUtil(repository, DocSuiteContext.Current.User.FullUserName, dto)

            If Not currentRepositoryRights.IsDocumentsViewable AndAlso Not currentRepositoryRights.IsCurrentUserAuthorized Then
                Throw New InvalidOperationException(String.Concat("Non si possiedono i diritti di visualizzazione dell'archivio ", keysModel.Key, "."))
            End If
            Dim logMessage As String = String.Format("Visualizzato documento ""{0}"" [{1}]", documentName, keysModel.Value)

            UDSLogFacade.InsertLog(dto.Id, repository.Id, repository.DSWEnvironment, logMessage, WebApientity.UDSLogType.DocumentView)

        End Sub

        Protected Overrides Function CheckRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function

        Protected Overrides Function CheckPrivacyRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function

    End Class
End Namespace