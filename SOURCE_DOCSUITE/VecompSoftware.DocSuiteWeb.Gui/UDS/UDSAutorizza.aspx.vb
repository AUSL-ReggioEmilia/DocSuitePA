Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.UDS
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Logging
Imports System.Web

Partial Public Class UDSAutorizza
    Inherits UDSBasePage

#Region " Fields "

    Private _udsSource As UDSDto
    Private _currentRepositoryRigths As UDSRepositoryRightsUtil
    Public Const NOTIFICATION_ERROR_ICON As String = "delete"
    Public Const NOTIFICATION_SUCCESS_ICON As String = "ok"
    Public Const COMMAND_SUCCESS As String = "Attendere il termine dell'attività di {0}."
    Private Const ON_ERROR_FUNCTION As String = "onError('{0}')"
    Private Const UDS_SUMMARY_PATH As String = "~/UDS/UDSView.aspx?Type=UDS&IdUDS={0}&IdUDSRepository={1}"

#End Region

#Region " Properties "
    Public ReadOnly Property UDSSource As UDSDto
        Get
            If _udsSource Is Nothing Then
                _udsSource = GetSource()
            End If
            Return _udsSource
        End Get
    End Property

    Private ReadOnly Property CurrentRepositoryRigths As UDSRepositoryRightsUtil
        Get
            If _currentRepositoryRigths Is Nothing Then
                _currentRepositoryRigths = New UDSRepositoryRightsUtil(CurrentUDSRepository, DocSuiteContext.Current.User.FullUserName, UDSSource)
            End If
            Return _currentRepositoryRigths
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub


    Protected Sub UDSAutorizza_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As Object() = arg.Split("|"c)
        If arguments.Length = 0 Then
            Exit Sub
        End If

        Dim argumentName As String = arguments(0).ToString()

        Select Case argumentName
            Case "callback"
                Dim udsId As Guid = Guid.Parse(arguments(1).ToString())
                Dim udsRepositoryId As Guid = Guid.Parse(arguments(2).ToString())
                If Not udsId.Equals(Guid.Empty) AndAlso Not udsRepositoryId.Equals(Guid.Empty) Then
                    Dim url As String = UDS_SUMMARY_PATH
                    Response.Redirect(String.Format(url, udsId, udsRepositoryId))
                End If
        End Select
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        If UDSSource Is Nothing Then
            Exit Sub
        End If

        Try
            Dim correlationId As Guid = Guid.Empty
            If (Not Guid.TryParse(HFcorrelatedCommandId.Value, correlationId)) Then
                AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, "Errore generale, contattare assistenza : CorrelationId is not Valid."))
                Exit Sub
            End If

            Dim udsModel As UDSModel = UDSSource.UDSModel
            Dim models As ICollection(Of ReferenceModel) = New List(Of ReferenceModel)
            Dim model As ReferenceModel
            For Each role As Role In uscAutorizza.GetRoles()
                model = New ReferenceModel()
                model.AuthorizationType = AuthorizationType.Accounted
                model.EntityId = role.Id
                model.UniqueId = role.UniqueId
                models.Add(model)
            Next
            udsModel.FillAuthorizations(models)
            Dim sendedCommandId As Guid = CurrentUDSRepositoryFacade.SendCommandUpdateData(CurrentIdUDSRepository.Value, CurrentIdUDS.Value, correlationId, udsModel)
            FileLogger.Info(LoggerName, String.Format("Command sended with Id {0} and CorrelationId {0}", sendedCommandId, correlationId))
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Dim exceptionMessage As String = String.Format("Errore nella fase di salvataggio: {0}", ProtocolEnv.DefaultErrorMessage)
            AjaxManager.ResponseScripts.Add(String.Format(ON_ERROR_FUNCTION, HttpUtility.JavaScriptStringEncode(exceptionMessage)))
        End Try

    End Sub


#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UDSAutorizza_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAutorizza, uscAutorizza)
    End Sub

    Private Sub Initialize()

        If Not CurrentRepositoryRigths.IsAuthorizable Then
            Dim errorMessage As String = "Non è possibile visualizzare l'Archivio richiesto. Verificare se si dispone di sufficienti autorizzazioni."
            Throw New DocSuiteException(String.Format("Archivio: {0} n. {1}/{2:0000000} ", UDSSource.UDSModel.Model.Title, UDSSource.Year, UDSSource.Number, errorMessage))
        End If
        uscUDS.ActionType = uscUDS.ACTION_TYPE_AUTHORIZE
        uscUDS.UDSItemSource = UDSSource.UDSModel
        uscUDS.CurrentUDSRepositoryId = CurrentIdUDSRepository
        uscUDS.UDSYear = UDSSource.Year
        uscUDS.UDSNumber = UDSSource.Number
        uscUDS.UDSSubject = UDSSource.Subject
        uscUDS.UDSRegistrationDate = UDSSource.RegistrationDate
        uscUDS.UDSRegistrationUser = UDSSource.RegistrationUser
        uscUDS.UDSLastChangedDate = UDSSource.LastChangedDate
        uscUDS.UDSLastChangedUser = UDSSource.LastChangedUser
        uscUDS.UDSContainer = UDSSource.UDSRepository.Container
        uscUDS.UDSId = UDSSource.Id
        Dim roles As New List(Of Role)
        If UDSSource.Authorizations.Count() > 0 Then
            For Each item As UDSEntityRoleDto In UDSSource.Authorizations
                If item.IdRole.HasValue Then
                    Dim role As Role = Facade.RoleFacade.GetById(item.IdRole.Value)
                    roles.Add(role)
                End If
            Next
        End If

        If ProtocolEnv.MultiDomainEnabled AndAlso ProtocolEnv.TenantAuthorizationEnabled Then
            uscAutorizza.TenantEnabled = True
        End If

        uscAutorizza.SourceRoles = roles
        uscAutorizza.DataBind()

        Title = String.Format("{0} - Autorizzazioni", UDSSource.UDSModel.Model.Title)
    End Sub

    Private Sub ShowNotification(message As String, icon As String)
        udsNotification.ContentIcon = icon
        udsNotification.Text = message
        udsNotification.Show()
    End Sub

#End Region

End Class

