Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.AVCP
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Imports System.Collections.Generic

Namespace Series

    Public Class uscLotto
        Inherits DocSuite2008BaseControl

#Region " Properties "

        Private Property Lot() As pubblicazioneLotto
            Get
                Return DirectCast(ViewState("pubblicazioneLotto"), pubblicazioneLotto)
            End Get
            Set(value As pubblicazioneLotto)
                ViewState("pubblicazioneLotto") = value
            End Set
        End Property
        Private Property CurrentAziendeBando() As ICollection(Of Contact)
            Get
                Return DirectCast(ViewState("aziendeBando"), ICollection(Of Contact))
            End Get
            Set(value As ICollection(Of Contact))
                ViewState("aziendeBando") = value
            End Set
        End Property
        Private Property CurrentAziendePartecipanti() As ICollection(Of Contact)
            Get
                If (Not DirectCast(ViewState("aziendePartecipanti"), ICollection(Of Contact)) Is Nothing) Then
                    Return DirectCast(ViewState("aziendePartecipanti"), ICollection(Of Contact))
                End If
                Return New List(Of Contact)
            End Get
            Set(value As ICollection(Of Contact))
                ViewState("aziendePartecipanti") = value
            End Set
        End Property
        Private Property CurrentAziendeAggiudicatarie() As ICollection(Of Contact)
            Get
                Return If(TryCast(ViewState("aziendeAggiudicatarie"), ICollection(Of Contact)), New List(Of Contact))
            End Get
            Set(value As ICollection(Of Contact))
                ViewState("aziendeAggiudicatarie") = value
            End Set
        End Property

        Public Property DefaultSubject As String

        Public Property DefaultStructName As String
#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            lblSumAziendeAggiudicatarie.Text = CurrentAziendeAggiudicatarie.Count
            lblSumAziendePartecipanti.Text = CurrentAziendePartecipanti.Count
            If Not Page.IsPostBack Then
                For Each item As sceltaContraenteType In [Enum].GetValues(GetType(sceltaContraenteType))
                    choice.Items.Add(New RadComboBoxItem(item.GetXmlName(), item.ToString("D")))
                Next
            End If
        End Sub
        Private Sub uscAziendePartecipanti_CompleteSelection(sender As Object, e As ContactEventArgs) Handles uscAziendePartecipanti.CompleteSelectionAziende
            CurrentAziendePartecipanti = e.ContactTarget
            lblSumAziendePartecipanti.Text = CurrentAziendePartecipanti.Count
            ' Ricreo i partecipanti in Lot
            Dim plp As pubblicazioneLottoPartecipanti = New pubblicazioneLottoPartecipanti()
            Dim partecipanti(CurrentAziendePartecipanti.Count - 1) As singoloType
            For i As Integer = 0 To CurrentAziendePartecipanti.Count - 1
                partecipanti(i) = New singoloType() With {.Item = CurrentAziendePartecipanti(i).FiscalCode, _
                                                         .ragioneSociale = CurrentAziendePartecipanti(i).FullDescription}
            Next
            plp.partecipante = partecipanti
            Lot.partecipanti = plp

            ' Reload aziende
            LoadAziendeAggiudicatarie()
            lblSumAziendePartecipanti.Text = CurrentAziendePartecipanti.Count
            HideWindows(windowsAziendePartecipanti)
        End Sub

        Private Sub uscAziendePartecipanti_NeedFinder(sender As Object, e As ContactEventArgs) Handles uscAziendePartecipanti.NeedFinder

            Dim contattiRubrica As List(Of Contact) = CurrentAziendeBando.Where(Function(f) f.Description.Contains(e.Description)).ToList()
            Dim contattiTarget As List(Of Contact) = uscAziendePartecipanti.GetAziendeTarget()
            contattiRubrica.AddRange(contattiTarget.Where(Function(f) Not contattiRubrica.Any(Function(c) c.Id = f.Id)).ToList())
            uscAziendePartecipanti.ForceLoadingSource(contattiRubrica, contattiTarget)
        End Sub

        Private Sub uscAziendeAggiudicatarie_CompleteSelection(sender As Object, e As ContactEventArgs) Handles uscAziendeAggiudicatarie.CompleteSelectionAziende
            CurrentAziendeAggiudicatarie = e.ContactTarget
            lblSumAziendeAggiudicatarie.Text = CurrentAziendeAggiudicatarie.Count
            ' Ricreo gli aggiudicatari in Lot
            Dim pla As pubblicazioneLottoAggiudicatari = New pubblicazioneLottoAggiudicatari()
            Dim aggiudicatari(CurrentAziendeAggiudicatarie.Count - 1) As singoloType
            For i As Integer = 0 To CurrentAziendeAggiudicatarie.Count - 1
                aggiudicatari(i) = New singoloType() With {.Item = CurrentAziendeAggiudicatarie(i).FiscalCode, _
                                                           .ragioneSociale = CurrentAziendeAggiudicatarie(i).FullDescription}
            Next
            pla.aggiudicatario = aggiudicatari
            Lot.aggiudicatari = pla
            lblSumAziendeAggiudicatarie.Text = CurrentAziendeAggiudicatarie.Count
            HideWindows(windowsAziendeAggiudicatarie)
        End Sub

        Private Sub btnElencoAziendePartecipanti_Click(sender As Object, e As EventArgs) Handles btnElencoAziendePartecipanti.Click
            ShowWindows(windowsAziendePartecipanti)
        End Sub
        Private Sub btnElencoAziendeAggiudicatarie_Click(sender As Object, e As EventArgs) Handles btnElencoAziendeAggiudicatarie.Click
            ShowWindows(windowsAziendeAggiudicatarie)
        End Sub

        Private Sub uscAziendePartecipanti_Transferring(sender As Object, e As ContactEventArgs) Handles uscAziendePartecipanti.Transferring
            ShowWindows(windowsAziendePartecipanti)
        End Sub

        Private Sub uscAziendeAggiudicatarie_Transferring(sender As Object, e As ContactEventArgs) Handles uscAziendeAggiudicatarie.Transferring
            ShowWindows(windowsAziendeAggiudicatarie)
        End Sub
