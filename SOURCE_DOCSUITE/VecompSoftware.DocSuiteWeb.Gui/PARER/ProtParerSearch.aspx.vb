Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtParerSearch
    Inherits CommonBasePage

#Region " Events "
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub cmdSearch_Click(sender As Object, e As EventArgs) Handles cmdSearch.Click
        ' Aggiorno la griglia
        Dim finder As New NHibernateProtocolFinder("ProtDB")

        If (CommonInstance.ApplyProtocolFinderSecurity(finder, SecurityType.Read, True)) Then
            If ProtocolEnv.SearchMaxRecords <> 0 Then
                finder.PageSize = ProtocolEnv.SearchMaxRecords
            End If
        End If

        If Not String.IsNullOrWhiteSpace(txtYear.Text) Then
            finder.Year = Short.Parse(txtYear.Text)
        End If

        ' numero di partenza
        finder.NumberFrom = txtNumberFrom.Text

        ' numero di arrivo
        finder.NumberTo = txtNumberTo.Text

        ' data di partenza
        finder.RegistrationDateFrom = dateParerFrom.SelectedDate

        ' data di arrivo
        finder.RegistrationDateTo = dateParerTo.SelectedDate

        ' status del Parer
        For Each parerStatus As RadComboBoxItem In rcbParerStatus.CheckedItems
            finder.ProtocolParerConservationStatus.Add(CType(parerStatus.Value, Integer))
        Next

        SessionSearchController.SaveSessionFinder(finder, SessionSearchController.SessionFinderType.ProtocolParerFinderType)
        Response.Redirect("../PARER/ProtParerGrid.aspx?Type=Prot")
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        For Each conservationStatus As KeyValuePair(Of ProtocolParerFacade.ProtocolParerConservationStatus, String) In ProtocolParerFacade.ConservationsStatus
            '' Converto prima in Int32 per avere il valore numerico che poi vado a caricare come string nella ddl
            rcbParerStatus.Items.Add(New RadComboBoxItem(conservationStatus.Value, Convert.ToInt32(conservationStatus.Key).ToString()))
        Next

        ''Carico eventuali valori mantenuti in finder
        Dim finder As NHibernateProtocolFinder = CType(SessionSearchController.LoadSessionFinder(SessionSearchController.SessionFinderType.ProtocolParerFinderType), NHibernateProtocolFinder)
        If finder Is Nothing Then
            Exit Sub
        End If

        If finder.Year.HasValue Then
            txtYear.Text = finder.Year.Value.ToString()
        End If

        ''Carico il numero di partenza
        txtNumberFrom.Text = finder.NumberFrom

        ''Carico il numero di arrivo
        txtNumberTo.Text = finder.NumberTo

        ''Carico la data di partenza
        dateParerFrom.SelectedDate = finder.RegistrationDateFrom
        ''Carico la data di arrivo
        dateParerTo.SelectedDate = finder.RegistrationDateTo

        ''Carico la tipologia di status
        For Each resolutionParerConservationStatusItem As Integer In finder.ProtocolParerConservationStatus
            rcbParerStatus.Items.FindItemByValue(resolutionParerConservationStatusItem.ToString).Checked = True
        Next
    End Sub
#End Region

End Class