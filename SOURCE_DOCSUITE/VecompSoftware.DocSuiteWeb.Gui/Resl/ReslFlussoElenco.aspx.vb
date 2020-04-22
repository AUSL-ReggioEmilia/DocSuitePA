Imports System.Net.Mail
Imports System.Linq
Imports System.Globalization
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers.PDF
Imports Telerik.Web.UI.Calendar
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Facade.Report
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging
Imports Microsoft.Reporting.WebForms
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.DocSuiteWeb.Gui.ResolutionGridBarController
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates

Partial Public Class ReslFlussoElenco
    Inherits ReslBasePage

#Region " Fields "

    Private _selezione As IList(Of Integer)
    Private _myStep As BarWorkflowStep
    Private _resolutionDocumentSeriesItemFacade As Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade
    Private _roles As IList(Of Role)

#End Region

#Region " Properties "

    Private ReadOnly Property SelezioneLista() As IList(Of Integer)
        Get
            If _selezione Is Nothing Then
                _selezione = CreateSelectionList(Selezione)
            End If
            Return _selezione
        End Get
    End Property

    Private ReadOnly Property Selezione() As String
        Get
            Return Request.QueryString("Selezione").Replace("-", ",")
        End Get
    End Property

    Private ReadOnly Property Tipologia() As Short
        Get
            Return Request.QueryString.GetValue(Of Short)("Tipologia")
        End Get
    End Property

    Public ReadOnly Property CurrentResolutionDocumentSeriesItemFacade As Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade
        Get
            If _resolutionDocumentSeriesItemFacade Is Nothing Then
                _resolutionDocumentSeriesItemFacade = New Facade.NHibernate.Resolutions.ResolutionDocumentSeriesItemFacade()
            End If
            Return _resolutionDocumentSeriesItemFacade
        End Get
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        uscLetteraRegione.IsRequired = False
        uscLetteraGestione.IsRequired = False

        WebUtils.ObjAttDisplayNone(txtLastWorkflowDate)
        InitializeAjax()
        If Not String.IsNullOrEmpty(Action) Then
            If Action.Eq("Pubblicazione") And Tipologia = ResolutionType.IdentifierDetermina Then
                rdpData.AutoPostBack = True
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpData, pnlDataFine)
                AjaxManager.AjaxSettings.AddAjaxSetting(rdpData, rdpDataFine)
                DirectCast(rdpData.Controls(0), RadDateInput).Attributes.Add("disabled", "disabled")
            Else
                rdpData.AutoPostBack = False
                DirectCast(rdpData.Controls(0), RadDateInput).Attributes.Remove("disabled")
            End If
        End If

        If Not IsPostBack Then Initialize()
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim confirmSuccess As Boolean = False
        Dim workStep As TabWorkflow = Nothing
        Dim fileTemp As String = String.Empty
        ' Dim sFiles As String
        CommonInstance.UserDeleteTemp(TempType.P)

        If (ResolutionEnv.Configuration.Eq(ConfTo)) Then
            Try
                FileHelper.CopySafe(String.Concat(CommonInstance.AppPath, ResolutionFacade.PathStampeTo, "logo.jpg"), String.Concat(CommonInstance.AppTempPath, "logo.jpg"), True)
                FileHelper.CopySafe(String.Concat(CommonInstance.AppPath, ResolutionFacade.PathStampeTo, "logo2.jpg"), String.Concat(CommonInstance.AppTempPath, "logo2.jpg"), True)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, ex.Message, ex)
            End Try
        End If
        'setto la pagina e l'oggetto in base alla tipologia
        Dim sObject As String = String.Empty
        Dim gestioneDigitale As Boolean = False
        Select Case Tipologia
            Case ResolutionType.IdentifierDelibera
                sObject = "Delibere"
                gestioneDigitale = ResolutionEnv.GestioneDigitaleDelibere
            Case ResolutionType.IdentifierDetermina
                sObject = "Determine"
                gestioneDigitale = ResolutionEnv.GestioneDigitaleDetermine
        End Select

        Dim adoptionDate As String = String.Empty


        If SelezioneLista.Count > 0 Then
            Dim resl As Resolution = Facade.ResolutionFacade.GetById(SelezioneLista(0))
            If resl IsNot Nothing Then
                adoptionDate = resl.AdoptionDate.DefaultString
            End If

        End If

        'in base all'Action
        Select Case Action
            Case "Adozione"
                If (ResolutionEnv.Configuration.Eq(ConfTo)) Then
                    ToAdoption(gestioneDigitale, fileTemp, confirmSuccess, workStep)
                ElseIf (ResolutionEnv.Configuration.Eq(ConfAuslPc)) Then
                    PcAdoption(confirmSuccess, workStep)
                End If

            Case "TrasmDaAffariGenerali"
                If (ResolutionEnv.Configuration.Eq(ConfTo)) Then
                    ToCollaborationFromAffariGenerali(confirmSuccess)
                End If

            Case "TrasmAvvenutaAdozione"
                '-- Salvo la data
                confirmSuccess = Facade.ResolutionFacade.UpdateProposerWarningDate(Selezione, rdpData.SelectedDate, DocSuiteContext.Current.User.FullUserName)
                '-- 
                Dim resolutions As IList(Of Resolution) = Facade.ResolutionFacade.GetResolutions(SelezioneLista)
                fileTemp = String.Concat(CommonUtil.UserDocumentName, "-Print-", String.Format("{0:HHmmss}", Now()), FileHelper.PDF)
                Dim fSource As String
                Dim fDestination As String

                If resolutions IsNot Nothing AndAlso resolutions.Count > 0 Then

                    fileTemp = String.Concat(CommonUtil.UserDocumentName, "-Print-", String.Format("{0:HHmmss}", Now()), FileHelper.PDF)
                    fSource = String.Concat(CommonInstance.AppPath, ResolutionFacade.PathStampeTo, "LetteraTrasmAvvenutaAdozione.rdlc")
                    fDestination = String.Concat(CommonInstance.AppTempPath, fileTemp)

                    Dim rpt As New ReslLetteraTrasmAvvenutaAdozione
                    rpt.DataSource = resolutions
                    rpt.RdlcPrint = fSource
                    rpt.DoPrint()

                    Dim p(3) As ReportParameter
                    p(0) = New ReportParameter("DataStampa", Format(rdpData.SelectedDate, "dd/MM/yyyy"))
                    p(1) = New ReportParameter("Heading", resolutions(0).Container.HeadingLetter)
                    p(2) = New ReportParameter("Ruolo", cboRuolo.SelectedItem.Text)
                    p(3) = New ReportParameter("NomeCognome", txtFirma.Text)
                    rpt.TablePrint.LocalReport.SetParameters(p)

                    Dim rendered As Byte() = rpt.TablePrint.LocalReport.Render("PDF", Nothing, "", "", "", Nothing, Nothing)
                    File.WriteAllBytes(fDestination, rendered)
                End If

            Case "TrasmAdozioneCollegioSindacaleFirmaDigitale"
                Dim dateFrom As Date
                Dim dateTo As Date
                Dim adoptionDateFrom As String = String.Empty
                Dim adoptionDateTo As String = String.Empty
                Dim reslList As List(Of Resolution) = New List(Of Resolution)

                For Each item As Integer In SelezioneLista
                    reslList.Add(Facade.ResolutionFacade.GetById(item))
                Next

                reslList.OrderBy(Function(x) x.AdoptionDate)
                adoptionDateFrom = reslList.FirstOrDefault.AdoptionDate.DefaultString
                adoptionDateTo = reslList.LastOrDefault.AdoptionDate.DefaultString

                If DateTime.TryParse(adoptionDateFrom, dateFrom) AndAlso DateTime.TryParse(adoptionDateTo, dateTo) Then
                    confirmSuccess = ToAdoptionCsDigitalSignatureAction(dateFrom, dateTo, reslList.FirstOrDefault.Container.HeadingLetter)
                Else
                    AjaxAlert("Date inserite non valide.")
                End If

            Case "TrasmAdozioneOrganoControllo"
                '-- Salvataggio del num. Protocollo e date per le delibere selezionate
                Dim protocols As IList(Of Protocol)

                Dim pCollegio As Protocol = Nothing
                protocols = uscLetteraCollegio.GetProtocols()
                If Not protocols.IsNullOrEmpty Then
                    pCollegio = protocols(0)
                End If

                Dim pRegione As Protocol = Nothing
                protocols = uscLetteraRegione.GetProtocols()
                If Not protocols.IsNullOrEmpty Then
                    pRegione = protocols(0)
                End If

                Dim pGestione As Protocol = Nothing
                protocols = uscLetteraGestione.GetProtocols()
                If Not protocols.IsNullOrEmpty Then
                    pGestione = protocols(0)
                End If

                confirmSuccess = Facade.ResolutionFacade.UpdateOC(Selezione, rdpSpedCollegioData.SelectedDate, pCollegio, rdpSpedData.SelectedDate, pRegione, rdpSpedGestioneData.SelectedDate, pGestione, DocSuiteContext.Current.User.FullUserName)

                If ResolutionEnv.IsSendMailEnabled Then
                    Dim sError As String = ""
                    'Recuperare il protocollo della lettera (trasmissione al collegio - tiff)
                    Try
                        Dim letteraFileInfo As DocumentInfo = ProtocolFacade.RecuperoProtocollo(pCollegio, "Lettera")
                        Select Case Tipologia
                            Case ResolutionType.IdentifierDelibera
                                Dim subject As String = String.Format("Invio Elenco {0} Adottate - Data Adozione {1}", sObject, adoptionDate)
                                Facade.UserErrorFacade.SmtpLogSendMail(subject, "", letteraFileInfo, sError, ResolutionEnv.SupervisoryMailTo)
                            Case ResolutionType.IdentifierDetermina
                                Dim subject As String = String.Format("Invio Elenco {0} Adottate", sObject)
                                Facade.UserErrorFacade.SmtpLogSendMail(subject, "", letteraFileInfo, sError, ResolutionEnv.OCDetermMailTo)
                        End Select
                    Catch ex As Exception
                        Dim errorMsg As String = String.Format("Impossibile inviare.{0}Impossibile recuperare il Protocollo di Trasmissione.", Environment.NewLine)
                        FileLogger.Error(LoggerName, errorMsg, ex)
                        Facade.ProtocolLogFacade.Insert(pCollegio, ProtocolLogEvent.PE, String.Concat("Errore in Recupero Protocollo: ", ex.Message))

                        AjaxAlert(errorMsg)
                    End Try
                    Try
                        'Solo per le delibere
                        If Tipologia = ResolutionType.IdentifierDelibera Then
                            '--Invio Mail Controllo di Gestione (lettera di trasmissione)
                            If pGestione IsNot Nothing Then
                                'Recuperare il protocollo della lettera (trasmissione al contr. di Gestione - tiff)
                                Dim letteraFileInfo As DocumentInfo = ProtocolFacade.RecuperoProtocollo(pGestione, "Lettera")
                                Dim subject As String = String.Format("Invio Elenco {0} Adottate", sObject)
                                Facade.UserErrorFacade.SmtpLogSendMail(subject, "", letteraFileInfo, sError, ResolutionEnv.ManagementMailTo)
                            End If
                        End If
                    Catch ex As Exception
                        Facade.ProtocolLogFacade.Insert(pGestione, ProtocolLogEvent.PE, "Errore in Recupero Protocollo: " & ex.Message)
                        FileLogger.Warn(LoggerName, String.Concat("Errore in Recupero Protocollo: ", ex.Message), ex)

                        AjaxAlert("Impossibile inviare Protocollo di Controllo di Gestione.")
                    End Try
                End If
            Case "Esecutivita"
                '-- Esecutività le delibere
                confirmSuccess = Facade.ResolutionFacade.UpdateEffectivenessDate(Selezione, rdpData.SelectedDate, DocSuiteContext.Current.User.FullUserName)

                Dim resolutions As IList(Of Resolution) = Facade.ResolutionFacade.GetResolutions(SelezioneLista)
                For Each item As Resolution In resolutions
                    Facade.ResolutionFacade.SendUpdateResolutionCommand(item)
                Next

                If Tipologia = ResolutionType.IdentifierDelibera Then
                    'Genera lettera
                    'If b Then b = ResolutionUtil.GetInstance.GeneraLettera(Tipologia, SelezioneLista, "E", FileTemp)
                End If
                fileTemp = ""
            Case "Pubblicazione"
                confirmSuccess = ToPublication()
        End Select

        If confirmSuccess Then
            If String.IsNullOrEmpty(fileTemp) Then
                AjaxManager.ResponseScripts.Add("return CloseWindow('');")
            Else
                AjaxManager.ResponseScripts.Add(String.Concat("return CloseWindow('", CommonInstance.AppTempHttp, fileTemp, "');"))
            End If
        Else
            AjaxAlert(String.Format("Sono occorsi degli errori durante l'avanzamento di step. Contattare l'assistenza tecnica fornendo l'ID {0}-{1}", Action, String.Join("#", SelezioneLista)))
        End If
    End Sub

    Private Sub RdpDataSelectedDateChanged(ByVal sender As Object, ByVal e As SelectedDateChangedEventArgs) Handles rdpData.SelectedDateChanged
        If Not String.IsNullOrEmpty(Action) AndAlso (Action.Eq("Pubblicazione") AndAlso Tipologia = ResolutionType.IdentifierDetermina) Then
            If rdpData.SelectedDate.HasValue Then
                rdpDataFine.SelectedDate = DateAdd(DateInterval.Day, 10, rdpData.SelectedDate.Value)
            End If
        End If
    End Sub

    Private Sub UscLetteraCollegioProtocolAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscLetteraCollegio.ProtocolAdded
        Dim p As Protocol = uscLetteraCollegio.GetProtocols()(0)
        If Tipologia = ResolutionType.IdentifierDetermina Then
            uscLetteraGestione.AddProtocol(p)
        End If
    End Sub

    Private Sub UscLetteraCollegioProtocolRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscLetteraCollegio.ProtocolRemoved
        If Tipologia = ResolutionType.IdentifierDetermina Then
            uscLetteraGestione.ClearProtocols()
        End If
    End Sub

    Private Sub UscLetteraGestioneProtocolAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscLetteraGestione.ProtocolAdded
        Dim p As Protocol = uscLetteraGestione.GetProtocols()(0)
        If Tipologia = ResolutionType.IdentifierDetermina Then
            uscLetteraCollegio.AddProtocol(p)
        End If
    End Sub

    Private Sub UscLetteraGestioneProtocolRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscLetteraGestione.ProtocolRemoved
        If Tipologia = ResolutionType.IdentifierDetermina Then
            uscLetteraCollegio.ClearProtocols()
        End If
    End Sub

    Private Sub UscVisioneFirmaContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscVisioneFirma.RoleUserContactAdded
        AddRoleContact(uscVisioneFirma)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(rdpSpedCollegioData, rdpSpedData)
        AjaxManager.AjaxSettings.AddAjaxSetting(rdpSpedCollegioData, rdpSpedGestioneData)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscLetteraCollegio, uscLetteraGestione.TreeViewControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscLetteraGestione, uscLetteraCollegio.TreeViewControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub Initialize()
        'Recupero il Finder per avere maggiori informazioni sulla ricerca fatta
        Dim finder As NHibernateResolutionWorkflowFinder = SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ReslFlussoFinderType)

        Dim s As String = Nothing
        Dim sData As Date?
        CommonInstance.UserDeleteTemp(TempType.I)

        pnlNextStep.Visible = True
        rdpData.Enabled = True
        pnlMessage.Visible = False
        '-- Lettera (servizi - pubblicazione - esecutività)
        uscLettera.ButtonSelectVisible = DocSuiteContext.Current.IsProtocolEnabled

        '-- Lettera al Collegio Sindacale
        uscLetteraCollegio.ButtonSelectVisible = DocSuiteContext.Current.IsProtocolEnabled

        '-- Lettera alla Regione
        uscLetteraRegione.ButtonSelectVisible = DocSuiteContext.Current.IsProtocolEnabled

        '-- Lettera C. Gestione
        uscLetteraGestione.ButtonSelectVisible = DocSuiteContext.Current.IsProtocolEnabled

        '-- Ruolo
        cboRuolo.Items.Clear()
        WebUtils.ObjDropDownListAdd(cboRuolo, "Il Funzionario Delegato", "F")
        WebUtils.ObjDropDownListAdd(cboRuolo, "Il Responsabile del Procedimento", "R")
        WebUtils.ObjDropDownListAdd(cboRuolo, "Il Direttore", "D")
        '--
        Select Case Action
            Case "TrasmDaAffariGenerali"
                lblFlusso.Text = "Data Trasmissione"
                lblTitolo.Text = "Avvia collaborazione"
                pnlNextStep.Visible = False
                pnlData.Visible = False
                pnlFirma.Visible = False
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = False
                pnlCollaborationSigner.Visible = False
                pnlServiceCode.Visible = False
                lblActualStep.Text = "Agli Affari Generali"
                lblNextStep.Text = "Da Adottare"
            Case "Adozione"
                lblFlusso.Text = "Data Flusso"
                lblTitolo.Text = "Adozione - Inserimento"
                lblActualStep.Text = "Proposta"
                lblNextStep.Text = "Adozione"
                pnlData.Visible = True
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = False
                pnlFirma.Visible = False
                pnlCollaborationSigner.Visible = False
                pnlServiceCode.Visible = False
                ' Solo per Torino visualizzo il pannello per la gestione del frontalino digitale
                If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
                    pnlFrontalino.Visible = True
                End If
                '--
                Dim codeWorkflow As String = Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, Tipologia)
                _myStep = Facade.TabWorkflowFacade.GetByDescription(Action, codeWorkflow).Id.ResStep

                If Facade.TabWorkflowFacade.SqlTabWorkflowManagedWData(0, codeWorkflow, _myStep, s) Then
                    rdpData.Enabled = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "INS", "")
                    '--
                    rfvData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "OBB", "")
                    sData = Facade.ResolutionFacade.GetMinMaxWorkflowDate(SelezioneLista, "ProposeDate")
                    If sData.HasValue Then
                        cvCompareData.ErrorMessage &= String.Format("{0:dd/MM/yyyy}", sData)
                        txtLastWorkflowDate.Text = String.Format("{0:dd/MM/yyyy}", sData)
                    End If
                    cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "INS", "Tested")
                    If TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "TODAY", "") Then
                        rdpData.SelectedDate = Date.Today
                    End If
                    If (TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "ServiceNumber", "INS", "")) Then
                        pnlServiceCode.Visible = True
                        DoDataBindDdlServizio()
                        rfvServizio.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "ServiceNumber", "OBB", "")
                    End If
                End If

            Case "TrasmAvvenutaAdozione"
                lblFlusso.Text = "Data Trasmissione"
                lblTitolo.Text = "Lettera Trasmissione Avvenuta Adozione - Salva"
                pnlNextStep.Visible = False
                pnlData.Visible = True
                pnlFirma.Visible = True
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = False
                pnlCollaborationSigner.Visible = False
                pnlServiceCode.Visible = False

            Case "TrasmAdozioneCollegioSindacaleFirmaDigitale"
                lblActualStep.Text = "Adozione"
                lblNextStep.Text = "Trasmissione a Collegio Sindacale con Firma Digitale"
                lblTitolo.Text = "Trasmissione Adozione Collegio Sindacale con Firma Digitale"
                pnlData.Visible = False
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = False
                pnlFirma.Visible = False
                pnlCollaborationSigner.Visible = True
                pnlServiceCode.Visible = False

                Dim containers As IList(Of Container) = SelezioneLista.Select(Function(idr) Facade.ResolutionFacade.GetById(idr).Container).ToList()
                Dim nameContainers As String = String.Join(","c, containers.GroupBy(Function(grp) grp.Id) _
                                               .Select(Function(c) c.First()) _
                                               .ToList() _
                                               .Select(Function(cn) cn.Name))

                lblCollaborationSignerWarning.Text = "Inserire i firmatari. Verrà creata una unica collaborazione per tutti i contenitori."
                'carico il testo dell'oggetto
                txtCollaborationObject.Text = String.Format("Trasmissione elenco e copie delle {0}{1} adottate in data {2} per invio al Collegio Sindacale",
                                                            If(Tipologia = ResolutionType.IdentifierDelibera, "deliberazioni", "determinazioni"),
                                                            String.Format(" contenitore ""{0}""",
                                                            nameContainers),
                                                    finder.InDate.DefaultString)

            Case "TrasmAdozioneOrganoControllo"
                lblFlusso.Text = "Trasmissione Adozione Organi di Controllo"
                lblActualStep.Text = "Adozione"
                lblNextStep.Text = "Trasmissione Organi di Controllo"
                lblTitolo.Text = "Lettera Trasmissione Adozione all'Organo di Controllo - Salva"
                pnlData.Visible = False
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = True
                pnlFirma.Visible = False
                pnlCollaborationSigner.Visible = False
                pnlServiceCode.Visible = False


            Case "Pubblicazione"
                pnlMessage.Visible = True
                lblFlusso.Text = "Data Flusso"
                lblTitolo.Text = "Pubblicazione - Inserimento"
                lblActualStep.Text = "Invio OC"
                lblNextStep.Text = "Pubblicazione"
                pnlData.Visible = False
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = False
                pnlCollaborationSigner.Visible = False
                pnlFirma.Visible = False
                pnlServiceCode.Visible = False
                txtCollaborationObject.Text = String.Concat("Certificazione di avvenuta pubblicazione all'albo aziendale on-line delle ",
                                                            If(Tipologia = ResolutionType.IdentifierDelibera, "deliberazioni", "determinazioni"))
                _myStep = BarWorkflowStep.Pubblicazione

            Case "Esecutivita"
                lblFlusso.Text = "Data Flusso"
                lblTitolo.Text = "Esecutività - Inserimento"
                lblActualStep.Text = "Pubblicazione"
                lblNextStep.Text = "Esecutività"
                pnlData.Visible = True
                pnlLettera.Visible = False
                pnlOrganoContollo.Visible = False
                pnlFirma.Visible = False
                pnlCollaborationSigner.Visible = False
                pnlServiceCode.Visible = False

                _myStep = 4
                Dim ss As String = Request.QueryString("PublishingDate")
                If Not String.IsNullOrEmpty(ss) Then ss = DateTime.ParseExact(ss, "yyyyMMdd", Nothing)
                rdpData.SelectedDate = DateAdd(DateInterval.Day, 10, CommonUtil.ConvData(ss))
                '--
                Dim codeWorkflow As String = Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, Tipologia)
                If Facade.TabWorkflowFacade.SqlTabWorkflowManagedWData(0, codeWorkflow, _myStep, s) Then
                    rdpData.Enabled = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "INS", "")
                    '--
                    rfvData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "OBB", "")
                    sData = Facade.ResolutionFacade.GetMinMaxWorkflowDate(SelezioneLista, "ProposeDate")
                    If sData.HasValue Then
                        cvCompareData.ErrorMessage &= String.Format("{0:dd/MM/yyyy}", sData)
                        txtLastWorkflowDate.Text = String.Format("{0:dd/MM/yyyy}", sData)
                    End If
                    cvCompareData.Visible = TabWorkflowFacade.TestManagedWorkflowDataProperty(s, "Date", "INS", "Tested")
                End If

        End Select

        MasterDocSuite.HistoryTitle = lblTitolo.Text
    End Sub

    Private Function GeneraFrontalino(ByVal idResl As Integer, ByRef fileText As String, ByRef sw As StreamWriter, ByVal isFirst As Boolean) As Boolean
        Dim s As String
        Dim resl As Resolution = Facade.ResolutionFacade.GetById(idResl)

        If resl IsNot Nothing Then
            If (Not isFirst) Then
                sw.Write("<P CLASS='BREAK'></P><BR>")
            End If
            'convert
            fileText = fileText.Replace("@CODIFICA@", "<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">")
            fileText = fileText.Replace("@NUMERO@", resl.NumberFormat("{0:0000000}"))
            fileText = fileText.Replace("@CODICESERV@", UCase("" & resl.ResolutionContactProposers.ElementAt(0).Contact.Code))
            fileText = fileText.Replace("@ANNO@", resl.Year)
            fileText = fileText.Replace("@DATAADOZIONE@", Format(resl.AdoptionDate, "dd MMMM yyyy"))
            s = StringHelper.ReplaceCrLf(resl.ResolutionObject)
            fileText = fileText.Replace("@OGGETTO@", s)
            fileText = fileText.Replace("@ANNOLET@", StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Year)))
            fileText = fileText.Replace("@GIORNOLET@", StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Day)))
            fileText = fileText.Replace("@MESELET@", StringHelper.UppercaseFirst(resl.AdoptionDate.Value.MonthName))
            fileText = fileText.Replace("@ART14@", If(resl.OCSupervisoryBoard.GetValueOrDefault(False), "art.14", ""))
            fileText = fileText.Replace("@REGIONE@", If(resl.OCRegion.GetValueOrDefault(False), "Regione", ""))
            fileText = fileText.Replace("@GESTIONE@", If(resl.OCManagement.GetValueOrDefault(False), "Controllo Gestione", ""))
            fileText = fileText.Replace("@OCCorteConti@", If(resl.OCCorteConti.GetValueOrDefault(False), "Corte dei Conti", ""))

            s = StringHelper.ReplaceCrLf(resl.OtherDescription)
            fileText = fileText.Replace("@ALTRO@", If(resl.OCOther.GetValueOrDefault(False), s, ""))
            Dim sFront As String = If(String.IsNullOrEmpty(resl.Container.HeadingFrontalino), String.Empty, resl.Container.HeadingFrontalino)
            sFront = sFront.Replace("@MOTIVAZIONE@", "")
            fileText = fileText.Replace("@HEADINGF@", sFront)
            sw.Write(fileText)
            Return True
        End If
    End Function

    Private Sub AddRoleContact(ByRef contattiControl As uscContattiSel)
        If String.IsNullOrEmpty(contattiControl.JsonContactAdded) Then
            Exit Sub
        End If

        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(contattiControl.JsonContactAdded)

        Try
            Dim lastAddedNode As RadTreeNode = contattiControl.TreeViewControl.Nodes(0).Nodes.FindNodeByText(contact.Description)
            If Not lastAddedNode Is Nothing Then
                lastAddedNode.Checked = True
            End If
        Catch ex As Exception
            FileLogger.Warn(Facade.CollaborationFacade.LoggerName, ex.Message, ex)
        End Try
    End Sub

    Private Function GenerateReportAndCollaboration(ByVal parameters As Dictionary(Of String, String), ByVal rdlcPath As FileInfo, ByVal resolutions As IList(Of Resolution), ByVal collaborationTitle As String, ByVal toWorkflowStep As TOWorkflow) As Integer
        Return GenerateReportAndCollaboration(parameters, rdlcPath, Nothing, resolutions, collaborationTitle, toWorkflowStep)
    End Function

    Private Function GenerateReportAndCollaboration(ByVal parameters As Dictionary(Of String, String), ByVal rdlcPath As FileInfo, ByVal container As Container, ByVal resolutions As IList(Of Resolution), ByVal collaborationTitle As String, ByVal toWorkflowStep As TOWorkflow) As Integer
        'Aggiorno il valore di Container se presente altrimenti lo aggiungo
        If container IsNot Nothing Then
            If parameters.ContainsKey("Container") Then
                parameters("Container") = container.Name
            Else
                parameters.Add("Container", container.Name)
            End If
        End If

        'Genero effettivamente il report
        Dim filename As String = String.Format("Trasmissione elenco {0} adottate.pdf", If(Tipologia = ResolutionType.IdentifierDelibera, "deliberazioni", "determinazioni"))
        Dim doc As DocumentInfo = ReportFacade.GenerateReport(Of Resolution)(rdlcPath, parameters, resolutions).DoPrint(filename)

        ' Utenti firmatari
        Dim signers As New List(Of CollaborationContact)
        SetContactM(uscVisioneFirma.TreeViewControl.Nodes(0), signers)
        Try
            Return Facade.ResolutionFacade.StartLetterProcess(doc, resolutions, signers, collaborationTitle, toWorkflowStep)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Problema inserimento da collaborazione", ex)
            If container IsNot Nothing Then
                AjaxAlert(String.Format("Impossibile generare la collaborazione per il contenitore {0} ({1})", container.Name, container.Id))
            Else
                AjaxAlert("Impossibile generare la collaborazione")
            End If
            Return -1
        End Try
    End Function

    Private Shared Function DuplicaDocumento(ByVal sourceList As List(Of DocumentInfo), ByRef idCatena As Integer, ByVal resl As Resolution, ByVal append As Boolean, ByVal onlySigned As Boolean, ByVal signature As String, Optional addNewDoc As Boolean = False) As Integer
        Dim documents As New List(Of BiblosDocumentInfo)

        '' Se ho un idCatena valido allora carico i documenti presenti su tale id (eventualmente solo se firmati)
        If idCatena > 0 Then
            For Each biblosDocumentInfo As BiblosDocumentInfo In From biblosDocumentInfo1 In BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idCatena) Where Not onlySigned OrElse biblosDocumentInfo1.IsSigned
                biblosDocumentInfo.Signature = signature
                documents.Add(biblosDocumentInfo)
            Next
        End If

        '' Effettuo il merge dei documenti precedenti con quelli nuovi
        If sourceList Is Nothing Then sourceList = New List(Of DocumentInfo)
        If documents.Count > 0 Then
            '' Se ci sono documenti da aggiungere li aggiungo
            sourceList.AddRange(documents)
        Else
            '' Se non devo aggiungere un nuovo documento alla lista
            If Not addNewDoc Then
                '' Altrimenti cancello la lista in modo che non copi il frontalino
                sourceList.Clear()
            End If

        End If

        '' Se non richiesto l'append genero una nuova catena biblos, altrimenti riutilizzo quella passata
        If Not append Then idCatena = 0

        Return DocumentInfoFactory.ArchiveDocumentsInBiblos(sourceList, resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idCatena)
    End Function

    Private Shared Function DuplicaDocumento(ByVal sourceList As List(Of DocumentInfo), ByRef idCatena As Guid, ByVal resl As Resolution, ByVal append As Boolean, ByVal onlySigned As Boolean, ByVal signature As String) As Guid
        Dim documents As New List(Of BiblosDocumentInfo)

        '' Se ho un idCatena valido allora carico i documenti presenti su tale id (eventualmente solo se firmati)
        If idCatena <> Guid.Empty Then
            For Each biblosDocumentInfo As BiblosDocumentInfo In From biblosDocumentInfo1 In BiblosDocumentInfo.GetDocuments(resl.Location.DocumentServer, idCatena) Where Not onlySigned OrElse biblosDocumentInfo1.IsSigned
                biblosDocumentInfo.Signature = signature
                documents.Add(biblosDocumentInfo)
            Next
        End If

        '' Effettuo il merge dei documenti precedenti con quelli nuovi
        If sourceList Is Nothing Then sourceList = New List(Of DocumentInfo)
        If documents.Count > 0 Then
            '' Se ci sono documenti da aggiungere li aggiungo
            sourceList.AddRange(documents)
        Else
            '' Altrimenti cancello la lista in modo che non copi il frontalino
            sourceList.Clear()
        End If


        '' Se non richiesto l'append genero una nuova catena biblos, altrimenti riutilizzo quella passata
        If Not append Then idCatena = Guid.Empty

        Return DocumentInfoFactory.ArchiveDocumentsInBiblos(sourceList, resl.Location.DocumentServer, resl.Location.ReslBiblosDSDB, idCatena)
    End Function

    Private Function GetSignature(ByRef resl As Resolution, ByVal documentTypeAcronym As String, ByVal stato As String, ByVal signature As String, ByVal data As DateTime, ByVal number As String, Optional resolution As Resolution = Nothing) As String
        If String.IsNullOrEmpty(signature) Then signature = "*"

        If stato.Eq("Adozione") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(resl.Id, data.Year.ToString(), number, data.ToString("dd/MM/yyyy"), True, resolution)

            Select Case documentTypeAcronym
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case "DO"
                    signature &= " (Documento Omissis)"
                Case "AO"
                    signature &= " (Allegato Omissis)"
                Case "F"
                    signature &= " (Frontalino)"
            End Select
        ElseIf stato.Eq("Documento Adozione") Then
            '--                
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(resl.Id, , , , True)
            Select Case documentTypeAcronym
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case "DO"
                    signature &= " (Documento Omissis)"
                Case "AO"
                    signature &= " (Allegato Omissis)"
                Case "F"
                    signature &= " (Frontalino)"
            End Select
        ElseIf stato.Eq("Pubblicazione") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(resl.Id, , , , True)
            Select Case documentTypeAcronym
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case "DO"
                    signature &= " (Documento Omissis)"
                Case "AO"
                    signature &= " (Allegato Omissis)"
                Case "F"
                    signature &= " (Frontalino)"
                Case Else
                    signature &= String.Format(": Inserimento Albo {0:dd/MM/yyyy}", data)
            End Select
        ElseIf stato.Eq("Pubblicazione") OrElse stato.Eq("Esecutività") Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(resl.Id, , , , True)
            If ResolutionEnv.ExpirationDateSignatureEnabled AndAlso resl.Type.Description.Eq("Delibera") Then
                signature &= " Data Scad. " & data.AddDays(ResolutionEnv.PublicationEndDays).ToString("dd/MM/yyyy")
            End If
            Select Case documentTypeAcronym
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case "DO"
                    signature &= " (Documento Omissis)"
                Case "AO"
                    signature &= " (Allegato Omissis)"
                Case "F"
                    signature &= " (Frontalino)"
            End Select
        ElseIf stato = "Ritiro Pubblicazione" Then
            signature = Facade.ResolutionFacade.SqlResolutionGetNumber(resl.Id, , , , True)
            Select Case documentTypeAcronym
                Case "A"
                    signature &= " (Allegato)"
                Case "AR"
                    signature &= " (Allegato Riservato)"
                Case "AN"
                    signature &= " (Annesso)"
                Case "DO"
                    signature &= " (Documento Omissis)"
                Case "AO"
                    signature &= " (Allegato Omissis)"
                Case "F"
                    signature &= " (Frontalino)"
            End Select
        End If

        If Not signature.Contains("*") Then
            ' Inserimento acronimo aziendale per la signature
            Select Case ResolutionEnv.SignatureType
                Case 0
                Case 1
                    signature = ResolutionEnv.CorporateAcronym & " " & signature
            End Select
        End If

        Return signature
    End Function

    Private Function CreateSelectionList(ByVal source As String) As IList(Of Integer)
        If String.IsNullOrEmpty(source) Then
            Return New List(Of Integer)()
        End If

        Return source.Split(","c).Select(Function(s) Integer.Parse(s)).Distinct().ToList()
    End Function

    ''' <summary> Permette di creare una lista Contatti selezionati </summary>
    Private Sub SetContactM(ByVal nodo As RadTreeNode, ByRef lista As IList(Of CollaborationContact))
        If nodo.Nodes.Count <> 0 Then
            For Each tn As RadTreeNode In nodo.Nodes
                SetContactM(tn, lista)
            Next
            Exit Sub
        End If

        If String.IsNullOrEmpty(nodo.Attributes(uscContattiSel.ManualContactAttribute)) Then
            Exit Sub
        End If

        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(nodo.Attributes(uscContattiSel.ManualContactAttribute))
        Dim checked As Boolean = nodo.Checkable AndAlso nodo.Checked
        lista.Add(New CollaborationContact(contact, checked))

    End Sub

    ''' <summary>
    ''' Azione relativa a Trasmissione Adozione Collegio Sindacale Firma Digitale
    ''' </summary>
    ''' <remarks>Primo tentativo di semplificare l'evento di conferma</remarks>
    Private Function ToAdoptionCsDigitalSignatureAction(adoptionDateFrom As DateTime, adoptionDateTo As DateTime, headingLetter As String) As Boolean
        'Genero il frontalino e attivo la collaborazione
        Try

            Dim tipologiaAtto As String
            If Tipologia = ResolutionType.IdentifierDelibera Then
                tipologiaAtto = "Del"
            ElseIf Tipologia = ResolutionType.IdentifierDetermina Then
                tipologiaAtto = "Det"
            End If

            Dim reportPath As String = Server.MapPath(Path.Combine(DocSuiteContext.Current.ProtocolEnv.ReportLibraryPath, String.Format("Letters_{0}_{1}.rdlc", tipologiaAtto, Action)))
            Dim rdlcInfo As New FileInfo(reportPath)
            If Not rdlcInfo.Exists Then
                Throw New FileNotFoundException(String.Format("Il frontalino {0} non è stato trovato.", rdlcInfo.FullName))
            End If

            Dim parameters As New Dictionary(Of String, String)()
            parameters.Add("Azienda", DocSuiteContext.Current.ProtocolEnv.CorporateAcronym)
            parameters.Add("ResolutionType", Tipologia.ToString())
            If Tipologia = 1 Then
                parameters.Add("DataAdozioneDa", adoptionDateFrom.ToString("dd/MM/yyyy"))
                parameters.Add("HeadingLetter", headingLetter)
            Else
                parameters.Add("DataAdozioneDa", adoptionDateFrom.ToString("dd/MM/yyyy"))
                parameters.Add("DataAdozioneA", adoptionDateTo.ToString("dd/MM/yyyy"))
            End If

            'Separo per gruppi di Container
            Dim resolutions As IList(Of Resolution) = SelezioneLista.Select(Function(s) Facade.ResolutionFacade.GetById(s)).ToList()
            Dim idCollaborationResult As Integer = GenerateReportAndCollaboration(parameters, rdlcInfo, resolutions,
                                                                    txtCollaborationObject.Text, TOWorkflow.RicercaFlussoInvioAdozioneCollegioSindacaleFirmaDigitale)

            If idCollaborationResult > 0 Then
                Const template As String = "Collaborazione n.({0}) inserita con successo.{1}" &
                                                    "Il processo si concluderà automaticamente con la protocollazione delle collaborazioni."
                AjaxAlert(String.Format(template, idCollaborationResult, Environment.NewLine))
                Return True
            Else
                AjaxAlert("Alcune collaborazioni non sono state inserite correttamente.")
                Return False
            End If
        Catch ex As Exception
            Dim errorMsg As String = String.Format("Impossibile effettuare l'invio al collegio sindacale:{1}{0}", ex.Message, Environment.NewLine)
            FileLogger.Error(LoggerName, errorMsg, ex)
            AjaxAlert(errorMsg)
        End Try
        Return False
    End Function

    Private Function ToPublication() As Boolean
        Try
            Dim ss As String = String.Empty
            Dim publicationDate As Date

            Select Case Tipologia
                Case ResolutionType.IdentifierDelibera
                    ss = Request.QueryString("CollegioWarningDate")
                    If Not String.IsNullOrEmpty(ss) Then
                        ss = DateTime.ParseExact(ss, "yyyyMMdd", Nothing).ToString()
                    End If
                    publicationDate = DateAdd(DateInterval.Day, 5, CommonUtil.ConvData(ss))
                Case ResolutionType.IdentifierDetermina
                    Dim sProt() As String
                    sProt = Split(Request.QueryString("Protocollo"), "|")
                    If sProt.Length = 4 Then
                        ss = sProt(3)
                    End If
                    publicationDate = DateAdd(DateInterval.Day, 5, CommonUtil.ConvData(ss))
            End Select

            Dim isConfTo As Boolean = False
            If (ResolutionEnv.Configuration = ConfTo) Then
                isConfTo = True
            End If
            'Pubblicare le delibere
            Dim actionSuccess As Boolean = Facade.ResolutionFacade.UpdatePublishingDate(Selezione, publicationDate, DocSuiteContext.Current.User.FullUserName, isConfTo)

            If Not actionSuccess Then
                AjaxAlert("Errore in aggiornamento data di Pubblicazione")
                Return False
            End If

            Dim resolutionIds As Integer() = Selezione.Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()

            'Gestisco la cache
            'todo: trovare una maniera migliore per gestire le casisteche di cache NH
            For Each resolutionId As Integer In resolutionIds
                Dim resl As Resolution = Facade.ResolutionFacade.GetById(resolutionId)
                Facade.ResolutionFacade.Evict(resl)
            Next

            For Each resolutionId As Integer In resolutionIds
                Dim resl As Resolution = Facade.ResolutionFacade.GetById(resolutionId)
                CurrentResolutionDocumentSeriesItemFacade.ConfirmAndPublishSeries(resl)

                'Perchè utilizzare parametri della ParameterEnv di Protocollo quando siamo in Atti?
                'todo: da portare la chiave nella ParameterEnv di Atti
                If (DocSuiteContext.Current.ProtocolEnv.PdfPrint) Then
                    Dim currentFileResolution As FileResolution = Facade.FileResolutionFacade.GetByResolution(resl)(0)
                    currentFileResolution.IdFrontespizio = Nothing
                    Facade.FileResolutionFacade.Save(currentFileResolution)
                End If

                'Invio comando di aggiornamento Resolution alle WebApi
                Facade.ResolutionFacade.SendUpdateResolutionCommand(resl)

                Try
                    'Se il parametro degli avanzamenti di step automatici è attivo, creo un attività del JeepService per ogni atto selezionato. 
                    If ResolutionEnv.AutomaticActivityStepEnabled Then
                        Dim selection As IList(Of PublicationResolutionDocumentModel) = CurrentResolutionDocuments
                        If selection IsNot Nothing AndAlso selection.Count > 0 Then
                            Dim activityDocument As ResolutionActivityDocumentModel
                            Dim selectedItem As PublicationResolutionDocumentModel = selection.Where(Function(r) r.IdResolution.Equals(resolutionId)).FirstOrDefault()

                            If selectedItem IsNot Nothing AndAlso resl IsNot Nothing AndAlso resl.PublishingDate.HasValue Then
                                activityDocument = New ResolutionActivityDocumentModel()
                                activityDocument.Ids = selectedItem.IdDocuments
                                activityDocument.IsPrivacy = False
                                If resl.Container.Privacy.HasValue AndAlso resl.Container.Privacy.Value = 1 Then
                                    activityDocument.IsPrivacy = True
                                End If
                                'Creazione attività di pubblicazione web per Jeep Service
                                Facade.ResolutionActivityFacade.CreatePublicationActivity(resl, activityDocument)

                                'Creazione attività di esecutività per Jeep Service
                                Dim effectivenessDate As DateTimeOffset = DateAdd(DateInterval.Day, 10, resl.PublishingDate.Value)
                                Facade.ResolutionActivityFacade.CreateExecutiveActivity(resl, effectivenessDate)
                            End If
                        End If

                    End If

                Catch ex As Exception
                    FileLogger.Error(LoggerName, String.Concat("Errore nella creazione dell'activity del Jeep Service per la pubblicazione automatica dell'atto n. ", resl.Id, ". Il processo di pubblicazione massiva non è andata a buon fine: ", ex.Message), ex)
                    CurrentResolutionDocuments = Nothing
                    Return False
                End Try
            Next
            CurrentResolutionDocuments = Nothing
            Return actionSuccess
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("Errore nella pubblicazione massiva degli atti con id: ", Selezione, " - Errore: ", ex.Message), ex)
            CurrentResolutionDocuments = Nothing
            Return False
        End Try

    End Function

    Private Sub ToAdoption(ByRef gestioneDigitale As Boolean, ByRef fileTemp As String, ByRef confirmSuccess As Boolean, ByRef workStep As TabWorkflow)
        _myStep = BarWorkflowStep.TrasmissioneServizi
        '-- Ordinare le proposte per ufficio proponente e numero provvisorio
        Dim proposte As IList(Of Resolution) = Facade.ResolutionFacade.GetResolutionsOrderProposerCode(SelezioneLista)
        If proposte IsNot Nothing Then
            If Facade.TabWorkflowFacade.GetByStep(Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, Tipologia), _myStep, workStep) Then
                If gestioneDigitale Then
                    'nuova gestione: i frontalini entrano nella catena dei documenti della delibera
                    fileTemp = String.Empty
                    Dim allPdfs As List(Of ResolutionFrontispiece) = New List(Of ResolutionFrontispiece)
                    For Each prop As Resolution In proposte
                        Dim printer As ReslFrontalinoPrintPdfTO = New ReslFrontalinoPrintPdfTO()

                        'Aggiorno l'elenco documenti
                        Dim idDocumenti As Integer = 0
                        Dim idAllegati As Integer = 0
                        Dim idDocumentiOmissis As Nullable(Of Guid) = Nothing
                        Dim idAllegatiOmissis As Nullable(Of Guid) = Nothing
                        Dim adoptionDate As DateTime = DateTime.ParseExact(rdpData.SelectedDate.Value, "dd/MM/yyyy", CultureInfo.CurrentCulture)

                        'Preparo una resolution per generare il frontalino con alcuni campi popolati, pur non essendo ancora adottata. 
                        'In questo modo riesco a generare il frontalino correttamente prima che l'atto sia adottato -> se va in errore la generazione del frontalino di adozione, l'atto è ancora in stato di proposta.

                        Dim year As Short = 0
                        Dim number As Integer = 0
                        If prop.Number.HasValue AndAlso prop.Year.HasValue Then
                            year = prop.Year.Value
                            number = prop.Number.Value
                        Else
                            Dim yearAndnumber As Tuple(Of Short, Integer) = Facade.ResolutionFacade.CalculateYearAndNumber(prop.ProposeDate.Value, adoptionDate, Tipologia)
                            year = yearAndnumber.Item1
                            number = yearAndnumber.Item2
                        End If
                        Dim frontispieceResolution As Resolution = DirectCast(prop.Clone(), Resolution)
                        frontispieceResolution.Year = year
                        frontispieceResolution.Number = number
                        frontispieceResolution.AdoptionDate = adoptionDate
                        frontispieceResolution.Container = DirectCast(prop.Container.Clone(), Container)
                        Dim reslContact As ResolutionContact = New ResolutionContact()
                        reslContact.Contact = DirectCast(prop.ResolutionContactProposers(0).Contact.Clone(), Contact)
                        frontispieceResolution.ResolutionContactProposers = New List(Of ResolutionContact)() From {reslContact}

                        Try
                            Dim digitale As Boolean = (radioFrontalino.SelectedValue = "1")
                            Dim frontaliniSignature As String = GetSignature(prop, String.Empty, workStep.Description, "*", adoptionDate, String.Empty, frontispieceResolution)
                            Dim frontalini As ICollection(Of ResolutionFrontispiece) = printer.GeneraFrontalini(frontispieceResolution)
                            allPdfs.AddRange(frontalini)

                            'Salvataggio dei frontalini in biblos e update di fileresolution
                            If digitale Then
                                printer.SaveBiblosFrontispieces(frontalini, prop, frontaliniSignature)
                            End If


                            Dim frontalinoStandard As New List(Of DocumentInfo)
                            Dim frontalinoPrivacy As New List(Of DocumentInfo)
                            ' In posizione 0 si trova il frontalino standard
                            ' In posizione 1 si trova quello privacy (se previsto dal contenitore)
                            If digitale Then
                                'Copia e incolla del codice della ReslFlusso
                                Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(prop.Location.DocumentServer, prop.Location.ReslBiblosDSDB, prop.File.IdFrontespizio.Value)
                                'Dim chainenum As Integer = If(prop.Container.Privacy.GetValueOrDefault(0) = 1, 1, 0)
                                'Dim document As New TempFileDocumentInfo(BiblosFacade.SaveUniquePdfToTempNoSignature(docs(chainenum)))
                                'document.Name = "Frontalino.pdf"
                                'document.Signature = frontaliniSignature
                                frontalinoStandard.Add(docs(0))
                                If docs.Count > 1 Then
                                    frontalinoPrivacy.Add(docs(1))
                                End If
                                'frontalini.Add(document)
                            End If

                            '-- Adottare le proposte ordinate
                            confirmSuccess = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(prop.Id, Tipologia, Not prop.Number.HasValue, workStep, rdpData.SelectedDate.Value, "N", -1, -1, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)

                            'Calcolo il workstep precedente
                            Dim precedingWorkstep As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(prop.Id)
                            'Duplica documento principale                            
                            idDocumenti = DuplicaDocumento(frontalinoStandard, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldDocument), Integer), prop, False, True, frontaliniSignature)
                            'Duplica allegati
                            Dim allegatoSignature As String = GetSignature(prop, "A", workStep.Description, "*", adoptionDate, String.Empty, frontispieceResolution)
                            idAllegati = DuplicaDocumento(Nothing, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldAttachment), Integer), prop, False, True, allegatoSignature)
                            'Duplica documento principale omissis
                            Dim documentoOmissisSignature As String = GetSignature(prop, "DO", workStep.Description, "*", adoptionDate, String.Empty, frontispieceResolution)
                            idDocumentiOmissis = DuplicaDocumento(frontalinoPrivacy, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldDocumentsOmissis), Guid), prop, False, True, documentoOmissisSignature)
                            'Duplica allegati omissis
                            Dim allegatoOmissisSignature As String = GetSignature(prop, "AO", workStep.Description, "*", adoptionDate, String.Empty, frontispieceResolution)
                            idAllegatiOmissis = DuplicaDocumento(Nothing, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldAttachmentsOmissis), Guid), prop, False, True, allegatoOmissisSignature)
                        Catch ex As Exception
                            FileLogger.Error(LoggerName, String.Format("Errore nella generazione dei file di frontespizio da Ricerca Flusso: {0}", ex.Message), ex)
                            Facade.ResolutionLogFacade.Insert(prop, ResolutionLogType.RF, "Errore nella generazione dei file di frontespizio da Ricerca Flusso.")
                            AjaxAlert(String.Format("Errore nella generazione dei file di frontespizio: {0}", ex.Message))
                            Return
                        End Try

                        'Fare questa chiamata con gli id aggiornati ai valori della duplicazione dei documenti e frontalino aggiunto davanti
                        If confirmSuccess Then confirmSuccess = Facade.ResolutionWorkflowFacade.InsertNextStep(prop.Id, _myStep - 1S, idDocumenti, idAllegati, 0, Guid.Empty, If(idDocumentiOmissis.HasValue, idDocumentiOmissis.Value, Guid.Empty), If(idAllegatiOmissis.HasValue, idAllegatiOmissis.Value, Guid.Empty), DocSuiteContext.Current.User.FullUserName)

                        ' Documento Principale
                        If idDocumenti <> -1 Then
                            If Not String.IsNullOrEmpty(workStep.FieldDocument) Then
                                If confirmSuccess Then confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idDocumenti, workStep.FieldDocument)
                            End If
                        End If

                        ' Allegati
                        If idAllegati <> -1 Then
                            If Not String.IsNullOrEmpty(workStep.FieldAttachment) Then
                                If confirmSuccess Then
                                    confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idAllegati, workStep.FieldAttachment)
                                End If
                            End If
                        End If

                        ' Documento Omissis --> se ha valore, anche fosse Guid.Empty, allora lo utilizza per eventualmente sbiancare la catena,
                        ' altrimenti ignora la modifica e lascia tutto com'è
                        If idDocumentiOmissis.HasValue Then
                            If Not String.IsNullOrEmpty(workStep.FieldDocumentsOmissis) Then
                                If confirmSuccess Then confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idDocumentiOmissis, workStep.FieldDocumentsOmissis)
                            End If
                        End If

                        ' Allegati Omissis --> se ha valore, anche fosse Guid.Empty, allora lo utilizza per eventualmente sbiancare la catena,
                        ' altrimenti ignora la modifica e lascia tutto com'è
                        If idAllegatiOmissis.HasValue Then
                            If Not String.IsNullOrEmpty(workStep.FieldAttachmentsOmissis) Then
                                If confirmSuccess Then
                                    confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idAllegatiOmissis, workStep.FieldAttachmentsOmissis)
                                End If
                            End If
                        End If

                        Facade.ResolutionLogFacade.Log(prop, ResolutionLogType.RW, "Adozione Atto da Ricerca Flusso")
                        'Invio comando di creazione Resolution alle WebApi
                        Facade.ResolutionFacade.SendCreateResolutionCommand(prop)
                    Next

                    ' Creo il pdf Finale
                    If allPdfs.Count > 1 Then
                        fileTemp = String.Format("{0}Fontalini_{1}.{2}", CommonUtil.UserDocumentName, Guid.NewGuid().ToString("N"), FileHelper.PDF)
                        Dim fDestination As String = String.Concat(CommonInstance.AppTempPath, fileTemp)
                        Dim managerPdf As New PdfMerge()
                        For Each doc As ResolutionFrontispiece In allPdfs
                            managerPdf.AddDocument(doc.Path)
                        Next
                        managerPdf.Merge(fDestination)
                        confirmSuccess = True
                    ElseIf allPdfs.Count = 1 Then
                        Dim temp As New FileInfo(allPdfs.First().Path)
                        fileTemp = temp.Name
                    End If

                Else
                    'vecchia gestione: i frontalini non entrano nella catena dei documenti della delibera ma vanno stampati ed importati a mano
                    Dim isFirst As Boolean = True
                    fileTemp = String.Concat(CommonUtil.UserDocumentName, "-Print-", String.Format("{0:HHmmss}", Now()), ".htm")
                    Dim fSource As String = String.Concat(CommonInstance.AppPath & ResolutionFacade.PathStampeTo, "Frontalino.htm")
                    Dim fDestination As String = String.Concat(CommonInstance.AppTempPath, fileTemp)
                    'read
                    Dim sr As StreamReader = New StreamReader(fSource)
                    Dim fText As String = sr.ReadToEnd
                    sr.Close()
                    'For write
                    Dim sw As StreamWriter = New StreamWriter(fDestination)

                    For Each prop As Resolution In proposte
                        '-- Adottare le proposte ordinate
                        confirmSuccess = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(prop.Id, Tipologia, Not prop.Number.HasValue, workStep, rdpData.SelectedDate.Value, "N", -1, -1, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)
                        If confirmSuccess Then
                            confirmSuccess = Facade.ResolutionWorkflowFacade.InsertNextStep(prop.Id, _myStep - 1S, 0, 0, 0, Guid.Empty, Guid.Empty, Guid.Empty, DocSuiteContext.Current.User.FullUserName)
                        End If
                        Facade.ResolutionLogFacade.Log(prop, ResolutionLogType.RW, "Adozione Atto da Ricerca Flusso")
                        'Invio comando di creazione Resolution alle WebApi
                        Facade.ResolutionFacade.SendCreateResolutionCommand(prop)
                        '-- Genera il frontalino
                        If confirmSuccess Then
                            confirmSuccess = GeneraFrontalino(prop.Id, fText, sw, isFirst)
                        End If
                        isFirst = False
                    Next
                    '-- Visualizza i frontespizi ordinati
                    sw.Close()
                End If
            End If
        End If
    End Sub
#Region "AgliAffariGenerali"
    Private Sub ToCollaborationFromAffariGenerali(ByRef confirmSuccess As Boolean)
        '_myStep = BarWorkflowStep.TrasmissioneServizi
        '-- Ordinare le proposte per ufficio proponente e numero provvisorio
        Dim proposte As IList(Of Resolution) = Facade.ResolutionFacade.GetResolutionsOrderProposerCode(SelezioneLista)
        Dim containerName As String

        Dim idDocumenti As Integer = 0
        Dim idAllegati As Integer = 0
        Dim idDocumentiOmissis As Nullable(Of Guid) = Nothing
        Dim idAllegatiOmissis As Nullable(Of Guid) = Nothing
        Dim idAnnexes As Guid = Guid.Empty

        Dim _currentTemplateCollaborationFinder As TemplateCollaborationFinder
        If proposte IsNot Nothing Then
            '--- crea collaborazione 
            For Each prop As Resolution In proposte
                containerName = prop.Container.Name
                _currentTemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                _currentTemplateCollaborationFinder.ResetDecoration()
                _currentTemplateCollaborationFinder.EnablePaging = False
                _currentTemplateCollaborationFinder.Name = containerName
                _currentTemplateCollaborationFinder.ExpandProperties = True

                Dim currentTemplate As WebAPIDto(Of TemplateCollaboration) = _currentTemplateCollaborationFinder.DoSearch().FirstOrDefault()
                If currentTemplate Is Nothing OrElse currentTemplate.Entity Is Nothing Then
                    Throw New DocSuiteException(String.Format("Errore in caricamento Template, nessun Template di collaborazione trovato con name {0}", containerName))
                End If
                Dim collaborationFromAffariGenerali As New Collaboration

                collaborationFromAffariGenerali.IdPriority = currentTemplate.Entity.IdPriority
                collaborationFromAffariGenerali.CollaborationObject = prop.ResolutionObject
                collaborationFromAffariGenerali.Note = prop.Note
                collaborationFromAffariGenerali.IdStatus = CollaborationStatusType.IN.ToString()
                collaborationFromAffariGenerali.TemplateName = currentTemplate.Entity.Name
                collaborationFromAffariGenerali.DocumentType = CollaborationDocumentType.D.ToString()

                Dim collaborationUser As CollaborationUser
                Dim segreterie As New List(Of CollaborationUser)
                Dim altriDestinatari As New List(Of CollaborationContact)
                Dim destinatariFirma As New List(Of CollaborationContact)
                For Each secretary As TemplateCollaborationUser In currentTemplate.Entity.TemplateCollaborationUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Secretary) AndAlso x.Role IsNot Nothing).OrderBy(Function(o) o.Incremental)
                    Dim nhRole As Role = Facade.RoleFacade.GetById(secretary.Role.EntityShortId)
                    If nhRole IsNot Nothing Then
                        collaborationUser = New CollaborationUser(nhRole)
                        collaborationUser.DestinationFirst = secretary.IsRequired
                        segreterie.Add(collaborationUser)
                    End If
                Next

                ' Utenti firmatari
                For Each signer As TemplateCollaborationUser In currentTemplate.Entity.TemplateCollaborationUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Signer) AndAlso Not String.IsNullOrEmpty(x.Account)).OrderBy(Function(o) o.Incremental)
                    destinatariFirma.Add(GetCollaboratinContact(signer, currentTemplate.Entity.DocumentType))
                Next

                For Each contactRestitution As TemplateCollaborationUser In currentTemplate.Entity.TemplateCollaborationUsers.Where(Function(x) x.UserType.Equals(TemplateCollaborationUserType.Person) AndAlso Not String.IsNullOrEmpty(x.Account)).OrderBy(Function(o) o.Incremental)
                    altriDestinatari.Add(GetCollaboratinContact(contactRestitution, currentTemplate.Entity.DocumentType))
                Next

                'aggiunge settore proponente come segreteria 
                If prop.ResolutionContactProposers IsNot Nothing Then
                    Dim nhRole As Role = prop.ResolutionContactProposers(0).Contact.Role
                    If nhRole IsNot Nothing AndAlso Not segreterie.Any(Function(x) x.IdRole = nhRole.Id) Then
                        collaborationUser = New CollaborationUser(nhRole)
                        collaborationUser.DestinationFirst = False
                        collaborationUser.DestinationType = DestinatonType.S.ToString()
                        segreterie.Insert(0, collaborationUser)
                    End If
                End If

                FacadeFactory.Instance.CollaborationFacade.Insert(collaborationFromAffariGenerali, destinatariFirma, altriDestinatari, segreterie)
                Dim resolutionXml As New ResolutionXML() With {
                .Id = prop.Id,
                .ResolutionsToUpdate = New List(Of Integer)({prop.Id})
                }

                Facade.ResolutionLogFacade.Log(prop, ResolutionLogType.CC, "Collaborazione generata da 'Raccolta firme per adozione'")
                Facade.ProtocolDraftFacade.AddProtocolDraft(collaborationFromAffariGenerali, String.Format("Collaborazione da Proposta n.{0} in affari generali", prop.Id), resolutionXml)

                'documenti
                Dim fileMain As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
                Dim fileMainOmissis As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
                Dim fileAttachements As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
                Dim fileAllegatiOmissis As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
                Dim fileAnnexes As IList(Of DocumentInfo) = New List(Of DocumentInfo)()

                If prop.File.IdProposalFile.HasValue AndAlso prop.File.IdProposalFile.Value <> 0 Then
                    fileMain = GetResolutionDocuments(prop.Location.DocumentServer, prop.Location.ReslBiblosDSDB, prop.File.IdProposalFile.Value)
                    AddDocumentsToVersioning(fileMain, VersioningDocumentGroup.MainDocument, collaborationFromAffariGenerali)
                End If

                If prop.File.IdAttachements.HasValue AndAlso prop.File.IdAttachements.Value <> 0 Then
                    fileAttachements = GetResolutionDocuments(prop.Location.DocumentServer, prop.Location.ReslBiblosDSDB, prop.File.IdAttachements.Value)
                    AddDocumentsToVersioning(fileAttachements, VersioningDocumentGroup.Attachment, collaborationFromAffariGenerali)
                End If

                If Not prop.File.IdMainDocumentsOmissis.Equals(Guid.Empty) Then
                    fileMainOmissis = GetResolutionDocuments(prop.Location.DocumentServer, prop.File.IdMainDocumentsOmissis)
                    AddDocumentsToVersioning(fileMainOmissis, VersioningDocumentGroup.MainDocumentOmissis, collaborationFromAffariGenerali)
                End If

                If Not prop.File.IdAttachmentsOmissis.Equals(Guid.Empty) Then
                    fileAllegatiOmissis = GetResolutionDocuments(prop.Location.DocumentServer, prop.File.IdAttachmentsOmissis)
                    AddDocumentsToVersioning(fileAllegatiOmissis, VersioningDocumentGroup.AttachmentOmissis, collaborationFromAffariGenerali)
                End If

                If Not prop.File.IdAnnexes.Equals(Guid.Empty) Then
                    fileAnnexes = GetResolutionDocuments(prop.Location.DocumentServer, prop.File.IdAnnexes)
                    AddDocumentsToVersioning(fileAnnexes, VersioningDocumentGroup.Annexed, collaborationFromAffariGenerali)
                End If
                'Sospende l'atto cosi non puo essere visualizzato
                prop.Status = Facade.ResolutionStatusFacade.GetById(ResolutionStatusId.Sospeso)
                Facade.ResolutionFacade.UpdateOnly(prop)
            Next
            confirmSuccess = True
        End If
    End Sub
    Private Function GetResolutionDocuments(ByVal documentServer As String, ByVal reslBiblosDSDB As String, ByVal idChain As Integer) As IList(Of DocumentInfo)
        Dim listFile As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        Dim attachs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(documentServer, reslBiblosDSDB, idChain)
        For Each di As BiblosDocumentInfo In attachs
            listFile.Add(di)
        Next
        Return listFile
    End Function

    Private Function GetResolutionDocuments(ByVal documentServer As String, ByVal guidChain As Guid) As IList(Of DocumentInfo)
        Dim listFile As IList(Of DocumentInfo) = New List(Of DocumentInfo)()
        Dim attachs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(documentServer, guidChain)
        For Each di As BiblosDocumentInfo In attachs
            listFile.Add(di)
        Next
        Return listFile
    End Function
    Private Function AddDocumentsToVersioning(documentList As IList(Of DocumentInfo), documentGroup As String, collaboration As Collaboration) As Boolean
        Dim hasError As Boolean = False
        For Each doc As DocumentInfo In documentList
            Try
                doc.Signature = Facade.CollaborationFacade.GetSignature(collaboration.Id)
                Facade.CollaborationVersioningFacade.AddDocumentToVersioning(collaboration, doc, documentGroup)
            Catch ex As Exception
                Dim message As String = String.Format("Errore in inserimento documento [{0}] di tipo [{1}]: {2}", doc.Name, documentGroup, ex.Message)
                FileLogger.Warn(Facade.CollaborationFacade.LoggerName, message, ex)
                hasError = True
            End Try
        Next
        If hasError Then
            Return False
        End If
        Return True
    End Function
    Private Function GetCollaboratinContact(ByVal templateCollaborationUser As TemplateCollaborationUser, ByVal templateDocumentType As String) As CollaborationContact
        Dim contact As New CollaborationContact()
        Dim user As AccountModel = CommonAD.GetAccount(templateCollaborationUser.Account)
        Dim roleUserId As String = String.Empty
        Dim description As String = String.Empty
        Dim email As String = String.Empty
        If user IsNot Nothing Then
            description = user.DisplayName
            email = user.Email
        End If
        If templateCollaborationUser.Role IsNot Nothing Then
            roleUserId = templateCollaborationUser.Role.EntityShortId.ToString()
            Dim roleUser As RoleUser = Facade.RoleUserFacade.GetByRoleIdAndAccount(templateCollaborationUser.Role.EntityShortId, templateCollaborationUser.Account, True).FirstOrDefault()
            If roleUser IsNot Nothing Then
                description = Facade.CollaborationFacade.GetSignerDescription(roleUser.Description, roleUser.Account, templateDocumentType)
                If Not String.IsNullOrEmpty(roleUser.Email) Then
                    email = roleUser.Email
                End If
            End If
        End If
        If String.IsNullOrEmpty(description) AndAlso String.IsNullOrEmpty(email) Then
            FileLogger.Warn(LoggerName, String.Concat("Non sono stati trovati riferimenti per l'account ", templateCollaborationUser.Account))
            description = templateCollaborationUser.Account
        End If
        contact.Account = templateCollaborationUser.Account
        contact.DestinationName = description
        If contact.DestinationName.Contains("(") Then
            contact.DestinationName = contact.DestinationName.Substring(0, contact.DestinationName.IndexOf("(")).Trim()
        End If
        contact.DestinationEMail = email
        contact.DestinationFirst = templateCollaborationUser.IsRequired

        Return contact
    End Function





#End Region


    Private Sub PcAdoption(ByRef confirmSuccess As Boolean, ByRef workStep As TabWorkflow)
        Dim codeWorkflow As String = Facade.TabMasterFacade.GetFieldValue("WorkflowType", DocSuiteContext.Current.ResolutionEnv.Configuration, Tipologia)
        _myStep = Facade.TabWorkflowFacade.GetByDescription(Action, codeWorkflow).Id.ResStep

        Dim proposte As IList(Of Resolution) = Facade.ResolutionFacade.GetResolutionOrderProposeDate(SelezioneLista)
        If proposte IsNot Nothing Then
            If Facade.TabWorkflowFacade.GetByStep(codeWorkflow, _myStep, workStep) Then
                For Each prop As Resolution In proposte

                    'Aggiorno l'elenco documenti
                    Dim idDocumenti As Integer = 0
                    Dim idAllegati As Integer = 0
                    Dim idDocumentiOmissis As Nullable(Of Guid) = Nothing
                    Dim idAllegatiOmissis As Nullable(Of Guid) = Nothing

                    Dim idAnnexes As Guid = Guid.Empty
                    Dim idPrivacyAttachment As Integer = -1
                    Dim number As String = String.Empty

                    Try
                        number = ServiceNumberCheck(prop)
                        Dim numServ As String = If(pnlServiceCode.Visible, number, "N")

                        'Pulitura del campo declineNote in quanto il messaggio è stato superato
                        If prop.DeclineNote IsNot Nothing Then
                            prop.DeclineNote = Nothing
                        End If

                        'Lo recupero ora altrimenti viene cancellato se abilitato il parametro EnableFlushAnnexed
                        idAnnexes = ReflectionHelper.GetPropertyCase(prop.File, workStep.FieldAnnexed)

                        If pnlServiceCode.Visible Then
                            SetAdoptionRole(prop, ddlServizio.SelectedValue)
                        End If

                        If prop.Type.Id.Equals(ResolutionType.IdentifierDetermina) AndAlso numServ <> "N" Then
                            If Not ValidateAdoptionDate(prop, numServ, workStep.ManagedWorkflowData) Then
                                FileLogger.Error(LoggerName, String.Format("E' stata inserita una data di Adozione antecedente alla data di Adozione dell'ultima {0} presente.", Facade.ResolutionTypeFacade.GetDescription(prop.Type.Id)))
                                AjaxAlert(String.Format("Attenzione! E' stata inserita una data di Adozione antecedente alla data di Adozione dell'ultima {0} presente.", Facade.ResolutionTypeFacade.GetDescription(prop.Type.Id)))
                                Exit Sub
                            End If
                        End If

                        '-- Adottare le proposte ordinate
                        confirmSuccess = Facade.ResolutionFacade.SqlResolutionUpdateWorkflowData(prop.Id, Tipologia, Not prop.Number.HasValue, workStep, rdpData.SelectedDate.Value, numServ, -1, -1, Guid.Empty, "N", True, DocSuiteContext.Current.User.FullUserName)

                        Dim frontaliniSignature As String = GetSignature(prop, String.Empty, workStep.Description, "*", prop.AdoptionDate.Value, String.Empty)

                        Dim frontalinoStandard As New List(Of DocumentInfo)
                        Dim frontalinoPrivacy As New List(Of DocumentInfo)
                        ' In posizione 0 si trova il frontalino standard
                        ' In posizione 1 si trova quello privacy (se previsto dal contenitore)

                        'Calcolo il workstep precedente
                        Dim precedingWorkstep As TabWorkflow = Facade.TabWorkflowFacade.GetByResolution(prop.Id)
                        'Duplica documento principale
                        idDocumenti = DuplicaDocumento(frontalinoStandard, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldDocument), Integer), prop, False, True, frontaliniSignature)

                        'Duplica allegati
                        idAllegati = DuplicaDocumento(Nothing, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldAttachment), Integer), prop, False, True, frontaliniSignature)

                        'Duplica documento principale omissis
                        idDocumentiOmissis = DuplicaDocumento(frontalinoPrivacy, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldDocumentsOmissis), Guid), prop, False, True, frontaliniSignature)
                        'Duplica allegati omissis
                        idAllegatiOmissis = DuplicaDocumento(Nothing, CType(ReflectionHelper.GetPropertyCase(prop.File, precedingWorkstep.FieldAttachmentsOmissis), Guid), prop, False, True, frontaliniSignature)

                        If ResolutionEnv.GenerateFrontalinoInAdoptionState AndAlso prop.WorkflowType = Facade.TabMasterFacade.GetFieldValue("WorkflowType", "AUSL-PC", Tipologia) AndAlso workStep.Description.Eq(WorkflowStep.ADOZIONE) Then
                            Dim num As String = String.Empty
                            If Not numServ.Eq("N") Then
                                num = number
                            End If
                            If (Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", prop.Type.Id)) Then
                                num = String.Empty
                            End If
                            Dim location As Location = prop.Location
                            Dim idCatenaFrontalino As Integer

                            Dim presentSigners As IList(Of String) = Nothing
                            Dim collaborationResl As Collaboration = FacadeFactory.Instance.CollaborationFacade.GetByResolution(prop)
                            If collaborationResl IsNot Nothing AndAlso ResolutionEnv.DelibereSignsReportEnabled Then
                                presentSigners = Facade.ResolutionFacade.GetPresentDocumentSigners(prop, workStep, idDocumenti)
                            End If
                            ResolutionUtil.GetInstance().InserisciFrontalino(CType(rdpData.SelectedDate, Date), prop.Id, Tipologia, workStep.Description, location, idCatenaFrontalino, num, _myStep - 1, ddlServizio.SelectedValue, presentSigners)
                        End If

                    Catch ex As Exception
                        FileLogger.Warn(LoggerName, String.Format("Errore nella generazione dei file di frontespizio: {0}", ex.Message), ex)
                        AjaxAlert(String.Format("Errore nella generazione dei file di frontespizio: {0}", ex.Message))
                        Return
                    End Try

                    'Fare questa chiamata con gli id aggiornati ai valori della duplicazione dei documenti e frontalino aggiunto davanti
                    If confirmSuccess Then confirmSuccess = Facade.ResolutionWorkflowFacade.InsertNextStep(prop.Id, _myStep - 1S, idDocumenti, idAllegati, 0, Guid.Empty, If(idDocumentiOmissis.HasValue, idDocumentiOmissis.Value, Guid.Empty), If(idAllegatiOmissis.HasValue, idAllegatiOmissis.Value, Guid.Empty), DocSuiteContext.Current.User.FullUserName)

                    ' Documento Principale
                    If idDocumenti <> -1 Then
                        If Not String.IsNullOrEmpty(workStep.FieldDocument) Then
                            If confirmSuccess Then confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idDocumenti, workStep.FieldDocument)
                        End If
                    End If

                    ' Allegati
                    If idAllegati <> -1 Then
                        If Not String.IsNullOrEmpty(workStep.FieldAttachment) Then
                            If confirmSuccess Then
                                confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idAllegati, workStep.FieldAttachment)
                            End If
                        End If
                    End If

                    ' Documento Omissis --> se ha valore, anche fosse Guid.Empty, allora lo utilizza per eventualmente sbiancare la catena,
                    ' altrimenti ignora la modifica e lascia tutto com'è
                    If idDocumentiOmissis.HasValue Then
                        If Not String.IsNullOrEmpty(workStep.FieldDocumentsOmissis) Then
                            If confirmSuccess Then confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idDocumentiOmissis, workStep.FieldDocumentsOmissis)
                        End If
                    End If

                    ' Allegati Omissis --> se ha valore, anche fosse Guid.Empty, allora lo utilizza per eventualmente sbiancare la catena,
                    ' altrimenti ignora la modifica e lascia tutto com'è
                    If idAllegatiOmissis.HasValue Then
                        If Not String.IsNullOrEmpty(workStep.FieldAttachmentsOmissis) Then
                            If confirmSuccess Then
                                confirmSuccess = Facade.ResolutionFacade.SqlResolutionFileUpdate(prop.Id, idAllegatiOmissis, workStep.FieldAttachmentsOmissis)
                            End If
                        End If
                    End If

                    ' Settori
                    If workStep.ExistWorkflowData(TabWorkflow.WorkflowField.Role) Then
                        Dim roleId As Integer = workStep.ExtractoWorkflowData(Of Integer)(TabWorkflow.WorkflowField.Role)
                        Facade.ResolutionRoleFacade.AddRole(prop, roleId, DocSuiteContext.Current.ResolutionEnv.DefaultResolutionRoleType)
                    End If

                    Facade.ResolutionLogFacade.Log(prop, ResolutionLogType.RW, "Adozione Atto da Ricerca Flusso")
                    'Invio comando di creazione Resolution alle WebApi
                    Facade.ResolutionFacade.SendCreateResolutionCommand(prop)
                Next
            End If
        End If
    End Sub

    Private Sub DoDataBindDdlServizio()
        ddlServizio.Items.Clear()

        _roles = Facade.RoleFacade.GetUserRoles(DSWEnvironment.Resolution, 1, True)

        Dim ddlServizioDictionary As Dictionary(Of String, String) = (From role In _roles Where Not String.IsNullOrEmpty(role.ServiceCode)).ToDictionary(Function(role) String.Format("{0} ({1})", role.ServiceCode, role.Name), Function(role) role.Id.ToString())
        ddlServizio.DataSource = From item In ddlServizioDictionary Order By item.Key
        ddlServizio.DataValueField = "Value"
        ddlServizio.DataTextField = "Key"
        ddlServizio.DataBind()

        'Aggiungo il valore iniziale
        If ddlServizio.Items.Count > 1 Then
            ddlServizio.Items.Insert(0, New ListItem("Seleziona Servizio", String.Empty))
        End If

    End Sub
    Private Function ServiceNumberCheck(ByVal prop As Resolution) As String

        Dim annoCorrente As Integer = Year(Date.Now)
        If rdpData.SelectedDate.HasValue Then
            annoCorrente = Year(rdpData.SelectedDate.Value)
        End If

        If prop.Type.Id.Equals(ResolutionType.IdentifierDelibera) OrElse (prop.Type.Id.Equals(ResolutionType.IdentifierDetermina) AndAlso ResolutionEnv.IncrementalNumberEnabled) Then
            Return Facade.ResolutionFacade.GetNextFreeServiceNumber(prop.Id, annoCorrente, prop.Type.Id).ToString().PadLeft(4, "0"c)
        Else
            '' Calcolo il numero di servizio successivo sse non ho il valore già impostato
            '' (se il campo è bloccato infatti conterrà, in fase di modifica, tutto il codice completo)
            If pnlServiceCode.Visible AndAlso Not String.IsNullOrEmpty(ddlServizio.SelectedValue) AndAlso ddlServizio.Items.Count > 0 Then
                Dim serviceCode As String = Facade.RoleFacade.GetById(CType(ddlServizio.SelectedValue, Integer)).ServiceCode.ToString()
                Return String.Format("{0}/{1}", serviceCode, Facade.ResolutionFacade.GetNextFreeServiceNumber(prop.Id, annoCorrente, serviceCode, Nothing).ToString().PadLeft(4, "0"c))
            End If
        End If
        Return String.Empty

    End Function

    Private Shared Sub SetAdoptionRole(ByRef resl As Resolution, ByVal role As String)
        Dim idRole As Short
        If resl.AdoptionRoleId Is Nothing AndAlso Not String.IsNullOrEmpty(role) AndAlso Short.TryParse(role, idRole) AndAlso idRole <> 0 Then
            resl.AdoptionRoleId = idRole
            FacadeFactory.Instance.ResolutionFacade.UpdateNoLastChange(resl)
        End If
    End Sub

    Private Function ValidateAdoptionDate(ByVal resl As Resolution, ByVal serviceCode As String, ByVal workflowManagedData As String) As Boolean
        Dim compareEnable As Boolean = TabWorkflowFacade.TestManagedWorkflowDataProperty(workflowManagedData, "Date", "INS", "Compare")
        If Not compareEnable Then
            Return True
        End If

        Dim selectedDate As Date = Date.MinValue
        If rdpData.SelectedDate.HasValue Then
            selectedDate = rdpData.SelectedDate.Value
        End If

        Dim checkAdoptionDate As Boolean = Facade.ResolutionFacade.CheckPreviousResolutionAdoptionDateMinus(resl, selectedDate, serviceCode)
        If Not checkAdoptionDate Then
            Return False
        End If
        Return True
    End Function

    Private Function CollaborationSignParametersReport(coll As Collaboration, absents As IList(Of String), managersParameter As IDictionary(Of String, String), prop As Resolution) As Dictionary(Of String, String)
        Dim parametersForReport As Dictionary(Of String, String) = New Dictionary(Of String, String)

        parametersForReport.Add("IdCollaboration", coll.Id.ToString())
        parametersForReport.Add("RegistrationDate", coll.RegistrationDate.ToString("dd/MM/yyyy"))
        parametersForReport.Add("Subject", coll.CollaborationObject)
        Dim reslNumber As String = Facade.ResolutionFacade.CalculateFullNumber(prop, prop.Type.Id, False)
        parametersForReport.Add("ResolutionNumber", reslNumber)
        parametersForReport.Add("ResolutionType", prop.Type.Id.ToString())
        parametersForReport.Add("ResolutionAdoptionDate", prop.AdoptionDate.Value.ToString("dd/MM/yyyy"))

        If (Not absents.IsNullOrEmpty()) Then
            parametersForReport.Add("AbsentManagers", JsonConvert.SerializeObject(absents))
        End If

        If (Not managersParameter.IsNullOrEmpty()) Then
            parametersForReport.Add("Managers", JsonConvert.SerializeObject(managersParameter))
        End If


        Return parametersForReport
    End Function
#End Region

End Class