Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

Partial Class ProtModificaOggetti
    Inherits ProtBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If DocSuiteContext.Current.ProtocolEnv.EnvChangeObject Is Nothing Then
            Throw New DocSuiteException("Modifica oggetti", String.Format("Parametri non configurati. {0}", ProtocolEnv.DefaultErrorMessage))
        End If
        BDViewer.Attributes().Item("src") = "about:blank"
        uscObjectModifier.Disable()

        InitializeAjax()
        InitializeGrid()
    End Sub

    Protected Sub grid_OnItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs)
        Select Case e.CommandName
            Case "ShowProt"
                Dim arguments As String() = Split(e.CommandArgument, "|", 2)
                OpenDocument(arguments(0), arguments(1))
                uscProtGrid.Grid.SelectedIndexes.Clear()
                uscProtGrid.Grid.SelectedIndexes.Add(e.Item.ItemIndex)
        End Select
    End Sub

    Private Sub uscObjectFinder_DoSearch(ByVal sender As Object, ByVal e As ProtocolObjectFinderEventArgs) Handles uscObjectFinder.DoSearch
        Dim gvProtocols As BindGrid = uscProtGrid.Grid
        Dim protocols As IList(Of ProtocolHeader)

        uscObjectModifier.Clear()
        ViewState.Item("ProtYear") = String.Empty
        ViewState.Item("ProtNumber") = String.Empty
        ViewState.Item("ProtObject") = String.Empty
        uscObjectModifier.Disable()

        gvProtocols.PageSize = DocSuiteContext.Current.ProtocolEnv.EnvChangeObject.MaxRecords
        gvProtocols.Finder = e.Finder
        gvProtocols.DataBindFinder()
        protocols = CType(gvProtocols.DataSource, IList(Of ProtocolHeader))
        lblCounter.Text = "(" & protocols.Count & "/" & gvProtocols.VirtualItemCount & ")"
        If gvProtocols.VirtualItemCount > 0 Then
            gvProtocols.SelectedIndexes.Add(0)
            OpenDocument(protocols(0).Year, protocols(0).Number)
        Else
            uscObjectModifier.Clear()
            uscObjectModifier.Disable()
        End If
    End Sub

    Private Sub uscObjectModifier_DoConfirm(ByVal sender As Object, ByVal e As EventArgs) Handles uscObjectModifier.DoConfirm
        Dim objText As String = uscObjectModifier.ObjectControl.Text
        If Not String.IsNullOrEmpty(objText) Then
            If objText = ViewState.Item("ProtObject") Then
                AjaxAlert("Il campo oggetto non è stato modificato.")
            Else
                Try
                    Facade.ProtocolFacade.UpdateProtocolObject(ViewState.Item("ProtYear"), ViewState.Item("ProtNumber"), ViewState.Item("ProtObject"), uscObjectModifier.ObjectControl.Text)
                Catch ex As Exception
                    FileLogger.Warn(LoggerName, "Impossibile eseguire il salvataggio dei dati: " & ex.Message, ex)
                    AjaxAlert("Impossibile eseguire il salvataggio dei dati.")
                End Try
            End If
        Else
            AjaxAlert("Occorre inserire un oggetto per il protocollo.")
        End If
        uscObjectFinder_DoSearch(Me, New ProtocolObjectFinderEventArgs(uscObjectFinder.Finder))
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder.SearchButtonControl, uscProtGrid.Grid, Me.MasterDocSuite.AjaxLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder.SearchButtonControl, uscObjectModifier.ObjectControl.TextBoxControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder.SearchButtonControl, uscObjectModifier.ConfirmButtonControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder.SearchButtonControl, uscObjectModifier.PanelObjectData)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectFinder.SearchButtonControl, radAjaxPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscObjectModifier.ConfirmButtonControl, uscProtGrid.Grid, Me.MasterDocSuite.AjaxLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtGrid, uscObjectModifier.PanelObjectData)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtGrid, uscObjectModifier.ObjectControl.TextBoxControl)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtGrid, uscObjectModifier.ConfirmButtonControl)
    End Sub

    Private Sub InitializeGrid()
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_CATEGORY_NAME)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_CLIENT_SELECT)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_CONTAINER_NAME)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_VIEW_FASCICLES)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_VIEW_LINKS)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_PROTOCOL_OBJECT)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_PROTOCOL_STATUS)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_PROTOCOL_TYPE)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_PROTOCOL_TYPE_SHORT_DESCRIPTION)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_HAS_READ)
        uscProtGrid.DisableColumn(uscProtGrid.COLUMN_PROTOCOL_CONTACT)
        uscProtGrid.Grid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None
        uscProtGrid.Grid.AllowSorting = False
        uscProtGrid.Grid.GroupingEnabled = False
        uscProtGrid.Grid.AllowFilteringByColumn = False
        uscProtGrid.RemoveItemCommandHandler()
        AddHandler uscProtGrid.Grid.ItemCommand, AddressOf grid_OnItemCommand
    End Sub

    Private Sub OpenDocument(ByVal Year As String, ByVal Number As String)
        Throw New NotImplementedException("Passaggio a ViewerLight non implementato")
        'Dim script As String = String.Empty
        'Dim MailBody As String = String.Empty
        'Dim protocol As Protocol = Nothing

        'protocol = Facade.ProtocolFacade.GetById(Year, Number)
        'If protocol IsNot Nothing Then
        '    uscObjectModifier.DataSource = protocol
        '    uscObjectModifier.DataBind()

        '    uscObjectModifier.Enable()
        '    ViewState.Item("ProtYear") = protocol.Year
        '    ViewState.Item("ProtNumber") = protocol.Number
        '    ViewState.Item("ProtocolObject") = uscObjectModifier.ObjectControl.Text

        '    Dim location As Location = protocol.Location
        '    script = SmartClientFacade.GetBiblosSmartClientScript(location.DocumentServer, location.ProtBiblosDSDB, protocol.IdDocument, protocol.IdAttachments, MailBody)
        '    Facade.ProtocolLogFacade.Insert(protocol, "PD", "")
        '    AjaxManager.ResponseScripts.Add("document.getElementById('" & BDViewer.ClientID & "').src = '" & StringHelper.EncodeJS(CommonInstance.AppBiblosDSPage & CommonUtil.ChkGenera(script)) & "';")
        'End If
    End Sub

#End Region

End Class
