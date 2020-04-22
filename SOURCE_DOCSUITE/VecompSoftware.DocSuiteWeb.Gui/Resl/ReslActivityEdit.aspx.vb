Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ReslActivityEdit
    Inherits ReslBasePage

#Region " Fields "
    Private _currentResolutionActivity As ResolutionActivity
#End Region

#Region " Properties "
    Public ReadOnly Property IdResolutionActivity As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid?)("IdActivity", Nothing)
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionActivty As ResolutionActivity
        Get
            If _currentResolutionActivity Is Nothing AndAlso IdResolutionActivity.HasValue Then
                _currentResolutionActivity = Facade.ResolutionActivityFacade.GetById(IdResolutionActivity.Value)
            End If
            Return _currentResolutionActivity
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        If CurrentResolutionActivty.Status <> ResolutionActivityStatus.ToBeProcessed Then
            AjaxAlert("Attenzione: non è possibile modificare la data di un attività completata.")
            Exit Sub
        End If

        btnSave.Enabled = True
        If Not IsPostBack Then
            txtDate.SelectedDate = CurrentResolutionActivty.ActivityDate.DateTime
            cvCompareData.ValueToCompare = DateTime.Today.ToShortDateString()
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If CurrentResolutionActivty.ActivityType = ResolutionActivityType.Effectiveness AndAlso Facade.ResolutionActivityFacade.CheckPublicationActivityDate(CurrentResolutionActivty.Resolution, txtDate.SelectedDate.Value) Then
            AjaxAlert("Attenzione: la data di esecutività deve essere superiore alla data di pubblicazione.")
            Exit Sub
        End If
        CurrentResolutionActivty.ActivityDate = DateTimeOffset.Parse(txtDate.SelectedDate.Value.ToString())
        Facade.ResolutionActivityFacade.Update(CurrentResolutionActivty)
        Facade.ResolutionLogFacade.Insert(CurrentResolutionActivty.Resolution, ResolutionLogType.RA, String.Concat("Modificata data prevista attività JeepService per step di ", CurrentResolutionActivty.ActivityType.GetDescription()))
        AjaxManager.ResponseScripts.Add("CloseWindow('RefreshResults')")
    End Sub
#End Region


End Class