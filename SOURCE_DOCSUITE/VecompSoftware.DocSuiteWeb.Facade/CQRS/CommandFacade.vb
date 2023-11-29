Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports Newtonsoft.Json

Public Class CommandFacade(Of T As ICommand)
    Implements ICommandFacade(Of T)

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
    Public Function Read(ByVal command As T) As T Implements ICommandFacade(Of T).Read
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' Invio messaggio di lettura utilizzando un controller custom
    ''' </summary>
    ''' <remarks>Da implementare</remarks>
    Public Function Read(ByVal apiController As String, ByVal command As T) As T Implements ICommandFacade(Of T).Read
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' Invio messaggio utilizzando un controller custom
    ''' </summary>
    Public Sub Push(ByVal command As T) Implements ICommandFacade(Of T).Push
        Try
            Dim outboxMessage As New OutboxMessage With {
                .UniqueId = command.Id,
                .MessageBody = JsonConvert.SerializeObject(command, _serializerSettings),
                .MessageType = MessageType.Command,
                .MessageTypeName = command.CommandName
            }

            FileLogger.Debug(LoggerName, $"Saving '{command.CommandName}' with id '{command.Id}' as outbox message '{JsonConvert.SerializeObject(outboxMessage, Formatting.Indented)}'")
            FacadeFactory.Instance.OutboxFacade.Save(outboxMessage)
            FileLogger.Debug(LoggerName, $"Outbox message for '{command.CommandName}' with id '{outboxMessage.UniqueId}' saved successfully")
        Catch ex As Exception
            FileLogger.Error(LoggerName, $"Error saving the outbox message for command '{command.CommandName}' with id '{command.Id}'", ex)

            Throw New DocSuiteException($"Error saving the outbox message for command '{command.CommandName}' with id '{command.Id}'", ex)
        End Try
    End Sub

#End Region

End Class
