Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Logging

Partial Class CommIntro
    Inherits CommBasePage

#Region "Fields"
    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder
#End Region

#Region "Properties"
    Private ReadOnly Property CurrentTemplateCollaborationFinder As TemplateCollaborationFinder
        Get
            If _currentTemplateCollaborationFinder Is Nothing Then
                _currentTemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                _currentTemplateCollaborationFinder.ResetDecoration()
                _currentTemplateCollaborationFinder.EnablePaging = False
            End If
            Return _currentTemplateCollaborationFinder
        End Get
    End Property

    Private Property ShowErrorPanel As Boolean
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        imbUtente.Image.ImageUrl = ImagePath.SmallEmpty

        Dim userLabel As New StringBuilder()
        If DocSuiteContext.Current.ProtocolEnv.MultiDomainEnabled Then
            userLabel.AppendFormat("Utente: {0}\{1}", DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.User.UserName)
        Else
            userLabel.AppendFormat("Utente: {0}", DocSuiteContext.Current.User.UserName)
        End If
        If Not String.IsNullOrEmpty(CommonUtil.GetInstance().UserDescription) Then
            userLabel.AppendFormat(" - {0}", CommonUtil.GetInstance().UserDescription)
        End If
        UserConnected.Text = userLabel.ToString()

        If Not String.IsNullOrEmpty(CommonInstance.UserMail) Then
            UserMail.Text = "eMail: " & CommonInstance.UserMail
        End If

        imbUtente.NavigateUrl = String.Concat("../Utlt/UtltConfig.aspx?", CommonShared.AppendSecurityCheck("Type=Comm"))

        If CommonInstance.AppAccessOk Then
            UserDomain.Visible = False
        Else
            UserDomain.Visible = True
            UserDomain.Text = "Dominio: " & CommonShared.UserDomain
            AjaxAlert("Errore di Accesso.{0}L'Utente non appartiene al Dominio corretto{0}Verificare il campo Domain in ParameterEnv", Environment.NewLine)
        End If

        Version.Text = String.Format("Versione {0}", GetDSWVersion())
        If CommonShared.HasGroupAdministratorRight Then
            AddVersion("VecompSoftware.Clients.WebAPI")
            AddVersion("VecompSoftware.Core.Command")
            AddVersion("VecompSoftware.DocSuiteWeb.Entity")
            AddVersion("VecompSoftware.DocSuiteWeb.Model")
            AddVersion("VecompSoftware.DocSuiteWeb.Data")
            AddVersion("VecompSoftware.DocSuiteWeb.Data.Entity")
            AddVersion("VecompSoftware.DocSuiteWeb.Facade")
            AddVersion("VecompSoftware.Helpers")
            AddVersion("VecompSoftware.Helpers.EInvoice")
            AddVersion("VecompSoftware.Helpers.NHibernate")
            AddVersion("VecompSoftware.Helpers.PDF")
            AddVersion("VecompSoftware.Helpers.Web")
            AddVersion("VecompSoftware.Helpers.Compress")
            AddVersion("VecompSoftware.Helpers.XML")
            AddVersion("VecompSoftware.Helpers.XML.Converters")
            AddVersion("VecompSoftware.NHibernateManager")
            AddVersion("VecompSoftware.Services.Biblos")
            AddVersion("VecompSoftware.Services.Logging")
            AddVersion("VecompSoftware.Services.Command")
            AddVersion("VecompSoftware.Services.StampaConforme")
            AddVersion("VecompSoftware.Services.SignService")
        Else
            rowVersions.Visible = False
        End If

        ' Se presente imposto il logo aziendale
        imgLogo.ImageUrl = "Images/home/docsuite-pa-blue.png"
        Try
            If File.Exists(CommonInstance.AppPath & "Comm\Images\Home\AziendaLogo.Gif") Then
                imgLogo.ImageUrl = "Images/home/AziendaLogo.gif"
            End If
            If File.Exists(CommonInstance.AppPath & "Comm\Images\Home\AziendaLogo.jpg") Then
                imgLogo.ImageUrl = "Images/home/AziendaLogo.jpg"
            End If
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore nei caricamenti immagini iniziali.", ex)
        End Try

        ''Gestione protocolli annullati
        CheckProtocolsToRecover()
        'Gestione Template di collaborazione annullati
        CheckTemplateCollaborationsToRecover()
        'Gestione attività di avanzamento flusso atti in errore
        CheckResolutionActivities()
        'Gestione caselle pec in errore
        CheckPecMailBoxes()

        InitializeErrorPanel()
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeErrorPanel()
        panelSeparator.Visible = ShowErrorPanel
        errorPanel.Visible = ShowErrorPanel
        mainPanel.Width = If(ShowErrorPanel, Unit.Percentage(80), Unit.Percentage(100))
    End Sub
    Private Sub AddVersion(name As String)
        Dim assembly As Assembly = Assembly.Load(name)
        If assembly Is Nothing Then
            Return
        End If

        Dim lblName As New Label With {
            .Text = assembly.GetName().Name.Replace("VecompSoftware.", String.Empty),
            .Width = 300
        }
        phVersions.Controls.Add(lblName)

        Dim lbl As New Label With {
            .Text = String.Format("V. {0} Revision {1}", assembly.GetName().Version.ToString(3), assembly.GetName().Version.Revision),
            .Width = 180
        }
        phVersions.Controls.Add(lbl)
    End Sub

    Private Function GetDSWVersion() As String
        If CommonShared.HasGroupAdministratorRight Then
            Return Assembly.GetExecutingAssembly().GetName().Version.ToString(4)
        Else
            Return Assembly.GetExecutingAssembly().GetName().Version.ToString(3)
        End If
    End Function

    Private Sub CheckProtocolsToRecover()
        If Not DocSuiteContext.Current.ProtocolEnv.IsProtocolRecoverEnabled OrElse Not DocSuiteContext.Current.ProtocolEnv.CheckRecoverToProtocol Then
            Exit Sub
        End If

        Dim protocolliErrati As Integer = Facade.ProtocolFacade.GetRecoveringProtocolsFinder.Count()
        If protocolliErrati > 0 Then
            Dim howManyProtocols As String = If(protocolliErrati = 1, "È presente 1 protocollo ", "Sono presenti {0} protocolli ")
            lblRecoveringProtocols.Text = String.Format(String.Format("{0} in stato di errore ancora da gestire.<br/>Utilizzare la gestione ""Recupero Errori"" oppure cliccare su Correggi", howManyProtocols), protocolliErrati)
            lblRecoveringProtocols.Visible = True
            btnProtocolCorrect.Visible = True
            ShowErrorPanel = True
        End If
    End Sub

    Private Sub CheckTemplateCollaborationsToRecover()
        If Not CommonShared.UserConnectedBelongsTo(DocSuiteContext.Current.ProtocolEnv.TemplateCollaborationGroups) Then
            Exit Sub
        End If

        Try
            Dim templates As ICollection(Of WebAPIDto(Of TemplateCollaboration)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.Status = TemplateCollaborationStatus.NotActive
                        Return finder.DoSearch()
                    End Function)

            If templates.Count > 0 Then
                lblRecoveringTemplateCollaborations.Text = String.Format("Sono presenti {0} Template in stato di errore ancora da gestire.<br/>Cliccare su Correggi Template", templates.Count)
                lblRecoveringTemplateCollaborations.Visible = True
                btnTemplateCollaborationCorrects.Visible = True
                ShowErrorPanel = True
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
        End Try
    End Sub

    Private Sub CheckResolutionActivities()
        If Not DocSuiteContext.Current.ResolutionEnv.AutomaticActivityStepEnabled OrElse Not CommonShared.UserConnectedBelongsTo(ResolutionEnv.ErrorAutomaticActivitiesGroup.ToString()) Then
            Exit Sub
        End If

        Dim errorActivities As Long = Facade.ResolutionActivityFacade.CountErrorActivities()
        If errorActivities > 0 Then
            Dim howManyActivities As String = If(errorActivities = 1, "È presente 1 attività di avanzamento automatico flusso atti ", "Sono presenti {0} attività di avanzamento automatico flusso atti ")
            lblRecoverResolutionActivities.Text = String.Format(String.Format("{0} in stato di errore ancora da gestire.<br/>Cliccare su Vedi attività in errore.", howManyActivities), errorActivities)
            lblRecoverResolutionActivities.Visible = True
            btnRecoverResolutionActivities.Visible = True
            ShowErrorPanel = True
        End If
    End Sub

    Private Sub CheckPecMailBoxes()
        If Not CommonShared.HasGroupAdministratorRight OrElse Not DocSuiteContext.Current.ProtocolEnv.IsPECEnabled Then
            Exit Sub
        End If

        Dim loginErrors As Long = Facade.PECMailboxFacade.CountLoginErrorPECBoxes()
        If loginErrors > 0 Then
            Dim howManyErrors As String = If(loginErrors = 1, "È presente 1 casella pec ", "Sono presenti {0} caselle pec ")
            lblPECMailBoxError.Text = String.Format(String.Format("{0} in stato di errore ancora da gestire.<br/>Cliccare su Vedi caselle pec in errore.", howManyErrors), loginErrors)
            lblPECMailBoxError.Visible = True
            btnPecMailBoxError.Visible = True
            ShowErrorPanel = True
        End If
    End Sub

#End Region

End Class


