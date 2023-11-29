Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Services.Command.CQRS.Events
Imports VecompSoftware.Services.Logging

Public Class EventFacade(Of T As IEvent)
    Implements IEventFacade(Of T)

#Region "Fields"
    Private _serializerSettings As JsonSerializerSettings = New JsonSerializerSettings() With {
            .NullValueHandling = NullValueHandling.Ignore,
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            .PreserveReferencesHandling = PreserveReferencesHandling.All,
            .TypeNameHandling = TypeNameHandling.Objects}
#End Region

#Region "Properties"
    Public ReadOnly Property LoggerName As String
        Get
            Return LogName.FileLog
        End Get
    End Property

    Public ReadOnly Property WebApi As WebAPIHelper
        Get
            Return New WebAPIHelper()
        End Get
    End Property

    Public ReadOnly Property Tenant As TenantModel
        Get
            Return DocSuiteContext.Current.CurrentTenant
        End Get
    End Property
#End Region

#Region "Costructors"

#End Region

#Region "Methods"
    ''' <summary>
    ''' Invio messaggio di lettura utilizzando un controller base
    ''' </summary>
    ''' <remarks>Da implementare</remarks>
    Public Function Read(ByVal evt As T) As T Implements IEventFacade(Of T).Read
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' Invio messaggio di lettura utilizzando un controller custom
    ''' </summary>
    ''' <remarks>Da implementare</remarks>
    Public Function Read(ByVal apiController As String, ByVal evt As T) As T Implements IEventFacade(Of T).Read
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' Invio messaggio utilizzando un controller custom
    ''' </summary>
    Public Sub Push(ByVal evt As T) Implements IEventFacade(Of T).Push
        Try
            Dim outboxMessage As New OutboxMessage With {
                .UniqueId = evt.Id,
                .MessageBody = JsonConvert.SerializeObject(evt, _serializerSettings),
                .MessageType = MessageType.Event,
                .MessageTypeName = evt.EventName
            }

            FileLogger.Debug(LoggerName, $"Saving '{evt.EventName}' with id '{evt.Id}' as outbox message '{JsonConvert.SerializeObject(outboxMessage, Formatting.Indented)}'")
            FacadeFactory.Instance.OutboxFacade.Save(outboxMessage)
            FileLogger.Debug(LoggerName, $"Outbox message for '{evt.EventName}' with id '{outboxMessage.UniqueId}' saved successfully")
        Catch ex As Exception
            FileLogger.Error(LoggerName, $"Error saving the outbox message for event '{evt.EventName}' with id '{evt.Id}'", ex)

            Throw New DocSuiteException($"Error saving the outbox message for event '{evt.EventName}' with id '{evt.Id}'", ex)
        End Try
    End Sub
#End Region

End Class
