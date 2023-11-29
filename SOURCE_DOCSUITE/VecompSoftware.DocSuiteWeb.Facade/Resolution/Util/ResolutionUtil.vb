Imports System.Linq
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports VecompSoftware.Helpers
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports Newtonsoft.Json
Imports VecompSoftware.Services.Logging

<Obsolete("Spostare tutto nelle facade appropriate")>
Public Class ResolutionUtil

#Region " Fields "

    Public Const DefaultOdgNameFormat As String = "{0}-Print-{1:HHmmss}-{2}.mht"

    Private Shared _instance As ResolutionUtil
    Private _envGrouSelected As String = Nothing
    Protected _resolutionFacade As ResolutionFacade

#End Region

#Region " Constructor "

    Public Sub New()
        _resolutionFacade = New ResolutionFacade
    End Sub

#End Region

#Region " Properties "

    Protected ReadOnly Property ResolutionFacade() As ResolutionFacade
        Get
            Return _resolutionFacade
        End Get
    End Property

    Public ReadOnly Property CommonInstance() As CommonUtil
        Get
            Return CommonUtil.GetInstance
        End Get
    End Property

#End Region

#Region "Singleton"
    Public Shared Function GetInstance() As ResolutionUtil
        If _instance Is Nothing Then
            _instance = New ResolutionUtil
        End If
        Return (_instance)
    End Function
