Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Web.Services

Partial Public Class uscProtGridBar
    Inherits BaseGridBar

#Region " Fields "

#End Region

#Region " Properties "

    Public Overrides ReadOnly Property DocumentsButton() As RadButton
        Get
            Return btnDocuments
        End Get
    End Property

    Public ReadOnly Property ExportButton() As RadButton
        Get
            Return btnExport
        End Get
    End Property

    Public Overrides ReadOnly Property DeselectButton() As RadButton
        Get
            Return btnDeselectAll
        End Get
    End Property

    Public Overrides ReadOnly Property PrintButton() As RadButton
        Get
            Return btnStampa
        End Get
    End Property

    Public Overrides ReadOnly Property SelectButton() As RadButton
        Get
            Return btnSelectAll
        End Get
    End Property

    Public ReadOnly Property SetAssignButton() As RadButton
        Get
            Return btnAssign
        End Get
    End Property

    Public Overrides ReadOnly Property SetReadButton() As RadButton
        Get
            Return btnSetRead
        End Get
    End Property

    Public ReadOnly Property ChangeContainerButton() As RadButton
        Get
            Return btnChangeContainer
        End Get
    End Property

    Public Overrides ReadOnly Property LeftPanel() As Panel
        Get
            Return New Panel()
        End Get
    End Property

    Public Overrides ReadOnly Property MiddlePanel() As Panel
        Get
            Return New Panel()
        End Get
    End Property

    Public Overrides ReadOnly Property RightPanel() As Panel
        Get
            Return New Panel()
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
        If ChangeContainerButton.Visible Then
            InitializeChangeContainerDialog()
        End If
    End Sub

    Protected Overrides Sub SelectOrDeselectAll(ByVal Selected As Boolean)
        Dim count As Int32 = 0
        Dim maxCounter As Int32 = ProtocolEnv.SelectableProtocolThreshold

        ' Verifico le righe già selezionate, solo se sono in fase di selezione di altre righe.
        If Selected.Equals(True) Then
            For Each item As GridDataItem In _grid.Items
                Dim cb As CheckBox = item.FindControl("cbSelect")
                If cb.Enabled AndAlso (cb.Checked.Equals(Selected) AndAlso cb.Visible) Then
                    count = count + 1
                End If
            Next
        End If

        ' Applico la selezione o la deselezione
        For Each item As GridDataItem In _grid.Items
            Dim cb As CheckBox = item.FindControl("cbSelect")
            ' Seleziono solo quelli visibili
            If cb.Enabled AndAlso (Not cb.Checked.Equals(Selected) OrElse cb.Visible) Then
                ' per la deselezione non ci sono vincoli. Deseleziono tutti gli elementi della griglia.
                If Selected.Equals(True) AndAlso count >= maxCounter Then
                    BasePage.AjaxAlert("Non è possibile selezionare più di {0} protocolli", maxCounter)
                    Return
                End If
                cb.Checked = Selected
                count = count + 1
            End If
        Next
    End Sub

    Private Sub btnAssign_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAssign.Click
        Dim uniqueId As Guid?
        Dim allKeys As List(Of Guid) = New List(Of Guid)()
        Dim protocol As Protocol
        Dim protocolRights As ProtocolRights = Nothing

        For Each item As GridDataItem In _grid.Items.Cast(Of GridDataItem)().Where(Function(i) GetChecked(i))
            uniqueId = GetProtocolUniqueId(item)
            If uniqueId.HasValue Then
                protocol = FacadeFactory.Instance.ProtocolFacade.GetById(uniqueId.Value)
                protocolRights = New ProtocolRights(protocol)
                If protocolRights.IsEditable OrElse protocolRights.IsEditableAttachment.GetValueOrDefault(False) Then
                    allKeys.Add(uniqueId.Value)
                End If
            End If
        Next

        Dim serialized As String = JsonConvert.SerializeObject(allKeys)
        Dim encoded As String = HttpUtility.UrlEncode(serialized)
        Dim redirectUrl As String = "~/Prot/ProtAssegna.aspx?multiple=true&selectedKeys={0}&keys={1}"
        redirectUrl = String.Format(redirectUrl, encoded, encoded)
        Response.Redirect(redirectUrl)
    End Sub

    Private Sub BtnDocumentsClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim selection As New List(Of Guid)
        Dim currentProtocol As Protocol
        Dim statusCancel As Boolean
        Dim autorizza As Boolean
        ' Registro il log di visualizzazione dei documenti
        For Each uniqueIdProtocol As Guid In GetSelectedItems()
            currentProtocol = Facade.ProtocolFacade.GetById(uniqueIdProtocol)
            statusCancel = currentProtocol.IdStatus.GetValueOrDefault(ProtocolStatusId.Attivo) = ProtocolStatusId.Annullato
            autorizza = New ProtocolRights(currentProtocol, statusCancel).IsDocumentReadable

            If autorizza Then
                Facade.ProtocolLogFacade.Insert(currentProtocol.Year, currentProtocol.Number, "PD", "Visualizzazione documento da MultiCatena", DocSuiteContext.Current.User.FullUserName)
                selection.Add(currentProtocol.Id)
            End If
        Next

        If selection.IsNullOrEmpty() Then
            BasePage.AjaxAlert("Nessun protocollo selezionato", False)
            Return
        End If
        Dim serialized As String = JsonConvert.SerializeObject(selection)
        Dim encoded As String = HttpUtility.UrlEncode(serialized)
        Dim redirectUrl As String = "~/viewers/ProtocolViewer.aspx?multiple=true&keys={0}"
        redirectUrl = String.Format(redirectUrl, encoded)
        Response.Redirect(redirectUrl)
    End Sub

    Private Sub btnExport_Click(ByVal sender As Object, ByVal e As EventArgs)

        Dim listID As IList(Of Guid) = CType(GetSelectedItems(), List(Of Guid))

        Session("ExportStartDate") = DateTime.Now
        Dim resultsErrorString As String = CommExport.InitializeExportTask(listID)

        If Not String.IsNullOrEmpty(resultsErrorString) Then
            BasePage.AjaxAlert(resultsErrorString, False)
            Return
        End If

        SetupPageWithTaskRunning()

    End Sub

    Private Sub AjaxRequest_TaskCompleted(ByVal sender As Object, ByVal e As Telerik.Web.UI.AjaxRequestEventArgs)
        If e.Argument.Eq("E") Then
            WindowBuilder.LoadWindow("windowExportError", String.Format("../Prot/ProtExportResult.aspx?Data={0}&Module=BiblosDSExtract", Session("ExportStartDate")))
            Session.Remove("ExportStartDate")
            'Return New List(Of Integer) From {1, 2, 3}
        ElseIf e.Argument.StartsWith("updateProtocolContainer") Then

            Dim protocolIds As IList(Of Guid) = CType(GetSelectedItems(), IList(Of Guid))

            If protocolIds.Count = 0 Then
                AjaxManager.Alert("No protocols selected")
                Return
            End If

            Dim containerIdStr As String = e.Argument.Split(","c).Last()

            Dim selectedContainerId As Integer = -1
            Integer.TryParse(containerIdStr, selectedContainerId)

            If selectedContainerId = -1 Then
                'this will not likely happen
                AjaxManager.Alert("Seleziona un contenitore valido")
                Return
            End If

            Dim targetContainer As Container = Facade.ContainerFacade.GetById(selectedContainerId)

            If targetContainer Is Nothing Then
                Throw New Exception($"Could not find container with id {selectedContainerId}")
            End If

            For Each protocolId As Guid In protocolIds
                Dim protocol As Protocol = Facade.ProtocolFacade.GetById(protocolId)

                Dim protocolRights As ProtocolRights = New ProtocolRights(protocol)

                If protocol.Container IsNot Nothing AndAlso protocol.Container.Id = selectedContainerId Then
                    'protocol already has this container selected
                    Continue For
                End If

                If protocolRights.IsEditable Then
                    Dim newContainerMsg As String = $"{targetContainer.Id} ({targetContainer.Name})"
                    Dim oldContainerMsg As String = String.Empty
                    If protocol.Container IsNot Nothing Then
                        oldContainerMsg = String.Format("{0} ({1})", protocol.Container.Id, protocol.Container.Name)
                    End If

                    protocol.Container = targetContainer

                    Facade.ProtocolFacade.Update(protocol)
                    Facade.ProtocolFacade.SendUpdateProtocolCommand(protocol)
                    CreateFieldChangeLog(protocol, "Contenitore", oldContainerMsg, newContainerMsg)
                End If

            Next

            'always rebind to refresh checkbox events
            _grid.Rebind()
        End If
    End Sub


