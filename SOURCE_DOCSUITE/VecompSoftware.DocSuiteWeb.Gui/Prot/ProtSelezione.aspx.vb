Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ProtSelezione
    Inherits ProtBasePage

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        Initialize()

        If Not IsPostBack Then
            ' Inizializzo la combo dei contenitori con quelli in cui ho diritto di inserimento
            If ProtocolEnv.SuspendFilterMaskEnabled Then
                pnFilter.Visible = True
                Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)
                ddlContainer.Items.Add(New ListItem("", "")) ' Elemento vuoto
                For Each container As Container In containers
                    If container.IsActive = 1 AndAlso container.IsActiveRange() Then
                        ddlContainer.Items.Add(New ListItem(container.Name, container.Id.ToString()))
                    End If
                Next
            Else
                pnFilter.Visible = False
            End If

            ' Visualizzo l'elenco completo solo se non c'è la maschera di filtraggio
            If Not ProtocolEnv.SuspendFilterMaskEnabled Then
                BindGrid()
            End If
        End If
    End Sub

    Private Sub gvProtocols_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles gvProtocols.ItemCommand
        Select Case e.CommandName
            Case "View"
                Dim hiddenId As HiddenField = e.Item.FindControl("hdId")
                Dim splitted As String() = hiddenId.Value.Split("|"c)
                Dim selectedKey As New YearNumberCompositeKey(splitted(0), splitted(1))

                ViewDocument(selectedKey)

            Case "Sort"

            Case Else
                ' Recupera il protocollo selezionato.
                Dim currentArguments As String() = e.CommandArgument.ToString.Split("|"c)
                Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(currentArguments.GetValue(0), currentArguments.GetValue(1))

                If Not currentProtocol Is Nothing Then
                    'Tolto in quanto la logica di recupero può prevedere di non completare la fase di recupero di protocollo. Quindi permettere la possibilità di completarlo successivamente
                    'currentProtocol.IdStatus = ProtocolStatusId.Errato
                    'Facade.ProtocolFacade.Update(currentProtocol)
                End If
                Dim currentJs As String = String.Format("CloseWindow('{0}|{1}');", currentProtocol.Year.ToString, currentProtocol.Number.ToString)
                MasterDocSuite.AjaxManager.ResponseScripts.Add(currentJs)
        End Select
    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        If String.IsNullOrWhiteSpace(txtAnnulla.Text) Then
            AjaxAlert("E' necessario specificare gli estremi di annullamento.")
            Exit Sub
        End If

        Dim selectedProtocols As IList(Of Protocol) = Facade.ProtocolFacade.GetProtocols(GetSelectedProtocolKeys)
        If selectedProtocols.IsNullOrEmpty() Then
            AjaxAlert("E' necessario selezionare almeno un protocollo.")
            Exit Sub
        End If

        For Each p As Protocol In selectedProtocols
            p.IdStatus = ProtocolStatusId.Annullato
            p.LastChangedReason = txtAnnulla.Text.Trim
            Facade.ProtocolFacade.Update(p)

            If DocSuiteContext.Current.ProtocolEnv.IsLogEnabled Then
                Facade.ProtocolLogFacade.Insert(p, ProtocolLogEvent.PA, p.LastChangedReason)
            End If
        Next
        gvProtocols.DataBindFinder()
        txtAnnulla.Text = ""
        AjaxAlert("Protocolli annullati correttamente.")
    End Sub

    Private Sub btnPrintZebraLabel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrintDocumentLabel.Click, btnPrintAttachmentLabel.Click
        CommonShared.ZebraPrintData = Nothing
        Dim selectedProtocolKeys As IList(Of YearNumberCompositeKey) = GetSelectedProtocolKeys()
        If selectedProtocolKeys.Count <= 0 Then
            MasterDocSuite.AjaxManager.Alert("E' necessario selezionare almeno un protocollo.")
            Exit Sub
        End If

        CommonShared.ZebraPrintData = selectedProtocolKeys
        Dim qsParameters As String = String.Format("ChainType={0}", DirectCast(sender, Button).CommandArgument)
        Dim jsOpenWindow As String = String.Format("{0}_OpenWindow('../Prot/ProtZebraLabel.aspx','windowPrintLabel', 350, 150, '{1}');", Me.ID, CommonShared.AppendSecurityCheck(qsParameters))
        MasterDocSuite.AjaxManager.ResponseScripts.Add(String.Format("setTimeout(""{0}"", 600);", jsOpenWindow))
    End Sub

    Private Sub cmdFilter_Click(sender As Object, e As EventArgs) Handles cmdFilter.Click
        ' Imposto i filtri ed aggiorno le griglia
        BindGrid()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()

        With AjaxManager.AjaxSettings
            .AddAjaxSetting(gvProtocols, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
            .AddAjaxSetting(btnCancel, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
            .AddAjaxSetting(cmdFilter, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
            .AddAjaxSetting(btnCancel, txtAnnulla, MasterDocSuite.AjaxFlatLoadingPanel)
            If ProtocolEnv.ZebraPrinterEnabled AndAlso Facade.ComputerLogFacade.GetCurrent.ZebraPrinter IsNot Nothing Then
                .AddAjaxSetting(btnPrintDocumentLabel, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
                .AddAjaxSetting(btnPrintAttachmentLabel, gvProtocols, MasterDocSuite.AjaxDefaultLoadingPanel)
            End If
        End With
    End Sub

    Private Sub Initialize()
        Select Case Action
            Case "View"
                ' Visualizzazione elenco protocollo in riserva
                ' Visualizzo check-box, numero (senza link)
                Title = "Protocollo - Elenco Numeri di Protocollo per Recupero"
                gvProtocols.Columns.FindByUniqueName("IdLink").Visible = False
                gvProtocols.Columns.FindByUniqueName("Id").Visible = True
                gvProtocols.Columns.FindByUniqueName("ClientSelectColumn").Visible = True
                pnAnnulla.Visible = True

            Case "Recovery"
                Title = "Protocollo - Selezione"
                ' Visualizzo check-box, numero (con link)
                gvProtocols.Columns.FindByUniqueName("IdLink").Visible = True
                gvProtocols.Columns.FindByUniqueName("Id").Visible = False
                gvProtocols.Columns.FindByUniqueName("ClientSelectColumn").Visible = False
                pnAnnulla.Visible = False

        End Select
        ' Visualizzo la colonna documento sempre solo se attiva la pre-protocollazione.
        If ProtocolEnv.PreProtocollazioneEnabled Then
            gvProtocols.Columns.FindByUniqueName("Document").Visible = True
        End If
        ' Visualizzo il pulsante di stampa etichette solo se è configurata una stampante Zebra.
        btnPrintDocumentLabel.CommandArgument = ProtZebraLabel.ChainType.Document.ToString()
        btnPrintDocumentLabel.Visible = ProtocolEnv.ZebraPrinterEnabled AndAlso Facade.ComputerLogFacade.GetCurrent.ZebraPrinter IsNot Nothing
        btnPrintAttachmentLabel.CommandArgument = ProtZebraLabel.ChainType.Attachment.ToString()
        btnPrintAttachmentLabel.Visible = ProtocolEnv.ZebraPrinterEnabled AndAlso Facade.ComputerLogFacade.GetCurrent.ZebraPrinter IsNot Nothing
    End Sub

    Public Function HasDocument(ByVal year As Short, ByVal number As Integer) As Boolean
        Dim prot As Protocol = Facade.ProtocolFacade.GetById(year, number)
        Return prot.IdDocument.HasValue AndAlso prot.IdDocument.Value > 0
    End Function

    ''' <summary> Popola la griglia. </summary>
    Private Sub BindGrid()
        Dim finder As New NHibernateProtocolFinder("ProtDB")
        With finder
            .IdStatus = ProtocolStatusId.Sospeso
            .EnablePaging = False
            .EnableFetchMode = False
            ' Contenitore
            If ddlContainer.SelectedIndex > -1 AndAlso Not String.IsNullOrEmpty(ddlContainer.SelectedValue) Then
                .IdContainer = ddlContainer.SelectedValue
            End If
            ' Data protocollo
            If dpFrom.SelectedDate.HasValue Then
                .RegistrationDateFrom = dpFrom.SelectedDate.Value
            End If
            If dpTo.SelectedDate.HasValue Then
                .RegistrationDateTo = dpTo.SelectedDate.Value
            End If
            ' Numero
            If Not String.IsNullOrEmpty(txtNumber.Text) Then
                .Number = txtNumber.Text
            End If

        End With

        gvProtocols.Finder = finder
        gvProtocols.DataBindFinder()
    End Sub

    ''' <summary> Ritorna l'elenco delle chiavi selezionate. </summary>
    Private Function GetSelectedProtocolKeys() As IList(Of YearNumberCompositeKey)
        Dim retval As IList(Of YearNumberCompositeKey)

        Dim currentCheckBox As CheckBox
        Dim currentHiddenField As HiddenField
        Dim keyToAdd As YearNumberCompositeKey

        retval = New List(Of YearNumberCompositeKey)
        For Each i As Telerik.Web.UI.GridDataItem In gvProtocols.Items
            currentCheckBox = i.FindControl("cbSelect")
            currentHiddenField = i.FindControl("hdId")

            If currentCheckBox.Checked Then
                keyToAdd = New YearNumberCompositeKey
                keyToAdd.Year = CShort(currentHiddenField.Value.Split("|"c).GetValue(0))
                keyToAdd.Number = CInt(currentHiddenField.Value.Split("|"c).GetValue(1))
                retval.Add(keyToAdd)
            End If
        Next

        Return retval
    End Function

    Private Sub ViewDocument(ynck As YearNumberCompositeKey)
        Dim p As Protocol = Facade.ProtocolFacade.GetById(ynck)
        If p IsNot Nothing Then
            Dim parameters As String = "Year={0}&Number={1}&Type={2}"
            parameters = String.Format(parameters, p.Year, p.Number, "Prot")
            Dim viewerUrl As String = "~/Viewers/ProtocolViewer.aspx?" & CommonShared.AppendSecurityCheck(parameters)
            Response.Redirect(viewerUrl)
        End If
    End Sub

#End Region

End Class