#End Region

    Private Shared Function CreateMhtTableCell(ByVal openDeclaration As String, ByVal closeDaclaration As String, ByVal text As String) As String
        Return String.Format("{0}{1}{2}", openDeclaration, text, closeDaclaration)
    End Function

    Private Shared Function CreateMhtTableRow(ByVal openDeclaration As String, ByVal closeDaclaration As String, ByVal cells As String()) As String
        Dim streamTable As String = openDeclaration
        For Each cell As String In cells
            streamTable = String.Format("{0}{1}", streamTable, cell)
        Next
        Return String.Format("{0}{1}", streamTable, closeDaclaration)
    End Function

    Private Shared Function CreateHashTable(ByVal location As Location, ByVal idCatena As Integer) As String
        'Creo l'header della tabella
        Dim tableStream As String = "<table class=3DMsoTableGrid border=3D0 cellspacing=3D0 cellpadding=3D0 style=3D'border-collapse:collapse;border:none;mso-yfti-tbllook:1184; mso-padding-alt:0cm 5.4pt 0cm 5.4pt;mso-border-insideh:none;mso-border-insidev: none'>"

        Dim docs As List(Of DocumentInfo) = BiblosDocumentInfo.GetDocuments(New UIDChain(location.ReslBiblosDSDB, idCatena)).Cast(Of DocumentInfo)().ToList()

        For Each doc As DocumentInfo In docs

            'Nome del file
            Dim cellFileName As String = CreateMhtTableCell("<td width=3D379 valign=3Dtop style=3D'width:284.4pt;padding:0cm 5.4pt 0cm 5.4pt'><p class=3DMsoNormal><span style=3D'font-size:8.5pt;font-family:""Verdana"",""sans-serif""; color:black'>", "<o:p></o:p></span></p></td>", doc.Name)
            'Valore dell'hash
            Dim fileHash As String = doc.Hash.Replace("=", "=3D")
            Dim cellFileHash As String = CreateMhtTableCell("<td width=3D345 valign=3Dtop style=3D'width:258.75pt;padding:0cm 5.4pt 0cm 5.4pt'><p class=3DMsoNormal><i style=3D'mso-bidi-font-style:normal'><span style=3D'font-size:8.5pt;font-family:""Verdana"",""sans-serif"";color:black'>", "</span></i><span style=3D'font-size:8.5pt;font-family:""Verdana"",""sans-serif"";color:black'><o:p></o:p></span></p></td>", fileHash)
            tableStream = String.Format("{0}{1}", tableStream, CreateMhtTableRow("<tr>", "</tr>", New String() {cellFileName, cellFileHash}))
        Next

        'Chiudo la tabella
        tableStream = String.Format("{0}{1}", tableStream, "</table>")
        Return tableStream
    End Function

    Public Shared Function GeneraStampaODG(ByVal resl As Resolution, ByVal pWorkFlowData As String,
            ByVal pData As DateTime, ByVal pViewReslType As Short, ByVal pStepDesc As String,
            ByVal pStepTemplate As String,
            Optional ByVal pTempPath As String = "", Optional ByVal fileName As String = "", Optional ByVal workflow As TabWorkflow = Nothing,
            Optional ByVal applicationPath As String = "", Optional ByVal idRole As String = "", Optional presentSigners As IList(Of String) = Nothing) As FileInfo

        ' TODO: spostare nella ResolutionODGFacade
        If String.IsNullOrEmpty(applicationPath) Then
            applicationPath = CommonUtil.GetInstance.AppPath
        End If
        If String.IsNullOrEmpty(pTempPath) Then
            pTempPath = CommonUtil.GetInstance.AppTempPath
        End If

        'Selettore messo per compatibilità, sarebbe molto più opportuno passare ad una segnatura di metodo più light
        'solo con lo stretto necessario: resolution, pData, workflow
        'il resto è derivabile
        If (workflow IsNot Nothing) Then
            pViewReslType = resl.Type.Id
            pStepDesc = workflow.Description
            pStepTemplate = workflow.Template
        Else
            Dim twf As TabWorkflowFacade = New TabWorkflowFacade
            workflow = twf.GetByDescription(pStepDesc, resl.WorkflowType)
        End If

        Dim fSource As String

        If String.IsNullOrEmpty(pStepTemplate) Then
            If pViewReslType = ResolutionType.IdentifierDelibera Then
                If pStepDesc = "Ritiro Pubblicazione" Then
                    fSource = applicationPath & "Resl\Stampe\AUSL-RE\FronteSpizioDeliberaRitiro.mht"
                Else
                    fSource = applicationPath & "Resl\Stampe\AUSL-RE\FronteSpizioDelibera.mht"
                End If
            Else
                If pStepDesc = "Ritiro Pubblicazione" Then
                    fSource = applicationPath & "Resl\Stampe\AUSL-RE\FronteSpizioAttoRitiro.mht"
                Else
                    fSource = applicationPath & "Resl\Stampe\AUSL-RE\FronteSpizioAtto.mht"
                End If
            End If
        Else
            Dim fullPath As String = Path.Combine(applicationPath, "Resl\Stampe\")
            fSource = Path.Combine(fullPath, pStepTemplate)
        End If

        Dim fText As String = File.ReadAllText(fSource)

        'EF 20120112 Prima prova di generazione frontalino personalizzata. Da valutare.
        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("AUSL-PC") AndAlso DocSuiteContext.Current.ResolutionEnv.CustomFrontespiecePrintEnabled Then
            fText = fText.Replace("[OGGETTO]", HttpUtility.HtmlEncode(resl.ResolutionObject))
            Dim roleUserFacadeObject As New RoleUserFacade
            Select Case pViewReslType
                    'GESTIONE DELIBERE
                Case ResolutionType.IdentifierDelibera
                    fText = fText.Replace("[NUMERODELIBERA]", resl.Number)
                    fText = fText.Replace("[DATADELIBERA]", resl.AdoptionDate.DefaultString())

                    Select Case pStepDesc
                        Case "Adozione"
                            If DocSuiteContext.Current.ResolutionEnv.DelibereSignsReportEnabled Then
                                Dim message As String
                                Dim collaborationResl As Collaboration = FacadeFactory.Instance.CollaborationFacade.GetByResolution(resl)
                                If Not collaborationResl Is Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers IsNot Nothing Then
                                    'Ciclo su ogni direttore indicato da parametro, perchè nel parametro sono indicati i direttori che devono risultare nel frontalino di adozione
                                    For Each manager As CollaborationManagerModel In DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Values
                                        message = String.Empty
                                        Dim log As CollaborationLog = collaborationResl.CollaborationLogs.Where(Function(x) x.LogType = CollaborationLogType.JS.ToString()).OrderBy(Function(p) p.LogDate).LastOrDefault()
                                        'Controllo se il direttore è tra i firmatari del documento e tra i firmatari della collaborazione
                                        If presentSigners IsNot Nothing AndAlso presentSigners.Count > 0 AndAlso presentSigners.Any(Function(t) t.Eq(manager.Account)) AndAlso
                                        collaborationResl.CollaborationSigns.Any(Function(c) c.SignUser.Eq(manager.Account) AndAlso (Not c.IsAbsent.HasValue OrElse Not c.IsAbsent.Value)) Then
                                            message = "FIRMATO"

                                            'Controllo se il direttore ha firmato con funzioni vicariali, 
                                            'cioè se è il sostituto di un firmatario e il suo piu alto grado tra i disegni di funzione in cui è configurato è quello di VICE
                                            If log IsNot Nothing Then
                                                Dim absentManagers As IList(Of AbsentManager) = JsonConvert.DeserializeObject(Of AbsentManager())(log.LogDescription)
                                                Dim managerAccount As AbsentManager = absentManagers.Where(Function(x) x.Substitution.Eq(manager.Account)).FirstOrDefault()

                                                If managerAccount Is Nothing Then
                                                    FileLogger.Warn(LogName.FileLog, String.Concat("Il direttore ", manager.Account, " non è indicato come sostituto di firmatari della collaborazione"))
                                                Else
                                                    Dim previousSigner As CollaborationSign = collaborationResl.CollaborationSigns.Where(Function(c) c.SignUser.Eq(managerAccount.Manager)).FirstOrDefault()
                                                    If manager.IsAbsenceManaged AndAlso previousSigner IsNot Nothing AndAlso previousSigner.SignName.IndexOf("(D)") > -1 Then
                                                        message = String.Concat(message, " CON FUNZIONI VICARIALI")
                                                    End If
                                                End If
                                            End If
                                        End If

                                        'Controllo se il direttore NON è tra i firmatari del documento e se è tra i firmatari della collaborazione marcati come assenti
                                        If manager.IsAbsenceManaged AndAlso (presentSigners Is Nothing OrElse Not presentSigners.Any(Function(t) t.Eq(manager.Account))) AndAlso
                                            collaborationResl.CollaborationSigns.Any(Function(c) c.SignUser.Eq(manager.Account) AndAlso (c.IsAbsent.HasValue AndAlso c.IsAbsent.Value)) Then
                                            message = "ASSENTE"
                                            Dim absentSigner As CollaborationSign = collaborationResl.CollaborationSigns.Where(Function(c) c.SignUser.Eq(manager.Account) AndAlso (c.IsAbsent.HasValue AndAlso c.IsAbsent.Value)).First()
                                            If absentSigner.SignName.IndexOf("(D)") > -1 AndAlso log IsNot Nothing Then
                                                Dim absentManagers As IList(Of AbsentManager) = JsonConvert.DeserializeObject(Of AbsentManager())(log.LogDescription)
                                                Dim managerAccount As AbsentManager = absentManagers.Where(Function(x) x.Manager.Eq(manager.Account)).FirstOrDefault()

                                                If managerAccount Is Nothing Then
                                                    FileLogger.Warn(LogName.FileLog, String.Concat("Il direttore ", manager.Account, " non è tra i firmatari della collaborazione"))
                                                Else
                                                    Dim substitute As CollaborationSign = collaborationResl.CollaborationSigns.Where(Function(c) c.SignUser.Eq(managerAccount.Substitution) AndAlso (Not c.IsAbsent.HasValue OrElse (c.IsAbsent.HasValue AndAlso Not c.IsAbsent.Value))).FirstOrDefault()
                                                    If substitute IsNot Nothing Then
                                                        Dim description As String = substitute.SignName.Replace("- DA", String.Empty).Replace("- DS", String.Empty).Replace("- DG", String.Empty)
                                                        message = String.Concat(message, ", VICARIATO DA ", description)
                                                    End If
                                                End If
                                            End If
                                        End If

                                        If manager.IsAbsenceManaged AndAlso String.IsNullOrEmpty(message) Then
                                            message = "FIRMA NON PRESENTE"
                                        End If

                                        Select Case manager.Type
                                            Case "DG"
                                                fText = fText.Replace("[FIRMADG]", message)
                                            Case "DS"
                                                fText = fText.Replace("[FIRMADS]", message)
                                            Case "DA"
                                                fText = fText.Replace("[FIRMADA]", message)
                                            Case "DSS"
                                                fText = fText.Replace("[FIRMADSS]", message)
                                        End Select
                                    Next
                                End If
                            Else
                                fText = fText.Replace("[FIRMADG]", String.Empty)
                                fText = fText.Replace("[FIRMADS]", String.Empty)
                                fText = fText.Replace("[FIRMADA]", String.Empty)
                                fText = fText.Replace("[FIRMADSS]", String.Empty)
                            End If

                        Case "Pubblicazione"
                            'Protocollo di Invio al Collegio Sindacale
                            Dim protcollsind As String = Resolution.FormatProtocolLink(resl.SupervisoryBoardProtocolLink, String.Empty)
                            If (protcollsind = "") Then protcollsind = "-"
                            fText = fText.Replace("[PROTCOLLSIND]", protcollsind)
                            'Data di Invio al Collegio Sindacale
                            Dim datacollsind As String = String.Format("{0:dd/MM/yyyy}", resl.SupervisoryBoardWarningDate)
                            If (datacollsind = "") Then datacollsind = " - "
                            fText = fText.Replace("[DATACOLLSIND]", datacollsind)
                            'Protocollo di Invio alla Conferenza dei Sindaci
                            Dim protconfsind As String = Resolution.FormatProtocolLink(resl.ManagementProtocolLink, String.Empty)
                            If (protconfsind = "") Then protconfsind = " - "
                            fText = fText.Replace("[PROTCONFSIND]", protconfsind)
                            'Data di Invio alla Conferenza dei Sindaci
                            Dim dataconfsind As String = String.Format("{0:dd/MM/yyyy}", resl.ManagementWarningDate)
                            If (dataconfsind = "") Then dataconfsind = " - "
                            fText = fText.Replace("[DATACONFSIND]", dataconfsind)
                            'Protocollo di Invio alla Regione
                            Dim protinvioreg As String = Resolution.FormatProtocolLink(resl.RegionProtocolLink, String.Empty)
                            If (protinvioreg = "") Then protinvioreg = " - "
                            fText = fText.Replace("[PROTINVIOREG]", protinvioreg)
                            'Data di Invio alla Regione
                            Dim datainvioreg As String = String.Format("{0:dd/MM/yyyy}", resl.WarningDate)
                            If (datainvioreg = "") Then datainvioreg = " - "
                            fText = fText.Replace("[DATAINVIOREG]", datainvioreg)
                            'Data di Pubblicazione
                            fText = fText.Replace("[DATAPUBBL]", String.Format("{0:dd/MM/yyyy}", pData))
                            'Numero di Pubblicazione Web
                            Dim resolutionFacade As New ResolutionFacade()
                            Dim numeroPubblicazioneAttuale As String = resolutionFacade.GetLastOrNewPublicationNumber(resl, New List(Of Integer)(New Integer() {1, 2}))
                            Dim numeroPubblicazioneRevocato As String = resolutionFacade.GetLastRevokedNumber(resl)
                            If (Not String.IsNullOrEmpty(numeroPubblicazioneRevocato)) Then
                                numeroPubblicazioneRevocato = String.Format(" in sostituzione del n. {0}", numeroPubblicazioneRevocato)
                            End If
                            fText = fText.Replace("[NUMPUBBL]", numeroPubblicazioneAttuale)
                            'Numero sostitutivo Pubblicazione Web
                            fText = fText.Replace("[SOSTPUBBL]", numeroPubblicazioneRevocato)
                        Case "FrontalinoEsecutività", "Esecutività"
                            'Data risposta della Regione
                            Dim datarispreg As String = String.Format("{0:dd/MM/yyyy}", resl.ResponseDate)
                            If (datarispreg = "") Then datarispreg = " - "
                            fText = fText.Replace("[DATARISPDIREG]", datarispreg)
                            'Protocollo risposta della Regione
                            Dim protrispreg As String = Resolution.FormatProtocolLink(resl.ResponseProtocol, String.Empty)
                            If (protrispreg = "") Then protrispreg = " - "
                            fText = fText.Replace("[PROTRISPDIREG]", protrispreg)
                            'Data Invio chiarimenti alla Regione
                            Dim datarispauslareg As String = String.Format("{0:dd/MM/yyyy}", resl.ConfirmDate)
                            If (datarispauslareg = "") Then datarispauslareg = " - "
                            fText = fText.Replace("[DATARISPAUSLAREG]", datarispauslareg)
                            'Protocollo Invio Chiarimenti alla Regione
                            Dim protrispauslareg As String = Resolution.FormatProtocolLink(resl.ConfirmProtocol, String.Empty)
                            If (protrispauslareg = "") Then protrispauslareg = " - "
                            fText = fText.Replace("[PROTRISPAUSLAREG]", protrispauslareg)
                            'Risposta finale della Regione
                            Dim rispostareg As String = resl.ControllerOpinion
                            If (rispostareg = "") Then rispostareg = " - "
                            fText = fText.Replace("[RISPOSTAREG]", rispostareg)
                            ''Esecutività
                            Dim dataesec As String = CType(pData, String)
                            If String.IsNullOrEmpty(dataesec) Then
                                dataesec = " - "
                            End If
                            fText = fText.Replace("[DATAESEC]", dataesec)
                    End Select

                        ' Gestione DETERMINE
                Case ResolutionType.IdentifierDetermina
                    Dim roleName As String
                    Dim roleDirector As String
                    Dim roleUsers As IList(Of RoleUser)
                    Dim roleServiceCode As String = resl.ServiceNumber.Split("/"c)(0)
                    Dim roleId As Integer
                    If Not String.IsNullOrEmpty(idRole) AndAlso Integer.TryParse(idRole, roleId) Then
                        Dim roleFacadeObject As New RoleFacade
                        Dim roleObject As Role = roleFacadeObject.GetById(roleId)
                        If roleObject Is Nothing Then
                            Throw New DocSuiteException("Errore ricerca settore", String.Format("Codice [{0}] non trovato.", roleServiceCode))
                        End If

                        roleName = roleObject.Name
                        roleUsers = roleUserFacadeObject.GetByRoleId(roleId)
                        roleDirector = vbNullString

                        If Not roleUsers.IsNullOrEmpty() Then
                            If roleUsers.Any(Function(r) r.Type = RoleUserType.D.ToString()) Then
                                roleDirector = roleUsers.First(Function(ru) ru.Type = RoleUserType.D.ToString()).Description
                            Else
                                Throw New DocSuiteException(String.Format("Nel settore {0} non è configurato nessun dirigente.", roleObject.Name))
                            End If
                        End If

                    End If

                    fText = fText.Replace("[NOMESET]", roleName)
                    fText = fText.Replace("[DIRSET]", roleDirector)

                    fText = fText.Replace("[NUMERODETERMINA]", resl.ServiceNumber.Split("/"c)(1))
                    fText = fText.Replace("[DATADETERMINA]", resl.AdoptionDate.DefaultString())

                    Select Case pStepDesc
                        Case "Pubblicazione"
                            ''Protocollo Collegio Sindacale
                            Dim protCollSind As String = Resolution.FormatProtocolLink(resl.SupervisoryBoardProtocolLink, String.Empty)
                            If (protCollSind = "") Then protCollSind = " - "
                            fText = fText.Replace("[PROTCOLLSIND]", protCollSind)
                            ''Data Collegio Sindacale
                            Dim dataCollSind As String = String.Format("{0:dd/MM/yyyy}", resl.SupervisoryBoardWarningDate)
                            If (dataCollSind = "") Then dataCollSind = " - "
                            fText = fText.Replace("[DATACOLLSIND]", dataCollSind)
                            ''Data pubblicazione
                            Dim dataPubbl As String = String.Format("{0:dd/MM/yyyy}", pData)
                            If (dataPubbl = "") Then dataPubbl = " - "
                            fText = fText.Replace("[DATAPUBBL]", dataPubbl)

                            'Numero di Pubblicazione Web
                            Dim resolutionFacade As New ResolutionFacade()
                            Dim numeroPubblicazioneAttuale As String = resolutionFacade.GetLastOrNewPublicationNumber(resl, New List(Of Integer)(New Integer() {1, 2}))
                            Dim numeroPubblicazioneRevocato As String = resolutionFacade.GetLastRevokedNumber(resl)
                            If (Not String.IsNullOrEmpty(numeroPubblicazioneRevocato)) Then
                                numeroPubblicazioneRevocato = String.Format(" in sostituzione del n. {0}", numeroPubblicazioneRevocato)
                            End If
                            fText = fText.Replace("[NUMPUBBL]", numeroPubblicazioneAttuale)
                            'Numero sostitutivo Pubblicazione Web
                            fText = fText.Replace("[SOSTPUBBL]", numeroPubblicazioneRevocato)
                    End Select

            End Select
        Else
            If Not String.IsNullOrEmpty(resl.InclusiveNumber) AndAlso resl.InclusiveNumber <> "*" Then
                fText = fText.Replace("[NUMREF]", resl.InclusiveNumber)
            Else
                fText = fText.Replace("[NUMREF]", resl.Year.ToString & "/" & resl.ServiceNumber)
            End If

            fText = fText.Replace("[DATA]", resl.AdoptionDate.DefaultString())
            fText = fText.Replace("[OGGETTO]", HttpUtility.HtmlEncode(resl.ResolutionObject))

            If pViewReslType = ResolutionType.IdentifierDetermina Then
                If String.IsNullOrEmpty(resl.Container.Note) Then
                    fText = fText.Replace("[CONTAINER]", HttpUtility.HtmlEncode(resl.Container.Name.ToUpper))
                Else
                    fText = fText.Replace("[CONTAINER]", HttpUtility.HtmlEncode(resl.Container.Note.ToUpper))
                End If
            End If

            If pStepDesc = "Ritiro Pubblicazione" Then
                fText = fText.Replace("[DATACONTROLLOCONFORM]", String.Format("{0:dd/MM/yyyy}", resl.PublishingDate))
                fText = fText.Replace("[UTENTECONTROLLOCONFORM]", CommonAD.GetDisplayName(resl.PublishingUser))
                fText = fText.Replace("[DATARITIROLABEL]", ", ")
                fText = fText.Replace("[DATARITIRO]", String.Format("{0:dd/MM/yyyy}", pData))
                fText = fText.Replace("[DATAINSERT]", String.Format("{0:dd/MM/yyyy}", resl.PublishingDate))
                fText = fText.Replace("[ADOPTIONUSER]", CommonAD.GetDisplayName(resl.AdoptionUser))

                Dim fileResolutionFacade As FileResolutionFacade = New FileResolutionFacade()
                Dim fileResolution As FileResolution = fileResolutionFacade.GetById(resl.Id)
                'Calcolo l'ID catena dello step attuale
                Dim idCatena As Integer = ReflectionHelper.GetPropertyCase(fileResolution, workflow.FieldDocument)
                Dim streamTable As String = CreateHashTable(resl.Location, idCatena)
                fText = fText.Replace("[TABELLAHASH]", streamTable)
            Else
                fText = fText.Replace("[DATARITIROLABEL]", "")
                fText = fText.Replace("[DATARITIRO]", "")
                fText = fText.Replace("[DATAINSERT]", String.Format("{0:dd/MM/yyyy}", pData))
            End If

            If Not TabWorkflowFacade.TestManagedWorkflowDataProperty(pWorkFlowData, "Frontespizio", "LeaveDate", "") Then
                fText = fText.Replace("[DATADAL]", String.Format("{0:dd/MM/yyyy}", pData))
                fText = fText.Replace("[DATAAL]", String.Format("{0:dd/MM/yyyy}", pData.AddDays(15)))
                fText = fText.Replace("[DATARITIRO]", "")
            Else
                fText = fText.Replace("[DATADAL]", String.Format("{0:dd/MM/yyyy}", resl.PublishingDate))
                fText = fText.Replace("[DATAAL]", String.Format("{0:dd/MM/yyyy}", resl.PublishingDate.Value.AddDays(15)))
                fText = fText.Replace("[DATARITIRO]", ", " & String.Format("{0:dd/MM/yyyy}", pData))
            End If

            Dim wps As IList(Of WebPublication) = New WebPublicationFacade().GetByResolution(resl)
            If Not wps Is Nothing AndAlso wps.Count = 1 Then
                fText = fText.Replace("[NUMEROPUBBLICAZIONE]", wps(0).ExternalKey)
            Else
                fText = fText.Replace("[NUMEROPUBBLICAZIONE]", "")
            End If
        End If

        Dim tempFile As String = String.Empty
        If Not String.IsNullOrEmpty(fileName) Then
            tempFile = fileName
        Else
            tempFile = String.Format(DefaultOdgNameFormat, CommonShared.UserDocumentName, DateTime.Now, resl.Id)
        End If

        'EF 20120222 Correzione lettere accentate per l'mht profilo us-ascii
        fText = fText.Replace("À", "&Agrave;")
        fText = fText.Replace("È", "&Egrave;")
        fText = fText.Replace("É", "&Eacute;")
        fText = fText.Replace("Ì", "&Igrave;")
        fText = fText.Replace("Ò", "&Ograve;")
        fText = fText.Replace("Ù", "&Ugrave;")
        fText = fText.Replace("à", "&agrave;")
        fText = fText.Replace("è", "&egrave;")
        fText = fText.Replace("é", "&eacute;")
        fText = fText.Replace("ì", "&igrave;")
        fText = fText.Replace("ò", "&ograve;")
        fText = fText.Replace("ù", "&ugrave;")
        Dim destination As String = Path.Combine(pTempPath, tempFile)

        'EF 20120222 Specificato il profilo default che corrisponde all'ANSI puro
        Dim sw As StreamWriter = New StreamWriter(destination, False, Encoding.Default)
        sw.Write(fText)
        sw.Close()

        Return New FileInfo(destination)

    End Function

    Public Shared Function GroupOmissisTest() As Boolean
        If InStr(CommonUtil.UserConnectedGroups, "'" & DocSuiteContext.Current.ResolutionEnv.GroupOmissis & "'") <> 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function GeneraStampaConSostituzioni(ByVal resl As Resolution, ByVal FileText As String,
            ByRef sw As StreamWriter, ByVal IsFirst As Boolean, ByVal HeadingFrontalino As String, ByVal Omissis As Boolean) As Boolean

        Dim ss As String
        If resl IsNot Nothing Then
            If (Not IsFirst) Then
                sw.Write("<P CLASS='BREAK'></P><BR>")
            End If
            FileText = FileText.Replace("@CODIFICA@", "<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
            FileText = FileText.Replace("@NUMERO@", String.Format("{0:0000000}", resl.Number))
            FileText = FileText.Replace("@CODICESERV@", UCase("" & If(Not resl.ResolutionContactProposers.IsNullOrEmpty(), resl.ResolutionContactProposers(0).Contact.Code, String.Empty)))
            FileText = FileText.Replace("@ANNO@", If(resl.Year.HasValue, resl.Year.Value.ToString(), ""))
            FileText = FileText.Replace("@DATAADOZIONE@", String.Format("{0:dd MMMM yyyy}", resl.AdoptionDate).ToUpper)
            If (resl.ImmediatelyExecutive.GetValueOrDefault(False) = True) Then
                FileText = FileText.Replace("@IE@", "IMMEDIATAMENTE ESEGUIBILE")
            Else
                FileText = FileText.Replace("@IE@", "")
            End If
            If Not String.IsNullOrEmpty(resl.SupervisoryBoardProtocolLink) Then
                Dim s() As String = Split(resl.SupervisoryBoardProtocolLink, "|")
                FileText = FileText.Replace("@NUMPROT@", ProtocolFacade.ProtocolFullNumber(s(0), s(1)))
                FileText = FileText.Replace("@DATAPROT@", String.Format("{0:dd/MM/yyyy}", CDate(s(3))))
            Else
                FileText = FileText.Replace("@NUMPROT@", "")
                FileText = FileText.Replace("@DATAPROT@", "")
            End If
            If (resl.OCRegion.GetValueOrDefault(False)) Then
                FileText = FileText.Replace("@DATASPEDREGIONE@", String.Format("{0:dd/MM/yyyy}", resl.WarningDate))
                If Not String.IsNullOrEmpty(resl.RegionProtocolLink) Then
                    Dim s() As String = Split(resl.RegionProtocolLink, "|")
                    FileText = FileText.Replace("@NUMPROTSPEDREGIONE@", ProtocolFacade.ProtocolFullNumber(s(0), s(1)))
                Else
                    FileText = FileText.Replace("@NUMPROTSPEDREGIONE@", "")
                End If
                FileText = FileText.Replace("@DATARICREGIONE@", String.Format("{0:dd/MM/yyyy}", resl.ConfirmDate))
                FileText = FileText.Replace("@DGR@", "" & resl.DGR)
                FileText = FileText.Replace("@DATARISPREGIONE@", String.Format("{0:dd/MM/yyyy}", resl.ResponseDate))
                Dim sCommentoRegione As String = String.Empty
                If resl.ControllerStatus IsNot Nothing Then sCommentoRegione = resl.ControllerStatus.Description
                FileText = FileText.Replace("@COMMENTOREGIONE@", sCommentoRegione)
            Else
                FileText = FileText.Replace("@DATASPEDREGIONE@", "")
                FileText = FileText.Replace("@NUMPROTSPEDREGIONE@", "")
                FileText = FileText.Replace("@DATARICREGIONE@", "")
                FileText = FileText.Replace("@DGR@", "")
                FileText = FileText.Replace("@DATARISPREGIONE@", "")
                FileText = FileText.Replace("@COMMENTOREGIONE@", "")
            End If
            If (resl.OCManagement.GetValueOrDefault(False)) Then
                FileText = FileText.Replace("@DATAPROTGESTIONE@", String.Format("{0:dd/MM/yyyy}", resl.ManagementWarningDate))
                If Not String.IsNullOrEmpty(resl.ManagementProtocolLink) Then
                    Dim s() As String = Split(resl.ManagementProtocolLink, "|")
                    FileText = FileText.Replace("@NUMPROTGESTIONE@", ProtocolFacade.ProtocolFullNumber(s(0), s(1)))
                Else
                    FileText = FileText.Replace("@NUMPROTGESTIONE@", "")
                End If
            Else
                FileText = FileText.Replace("@DATAPROTGESTIONE@", "")
                FileText = FileText.Replace("@NUMPROTGESTIONE@", "")
            End If
            FileText = FileText.Replace("@DATAPUBBLICAZIONE@", String.Format("{0:dd MMMM yyyy}", resl.PublishingDate).ToUpper)
            FileText = FileText.Replace("@DATAESECUTIVITA@", String.Format("{0:dd MMMM yyyy}", resl.EffectivenessDate).ToUpper)
            FileText = FileText.Replace("@DATAODIERNA@", String.Format("{0:dd MMMM yyyy}", DateTime.Now))
            'guz
            Dim sOggetto As String = OggettoPrivacy(resl, Omissis)
            FileText = FileText.Replace("@OGGETTO@", sOggetto)
            FileText = FileText.Replace("@NOTE@", StringHelper.ReplaceCrLf(resl.Note))
            '--
            FileText = FileText.Replace("@ANNOLET@", StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Year)))
            FileText = FileText.Replace("@GIORNOLET@", StringHelper.UppercaseFirst(NumeriInLettereHelper.Convert(resl.AdoptionDate.Value.Day)))
            FileText = FileText.Replace("@MESELET@", StringHelper.UppercaseFirst(resl.AdoptionDate.Value.MonthName))
            FileText = FileText.Replace("@ART14@", If(resl.OCSupervisoryBoard.GetValueOrDefault(False), "art.14", ""))
            FileText = FileText.Replace("@REGIONE@", If(resl.OCRegion.GetValueOrDefault(False), "Regione", ""))
            FileText = FileText.Replace("@GESTIONE@", If(resl.OCManagement.GetValueOrDefault(False), "Controllo Gestione", ""))
            FileText = FileText.Replace("@OCCorteConti@", If(resl.OCCorteConti.GetValueOrDefault(False), "Corte dei Conti", ""))
            ss = StringHelper.ReplaceCrLf(resl.OtherDescription)
            FileText = FileText.Replace("@ALTRO@", If(resl.OCOther.GetValueOrDefault(False), ss, ""))
            Dim sFront As String = "" & resl.Container.HeadingFrontalino
            If Not String.IsNullOrEmpty(HeadingFrontalino) Then
                If InStr(sFront, "@MOTIVAZIONE@") > 0 Then
                    sFront = Mid(sFront, 1, InStr(sFront, "@MOTIVAZIONE@") - 1)
                    sFront &= HeadingFrontalino.Replace("%BR%", "<BR>")
                End If
            Else
                sFront = sFront.Replace("@MOTIVAZIONE@", "")
            End If
            FileText = FileText.Replace("@HEADINGF@", sFront)
            FileText = FileText.Replace("@HEADINGL@", "" & resl.Container.HeadingLetter)

            'write
            sw.Write(FileText)
        End If
    End Function

    Public Shared Function OggettoPrivacy(ByRef resl As Resolution, ByVal omissis As Boolean) As String
        Dim sOggetto As String = ""
        If ResolutionUtil.GroupOmissisTest And (resl.Container.Privacy.HasValue AndAlso resl.Container.Privacy.Value = 1) Then
            If omissis Then
                sOggetto = StringHelper.ReplaceCrLf(resl.ResolutionObjectPrivacy)
            Else
                sOggetto = StringHelper.ReplaceCrLf(resl.ResolutionObject)
            End If
        Else
            sOggetto = StringHelper.ReplaceCrLf(resl.ResolutionObject)
        End If
        Return sOggetto
    End Function

    Public Sub InserisciFrontalino(ByVal data As DateTime, ByVal idResolution As Integer, ByVal resolutionType As Short, ByVal stepDescription As String, ByVal location As Location, ByRef chainId As Integer, ByVal number As String, ByVal stepId As Short, ByVal idRole As String, Optional presentSigners As IList(Of String) = Nothing)
        Dim resolution As Resolution = ResolutionFacade.GetById(idResolution)
        Dim fileName As String = GetNomeFrontalino(resolution.Type.Id, stepDescription)
        Dim fi As FileInfo = GeneraFrontalino(data, resolution, stepDescription, stepId, idRole, presentSigners)

        Try
            Dim signature As String = ResolutionFacade.SqlResolutionGetNumber(resolution.Id, data.Year.ToString(), number, data, True)

            Dim frontalinoAttributes As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            frontalinoAttributes = Service.GetBaseAttributes(fileName, signature)
            frontalinoAttributes.Add(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, "0")

            Dim uid As UIDDocument = Service.AddFile(New UIDLocation() With {.Archive = location.ReslBiblosDSDB}, chainId,
                                    fi, frontalinoAttributes)

            If DocSuiteContext.Current.PrivacyLevelsEnabled Then
                FacadeFactory.Instance.ResolutionLogFacade.Insert(resolution, ResolutionLogType.LP, String.Format("Associato livello privacy {0} al {1} {2} [{3}]", 0, "documento", fileName, uid.Chain.Id))
            End If

            ResolutionFacade.SqlResolutionDocumentUpdate(idResolution, uid.Chain.Id, ResolutionFacade.DocType.Disposizione)
        Catch ex As Exception
            Dim resolutionLogFacade As New ResolutionLogFacade()
            resolutionLogFacade.Log(resolution, ResolutionLogType.RX, String.Format("ERR.ATTI.FRONT: Impossibile generare il frontespizio: {0}.", ex.Message))
            Throw New DocSuiteException("Errore generazione Frontespizio", "Impossibile generare il Frontespizio.", ex)
        End Try
    End Sub

    ''' <summary> Genera un FileInfo con i dati del frontalino. </summary>
    ''' <param name="data">Non so che data sia</param>
    ''' <param name="resolution">Atto di cui generare il frontalino</param>
    ''' <param name="stepDescription">Descrizione dello step di workflow</param>
    ''' <param name="stepId">Id dello step di workflow</param>
    Public Shared Function GeneraFrontalino(data As DateTime, resolution As Resolution, stepDescription As String, stepId As Short, idRole As String, Optional presentSigners As IList(Of String) = Nothing) As FileInfo
        If resolution Is Nothing Then
            Throw New Exception("Impossibile estrarre i dati.")
        End If

        Dim workflow As TabWorkflow = Nothing
        Dim workflowData As String = Nothing
        If (New TabWorkflowFacade).GetByStep(resolution.WorkflowType, stepId + 1S, workflow) Then
            workflowData = workflow.ManagedWorkflowData
        End If

        Try
            Dim info As FileInfo = GeneraStampaODG(resolution, workflowData, data, resolution.Type.Id, stepDescription, workflow.Template, "", "", workflow, "", idRole, presentSigners)
            Dim fileName As String = GetNomeFrontalino(resolution.Type.Id, stepDescription)
            Dim documentInfo As New FileDocumentInfo(info)
            Return documentInfo.SavePdfNoSignature(New DirectoryInfo(CommonUtil.GetInstance.AppTempPath), FileHelper.UniqueFileNameFormat(fileName, DocSuiteContext.Current.User.UserName))
        Catch ex As Exception
            Throw New DocSuiteException("Generazione Frontalino", String.Format("Errore in fase di generazione frontalino per resolution [{0}].", resolution.Id), ex)
        End Try

    End Function

    ''' <summary> Recupera il nome del frontalino. </summary>
    ''' <param name="resolutionTypeId">Tipo di atto</param>
    ''' <param name="stepDescription">Descrizione dello step di workflow</param>
    Private Shared Function GetNomeFrontalino(resolutionTypeId As Short, stepDescription As String) As String
        Dim retval As String = "Frontespizio.pdf"

        Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
            Case "AUSL-PC"
                If resolutionTypeId.Equals(ResolutionType.IdentifierDelibera) Then
                    Select Case stepDescription
                        Case "Adozione"
                            retval = "Relata di Adozione.pdf"
                        Case "Pubblicazione"
                            retval = "Relata di Pubblicazione.pdf"
                        Case "Esecutività"
                            retval = "Relata di Esecutività.pdf"
                    End Select
                ElseIf resolutionTypeId.Equals(ResolutionType.IdentifierDetermina) Then
                    Select Case stepDescription
                        Case "Adozione"
                            retval = "Relata di Adozione.pdf"
                        Case "Pubblicazione"
                            retval = "Relata di Pubblicazione ed Esecutività.pdf"
                    End Select
                End If

            Case Else
                Select Case stepDescription
                    Case "Ritiro Pubblicazione"
                        retval = "Frontespizio Ritirato.pdf"
                End Select
        End Select

        Return retval
    End Function
End Class
