Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class ProtRecupera
    Inherits ProtBasePage
    
#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not ProtocolEnv.ProtocolRecoverHandleEnabled Then
            cbShowAll.Checked = False
            cbShowAll.Visible = False
        End If

        chkDisableUnlinkPec.Enabled = ProtocolEnv.PecUnboundMode = 0 OrElse ProtocolEnv.PecUnboundMode = 2
        If ProtocolEnv.PecUnboundMode > 0 Then
            chkDisableUnlinkPec.Checked = True
        End If

        InitializeAjax()

        'Esegue la ricerca
        If Not Page.IsPostBack Then
            txtYear.Text = DateTime.Today.Year.ToString()
            DoSearch()
        End If

    End Sub

    Private Sub BtnCancelClick(sender As Object, e As EventArgs) Handles btnCancel.Click
        Dim selectedProtocols As IList(Of Protocol) = Facade.ProtocolFacade.GetProtocols(GetSelectedProtocolKeys)

        If selectedProtocols Is Nothing Then
            AjaxAlert("E' necessario selezionare almeno un protocollo.")
            Exit Sub
        End If

        Select Case True
            Case String.IsNullOrEmpty(txtAnnulla.Text.Trim)
                AjaxAlert("E' necessario specificare gli estremi di annullamento.")
            Case Not selectedProtocols.Count > 0
                AjaxAlert("E' necessario selezionare almeno un protocollo.")
            Case Else
                For Each p As Protocol In selectedProtocols
                    p.IdStatus = ProtocolStatusId.Annullato
                    p.LastChangedReason = txtAnnulla.Text.Trim
                    Facade.ProtocolFacade.Update(p)

                    If DocSuiteContext.Current.ProtocolEnv.IsLogEnabled Then Facade.ProtocolLogFacade.Insert(p, ProtocolLogEvent.PA, p.LastChangedReason)
                    
                    If (chkDisableUnlinkPec.Checked) Then Facade.ProtocolFacade.PecUnlink(p)
                Next
                DoSearch()
                txtAnnulla.Text = String.Empty
                AjaxAlert("Protocolli annullati correttamente.")
        End Select
    End Sub

    Protected Sub DataSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        If gvProtocols.DataSource IsNot Nothing Then
            Title = String.Format("Protocolli in Errore  - Risultati ({0}/{1})", gvProtocols.DataSource.Count, gvProtocols.VirtualItemCount)
        Else
            Title = "Protocolli in Errore  - Nessun Risultato"
        End If
        MasterDocSuite.Title = Title
        MasterDocSuite.HistoryTitle = Title
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        DoSearch()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, MasterDocSuite.TitleContainer)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, MasterDocSuite.TitleContainer)

        AddHandler uscProtocolGrid.Grid.DataBound, AddressOf DataSourceChanged
    End Sub

    ''' <summary> Ritorna l'elenco delle chiavi selezionate. </summary>
    Private Function GetSelectedProtocolKeys() As IList(Of YearNumberCompositeKey)
        Dim retval As New List(Of YearNumberCompositeKey)
        For Each griditem As GridDataItem In uscProtocolGrid.Grid.Items
            Dim currentCheckBox As CheckBox = DirectCast(griditem.FindControl("cbSelect"), CheckBox)
            If Not currentCheckBox.Checked Then
                Continue For
            End If

            Dim lbtViewProtocol As Label = DirectCast(griditem.FindControl("lblFullProtocolNumber"), Label)
            Dim keyToAdd As YearNumberCompositeKey = New YearNumberCompositeKey
            keyToAdd.Year = CShort(Mid(lbtViewProtocol.Text, 1, InStr(lbtViewProtocol.Text, "/") - 1))
            keyToAdd.Number = CInt(Mid(lbtViewProtocol.Text, InStr(lbtViewProtocol.Text, "/") + 1))
            retval.Add(keyToAdd)

        Next

        Return retval
    End Function

    Private Sub DoSearch()
        Dim gvProtocols As BindGrid = uscProtocolGrid.Grid
        Dim finder As NHibernateProtocolFinder = Facade.ProtocolFacade.GetRecoveringProtocolsFinder()
        If cbShowAll.Checked Then
            finder.RegistrationUser = String.Empty
        End If
        
        Dim year As Short
        If Short.TryParse(txtYear.Text, year) Then
            finder.Year = year
        End If

        'se si sceglie visualizza tutti, filtro solo quelli su cui ho diritto di lettura
        If cbShowAll.Checked Then
            CommonInstance.ApplyProtocolFinderSecurity(finder, SecurityType.Read, True)
        End If

        gvProtocols.PageSize = finder.PageSize
        gvProtocols.MasterTableView.SortExpressions.Clear()

        If DocSuiteContext.Current.ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
            gvProtocols.MasterTableView.SortExpressions.AddSortExpression("RegistrationDate ASC")
            gvProtocols.MasterTableView.AllowMultiColumnSorting = True
        End If

        gvProtocols.Finder = finder
        gvProtocols.DataBindFinder()
        gvProtocols.Visible = True
        gvProtocols.MasterTableView.AllowMultiColumnSorting = False
    End Sub

#End Region

End Class