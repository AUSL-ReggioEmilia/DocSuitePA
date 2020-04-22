Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports FascicleDocumentUnitFacade = VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles.FascicleDocumentUnitFacade

Partial Public Class ReslModifica
    Inherits ReslBasePage

#Region " Fields "

    Private _reslController As IChangerController(Of Resolution)
    Private _currentFascicleDocumentUnitFacade As FascicleDocumentUnitFacade

#End Region

#Region " Properties "

    Private Property Controller() As IChangerController(Of Resolution)
        Get
            Return _reslController
        End Get
        Set(ByVal value As IChangerController(Of Resolution))
            _reslController = value
        End Set
    End Property

    Private Property CurrentResolutionModel As ResolutionInsertModel
        Get
            If Session("CurrentResolutionModel") IsNot Nothing Then
                Return DirectCast(Session("CurrentResolutionModel"), ResolutionInsertModel)
            End If
            Return Nothing
        End Get
        Set(value As ResolutionInsertModel)
            If value Is Nothing Then
                Session.Remove("CurrentResolutionModel")
            Else
                Session("CurrentResolutionModel") = value
            End If
        End Set
    End Property

    Private ReadOnly Property CurrentFascicleDocumentUnitFacade As FascicleDocumentUnitFacade
        Get
            If _currentFascicleDocumentUnitFacade Is Nothing Then
                _currentFascicleDocumentUnitFacade = New FascicleDocumentUnitFacade(DocSuiteContext.Current.Tenants)
            End If
            Return _currentFascicleDocumentUnitFacade
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        uscReslChange.CurrentResolution = CurrentResolution
        Controller = ControllerFactory.CreateResolutionChangerController(uscReslChange)
        Controller.AttachEvents()
        If Not IsPostBack Then
            Controller.Initialize()
            Title = Facade.ResolutionTypeFacade.GetDescription(CurrentResolution.Type) & " - Modifica"
            Controller.DataBind()
        End If

    End Sub

    Private Sub btnConfirm_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConfirm.Click
        If CurrentResolution Is Nothing Then
            Exit Sub
        End If

        Dim errorMessage As String = String.Empty
        If Not Controller.ValidateData(errorMessage) Then
            AjaxAlert(errorMessage)
            Exit Sub
        End If

        If uscReslChange.IsVisibleContainer AndAlso uscReslChange.IsNullContainer Then
            AjaxAlert("Deve essere associato almeno un Contenitore")
            Exit Sub
        End If

        If ResolutionEnv.ResolutionKindEnabled AndAlso Not uscReslChange.IsAllDocumentSeriesLink Then
            AjaxAlert("Associare tutte le serie documentali")
            Exit Sub
        End If

        Try
            If ResolutionEnv.ResolutionKindEnabled AndAlso uscReslChange.CurrentResolutionDocumentSeriesDTO IsNot Nothing Then
                If CurrentResolutionModel IsNot Nothing Then
                    CurrentResolution.ResolutionKind = Me.uscReslChange.CurrentSelectedResolutionKind
                End If

                If Me.uscReslChange.DocumentSeriesToRemove IsNot Nothing Then
                    For Each resolutionToRemove As ResolutionDocumentSeriesItem In Me.uscReslChange.CurrentResolutionsDocumentSeriesItem
                        If uscReslChange.DocumentSeriesToRemove.Any(Function(x) x.DSItemId.HasValue AndAlso x.DSItemId.Value = resolutionToRemove.IdDocumentSeriesItem.Value) Then
                            Facade.ResolutionDocumentSeriesItemFacade.Delete(resolutionToRemove)
                        End If
                    Next
                End If

                For Each item As ResolutionChangeDSI In uscReslChange.CurrentResolutionDocumentSeriesDTO.Where(Function(x) Not x.ResolutionDSItemId.HasValue)
                    Dim resolutionSeries As ResolutionDocumentSeriesItem = New ResolutionDocumentSeriesItem()
                    resolutionSeries.Resolution = CurrentResolution
                    resolutionSeries.UniqueIdResolution = CurrentResolution.UniqueId
                    resolutionSeries.IdDocumentSeriesItem = item.DSItemId.Value
                    resolutionSeries.UniqueIdDocumentSeriesItem = FacadeFactory.Instance.DocumentSeriesItemFacade.GetById(item.DSItemId.Value).UniqueId
                    Facade.ResolutionDocumentSeriesItemFacade.Save(resolutionSeries)
                Next
                uscReslChange.CurrentResolutionDocumentSeriesDTO = Nothing
                uscReslChange.DraftSeriesItemAdded = Nothing
                uscReslChange.DocumentSeriesToRemove = Nothing
            End If

            Dim currentCategory As Category = If(CurrentResolution.SubCategory IsNot Nothing, CurrentResolution.SubCategory, CurrentResolution.Category)
            Controller.BindDataToObject(CurrentResolution)
            If currentCategory IsNot Nothing Then
                Dim toCompareCategory As Category = If(CurrentResolution.SubCategory IsNot Nothing, CurrentResolution.SubCategory, CurrentResolution.Category)
                If Not currentCategory.Id.Equals(toCompareCategory.Id) Then
                    If CurrentFascicleDocumentUnitFacade.GetFascicolatedIdDocumentUnit(CurrentResolution.UniqueId) IsNot Nothing Then
                        AjaxAlert("Non è possibile modificare il classificatore del documento in quanto già Fascicolato.")
                        Exit Sub
                    End If
                End If
            End If
            Facade.ResolutionFacade.Update(CurrentResolution)
            Facade.ResolutionLogFacade.Log(CurrentResolution, ResolutionLogType.RM)

            FacadeFactory.Instance.ResolutionFacade.SendUpdateResolutionCommand(CurrentResolution)

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in Modifica Atto", ex)
            AjaxAlert(String.Format("Errore in Modifica Atto. {0}", ProtocolEnv.DefaultErrorMessage))
            Exit Sub
        End Try

        Response.Redirect(String.Format("{0}?{1}", GetViewPageName(IdResolution), CommonShared.AppendSecurityCheck(String.Format("Type=Resl&idResolution={0}", IdResolution))))
    End Sub

#End Region

End Class