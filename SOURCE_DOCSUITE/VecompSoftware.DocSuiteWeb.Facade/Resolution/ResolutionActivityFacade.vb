Imports VecompSoftware.DocSuiteWeb.Data
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging

<DataObject()>
Public Class ResolutionActivityFacade
    Inherits BaseResolutionFacade(Of ResolutionActivity, Guid, NHibernateResolutionActivityDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub CreateActivity(resolution As Resolution, documents As ResolutionActivityDocumentModel, description As String, activityDate As DateTimeOffset, activityType As ResolutionActivityType)
        Dim activity As ResolutionActivity = New ResolutionActivity()
        activity.Resolution = resolution
        activity.ActivityType = activityType
        activity.Status = ResolutionActivityStatus.ToBeProcessed
        activity.WorkflowType = resolution.WorkflowType
        activity.ActivityDate = activityDate
        activity.Description = description
        If documents IsNot Nothing Then
            activity.JsonDocuments = JsonConvert.SerializeObject(documents)
        End If
        activity.UniqueIdResolution = resolution.UniqueId

        Save(activity)
        Factory.ResolutionLogFacade.Insert(resolution, ResolutionLogType.RA, String.Concat("Creata attività JeepService per step di ", activityType.GetDescription(), " dell'atto."))
        FileLogger.Info(LoggerName, String.Concat("Creata attività JeepService per step di ", activityType.GetDescription(), " dell'atto n. ", resolution.Id))
    End Sub

    Public Sub CreatePublicationActivity(resolution As Resolution, documents As ResolutionActivityDocumentModel)
        CreateActivity(resolution, documents, "Pubblicazione Web", resolution.PublishingDate.Value, ResolutionActivityType.Publication)
    End Sub

    Public Sub CreateExecutiveActivity(resolution As Resolution, effectivenessDate As DateTimeOffset)
        CreateActivity(resolution, Nothing, "Passaggio in esecutività", effectivenessDate, ResolutionActivityType.Effectiveness)
    End Sub


    Public Function CountErrorActivities() As Long
        Return _dao.CountErrorActivities()
    End Function

    Public Function GetToBeProcessedByType(type As ResolutionActivityType) As IList(Of ResolutionActivity)
        Return _dao.GetToBeProcessedByType(type)
    End Function

    Public Function CheckPublicationActivityDate(resolution As Resolution, selectedDate As DateTime) As Boolean
        Return _dao.CheckPublicationActivityDate(resolution, selectedDate)
    End Function

End Class