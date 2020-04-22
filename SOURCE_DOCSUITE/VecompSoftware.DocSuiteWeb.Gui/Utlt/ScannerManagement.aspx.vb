Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ScannerManagement
    Inherits CommonBasePage

#Region " Fields "

    Private _currentConfiguration As ScannerConfiguration
    Private _defaultConfiguration As ScannerConfiguration
    Private _nullScannerConfiguration As ScannerConfiguration
    Private _availableScannerConfiguration As IList(Of ScannerConfiguration)

#End Region

#Region " Properties "

    Private ReadOnly Property CurrentConfiguration As ScannerConfiguration
        Get
            If ddlScannerConfiguration.SelectedValue.Equals(NullScannerConfiguration.Id) Then
                _currentConfiguration = Nothing
            ElseIf _currentConfiguration Is Nothing OrElse _currentConfiguration IsNot Nothing AndAlso Not _currentConfiguration.Id.Equals(ddlScannerConfiguration.SelectedValue) Then
                _currentConfiguration = Facade.ScannerConfigurationFacade.GetById(ddlScannerConfiguration.SelectedValue)
            End If
            Return _currentConfiguration
        End Get
    End Property

    Private ReadOnly Property DefaultConfiguration As ScannerConfiguration
        Get
            If _defaultConfiguration Is Nothing Then
                _defaultConfiguration = Facade.ScannerConfigurationFacade.GetDefaultConfiguration()
            End If
            Return _defaultConfiguration
        End Get
    End Property

    Private ReadOnly Property NullScannerConfiguration As ScannerConfiguration
        Get
            If _nullScannerConfiguration Is Nothing Then
                _nullScannerConfiguration = New ScannerConfiguration
                _nullScannerConfiguration.Id = -1
                _nullScannerConfiguration.Description = "Selezionare una configurazione"
            End If
            Return _nullScannerConfiguration
        End Get
    End Property

    Private ReadOnly Property AvailableScannerConfiguration As IList(Of ScannerConfiguration)
        Get
            If _availableScannerConfiguration Is Nothing Then
                _availableScannerConfiguration = New List(Of ScannerConfiguration)
                _availableScannerConfiguration = Facade.ScannerConfigurationFacade.GetAllOrdered("Description ASC")
                _availableScannerConfiguration.Insert(0, NullScannerConfiguration)
            End If
            Return _availableScannerConfiguration
        End Get
    End Property

    Private ReadOnly Property SelectedScannerParameter As IList(Of ScannerParameter)
        Get
            Dim selected As New List(Of ScannerParameter)
            Dim sp As ScannerParameter
            For Each r As GridDataItem In rgScannerParameter.Items
                If DirectCast(r.FindControl("chkSelect"), CheckBox).Checked Then
                    sp = New ScannerParameter
                    sp = Facade.ScannerParameterFacade.GetById(DirectCast(r.FindControl("lblId"), Label).Text)
                    selected.Add(sp)
                End If
            Next
            Return selected
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            If (DocSuiteContext.Current.ProtocolEnv.ScannerConfigurationEnabled) Then
                Initialize()
            End If
        End If
    End Sub

    Private Sub cmdFilterScannerParameter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdFilterScannerParameter.Click
        BindScannerParameter()
    End Sub

    Private Sub ddlScannerConfiguration_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlScannerConfiguration.SelectedIndexChanged
        BindScannerParameter()
        RefreshForm()
    End Sub

    Private Sub cmdRenameScannerConfiguration_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRenameScannerConfiguration.Click
        If CurrentConfiguration IsNot Nothing Then
            Dim previousId As Integer = CurrentConfiguration.Id
            RenameScannerConfiguration()
            BindDdlConfiguration()
            ddlScannerConfiguration.SelectedValue = previousId
            RefreshForm()
        End If
    End Sub

    Private Sub rgScannerParameter_ItemCreated(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles rgScannerParameter.ItemCreated
        If TypeOf e.Item Is GridDataItem Then
            Dim chkSelect As CheckBox = DirectCast(e.Item.FindControl("chkSelect"), CheckBox)
            Dim txtName, txtValue, txtDescription As RadTextBox
            txtName = e.Item.FindControl("txtName")
            txtValue = e.Item.FindControl("txtValue")
            txtDescription = e.Item.FindControl("txtDescription")
            Dim jsConfirm As String = String.Format("ConfirmOnEnter('{0}');", cmdConfirmScannerParameter.ClientID)
            Dim jsArrow As String = "CycleOnArrow('ctl00_cphContent_rgScannerParameter_ctl00_ctl{2}_{0}', 'ctl00_cphContent_rgScannerParameter_ctl00_ctl{1}_{0}');"

            Dim currentRowId As Integer = Integer.Parse(txtName.ClientID.Split("_"c).GetValue(4).ToString.Replace("ctl", ""))
            Dim nextRowId As String = (currentRowId - 2).ToString.PadLeft(2, "0"c)
            Dim prevRowId As String = (currentRowId + 2).ToString.PadLeft(2, "0"c)

            txtName.Attributes.Add("onkeypress", jsConfirm)
            txtValue.Attributes.Add("onkeypress", jsConfirm)
            txtDescription.Attributes.Add("onkeypress", jsConfirm)

            chkSelect.Attributes.Add("onkeydown", String.Format(jsArrow, chkSelect.ID, nextRowId, prevRowId))
            txtName.Attributes.Add("onkeydown", String.Format(jsArrow, txtName.ID, nextRowId, prevRowId))
            txtValue.Attributes.Add("onkeydown", String.Format(jsArrow, txtValue.ID, nextRowId, prevRowId))
            txtDescription.Attributes.Add("onkeydown", String.Format(jsArrow, txtDescription.ID, nextRowId, prevRowId))
        End If
    End Sub


    Private Sub cmdConfirmScannerParameter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdConfirmScannerParameter.Click
        If CurrentConfiguration IsNot Nothing Then
            SaveScannerParameter()
            BindScannerParameter()
        End If
    End Sub

    Private Sub cmdAddScannerConfiguration_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddScannerConfiguration.Click
        SaveScannerParameter()
        AddScannerConfiguration()
        BindDdlConfiguration()
        ddlScannerConfiguration.SelectedValue = NullScannerConfiguration.Id
        RefreshForm()
        BindScannerParameter()
    End Sub

    Private Sub cmdAddScannerParameter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddScannerParameter.Click
        If CurrentConfiguration IsNot Nothing Then
            SaveScannerParameter()
            AddScannerParameter()
            BindScannerParameter()
        End If
    End Sub

    Private Sub cmdDeleteScannerConfiguration_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDeleteScannerConfiguration.Click
        If CurrentConfiguration IsNot Nothing AndAlso CurrentConfiguration.ComputerLogs.Count.Equals(0) Then
            DeleteScannerConfiguration()
            BindDdlConfiguration()
            ddlScannerConfiguration.SelectedValue = NullScannerConfiguration.Id
            RefreshForm()
            BindScannerParameter()
        Else : AjaxAlert("La configurazione selezionata è correntemente in uso.")
        End If
    End Sub

    Private Sub cmdDuplicateScannerConfiguration_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDuplicateScannerConfiguration.Click
        If CurrentConfiguration IsNot Nothing Then
            DuplicateScannerConfiguration()
            BindDdlConfiguration()
            ddlScannerConfiguration.SelectedValue = NullScannerConfiguration.Id
            RefreshForm()
            BindScannerParameter()
        End If
    End Sub

    Private Sub cmdDefaultScannerConfiguration_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDefaultScannerConfiguration.Click
        DefaultScannerConfiguration()
        Dim previousId As Integer = CurrentConfiguration.Id
        BindDdlConfiguration()
        ddlScannerConfiguration.SelectedValue = previousId
        RefreshForm()
    End Sub

    Private Sub cmdDeleteScannerParameter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDeleteScannerParameter.Click
        DeleteScannerParameter()
        BindScannerParameter()
    End Sub

#End Region

#Region " Methods "

    Private Sub RefreshForm()
        txtScannerConfigurationName.Enabled = CurrentConfiguration IsNot Nothing
        cmdAddScannerParameter.Enabled = CurrentConfiguration IsNot Nothing
        cmdDeleteScannerParameter.Enabled = CurrentConfiguration IsNot Nothing
        cmdRenameScannerConfiguration.Enabled = CurrentConfiguration IsNot Nothing
        cmdDuplicateScannerConfiguration.Enabled = CurrentConfiguration IsNot Nothing
        cmdDeleteScannerConfiguration.Enabled = CurrentConfiguration IsNot Nothing
        cmdDefaultScannerConfiguration.Enabled = CurrentConfiguration IsNot Nothing AndAlso Not CurrentConfiguration.IsDefault

        If CurrentConfiguration IsNot Nothing Then
            txtScannerConfigurationName.Text = CurrentConfiguration.Description
        Else
            txtScannerConfigurationName.Text = ""
        End If
    End Sub

    Private Sub AjaxTriggeredControls(ByVal p_control As Control)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, ddlScannerConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, txtScannerConfigurationName)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, cmdRenameScannerConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, cmdAddScannerParameter)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, cmdDeleteScannerParameter)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, cmdDuplicateScannerConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, cmdDeleteScannerConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, cmdDefaultScannerConfiguration)
        AjaxManager.AjaxSettings.AddAjaxSetting(p_control, rgScannerParameter, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub


    Private Sub InitializeAjax()
        AjaxTriggeredControls(cmdRenameScannerConfiguration)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdFilterScannerParameter, rgScannerParameter, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxTriggeredControls(ddlScannerConfiguration)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdConfirmScannerParameter, rgScannerParameter, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxTriggeredControls(cmdAddScannerConfiguration)

        AjaxManager.AjaxSettings.AddAjaxSetting(cmdAddScannerParameter, rgScannerParameter, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDeleteScannerParameter, rgScannerParameter, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxTriggeredControls(cmdDuplicateScannerConfiguration)

        AjaxTriggeredControls(cmdDeleteScannerConfiguration)

        AjaxTriggeredControls(cmdDefaultScannerConfiguration)
    End Sub

    Private Sub Initialize()
        If Not DocSuiteContext.Current.ProtocolEnv.ScannerConfigurationEnabled Then
            Throw New DocSuiteException("Impossibile visualizzare la pagina") With {.Descrizione = "Configurazione scanner non abilitata, controllare parametro ScannerConfigurationEnabled e parametri ComputerLog"}
        End If

        BindDdlConfiguration()
        ddlScannerConfiguration.SelectedValue = NullScannerConfiguration.Id
        txtScannerConfigurationName.Attributes.Add("onkeypress", String.Format("ConfirmOnEnter('{0}');", cmdRenameScannerConfiguration.ClientID))
        RefreshForm()
        BindScannerParameter()
    End Sub

    Private Sub BindScannerParameter()
        Dim finder As New NHibernateScannerParameterFinder
        If Not String.IsNullOrEmpty(txtFilterName.Text) Then
            finder.Name = txtFilterName.Text
        End If
        If Not String.IsNullOrEmpty(txtFilterValue.Text) Then
            finder.Value = txtFilterValue.Text
        End If
        If Not String.IsNullOrEmpty(txtFilterDescription.Text) Then
            finder.Description = txtFilterDescription.Text
        End If
        If Not String.IsNullOrEmpty(ddlScannerConfiguration.SelectedValue) Then
            finder.ScannerConfigurationId = ddlScannerConfiguration.SelectedValue
        End If

        rgScannerParameter.DataSource = finder.DoSearch
        rgScannerParameter.DataBind()
    End Sub

    Private Sub BindDdlConfiguration()
        ddlScannerConfiguration.DataSource = AvailableScannerConfiguration
        ddlScannerConfiguration.DataTextField = "Description"
        ddlScannerConfiguration.DataValueField = "Id"
        ddlScannerConfiguration.DataBind()

        If DefaultConfiguration Is Nothing Then
            Exit Sub
        End If

        For Each item As ListItem In ddlScannerConfiguration.Items
            If item.Value.Eq(DefaultConfiguration.Id.ToString()) Then
                item.Text = "* " & item.Text
                Exit For
            End If
        Next
    End Sub

    Private Sub SaveScannerParameter()
        Dim sp As ScannerParameter
        For Each r As GridDataItem In rgScannerParameter.Items
            sp = New ScannerParameter
            sp.Id = DirectCast(r.FindControl("lblId"), Label).Text
            sp.Name = DirectCast(r.FindControl("txtName"), RadTextBox).Text
            sp.Value = DirectCast(r.FindControl("txtValue"), RadTextBox).Text
            sp.Description = DirectCast(r.FindControl("txtDescription"), RadTextBox).Text
            sp.ScannerConfiguration = CurrentConfiguration

            Facade.ScannerParameterFacade.Update(sp)
        Next
    End Sub

    Private Sub AddScannerParameter()
        Dim sp As New ScannerParameter
        sp.ScannerConfiguration = CurrentConfiguration

        Facade.ScannerParameterFacade.Save(sp)
    End Sub

    Private Sub DeleteScannerParameter()
        For Each sp As ScannerParameter In SelectedScannerParameter
            Facade.ScannerParameterFacade.Delete(sp)
        Next
    End Sub

    Private Sub RenameScannerConfiguration()
        If CurrentConfiguration IsNot Nothing Then
            CurrentConfiguration.Description = txtScannerConfigurationName.Text
            Facade.ScannerConfigurationFacade.Update(CurrentConfiguration)
        End If
    End Sub

    Private Sub AddScannerConfiguration()
        Dim sc As New ScannerConfiguration
        sc.Description = "Nuova Configurazione"

        Facade.ScannerConfigurationFacade.Save(sc)
        _availableScannerConfiguration = Nothing
    End Sub

    Private Sub DeleteScannerConfiguration()
        If CurrentConfiguration IsNot Nothing AndAlso CurrentConfiguration.ComputerLogs.Count.Equals(0) Then
            Facade.ScannerConfigurationFacade.Delete(CurrentConfiguration)
            _availableScannerConfiguration = Nothing
        End If
    End Sub

    Private Sub DuplicateScannerConfiguration()
        Dim sc As ScannerConfiguration = CurrentConfiguration.Clone
        sc.Id = Nothing
        sc.ComputerLogs = New List(Of ComputerLog)
        sc.ScannerParameters = New List(Of ScannerParameter)
        sc.Description = String.Format("Copia di ({0})", CurrentConfiguration.Description)
        sc.IsDefault = False

        Dim spToAdd As ScannerParameter
        For Each sp As ScannerParameter In CurrentConfiguration.ScannerParameters
            spToAdd = sp.Clone
            spToAdd.Id = Nothing
            spToAdd.ScannerConfiguration = sc

            sc.ScannerParameters.Add(spToAdd)
        Next

        Facade.ScannerConfigurationFacade.Save(sc)
        _availableScannerConfiguration = Nothing
    End Sub

    Private Sub DefaultScannerConfiguration()
        Facade.ScannerConfigurationFacade.SetDefaultConfiguration(CurrentConfiguration)
        _defaultConfiguration = Nothing
    End Sub

#End Region

End Class