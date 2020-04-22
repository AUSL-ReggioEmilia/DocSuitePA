Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Command.CQRS.Commands
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Parameters

Public Class CommandFacade(Of T As ICommand)
    Implements ICommandFacade(Of T)

#Region "Fields"

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
        If WebApi.SendRequest(Tenant.WebApiClientConfig, Tenant.OriginalConfiguration, command) Then
            FileLogger.Info(LoggerName, String.Format("[CommandFacade - Send] Comando {0} correttamente spedito", command.ToString()))
        Else
            Dim errorMessage As String = String.Format("Errore in esecuzione del comando {0}", command.ToString())
            FileLogger.Error(LoggerName, String.Format("[CommandFacade - Send] {0}", errorMessage))
            Throw New DocSuiteException("[CommandFacade - Send]", errorMessage)
        End If
    End Sub

#End Region

End Class
