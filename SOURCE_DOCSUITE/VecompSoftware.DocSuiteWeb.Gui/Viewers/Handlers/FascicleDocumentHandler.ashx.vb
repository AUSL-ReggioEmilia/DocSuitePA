Imports System.Web
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports System.Web.SessionState
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS
Imports VecompSoftware.DocSuiteWeb.Entity.UDS

Namespace Viewers.Handlers

    Public Class FascicleDocumentHandler
        Inherits DocumentHandler
        Implements IRequiresSessionState


        Private _udsLogFacade As UDSLogFacade

        Public ReadOnly Property UDSLogFacade As UDSLogFacade
            Get
                If _udsLogFacade Is Nothing Then
                    _udsLogFacade = New UDSLogFacade(DocSuiteContext.Current.Tenants)
                End If
                Return _udsLogFacade
            End Get
        End Property

        Protected ReadOnly Property UserVisibilityAuthorized As Boolean
            Get
                If CurrentHttpContext IsNot Nothing Then
                    Return CurrentHttpContext.Request.QueryString.GetValueOrDefault(Of Boolean)("UserVisibilityAuthorized", False)
                End If
                Return False
            End Get
        End Property

        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            _currentHttpContext = context

            If Not Miscellanea Then
                CheckDocumentUnitExistence = True
            End If
            ExpandDocumentUnitRoles = DocSuiteContext.Current.PrivacyEnabled
            FromFascicle = True
            ElaborateDocument(context)
        End Sub

        Protected Overrides Function CheckRight() As Boolean
            If Not Miscellanea AndAlso Not UserVisibilityAuthorized AndAlso UniqueId.HasValue AndAlso UniqueId.Value <> Guid.Empty Then
                Return CurrentODataFacade.HasViewableRight(UniqueId.Value, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            Else
                Return True
            End If
        End Function
        Protected Overrides Function CheckPrivacyRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function

        Protected Overrides Sub LogView(documentName As String, guid As Guid)
            If Not Miscellanea AndAlso Environment.HasValue AndAlso CurrentDocumentUnit IsNot Nothing Then
                Dim logDescription As String = String.Format("Visualizzato documento ""{0}"" [{1}]", documentName, guid.ToString("N"))
                Select Case Environment.Value
                    Case DSWEnvironment.Protocol
                        FacadeFactory.ProtocolLogFacade.Insert(CurrentDocumentUnit.Year, CurrentDocumentUnit.Number, ProtocolLogEvent.PD.ToString(), logDescription)
                    Case DSWEnvironment.Resolution
                        FacadeFactory.ResolutionLogFacade.Insert(CurrentDocumentUnit.EntityId, ResolutionLogType.RD, logDescription)
                    Case DSWEnvironment.DocumentSeries
                        FacadeFactory.DocumentSeriesItemLogFacade.AddLog(CurrentDocumentUnit.EntityId, DocumentSeriesItemLogType.SD, logDescription)
                    Case >= 100
                        UDSLogFacade.InsertLog(UniqueId.Value, CurrentDocumentUnit.UDSRepository.UniqueId, Environment.Value, logDescription, UDSLogType.DocumentView)
                End Select
            End If
        End Sub

    End Class
End Namespace