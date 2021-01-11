
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Telerik.Web.UI

Partial Class UtltProtSospendi
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "
    Private Sub Page_InitComplete(ByVal sender As Object, ByVal e As EventArgs) Handles Me.InitComplete
        ChkVerificaEnabled = False
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSttings()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim protocolSuspended As IList(Of String) = New List(Of String)
        Try
            If Not txtSuspendNumber.Value.HasValue Then
                AjaxAlert("Nessun valore definito nel campo 'Numero Protocolli'")
                Return
            End If

            If Not rdpSuspendDate.SelectedDate.HasValue Then
                AjaxAlert("Nessuna data selezionata per la procedura di sospensione")
                Return
            End If

            Dim selectedYear As Short? = GetSelectedYear()
            If Not selectedYear.HasValue Then
                AjaxAlert("Selezionare un anno di riferimento per la procedura di sospensione")
                Return
            End If

            Dim currentParameter As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
            Dim maxDate As DateTime = If(selectedYear = currentParameter.LastUsedYear, DateTime.Now, New DateTime(selectedYear.Value, 12, 31))
            If rdpSuspendDate.SelectedDate.Value > maxDate Then
                AjaxAlert("La data di sospensione deve essere minore o uguale alla data massima dell'anno selezionato.")
                Return
            End If

            Dim lastProtocol As Protocol = Facade.ProtocolFacade.GetLastProtocolByYear(selectedYear.Value)
            If lastProtocol IsNot Nothing AndAlso rdpSuspendDate.SelectedDate.Value < lastProtocol.RegistrationDate.Date Then
                AjaxAlert("La data di sospensione deve essere maggiore o uguale alla data di registrazione dell'ultimo Protocollo.")
                Return
            End If

            Dim numberToSuspend As Integer = Convert.ToInt32(txtSuspendNumber.Value.Value)
            protocolSuspended = Facade.ProtocolFacade.Suspend(numberToSuspend, rdpSuspendDate.SelectedDate.Value, selectedYear)
            rptSospesiResults.Visible = protocolSuspended.Count > 0
            rptSospesiResults.DataSource = protocolSuspended
            rptSospesiResults.DataBind()
            ResetForm()
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Concat("[UtltProtSospendi] Si è verificato un errore: ", ex.Message), ex)
            AjaxAlert(String.Concat("si è verificato un errore: ", ex.Message))
        End Try
    End Sub

    Private Sub txtSuspendNumber_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSuspendNumber.TextChanged
        If String.IsNullOrEmpty(txtSuspendNumber.Text) Then
            txtSuspendToNumber.Text = String.Empty
            Return
        End If

        Dim lastNumber As Integer = 0
        If Not String.IsNullOrEmpty(txtYearNumber.Text) Then
            lastNumber = Integer.Parse(txtYearNumber.Text.Split("/"c)(1))
        End If
        txtSuspendToNumber.Text = String.Format("{0:0000000}", lastNumber + Integer.Parse(txtSuspendNumber.Text))
    End Sub

    Protected Sub rcbSelectSuspendYear_SelectedIndexChanged(sender As Object, e As RadComboBoxSelectedIndexChangedEventArgs)
        ResetForm()
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjaxSttings()
        AjaxManager.AjaxSettings.AddAjaxSetting(txtSuspendNumber, txtSuspendToNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, rptSospesiResults)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, txtYearNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, txtRegistrationDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, rdpSuspendDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, txtSuspendNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, txtSuspendToNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, txtCurrentDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbSelectSuspendYear, txtYearNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbSelectSuspendYear, txtRegistrationDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbSelectSuspendYear, rdpSuspendDate)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbSelectSuspendYear, txtSuspendNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbSelectSuspendYear, txtSuspendToNumber)
        AjaxManager.AjaxSettings.AddAjaxSetting(rcbSelectSuspendYear, txtCurrentDate)
    End Sub

    Private Sub Initialize()
        rptSospesiResults.Visible = False
        txtSuspendNumber.Text = String.Empty
        rdpSuspendDate.SelectedDate = Nothing
        txtSuspendToNumber.Text = String.Empty

        Title = "Sospensione numeri di Protocollo"

        txtCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy")
        Dim currentParameter As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
        Dim currentYear As Short = currentParameter.LastUsedYear
        Dim previousYear As Integer = currentYear - 1
        rcbSelectSuspendYear.Items.Add(New RadComboBoxItem(currentYear.ToString(), currentYear.ToString()))
        rcbSelectSuspendYear.Items.Add(New RadComboBoxItem(previousYear.ToString(), previousYear.ToString()))
        rcbSelectSuspendYear.SelectedValue = currentYear.ToString()

        Dim lastProtocol As Protocol = Facade.ProtocolFacade.GetLastProtocolByYear(currentYear)
        If lastProtocol IsNot Nothing Then
            txtYearNumber.Text = lastProtocol.FullNumber
            txtRegistrationDate.Text = lastProtocol.RegistrationDate.ToLocalTime().DefaultString()
            rdpSuspendDate.SelectedDate = lastProtocol.RegistrationDate.ToLocalTime().DateTime
        End If
    End Sub

    Private Sub ResetForm()
        txtYearNumber.Text = String.Empty
        txtRegistrationDate.Text = String.Empty
        rdpSuspendDate.SelectedDate = Nothing
        txtSuspendNumber.Text = String.Empty
        txtSuspendToNumber.Text = String.Empty

        Dim selectedYear As Short? = GetSelectedYear()
        If Not selectedYear.HasValue Then
            Return
        End If

        Dim lastProtocol As Protocol = Facade.ProtocolFacade.GetLastProtocolByYear(selectedYear.Value)
        Dim currentParameter As Parameter = Facade.ParameterFacade.GetCurrentAndRefresh()
        txtCurrentDate.Text = If(selectedYear = currentParameter.LastUsedYear, DateTime.Now.ToString("dd/MM/yyyy"), New DateTime(selectedYear.Value, 12, 31).ToString("dd/MM/yyyy"))
        If lastProtocol IsNot Nothing Then
            txtYearNumber.Text = lastProtocol.FullNumber
            txtRegistrationDate.Text = lastProtocol.RegistrationDate.ToLocalTime().DefaultString()
            rdpSuspendDate.SelectedDate = lastProtocol.RegistrationDate.ToLocalTime().DateTime
        End If
        Call txtSuspendNumber_TextChanged(Me, New EventArgs())
    End Sub

    Private Function GetSelectedYear() As Short?
        If String.IsNullOrEmpty(rcbSelectSuspendYear.SelectedValue) Then
            Return Nothing
        End If
        Return Short.Parse(rcbSelectSuspendYear.SelectedValue)
    End Function
#End Region

End Class
