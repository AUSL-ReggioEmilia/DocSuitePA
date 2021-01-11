Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.Helpers.PDF
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.DTO.Resolutions
Imports VecompSoftware.Helpers.ExtensionMethods

Partial Public Class ReslPrint
    Inherits ReslBasePage

#Region " Properties "

    Private ReadOnly Property PrintType() As String
        Get
            Return Request.QueryString("PrintType")
        End Get
    End Property

    Private ReadOnly Property CurrentTypeSelected As Short
        Get
            Return Convert.ToInt16(rblTipologia.SelectedValue)
        End Get
    End Property

    Public Property CurrentPrinterSession As Object
        Get
            Return Session("Printer")
        End Get
        Set(value As Object)
            Session("Printer") = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CommonInstance.UserDeleteTemp(TempType.P)
        AddNumber.Visible = False

        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub rblTipologia_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rblTipologia.SelectedIndexChanged
        UpdateTipologia()
    End Sub

    Private Sub chkLettera_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkLettera.CheckedChanged
        UpdateLettera()
    End Sub

    Private Sub rblTipoLettera_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblTipoLettera.SelectedIndexChanged
        VisualizzaCampiRicercaLettera(CurrentTypeSelected)
    End Sub

    Private Sub rblTipoLetteraDet_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rblTipoLetteraDet.SelectedIndexChanged
        VisualizzaCampiRicercaLettera(CurrentTypeSelected)
    End Sub

    'TODO: Prevedere se è possibile spostare la gestione del Finder nella Facade
    Private Sub AddNumber_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddNumber.Click, cmdAddNumberByAdottataIntervallo.Click
        Dim finder As New NHibernateResolutionFinder("ReslDB")
        finder.EagerLog = False
        Dim typeLetterSelected As String = Nothing
        Select Case CurrentTypeSelected
            Case ResolutionType.IdentifierDelibera
                typeLetterSelected = rblTipoLettera.SelectedValue
                finder.Delibera = True
                finder.Determina = False
            Case ResolutionType.IdentifierDetermina
                typeLetterSelected = rblTipoLetteraDet.SelectedValue
                finder.Delibera = False
                finder.Determina = True
        End Select
        finder.EnablePaging = False
        finder.EnableStatus = False

        finder.SortExpressions.Add("Number", "ASC")

        Dim sql As String = String.Empty
        Select Case typeLetterSelected
            Case "TAR-B", "TAR-A", "TAR-P", "AL"
                finder.ContainerIds = ddlContainers.SelectedValue
                finder.OCRegion = True

                Dim resolutions As IList(Of Resolution)
                Select Case CurrentTypeSelected
                    Case ResolutionType.IdentifierDelibera
                        GeneraAdozioneIntervallo(sql)
                        finder.SQLExpressions.Add("SqlExpression", New SQLExpression(sql))
                        resolutions = finder.DoSearch()

                        cblNumbers.Items.Clear()
                        cblNumbers.DataSource = resolutions
                        cblNumbers.DataTextField = "Number"
                        cblNumbers.DataValueField = "Id"
                        cblNumbers.DataBind()
                        If resolutions.Count = 0 Then
                            cblNumbers.Items.Add("Nessun elemento trovato.")
                            cblNumbers.Items(0).Enabled = False
                        End If
                    Case ResolutionType.IdentifierDetermina
                        sql = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) = '" & String.Format("{0:yyyyMMdd}", AdoptionDate.SelectedDate) & "'"
                        finder.SQLExpressions.Add("SqlExpression", New SQLExpression(sql))
                        resolutions = finder.DoSearch()

                        lstNumber.Items.Clear()
                        For Each resl As Resolution In resolutions
                            lstNumber.Items.Add(resl.NumberFormat("{0:0000000}"))
                        Next

                        txtNumber.Text = ""
                        If resolutions.Count = 0 Then
                            pnlListaNumeroRegione.Visible = False
                        Else
                            pnlListaNumeroRegione.Visible = True
                        End If
                End Select
        End Select
    End Sub

    Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSelectAll.Click
        SelectOrDeselectAll(True)
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnDeselectAll.Click
        SelectOrDeselectAll(False)
    End Sub

    Private Sub cmdStampa_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdStampa.Click
        CommonInstance.UserDeleteTemp(TempType.P)

        Select Case PrintType
            Case "F"
                If Not CheckedNode(tvwContenitore) Then
                    AjaxAlert("Campo contenitore obbligatorio.")
                    Exit Sub
                End If
                StampaModelloPredefinito()
            Case "LT"
                Dim seleziona As String = Nothing
                StampaLettera(PrintType, , seleziona)
                If chkElenco.Checked And seleziona <> "" Then
                    StampaElenco(False)
                End If
            Case "EDA"
                If Not CheckedNode(tvwContenitore) Then
                    AjaxAlert("Campo contenitore obbligatorio.")
                    Exit Sub
                End If
                If chkLettera.Checked Then
                    StampaLettera(PrintType, "TAS")
                End If
                StampaElenco()
            Case "RG"
                If Not CheckedNode(tvwContenitore) Then
                    AjaxAlert("Campo contenitore obbligatorio.")
                    Exit Sub
                End If
                Select Case CurrentTypeSelected
                    Case ResolutionType.IdentifierDelibera
                        StampaRegistroDel()
                    Case ResolutionType.IdentifierDetermina
                        StampaRegistroDetermine()
                End Select
            Case "REGPUB"
                If Not CheckedNode(tvwContenitore) Then
                    AjaxAlert("Campo contenitore obbligatorio.")
                    Exit Sub
                End If
                StampaRegistroPub()

        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        With AjaxManager.AjaxSettings
            .AddAjaxSetting(rblTipologia, pnlHeaders)
            .AddAjaxSetting(rblTipoLettera, pnlHeaders)
            .AddAjaxSetting(rblTipoLetteraDet, pnlHeaders)
            .AddAjaxSetting(cmdStampa, pnlHeaders)
            .AddAjaxSetting(AddNumber, pnlHeaders)
            .AddAjaxSetting(btnSelectAll, pnlHeaders)
            .AddAjaxSetting(btnDeselectAll, pnlHeaders)

            .AddAjaxSetting(rblTipologia, pnlControls)
            .AddAjaxSetting(rblTipoLettera, pnlControls)
            .AddAjaxSetting(rblTipoLetteraDet, pnlControls)
            .AddAjaxSetting(cmdStampa, pnlControls)
            .AddAjaxSetting(AddNumber, pnlControls)
            .AddAjaxSetting(btnSelectAll, pnlControls)
            .AddAjaxSetting(btnDeselectAll, pnlControls)
        End With
    End Sub

    Private Sub Initialize()
        BindRuolo()

        Dim reslContainers As New List(Of ListItem)
        Dim activeContainers As IList(Of ContainerRightsDto) = Facade.ContainerFacade.GetAllRights("Resl", 1)

        For Each container As ContainerRightsDto In activeContainers
            Dim containerName As String = container.Name
            Dim containerId As String = container.ContainerId.ToString()
            reslContainers.Add(New ListItem(containerName, containerId))
        Next

        If reslContainers IsNot Nothing AndAlso reslContainers.Count > 0 Then
            ddlContainers.Items.Add(New ListItem(String.Empty, String.Empty))
            For Each reslContainer As ListItem In reslContainers
                ddlContainers.Items.Add(reslContainer)
            Next
        End If

        chbOmissis.Text = String.Format("Inverti Oggetto/{0} per omissis", ResolutionEnv.ResolutionObjectPrivacyLabel)
        txtYear.Text = DateTime.Now.Year.ToString()
        ' Omissis
        pnlOmissis.Visible = ResolutionUtil.GroupOmissisTest

        pnlData.Visible = False
        pnlListaNumeroRegione.Visible = False

        InitializePrintType()

        rblTipologia.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeliberaCaption, ResolutionType.IdentifierDelibera.ToString()))
        rblTipologia.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeterminaCaption, ResolutionType.IdentifierDetermina.ToString()))
        rblTipologia.SelectedValue = ResolutionType.IdentifierDelibera.ToString()

        UpdateTipologia()

        If ResolutionEnv.SecurityPrint Then
            ' Solo contenitori con almeno diritto di sommario
            Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Resolution, ResolutionRightPositions.Adoption, True)

            reslContainers.Clear()
            For Each container As Container In containers
                reslContainers.Add(New ListItem(container.Name, container.Id.ToString()))
            Next
        End If

        If reslContainers IsNot Nothing AndAlso reslContainers.Count > 0 Then
            tvwContenitore.Nodes.Clear()
            Dim parentNode As New RadTreeNode
            parentNode.Text = "Contenitori"
            parentNode.Expanded = True
            parentNode.Checkable = False
            tvwContenitore.Nodes.Add(parentNode)
            For Each container As ListItem In reslContainers
                Dim childNode As New RadTreeNode
                childNode.Text = container.Text
                childNode.Value = container.Value
                childNode.ImageUrl = ImagePath.SmallBoxOpen
                childNode.Expanded = True
                tvwContenitore.Nodes(0).Nodes.Add(childNode)
            Next
        Else
            cmdStampa.Enabled = False
        End If

        'Setto il nodo root nei proponenti
        uscPropInterop.ContactRoot = ResolutionEnv.ProposerContact

        VisualizzaCampiRicercaLettera(CurrentTypeSelected)
    End Sub

    Private Sub InitializePrintType()
        If String.IsNullOrEmpty(PrintType) Then
            Throw New DocSuiteException("Print type Exception", "Parametro PrintType non valorizzato")
        End If

        Select Case PrintType
            Case "REGPUB"
                Title = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, "Stampa Registro delle pubblicazioni")
                pnlUPDigital.Visible = False
                pnlFDigital.Visible = False
            Case "F"
                Title = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, "Stampa Frontalino")
                pnlNumero.Visible = True
                rfvPrintDate_From.Visible = False
                rfvPrintDate_To.Visible = False
                pnlPrintLettere.Visible = False
                pnlHeading.Visible = True
                pnlUPDigital.Visible = False
                pnlFDigital.Visible = True
            Case "LT"
                Title = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, "Stampa Lettere di trasmissione")
                pnlPrint.Visible = False
                pnlPrintLettere.Visible = True
                pnlContenitoreTvw.Visible = False
                pnlHeading.Visible = False
                pnlPropInterop.Visible = False
                pnlUPDigital.Visible = False
                pnlFDigital.Visible = False
            Case "EDA"
                Title = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, "Stampa Elenco Provvedimenti Adottati")
                pnlUPDigital.Visible = False
                pnlFDigital.Visible = False
            Case "RG"
                Title = String.Format("{0} - {1}", Facade.TabMasterFacade.TreeViewCaption, "Stampa Registro Giornaliero")
                pnlNumero.Visible = False
                pnlPrintLettere.Visible = False
                pnlHeading.Visible = False
                pnlUPDigital.Visible = False
                pnlFDigital.Visible = False
        End Select
    End Sub

    Private Sub VisualizzaCampiRicercaLettera(ByVal selectedType As Short)
        If PrintType <> "LT" Then
            Exit Sub
        End If

        Dim typeLetterSelected As String = Nothing
        Select Case selectedType
            Case ResolutionType.IdentifierDelibera
                pnlTipoLettere.Visible = True
                pnlTipoLettereDet.Visible = False
                typeLetterSelected = rblTipoLettera.SelectedValue
            Case ResolutionType.IdentifierDetermina
                pnlTipoLettere.Visible = False
                pnlTipoLettereDet.Visible = True
                typeLetterSelected = rblTipoLetteraDet.SelectedValue
        End Select

        pnlAltraLettera.Visible = False
        pnlAdottata.Visible = False
        pnlAdottataIntervallo.Visible = False
        cmdAddNumberByAdottataIntervallo.Visible = False
        pnlNumeroRegione.Visible = False
        pnlNumeroRegioneMultiplo.Visible = False
        pnlPubblicata.Visible = False
        pnlEsecutivita.Visible = False
        pnlFirma.Visible = False
        pnlData.Visible = False
        pnlProtocollo.Visible = False
        pnlContenitore.Visible = True
        pnlStampaElenco.Visible = False
        pnlListaNumeroRegione.Visible = False

        Select Case typeLetterSelected
            Case "TAD"
                pnlAdottataIntervallo.Visible = True
                cboRuolo.SelectedValue = "R"
                txtFirma.Text = ""
                pnlFirma.Visible = True
                pnlData.Visible = True
            Case "TACS", "TAG"
                pnlAdottata.Visible = True

            Case "TAR-B", "TAR-A", "TAR-P"
                Select Case selectedType
                    Case ResolutionType.IdentifierDelibera
                        pnlAdottataIntervallo.Visible = True
                        cmdAddNumberByAdottataIntervallo.Visible = True
                        pnlNumeroRegioneMultiplo.Visible = True

                    Case ResolutionType.IdentifierDetermina
                        pnlAdottata.Visible = True
                        pnlNumeroRegione.Visible = True
                End Select

            Case "P"
                Select Case selectedType
                    Case ResolutionType.IdentifierDelibera
                        pnlPubblicata.Visible = True
                    Case ResolutionType.IdentifierDetermina
                        pnlContenitore.Visible = False
                        cboRuolo.SelectedValue = "R"
                        txtFirma.Text = ""
                        pnlFirma.Visible = True
                        lblProtocollo.Text = "Prot. Trasm. Adozione CS"
                        pnlProtocollo.Visible = True
                End Select
            Case "AL"
                pnlAltraLettera.Visible = True
                pnlAdottata.Visible = True
                pnlNumeroRegione.Visible = True
        End Select
    End Sub

    Private Sub VisualizzaCampiRegistroPubblicazioni()
        If PrintType <> "REGPUB" Then
            Exit Sub
        End If

        pnlPrint.Visible = False
        pnlNumero.Visible = False
        pnlHeading.Visible = False

        pnlPrintLettere.Visible = True

        pnlGestione.Visible = False
        pnlTipoLettere.Visible = False
        pnlTipoLettereDet.Visible = False
        pnlAltraLettera.Visible = False
        pnlFirma.Visible = False
        pnlData.Visible = False

        pnlAdottata.Visible = False
        pnlAdottataIntervallo.Visible = True

        'checkbox per stampa lettera, visibile solo per determine
        pnlStampaLettera.Visible = False
        pnlProtocollo.Visible = False
        pnlNumero.Visible = False
        pnlPropInterop.Visible = False
        pnlHeading.Visible = False
        pnlContenitoreTvw.Visible = True
        pnlContenitore.Visible = False
        pnlListaNumeroRegione.Visible = False
    End Sub

    Private Sub VisualizzaCampiRicercaStampaElenco(ByVal selectedType As Short)
        If PrintType <> "EDA" Then
            Exit Sub
        End If
        pnlPrint.Visible = False
        pnlPrintLettere.Visible = True

        pnlGestione.Visible = True
        pnlTipoLettere.Visible = False
        pnlTipoLettereDet.Visible = False
        pnlAltraLettera.Visible = False
        pnlFirma.Visible = False
        pnlData.Visible = False

        pnlAdottata.Visible = False
        If selectedType = ResolutionType.IdentifierDelibera Then
            pnlAdottata.Visible = True
        End If

        pnlAdottataIntervallo.Visible = False
        If selectedType = ResolutionType.IdentifierDetermina Then
            pnlAdottataIntervallo.Visible = True
        End If
        'checkbox per stampa lettera, visibile solo per determine
        pnlStampaLettera.Visible = False
        If selectedType = ResolutionType.IdentifierDetermina Then
            pnlStampaLettera.Visible = True
        End If

        pnlProtocollo.Visible = False
        pnlNumero.Visible = False
        pnlPropInterop.Visible = False
        pnlHeading.Visible = False
        pnlContenitoreTvw.Visible = True
        pnlContenitore.Visible = False
        pnlListaNumeroRegione.Visible = False
    End Sub

    Private Sub VisualizzaCampiRicercaStampaRegistro(ByVal selectedType As Short)
        If (PrintType <> "RG") Then
            Exit Sub
        End If
        pnlPropInterop.Visible = False
        If selectedType = ResolutionType.IdentifierDetermina Then
            pnlPropInterop.Visible = True
        End If
    End Sub

    Private Sub SelectOrDeselectAll(ByVal selected As Boolean)
        Dim tn As RadTreeNode
        For Each tn In tvwContenitore.Nodes(0).Nodes
            tn.Checked = selected
        Next
    End Sub

    Private Function CheckedNode(ByVal tvw As RadTreeView) As Boolean
        Dim tn As RadTreeNode
        For Each tn In tvw.Nodes(0).Nodes
            If tn.Checked = True Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub GeneraAdozioneIntervallo(ByRef s As String)
        If AdoptionDate_From.SelectedDate.HasValue And AdoptionDate_To.SelectedDate.HasValue Then
            s = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) >= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_From.SelectedDate) & "' AND " &
             "CONVERT(varchar(8),{alias}.AdoptionDate, 112) <= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_To.SelectedDate) & "'"
        Else
            If AdoptionDate_From.SelectedDate.HasValue Then
                s = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) >= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_From.SelectedDate) & "'"
            End If
            If AdoptionDate_To.SelectedDate.HasValue Then
                s = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) <= '" & String.Format("{0:yyyyMMdd}", AdoptionDate_To.SelectedDate) & "'"
            End If
        End If
    End Sub

    'TODO: Prevedere spostamento in Facade se possibile
    Private Function GetFinder(ByVal orderType As String, Optional ByRef sContName As String = "NoCalc") As NHibernateResolutionFinder
        ' Creo il finder
        Dim finder As New NHibernateResolutionFinder("ReslDB")
        finder.EagerLog = False

        ' Tipologia
        Select Case rblTipologia.SelectedValue
            Case ResolutionType.IdentifierDelibera
                finder.Delibera = True
                finder.Determina = False
            Case ResolutionType.IdentifierDetermina
                finder.Delibera = False
                finder.Determina = True
        End Select

        Dim sSql As String = String.Empty

        If pnlNumero.Visible Then
            If PrintDate_From.SelectedDate.HasValue Then
                sSql &= " AND CONVERT(varchar(8),{alias}.AdoptionDate, 112) >= '" & String.Format("{0:yyyyMMdd}", PrintDate_From.SelectedDate) & "'"
            End If
            If PrintDate_To.SelectedDate.HasValue Then
                sSql &= " AND CONVERT(varchar(8),{alias}.AdoptionDate, 112) <= '" & String.Format("{0:yyyyMMdd}", PrintDate_To.SelectedDate) & "'"
            End If
            If Not String.IsNullOrEmpty(txtYear.Text) Then
                finder.Year = txtYear.Text
            End If
            If Not String.IsNullOrEmpty(txtNumber_From.Text) Then
                sSql &= " AND {alias}.Number >= " & CInt(txtNumber_From.Text)
            End If
            If Not String.IsNullOrEmpty(txtNumber_To.Text) Then
                sSql &= " AND {alias}.Number <= " & CInt(txtNumber_To.Text)
            End If
        Else
            sSql &= " AND CONVERT(varchar(8),{alias}.AdoptionDate, 112) >= '" & String.Format("{0:yyyyMMdd}", PrintDate_From.SelectedDate) & "' " &
              "AND CONVERT(varchar(8),{alias}.AdoptionDate, 112) <= '" & String.Format("{0:yyyyMMdd}", PrintDate_To.SelectedDate) & "'"
        End If

        ' Aggancio la SqlExpression
        If sSql.StartsWith(" AND ") Then sSql = sSql.Substring(5)
        If Not String.IsNullOrEmpty(sSql.Trim) Then finder.SQLExpressions.Add("SqlExpression", New SQLExpression(sSql))

        ' Interop Proposers
        If pnlPropInterop.Visible Then
            Dim proposers As IList(Of ContactDTO) = Me.uscPropInterop.GetContacts(False)
            ' new multiselect
            If proposers IsNot Nothing AndAlso proposers.Count > 0 Then
                Dim s As String = String.Empty
                Dim i As Integer
                For i = 0 To proposers.Count - 1
                    If Not String.IsNullOrEmpty(s) Then s &= "§"
                    s &= proposers(i).Contact.FullIncrementalPath
                Next i
                finder.InteropProposers = s
            End If
        End If

        ' Contenitori
        Dim sID As String = ""
        Dim tn As RadTreeNode
        For Each tn In tvwContenitore.Nodes(0).Nodes
            If tn.Checked Then
                If sID <> "" Then sID &= ","
                sID &= tn.Value
                If sContName <> "NoCalc" Then
                    If sContName <> "" Then sContName &= ", "
                    sContName &= tn.Text
                End If
            End If
        Next tn
        If sID <> "" Then
            finder.ContainerIds = sID
        End If

        'Order by
        Select Case orderType
            Case ""
                finder.IsPrint = True
        End Select

        finder.EnablePaging = False
        finder.EnableStatus = False

        Return finder
    End Function

    Private Sub StampaRegistroPub(Optional ByVal printByContainer As Boolean = True)

        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")

        'Tipologia
        Select Case rblTipologia.SelectedValue
            Case ResolutionType.IdentifierDelibera
                finder.Delibera = True
                finder.Determina = False
            Case ResolutionType.IdentifierDetermina
                finder.Delibera = False
                finder.Determina = True
        End Select

        Dim sql As String = String.Empty
        GeneraAdozioneIntervallo(sql)

        finder.SQLExpressions.Add("AdoptionDate", New SQLExpression(sql))
        ' Solo gli atti realmente pubblicati
        finder.Pubblicata = True

        ' Contenitori
        Dim sContName As String = String.Empty
        Dim tn As RadTreeNode
        If printByContainer = True Then
            Dim sID As String = String.Empty
            For Each tn In tvwContenitore.Nodes(0).Nodes
                If tn.Checked Then
                    If sID <> "" Then sID &= ","
                    sID &= tn.Value

                    If sContName <> "" Then sContName &= ", "
                    sContName &= tn.Text

                End If
            Next tn
            finder.ContainerIds = sID
        End If

        'Gestione
        If chkGestione.Checked = True Then
            finder.OCManagement = True
        End If

        finder.EnablePaging = False
        finder.IsPrint = True
        finder.EnableStatus = False

        If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
            finder.NotNumber = 0 ' Escludo le resolution di test
        End If


        If finder.Delibera Then
            Dim elencoPrint As New ReslRegPub()
            elencoPrint.Omissis = chbOmissis.Checked
            ' elencoPrint.AdoptionDate = AdoptionDate.SelectedDate
            elencoPrint.Finder = finder
            elencoPrint.ContainersName = sContName
            CurrentPrinterSession = elencoPrint
            AjaxManager.ResponseScripts.Add("window.open('../Comm/CommPrint.aspx?Type=Resl&PrintName=RESL00001');")
        Else
            Dim elencoPrint As New ReslRegPub()
            elencoPrint.Omissis = chbOmissis.Checked
            ' elencoPrint.AdoptionDate = AdoptionDate.SelectedDate
            elencoPrint.Finder = finder
            elencoPrint.ContainersName = sContName
            CurrentPrinterSession = elencoPrint
            AjaxManager.ResponseScripts.Add("window.open('../Comm/CommPrint.aspx?Type=Resl&PrintName=RESL00002');")
        End If

    End Sub

    Private Sub StampaElenco(Optional ByVal filterByContainer As Boolean = True)

        Dim finder As NHibernateResolutionFinder = New NHibernateResolutionFinder("ReslDB")
        'Verifico se visualizzare solo le resolution attive
        If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalShowOnlyActive Then
            finder.IdStatus = 0
        End If

        'Tipologia
        Dim sql As String = String.Empty
        Select Case CurrentTypeSelected
            Case ResolutionType.IdentifierDelibera
                finder.Delibera = True
                finder.Determina = False
                sql = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) = '" & String.Format("{0:yyyyMMdd}", AdoptionDate.SelectedDate) & "'"
            Case ResolutionType.IdentifierDetermina
                finder.Delibera = False
                finder.Determina = True
                GeneraAdozioneIntervallo(sql)
        End Select

        finder.SQLExpressions.Add("AdoptionDate", New SQLExpression(sql))

        ' Contenitori
        Dim sContName As String = String.Empty
        Dim tn As RadTreeNode
        If filterByContainer = True Then
            Dim sID As String = String.Empty
            For Each tn In tvwContenitore.Nodes(0).Nodes
                If tn.Checked Then
                    If sID <> "" Then sID &= ","
                    sID &= tn.Value

                    If sContName <> "" Then sContName &= ", "
                    sContName &= tn.Text

                End If
            Next tn
            finder.ContainerIds = sID
        End If

        'Gestione
        If chkGestione.Checked = True Then
            finder.OCManagement = True
        End If

        finder.EnablePaging = False
        finder.IsPrint = True
        finder.EnableStatus = False

        If finder.Delibera Then
            Dim elencoPrint As New ReslElencoDelPrint()
            elencoPrint.Omissis = chbOmissis.Checked
            elencoPrint.AdoptionDate = AdoptionDate.SelectedDate
            elencoPrint.Finder = finder
            elencoPrint.ContainersName = sContName
            CurrentPrinterSession = elencoPrint
            AjaxManager.ResponseScripts.Add("window.open('../Comm/CommPrint.aspx?Type=Resl&PrintName=ReslElencoDelPrint');")
        Else
            Dim elencoPrint As New ReslElencoDetPrint()
            elencoPrint.Omissis = chbOmissis.Checked
            elencoPrint.AdoptionDate = AdoptionDate.SelectedDate
            elencoPrint.Finder = finder
            CurrentPrinterSession = elencoPrint
            AjaxManager.ResponseScripts.Add("window.open('../Comm/CommPrint.aspx?Type=Resl&PrintName=ReslElencoDetPrint');")
        End If

    End Sub

    Private Sub StampaLettera(ByVal printType As String, Optional ByVal typeLetter As String = "", Optional ByRef selezione As String = "")
        Dim finder As New NHibernateResolutionFinder("ReslDB")
        finder.EagerLog = False
        Dim sql As String = String.Empty
        Dim sProt As String = String.Empty

        If typeLetter = "" Then
            Select Case CurrentTypeSelected
                Case ResolutionType.IdentifierDelibera
                    typeLetter = rblTipoLettera.SelectedValue
                    finder.Delibera = True
                    finder.Determina = False
                Case ResolutionType.IdentifierDetermina
                    typeLetter = rblTipoLetteraDet.SelectedValue
                    finder.Delibera = False
                    finder.Determina = True
            End Select
        End If

        finder.ContainerIds = ddlContainers.SelectedValue

        Select Case typeLetter
            Case "TAD"
                GeneraAdozioneIntervallo(sql)
            Case "TACS", "TAG"
                sql = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) = '" & String.Format("{0:yyyyMMdd}", AdoptionDate.SelectedDate) & "'"
            Case "TAR-B", "TAR-A", "TAR-P", "AL"
                sql = "CONVERT(varchar(8),{alias}.AdoptionDate, 112) = '" & String.Format("{0:yyyyMMdd}", AdoptionDate.SelectedDate) & "'"
                finder.OCRegion = True
                Dim number As Integer
                If Integer.TryParse(txtNumber.Text, number) Then
                    finder.Number = number
                End If
            Case "P"
                Select Case CurrentTypeSelected
                    Case ResolutionType.IdentifierDelibera
                        sql = "CONVERT(varchar(8),{alias}.PublishingDate, 112) = '" & String.Format("{0:yyyyMMdd}", PublishingDate.SelectedDate) & "'"
                    Case ResolutionType.IdentifierDetermina
                        finder.SupervisoryBoardProtocolLink = uscProtocollo.GetFirstProtocolLink()
                        If Not String.IsNullOrEmpty(finder.SupervisoryBoardProtocolLink) Then
                            sProt = finder.SupervisoryBoardProtocolLink
                        End If
                End Select
        End Select

        'Se la stringa dell'espressione SQL non è vuota
        If Not String.IsNullOrEmpty(sql) Then
            finder.SQLExpressions.Add("SqlExpression", New SQLExpression(sql))
        End If

        finder.EnablePaging = False
        finder.EnableStatus = False

        Dim resolutions As IList(Of Resolution) = New List(Of Resolution)()
        Select Case typeLetter
            Case "TAR-B", "TAR-A", "TAR-P"
                Select Case CurrentTypeSelected
                    Case ResolutionType.IdentifierDelibera
                        Dim selectedResolutions As IList(Of Integer) = New List(Of Integer)
                        For Each i As ListItem In cblNumbers.Items
                            If i.Selected Then selectedResolutions.Add(i.Value)
                        Next
                        resolutions = Facade.ResolutionFacade.GetResolutions(selectedResolutions)
                    Case ResolutionType.IdentifierDetermina
                End Select
            Case Else
                resolutions = finder.DoSearch()
        End Select
        Dim keylist As IList(Of Integer) = New List(Of Integer)

        For Each resl As Resolution In resolutions
            If selezione <> "" Then
                selezione &= ","
            End If
            selezione &= resl.Id
            keylist.Add(resl.Id)
        Next

        If selezione = "" Then
            AjaxAlert("Nessuna " & rblTipologia.SelectedItem.Text & " trovata.")
            Exit Sub
        End If

        Dim FileTemp As String = String.Empty
        Dim FileSource As String = String.Empty
        If pnlAltraLettera.Visible Then
            FileSource = Me.uscDocumento.GetFirstTemplate()
        End If

        'campi data
        Dim sData As String = String.Empty
        If txtData.SelectedDate.HasValue Then
            sData = txtData.SelectedDate
        End If
        Dim sAdoptionDateTo As String = String.Empty
        If AdoptionDate_To.SelectedDate.HasValue Then
            sAdoptionDateTo = AdoptionDate_To.SelectedDate
        End If

        Facade.ResolutionFacade.GeneraLetteraToString(CShort(rblTipologia.SelectedValue), keylist, typeLetter, FileTemp, sProt, FileSource, sData, cboRuolo.SelectedItem.Text, txtFirma.Text, sAdoptionDateTo)

        ' Redirect
        Dim script As String = "window.open('" & DocSuiteContext.Current.CurrentTenant.DSWUrl & "/Temp/" & FileTemp & "');"
        AjaxManager.ResponseScripts.Add(script)
    End Sub

    Private Function StampaFrontalino(ByVal resolutions As IList(Of Resolution)) As String
        Dim archive As Boolean = radioFrontalino.SelectedValue = "1"
        Dim files As List(Of ResolutionFrontispiece) = New List(Of ResolutionFrontispiece)
        Dim toProcessFiles As ICollection(Of ResolutionFrontispiece)
        For Each resolution As Resolution In resolutions
            toProcessFiles = New List(Of ResolutionFrontispiece)
            ' Il printer deve essere inizializzato ogni volta per evitare che riaccodi pagine su pagine
            Dim printer As New ReslFrontalinoPrintPdfTO()
            ' Genero i frontalini            
            Try
                toProcessFiles = printer.GeneraFrontalini(resolution)
                files.AddRange(toProcessFiles)
            Catch ex As Exception
                FileLogger.Warn(LoggerName, String.Format("Errore nella generazione dei file di frontespizio da Stampa Frontalino: {0}", ex.Message), ex)
                Facade.ResolutionLogFacade.Insert(CurrentResolution, ResolutionLogType.RF, "Errore nella generazione dei file di frontespizio da Stampa Frontalino.")
                AjaxAlert(String.Format("Errore nella generazione dei file di frontespizio: {0}", ex.Message))
                Exit Function
            End Try

            If archive Then
                ' Verifico la posizione in cui devono essere salvati
                Dim blr As New BusinessLogicResolution(resolution)
                ''Aggiunge al documento principale solo se questo esiste.

                If blr.HasDocument() Then
                    blr.AddFrontalino(toProcessFiles)
                End If

                'In nessuna sezione del codice di gestione dei frontalini
                'digitali viene salvato il frontalino negli allegati, come mai in Stampe si?!?
                ''Aggiunge agli allegati solo se questi esistono.
                'If blr.HasAttachments() Then
                '    blr.AddFrontalino(files, True)
                'End If

                ''EF 20120315 Aggiorna sempre e comunque anche idFrontespizio
                blr.ParcheggiaFrontalino(toProcessFiles)
            End If

            If archive Then
                Facade.ResolutionLogFacade.Log(resolution, ResolutionLogType.RP, "Frontalino predisposto per gestione digitale")
            Else
                Facade.ResolutionLogFacade.Log(resolution, ResolutionLogType.RP, "Frontalino creato per stampa")
            End If
        Next

        Dim fileTemp As String = String.Empty
        If archive Then
            AjaxAlert(String.Format("Creazione di {0} document{1} di 'Frontalino' avvenuta con successo.", files.Count, If(files.Count = 1, "o", "i")))
        Else
            ' Visualizzo documenti a video
            fileTemp = String.Format("{0}-PrintF-{1:HHmmss}_f_all.pdf", CommonUtil.UserDocumentName, Now())
            Dim myDestination As String = CommonInstance.AppTempPath & fileTemp
            Dim managerPdf As New PdfMerge()
            For Each doc As ResolutionFrontispiece In files
                managerPdf.AddDocument(doc.Path)
            Next
            managerPdf.Merge(myDestination)
        End If
        Return fileTemp
    End Function

    Private Function StampaUltimaPagina(ByVal resolutions As IList(Of Resolution)) As String
        Dim fileTemp As String = String.Empty
        ' nuova gestione con report viewer
        Dim _allPdfs As New List(Of String)
        For Each resl As Resolution In resolutions
            Dim ultimaPaginaPrintPdf As New ReslUltimaPaginaPrintPdf(chbOmissis.Checked)
            Dim path As String = ultimaPaginaPrintPdf.GeneraUltimaPagina(resl, False)
            _allPdfs.Add(path)
            ' LOG
            Facade.ResolutionLogFacade.Log(resl, ResolutionLogType.RP, "Ultima pagina creata per stampa")
        Next

        If chkDigitalUP.Checked Then
            AjaxAlert(String.Format("Creazione di {0} document{1} di 'Ultima Pagina' avvenuta con successo.", _allPdfs.Count, If(_allPdfs.Count = 1, "o", "i")))
        Else
            ' Visualizzo documenti a video
            fileTemp = String.Format("{0}-PrintUP-{1:HHmmss}_up_all.pdf", CommonUtil.UserDocumentName, Now())
            Dim myDestination As String = CommonInstance.AppTempPath & fileTemp
            Dim managerPDF As PdfMerge = New PdfMerge()
            For Each doc As String In _allPdfs
                managerPDF.AddDocument(doc)
            Next
            managerPDF.Merge(myDestination)
        End If
        Return fileTemp
    End Function

    Private Sub StampaModelloPredefinito()
        Dim gestioneDigitale As Boolean = False
        Select Case Short.Parse(rblTipologia.SelectedValue)
            Case ResolutionType.IdentifierDelibera
                gestioneDigitale = ResolutionEnv.GestioneDigitaleDelibere
            Case ResolutionType.IdentifierDetermina
                gestioneDigitale = ResolutionEnv.GestioneDigitaleDetermine
        End Select
        Dim resolutions As IList(Of Resolution) = GetFinder(String.Empty).DoSearch()

        If (resolutions.Count > 0) Then
            Dim fileTemp As String = String.Empty
            If (gestioneDigitale) Then
                If PrintType = "F" Then
                    fileTemp = StampaFrontalino(resolutions)
                End If
            Else
                'TODO: Torino ormai usa solamente la generazione dei frontalini in PDF tramite file rdlc.
                'Verificare con il cliente se è possibile eliminare la sezione del frontalino per HTML.
                Dim IsFirst As Boolean = True
                fileTemp = CommonUtil.UserDocumentName & "-Print-" & String.Format("{0:HHmmss}", Now()) & ".htm"
                Dim fSource As String = CommonInstance.AppPath & ResolutionFacade.PathStampeTo
                Select Case PrintType
                    Case "F"
                        fSource &= "Frontalino.htm"
                End Select

                Dim fDestination As String = CommonInstance.AppTempPath & fileTemp
                Try
                    FileHelper.CopySafe(CommonInstance.AppPath & ResolutionFacade.PathStampeTo & "logo.jpg", CommonInstance.AppTempPath & "logo.jpg", True)
                    FileHelper.CopySafe(CommonInstance.AppPath & ResolutionFacade.PathStampeTo & "logo2.jpg", CommonInstance.AppTempPath & "logo2.jpg", True)
                Catch ex As Exception
                    FileLogger.Warn(LoggerName, ex.Message, ex)
                End Try
                'read
                Dim fText As String
                Using sr As StreamReader = New StreamReader(fSource)
                    fText = sr.ReadToEnd()
                    sr.Close()
                End Using

                'For write
                Using sw As StreamWriter = New StreamWriter(fDestination)
                    For Each resl As Resolution In resolutions
                        ResolutionUtil.GeneraStampaConSostituzioni(resl, fText, sw, IsFirst, txtHeadingFrontalino.Text, chbOmissis.Checked)
                        IsFirst = False
                    Next
                    sw.Close()
                End Using
            End If


            ' Redirect
            If fileTemp.Length > 0 Then
                If gestioneDigitale Then
                    Response.Redirect(String.Concat("~/Viewers/TempFileViewer.aspx?DownloadFile=", fileTemp, "&Label=Frontalino", Path.GetExtension(fileTemp)))
                Else
                    Response.Redirect(CommonInstance.AppTempHttp & fileTemp)
                End If
            End If
        Else
            AjaxAlert("Nessuna " & rblTipologia.SelectedItem.Text & " trovata.")
        End If
    End Sub

    Private Sub StampaRegistroDel()
        Dim sContName As String = ""

        Dim journalPrint As New ReslJournalDelPrint()
        journalPrint.Finder = GetFinder("", sContName)
        'Verifico se visualizzare solo le resolution attive
        If (DocSuiteContext.Current.ResolutionEnv.ResolutionJournalShowOnlyActive) Then journalPrint.Finder.IdStatus = 0
        journalPrint.Tipologia = CShort(rblTipologia.SelectedValue)
        journalPrint.Omissis = chbOmissis.Checked
        journalPrint.ContainersName = sContName
        journalPrint.TitlePrint = ""
        CurrentPrinterSession = journalPrint

        'redirect
        Response.Redirect("../Comm/CommPrint.aspx?Type=Resl&PrintName=ReslJournalDelPrint")
    End Sub

    Private Sub StampaRegistroDetermine()
        Dim sContName As String = ""

        Dim journalPrint As New ReslJournalDetPrint()
        journalPrint.Finder = GetFinder("", sContName)
        'Verifico se visualizzare solo le resolution attive
        If (DocSuiteContext.Current.ResolutionEnv.ResolutionJournalShowOnlyActive) Then journalPrint.Finder.IdStatus = 0
        journalPrint.Tipologia = CShort(rblTipologia.SelectedValue)
        journalPrint.Omissis = chbOmissis.Checked
        journalPrint.ContainersName = sContName
        journalPrint.TitlePrint = ""
        CurrentPrinterSession = journalPrint

        'redirect
        Response.Redirect("../Comm/CommPrint.aspx?Type=Resl&PrintName=ReslJournalDetPrint")
    End Sub

    Private Sub BindRuolo()
        cboRuolo.Items.Clear()

        Dim codes As IDictionary(Of String, String) = Facade.PrintLetterRoleFacade.GetCodesDictionary()
        If Not codes.Any() Then
            Exit Sub
        End If

        For Each code As KeyValuePair(Of String, String) In codes.OrderBy(Function(o) o.Value)
            cboRuolo.Items.Add(New ListItem(code.Value, code.Key))
        Next
        cboRuolo.DataBind()
    End Sub

    Private Sub UpdateLettera()
        If rblTipologia.SelectedValue = ResolutionType.IdentifierDetermina Then
            pnlFirma.Visible = chkLettera.Checked
            cboRuolo.SelectedValue = "D"
            txtFirma.Text = "(Dott. Gerardo GATTO)"
        Else
            pnlFirma.Visible = False
        End If
    End Sub

    Private Sub UpdateTipologia()
        VisualizzaCampiRicercaLettera(CurrentTypeSelected)
        VisualizzaCampiRicercaStampaElenco(CurrentTypeSelected)
        VisualizzaCampiRegistroPubblicazioni()
        If PrintType = "EDA" Then
            UpdateLettera()
        End If
        VisualizzaCampiRicercaStampaRegistro(CurrentTypeSelected)
    End Sub
#End Region

End Class