#End Region

#Region " Methods "

        Public Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(btnElencoAziendePartecipanti, windowsAziendePartecipanti)
            AjaxManager.AjaxSettings.AddAjaxSetting(btnElencoAziendeAggiudicatarie, windowsAziendeAggiudicatarie)

            AjaxManager.AjaxSettings.AddAjaxSetting(uscAziendePartecipanti, panelIntestazione)
            AjaxManager.AjaxSettings.AddAjaxSetting(uscAziendePartecipanti, windowsAziendeAggiudicatarie)

            AjaxManager.AjaxSettings.AddAjaxSetting(uscAziendeAggiudicatarie, panelIntestazione)
        End Sub

        Private Sub ShowWindows(window As RadWindow)
            Dim script As String = String.Concat("function f(){$find(""", window.ClientID, """).show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);")
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
        End Sub

        Private Sub HideWindows(window As RadWindow)
            Dim script As String = String.Concat("function f(){$find(""", window.ClientID, """).close(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);")
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, True)
        End Sub

        Public Sub SetLotto(ByVal source As pubblicazioneLotto)
            Lot = source

            cig.Text = Lot.cig

            structFiscalCode.Text = String.Empty
            structName.Text = String.Empty
            If Lot.strutturaProponente IsNot Nothing Then
                structFiscalCode.Text = Lot.strutturaProponente.codiceFiscaleProp
                structName.Text = Lot.strutturaProponente.denominazione
            End If
            If (String.IsNullOrEmpty(structName.Text)) Then
                structName.Text = DefaultStructName
            End If
            subject.Text = Lot.oggetto
            If (String.IsNullOrEmpty(subject.Text)) Then
                subject.Text = DefaultSubject
            End If
            choice.SelectedValue = Lot.sceltaContraente.ToString("D")
            awardAmount.Value = Lot.importoAggiudicazione
            paidAmount.Value = Lot.importoSommeLiquidate

            begin.SelectedDate = Nothing
            [end].SelectedDate = Nothing
            If Lot.tempiCompletamento IsNot Nothing Then
                If Lot.tempiCompletamento.dataInizio <> Nothing Then
                    begin.SelectedDate = Lot.tempiCompletamento.dataInizio
                End If
                If Lot.tempiCompletamento.dataUltimazione <> Nothing Then
                    [end].SelectedDate = Lot.tempiCompletamento.dataUltimazione
                End If
            End If
        End Sub

        Public Function GetLotto() As pubblicazioneLotto
            Lot.cig = cig.Text

            If Lot.strutturaProponente Is Nothing Then
                Lot.strutturaProponente = New pubblicazioneLottoStrutturaProponente()
            End If
            Lot.strutturaProponente.codiceFiscaleProp = structFiscalCode.Text
            Lot.strutturaProponente.denominazione = structName.Text

            Lot.oggetto = subject.Text
            Lot.sceltaContraente = DirectCast(Integer.Parse(choice.SelectedValue), sceltaContraenteType)

            Lot.importoAggiudicazione = 0D
            If awardAmount.Value.HasValue Then
                Lot.importoAggiudicazione = CType(awardAmount.Value.Value, Decimal)
            End If

            Lot.importoSommeLiquidate = 0D
            If paidAmount.Value.HasValue Then
                Lot.importoSommeLiquidate = CType(paidAmount.Value, Decimal)
            End If


            If Not begin.SelectedDate.HasValue AndAlso Not [end].SelectedDate.HasValue Then
                Lot.tempiCompletamento = Nothing
            Else
                If Lot.tempiCompletamento Is Nothing Then
                    Lot.tempiCompletamento = New pubblicazioneLottoTempiCompletamento()
                End If
                Lot.tempiCompletamento.dataInizio = DateTime.Today
                If begin.SelectedDate.HasValue Then
                    Lot.tempiCompletamento.dataInizio = begin.SelectedDate.Value.Date
                End If
                Lot.tempiCompletamento.dataInizioSpecified = True

                Lot.tempiCompletamento.dataUltimazione = DateTime.Today.AddYears(1)
                If [end].SelectedDate.HasValue Then
                    Lot.tempiCompletamento.dataUltimazione = [end].SelectedDate.Value.Date
                End If
                Lot.tempiCompletamento.dataUltimazioneSpecified = True
            End If
            Return Lot
        End Function

        Public Sub SetAziendeBando(ByVal source As IList(Of Contact))
            CurrentAziendeBando = source
            LoadAziende()
        End Sub

        Public Sub LoadAziende()
            LoadAziendePartecipanti()
            LoadAziendeAggiudicatarie()
        End Sub

        Public Sub LoadAziendePartecipanti()
            CurrentAziendePartecipanti = New List(Of Contact)
            If (Lot.partecipanti IsNot Nothing AndAlso Lot.partecipanti.partecipante IsNot Nothing AndAlso CurrentAziendeBando IsNot Nothing) Then
                For Each partecipante As singoloType In Lot.partecipanti.partecipante
                    Dim partecipanteToAdd As Contact = CurrentAziendeBando.Where(Function(x) x.FiscalCode.Eq(partecipante.Item)).FirstOrDefault()
                    If Not partecipanteToAdd Is Nothing Then
                        CurrentAziendePartecipanti.Add(partecipanteToAdd)
                    End If
                Next
            End If

            uscAziendePartecipanti.ForceLoadingSource(CurrentAziendeBando, CurrentAziendePartecipanti)
            lblSumAziendePartecipanti.Text = CurrentAziendePartecipanti.Count.ToString()
        End Sub

        Public Sub LoadAziendeAggiudicatarie()
            CurrentAziendeAggiudicatarie = New List(Of Contact)
            If (Lot.aggiudicatari IsNot Nothing AndAlso Lot.aggiudicatari.aggiudicatario IsNot Nothing AndAlso CurrentAziendeBando IsNot Nothing) Then
                For Each aggiudicatario As singoloType In Lot.aggiudicatari.aggiudicatario
                    Dim aggiudicatarioToAdd As Contact = CurrentAziendeBando.Where(Function(x) x.FiscalCode.Eq(aggiudicatario.Item)).FirstOrDefault()
                    If Not aggiudicatarioToAdd Is Nothing Then
                        CurrentAziendeAggiudicatarie.Add(aggiudicatarioToAdd)
                    End If
                Next
            End If

            uscAziendeAggiudicatarie.ForceLoadingSource(CurrentAziendePartecipanti, CurrentAziendeAggiudicatarie)
            lblSumAziendeAggiudicatarie.Text = "0"
            If (CurrentAziendeAggiudicatarie IsNot Nothing) Then
                lblSumAziendeAggiudicatarie.Text = CurrentAziendeAggiudicatarie.Count.ToString()
            End If
        End Sub

#End Region

    End Class
End Namespace