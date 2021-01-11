Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data

Namespace Viewers.Handlers

    Public Class ResolutionDocumentHandler
        Inherits DocumentHandler

        Private _currentResolution As Resolution

        Private ReadOnly Property CurrentResolution As Resolution
            Get
                If _currentResolution Is Nothing AndAlso UniqueId.HasValue Then
                    _currentResolution = FacadeFactory.ResolutionFacade.GetByUniqueId(UniqueId.Value)
                End If
                Return _currentResolution
            End Get
        End Property


        Overrides Sub ProcessRequest(ByVal context As HttpContext)
            _currentHttpContext = context
            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                IdContainer = CurrentResolution.Container.Id
            End If
            CheckDocumentUnitExistence = CurrentResolution.AdoptionDate.HasValue AndAlso Not CurrentResolution.EffectivenessDate.HasValue
            ElaborateDocument(context)
        End Sub

        Protected Overrides Function CheckRight() As Boolean
            If Not CurrentResolution.AdoptionDate.HasValue OrElse CurrentResolution.EffectivenessDate.HasValue Then
                Return True
            End If
            Dim viewRight As Boolean = CurrentODataFacade.HasViewableRight(UniqueId.Value, DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain)
            Return viewRight
        End Function

        Protected Overrides Function CheckPrivacyRight() As Boolean
            'TODO: implementare logica specifica
            Return True
        End Function

        Protected Overrides Sub LogView(documentName As String, guid As Guid)
            FacadeFactory.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.RD, String.Format("Visualizzato documento ""{0}"" [{1}]", documentName, guid.ToString("N")))
        End Sub

    End Class
End Namespace