#End Region

#Region " Methods "

    Private Sub InitializeChangeContainerDialog()

        Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, True)

        If availableContainers.Count = 0 Then
            'if there are no containers the button remains disabled and no checkbox events are added to enable it
            Return
        End If

        AjaxManager.ResponseScripts.Add("ccAddCheckboxEvents();")

        For Each avCont As Container In availableContainers
            ddlContainers.Items.Add(New RadComboBoxItem(avCont.Name, avCont.Id.ToString()))
        Next

        btnChangeContainer.OnClientClicked = "ccShowContainerChangeDialog"

        'preinitialize value of hidden field
        hfSelectedContainer.Value = availableContainers(0).Id.ToString()
    End Sub

    Private Sub CreateFieldChangeLog(ByRef protocol As Protocol, ByVal message As String, ByVal oldValue As String, ByVal newValue As String)
        If oldValue.Eq(newValue) Then
            Exit Sub
        End If

        Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, message & " (old): " & oldValue)
        Facade.ProtocolLogFacade.Insert(protocol, ProtocolLogEvent.PM, message & " (new): " & newValue)
    End Sub

    Protected Overrides Sub InitializeAjaxSettings()
        MyBase.InitializeAjaxSettings()
        If AjaxEnabled Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnDocuments, Grid)
        End If

        WindowBuilder.RegisterWindowManager(RadWindowManager)
        WindowBuilder.RegisterOpenerElement(btnExport)
        WindowBuilder.RegisterOpenerElement(AjaxManager)

    End Sub

    Protected Overrides Sub AttachEvents()
        MyBase.AttachEvents()
        AddHandler btnDocuments.Click, AddressOf BtnDocumentsClick
        AddHandler btnExport.Click, AddressOf btnExport_Click
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequest_TaskCompleted
    End Sub

    Protected Overrides Sub ConfigureSetReadProperties()
        LogType = "P1"
        LogDescription = "Marcato come già letto"
        SetReadFunction = New SetReadDelegate(AddressOf Facade.ProtocolLogFacade.Insert)
    End Sub

    Private Function GetChecked(item As GridDataItem) As Boolean
        Dim chk As CheckBox = CType(item("colClientSelect").Controls(1), CheckBox)
        Return chk IsNot Nothing AndAlso chk.Checked
    End Function

    Private Function GetProtocolUniqueId(item As GridDataItem) As Guid?
        Dim hf_protocol_unique As HiddenField = CType(item.FindControl("hf_protocol_unique"), HiddenField)
        Dim uniqueId As Guid = Guid.Empty
        If hf_protocol_unique Is Nothing OrElse Not Guid.TryParse(hf_protocol_unique.Value, uniqueId) OrElse uniqueId = Guid.Empty Then
            Return Nothing
        End If
        Return uniqueId
    End Function

    Public Overrides Function GetSelectedItems() As IList
        Dim ids As New List(Of Guid)

        For Each item As GridDataItem In _grid.Items
            Dim cb As CheckBox = DirectCast(item.FindControl("cbSelect"), CheckBox)
            If Not cb.Checked Then
                Continue For
            End If

            Dim lb As LinkButton = DirectCast(item.FindControl("lbtViewProtocol"), LinkButton)
            If Not lb.CommandName.Eq("ViewProtocol") Then
                Continue For
            End If

            ids.Add(GetProtocolUniqueId(item).Value)
        Next

        Return ids
    End Function

    Private Sub SetupPageWithTaskRunning()
        Dim url As String = "~\Prot\ProtImportProgress.aspx?" & CommonShared.AppendSecurityCheck("Type=Prot&Title=Esportazione Documenti&TaskType=E")
        WindowBuilder.LoadWindow("wndProgress", url, "onTaskCompleted", Unit.Pixel(500), Unit.Pixel(250))
    End Sub

    Protected Overrides Sub Print()
        Dim protocolprint As New ProtocolPrint()
        protocolprint.ListId = CType(GetSelectedItems(), List(Of Guid))
        Session.Add("Printer", protocolprint)
        Response.Redirect("..\Comm\CommPrint.aspx?Type=Prot&PrintName=Protocol")
    End Sub

    Public Overrides Sub Show()
        MyBase.Show()
        ShowDocumentButton()
        ShowExportButton()
    End Sub

    Public Overrides Sub Hide()
        MyBase.Hide()
        DocumentsButton.Visible = False
    End Sub

    Public Sub ShowDocumentButton()
        DocumentsButton.Visible = DocSuiteContext.Current.ProtocolEnv.EnableMultiChainView
    End Sub

    Public Sub ShowExportButton()
        ExportButton.Visible = False
        If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.ExportPath) Then
            ExportButton.Visible = True
        End If
    End Sub

    Public Sub InitializeChangeContainerFunctionality()
        ChangeContainerButton.Visible = True
    End Sub

    Protected Overrides Sub SetReadedSelectedItems()
        Dim selected As Boolean = False
        Dim errorMessage As String = String.Empty

        Dim listId As IList(Of Guid) = CType(GetSelectedItems(), IList(Of Guid))
        Try
            Dim protocol As Protocol
            For Each key As Guid In listId
                protocol = Facade.ProtocolFacade.GetById(key)
                SetReadFunction()(protocol.Year, protocol.Number, LogType, LogDescription)
                selected = True
            Next
        Catch ex As Exception
            errorMessage = "Errore durante la segnature come già letti"
            selected = False
        End Try

        SendSetReadEvent(selected, errorMessage)
    End Sub

#End Region

End Class